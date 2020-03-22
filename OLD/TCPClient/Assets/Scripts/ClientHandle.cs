using TCPServer;
using UnityEngine;

public class ClientHandle : MonoBehaviour
{
    public static void Welcome(Packet packet)
    {
        string msg = packet.ReadString();
        int myid = packet.ReadInt();
        
        Debug.Log("Welcome Message received " + msg);
        Client.Instance.myId = myid;
        ClientSend.WelcomeReceived();
    }
}
