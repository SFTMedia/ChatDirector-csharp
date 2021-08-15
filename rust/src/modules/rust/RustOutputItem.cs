using ChatDirector.core;

namespace Oxide.Ext.RustChatDirector
{
    public class RustOutputItem : IItem
    {
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