using UnityEngine.Networking;

namespace MultiplayerVR.Network
{
    public class VRNetworkDiscovery : NetworkDiscovery
    {
        public delegate void BroadcastEvent(string fromAddress, string data);

        public event BroadcastEvent OnReceivedNetworkBroadcast = delegate { };

        public override void OnReceivedBroadcast(string fromAddress, string data)
        {
            OnReceivedNetworkBroadcast(fromAddress, data);
        }
    }
}
