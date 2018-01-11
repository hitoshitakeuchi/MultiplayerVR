#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;

namespace MovementPlus
{

    [ExecuteInEditMode]
    public class SceneViewCameraController : MonoBehaviour
    {

        public bool active = false;

        private bool doUpdate = false;

        void OnEnable()
        {
            if (!Application.isPlaying) EditorApplication.update += EditorUpdate;
            doUpdate = true;
        }

        void OnDisable()
        {
            doUpdate = false;
        }

        void EditorUpdate()
        {
            if (!doUpdate) return;
            if (!active) return;
            Vector3 position = SceneView.lastActiveSceneView.pivot;
            Quaternion rotation = SceneView.lastActiveSceneView.rotation;
            //SceneView.lastActiveSceneView.size = 0;
            position = transform.position;
            rotation = transform.rotation;
            SceneView.lastActiveSceneView.pivot = position;
            SceneView.lastActiveSceneView.rotation = rotation;
            SceneView.lastActiveSceneView.Repaint();
        }
    }
}
#endif