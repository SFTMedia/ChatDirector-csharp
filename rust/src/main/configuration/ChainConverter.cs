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
                Console.WriteLine("(DEBUG) before loop"+ reader.TokenType);
            while (reader.TokenType!=JsonToken.EndArray) {
                Console.WriteLine("(DEBUG) loop"+ reader.TokenType);
                IItem item;
                if (reader.TokenType==JsonToken.StartObject) {
                    reader.Read(); //Should be ObjectStart
                    reader.Read(); //Should be String
                    var itemName = (string)reader.Value;
                Console.WriteLine("(DEBUG) itemname"+ reader.Value);
                    item = (IItem)serializer.Deserialize(reader,ChatDirector.getConfigStaging().getItemClass(itemName));
                Console.WriteLine("(DEBUG) item"+ item);
                    reader.Read(); //Should be ObjectEnd
                } else {
                    reader.Read(); //Should be String
                    var itemName = (string)reader.Value;
                Console.WriteLine("(DEBUG) itemname"+ itemName);
                    var itemType = ChatDirector.getConfigStaging().getItemClass(itemName);
                    if (itemType != null ) {
                        item = (IItem)Activator.CreateInstance(itemType);
                Console.WriteLine("(DEBUG) item"+ item);
                    } else {
                        output.setInvalidItem();
                        throw new Exception("Item of type "+itemName+" not found.");
                    }
                }
                Console.WriteLine("(DEBUG) adding "+ item);
                output.addItem(item);
            }
            while(output.items.Contains(null)){
                output.items.Remove(null);
            }
            if(output.items.Count==0) {
                throw new Exception("No items parsed in chain");
            }
            reader.Read(); //Should be ArrayEnd
            return output;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}