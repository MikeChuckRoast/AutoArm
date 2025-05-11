using System.Diagnostics;
using System.Net.Sockets;
using System.Text;

namespace AutoArm
{
    public class UdpListener
    {
        private readonly UdpClient _udpClient;
        private readonly Action<string> _onDataReceived;
        private CancellationTokenSource? _cancellationTokenSource;

        public UdpListener(int port, Action<string> onDataReceived)
        {
            _udpClient = new UdpClient(port);
            _onDataReceived =
                onDataReceived ?? throw new ArgumentNullException(nameof(onDataReceived));
        }

        public void Start()
        {
            // Initialize the CancellationTokenSource
            _cancellationTokenSource = new CancellationTokenSource();
            Debug.WriteLine("Starting UDP listener...");

            // Start listening asynchronously
            Task.Run(() => StartListeningAsync(_cancellationTokenSource.Token));
        }

        private async Task StartListeningAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    // Wait for data asynchronously
                    UdpReceiveResult result = await _udpClient.ReceiveAsync();

                    // Decode received data
                    string receivedData = Encoding.UTF8.GetString(result.Buffer);

                    // Invoke the delegate to handle the received data
                    _onDataReceived.Invoke(receivedData);
                }
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine("Listening canceled.");
            }
            catch (SocketException ex)
            {
                Debug.WriteLine($"SocketException: {ex.Message}");
            }
            finally
            {
                _udpClient.Dispose();
                Debug.WriteLine("UDP listener stopped.");
            }
        }

        public void Stop()
        {
            if (_cancellationTokenSource != null)
            {
                Debug.WriteLine("Stopping UDP listener...");
                _cancellationTokenSource.Cancel(); // Signal cancellation
                _cancellationTokenSource.Dispose(); // Clean up resources
                _cancellationTokenSource = null;
            }
        }

        public void Dispose()
        {
            Stop();
            _udpClient.Dispose();
        }
    }
}
