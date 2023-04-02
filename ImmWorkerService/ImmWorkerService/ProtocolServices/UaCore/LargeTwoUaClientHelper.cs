using Opc.Ua;
using Opc.Ua.Client;
using System.Collections;

namespace ImmWorkerService.ProtocolServices.UaCore;

public class LargeTwoUaClientHelper : UaClientHelper
{
    public LargeTwoUaClientHelper(string serverUrl) : base(serverUrl)
    {
    }
}
