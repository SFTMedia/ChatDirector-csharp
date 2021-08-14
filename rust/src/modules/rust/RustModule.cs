using System;
using System.Collections.Generic;
using Oxide.Ext.ChatDirector.core;

namespace Oxide.Ext.ChatDirector
{
    public class RustModule : IModule
    {
        public List<string> getItemNames()
        {
            string[] temp = { "rust-input", "rust-output" };
            return new List<string>(temp);
        }
        public Context getContext(object obj)
        {
            Context output = new Context();
            if (typeof(BasePlayer).IsAssignableFrom(obj.GetType())) {
                BasePlayer basePlayer = (BasePlayer)obj;
                output.Add("PLAYER_NAME", basePlayer.displayName);
                output.Add("PLAYER_UUID", basePlayer.OwnerID.ToString());
            }

            if(typeof(ConVar.Chat.ChatChannel).IsAssignableFrom(obj.GetType())) {
                ConVar.Chat.ChatChannel chatChannel = (ConVar.Chat.ChatChannel)obj;
                output.Add("CHAT_CHANNEL", chatChannel.ToString());
            }

            return output;
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
                case "rust-output":
                    return typeof(RustOutputItem);
                case "rust-input":
                    return typeof(RustInputItem);
                default:
                    return null;
            }
        }
    }
}