using Packet_Library;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Game_Server
{
    // Represents connected client on the server, handles TCP communication and stores UDP endpoint
	internal class ConnectedUser
	{

        private Socket _socket;
        private NetworkStream _stream;
        private StreamReader _reader;
        private StreamWriter _writer;
        public int ClientId { get; private set; }
        public IPEndPoint? UdpEndPoint { get; set; }

        // Initializes connected user with TCP socket and unique client ID
        public ConnectedUser(Socket socket, int clientId)
        {
            _socket = socket;
            ClientId = clientId;

            _stream = new NetworkStream(_socket);
            _reader = new StreamReader(_stream, Encoding.UTF8);
            _writer = new StreamWriter(_stream, Encoding.UTF8);
        }

        // Closes all streams
        public void Close()
        {
            try
            {
                _writer.Close();   
                _reader.Close();   
                _stream.Close();   
                _socket.Close();   
            }
            catch (Exception e)
            {
                Debug.WriteLine("ERROR closing client: " + e.Message);
            }
        }

        // Reads from the client
        public string? Read()
        {
            try
            {
                string? message = _reader.ReadLine();
                Debug.WriteLine("Server Message: " + message); 

                return message;
            }
            catch (Exception e)
            {
                Debug.WriteLine("ERROR:" + e.Message);
                return null;
            }
        }

        // Sends text to the client over TCP
        public void Send(string message)
        {
            try
            {
                _writer.WriteLine(message); 
                _writer.Flush();            
            }
            catch(Exception e)
            {
                Debug.WriteLine("ERROR:" + e.Message);
            }
        }
    }
}
