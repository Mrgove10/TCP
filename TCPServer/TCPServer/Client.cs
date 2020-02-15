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

                receiveBuffer = new byte[dataBufferSize]; //initis the bufer with the correct size

                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
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

                    //TODO : hadle the data
                    stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null); //Continue reading data from the stream
                }
                catch (Exception _ex)
                {
                    Console.WriteLine(_ex);
                    throw;
                }
            }
        }
    }
}