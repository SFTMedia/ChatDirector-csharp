namespace ChatDirector.core
{
    public interface IItem : IValid
    {
        /**
         * Processes an item
         * 
         * @param context
         * @return A valid string
         */
        Context process(Context context);
    }
}