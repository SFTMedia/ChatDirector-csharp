using System.Threading;
using System;

namespace ChatDirector.core
{
    public class TimedLoad
    {
    private static TimedLoad instance;
    bool loop = true;
    public TimedLoad()
    {
        if (instance != null)
        {
            loop = false;
        }
        else
        {
            instance = this;
        }
    }
    public void run()
    {
        Console.WriteLine("Starting Timed load");
        while (loop)
        {
            Console.WriteLine("Timed load attempting to load");
            try
            {
                if (ChatDirector.getInstance().load())
                {
                    if (!ChatDirector.hasChains())
                    {
                        throw new Exception("No CHAINS!");
                    }
                    Console.WriteLine("Timed load completed.");
                    loop = false;
                    instance = null;
                    return;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            Console.WriteLine("Timed load sleeping");
            Thread.Sleep(10000);
        }
    }
}
}