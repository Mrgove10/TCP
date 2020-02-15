using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace TCPServer
{
    class Server
    {
        public static int MaxPlayer { get; private set; }
        public static int Port { get; private set; }
        public static Dictionary<int, Client> clients = new Dictionary<int, Client>();
        public delegate void PacketHandler(int fromClient, Packet packet);
        public static Dictionary<int, PacketHandler> PacketHandlers;

        private static TcpListener TcpListener;

        /// <summary>
        /// Server start methode
        /// </summary>
        /// <param name="maxPlayers"></param>
        /// <param name="port"></param>
        public static void Start(int maxPlayers, int port)
        {
            MaxPlayer = maxPlayers;
            Port = port;

            Console.WriteLine("Starting Server on port " + port);
            InitialiseServerData();

            TcpListener = new TcpListener(IPAddress.Any, Port);
            TcpListener.Start();
            TcpListener.BeginAcceptTcpClient(new AsyncCallback(TcpConnectCallback), null); //looks forclients connecting

            Console.WriteLine("Server stated succesfully");
        }


        /// <summary>
        /// Called when a client is connecting to the server
        /// </summary>
        /// <param name="result"></param>
        private static void TcpConnectCallback(IAsyncResult result)
        {
            TcpClient _client = TcpListener.EndAcceptTcpClient(result); //accept the connnection of the client
            TcpListener.BeginAcceptTcpClient(new AsyncCallback(TcpConnectCallback), null); //continues listenigng for another client

            Console.WriteLine("new connection from : " + _client.Client.RemoteEndPoint);

            for (int i = 1; i <= MaxPlayer; i++)
            {
                if (clients[i].tcp.socket == null) //slot is empty
                {
                    clients[i].tcp.Connect(_client); //assigh the new client
                    return;
                }
            }

            Console.WriteLine("Server is full");
        }

        /// <summary>
        /// Init methode for the server
        /// </summary>
        private static void InitialiseServerData()
        {
            for (int i = 1; i <= MaxPlayer; i++)
            {
                clients.Add(i, new Client(i));
            }
            PacketHandlers = new Dictionary<int, PacketHandler>()
            {
                {(int)ClientPackets.welcomeReceived, ServerHandle.WelcomeReceived}
            };
            Console.WriteLine("Initialised packets");
        }
    }
}