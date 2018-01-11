using UnityEngine;

namespace MultiplayerVR.Network
{
    [RequireComponent(typeof(VRNetworkManager))]
    public class VRNetworkManagerHUD : MonoBehaviour
    {
        public bool ShowGUI = true;

        public int OffsetX;

        public int OffsetY;

        const int WIDTH = 150;

        const int HEIGHT = 20;

        void OnGUI()
        {
            if (!ShowGUI)
            {
                return;
            }

            int oy = OffsetY - HEIGHT;
            int dy = 30;

            VRNetworkManager networkManager = GetComponent<VRNetworkManager>();

            if (GUI.Button(new Rect(OffsetX, oy += dy, WIDTH, HEIGHT), "Start Host"))
            {
                networkManager.StartHost();
            }

            if (GUI.Button(new Rect(OffsetX, oy += dy, WIDTH, HEIGHT), "Stop Host"))
            {
                networkManager.StopHost();
            }

            if (GUI.Button(new Rect(OffsetX, oy += dy, WIDTH, HEIGHT), "Start Client"))
            {
                networkManager.StartClient();
            }

            if (GUI.Button(new Rect(OffsetX, oy += dy, WIDTH, HEIGHT), "Stop Client"))
            {
                networkManager.StopClient();
            }
        }
    }
}
