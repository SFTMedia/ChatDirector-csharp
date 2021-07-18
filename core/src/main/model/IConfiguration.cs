using System;
using System.Collections.Generic;
namespace ChatDirector.core
{
    public interface IConfiguration : ILoadable, IValid
    {
        public Dictionary<string, Chain> getChains();
        public IModule getModule(Type class1);
        public ILoadable getOrCreateDaemon(Type class1);
        public bool isDebug();
        public bool isTesting();
    }
}