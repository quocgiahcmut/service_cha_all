using Opc.Ua;
using Opc.Ua.Client;
using System.Collections;

namespace ImmWorkerService.ProtocolServices.UaCore;

public class LargeThreeUaClientHelper : UaClientHelper
{
    public LargeThreeUaClientHelper(string serverUrl) : base(serverUrl)
    {
    }
}
