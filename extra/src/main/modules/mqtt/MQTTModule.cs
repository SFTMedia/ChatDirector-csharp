using ChatDirector.core;
using System;
using System.Collections.Generic;
using M2Mqtt.Messages;

namespace ChatDirector.extra
{
    public class MQTTModule : IModule
    {
        public bool load()
        {
            if (ChatDirector.core.ChatDirector.getConfig().getModuleData() == null
                    || ChatDirector.core.ChatDirector.getConfig().getModuleData()["mqtt"] == null)
            {
                if (ChatDirector.core.ChatDirector.getConfig().hasDaemon(typeof(MQTTConnections)))
                {
                    // Only spit out a warning if there were MQTT items
                    Console.WriteLine("Failed to load MQTT module, no module_data");
                }
                else
                {
                    // Of if debug mode is on
                    //Console.WriteLine(Level.INFO,"Failed to load MQTT module, no module_data. If you are not using MQTT items, you can safely ignore this.");
                }
                return true;
            }
            MQTTConnections connections = (MQTTConnections)ChatDirector.core.ChatDirector.getConfig()
                    .getOrCreateDaemon(typeof(MQTTConnections));
            var moduleData = ChatDirector.core.ChatDirector.getConfig().getModuleData()["mqtt"];
            foreach (string connectionKey in moduleData.Keys)
            {
                connections.Add(connectionKey, new MQTTConnection(moduleData[connectionKey]));
            }
            return true;
        }

        public bool unload()
        {
            return true;
        }
        public bool isValid()
        {
            return true;
        }
        public List<string> getItemNames()
        {
            return new List<string>(new string[] { "mqtt-input", "mqtt-output" });
        }
        public Type getItemClass(string type)
        {
            switch (type)
            {
                case "mqtt-input":
                    return typeof(MQTTInputItem);
                case "mqtt-output":
                    return typeof(MQTTOutputItem);
            }
            return null;
        }

        public Context getContext(Object obj)
        {
            Context output = new Context();
            if (typeof(MqttMsgPublishEventArgs).IsAssignableFrom(obj.GetType()))
            {
                output.Add("MQTT_TOPIC", ((MqttMsgPublishEventArgs)obj).Topic);
                output.Add("CURRENT", BitConverter.ToString(((MqttMsgPublishEventArgs)obj).Message));
                output.Add("MQTT_MESSAGE", output.getCurrent());
            }
            return output;
        }
    }
}