using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using ChatDirector.core;
using System;
using System.Security.Cryptography;
using System.Text;

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
            var hmac = new HMACSHA512(Encoding.Default.GetBytes(token));
            byte[] expectedHash = new byte[hmac.HashSize / 8];
            var input = context.getCurrent().Substring(expectedHash.Length);
            var realHash = hmac.ComputeHash(Encoding.Default.GetBytes(input));

            if (expectedHash == realHash) {
                // All is dandy
                var deserializer = new DeserializerBuilder().WithNamingConvention(HyphenatedNamingConvention.Instance).Build();
                var output = deserializer.Deserialize<Context>(input);
                return output;
            } else {
                Console.Error.WriteLine("Someone attempted to run commands without the proper token " + input);
                return new Context().halt();
            }
        }
    }
}