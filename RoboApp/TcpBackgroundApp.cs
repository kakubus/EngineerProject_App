using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;

namespace RoboApp
{
    public class TcpBackgroundApp
    {
        public class TcpBackgroundWorker
        {
            private readonly TcpClient _client;
            private readonly CancellationTokenSource _cancellationTokenSource;

            public TcpBackgroundWorker()
            {
                _client = new TcpClient();
                _cancellationTokenSource = new CancellationTokenSource();
            }

            public async void Start(string server, int port)
            {
                
                // Łączenie się z serwerem
                try
                {
                    await _client.ConnectAsync(server, port);

                }
                catch (System.Net.Sockets.SocketException e)
                {
                    
                }

                // Pętla wysyłająca dane do serwera co 5 sekund
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    // Wysyłanie danych
                    var data = "";
                    
                    if(_client.Connected == true)
                    {
                       // await _client.GetStream().WriteAsync(Encoding.UTF8.GetBytes(data), 0, data.Length);
                    }



                    // Oczekiwanie 5 sekund
                    await Task.Delay(2000);
                }

                // Zamykanie połączenia
                _client.Close();
            }

            public async Task<string> SendMessage(string message)
            {
                // Wysyłanie wiadomości do serwera
                var data = Encoding.UTF8.GetBytes(message);
                await _client.GetStream().WriteAsync(data, 0, data.Length);

                // Odbieranie odpowiedzi od serwera
                var responseData = new byte[1024];
                var responseLength = await _client.GetStream().ReadAsync(responseData, 0, responseData.Length);
                return Encoding.UTF8.GetString(responseData, 0, responseLength);
            }

            public void Stop()
            {
                // Żądanie zatrzymania pętli wysyłającej dane
                _cancellationTokenSource.Cancel();
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
