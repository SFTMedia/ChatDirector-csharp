using System.Collections.Generic;
using ChatDirector.core;
using System;
namespace ChatDirector.extra
{
    public class MQTTConnections : Dictionary<string, MQTTConnection>, ILoadable
    {
        bool loaded = false;
        public bool load()
        {
            if (!loaded)
            {
                loaded = true;
                foreach (MQTTConnection connection in this.Values)
                {
                    if (!connection.load())
                    {
                        Console.WriteLine(connection + " failed to load.");
                        return false;
                    }
                }
            }
            return true;
        }
        public bool unload()
        {
            foreach (MQTTConnection connection in this.Values)
            {
                if (!connection.unload())
                {
                    Console.WriteLine(connection + " failed to load.");
                    return false;
                }
            }
            return true;
        }
    }
}