using System;
using System.Net;
using System.Net.Sockets;
using NAudio.Wave;

class testmain
{
    const int SendPort = 5005;  // 서버의 음성 데이터 수신 포트
    const int ReceivePort = 5006; // 서버의 음성 데이터 전송 포트

    static void Main(string[] args)
    {
        UdpClient sendClient = new UdpClient();
        UdpClient receiveClient = new UdpClient(ReceivePort);

        IPEndPoint sendEndPoint = new IPEndPoint(IPAddress.Loopback, SendPort);
        IPEndPoint receiveEndPoint = new IPEndPoint(IPAddress.Any, ReceivePort);

        WaveFormat waveFormat = new WaveFormat(44100, 16, 1); // 44.1kHz, 16-bit, Mono
        BufferedWaveProvider waveProvider = new BufferedWaveProvider(waveFormat);
        WaveOutEvent waveOut = new WaveOutEvent();

        waveOut.Init(waveProvider);
        waveOut.Play();

        // 음성 데이터 전송을 위한 쓰레드
        var sendThread = new Thread(() =>
        {
            using (var waveIn = new WaveInEvent())
            {
                waveIn.WaveFormat = waveFormat;
                waveIn.BufferMilliseconds = 50; // 50ms 버퍼링
                waveIn.DataAvailable += (s, e) =>
                {
                    // 음성 데이터를 서버로 전송
                    sendClient.Send(e.Buffer, e.BytesRecorded, sendEndPoint);
                };

                waveIn.StartRecording();
                Console.WriteLine("음성 전송을 시작합니다. 종료하려면 Enter를 누르세요.");
                Console.ReadLine();
                waveIn.StopRecording();
            }
        });

        // 음성 데이터 수신을 위한 메인 쓰레드
        var receiveThread = new Thread(() =>
        {
            try
            {
                while (true)
                {
                    // 데이터 수신
                    byte[] data = receiveClient.Receive(ref receiveEndPoint);
                    waveProvider.AddSamples(data, 0, data.Length);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("예외 발생: " + e.Message);
            }
            finally
            {
                receiveClient.Close();
            }
        });

        receiveThread.Start();
        sendThread.Start();

        receiveThread.Join();
        sendThread.Join();
    }
}
