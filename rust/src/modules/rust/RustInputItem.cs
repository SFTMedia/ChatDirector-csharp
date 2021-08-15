using ChatDirector.core;

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
            ((RustInputItemDaemon)ChatDirector.core.ChatDirector.getConfigStaging().getOrCreateDaemon(typeof(RustInputItemDaemon))).addItem(this);
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