using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class ClientHandle : MonoBehaviour
{
    public static void Welcome(Packet _packet)
    {
        string _msg = _packet.ReadString();
        int myId = _packet.ReadInt();

        Debug.Log($"Message from Server: {_msg}");
        Client.instance.myId = myId;
        ClientSend.WelcomeReceived();

        Client.instance.udp.Connected(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);
    } 

}
