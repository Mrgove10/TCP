using System;

namespace TCPServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "TCPServer";

            Server.Start(50, 26950);
            
            Console.ReadKey(); 
        }
    }
}