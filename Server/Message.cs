using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Server
/*    Добавим JSON сериализацию и десериализацию в наш класс.И протестируем его путем компоновки простого сообщения, сериализации и десериализации этого сообщения.
Код Message может быть модифицирован следующим образом:*/
{
    public class Message
    {

        public string? Text { get; set; }
        public DateTime DateTime { get; set; }
        public string? NicknameFrom { get; set; }
        public string? NicknameTo { get; set; }
        public bool Ask = false;
        [JsonIgnore]
        public IPEndPoint LocalEndPoint { get; set; }
        public string LocalEndPointString
        {
            get => LocalEndPoint != null ? $"{LocalEndPoint.Address}:{LocalEndPoint.Port}" : "";
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    var parts = value.Split(':');
                    if (parts.Length == 2 && IPAddress.TryParse(parts[0], out var address) && int.TryParse(parts[1], out var port))
                    {
                        LocalEndPoint = new IPEndPoint(address, port);
                    }
                }
            }
        }

        public string SerializeMessageToJson() => JsonSerializer.Serialize(this);

        public static Message? DeserializeFromJson(string json) => JsonSerializer.Deserialize<Message>(json);

        public override string? ToString()
        {
            return $"{DateTime} {NicknameFrom}: {Text} \nTo: {NicknameTo}";
        }

    }
}
