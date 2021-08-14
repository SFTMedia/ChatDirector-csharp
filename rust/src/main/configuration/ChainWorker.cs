using System;
namespace Oxide.Ext.ChatDirector.core
{
    public class ChainWorker
    {
        private Chain chain;
        private Context context;
        private IItem item;
        public ChainWorker(Chain chain)
        {
            this.chain = chain;
        }
        public ChainWorker(Chain chain, Context context): this(chain)
        {
            this.context = context;
        }
        public ChainWorker(Chain chain, IItem item): this(chain)
        {
            this.item = item;
        }
        public ChainWorker(Chain chain, IItem item, Context context): this(chain, item)
        {
            this.context = context;
        }
        public void run()
        {
            runAt(item, context);
        }
        /**
         * Runs chain from Start
         * 
         * @param context
         * @return Modified Contexts
         */
        public Context run(Context context)
        {
            Context output = context;
            output.merge(runAt(0, context));
            return output;
        }
        /**
         * This Starts execution at an item.
         * 
         * @param item
         * @param context
         * @return Modified Contexts
         */
        public Context runAt(IItem item, Context context)
        {
            int index = chain.getItems().IndexOf(item);
            if (index == -1)
            {
                Console.WriteLine(item + " not found in this " + chain + " chain.");
                return new Context().halt();
            }
            return runAt(index, context);
        }
        /**
         * This Starts execution at an index.
         * 
         * @param indexOf
         * @return Modified Contexts
         */
        private Context runAt(int indexOf, Context context)
        {
            Context output;
            for (int i = indexOf; i < chain.getItems().Count; i++)
            {
                if (ChatDirector.getConfig().isDebug())
                {
                    Console.WriteLine("Starting process of " + chain.getItems()[i]);
                    Console.WriteLine(context.ToString());
                }
                output = chain.getItems()[i].process(context);
                if (ChatDirector.getConfig().isDebug())
                {
                    Console.WriteLine("Ended process of " + chain.getItems()[i] + " with changed context " + output.ToString());
                }
                // Setup LAST and CURRENT contexts
                if (output != null)
                {
                    // Only if CURRENT was changed and not null set LAST
                    if (context["CURRENT"] != null && output["CURRENT"] != null
                            && !output.getCurrent().Equals(context.getCurrent()))
                    {
                        output.Add("LAST", context["CURRENT"]);
                    }
                    context.merge(output);
                    if (context.isHalt())
                    {
                        if (ChatDirector.getConfig().isDebug())
                        {
                            Console.WriteLine("Quitting chain, Halt received. " + this);
                        }
                        break;
                    }
                }
                else
                {
                    if (ChatDirector.getConfig().isDebug())
                    {
                        Console.WriteLine("Quitting chain. " + this);
                    }
                    break;
                }
            }
            if (ChatDirector.getConfig().isDebug())
            {
                Console.WriteLine("Ended process of " + this + " with context " + context);
            }
            return context;
        }
    }
}