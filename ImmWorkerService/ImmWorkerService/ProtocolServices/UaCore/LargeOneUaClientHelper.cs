using Opc.Ua;
using Opc.Ua.Client;
using System.Collections;

namespace ImmWorkerService.ProtocolServices.UaCore;

public class LargeOneUaClientHelper : UaClientHelper
{
    public LargeOneUaClientHelper(string serverUrl) : base(serverUrl)
    {
    }
}
