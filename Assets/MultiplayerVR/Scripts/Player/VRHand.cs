using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using System.Collections;

namespace MultiplayerVR.Player
{
    public enum HandSide
    {
        Left,
        Right
    }

    public class VRHand : NetworkBehaviour
    {
        [SyncVar]
        public HandSide HandSide;

        [SyncVar]
        public NetworkInstanceId OwnerId;

        [SyncVar]
        public bool Trigger;

        public GameObject Prefab;
        GameObject localPlayer;
        GameObject trackedController;
        SteamVR_Controller.Device steamDevice;
        public GameObject Bullet;

        MultiplayerVR.Bootstrap.CommandLineParser constantMain;

        public override void OnStartClient()
        {
            localPlayer = ClientScene.FindLocalObject(OwnerId);
            transform.position = localPlayer.transform.position;
            transform.rotation = localPlayer.transform.rotation;
            transform.parent = localPlayer.transform;
        }

        public override void OnStartAuthority()
        {
            trackedController = GameObject.Find(string.Format("Controller ({0})", HandSide.ToString("G").ToLowerInvariant()));
            transform.position = trackedController.transform.position;
            transform.rotation = trackedController.transform.rotation;
            transform.parent = trackedController.transform;
            steamDevice = SteamVR_Controller.Input((int)trackedController.GetComponent<SteamVR_TrackedObject>().index);
            CmdSpawnTool(HandSide);
        }

        void Start()
        {
            Vector3 pos = transform.position;
            transform.position = new Vector3(pos.x, pos.y + 0.025f, pos.z + 0.025f);

            constantMain = GameObject.Find("Bootstrap").GetComponent<MultiplayerVR.Bootstrap.CommandLineParser>();
        }

        void Update()
        {
            if (!hasAuthority)
            {
                //Debug.LogFormat("Trigger{0}: {1}", netId, Trigger);
                return;
            }

            if (steamDevice.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger))
            {
                //Debug.LogFormat("trigger down {0}", OwnerId);
                CmdTriggerPress(true);
                steamDevice.TriggerHapticPulse(500, Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad);
                Bullet.SetActive(true);
            }
            else if (steamDevice.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger))
            {
                //Debug.LogFormat("trigger up {0}", OwnerId);
                CmdTriggerPress(false);
                Bullet.SetActive(false);
            }
            else if (steamDevice.GetPress(SteamVR_Controller.ButtonMask.Trigger))
            {
                //Debug.LogFormat("trigger pressing {0}", OwnerId);
                steamDevice.TriggerHapticPulse(100, Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad);
            }
        }

        [Command]
        void CmdSpawnTool(HandSide handSide)
        {
            var hoge = (GameObject)Instantiate(Prefab, transform);
            hoge.transform.localPosition = Vector3.zero;
            hoge.transform.localRotation = Quaternion.identity;
            hoge.GetComponent<Fude>().OwnerId = netId;
            NetworkServer.SpawnWithClientAuthority(hoge, NetworkServer.FindLocalObject(OwnerId));
        }

        [Command]
        void CmdTriggerPress(bool press)
        {
            Trigger = press;
            RpcTriggerPress();
        }

        [ClientRpc]
        void RpcTriggerPress()
        {
            if (constantMain.TargetRole == MultiplayerVR.Bootstrap.CommandLineParser.Role.Host)
            {
                if (Trigger == true)
                {
                    Bullet.SetActive(true);
                }
                if (Trigger == false)
                {
                    Bullet.SetActive(false);
                }
            }

            if (constantMain.TargetRole == MultiplayerVR.Bootstrap.CommandLineParser.Role.Client)
            {
                if (Trigger == false)
                {
                    Bullet.SetActive(true);
                }
                if (Trigger == true)
                {
                    Bullet.SetActive(false);
                }
            }
        }
    }
}
