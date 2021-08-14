using Oxide.Ext.ChatDirector.core;
using ChatDirectorMain = Oxide.Ext.ChatDirector.core.ChatDirector;
using System.Collections.Generic;


namespace Oxide.Ext.ChatDirector
{
    enum InputTypes
    {
        ServerStarted,
        ServerStopped,
        Chat,
        Login,
        Logout,
    }

    public class RustInputItemDaemon : IDaemon
    {
        private Dictionary<InputTypes, List<RustInputItem>> items = new Dictionary<InputTypes, List<RustInputItem>>();

        public void addItem(IItem item)
        {
            var rustInputItem = (RustInputItem)item;
            List<InputTypes> inputTypes = new List<InputTypes>();
            if (rustInputItem.login)
            {
                inputTypes.Add(InputTypes.Login);
            }
            if (rustInputItem.logout)
            {
                inputTypes.Add(InputTypes.Logout);
            }
            if (rustInputItem.server_started)
            {
                inputTypes.Add(InputTypes.ServerStarted);
            }
            if (rustInputItem.server_stopped)
            {
                inputTypes.Add(InputTypes.ServerStopped);
            }
            if (rustInputItem.chat)
            {
                inputTypes.Add(InputTypes.Chat);
            }
            foreach (var type in inputTypes)
            {
                if (!items.ContainsKey(type))
                {
                    items.Add(type, new List<RustInputItem>());
                }
                items[type].Add(rustInputItem);
            }
        }

        public bool load()
        {
            // Nothing needed here
            return true;
        }

        public bool unload()
        {
            // Nothing needed here
            return true;
        }

        public void OnServerInitialized(bool initial)
        {
            foreach (var item in this.items[InputTypes.ServerStarted])
            {
                var rustModule = Oxide.Plugins.RustChatDirector.instance.getModule(typeof(RustModule));

                var context = new Context();
                context.Add("SERVER_HOTLOAD", initial.ToString());

                ChatDirectorMain.run(item, context, true);
            }
        }

        public void OnServerShutdown()
        {
            foreach (var item in this.items[InputTypes.ServerStopped])
            {
                ChatDirectorMain.run(item, new Context(), true);
            }
        }

        public object OnPlayerChat(BasePlayer player, string message, ConVar.Chat.ChatChannel channel)
        {
            foreach (var item in this.items[InputTypes.Chat])
            {
                var rustModule = Oxide.Plugins.RustChatDirector.instance.getModule(typeof(RustModule));

                var context = new Context();
                context.Add("CHAT_MESSAGE", message);

                var contextFromPlayer = rustModule.getContext(player);
                foreach (var contextEntry in contextFromPlayer)
                {
                    context.Add(contextEntry.Key, contextEntry.Value);
                }
                var contextFromChatChannel = rustModule.getContext(channel);
                foreach (var contextEntry in contextFromChatChannel)
                {
                    context.Add(contextEntry.Key, contextEntry.Value);
                }

                ChatDirectorMain.run(item, context, true);
            }
            return null;
        }

        public void OnPlayerConnected(BasePlayer player)
        {
            foreach (var item in this.items[InputTypes.Chat])
            {
                var chatDirector = Oxide.Plugins.RustChatDirector.instance;

                var rustModule = chatDirector.getModule(typeof(RustModule));

                var context = new Context();

                var contextFromPlayer = rustModule.getContext(player);
                foreach (var contextEntry in contextFromPlayer)
                {
                    context.Add(contextEntry.Key, contextEntry.Value);
                }

                ChatDirectorMain.run(item, context, true);
            }
        }

        public void OnPlayerDisconnected(BasePlayer player, string reason)
        {
            foreach (var item in this.items[InputTypes.Chat])
            {
                var rustModule = Oxide.Plugins.RustChatDirector.instance.getModule(typeof(RustModule));

                var context = new Context();

                var contextFromPlayer = rustModule.getContext(player);
                foreach (var contextEntry in contextFromPlayer)
                {
                    context.Add(contextEntry.Key, contextEntry.Value);
                }

                ChatDirectorMain.run(item, context, true);
            }
        }
    }
}