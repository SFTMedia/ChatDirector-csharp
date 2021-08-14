using System;
using System.Collections.Generic;
namespace Oxide.Ext.ChatDirector.core
{
    public interface IModule : ILoadable, IValid
    {
        List<string> getItemNames();
        Type getItemClass(string type);
        Context getContext(object obj);
    }
}