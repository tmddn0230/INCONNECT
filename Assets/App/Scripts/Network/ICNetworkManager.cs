using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// Net
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.IO;

public class ICNetworkManager : MonoBehaviour
{
    // Test InputField UI
    public InputField mIPInput, mPortInput, mNickInput;
    String mClientName;

    bool bSocketReady;
    TcpClient mSocket;
    NetworkStream mStream;
    StreamWriter mWriter;
    StreamReader mReader;

    public void ConnectToServer()
    {
        // if Client Connected aready, return
        if (bSocketReady) return;

        // HOST / PORT
        string ip = "127.0.0.1";
        int port = 25000;

        // Create Socket 
        try
        {
            mSocket = new TcpClient(ip, port);
            mStream = mSocket.GetStream();
            mWriter = new StreamWriter(mStream);
            mReader = new StreamReader(mStream);
            bSocketReady = true;
        }
        catch(Exception e)
        {
            Debug.Log($"Error: Can't Create Client Socket {e}");
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        ConnectToServer();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Send("SIBAL HUCK");
        }
    }

    void Send(string data)
    {
        if (!bSocketReady) return;

        mWriter.WriteLine(data);
        mWriter.Flush();
    }

    void OnApplicationQuit()
    {
        CloseSocket();
    }

    void CloseSocket()
    {
        if (!bSocketReady) return;

        mWriter.Close();
        mReader.Close();
        mSocket.Close();
        bSocketReady = false;
    }


}
