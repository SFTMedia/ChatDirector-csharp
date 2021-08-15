using System;
using Oxide.Ext.ChatDirector.core;
using Oxide.Ext.ChatDirector;
namespace Oxide.Ext.ChatDirector.run
{
    public class ChatDirectorConsole
    {
        static void Main(string[] args)
        {
            Oxide.Ext.ChatDirector.core.ChatDirector instance = new Oxide.Ext.ChatDirector.core.ChatDirector("{\"debug\":true,\"chains\":{\"example\":[{\"rust-input\":{\"chat\":true,\"server_started\":true,\"server_stopped\":true,\"login\":true,\"logout\":true},\"echo\":\"Hello world\"}]}}");
	    instance.load();
	    foreach (var pair in instance.getChains()) {
		    Console.WriteLine(pair.Key);
		    Console.WriteLine(pair.Value);
		    Console.WriteLine("test");
	    }
	    Console.WriteLine("te");
        
        }
    }
}