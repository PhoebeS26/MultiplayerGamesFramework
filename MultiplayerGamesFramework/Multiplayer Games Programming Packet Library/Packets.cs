using Packet_Library;
using System.Net;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Packet_Library
{
    // Enum referencing different packet types
    public enum PacketType
    {
        MESSAGE,
        ASSIGN_CLIENT_ID,
        POSITION,
        GAME_STATE,
        UDP_POSITION,
        ENCRYPTED_PACKET,
        PUBLIC_KEY
    }

    // Enum representing replicated game objects
    public enum ReplicatedObject 
    {
        Ball,
        LeftPaddle,
        RightPaddle
    }

    // Enum representing types of game state updates
    public enum GameStateType 
    {
        SCORE_UPDATE,
        GAME_OVER
    }

    // Base class for all packets
    [JsonConverter(typeof(PacketConverter))]
    public abstract class Packet
    {
        [JsonPropertyName("Type")]
        public PacketType m_Type { get; set; }
        public int m_ID { get; set; }

        public string ToJson()
        {
            var options = new JsonSerializerOptions
            {
                IncludeFields = true,
                Converters = { new PacketConverter() }
            };

            return JsonSerializer.Serialize(this, options);
        }

        public static Packet? Deserialise(string json)
        {
            var options = new JsonSerializerOptions
            {
                IncludeFields = true,
                Converters = { new PacketConverter() }
            };

            return JsonSerializer.Deserialize<Packet>(json, options);
        }

        public virtual void PrintToConsole()
        {
            Console.WriteLine($"Packet Type: {m_Type}");
        }
    }

    // Text message packet
    public class MessagePacket : Packet
    {
        [JsonPropertyName("Message")]
        public string? Message { get; set; }

        public MessagePacket()
        {
            m_Type = PacketType.MESSAGE;
        }

        public MessagePacket(string message)
        {
            m_Type = PacketType.MESSAGE;
            Message = message;
        }

        public override void PrintToConsole()
        {
            Console.WriteLine($"[MessagePacket] Message: {Message}");
        }
    }

    // Assign unique client IDs
    public class AssignClientIDPacket : Packet
    {
        [JsonPropertyName("ClientID")]
        public int ClientID { get; set; }

        public AssignClientIDPacket()
        {
            m_Type = PacketType.ASSIGN_CLIENT_ID;
        }

        public AssignClientIDPacket(int id)
        {
            m_Type = PacketType.ASSIGN_CLIENT_ID;
            ClientID = id;
        }

        public override void PrintToConsole()
        {
            Console.WriteLine($"[AssignClientIDPacket] Client ID: {ClientID}");
        }
    }

    // Position packet for TCP updates
    public class PositionPacket : Packet
    {
        [JsonPropertyName("objectId")]
        public ReplicatedObject m_ObjectID { get; set; }

        [JsonPropertyName("Position - x")]
        public float m_X { get; set; }

        [JsonPropertyName("Position - y")]
        public float m_Y { get; set; }

        [JsonPropertyName("Rotation")]
        public float m_Rotation { get; set; }

        public PositionPacket()
        {
            m_Type = PacketType.POSITION;
        }

        public PositionPacket(ReplicatedObject id, float x, float y, float rotation)
        {
            m_ObjectID = id;
            m_Type = PacketType.POSITION;
            m_X = x;
            m_Y = y;
            m_Rotation = rotation;
        }

        public override void PrintToConsole()
        {
            Console.WriteLine($"[PositionPacket] X: {m_X}, Y: {m_Y}, Rot: {m_Rotation}");
        }
    }

    // UDP position packet
    public class UdpPositionPacket : Packet
    {
        [JsonPropertyName("clientId")]
        public int ClientId { get; set; }  

        [JsonPropertyName("objectId")]
        public ReplicatedObject ObjectId { get; set; }

        [JsonPropertyName("x")]
        public float X { get; set; }

        [JsonPropertyName("y")]
        public float Y { get; set; }

        [JsonPropertyName("rot")]
        public float Rotation { get; set; }

        public UdpPositionPacket()
        {
            m_Type = PacketType.UDP_POSITION;
        }

        public UdpPositionPacket(int clientId, ReplicatedObject id, float x, float y, float rot)
        {
            m_Type = PacketType.UDP_POSITION;
            ClientId = clientId;            
            ObjectId = id;
            X = x;
            Y = y;
            Rotation = rot;
        }
    }

    // Packet representing current game state
    public class GameStatePacket : Packet
    {
        [JsonPropertyName("LeftScore")]
        public int LeftScore { get; set; }

        [JsonPropertyName("RightScore")]
        public int RightScore { get; set; }

        [JsonPropertyName("Winner")]
        public string? Winner { get; set; }
        public GameStatePacket()
        {
            m_Type = PacketType.GAME_STATE;
        }
        public GameStatePacket(int leftScore, int rightScore, string? winner = null)
        {
            m_Type = PacketType.GAME_STATE;
            LeftScore = leftScore;
            RightScore = rightScore;
            Winner = winner;
        }

        public override void PrintToConsole()
        {
            Console.WriteLine($"[GameStatePacket] Left: {LeftScore}, Right: {RightScore}, Winner: {Winner}");
        }
    }

    // Packet for sending encrypted data
    public class EncryptPacket : Packet
    {
        [JsonPropertyName("Encryption")]
        public byte[] m_Encryption { get; set; }

        public EncryptPacket()
        {
            m_Type = PacketType.ENCRYPTED_PACKET;
            m_Encryption = new byte[0];
        }

        public EncryptPacket(int id, byte[] encryption)
        {
            m_ID = id;
            m_Encryption = encryption;
            m_Type = PacketType.ENCRYPTED_PACKET;
        }
    }

    // Packet for sending RSA public keys
    public class PublicKeyPacket : Packet
    {
        [JsonPropertyName("Modulus")]
        public string Modulus { get; set; }

        [JsonPropertyName("Exponent")]
        public string Exponent { get; set; }

        public PublicKeyPacket()
        {
            m_Type = PacketType.PUBLIC_KEY;
            Modulus = string.Empty;
            Exponent = string.Empty;
        }

        public PublicKeyPacket(string modulusBase64, string exponentBase64)
        {
            m_Type = PacketType.PUBLIC_KEY;
            Modulus = modulusBase64;
            Exponent = exponentBase64;
        }

        public PublicKeyPacket(RSAParameters rsaParams)
        {
            m_Type = PacketType.PUBLIC_KEY;
            Modulus = Convert.ToBase64String(rsaParams.Modulus ?? Array.Empty<byte>());
            Exponent = Convert.ToBase64String(rsaParams.Exponent ?? Array.Empty<byte>());
        }

        public RSAParameters ToRSAParameters()
        {
            return new RSAParameters
            {
                Modulus = Convert.FromBase64String(Modulus),
                Exponent = Convert.FromBase64String(Exponent)
            };
        }
    }

    // Converter to handle serialization and deserialization
    public class PacketConverter : JsonConverter<Packet>
    {
        public override Packet? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
            {
                var root = doc.RootElement;

                if (root.TryGetProperty("Type", out var typeProperty))
                {
                    PacketType type = (PacketType)typeProperty.GetInt32();

                    // Deserialize json into correct subclass based on type
                    return type switch
                    {
                        PacketType.MESSAGE => JsonSerializer.Deserialize<MessagePacket>(root.GetRawText(), options),
                        PacketType.ASSIGN_CLIENT_ID => JsonSerializer.Deserialize<AssignClientIDPacket>(root.GetRawText(), options),
                        PacketType.POSITION => JsonSerializer.Deserialize<PositionPacket>(root.GetRawText(), options),
                        PacketType.GAME_STATE => JsonSerializer.Deserialize<GameStatePacket>(root.GetRawText(), options),
                        PacketType.UDP_POSITION => JsonSerializer.Deserialize<UdpPositionPacket>(root.GetRawText(), options),
                        PacketType.ENCRYPTED_PACKET => JsonSerializer.Deserialize<EncryptPacket>(root.GetRawText(), options),
                        PacketType.PUBLIC_KEY => JsonSerializer.Deserialize<PublicKeyPacket>(root.GetRawText(), options),

                        _ => throw new JsonException("Unknown packet type")
                    };
                }
                throw new JsonException("Missing Type property in packet JSON");
            }
        }
        public override void Write(Utf8JsonWriter writer, Packet value, JsonSerializerOptions options)
        {
            // Serialize packet inlcuding its actual type
            JsonSerializer.Serialize(writer, value, value.GetType(), options);
        }
    }
}

    