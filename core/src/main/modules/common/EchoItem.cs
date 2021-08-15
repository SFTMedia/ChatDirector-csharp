using System;
#if Serializer_YamlDotNet
using YamlDotNet.Core;
using YamlDotNet.Serialization;
#elif Serializer_Newtonsoft
using Newtonsoft.Json;
#endif
namespace ChatDirector.core
{
    public class EchoItem : IItem
#if Serializer_YamlDotNet
    , IYamlConvertible
#endif
    {
        internal string format;
        public bool isValid()
        {
            return format != null;
        }
        public Context process(Context context)
        {
            return new Context(ChatDirector.format(format, context));
        }
#if Serializer_YamlDotNet
        public void Read(IParser parser, Type expectedType, ObjectDeserializer nestedObjectDeserializer)
        {
            this.format = (string)nestedObjectDeserializer(typeof(string));
        }
        public void Write(IEmitter emitter, ObjectSerializer nestedObjectSerializer)
        {
            nestedObjectSerializer(this.format);
        }
#elif Serializer_Newtonsoft
public class Converter : JsonConverter
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
#endif
    }
}