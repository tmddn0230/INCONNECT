using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

class Program
{
    private static readonly Dictionary<IPEndPoint, int> clients = new Dictionary<IPEndPoint, int>();
    private static readonly object lockObj = new object();

    static void Main()
    {
        int port = 5005;
        UdpClient udpServer = new UdpClient(port);
        Console.WriteLine($"UDP 서버가 {port} 포트에서 음성 데이터를 수신합니다.");
        while (true)
        {
            Thread.Sleep(10);
            try
            {
                //if (!udpServer.Client.Connected)
                //{
                //    ClientDisconnect(udpServer);
                //}
                IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] clientData = udpServer.Receive(ref clientEndPoint);
                Console.WriteLine($"수신한 데이터 길이: {clientData.Length} from {clientEndPoint}");

                lock (lockObj)
                {
                    // 첫 번째 패킷으로 포트 정보를 받아서 저장
                    if (clientData.Length == 4) // 예를 들어, 포트 정보가 4바이트로 전달된다고 가정
                    {
                        int localPort = BitConverter.ToInt32(clientData, 0);
                        if (!clients.ContainsKey(clientEndPoint))
                        {
                            clients[clientEndPoint] = localPort;
                            Console.WriteLine($"클라이언트 추가: {clientEndPoint} (로컬 포트: {localPort})");
                        }
                    }
                    else
                    {
                        // 데이터 송신
                        foreach (var client in clients)
                        {
                            // 데이터를 보낸 클라이언트에게는 전송하지 않음
                            if (!client.Key.Equals(clientEndPoint))
                            {
                                try
                                {
                                    udpServer.Send(clientData, clientData.Length, new IPEndPoint(IPAddress.Loopback, client.Value));
                                }
                                catch (SocketException e)
                                {
                                    Console.WriteLine($"전송 오류 (SocketException): {e.Message}");
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine($"전송 오류: {e.Message}");
                                }
                            }
                        }
                    }
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine($"수신 오류 (SocketException): {e.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"수신 오류: {e.Message}");
            }
        }
    }

    static void ClientDisconnect(UdpClient udpServer)
    {
       // clients.Remove(udpServer)
    }
}
