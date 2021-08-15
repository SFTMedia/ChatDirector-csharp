namespace ChatDirector.core
{
    public interface IPermission
    {
        string getPrefix(string playerName);
        string getSuffix(string playerName);
        string getGroup(string playerName);
    }
}