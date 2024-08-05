using System;
using System.Net;
using System.Net.Sockets;

class ICVmain
{
    const int ReceivePort = 5005;  // 음성 데이터 수신 포트
    const int SendPort = 5006;     // 음성 데이터 전송 포트

    static void Main(string[] args)
    {
        UdpClient receiveClient = new UdpClient(ReceivePort);
        UdpClient sendClient = new UdpClient();

        IPEndPoint receiveEndPoint = new IPEndPoint(IPAddress.Any, ReceivePort);
        IPEndPoint sendEndPoint = new IPEndPoint(IPAddress.Loopback, SendPort);

        Console.WriteLine("UDP 서버가 {0} 포트에서 음성 데이터를 수신하고 {1} 포트로 전송합니다.", ReceivePort, SendPort);

        try
        {
            while (true)
            {
                // 데이터 수신
                byte[] data = receiveClient.Receive(ref receiveEndPoint);
                Console.WriteLine("수신한 데이터 길이: {0}", data.Length);

                // 받은 데이터를 클라이언트에게 다시 전송
                sendClient.Send(data, data.Length, sendEndPoint);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("예외 발생: " + e.Message);
        }
        finally
        {
            receiveClient.Close();
            sendClient.Close();
        }
    }
}
