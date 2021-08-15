using ChatDirector.core;
namespace ChatDirector.extra
{
    public class MQTTItem : IItem
    {
        public string connection;
        public bool isValid()
        {
            return ValidationUtils.hasContent(new string[]{connection});
        }
        public Context process(Context context)
        {
            return new Context();
        }
    }
}