using System;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
namespace ChatDirector.core
{
    public class EchoItem : IItem, IYamlConvertible
    {
        string format;
        public bool isValid()
        {
            return format != null;
        }
        public Context process(Context context)
        {
            return new Context(ChatDirector.format(format, context));
        }
        public void Read(IParser parser, Type expectedType, ObjectDeserializer nestedObjectDeserializer)
        {
            this.format = (string)nestedObjectDeserializer(typeof(string));
        }
        public void Write(IEmitter emitter, ObjectSerializer nestedObjectSerializer)
        {
            nestedObjectSerializer(this.format);
        }
    }
}