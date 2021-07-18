using System;
using System.Collections.Generic;
namespace ChatDirector.core
{
    public interface IModule : ILoadable, IValid
    {
        public List<string> getItemNames();
        public Type getItemClass(string type);
        public Context getContext(object obj);
    }
}