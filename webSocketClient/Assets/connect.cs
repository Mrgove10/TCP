using Quobject.SocketIoClientDotNet.Client;
using UnityEngine;

public class connect : MonoBehaviour
{

    void c()
    {
        var socket = IO.Socket("http://localhost:3000");
        socket.On(Socket.EVENT_CONNECT, () =>
        {
            socket.Emit("hi");
        });

        socket.On("hi", (data) =>
        {
            Debug.Log(data);
            socket.Disconnect();
        });
    }
  
}