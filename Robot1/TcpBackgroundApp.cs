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
using System.Globalization;

namespace Robot1
{
    public class TcpBackgroundApp 
    {

      
        
        public class TcpBackgroundWorker : INotifyPropertyChanged
        {




            double[] RoboParameters = new double[25];

          
                public double GetStringCompassPosition(string x, string y, string z)
                {
                double position;
                double dbl_x, dbl_y, dbl_z;

                dbl_x = double.Parse(x, CultureInfo.InvariantCulture.NumberFormat);
                dbl_y = double.Parse(y, CultureInfo.InvariantCulture.NumberFormat);
                dbl_z = double.Parse(z, CultureInfo.InvariantCulture.NumberFormat);

                    double direction = Math.Atan2(dbl_y, dbl_x);
                    if(direction < 0)
                        direction += 2 * Math.PI;

                    if (direction > 2 * Math.PI)
                    {
                        direction -= 2 * Math.PI;
                    }
                position = direction * 180 / Math.PI;
             
                
                    return Math.Round(position, 2, MidpointRounding.ToZero);
                }
            private double _compassPosition;
            public double compassPosition
            {
                get { return _compassPosition; }
                set
                {
                    _compassPosition = value;
                    OnPropertyChanged();
                }
            }


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

            public double vA
            {
                get { return RoboParameters[0]; }
                set
                {
                    var temp = (double)(((double)((value / 60) * 2 * (Math.PI))) * 0.040075);
                    RoboParameters[0] = Math.Round(temp, 3, MidpointRounding.ToEven);
                    OnPropertyChanged();
                }
            }

            public double vB
            {
                get { return RoboParameters[1]; }
                set
                {
                    var temp = (double)(((double)((value / 60) * 2 * (Math.PI))) * 0.040075);
                    RoboParameters[1] = Math.Round(temp, 3, MidpointRounding.ToEven);
                    OnPropertyChanged();
                }
            }

            public double vC
            {
                get { return RoboParameters[2]; }
                set
                {
                    var temp = (double)(((double)((value / 60) * 2 * (Math.PI))) * 0.040075);
                    RoboParameters[2] = Math.Round(temp, 3, MidpointRounding.ToEven);
                    OnPropertyChanged();
                }
            }

            public double vD
            {
                get { return RoboParameters[3]; }
                set
                {
                    var temp = (double)(((double)((value / 60) * 2 * (Math.PI))) * 0.040075);
                    RoboParameters[3] = Math.Round(temp, 3, MidpointRounding.ToEven);
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

            private TcpClient _client;
            //Nowe rozwiazanie
            IPAddress listeningIP = IPAddress.Parse("192.168.0.2");

            IPAddress sendingIP = IPAddress.Parse("192.168.0.1");

            private string _recvMessage; // Zmienna przechowujaca odebrane dane 


            private TcpListener _listen;
            private int portTo = 1000;  //TEMP
            private int portFrom = 60890; // TEMP
                                          //    private string server1 = "192.168.0.2"; // TEMP 
            IPEndPoint ipEndPoint;
            Socket listener;

            private Socket clientSocket = null;
            private NetworkStream stream = null;

            private readonly CancellationTokenSource _cancellationTokenSource;
            private string _connectionStatus;

            private Mutex _RecvMessageMutex = new Mutex();

            public TcpBackgroundWorker()
            {
                _connectionStatus = "Connection: Disconnected";
                
                //    _listen = new TcpListener(listeningIP, portFrom);

              
                

                _cancellationTokenSource = new CancellationTokenSource();
                _recvMessage = "To connect to the robot, connect to the Wifi network named ROBO-1. After this action, you can connect to the Robot.";
                
            }

            public void RestartConnection()
            {
                _client = new TcpClient();
                //          _listen.Start();
            }

            public async Task Start(string server, int port)
            {

                // Łączenie się z serwerem
                //        _listen.Start();

                RestartConnection();

                try
                {
                    ConnectionStatus = "ROBO-1 status: Connected";
                    //  _listen.Start();

                    await _client.ConnectAsync(sendingIP, portTo);
                    //
                }
                catch (System.Net.Sockets.SocketException e)
                {
                    RecvMessage = "START(): " + e.ToString();
                    OnPropertyChanged(nameof(RecvMessage));
                    this.Stop();
                }
  


            }

            public async Task<string> SendMessage(string message)
            {


                if (_client.Connected == true)
                {
          
                    Byte[] data = Encoding.ASCII.GetBytes(message);
                    try
                    {
                        await _client.GetStream().WriteAsync(data, 0, data.Length); // dać try / catch
                    }
                    catch (System.IO.IOException e)
                    {
                        RecvMessage = "SEND(): " + e.ToString();
                        this.Stop();
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
                try
                {
                    
                    ipEndPoint = new(listeningIP, portFrom);
                    listener = new(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                    
                    listener.Bind(ipEndPoint);
                    listener.Listen(0);

                    using (Socket handler = await listener.AcceptAsync())
                    {
                        
                   
                        //var handler = await listener.AcceptAsync();
               

                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    var buffer = new byte[192];
                    int received;
                    string response;
                    try
                    {
                       // handler.ReceiveBufferSize = buffer.Length;
                       // handler.ReceiveTimeout = 100;
                        received = await handler.ReceiveAsync(buffer);
                   
                            response = Encoding.ASCII.GetString(buffer, 0, received);

                        string message = response;


                        string[] cutMsg = message.Split("\r\n");
                        _RecvMessageMutex.WaitOne();
                      
                        RecvMessage = "RoboOut: " + cutMsg[0];

                        _RecvMessageMutex.ReleaseMutex();

        
                        OnPropertyChanged(nameof(RecvMessage));

                        var eom = "<|EOM|>";
                        if (response.IndexOf(eom) > -1 )
                        {

                                  RecvMessage = "RoboOut: .. " + cutMsg[0];
                            //     OnPropertyChanged(nameof(RecvMessage));
                            break;
                        }
                       
                    

                            string[] parsedParameters = cutMsg[0].Split(", "); // powinno dac 25 elementow

                            if (parsedParameters.Count() == 25)
                            {
                                    try
                                    {
                                        vA = double.Parse((from rpm in parsedParameters select rpm).ElementAt(0), CultureInfo.InvariantCulture.NumberFormat);
                                        OnPropertyChanged(nameof(vA));
                                        vB = double.Parse((from rpm in parsedParameters select rpm).ElementAt(1), CultureInfo.InvariantCulture.NumberFormat);
                                        OnPropertyChanged(nameof(vB));
                                        vC = double.Parse((from rpm in parsedParameters select rpm).ElementAt(2), CultureInfo.InvariantCulture.NumberFormat);
                                        OnPropertyChanged(nameof(vC));
                                        vD = double.Parse((from rpm in parsedParameters select rpm).ElementAt(3), CultureInfo.InvariantCulture.NumberFormat);
                                        OnPropertyChanged(nameof(vD));

                                        // Pobieranie wartosci z kompasu
                                        var cX_temp = (from comp in parsedParameters select comp).ElementAt(4);
                                        var cY_temp =(from comp in parsedParameters select comp).ElementAt(5);
                                        var cZ_temp = (from comp in parsedParameters select comp).ElementAt(6);

                                        compassPosition = GetStringCompassPosition(cX_temp, cY_temp, cZ_temp);
                                        OnPropertyChanged(nameof(compassPosition)); 
                                    }
                                    catch(System.FormatException e)
                                    {

                                    }
                                    /*
                                    for (int i = 0; i < 25; i++)
                                    { 
                                    if ((float.TryParse(parsedParameters[i], out RoboParameters[i])) == true)
                                    {
                                          //  var linq = from vA, vB, vC, vD in RoboParameters select rpm;
                                            
                                            try
                                            {
                                                vA = float.Parse(parsedParameters[0], CultureInfo.InvariantCulture.NumberFormat);
                                                OnPropertyChanged(nameof(vA));

                                                vB = float.Parse(parsedParameters[1], CultureInfo.InvariantCulture.NumberFormat);
                                                OnPropertyChanged(nameof(vB));

                                                vC = float.Parse(parsedParameters[2], CultureInfo.InvariantCulture.NumberFormat);
                                                OnPropertyChanged(nameof(vC));

                                                vD = float.Parse(parsedParameters[3], CultureInfo.InvariantCulture.NumberFormat);
                                                OnPropertyChanged(nameof(vD));
                                            }
                                            catch(System.FormatException e)
                                            {
                                                
                                            }
                                        
                                    }

                                }*/

                                }
                                await Task.Delay(20);


                        }
                        catch (System.NullReferenceException e)
                    {
                        RecvMessage += " " + e.ToString();
                        OnPropertyChanged(nameof(RecvMessage));
                 
                    }
                    

                }
                handler.Shutdown(SocketShutdown.Both);
                //handler.Close();  //TESTOWO WYLACZONE!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                RecvMessage = "Listening stopped";
                OnPropertyChanged(nameof(RecvMessage));

                    }

                }
                catch (SocketException e)
                {
                    RecvMessage = e.ToString();
                    OnPropertyChanged(nameof(RecvMessage));
                }
                /* -- tu odkomentowac
                //_listen.Start();
                //   RestartListen();
                try
                {

                   // clientSocket = _listen.AcceptSocket();
                    clientSocket = await _listen.AcceptSocketAsync();
                }
                catch (SocketException e)
                {
                    clientSocket.Close();
                    stream.Close();
                    _RecvMessageMutex.WaitOne();
                    _recvMessage = e.ToString();
                    _RecvMessageMutex.ReleaseMutex();
                    _listen.Stop();
                    return;
                }
                bool dataAv = false;
                stream = new NetworkStream(clientSocket);
                
                await Task.Delay(200);
                dataAv = stream.DataAvailable;
                
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
                            OnPropertyChanged(nameof(RecvMessage));
                          //  OnDataArrived?.Invoke(RecvMessage);

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
                
                
               // OnDataArrived.Invoke(Message());
                //  _listen.Stop();
                */

            }


            public void Stop()
            {

                _cancellationTokenSource.Cancel();

                if(listener!=null)
                {
                    ipEndPoint = null;
                    listener.Close();
                    
                }

                if (_client != null)
                {
                    _client.Close();
                }
               

                ConnectionStatus = "ROBO-1 status: Disconnected";
                OnPropertyChanged(nameof(ConnectionStatus));
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
