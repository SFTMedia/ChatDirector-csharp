using Oxide.Ext.ChatDirector;

namespace Oxide.Plugins
{
    [Info("Chat Director", "blalp", "0.0.2")]
    [Description("Chat Director")]
    public class RustChatDirector : CovalencePlugin
    {
        public static Oxide.Ext.ChatDirector.core.ChatDirector instance { get; set; }
        private static RustInputItemDaemon itemDaemon { get; set; }
        private void Init()
        {
            // Hope that The directory is set correctly for config.yml in the root
            this.LoadConfig();
            instance = new Oxide.Ext.ChatDirector.core.ChatDirector(this.Config);
            instance.loadConfig();
            itemDaemon = (RustInputItemDaemon)instance.getOrCreateDaemon(typeof(RustInputItemDaemon));
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
