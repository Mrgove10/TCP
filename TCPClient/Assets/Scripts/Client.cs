using System;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using TCPServer;

public class Client : MonoBehaviour
{
    public static Client Instance;
    public static int DataBufferSize = 4096;

    public string ip = "127.0.0.1";
    public int port = 26950;
    public int myId = 0;
    public TCP tcp;

    private delegate void PacketHandler(Packet packet);

    private static Dictionary<int, PacketHandler> _packetHandlers;

    private void Awake()
    {
        //Singleton check
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != null)
        {
            Debug.Log("Instance already exists");
            Destroy(this);
        }
    }

    private void Start()
    {
        tcp = new TCP();
    }

    public void ConnectToServer()
    {
        InitializeClientData();
        tcp.Connect();
    }

    public class TCP
    {
        public TcpClient socket;

        private Packet receiveData;
        private NetworkStream stream;
        private byte[] receiveBuffer;

        /// <summary>
        /// Main connect methode to the server
        /// </summary>
        public void Connect()
        {
            socket = new TcpClient() //creates a new tcpserver with the connect buffer sizes
            {
                ReceiveBufferSize = DataBufferSize,
                SendBufferSize = DataBufferSize
            };
            receiveBuffer = new byte[DataBufferSize];
            socket.BeginConnect(Instance.ip, Instance.port, ConnectCallback, socket); //begins the connection to the server
        }

        /// <summary>
        /// is called when the cleint gets connectedto the server
        /// </summary>
        /// <param name="_result"></param>
        private void ConnectCallback(IAsyncResult _result)
        {
            socket.EndConnect(_result); //ends the conection
            if (!socket.Connected) //if it is connected the return
            {
                return;
            }

            stream = socket.GetStream(); //Sets the stream

            receiveData = new Packet();

            stream.BeginRead(receiveBuffer, 0, DataBufferSize, ReceiveCallback, null); //reade the stream
        }

        /// <summary>
        /// Sends data to the server
        /// </summary>
        /// <param name="packet"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void SendData(Packet packet)
        {
            try
            {
                if (socket != null)
                {
                    stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        /// <summary>
        /// Called when the read is finised
        /// </summary>
        /// <param name="_result"></param>
        private void ReceiveCallback(IAsyncResult _result)
        {
            try
            {
                int byteLength = stream.EndRead(_result); //handle the end of an async read

                if (byteLength <= 0)
                {
                    //Todo disconnect
                    return;
                }

                byte[] data = new byte[byteLength];
                Array.Copy(receiveBuffer, data, byteLength); //copy in a new array

                receiveData.Reset(HandleData(data)); // in case a packet is split between two deliveries (TCP protocol)
                stream.BeginRead(receiveBuffer, 0, DataBufferSize, ReceiveCallback, null); //Continue reading data from the stream
            }
            catch
            {
                //Todo disconnect
            }
        }

        /// <summary>
        /// Handled all the data
        /// mostly to do whit tcp stuff
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private bool HandleData(byte[] data)
        {
            int packetLength = 0;

            receiveData.SetBytes(data);

            if (receiveData.UnreadLength() >= 4)
            {
                packetLength = receiveData.ReadInt();
                if (packetLength <= 0)
                {
                    return true;
                }
            }

            while (packetLength > 0 && packetLength <= receiveData.UnreadLength())
            {
                byte[] packetBytes = receiveData.ReadBytes(packetLength);
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet packet = new Packet(packetBytes))
                    {
                        int packetId = packet.ReadInt();
                        _packetHandlers[packetId](packet);
                    }
                });

                packetLength = 0;
                if (receiveData.UnreadLength() >= 4)
                {
                    packetLength = receiveData.ReadInt();
                    if (packetLength <= 0)
                    {
                        return true;
                    }
                }
            }

            if (packetLength <= 1)
            {
                return true;
            }

            return false;
        }
    }    

    /// <summary>
    /// Initialises the packets 
    /// </summary>
    private void InitializeClientData()
    {
        _packetHandlers = new Dictionary<int, PacketHandler>()
        {
            {(int) ServerPackets.welcome, ClientHandle.Welcome}
        };
        Debug.Log("initialized packets");
    }
}