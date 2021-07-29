using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using ChatDirector.core;
using System.Security.Cryptography;
using System.Text;
using System;

namespace ChatDirector.state
{
    public class SerializeStateItem : IItem
    {
        string token;

        public bool isValid()
        {
            return ValidationUtils.hasContent(new string[] { token }) && token.Length > 32;
        }

        public Context process(Context context)
        {
            var serializer = new SerializerBuilder().WithNamingConvention(HyphenatedNamingConvention.Instance).Build();
            var output = serializer.Serialize(context);
            var hmac = new HMACSHA512(Encoding.Default.GetBytes(token));
            var hash = hmac.ComputeHash(Encoding.Default.GetBytes(output));
            return new Context(BitConverter.ToString(hash)+output);
        }
    }
}