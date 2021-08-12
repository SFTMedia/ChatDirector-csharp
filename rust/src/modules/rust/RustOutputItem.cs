using ChatDirector.core;

namespace Oxide.Ext.ChatDirector
{
    public abstract class RustOutputItem : IItem
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