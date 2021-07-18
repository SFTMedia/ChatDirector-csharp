namespace ChatDirector.core
{
    public abstract class PassItem : IItem
    {
        public abstract bool isValid();
        public Context process(Context context)
        {
            return new Context();
        }
    }
}