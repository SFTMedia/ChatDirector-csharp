using System;
using System.Collections.Generic;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace ChatDirector.core
{
    public class Chain : IValid, IYamlConvertible
    {
        List<IItem> items = new List<IItem>();
        //@JsonIgnore
        bool invalidItem;
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
            // TODO: implement this
            throw new NotImplementedException();
        }

        public void Write(IEmitter emitter, ObjectSerializer nestedObjectSerializer)
        {
            throw new NotImplementedException();
        }
    }
}