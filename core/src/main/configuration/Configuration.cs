using System;
#if Serializer_YamlDotNet
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Core.Events;
#elif Serializer_Newtonsoft
using Newtonsoft.Json;
#endif
using System.Collections.Generic;
using System.Linq;
namespace ChatDirector.core
{
    public class Configuration : IConfiguration
    #if Serializer_YamlDotNet
    , IYamlConvertible
    #endif
    {
        bool debug { get; set; }
        // Do not allow the user to specify whether or not they are in testing mode,
        // that should only be done programmatically in the unit tests.
        #if Serializer_YamlDotNet
        [YamlIgnore]
        #elif Serializer_Newtonsoft
        [JsonIgnore]
        #endif
        private bool testing = false;
        internal List<IModule> modules { get; }
        internal Dictionary<Type, ILoadable> daemons = new Dictionary<Type, ILoadable>();
        internal Dictionary<string, Chain> chains = new Dictionary<string, Chain>();
        // This is for storage of generic keys that modules may need.
        // The first key is the module name
        internal Dictionary<string, Dictionary<string, string>> moduleData = new Dictionary<string, Dictionary<string, string>>();
        /**
         * Maintain this list as some things run in separate threads, so with an item
         * you need to be able to get the chain object to start execution. Look for a
         * better solution.
         */
        static Dictionary<IItem, Chain> items = new Dictionary<IItem, Chain>();
        private IEnumerable<Type> getModuleTypes()
        {
            // https://stackoverflow.com/questions/720157/finding-all-classes-with-a-particular-attribute
            // This will take awhile to run especially if many classes are already loaded.
            return from a in AppDomain.CurrentDomain.GetAssemblies()
                   from t in a.GetTypes()
                   where typeof(IModule).IsAssignableFrom(t)
                    && t != typeof(IModule)
                   select t;
        }
        private List<IModule> createModules(IEnumerable<Type> moduleTypes)
        {
            var modules = new List<IModule>();
            foreach (var moduleType in moduleTypes)
            {
                modules.Add((IModule)Activator.CreateInstance(moduleType));
            }
            return modules;
        }
        public Configuration()
        {
            modules = createModules(getModuleTypes());
        }
        public void addModule(IModule module)
        {
            // No need to add a duplicate module.
            if(!this.modules.Contains(module)) {
                this.modules.Add(module);
            }
        }
        // https://stackoverflow.com/questions/58102069/how-to-do-a-partial-deserialization-with-jackson#58102226
        public bool load()
        {
            if (debug)
            {
                Console.WriteLine("Modules");
                foreach (IModule module in modules)
                {
                    Console.WriteLine("\t" + module);
                }
                Console.WriteLine();
                Console.WriteLine("Module Data");
                foreach (var moduleKey in moduleData.Keys)
                {
                    Console.WriteLine("\t" + moduleKey + ": ");
                    foreach (var itemKey in moduleData[moduleKey].Keys)
                    {
                        Console.WriteLine("\t\t" + itemKey + ": " + moduleData[moduleKey][itemKey]);
                    }
                }
                Console.WriteLine();
                Console.WriteLine("Daemons");
                foreach (ILoadable daemon in daemons.Values)
                {
                    Console.WriteLine("\t" + daemon);
                }
                Console.WriteLine();
                Console.WriteLine("Chains");
                foreach (string pipeKey in chains.Keys)
                {
                    Console.WriteLine("\t" + "Chain " + pipeKey);
                    if (chains[pipeKey] != null)
                    {
                        foreach (IItem item in chains[pipeKey].getItems())
                        {
                            Console.WriteLine("\t\t" + item);
                        }
                    }
                }
            }
            if (debug)
            {
                Console.WriteLine();
                Console.WriteLine("Loading Modules...");
            }
            foreach (IModule module in modules)
            {
                if (debug)
                {
                    Console.WriteLine("\t" + module);
                }
                if (!module.load())
                {
                    Console.WriteLine("Module " + module.GetType() + " " + module + " failed to load.");
                    return false;
                }
            }
            if (debug)
            {
                Console.WriteLine();
                Console.WriteLine("Checking Validity...");
            }
            if (!isValid())
            {
                return false;
            }
            if (debug)
            {
                Console.WriteLine();
                Console.WriteLine("Loading Daemons...");
            }
            foreach (ILoadable daemon in daemons.Values)
            {
                Console.WriteLine("\t" + daemon);
                if (!daemon.load())
                {
                    Console.WriteLine("daemon " + daemon.GetType() + " " + daemon + " failed to load.");
                    return false;
                }
                // Load all items into their respecting daemons
                if (typeof(IDaemon).IsAssignableFrom(daemon.GetType())) {
                    foreach (var chain in this.chains)
                    {
                        foreach (var item in chain.Value.getItems())
                        {
                            if (item.GetType() == ((IDaemon)daemon).getItemType()) {
                                ((IDaemon)daemon).addItem(item);
                            }
                        }
                    }
                }
            }
            return true;
        }
        public Type getItemClass(string itemType, IEnumerable<IModule> inputModules)
        {
            foreach (IModule module in inputModules)
            {
                if (module.getItemNames().Contains(itemType))
                {
                    return module.getItemClass(itemType);
                }
            }
            return null;
        }
        public bool unload()
        {
            foreach (ILoadable daemon in daemons.Values)
            {
                daemon.unload();
            }
            foreach (IModule module in modules)
            {
                module.unload();
            }
            return true;
        }
        public Type getItemClass(string itemType)
        {
            return getItemClass(itemType, modules);
        }
        public IModule getModule(Type class1)
        {
            foreach (IModule module in modules)
            {
                if (module.GetType() == class1.GetType())
                {
                    return module;
                }
            }
            return null;
        }
        public bool hasDaemon(Type class1)
        {
            return daemons.ContainsKey(class1);
        }
        public ILoadable getOrCreateDaemon(Type class1)
        {
            if (daemons.ContainsKey(class1))
            {
                return daemons[class1];
            }
            else
            {
                ILoadable daemon = (ILoadable)Activator.CreateInstance(class1);
                daemons.Add(class1, daemon);
                return daemon;
            }
        }
        public bool isValid()
        {
            foreach (var chainKey in chains.Keys)
            {
                if (chains[chainKey] != null && !chains[chainKey].isValid())
                {
                    Console.WriteLine("chain: " + chains[chainKey].ToString() + " is not valid.");
                    return false;
                }
            }
            foreach (IModule module in modules)
            {
                if (!module.isValid())
                {
                    Console.WriteLine("module " + module.ToString() + " is not valid.");
                    return false;
                }
            }
            if (!ChatDirector.hasChains())
            {
                return false;
            }
            return true;
        }
        public Chain getChainForItem(IItem item)
        {
            if (items.ContainsKey(item))
            {
                return items[item];
            }
            return null;
        }
        public void putChainForItem(IItem item, Chain chain)
        {
            items.Add(item, chain);
        }
        public Dictionary<string, Chain> getChains()
        {
            return this.chains;
        }
        public bool isDebug()
        {
            return this.debug;
        }
        public bool isTesting()
        {
            return false;
        }
        public List<IModule> getModules()
        {
            return this.modules;
        }
        public Dictionary<string, Dictionary<string, string>> getModuleData()
        {
            return this.moduleData;
        }
        public IEnumerable<ILoadable> getDaemons()
        {
            return daemons.Values;
        }
#if Serializer_YamlDotNet
        public void Read(IParser parser, Type expectedType, ObjectDeserializer nestedObjectDeserializer)
        {
            ChatDirector.setConfigStaging(this);
            parser.Consume<MappingStart>();
            var moreChains = true;
            while (moreChains)
            {
                if (parser.Current.GetType() != typeof(Scalar))
                {
                    break;
                }
                switch (parser.Consume<Scalar>().Value)
                {
                    case "chains":
                        parser.Consume<MappingStart>();
                        var chainKey = parser.Consume<Scalar>().Value;
                        var chain = (Chain)nestedObjectDeserializer(typeof(Chain));
                        this.chains.Add(chainKey, chain);
                        parser.Consume<MappingEnd>();
                        break;
                    case "debug":
                        this.debug = Boolean.Parse(parser.Consume<Scalar>().Value);
                        break;
                    case "module_data":
                        parser.Consume<MappingStart>();
                        var test2 = parser.Consume<Scalar>();
                        parser.Consume<MappingEnd>();
                        throw new NotImplementedException();
                        break;
                    default:
                        moreChains = false;
                        break;
                }
            }
            parser.Consume<MappingEnd>();
        }
        public void Write(IEmitter emitter, ObjectSerializer nestedObjectSerializer)
        {
            throw new NotImplementedException();
        }
#elif Serializer_Newtonsoft
    public class Converter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.Equals(typeof(Configuration));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var output = new Configuration();
            ChatDirector.setConfigStaging(output);
            reader.Read(); // Should be JsonToken.StartObject
            var moreChains = true;
            while (moreChains)
            {
                if (reader.TokenType != JsonToken.PropertyName) // Was scaler
                {
                    break;
                }
                switch (reader.Value)
                {
                    case "chains":
                        reader.Read(); // Should be ObjectStart
                        reader.Read(); //Should be String
                        var chainKey = (string)reader.Value;
                        reader.Read();// Start the chain
                        var chain = serializer.Deserialize<Chain>(reader);
                        output.chains.Add(chainKey, chain);
                        reader.Read(); //Should be ObjectEnd
                        break;
                    case "debug":
                        reader.Read(); //Should be string
                        if (reader.TokenType == JsonToken.Boolean)
                        {
                            output.debug = (bool)reader.Value;
                        }
                        else
                        {
                            output.debug = Boolean.Parse((string)reader.Value);
                        }
                        reader.Read(); //Advance one object
                        break;
                    case "module_data":
                        reader.Read(); //Should be ObjectStart
                        reader.Read(); //Should be string
                        var test2 = serializer.Deserialize<Dictionary<String, Dictionary<String, String>>>(reader);
                        output.moduleData = test2;
                        reader.Read(); //Should be object end
                        throw new NotImplementedException();
                        break;
                    default:
                        moreChains = false;
                        break;
                }
            }
            reader.Read(); //Should be ObjectEnd
            return output;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
#endif
    }
}