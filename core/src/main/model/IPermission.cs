namespace ChatDirector.core
{
    public interface IPermission
    {
        public string getPrefix(string playerName);
        public string getSuffix(string playerName);
        public string getGroup(string playerName);
    }
}