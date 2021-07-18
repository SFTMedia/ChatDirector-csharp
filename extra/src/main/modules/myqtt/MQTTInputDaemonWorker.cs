using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt;
using System;
using ChatDirector.core;
using System.Collections.Generic;

namespace ChatDirector.extra
{
    public class MQTTInputDaemonWorker
    {
        private List<MQTTInputItem> items;
        private string connectionName;
        public MQTTInputDaemonWorker(MqttClient client, List<MQTTInputItem> items, string connectionName)
        {
            this.items=items;
            this.connectionName=connectionName;
            client.MqttMsgPublishReceived += handleMessage;
        }
        public void handleMessage(object sender, MqttMsgPublishEventArgs e)
        {
            try
            {
                Context context = ChatDirector.core.ChatDirector.getInstance().getModule(typeof(MQTTModule)).getContext(e);
                foreach (MQTTInputItem item in items)
                {
                    ChatDirector.core.ChatDirector.run(item, context, true);
                }
            }
            catch (Exception el)
            {
                Console.WriteLine("FAILED to get message from MQTT connection " + connectionName);
                Console.WriteLine(el.ToString());
            }
        }
    }
}