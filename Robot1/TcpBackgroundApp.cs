using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Collections.Generic;
using Microsoft.Maui.Controls;
using System.Xml.Linq;
using System.IO;
using System;


namespace Robot1
{
    public class TcpBackgroundApp
    {
        public class TcpBackgroundWorker
        {
            private readonly TcpClient _client;
            //Nowe rozwiazanie

            IPAddress listeningIP = IPAddress.Parse("192.168.0.2");
            IPAddress sendingIP = IPAddress.Parse("192.168.0.1");

            public string RecvMessage; // Zmienna przechowujaca odebrane dane 

            private TcpListener _listen;
            private int portTo = 1000;  //TEMP
            private int portFrom = 60890; // TEMP
            private string server1 = "192.168.0.2"; // TEMP 

            private Socket clientSocket = null;
            private NetworkStream stream = null;

            private readonly CancellationTokenSource _cancellationTokenSource;
            public string ConnectionStatus;

            private Mutex _recvMessageMutex = new Mutex();

            public TcpBackgroundWorker()
            {
                ConnectionStatus = "Disconnected";
                RecvMessage = "Null";
                _client = new TcpClient();
                _listen = new TcpListener(listeningIP, portFrom);
                _cancellationTokenSource = new CancellationTokenSource();
            }

            public async Task Start(string server, int port)
            {

                // Łączenie się z serwerem
                
                try
                    {
                    _listen.Start();

                    await _client.ConnectAsync(sendingIP, portTo);
                    }
                    catch (System.Net.Sockets.SocketException e)
                    {
                        RecvMessage = "START(): " + e.ToString();
                        this.Stop();
                    }
                   // await Task.Delay(1000);
            
                

                // Petla sprawdzajaca polaczenie
                /*     while (!_cancellationTokenSource.IsCancellationRequested)
                     {
                         if (_client.Connected == true)
                         {
                             ConnectionStatus = "Connected";

                         }

                         else
                         {
                             ConnectionStatus = "Disconnected";
                         }
                         await Task.Delay(50);

                     }*/
            }

            public async Task<string> SendMessage(string message)
            {


                if (_client.Connected == true)
                {
                    ConnectionStatus = "Connected";


                    Byte[] data = Encoding.ASCII.GetBytes(message);
                    try
                    {
                        await _client.GetStream().WriteAsync(data, 0, data.Length); // dać try / catch
                    }
                    catch (System.IO.IOException e)
                    {

                    }
                    
                    


                    var responseData = new byte[1024];
                    var responseLength = 128; //await _client.GetStream().ReadAsync(responseData, 0, responseData.Length);

                    return Encoding.ASCII.GetString(responseData, 0, responseLength);
                }
                else
                {
                    ConnectionStatus = "Disconnected";
                    return null;
                }

            }

            public async Task ListenMessage(string server, int port) //na ta chwile argumenty funkcji nie wykorzystywane.
            {

                try { 
            
                    clientSocket = _listen.AcceptSocket();
                    stream = new NetworkStream(clientSocket);

                    while (true)
                    {
                        byte[] buffer = new byte[192];
                        int bytesReceived = 0;

                        try
                        {
                            while (((bytesReceived = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0))
                            {
                                string message = Encoding.ASCII.GetString(buffer, 0, bytesReceived);
                                string[] cutMsg = message.Split("\r\n");
                                _recvMessageMutex.WaitOne();
                                RecvMessage = cutMsg[0];
                                _recvMessageMutex.ReleaseMutex();
                            }
                        }
                        catch (System.IO.IOException e)
                        {
                            clientSocket.Close();
                            stream.Close();
                            _listen.Stop();
                            return;
                        }
                        await Task.Delay(50);
                        
                    }
                }
                catch (SocketException e)
                {
                    clientSocket.Close();
                    stream.Close();
                    _recvMessageMutex.WaitOne();
                    RecvMessage = e.ToString();
                    _recvMessageMutex.ReleaseMutex();
                    _listen.Stop();
                }
                //  _listen.Stop();

            }


            public void Stop()
            {
                // Żądanie zatrzymania pętli wysyłającej/odbierającej dane
                _cancellationTokenSource.Cancel();
                _listen.Stop();
                if (clientSocket != null)
                {
                    clientSocket.Close();
                }
                
                //      stream.Close();
                //      _client.Close();
            }

            public string Message()
            {
                string temp = "";
                _recvMessageMutex.WaitOne();
                temp = RecvMessage;
                _recvMessageMutex.ReleaseMutex();
                return temp;

            }

            public bool IsConnected()
            {
                return _client.Connected;
            }

            public string GetError()
            {
                return "";
            }
        }
    }
}
