using ChatDirector.core;
using M2Mqtt;
using System.Net;
using System;
namespace ChatDirector.extra
{
    public class MQTTConnection : ILoadable
    {
        // https://github.com/fusesource/mqtt-client
        // tcp://localhost:1883
        /*
         * ssl:// - Use the JVM default version of the SSL algorithm. sslv*:// - Use a
         * specific SSL version where * is a version supported by your JVM. Example:
         * sslv3 tls:// - Use the JVM default version of the TLS algorithm. tlsv*:// -
         * Use a specific TLS version where * is a version supported by your JVM.
         * Example: tlsv1.1
         */
        string address;
        public MqttClient mqtt;
        public MQTTConnection(string address)
        {
            this.address = address;
            mqtt = new MqttClient(address);
        }
        public bool load()
        {
            mqtt.Connect(Guid.NewGuid().ToString());
            return true;
        }
        public bool unload()
        {
            this.mqtt.Disconnect();
            return true;
        }
    }
}