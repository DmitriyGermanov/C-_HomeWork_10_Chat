using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Client.Messages.Fabric
{
    public class MessageCreatorDefault : BaseMessageFabric
    {
        public override BaseMessage FactoryMethod() => new DefaultMessage();
        public override BaseMessage FactoryMethod(IPEndPoint iPEndPoint) => new DefaultMessage(iPEndPoint);
        public BaseMessage FactoryMethodWIthText(string text) => new DefaultMessage(text);
    }
}
