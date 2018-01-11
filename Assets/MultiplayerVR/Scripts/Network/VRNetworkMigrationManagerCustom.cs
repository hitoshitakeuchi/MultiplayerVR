using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using System.Linq;
using System.Collections;

namespace MultiplayerVR
{
    public class VRNetworkMigrationManagerCustom : NetworkMigrationManager
    {
        protected override void OnClientDisconnectedFromHost(NetworkConnection conn, out SceneChangeOption sceneChange)
        {
            sceneChange = SceneChangeOption.StayInOnlineScene;

            PeerInfoMessage hostInfo;
            bool newHost;
            if (FindNewHost(out hostInfo, out newHost))
            {
                newHostAddress = hostInfo.address;
                if (newHost)
                    waitingToBecomeNewHost = true;
                else
                    waitingReconnectToNewHost = true;
            }
            else
            {
                if (LogFilter.logError) Debug.LogError("No Host found.");

                NetworkManager.singleton.SetupMigrationManager(null);
                NetworkManager.singleton.StopHost();
                Reset(ClientScene.ReconnectIdInvalid);
            }

            //if (LogFilter.logDebug) Debug.Log("BecomeHost: " + waitingToBecomeNewHost + ", ReconnectToNewHost: " + waitingReconnectToNewHost);

            if (waitingToBecomeNewHost)
            {
                BecomeNewHost(NetworkManager.singleton.networkPort);
            }
            else if (waitingReconnectToNewHost)
            {
                Reset(oldServerConnectionId);
                Reset(ClientScene.ReconnectIdHost);
                NetworkManager.singleton.networkAddress = newHostAddress;
                NetworkManager.singleton.client.ReconnectToNewHost(newHostAddress, NetworkManager.singleton.networkPort);
            }
        }
    }
}
