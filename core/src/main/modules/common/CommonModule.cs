using System;
using System.Collections;
using System.Collections.Generic;

namespace ChatDirector.core
{
    public class CommonModule : IModule
    {
        public List<string> getItemNames()
        {
            string[] temp = { "pass", "stop", "halt", "break", "echo", "reload" };
            return new List<string>(temp);
        }

        public Context getContext(object obj)
        {
            return new Context();
        }
        public bool load()
        {
            return true;
        }
        public bool unload()
        {
            return true;
        }
        public bool isValid()
        {
            return true;
        }

        public Type getItemClass(string type)
        {
            switch (type)
            {
                case "stop":
                case "halt":
                    return typeof(HaltItem);
                case "break":
                    return typeof(BreakItem);
                case "echo":
                    return typeof(EchoItem);
                case "reload":
                    return typeof(ReloadItem);
                default:
                    return null;
            }
        }
    }
}