using UnityEngine;
using UnityEngine.Networking;

namespace MultiplayerVR.Network
{
    [RequireComponent(typeof(VRNetworkDiscovery))]
    public class VRNetworkManager : NetworkManager
    {
        public GameObject VRPlayerPrefab;
        //private int playerCount = 0;
        VRNetworkDiscovery networkDiscovery;

        void Start()
        {
            this.networkDiscovery = GetComponent<VRNetworkDiscovery>();
            this.networkDiscovery.OnReceivedNetworkBroadcast += networkDiscovery_OnReceivedNetworkBroadcast;
        }

        void networkDiscovery_OnReceivedNetworkBroadcast(string fromAddress, string data)
        {
            if (client != null)
            {
                return;
            }

            Debug.Log("Received fromAddress:" + fromAddress);
            Debug.Log("Received data:" + data);

            this.networkAddress = fromAddress;
            base.StartClient();
        }

        public new void StartClient()
        {
            Debug.Log("StartClient");
            this.networkDiscovery.Initialize();
            this.networkDiscovery.StartAsClient();
        }

        public new void StopClient()
        {
            Debug.Log("StopClient");
            this.networkDiscovery.StopBroadcast();
            base.StopClient();
        }

        public override void OnStartServer()
        {
            Debug.Log("OnStartServer");
            this.networkDiscovery.Initialize();
            this.networkDiscovery.StartAsServer();
        }

        public override void OnStopServer()
        {
            Debug.Log("OnStopServer");
            this.networkDiscovery.StopBroadcast();
        }

        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId, NetworkReader extraMessageReader)
        {
            Player.SpawnMessage message = new Player.SpawnMessage();
            message.Deserialize(extraMessageReader);

            Transform startPosition = this.GetStartPosition();
            GameObject newPlayer = (GameObject)Instantiate(
                message.IsVrPlayer ? this.VRPlayerPrefab : this.playerPrefab,
                startPosition == null ? Vector3.zero : startPosition.position,
                startPosition == null ? Quaternion.identity : startPosition.rotation);

            NetworkServer.AddPlayerForConnection(conn, newPlayer, playerControllerId);
        }

        public override void OnClientConnect(NetworkConnection conn)
        {
            Player.SpawnMessage message = new Player.SpawnMessage();
            message.IsVrPlayer = UnityEngine.VR.VRSettings.enabled;
            ClientScene.AddPlayer(client.connection, 0, message);
        }
    }
}
