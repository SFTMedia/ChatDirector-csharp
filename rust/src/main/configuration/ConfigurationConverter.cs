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
            var output = new Configuration();
            ChatDirector.setConfigStaging(output);
            reader.Read(); // Should be JsonToken.StartObject
            var moreChains = true;
            while (moreChains)
            {
                if (reader.TokenType != JsonToken.String) // Was scaler
                {
                    break;
                }
                switch (reader.Value)
                {
                    case "chains":
                        reader.Read(); // Should be ObjectStart
                        reader.Read(); //Should be String
                        var chainKey = (string)reader.Value;
                        var chain = serializer.Deserialize<Chain>(reader);
                        output.chains.Add(chainKey, chain);
                        reader.Read(); //Should be ObjectEnd
                        break;
                    case "debug":
                        reader.Read(); //Should be string
                        if (reader.TokenType == JsonToken.Boolean)
                        {
                            output.debug = (bool)reader.Value;
                        }
                        else
                        {
                            output.debug = Boolean.Parse((string)reader.Value);
                        }
                        break;
                    case "module_data":
                        reader.Read(); //Should be ObjectStart
                        reader.Read(); //Should be string
                        var test2 = serializer.Deserialize<Dictionary<String, Dictionary<String, String>>>(reader);
                        output.moduleData = test2;
                        reader.Read(); //Should be object end
                        throw new NotImplementedException();
                        break;
                    default:
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