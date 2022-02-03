using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;

public class Client : MonoBehaviour
{
    public static Client instance;
    public static int dataBufferSize = 4096;

    public string ip = "127.0.0.1";
    public int port = 26950;
    public int myId = 0;
    public TCP tcp;

    private delegate void PacketHandler(Packet _packet);
    private static Dictionary<int, PacketHandler> packetHandlers;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("instance already exist, destroying objects");
            Destroy(this);
        }
    }

    private void Start()
    {
        tcp = new TCP();
    }

    public void ConnectedToServer()
    {
        InitializeClientData();
        tcp.Connect();
    }

    public class TCP
    {
        public TcpClient socket;

        private NetworkStream stream;
        private Packet receivedData;
        private byte[] receiveBuffer;

        public void Connect()
        {
            socket = new TcpClient
            {
                ReceiveBufferSize = dataBufferSize,
                SendBufferSize = dataBufferSize
            };

            receiveBuffer = new byte[dataBufferSize];
            socket.BeginConnect(instance.ip, instance.port, ConnectCallBack, socket);
        }

        private void ConnectCallBack(IAsyncResult _result)
        {
            socket.EndConnect(_result);

            if (!socket.Connected)
            {
                return;
            }

            stream = socket.GetStream();

            receivedData = new Packet();

            stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallBack, null);
        }

        public void SendData(Packet _packet)
        {
            try
            {
                if (socket != null)
                {
                    stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);
                }
            }

            catch(Exception _ex)
            {
                Debug.Log($"Error sending data to server via TCP: {_ex}");
            }
        }


        private void ReceiveCallBack(IAsyncResult _result)
        {
            try
            {
                int _byteLength = stream.EndRead(_result);
                if(_byteLength <= 0)
                {
                    //TODO disnconnect.
                    return;
                }

                byte[] _data = new byte[_byteLength];
                Array.Copy(receiveBuffer, _data, _byteLength);

                receivedData.Reset(HandleData(_data));
                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallBack, null);
            }

            catch
            {
                // TODO: disconnect
            }
        }

        private bool HandleData(byte[] _data)
        {
            int _packetLengh = 0;

            receivedData.SetBytes(_data);

            if (receivedData.UnreadLength() >= 4)
            {
                _packetLengh = receivedData.ReadInt();
                if(_packetLengh <= 0)
                {
                    return true;
                }
            }

            while (_packetLengh > 0 && _packetLengh <= receivedData.UnreadLength())
            {
                byte[] _packetBytes = receivedData.ReadBytes(_packetLengh);
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet _packet = new Packet(_packetBytes))
                    {
                        int _packetId = _packet.ReadInt();
                        packetHandlers[_packetId](_packet);
                    }
                });

                _packetLengh = 0;

                if (receivedData.UnreadLength() >= 4)
                {
                    _packetLengh = receivedData.ReadInt();
                    if (_packetLengh <= 0)
                    {
                        return true;
                    }
                }
            }

            if (_packetLengh <= 1)
            {
                return true;
            }

            return false;
        }
    }

    private void InitializeClientData()
    {
        packetHandlers = new Dictionary<int, PacketHandler>()
        {
            {  (int)ServerPackets.welcome, ClientHandle.Welcome }
        };
        Debug.Log("Initialized packet");
    }
}
