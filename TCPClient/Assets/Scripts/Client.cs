using System;
using UnityEngine;
using System.Net.Sockets;

public class Client : MonoBehaviour
{
    public static Client Instance;
    public static int DataBufferSize = 4096;

    public string ip = "127.0.0.1";
    public int port = 26950;
    public int myId = 0;
    public TCP tcp;


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
        tcp.Connect();
    }

    public class TCP
    {
        public TcpClient socket;

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
            if (socket.Connected) //if it is connected the return
            {
                return;
            }

            stream = socket.GetStream(); //Sets the stream
            stream.BeginRead(receiveBuffer, 0, DataBufferSize, ReceiveCallback, null); //reade the stream
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

                //TODO : handle the data
                stream.BeginRead(receiveBuffer, 0, DataBufferSize, ReceiveCallback, null); //Continue reading data from the stream
            }
            catch (Exception _ex)
            {
                Console.WriteLine(_ex);
                throw;
            }
        }
    }
}