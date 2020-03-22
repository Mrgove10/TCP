using System;

namespace TCPServer
{
    public class ServerSend
    {
        /// <summary>
        /// Send data to a particular client
        /// </summary>
        /// <param name="toClient"></param>
        /// <param name="packet"></param>
        private static void SendTCPData(int toClient, Packet packet)
        {
            packet.WriteLength();
            Server.clients[toClient].tcp.SendData(packet);
        }

        /// <summary>
        /// Sends data to all connected clients
        /// </summary>
        /// <param name="packet"></param>
        private static void SendTCPDataToAll(Packet packet)
        {
            packet.WriteLength();
            for (int i = 1; i < Server.MaxPlayer; i++)
            {
                Server.clients[i].tcp.SendData(packet);
            }
        }

        /// <summary>
        /// Sends data to all connected clients
        /// </summary>
        /// <param name="packet"></param>
        private static void SendTCPDataToAll(int exceptClient, Packet packet)
        {
            packet.WriteLength();
            for (int i = 1; i < Server.MaxPlayer; i++)
            {
                if (i != exceptClient)
                {
                    Server.clients[i].tcp.SendData(packet);
                }
            }
        }

        /// <summary>
        /// sends the welcome message
        /// </summary>
        /// <param name="toClient"></param>
        /// <param name="msg"></param>
        public static void Welcome(int toClient, string msg)
        {
            using (Packet packet = new Packet((int) ServerPackets.welcome))
            {
                packet.Write(msg);
                packet.Write(toClient);

                SendTCPData(toClient, packet);
            }
        }
    }
}