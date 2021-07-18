using System.Collections.Generic;
namespace ChatDirector.core
{
    public abstract class ItemDaemon : IDaemon
    {
        HashSet<IItem> items = new HashSet<IItem>();
        public void addItem(IItem item)
        {
            items.Add(item);
        }
        public bool load()
        {
            return true;
        }
        public bool unload()
        {
            items = new HashSet<IItem>();
            return true;
        }
        public HashSet<IItem> getItems()
        {
            return items;
        }
    }
}