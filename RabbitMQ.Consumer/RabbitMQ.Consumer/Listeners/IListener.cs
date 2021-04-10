using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ.Consumer.Listeners
{
    interface IListener
    {
        void Listen();
    }
}
