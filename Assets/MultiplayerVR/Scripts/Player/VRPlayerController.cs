using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace MultiplayerVR.Player
{
    public class VRPlayerController : NetworkBehaviour
    {
        public GameObject vrCameraRig;
        public GameObject leftHandPrefab;
        public GameObject rightHandPrefab;
        GameObject vrCameraRigInstance;

        public override void OnStartLocalPlayer()
        {
            // delete main camera
            DestroyImmediate(Camera.main.gameObject);

            // create camera rig and attach player model to it
            vrCameraRigInstance = (GameObject)Instantiate(
                vrCameraRig,
                transform.position,
                transform.rotation);

            var head = vrCameraRigInstance.GetComponentInChildren<SteamVR_Camera>().gameObject;
            transform.parent = head.transform;
            transform.localPosition = new Vector3(0f, 0f, -2.0f); // アバターの目線に合わせる

            StartCoroutine(TryDetectControllers());
        }

        IEnumerator TryDetectControllers()
        {
            while (true)
            {
                var controllers = vrCameraRigInstance.GetComponentsInChildren<SteamVR_TrackedObject>() ?? new SteamVR_TrackedObject[] { };

                // FIXME 2個コントローラーが見つからないと表示されない
                if (controllers.Length == 2)
                {
                    CmdSpawnHands(netId);
                    break;
                }

                yield return new WaitForSeconds(2f);
            }
        }

        [Command]
        void CmdSpawnHands(NetworkInstanceId playerId)
        {
            new Dictionary<HandSide, GameObject>
            {
                { HandSide.Left, leftHandPrefab },
                { HandSide.Right, rightHandPrefab },
            }.ToList().ForEach(v => spawnHand((GameObject)Instantiate(v.Value, transform), playerId, v.Key));
        }

        void spawnHand(GameObject hand, NetworkInstanceId playerId, HandSide handSide)
        {
            var vrHand = hand.GetComponent<VRHand>();
            vrHand.HandSide = handSide;
            vrHand.OwnerId = playerId;
            NetworkServer.SpawnWithClientAuthority(hand, connectionToClient);
        }
    }
}
