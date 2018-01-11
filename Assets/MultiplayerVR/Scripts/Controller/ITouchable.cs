using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

namespace MultiplayerVR
{
    public interface ITouchable
    {
        void Touch(NetworkInstanceId handId);
        void Untouch(NetworkInstanceId handId);
    }
}
