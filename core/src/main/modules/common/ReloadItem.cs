using System.Threading;
namespace ChatDirector.core
{
    public class ReloadItem : IItem
    {
        public bool isValid()
        {
            return true;
        }
        public Context process(Context context)
        {
            new Thread(new TimedLoad().run).Start();
            return new Context();
        }
    }
}