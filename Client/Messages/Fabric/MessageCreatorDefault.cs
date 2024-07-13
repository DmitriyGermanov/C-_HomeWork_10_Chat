using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Messages.Fabric
{
    internal class MessageCreatorDefault : BaseMessageFabric
    {
        public override BaseMessage FactoryMethod() => new DefaultMessage();
    }
}
