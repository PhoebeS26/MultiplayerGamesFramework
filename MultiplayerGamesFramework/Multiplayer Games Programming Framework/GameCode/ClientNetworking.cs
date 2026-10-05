using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Packet_Library;
using Game_Client.Core.Utilities;
using Game_Client.Core;
using System.Diagnostics;
using System.Numerics;
using System.Threading.Tasks;
using System.Security.Cryptography;


namespace Game_Client.GameCode
{
    internal class ClientNetworking
    {
        private static ClientNetworking Instance;
        public static ClientNetworking m_Instance
        {
            get
            {
                if (Instance == null)
                {
                    return Instance = new ClientNetworking();
                }

                return Instance;
            }
        }

        // Client ID assigned by the server
        int m_ClientID = -1;
        public int ClientID
        {
            get { return m_ClientID; }
        }

        // Callbacks for reacting to network events
        public Action<int> OnClientIDUpdate;
        public Action<Vector2, float> onRemoteShipMovement;
        public Action<ReplicatedObject, Vector2, float> onRemoteObjectMovement;
        public Action<int, int, string?> onGameStateUpdate;

        // For encrypting messages
        private RSAParameters? serverPublicKey;

        // TCP and UDP clients
        TcpClient m_TcpClient;
        UdpClient m_UdpClient;

        NetworkStream m_Stream;
        StreamReader m_Reader;
        StreamWriter m_Writer;

        // Connects to server using TCP and UDP initializes streams and starts processing threads
        ClientNetworking()
        {
            m_TcpClient = new TcpClient();
            m_UdpClient = new UdpClient();
        }

        public bool Connect(string ip, int port)
        {
            try
            {
                m_TcpClient.Connect(ip, port);
                m_UdpClient.Connect(ip, port);

                m_Stream = m_TcpClient.GetStream();
                m_Reader = new StreamReader(m_Stream, Encoding.UTF8);
                m_Writer = new StreamWriter(m_Stream, Encoding.UTF8);

                Run();
                return true;
            }
            catch (Exception e) 
            {
                Debug.WriteLine("ERROR: " + e.Message);
            }
            return false;
        }

        // Sets server public key for encryption
        public void SetServerPublicKey(PublicKeyPacket packet)
        {
            serverPublicKey = new RSAParameters
            {
                Modulus = Convert.FromBase64String(packet.Modulus),
                Exponent = Convert.FromBase64String(packet.Exponent)
            };
        }

        // Starts TCP and UDP processing
        public void Run()
        {
            Thread TCPThread = new Thread(TcpProcessServerResponse);
            TCPThread.Name = "TCP thread";
            TCPThread.Start();

            Task.Run(() => UdpProcessServerResponse());
        }

        // Listens for UDP packets
        async Task UdpProcessServerResponse()
        {
            try
            {
                while (m_TcpClient.Connected)
                {
                    UdpReceiveResult receiveResult = await m_UdpClient.ReceiveAsync();
                    string message = Encoding.UTF8.GetString(receiveResult.Buffer);

                    Packet? packet = Packet.Deserialise(message);

                    if (packet is UdpPositionPacket pos)
                    {
                        // Fire callback to update remote object positions
                        onRemoteObjectMovement?.Invoke(pos.ObjectId, new Vector2(pos.X, pos.Y), pos.Rotation);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Client UDP Read Method exception: " + e.Message);
            }
        }

        // Listens for TCP messages from server, handles assignment, messages, position updates, game state, and public key 
        private void TcpProcessServerResponse()
        {
            try
            {
                while (m_TcpClient.Connected)
                {
                    string message = m_Reader.ReadLine();

                    Packet? packet = Packet.Deserialise(message);
                    if (packet == null)
                    {
                        Console.WriteLine("Received null or invalid packet");
                        continue;
                    }

                    switch (packet.m_Type)
                    {
                        case PacketType.ASSIGN_CLIENT_ID:
                            {
                                AssignClientIDPacket idPacket = (AssignClientIDPacket)packet;
                                SetClientID(idPacket.ClientID);
                                Console.WriteLine($"Assigned Client ID: {idPacket.ClientID}");
                                break;
                            }

                        case PacketType.MESSAGE:
                            {
                                MessagePacket msgPacket = (MessagePacket)packet;
                                Console.WriteLine($"Server says: {msgPacket.Message}");
                                break;
                            }

                        case PacketType.POSITION:
                            {
                                PositionPacket posPacket = (PositionPacket)packet;

                                // Fires both callbacks
                                onRemoteShipMovement?.Invoke(new Vector2(posPacket.m_X, posPacket.m_Y), posPacket.m_Rotation);
                                onRemoteObjectMovement?.Invoke(posPacket.m_ObjectID, new Vector2(posPacket.m_X, posPacket.m_Y), posPacket.m_Rotation);

                                break;
                            }

                        case PacketType.GAME_STATE:
                            {
                                GameStatePacket statePacket = (GameStatePacket)packet;
                                onGameStateUpdate?.Invoke(statePacket.LeftScore, statePacket.RightScore, statePacket.Winner);
                                break;
                            }

                        case PacketType.PUBLIC_KEY:
                            {
                                // Store server key and send encrypted login
                                SetServerPublicKey((PublicKeyPacket)packet);
                                Console.WriteLine("Received server public key.");
                                Login();

                                break;
                            }

                        default:
                            {
                                Console.WriteLine($"Unknown packet type: {packet.m_Type}");
                                break;
                            }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("ERROR: " + e.Message);
            }
        }

        // Sends a position update by UDP
        public void UdpSendPosition(ReplicatedObject id, float x, float y, float rotation)
        {
            UdpPositionPacket packet = new UdpPositionPacket(ClientID, id, x, y, rotation);
            string json = packet.ToJson();
            byte[] bytes = Encoding.UTF8.GetBytes(json);
            m_UdpClient.Send(bytes, bytes.Length);
        }

        // Sends raw UDP messages
        public void UdpSendMessage(string message) 
        {
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            m_UdpClient.Send(bytes, bytes.Length);
        }

        // Sends message over TCP
        public void TCPSendMessage(string message)
        {
            try
            {
                m_Writer.WriteLine(message);
                m_Writer.Flush();
            }
            catch (Exception e) 
            {
                Debug.WriteLine("Failed to send message: " + e.Message);
            }
        }

        public void TCPSendGameState(GameStatePacket packet)
        {
            TCPSendMessage(packet.ToJson());
        }

        // Sends encrypted login message using server RSA public key
        public void Login()
        {
            if (serverPublicKey == null)
            {
                Console.WriteLine("ERROR: Server public key not received yet.");
                return;
            }

            MessagePacket message = new MessagePacket("Hello encrypted server!");
            string json = message.ToJson();
            byte[] data = Encoding.UTF8.GetBytes(json);

            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.ImportParameters(serverPublicKey.Value);

                byte[] encryptedBytes = rsa.Encrypt(data, false);

                EncryptPacket encryptedPacket = new EncryptPacket(
                    (int)PacketType.MESSAGE,  
                    encryptedBytes
                );

                TCPSendMessage(encryptedPacket.ToJson());
                Console.WriteLine("Encrypted login packet sent.");
            }
        }

        // Closes all network streams and reset client ID
        public void Disconnect()
        {
            try
            {
                Debug.WriteLine("Disconnecting client...");

                if (m_Reader != null) m_Reader.Close();
                if (m_Writer != null) m_Writer.Close();
                if (m_Stream != null) m_Stream.Close();
                if (m_TcpClient != null && m_TcpClient.Connected)
                    m_TcpClient.Close();

                SetClientID(0);

                Debug.WriteLine($"Client {ClientID} disconnected.");

            }
            catch (Exception e)
            {
                Debug.WriteLine("Disconnect error: " + e.Message);
            }
        }

        /// <summary>
        /// This function will set the network ID and update the windows title. Use this to help with debugging.
        /// </summary>
        /// <param name="networkID"></param>
        public void SetClientID(int networkID)
        {
            m_ClientID = networkID;
            OnClientIDUpdate?.Invoke(networkID);
        }
    }
}
