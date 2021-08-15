using Oxide.Ext.ChatDirector.core;

namespace Oxide.Ext.ChatDirector
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
            ((RustInputItemDaemon)Oxide.Ext.ChatDirector.core.ChatDirector.getConfigStaging().getOrCreateDaemon(typeof(RustInputItemDaemon))).addItem(this);
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