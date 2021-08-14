using System.Collections.Generic;
namespace Oxide.Ext.ChatDirector.core
{
    public class Context : Dictionary<string, string>
    {
        bool shouldHalt = false;
        List<string> removeKeys = new List<string>();
        public Context()
        {
        }
        public Context(string current)
        {
            this.Add("CURRENT", current);
        }
        public Context(string current, string last): this(current)
        {
            this.Add("LAST", last);
        }
        public Context(Context context)
        {
            this.merge(context);
        }
        /**
         * @return Value of CURRENT in contexts, or an empty string if CURRENT is not
         *         found
         */
        public string getCurrent()
        {
            if (this.ContainsKey("CURRENT"))
            {
                return this["CURRENT"];
            }
            else
            {
                return "";
            }
        }
        /**
         * @return Value of LAST in contexts, or an empty string if LAST is not found
         */
        public string getLast()
        {
            if (this.ContainsKey("LAST"))
            {
                return this["LAST"];
            }
            else
            {
                return "";
            }
        }
        /**
         * @return If the execution should halt
         */
        public bool isHalt()
        {
            return shouldHalt;
        }
        /**
         * Merges the specified context into this one
         * 
         * @param context The context to merge
         */
        public void merge(Context context)
        {
            foreach (var item in context)
            {
                this.Add(item.Key, item.Value);
            }
            this.shouldHalt = context.isHalt();
            foreach (string key in context.removeKeys)
            {
                this.Remove(key);
            }
        }
        public Context halt()
        {
            shouldHalt = true;
            return this;
        }
        public new bool Remove(string key)
        {
            removeKeys.Add(key);
            return base.Remove(key);
        }
        public Context diff(Context other)
        {
            Context output = new Context();
            foreach (string key in this.Keys)
            {
                if (!other.ContainsKey(key))
                {
                    other.removeKeys.Add(key);
                }
                else
                {
                    if (other[key] != this[key])
                    {
                        output.Add(key, other[key]);
                    }
                }
            }
            if (other.shouldHalt || shouldHalt)
            {
                output.shouldHalt = true;
            }
            foreach (string key in other.removeKeys)
            {
                if (!this.removeKeys.Contains(key))
                {
                    output.removeKeys.Add(key);
                }
            }
            return output;
        }
        public Context getContextAtPath(string str)
        {
            Context output = new Context();
            foreach (string key in this.Keys)
            {
                if (key.StartsWith(str))
                {
                    output.Add(key.Substring(str.Length), this[key]);
                }
            }
            return output;
        }
    }
}