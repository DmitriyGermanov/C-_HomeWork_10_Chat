using System.Text.Json;

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

        public string SerializeMessageToJson() => JsonSerializer.Serialize(this);

        public static Message? DeserializeFromJson(string json) => JsonSerializer.Deserialize<Message>(json);

        public override string? ToString()
        {
            return $"{DateTime} {NicknameFrom}: {Text} \nTo: {NicknameTo}";
        }

    }
}
