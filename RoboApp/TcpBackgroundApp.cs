using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Collections.Generic;
using Microsoft.Maui.Controls;
using System.Xml.Linq;


namespace RoboApp
{
    public class TcpBackgroundApp
    {
        public class TcpBackgroundWorker
        {
            private readonly TcpClient _client;
            //Nowe rozwiazanie
          
            IPAddress listeningIP = IPAddress.Parse("192.168.0.2");
            IPAddress sendingIP = IPAddress.Parse("192.168.0.1");


            private TcpClient _clientRecv;
            private TcpListener _listen;
            private int portTo = 1000;  //TEMP
            private int portFrom = 60890; // TEMP
            private string server1 = "192.168.0.2"; // TEMP 

            private readonly CancellationTokenSource _cancellationTokenSource;
            public string ConnectionStatus;
            public string RecvMessage;

            public TcpBackgroundWorker()
            {
                ConnectionStatus = "Disconnected";
                RecvMessage= string.Empty;
                _client = new TcpClient();
                _clientRecv = new TcpClient();
                _cancellationTokenSource = new CancellationTokenSource();
                _listen = new TcpListener(listeningIP, portFrom);
                _listen.Start();

            }

            public async void Start(string server, int port)
            {

                // Łączenie się z serwerem
                try
                {
                    await _client.ConnectAsync(sendingIP, portTo);
                    

                }
                catch (System.Net.Sockets.SocketException e)
                {
                    RecvMessage = "START(): " + e.ToString();
                }

                // Pętla wysyłająca dane do serwera co 5 sekund
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    // Wysyłanie danych
                    var data = "";
                    
                    if(_client.Connected == true)
                    {
                        ConnectionStatus = "Connected";
                        
                     // unused  await ListenMessage("127.0.0.1", portFrom);
                        

                        /*    var buffer = new byte[1024];
                            var bytesReceived = await _client.GetStream().ReadAsync(buffer, 0, buffer.Length);
                            var message = Encoding.UTF8.GetString(buffer, 0, bytesReceived);
                            RecvMessage = message;*/
                        // await _client.GetStream().WriteAsync(Encoding.UTF8.GetBytes(data), 0, data.Length);
                    }
                    else
                    {
                        ConnectionStatus = "Disconnected";
                    }
                   
                    await Task.Delay(50);

                }

                // Zamykanie połączenia
                //
                _client.Close();
            }

            public async Task<string> SendMessage(string message)
            {
               

                if (_client.Connected == true)
                {
                    ConnectionStatus = "Connected";

                   
                    Byte[] data = Encoding.ASCII.GetBytes(message);
                   
                    await _client.GetStream().WriteAsync(data, 0, data.Length); // dać try / catch
                
                  
                    var responseData = new byte[1024];
                    var responseLength = 128; //await _client.GetStream().ReadAsync(responseData, 0, responseData.Length);
                    
                    return Encoding.ASCII.GetString(responseData, 0, responseLength); // bylo UTF8
            }else{
                ConnectionStatus = "Disconnected";
                return null;
            }
            
        }

            public async Task ListenMessage(string server, int port)
            {
                
                try
                {
                    
                    Socket client = _listen.AcceptSocket();
  
                    NetworkStream stream = new NetworkStream(client);
                    // Read incoming data from the client
                    byte[] buffer = new byte[256];
                    int bytesReceived = 0;
                    while ((bytesReceived = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        // Convert the data to an ASCII string
                        string message = Encoding.ASCII.GetString(buffer, 0, bytesReceived);
                        string[] cutMsg = message.Split("\n");
                        RecvMessage = cutMsg[0];
                    }

                    // Close the connection
                    client.Close();
                    stream.Close();
                }
                catch (SocketException e)
                {
                    RecvMessage = e.ToString();
                }

            }

            public void Stop()
            {
                // Żądanie zatrzymania pętli wysyłającej/odbierającej dane
                _cancellationTokenSource.Cancel();
                _listen.Stop();
                // _client.Close();
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
