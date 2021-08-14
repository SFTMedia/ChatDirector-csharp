using System;
using System.Collections.Generic;
namespace Oxide.Ext.ChatDirector.core
{
    public interface IConfiguration : ILoadable, IValid
    {
        Dictionary<string, Chain> getChains();
        IModule getModule(Type class1);
        ILoadable getOrCreateDaemon(Type class1);
        bool isDebug();
        bool isTesting();
    }
}