using System.Collections.Generic;
namespace Oxide.Ext.ChatDirector.core
{
    public class ValidationUtils
    {
        public static bool hasContent(IEnumerable<string> strings)
        {
            foreach (string s in strings)
            {
                if (s == null || s.Trim().Length != 0)
                {
                    return false;
                }
            }
            return true;
        }
        public static bool hasContent(IEnumerable<Chain> chains)
        {
            foreach (Chain chain in chains)
            {
                if (chain == null || !chain.isValid())
                {
                    return false;
                }
            }
            return true;
        }
        public static bool anyOf(IEnumerable<bool> checks)
        {
            foreach (bool check in checks)
            {
                if (check)
                {
                    return true;
                }
            }
            return false;
        }
    }
}