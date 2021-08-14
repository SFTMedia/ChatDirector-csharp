using System;
using Newtonsoft.Json;
namespace Oxide.Ext.ChatDirector.core
{
    [JsonObject()]
    public class EchoItem : IItem
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
    }
}