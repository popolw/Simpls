using Horizon.XmlRpc.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMLRPC.Contracts;

namespace XMLRPC.Client
{
    public interface IAddServiceProxy : IXmlRpcProxy, IAddService
    {
    }
}
