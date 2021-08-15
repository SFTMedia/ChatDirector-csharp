using System.Text;
using ChatDirector.core;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace ChatDirector.extra
{
    public class MQTTOutputItem : MQTTItem
    {
        string topic;
        public new Context process(Context context)
        {
            Context output = new Context();
            MQTTConnection connection = ((MQTTConnections)ChatDirector.core.ChatDirector.getConfig()
                    .getOrCreateDaemon(typeof(MQTTConnections)))[this.connection];
            connection.mqtt.Publish(topic, Encoding.UTF8.GetBytes(context.getCurrent()), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
           
            return output;
        }
    }
}