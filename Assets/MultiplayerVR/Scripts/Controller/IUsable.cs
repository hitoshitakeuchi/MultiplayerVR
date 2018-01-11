using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

namespace MultiplayerVR
{
    public interface IUsable
    {
        void StartUsing(NetworkInstanceId handId);
        void StopUsing(NetworkInstanceId handId);
    }
}