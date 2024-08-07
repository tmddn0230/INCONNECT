using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.VisualBasic;
using NAudio.Wave;

class ICVClient
{
    static void Main(string[] args)
    {
        Console.WriteLine("UDP 클라이언트 시작");

        // INI 파일에서 포트 번호와 IP 주소 읽기
        var config = new ICVoiceClientCS.IniFile("client.ini");
        int localPort = int.Parse(config.Read("Ports", "LocalPort", "5001"));
        int serverPort = int.Parse(config.Read("Ports", "ServerPort", "5005"));
        string serverIpAddress = config.Read("Ports", "ServerIpAddress", "127.0.0.1");

        using (var receiveClient = new UdpClient(localPort))
        using (var sendClient = new UdpClient())
        {
            Console.WriteLine($"클라이언트가 {localPort} 포트에서 수신합니다.");

            // 서버에 로컬 포트 정보 전송
            byte[] localPortInfo = BitConverter.GetBytes(localPort);
            sendClient.Send(localPortInfo, localPortInfo.Length, new IPEndPoint(IPAddress.Parse(serverIpAddress), serverPort));

            // 음성 캡처 및 송신
            var waveIn = new WaveInEvent();
            var waveFormat = new WaveFormat(8000, 16, 1); // 8kHz, 16-bit, Mono
            waveIn.WaveFormat = waveFormat;
            waveIn.BufferMilliseconds = 2000; // 20ms buffer size
            waveIn.DataAvailable += (sender, e) =>
            {
                try
                {
                    // 서버에 데이터 송신
                    sendClient.Send(e.Buffer, e.BytesRecorded, new IPEndPoint(IPAddress.Parse(serverIpAddress), serverPort));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("송신 오류: " + ex.Message);
                }
            };
            waveIn.StartRecording();

            // 음성 수신 및 재생
            var waveOut = new WaveOutEvent();
            var buffer = new BufferedWaveProvider(waveFormat);
            waveOut.Init(buffer);
            waveOut.Play();

            Thread receiveThread = new Thread(() =>
            {
                IPEndPoint receiveEndPoint = new IPEndPoint(IPAddress.Any, localPort);

                try
                {
                    while (true)
                    {
                        byte[] data = receiveClient.Receive(ref receiveEndPoint);
                        buffer.AddSamples(data, 0, data.Length);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("수신 오류: " + e.Message);
                }
            });
            receiveThread.Start();

            // 클라이언트 종료를 기다리기 위한 대기
            Console.WriteLine("음성 통신을 종료하려면 Enter를 누르세요.");
            Console.ReadLine();

            waveIn.StopRecording();
            waveOut.Stop();
        }
    }
}
