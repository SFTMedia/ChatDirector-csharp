using ChatDirector.core;
namespace ChatDirector.extra
{
    public class MQTTInputItem : MQTTItem
    {
        public string topic;
        public MQTTInputItem()
        {
            ((IDaemon)ChatDirector.core.ChatDirector.getConfigStaging().getOrCreateDaemon(typeof(MQTTInputDaemon))).addItem(this);
        }
    }
}