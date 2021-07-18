namespace ChatDirector.core
{
    public class HaltItem : IItem
    {
        public bool isValid()
        {
            return true;
        }
        public Context process(Context context)
        {
            return new Context().halt();
        }
    }
}