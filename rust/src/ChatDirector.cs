using ChatDirector.rust;

namespace Oxide.Plugins
{
    [Info("Chat Director", "blalp", "0.0.1")]
    [Description("Chat Director")]
    public class RustChatDirector : CovalencePlugin
    {
        public static ChatDirector.core.ChatDirector instance { get; set; }
        private static RustInputItemDaemon itemDaemon { get; set; }
        private void Init()
        {
            // Hope that The directory is set correctly for config.yml in the root
            instance = new ChatDirector.core.ChatDirector();
        }

        void OnServerInitialized(bool initial)
        {
            itemDaemon.OnServerInitialized(initial);
        }

        void OnServerShutdown()
        {
            itemDaemon.OnServerShutdown();
        }

        object OnPlayerChat(BasePlayer player, string message, ConVar.Chat.ChatChannel channel)
        {
            itemDaemon.OnPlayerChat(player, message, channel);
            return null;
        }

        void OnPlayerConnected(BasePlayer player)
        {
            itemDaemon.OnPlayerConnected(player);
        }

        void OnPlayerDisconnected(BasePlayer player, string reason)
        {
            itemDaemon.OnPlayerDisconnected(player, reason);
        }
    }
}
