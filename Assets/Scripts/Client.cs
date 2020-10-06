using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;


public class Client : MonoBehaviour
{
    public static Client instance;
    public static int dataBufferSize = 4096;
    public string ip = "127.0.0.1";
    public int port = 26950;
    public int myId = 0;
    public Tcp tcp;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        //asdasd
        else if (instance != this)
        {
            Debug.Log("Client instance already exist");
            Destroy(this);
        }
    }

    private void Start()
    {
        tcp = new Tcp();
    }

    public void ConnectToServer()
    {
        tcp.Connect();
    }

    public class Tcp
    {
        public TcpClient socket;
        private NetworkStream stream;
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

        private void ConnectCallBack(IAsyncResult result)
        {
            socket.EndConnect(result);

            if (!socket.Connected)
            {
                return;
            }

            stream = socket.GetStream();

            stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallBack, null);
        }

        private void ReceiveCallBack(IAsyncResult result)
        {
            try
            {
                int byteLength = stream.EndRead(result);
                if (byteLength <= 0)
                {
                    return;
                }

                byte[] data = new byte[byteLength];
                Array.Copy(receiveBuffer, data, byteLength);
                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallBack, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error receiving Tcp data : {ex}");
            }
        }
    }

}
