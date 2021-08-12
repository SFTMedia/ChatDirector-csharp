namespace Oxide.Ext.ChatDirector
{
    using System;
    using System.Reflection;
    using Oxide.Core;
    using Oxide.Core.Extensions;
    using ChatDirector;

    public class ChatDirectorExtension : Extension
    {
        public ChatDirectorExtension(ExtensionManager manager) : base(manager) {}

        public override string Name => "ChatDirector";

        public override string Author => "blalp";

        public override VersionNumber Version => new VersionNumber(0, 0, 1);

        public override void OnModLoad()
        {
            Interface.Oxide.LogInfo("ChatDirector Extension loaded.");
        }
    }
}