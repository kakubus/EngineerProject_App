using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Collections.Generic;
using Microsoft.Maui.Controls;
using System.Xml.Linq;
using System.IO;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Robot1
{
    public class TcpBackgroundApp 
    {
        public class TcpBackgroundWorker : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            public string RecvMessage
            {
                get { return _recvMessage; }
                set
                {
                    _recvMessage = value;
                    OnPropertyChanged();
                }
            }

            public string ConnectionStatus
            {
                get { return _connectionStatus; }
                set
                {
                    _connectionStatus = value;
                    OnPropertyChanged();
                }
            }

            protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            public event Action<string> OnDataArrived;

            private readonly TcpClient _client;
            //Nowe rozwiazanie

            IPAddress listeningIP = IPAddress.Parse("192.168.0.2");
            IPAddress sendingIP = IPAddress.Parse("192.168.0.1");

            public string _recvMessage; // Zmienna przechowujaca odebrane dane 


            private TcpListener _listen;
            private int portTo = 1000;  //TEMP
            private int portFrom = 60890; // TEMP
        //    private string server1 = "192.168.0.2"; // TEMP 

            private Socket clientSocket = null;
            private NetworkStream stream = null;

            private readonly CancellationTokenSource _cancellationTokenSource;
            private string _connectionStatus;

            private Mutex _RecvMessageMutex = new Mutex();

            public TcpBackgroundWorker()
            {
                _connectionStatus = "Connection: Disconnected";
                RecvMessage = "To connect to the robot, connect to the Wifi network named \"ROBO-1\". After this action, you can connect to the Robot.";
                _client = new TcpClient();
                _listen = new TcpListener(listeningIP, portFrom);
                _cancellationTokenSource = new CancellationTokenSource();
            }

            public void RestartListen()
            {
               _listen.Start();
            }

            public async Task Start(string server, int port)
            {

                // Łączenie się z serwerem
                
                try
                    {
                        _listen.Start();

                        await _client.ConnectAsync(sendingIP, portTo);
                        ConnectionStatus = "ROBO-1 status: Connected";
                    }
                    catch (System.Net.Sockets.SocketException e)
                    {
                        RecvMessage = "START(): " + e.ToString();
                        this.Stop();
                    }

            }

            public async Task<string> SendMessage(string message)
            {


                if (_client.Connected == true)
                {
                    ConnectionStatus = "ROBO-1 status: Connected";


                    Byte[] data = Encoding.ASCII.GetBytes(message);
                    try
                    {
                        await _client.GetStream().WriteAsync(data, 0, data.Length); // dać try / catch
                    }
                    catch (System.IO.IOException e)
                    {
                        RecvMessage = "SENDMESSAGE(): " + e.ToString();
                    }
                    
                    


                    var responseData = new byte[1024];
                    var responseLength = 128; //await _client.GetStream().ReadAsync(responseData, 0, responseData.Length);

                    return Encoding.ASCII.GetString(responseData, 0, responseLength);
                }
                else
                {
                    _connectionStatus = "ROBO-1 status: Disconnected";
                    return null;
                }

            }

            public async Task ListenMessage(string server, int port) //na ta chwile argumenty funkcji nie wykorzystywane.
            {

                try {

                   
                        clientSocket = _listen.AcceptSocket(); 
             
                    
                    stream = new NetworkStream(clientSocket);

                    while (true || !_cancellationTokenSource.IsCancellationRequested)
                    {
                        byte[] buffer = new byte[192];
                        int bytesReceived = 0;

                        try
                        {
                            while (((bytesReceived = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0))
                            {
                                string message = Encoding.ASCII.GetString(buffer, 0, bytesReceived);
                                string[] cutMsg = message.Split("\r\n");
                                _RecvMessageMutex.WaitOne();
                                RecvMessage = "RoboOut: "+cutMsg[0];
                               
                                _RecvMessageMutex.ReleaseMutex();

                                OnDataArrived?.Invoke(RecvMessage);

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
                    _RecvMessageMutex.WaitOne();
                    _recvMessage = e.ToString();
                    _RecvMessageMutex.ReleaseMutex();
                    _listen.Stop();
                }
               // OnDataArrived.Invoke(Message());
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
                ConnectionStatus = "ROBO-1 status: Disconnected";

                // OnDataArrived.Invoke(Message());

                //      stream.Close();
                //      _client.Close();
            }

            public string Message()
            {
                string temp = "";
                _RecvMessageMutex.WaitOne();
                temp = this._recvMessage;
                _RecvMessageMutex.ReleaseMutex();
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
