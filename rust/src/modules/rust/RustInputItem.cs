using System.Linq.Expressions;
using ChatDirector.core;
using System;

namespace Oxide.Ext.RustChatDirector
{
    public class RustInputItem : IItem
    {
        public bool chat;
        public bool login;
        public bool logout;
        public bool server_started;
        public bool server_stopped;

        public RustInputItem()
        {
            Console.WriteLine("(DEBUG) registering...");
            ((RustInputItemDaemon)ChatDirector.core.ChatDirector.getConfigStaging().getOrCreateDaemon(typeof(RustInputItemDaemon))).addItem(this);
            Console.WriteLine("(DEBUG) did register?" + ((RustInputItemDaemon)ChatDirector.core.ChatDirector.getConfigStaging().getOrCreateDaemon(typeof(RustInputItemDaemon))).items.ContainsKey(InputTypes.ServerStarted));        
        }

        public bool isValid()
        {
            return true;
        }
        public Context process(Context context)
        {
            return new Context();
        }
    }
}