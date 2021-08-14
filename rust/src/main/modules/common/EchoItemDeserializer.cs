using System;
using Newtonsoft.Json;

namespace Oxide.Ext.ChatDirector.core
{
    public class EchoItemConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType.Equals(typeof(EchoItem)));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var output = new EchoItem();
            output.format = (string)reader.Value;
            return output;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((EchoItem)value).format);
        }
    }
}