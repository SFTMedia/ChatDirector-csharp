using System;
using System.Text;
using Newtonsoft.Json.Serialization;

namespace Oxide.Ext.ChatDirector.core
{
    public class SnakeCaseContractResolver : DefaultContractResolver
    {
        protected override string ResolvePropertyName(string propertyName)
        {
            // https://stackoverflow.com/questions/63055621/c-sharp-convert-camel-case-to-snake-case-with-two-capitals-next-to-each-other
            if(propertyName == null) {
                throw new ArgumentNullException(nameof(propertyName));
            }
            if(propertyName.Length < 2) {
                return propertyName;
            }
            var sb = new StringBuilder();
            sb.Append(char.ToLowerInvariant(propertyName[0]));
            for(int i = 1; i < propertyName.Length; ++i) {
                char c = propertyName[i];
                if(char.IsUpper(c)) {
                    sb.Append('_');
                    sb.Append(char.ToLowerInvariant(c));
                } else {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }
    }
}