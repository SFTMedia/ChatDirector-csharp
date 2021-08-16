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
            // Create the appropriate daemon, ChatDirector will search through the items and add them to the store later.
            ChatDirector.core.ChatDirector.getConfigStaging().getOrCreateDaemon(typeof(RustInputItemDaemon));
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