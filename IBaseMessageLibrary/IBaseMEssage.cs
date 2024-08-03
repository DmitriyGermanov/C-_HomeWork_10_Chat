using ServerCommonInterfacesLibrary;
using System.Net;

namespace IBaseMessageLibrary
{
    public interface IBaseMessage
    {
        int MessageID { get; set; }
        string? Text { get; set; }
        DateTime DateTime { get; set; }
        string? NicknameFrom { get; set; }
        string? NicknameTo { get; set; }
        int? UserIDFrom { get; set; }
        int? UserIdTo { get; set; }
        byte[] ClientNetId { get; set; }
        bool Ask { get; set; }
        bool DisconnectRequest { get; set; }
        bool UserIsOnline { get; set; }
        bool UserDoesNotExist { get; set; }
        IPEndPoint LocalEndPoint { get; set; }
        string LocalEndPointString { get; set; }
        string SerializeMessageToJson();
        string ToString();
        IClientBase ClientTo { get; set; }
        IClientBase ClientFrom { get; set; }
    }

  
}
}
