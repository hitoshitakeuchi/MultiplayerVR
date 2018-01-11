using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerVR
{
    public class Fude : NetworkBehaviour
    {
        [SyncVar]
        public NetworkInstanceId OwnerId;

        [SyncVar]
        public bool Trigger;

        public GameObject Sculpt;

        public override void OnStartClient()
        {
            var owner = ClientScene.FindLocalObject(OwnerId);
            transform.position = owner.transform.position;
            transform.rotation = owner.transform.rotation;
            transform.parent = owner.transform;
        }

        public void CmdTriggerPress(bool press)
        {
            if (Trigger == true)
            {
                Sculpt.SetActive(true);
                Debug.LogFormat("0");
            }
            if (Trigger == false)
            {
                Sculpt.SetActive(false);
                Debug.LogFormat("1");
            }
        }
    }
}
