using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfAppRestoranOrder.Services
{
    public class SimpleChat : IDisposable
    {
        private TcpListener _listener;
        private string _otherComputerIP = "127.0.0.1";
        private int _listenPort;
        private int _sendPort;
        private string _userName;
        private bool _isListening;

        public SimpleChat(string userName, bool isAdmin = false)
        {
            _userName = userName;

            // Адмін: слухає порт 45000, відправляє на порт 45001
            // Кухня: слухає порт 45001, відправляє на порт 45000
            if (isAdmin)
            {
                _listenPort = 45000;
                _sendPort = 45001;
            }
            else
            {
                _listenPort = 45001;
                _sendPort = 45000;
            }
        }

        public void StartListening()
        {
            try
            {
                _isListening = true;
                _listener = new TcpListener(IPAddress.Any, _listenPort);
                _listener.Start();
                _ = Task.Run(ListenForMessages);
               // Console.WriteLine($"[{_userName}] Слухаю порт {_listenPort}");
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"[{_userName}] Помилка запуску слухача: {ex.Message}");
            }
        }

        private async Task ListenForMessages()
        {
            while (_isListening)
            {
                try
                {
                    var client = await _listener.AcceptTcpClientAsync();
                    var stream = client.GetStream();

                    byte[] buffer = new byte[1024];
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                   // Console.WriteLine($"[{_userName}] Отримано: {message}");
                    OnMessageReceived?.Invoke(message);

                    client.Close();
                }
                catch (ObjectDisposedException)
                {
                    
                    break;
                }
                catch (Exception ex)
                {
                    if (_isListening)
                    {
                        MessageBox.Show($"[{_userName}] Помилка отримання: {ex.Message}");
                        //Console.WriteLine($"[{_userName}] Помилка отримання: {ex.Message}");
                    }
                    await Task.Delay(1000); 
                }
            }
        }

        public async Task SendMessage(string message)
        {
            try
            {
                using var client = new TcpClient();
               
                await client.ConnectAsync(_otherComputerIP, _sendPort).WaitAsync(TimeSpan.FromSeconds(2));

                var stream = client.GetStream();
                string fullMessage = $"[{_userName}]: {message}";
                byte[] data = Encoding.UTF8.GetBytes(fullMessage);
                await stream.WriteAsync(data, 0, data.Length);

                //Console.WriteLine($"[{_userName}] Відправлено: {fullMessage}");
            }
            catch (TimeoutException)
            {
                MessageBox.Show($"[{_userName}] Таймаут підключення до {_sendPort}");
               // Console.WriteLine($"[{_userName}] Таймаут підключення до {_sendPort}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"[{_userName}] Помилка відправки: {ex.Message}");
                //Console.WriteLine($"[{_userName}] Помилка відправки: {ex.Message}");
            }
        }

        public void StopListening()
        {
            _isListening = false;
            _listener?.Stop();
            Console.WriteLine($"[{_userName}] Слухач зупинено");
        }

        public void Dispose()
        {
            StopListening();
        }

        public Action<string> OnMessageReceived { get; set; }
    }
}