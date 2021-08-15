using System;
using System.Collections.Generic;
#if Serializer_YamlDotNet
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Core.Events;
#elif Serializer_Newtonsoft
using Newtonsoft.Json;
#endif
namespace ChatDirector.core
{
    public class Chain : IValid
#if Serializer_YamlDotNet
    , IYamlConvertible
#endif
    {
        internal List<IItem> items = new List<IItem>();
#if Serializer_YamlDotNet
        [YamlIgnore]
#elif Serializer_Newtonsoft
        [JsonIgnore]
#endif
        bool invalidItem;
        public Chain() { }
        public Chain(IEnumerable<IItem> items)
        {
            foreach (IItem item in items)
            {
                this.items.Add(item);
            }
        }
        /**
         * This Starts execution at an item on a new thread.
         * 
         * @param item
         * @param context
         */
        public void runAsync(IItem item, Context context)
        {
            ChainWorker chainWorker = new ChainWorker(this, item, context);
            chainWorker.run();
        }
        /**
         * @param item
         * @return Whether or not items contains item
         */
        public bool contains(IItem item)
        {
            return items.Contains(item);
        }
        public bool isValid()
        {
            foreach (IItem item in items)
            {
                if (!item.isValid())
                {
                    Console.WriteLine(item + " is not valid.");
                    return false;
                }
            }
            if (invalidItem)
            {
                Console.WriteLine("failed to read all items.");
                return false;
            }
            else
            {
                return true;
            }
        }
        public void addItem(IItem item)
        {
            items.Add(item);
            ChatDirector.addItem(item, this);
        }
        public List<IItem> getItems()
        {
            return items;
        }
        public void setInvalidItem()
        {
            invalidItem = true;
        }
        public Context run(Context context)
        {
            ChainWorker worker = new ChainWorker(this, context);
            return worker.run(context);
        }
        public Context runAt(IItem item, Context context)
        {
            ChainWorker worker = new ChainWorker(this, item, context);
            return worker.run(context);
        }
        public void runAsync(Context context)
        {
            runAsync(items[0], context);
        }
#if Serializer_YamlDotNet
        public void Read(IParser parser, Type expectedType, ObjectDeserializer nestedObjectDeserializer)
        {
            Configuration config = new Configuration();
            parser.Consume<SequenceStart>();
            while (parser.Current.GetType()!=typeof(SequenceEnd)) {
                IItem item;
                if (parser.Current.GetType()==typeof(MappingStart)) {
                    parser.Consume<MappingStart>();
                    var itemName = parser.Consume<Scalar>().Value;
                    item = (IItem)nestedObjectDeserializer(ChatDirector.getConfigStaging().getItemClass(itemName));
                    parser.Consume<MappingEnd>();
                } else {
                    var itemName = parser.Consume<Scalar>().Value;
                    var itemType = ChatDirector.getConfigStaging().getItemClass(itemName);
                    if (itemType != null ) {
                        item = (IItem)Activator.CreateInstance(itemType);
                    } else {
                        this.setInvalidItem();
                        throw new Exception("Item of type "+itemName+" not found.");
                    }
                }
                this.addItem(item);
            }
            while(items.Contains(null)){
                items.Remove(null);
            }
            if(items.Count==0) {
                throw new Exception("No items parsed in chain");
            }
            parser.Consume<SequenceEnd>();
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
            return objectType.Equals(typeof(Chain));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Chain output = new Chain();
            Configuration config = new Configuration();
            reader.Read(); //Should be ArrayStart
            reader.Read(); // Object Start
            while (reader.TokenType != JsonToken.EndObject)
            {
                IItem item;
                if (reader.TokenType == JsonToken.PropertyName)
                {
                    var itemName = (string)reader.Value;
                    reader.Read(); //Advance to object
                    var type = ChatDirector.getConfigStaging().getItemClass(itemName);
                    item = (IItem)serializer.Deserialize(reader, ChatDirector.getConfigStaging().getItemClass(itemName));
                    reader.Read(); //Should be ObjectEnd
                }
                else
                {
                    reader.Read(); //Should be String
                    var itemName = (string)reader.Value;
                    var itemType = ChatDirector.getConfigStaging().getItemClass(itemName);
                    if (itemType != null)
                    {
                        item = (IItem)Activator.CreateInstance(itemType);
                    }
                    else
                    {
                        output.setInvalidItem();
                        throw new Exception("Item of type " + itemName + " not found.");
                    }
                }
                output.addItem(item);
            }
            while (output.items.Contains(null))
            {
                output.items.Remove(null);
            }
            if (output.items.Count == 0)
            {
                throw new Exception("No items parsed in chain");
            }
            reader.Read(); //Should be ObjectEnd
            reader.Read(); //Should be ArrayEnd
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