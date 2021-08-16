using System;
using System.Collections.Generic;
using ChatDirector.core;

namespace ChatDirector.core
{
    public class ConsoleModule : IModule
    {
        public Context getContext(object obj)
        {
            return new Context();
        }

        public Type getItemClass(string type)
        {
            switch (type) {
                case "console-output":
                    return typeof(ConsoleOutputItem);
                default:
                    return null;
            }
        }

        public List<string> getItemNames()
        {
            string[] temp = { "console-output" };
            return new List<string>(temp);
        }

        public bool isValid()
        {
            return true;
        }

        public bool load()
        {
            return true;
        }

        public bool unload()
        {
            return false;
        }
    }
}