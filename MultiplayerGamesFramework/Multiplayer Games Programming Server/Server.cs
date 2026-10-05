using Packet_Library;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace Game_Server
{
	internal class Server
	{
        // Listener for TCP and UDP
		TcpListener m_TcpListener;
        UdpClient m_UdpListener;

        // Server RSA keypair for encryption and public key to share with clients
        private RSACryptoServiceProvider m_RsaProvider;
        public RSAParameters ServerPublicKey;

        // Thread safe collection of connected clients and reusable client IDs
        ConcurrentDictionary<int, ConnectedUser> m_Clients = new ConcurrentDictionary<int, ConnectedUser>();
        ConcurrentQueue<int> freedIds = new ConcurrentQueue<int>();

        // Counter for client IDs
        private int nextClientId = 0;

        // Generate unique client ID and reuse freed ones 
        private int GenerateUniqueId()
        {
            if (freedIds.TryDequeue(out int reusedId))
            {
                return reusedId;
            }
            return System.Threading.Interlocked.Increment(ref nextClientId);
        }

        public Server(string ipAddress, int port)
		{
            // Generate RSA keypair
            m_RsaProvider = new RSACryptoServiceProvider(1024);
            ServerPublicKey = m_RsaProvider.ExportParameters(false);

            Console.WriteLine("Server RSA keypair generated.");

            IPAddress ip = IPAddress.Parse(ipAddress);
			m_TcpListener = new TcpListener(ip, port);
            m_UdpListener = new UdpClient(port);
		}

		public void Start()
		{
			try
			{
                // Start UDP listener 
                Task.Run(() => UDPListen());

				m_TcpListener.Start();
				Console.WriteLine("Server has Started");

                // Accept incoming TCP connections
                while (true)
                {
                    Socket socket = m_TcpListener.AcceptSocket();
                    Console.WriteLine("Accepted a client!");

                    Thread clientThread = new Thread(() => ClientMethod(socket));
                    clientThread.Start();
                }
            }
            catch (Exception e)
			{
				Console.WriteLine("ERROR: " + e.Message);
			};
		}

		public void Stop()
		{
			m_TcpListener.Stop();
		}

        private void ClientMethod(Socket socket)
        {
            int clientId = GenerateUniqueId();
            ConnectedUser user = new ConnectedUser(socket, clientId);
            m_Clients[clientId] = user;

            try
            {
                // Send client ID and public key for encryption
                AssignClientIDPacket idPacket = new AssignClientIDPacket(clientId);
                user.Send(idPacket.ToJson());
                Console.WriteLine($"Sent client ID: {clientId}");

                PublicKeyPacket keyPacket = new PublicKeyPacket(Convert.ToBase64String(ServerPublicKey.Modulus), Convert.ToBase64String(ServerPublicKey.Exponent));

                user.Send(keyPacket.ToJson());
                Console.WriteLine($"Sent public key to client {clientId}");

                string? message;
                while ((message = user.Read()) != null)
                {
                    Packet? packet = Packet.Deserialise(message);

                    if (packet == null)
                    {
                        Console.WriteLine($"Client {clientId} sent invalid packet: {message}");
                        continue;
                    }

                    switch (packet.m_Type)
                    {
                        case PacketType.MESSAGE:
                            {
                                // Send message
                                try
                                {
                                    MessagePacket msgPacket = (MessagePacket)packet;
                                    Console.WriteLine($"Client {clientId} sent message: {msgPacket.Message}");

                                    MessagePacket response = new MessagePacket($"Server received: {msgPacket.Message}");
                                    user.Send(response.ToJson());
                                }
                                catch (Exception e) 
                                {
                                    Console.WriteLine($"Error handling MESSAGE packet from client {clientId}: {e.Message}");

                                }
                                break;
                            }

                        case PacketType.POSITION:
                            {
                                // Broadcast positions to all other clients
                                PositionPacket pos = (PositionPacket)packet;
                                foreach (ConnectedUser otherUser in m_Clients.Values) 
                                {
                                    if (otherUser.ClientId != clientId) 
                                    {
                                        try
                                        {
                                            otherUser.Send(packet.ToJson());
                                        }
                                        catch (Exception e) 
                                        {
                                            Console.WriteLine($"Failed to send to client {otherUser.ClientId}: {e.Message}");
                                        }
                                    }
                                }
                                break;
                            }

                        case PacketType.GAME_STATE:
                            {
                                // Send the updated game state to all clients
                                try
                                {
                                    GameStatePacket statePacket = (GameStatePacket)packet;

                                    m_Clients[clientId].Send(statePacket.ToJson());

                                    foreach (ConnectedUser otherUser in m_Clients.Values)
                                    {
                                        if (otherUser.ClientId != clientId)
                                        {
                                            try
                                            {
                                                otherUser.Send(packet.ToJson());
                                            }
                                            catch (Exception e)
                                            {
                                                Console.WriteLine($"Failed to send GAME_STATE to client {otherUser.ClientId}: {e.Message}");
                                            }
                                        }
                                    }
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine($"Error handling GAME_STATE packet from client {clientId}: {e.Message}");
                                }
                                break;
                            }

                        case PacketType.ENCRYPTED_PACKET:
                            {
                                // Decrypt incoming encrypted packet
                                EncryptPacket enc = (EncryptPacket)packet;

                                Console.WriteLine("Encrypted packet (Base64): " + Convert.ToBase64String(enc.m_Encryption));

                                byte[] decrypted = m_RsaProvider.Decrypt(enc.m_Encryption, false);
                                string json = Encoding.UTF8.GetString(decrypted);

                                Console.WriteLine("Decrypted packet JSON:");
                                Console.WriteLine(json);

                                Packet? original = Packet.Deserialise(json);

                                Console.WriteLine("Original packet type: " + original.m_Type);

                                break;
                            }

                        case PacketType.ASSIGN_CLIENT_ID:
                            {
                                Console.WriteLine($"Client {clientId} sent AssignClientIDPacket (ignored)");
                                break;
                            }

                        default:
                            {
                                Console.WriteLine($"Client {clientId} sent unknown packet type: {packet.m_Type}");
                                break;
                            }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Client {clientId} error: {e.Message}");
            }
            finally
            {
                // Clean up client on disconnect
                user.Close();
                m_Clients.TryRemove(clientId, out _);
                freedIds.Enqueue(clientId);
                Console.WriteLine($"Client {clientId} disconnected. Freed ID: {clientId}");
            }
        }
        private async Task UDPListen()
        {
            while (true)
            {
                UdpReceiveResult receiveResult = await m_UdpListener.ReceiveAsync();
                byte[] data = receiveResult.Buffer;
                string message = Encoding.UTF8.GetString(data);
               // Console.WriteLine("UDP Message Received: " + message);

                Packet? packet = Packet.Deserialise(message);
                if (packet == null)
                {
                    Console.WriteLine("Invalid UDP packet");
                    continue;
                }

                if (packet is UdpPositionPacket pos)
                {
                    //Console.WriteLine($"UDP Pos {pos.ObjectId}: {pos.X}, {pos.Y}");

                    // Identify sender client
                    ConnectedUser? senderClient = m_Clients.Values.FirstOrDefault(c => c.ClientId == pos.ClientId); 
                    if (senderClient != null)
                    {
                        // Set sender UDP endpoint
                        senderClient.UdpEndPoint ??= receiveResult.RemoteEndPoint;

                        // Broadcast to all other clients
                        foreach (var client in m_Clients.Values)
                        {
                            if (client.UdpEndPoint != null && client.UdpEndPoint != senderClient.UdpEndPoint)
                            {
                                try
                                {
                                    await m_UdpListener.SendAsync(data, data.Length, client.UdpEndPoint);
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine($"Failed UDP broadcast to client {client.ClientId}: {e.Message}");
                                }
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine($"UDP packet from unknown client: {message}");
                    }
                }
            }
        }
    }
}
