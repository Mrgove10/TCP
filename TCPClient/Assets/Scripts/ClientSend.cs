using System.Collections;
using System.Collections.Generic;
using TCPServer;
using UnityEngine;

public class ClientSend : MonoBehaviour
{
   private static void SendTCPData(Packet packet)
   {
      packet.WriteLength();
      Client.Instance.tcp.SendData(packet);
   }
   
   #region Packets

   public static void WelcomeReceived()
   {
      using (Packet packet = new Packet((int) ClientPackets.welcomeReceived))
      {
         packet.Write(Client.Instance.myId);
         packet.Write(UiManager.Instance.usernameInputField.text);
         
         SendTCPData(packet);
      }
   }
   
   #endregion
}
