using ChatDirector.core;
using System;
using System.Collections.Generic;

namespace ChatDirector.extra
{
    public class StateModule : IModule
    {
        public List<string> getItemNames()
        {
            return new List<string>(new string[] { "serialize-state", "deserialize-state" });
        }
        public Type getItemClass(string type)
        {
            switch (type)
            {
                case "serialize-state":
                    return typeof(SerializeStateItem);
                case "deserialize-state":
                    return typeof(DeserializeStateItem);
                default:
                    return null;
            }
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
            return ValidationUtils.hasContent(new string[] { ChatDirector.core.ChatDirector.getConfigStaging().getModuleData()["state"]["token"] });
        }
    }
}