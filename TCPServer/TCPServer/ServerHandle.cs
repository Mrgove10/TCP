using System;

namespace TCPServer
{
    public class ServerHandle
    {
        public static void WelcomeReceived(int fromClient, Packet packet)
        {
            int clientidcheck = packet.ReadInt();
            string username = packet.ReadString();
            
            Console.WriteLine(clientidcheck + " connected ans received " + username);
            if (fromClient != clientidcheck)
            {
                Console.WriteLine("EERROR wrong id !!");
            }
        }
    }
}