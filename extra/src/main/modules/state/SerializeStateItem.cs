#if Serializer_YamlDotNet
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
#elif Serializer_Newtonsoft
using Newtonsoft.Json;
#else
using Woah_There_You_Need_Either_YamlDotNet_or_Newtonsoft_to_use_this;
#endif
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
            string output = null;
            #if Serializer_YamlDotNet
            var serializer = new SerializerBuilder().WithNamingConvention(HyphenatedNamingConvention.Instance).Build();
            output = serializer.Serialize(context);
            #elif Serializer_Newtonsoft
            ERROR
            #endif
            var hmac = new HMACSHA512(Encoding.Default.GetBytes(token));
            var hash = hmac.ComputeHash(Encoding.Default.GetBytes(output));
            return new Context(BitConverter.ToString(hash)+output);
        }
    }
}