using MultiplayerVR.Network;
using NDesk.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerVR.Bootstrap
{
    public class CommandLineParser : MonoBehaviour
    {
        public enum Role
        {
            Server,
            Host,
            Client,
        }

        public Role TargetRole = Role.Host;

        public VRNetworkManager NetworkManager;

        void Start()
        {
            var optionSet = new OptionSet()
                .Add("r=|role=", role =>
                    TargetRole = (Role)Enum.Parse(typeof(Role), role, true));

            try
            {
                optionSet.Parse(Environment.GetCommandLineArgs());
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
                Application.Quit();
            }

            StartCoroutine(execRoleProcess());
        }

        IEnumerator execRoleProcess()
        {
            yield return new WaitForSeconds(1);

            new Dictionary<Role, Action>()
            {
                { Role.Server, () => NetworkManager.StartServer()},
                { Role.Host, () => NetworkManager.StartHost()},
                { Role.Client, () => NetworkManager.StartClient()},
            }[TargetRole]();
        }
    }
}
