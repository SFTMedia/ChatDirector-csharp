using ChatDirector.core;
using System.Collections.Generic;
using System.Threading;
using System;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt.Exceptions;

namespace ChatDirector.extra
{
    public class MQTTInputDaemon : IDaemon
    {
        List<MQTTInputItem> pendingItems = new List<MQTTInputItem>();
        Dictionary<string, Dictionary<string, List<MQTTInputItem>>> items = new Dictionary<string, Dictionary<string, List<MQTTInputItem>>>();
        public bool load()
        {
            MQTTConnections mqttConnections = (MQTTConnections)ChatDirector.core.ChatDirector.getConfig()
                    .getOrCreateDaemon(typeof(MQTTConnections));
            if (!mqttConnections.load())
            {
                Console.WriteLine("MQTT connections failed.");
                return false;
            }
            foreach (MQTTInputItem item in pendingItems)
            {
                if (!items.ContainsKey((item).connection))
                {
                    items.Add((item).connection, new Dictionary<string, List<MQTTInputItem>>());
                }
                if (!items[(item).connection].ContainsKey((item).topic))
                {
                    items[(item).connection].Add((item).topic, new List<MQTTInputItem>());
                }
                List<MQTTInputItem> existing = items[(item).connection][(item).topic];
                existing.Add(item);
            }
            foreach (string connetionName in items.Keys)
            {
                if (!mqttConnections.ContainsKey(connetionName))
                {
                    Console.WriteLine("MQTT Connection " + connetionName + " does not exist in module_data");
                    return false;
                }
                MQTTConnection connection = mqttConnections[connetionName];
                foreach (string itemTopicKey in items[connetionName].Keys)
                {
                    try
                    {
                        connection.mqtt.Subscribe(new string[] { itemTopicKey }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
                        new MQTTInputDaemonWorker(connection.mqtt,items[connetionName][itemTopicKey],connetionName);
                        if (ChatDirector.core.ChatDirector.getConfigStaging().isDebug())
                        {
                            Console.WriteLine("Subscribed to MQTT with topic " + itemTopicKey
                                    + " on connection " + connetionName + " with responce");
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("FAILED to subscribe to MQTT with topic " + itemTopicKey
                                + " on connection " + connetionName);
                        Console.WriteLine(e.ToString());
                        return false;
                    }
                }
            }
            return true;
        }

        public bool unload()
{
    return true;
}
public void addItem(IItem item)
{
    pendingItems.Add((MQTTInputItem)item);
}
    }
}