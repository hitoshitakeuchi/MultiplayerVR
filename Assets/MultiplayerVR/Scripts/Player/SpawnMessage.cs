using UnityEngine.Networking;

namespace MultiplayerVR.Player
{
    public class SpawnMessage : MessageBase
    {
        public bool IsVrPlayer;

        public override void Deserialize(NetworkReader reader)
        {
            IsVrPlayer = reader.ReadBoolean();
        }

        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(IsVrPlayer);
        }
    }
}
