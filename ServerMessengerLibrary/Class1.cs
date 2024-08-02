using System.Text.Json;

{
    public abstract class BaseMessage
    {
        public byte[] ClientNetId { get; set; }
        public string SerializeMessageToJson()
        {
            try
            {
                return JsonSerializer.Serialize(this);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return "false";
        }
    public static BaseMessage? DeserializeFromJson(string json) => JsonSerializer.Deserialize<DefaultMessage>(json);
}
}
