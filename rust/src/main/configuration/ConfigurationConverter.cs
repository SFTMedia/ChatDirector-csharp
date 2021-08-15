using System.Collections.Generic;
using System;
using Newtonsoft.Json;

namespace Oxide.Ext.ChatDirector.core
{
    public class ConfigurationConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.Equals(typeof(ConfigurationConverter));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Console.WriteLine("test");
            var output = new Configuration();
            ChatDirector.setConfigStaging(output);
            reader.Read(); // Should be JsonToken.StartObject
            var moreChains = true;
            while (moreChains)
            {
                if (reader.TokenType != JsonToken.String) // Was scaler
                {
                    Console.WriteLine("(DEBUG) reader "+reader.TokenType);
                    break;
                }
                switch (reader.Value)
                {
                    case "chains":
                        Console.WriteLine("(DEBUG) setting chains");
                        reader.Read(); // Should be ObjectStart
                        reader.Read(); //Should be String
                        Console.WriteLine("(DEBUG) "+reader.Value);
                        var chainKey = (string)reader.Value;
                        var chain = serializer.Deserialize<Chain>(reader);
                        Console.WriteLine("(DEBUG) "+chain);
                        output.chains.Add(chainKey, chain);
                        reader.Read(); //Should be ObjectEnd
                        break;
                    case "debug":
                        Console.WriteLine("(DEBUG) debug "+reader.Value);
                        reader.Read(); //Should be string
                        if (reader.TokenType == JsonToken.Boolean) {
                            output.debug = (bool)reader.Value;
                        } else {
                            output.debug = Boolean.Parse((string)reader.Value);
                        }
                        break;
                    case "module_data":
                        reader.Read(); //Should be ObjectStart
                        reader.Read(); //Should be string
                        var test2 = serializer.Deserialize<Dictionary<String,Dictionary<String,String>>>(reader);
                        output.moduleData = test2;
                        reader.Read(); //Should be object end
                        throw new NotImplementedException();
                        break;
                    default:
                        Console.WriteLine("(DEBUG) default" +reader.Value);
                        moreChains = false;
                        break;
                }
            }
            reader.Read(); //Should be ObjectEnd
            return output;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}