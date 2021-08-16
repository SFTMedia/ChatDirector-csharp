using System;
using ChatDirector.core;

namespace ChatDirector.core
{
    public class ConsoleOutputItem : IItem
    {
        public bool isValid()
        {
            return true;
        }

        public Context process(Context context)
        {
            Console.WriteLine(context.getCurrent());
            return new Context();
        }
    }
}