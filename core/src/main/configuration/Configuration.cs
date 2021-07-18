using System;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.Core.Events;

namespace ChatDirector.core
{
    public class Configuration : IConfiguration, IYamlConvertible
    {
        bool debug { get; set; }
        // Do not allow the user to specify whether or not they are in testing mode,
        // that should only be done programmatically in the unit tests.
        [YamlIgnore]
        private bool testing = false;
        List<IModule> modules { get; }
        Dictionary<Type, ILoadable> daemons = new Dictionary<Type, ILoadable>();
        Dictionary<string, Chain> chains = new Dictionary<string, Chain>();
        // This is for storage of generic keys that modules may need.
        // The first key is the module name
        Dictionary<string, Dictionary<string, string>> moduleData = new Dictionary<string, Dictionary<string, string>>();
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
        public IModule getModule(IModule class1)
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

        public IEnumerable<ILoadable> getDaemons()
        {
            return daemons.Values;
        }

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
    }
}