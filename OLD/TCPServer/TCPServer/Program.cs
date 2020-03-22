using System;
using System.Threading;

namespace TCPServer
{
    class Program
    {
        private static bool isRunning;

        static void Main(string[] args)
        {
            Console.Title = "TCPServer";
            isRunning = true;

            Thread mainThread = new Thread(new ThreadStart(MainThread));
            mainThread.Start();

            Server.Start(50, 26950);
        }


        private static void MainThread()
        {
            Console.WriteLine("Thread Started, running at " + Constants.TICK_PER_SEC + " tics pers second");
            DateTime nextloop = DateTime.Now;

            while (isRunning)
            {
                while (nextloop < DateTime.Now)
                {
                    GameLogic.Update();

                    nextloop = nextloop.AddMilliseconds(Constants.MS_PER_TICK);
                    
                    if (nextloop > DateTime.Now)
                    {
                        Thread.Sleep(nextloop - DateTime.Now);
                    }
                }
            }
        }
    }
}