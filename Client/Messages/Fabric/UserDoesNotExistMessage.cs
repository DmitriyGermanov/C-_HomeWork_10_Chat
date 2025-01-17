﻿using ClientMessengerLibrary;
using System.Net;

namespace Client.Messages
{
    internal class UserDoesNotExistMessage : BaseMessage
    {
        public UserDoesNotExistMessage()
        {
            DisconnectRequest = true;
            Ask = true;
            Text = string.Empty;
            NicknameFrom = string.Empty;
            NicknameTo = string.Empty;
            DateTime = new DateTime();
            UserDoesNotExist = true;
            UserIsOnline = false;
        }
        public UserDoesNotExistMessage(IPEndPoint iPEndPoint)
        {
            LocalEndPoint = iPEndPoint;
            DisconnectRequest = true;
            Ask = true;
            Text = string.Empty;
            NicknameFrom = string.Empty;
            NicknameTo = string.Empty;
            DateTime = new DateTime();
            UserDoesNotExist = true;
            UserIsOnline = false;
        }
    }
}
