namespace ChatDirector.core
{
    public class BreakItem : IItem
    {
        public bool isValid()
        {
            return true;
        }
        public Context process(Context context)
        {
            return null;
        }
    }
}