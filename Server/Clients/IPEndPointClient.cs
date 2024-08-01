﻿using Server.Clients.ClientsMenegement;
using Server.Messages;
using Server.ServerMessenger;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
/*GabilyAslanov: Мы собираемся сделать наш класс полностью клиент-серверным с возможностью отправки данных сразу нескольким клиентам. Доработаем наш код следующим образом. Представьте что наш сервер умеет работать как медиатор (умеет отправлять сообщения по имени клиента), а также умеет возвращать список всех подключенных к нему клиентов. Для этого доработаем наш класс сообщений добавив поле ToName.
GabilyAslanov: Доработаем систему команд. Имя пользователя сервера всегда будет Server. Если сервер получает команду (как текст сообщения):
register: то он добавляет клиента в свой список.
delete: он удаляет клиента из списка
если сервер не получает имени получателя то он отправляет сообщение всем клиентам
если сервер получает имя получателя то он отправляет сообщение одному конкретному клиенту. Код сервера должен выглядеть примерно следующим образом:*/
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace Server.Clients
{
    public class IPEndPointClient : ClientBase
    {
        private IPEndPoint clientEndPoint;
        public virtual IPEndPoint ClientEndPoint
        {
            get { return clientEndPoint; }
            set { clientEndPoint = value; }
        }
        public IPEndPointClient()
        { }
        public virtual string IpEndPointToString
        {
            get { return clientEndPoint?.ToString(); }
            set
            {
                /*if (IPEndPoint.TryParse(value, out var result)) 
                {
                    clientEndPoint = result;
                }
                else
                {
                    throw new Exception("Ошибка преобразования");
                }*/
                if (IPEndPoint.TryParse(value, out var result))
                    clientEndPoint = result;
            }
        }

        public override void Receive(BaseMessage message)
        {
            Task.Run(() =>
            {
                new UdpMessenger().SendMessageAsync(message, ClientEndPoint);
            });
        }

        public override void Send(BaseMessage message, IClientMeneger mediator)
        {
            mediator.Send(message, this);
        }

        public override string? ToString()
        {
            return $"Клиент в базе: {Name} с {ClientEndPoint.ToString()}";
        }
        //To-do: убрать, оставить только в messenger
        internal override async Task SendToClientAsync<IPEndPoint>(ClientBase? client, BaseMessage message, IMessageSourceServer<IPEndPoint> ms)
        {
            if (client is IPEndPointClient ipClient)
                await new UdpMessenger().SendMessageAsync(message, ipClient.clientEndPoint);
        }

    }
}