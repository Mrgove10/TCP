using System;
using System.Net.Sockets;

namespace TCPServer
{
    public class Client
    {
        public static int dataBufferSize = 4096; //4MB
        public int ID;
        public TCP tcp;

        /// <summary>
        /// Constructor for the client
        /// </summary>
        /// <param name="_clientId"></param>
        public Client(int _clientId)
        {
            ID = _clientId;
            tcp = new TCP(ID);
        }

        public class TCP
        {
            public TcpClient socket;
            private readonly int ID;
            private NetworkStream stream;
            private Packet receivedData;
            private byte[] receiveBuffer;

            /// <summary>
            /// TCP Constructor
            /// </summary>
            /// <param name="_id"></param>
            public TCP(int _id)
            {
                ID = _id;
            }

            /// <summary>
            /// Connect methode of the tcp client
            /// </summary>
            /// <param name="_socket"></param>
            public void Connect(TcpClient _socket)
            {
                socket = _socket;
                socket.ReceiveBufferSize = dataBufferSize;
                socket.ReceiveBufferSize = dataBufferSize;

                stream = socket.GetStream();

                receivedData = new Packet();
                receiveBuffer = new byte[dataBufferSize]; //initis the bufer with the correct size

                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);

                ServerSend.Welcome(ID, "Welcome to the server !");
            }

            /// <summary>
            /// Send data to a client
            /// </summary>
            /// <param name="packet"></param>
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
                    Console.WriteLine(e);
                }
            }

            /// <summary>
            /// Callback once the client has received some data
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

                    receivedData.Reset(HandleData(data));
                    stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null); //Continue reading data from the stream
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
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

                receivedData.SetBytes(data);

                if (receivedData.UnreadLength() >= 4)
                {
                    packetLength = receivedData.ReadInt();
                    if (packetLength <= 0)
                    {
                        return true;
                    }
                }

                while (packetLength > 0 && packetLength <= receivedData.UnreadLength())
                {
                    byte[] packetBytes = receivedData.ReadBytes(packetLength);
                    ThreadManager.ExecuteOnMainThread(() =>
                    {
                        using (Packet packet = new Packet(packetBytes))
                        {
                            int packetId = packet.ReadInt();
                            Server.PacketHandlers[packetId](ID, packet);
                        }
                    });

                    packetLength = 0;
                    if (receivedData.UnreadLength() >= 4)
                    {
                        packetLength = receivedData.ReadInt();
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
    }
}