using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Client
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
        public bool DisconnectRequest = false;
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
        public string SerializeMessageToJson()
        {
            try
            {
                return JsonSerializer.Serialize(this);
            }
            catch (Exception e)
            {
                {
                    Console.WriteLine(e);
                }
            }
            return "false";
        }

        public static Message? DeserializeFromJson(string json) => JsonSerializer.Deserialize<Message>(json);

        public override string? ToString()
        {
            StringBuilder sb = new();
            if (DateTime != null)
                sb.Append(DateTime + " ");
            if (NicknameFrom != null)
                sb.Append(NicknameFrom + ": ");
            if (Text != null)
                sb.Append(Text);
            if (NicknameTo != null)
                sb.Append("\n"+ NicknameTo);
            return sb.ToString();
        }

    }
}
