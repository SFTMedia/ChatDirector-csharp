using System;
using System.Collections.Generic;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Core.Events;
namespace ChatDirector.core
{
    public class Chain : IValid, IYamlConvertible
    {
        List<IItem> items = new List<IItem>();
        [YamlIgnore]
        bool invalidItem;
        public Chain() {}
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
    }
}