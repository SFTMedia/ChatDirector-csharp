using ChatDirector.core;
using System;
using System.Threading;
using System.IO;
namespace ChatDirector.console
{
    public class ChatDirectorConsole
    {
        static void Main(string[] args)
        {
            ChatDirector.core.ChatDirector chatDirector;
            if (args.Length >= 1 && File.Exists(args[0]))
            {
                chatDirector = new ChatDirector.core.ChatDirector(File.ReadAllText(args[0]));
            }
            else
            {
                chatDirector = new ChatDirector.core.ChatDirector();
            }
            try
            {
                if (!chatDirector.load())
                {
                    throw new Exception("Initial load failed.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                new Thread(new TimedLoad().run).Start();
            }
            Console.WriteLine("Finished Loading");
            string line;
            while (true)
            {
                Console.Write("ChatDirector > ");
                line = Console.ReadLine();
                if (line == "reload")
                {
                    Console.WriteLine("Reloading...");
                    new Thread(new TimedLoad().run).Start();
                }
                else
                {
                    Console.WriteLine("Only valid command is reload.");
                }
            }
        }
    }
}