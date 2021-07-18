using System.Collections.Generic;

namespace ChatDirector.core
{
    public interface IConfiguration : ILoadable, IValid
    {
        public Dictionary<string, Chain> getChains();
        public IModule getModule(IModule class1);
        public ILoadable getOrCreateDaemon(ILoadable class1);
        public bool isDebug();
        public bool isTesting();
    }
}