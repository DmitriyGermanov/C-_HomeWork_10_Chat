using NetMQ;
using NetMQ.Sockets;
using Client.Messages;

namespace Client.ClientMessenger
    internal class NetMqMessenger : IMessageSourceClient<byte[]>, IDisposable
{
    private readonly ResponseSocket _serverSocket;
    private bool _disposed = false;

    public NetMqMessenger()
    {
        _serverSocket = new ResponseSocket();
    }

    public async Task SendMessageAsync(BaseMessage message)
    {
        string jsonToSend = message.SerializeMessageToJson();
        _serverSocket.Connect($"tcp://127.0.0.1:12345");
        _serverSocket.SendFrame(jsonToSend);
        await Task.CompletedTask;
    }

    public async Task<BaseMessage> RecieveMessageAsync(CancellationToken ctoken)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(NetMqMessenger));

        var message = await _serverSocket.ReceiveFrameStringAsync(ctoken);
        return BaseMessage.DeserializeFromJson(message.Item1);
    }

    public byte[] GetServerEndPoint()
    {
        return System.Text.Encoding.UTF8.GetBytes("@tcp://127.0.0.1:12345");
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _serverSocket?.Dispose();
            _disposed = true;
        }
    }
}
}