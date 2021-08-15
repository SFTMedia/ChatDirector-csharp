using System;
using Newtonsoft.Json;

namespace Oxide.Ext.ChatDirector.core
{
    public class ChainConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.Equals(typeof(Chain));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Chain output = new Chain();
            Configuration config = new Configuration();
            reader.Read(); //Should be ArrayStart
            reader.Read(); // Object Start
            while (reader.TokenType != JsonToken.EndObject)
            {
                IItem item;
                if (reader.TokenType == JsonToken.PropertyName)
                {
                    var itemName = (string)reader.Value;
                    reader.Read(); //Advance to object
                    var type = ChatDirector.getConfigStaging().getItemClass(itemName);
                    item = (IItem)serializer.Deserialize(reader, ChatDirector.getConfigStaging().getItemClass(itemName));
                    reader.Read(); //Should be ObjectEnd
                }
                else
                {
                    reader.Read(); //Should be String
                    var itemName = (string)reader.Value;
                    var itemType = ChatDirector.getConfigStaging().getItemClass(itemName);
                    if (itemType != null)
                    {
                        item = (IItem)Activator.CreateInstance(itemType);
                    }
                    else
                    {
                        output.setInvalidItem();
                        throw new Exception("Item of type " + itemName + " not found.");
                    }
                }
                output.addItem(item);
            }
            while (output.items.Contains(null))
            {
                output.items.Remove(null);
            }
            if (output.items.Count == 0)
            {
                throw new Exception("No items parsed in chain");
            }
            reader.Read(); //Should be ObjectEnd
            reader.Read(); //Should be ArrayEnd
            return output;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}