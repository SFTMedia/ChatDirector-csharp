using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using ChatDirector.core;
using System;
using System.Security.Cryptography;

namespace ChatDirector.state
{
    public class DeserializeStateItem : IItem
    {
        private string token;
        public bool isValid()
        {
            return ValidationUtils.hasContent(new string[] { token }) && token.Length > 32;
        }

        public Context process(Context context)
        {
            var hmac = new HMACSHA512(token);
            byte[] expectedHash = new byte[hmac.HashSize / 8];
            var input = context.getCurrent().SubString(expectedHash.Length);
            var realHash = hmac.ComputeHash(input);

            if (expectedHash == realHash) {
                // All is dandy
                var deserializer = new DeserializerBuilder().WithNamingConvention(HyphenatedNamingConvention.Instance).Build();
                var output = deserializer.Deserialize<Context>(input);
                return output;
            } else {
                throw new Exception("Someone attempted to run commands without the proper token " + input);
            }
        }
    }
}