using UnityEditor;
using UnityEngine;

namespace MovementPlus
{

    [CanEditMultipleObjects]
    public class MovementPlusWindow : EditorWindow
    {

        private bool stopped = true;

        [MenuItem("Tools/MovementPlus")]
        static void Init()
        {
            EditorWindow window = EditorWindow.GetWindow(typeof(MovementPlusWindow));
            //if (window == null) window = new SceneGUI();
            window.titleContent = new GUIContent("MovementPlus");
            window.Show();
        }

        public void OnGUI()
        {
            if (stopped)
            {
                if (GUILayout.Button("Start All", GUILayout.MaxWidth(80), GUILayout.MaxHeight(32)))
                {
                    stopped = false;
                    MovementController[] objects = FindObjectsOfType<MovementController>();
                    for (int i = 0; i < objects.Length; i++)
                    {
                        objects[i].simulate = true;
                    }
                }
            }
            else
            {
                if (GUILayout.Button("Stop All", GUILayout.MaxWidth(80), GUILayout.MaxHeight(32)))
                {
                    stopped = true;
                    MovementController[] objects = FindObjectsOfType<MovementController>();
                    for (int i = 0; i < objects.Length; i++)
                    {
                        objects[i].simulate = false;

                        objects[i].time = 0;
                        objects[i].transform.localPosition = objects[i].defaultPosition;
                        objects[i].transform.localRotation = objects[i].defaultRotation;
                        //objects[i].transform.localScale = objects[i].defaultScale;

                        if (objects[i].transform.GetComponent<SpeedController>() != null)
                        {
                            objects[i].transform.GetComponent<SpeedController>().reset();
                        }
                    }
                }
            }

            /*if (GUILayout.Button("Reset All", GUILayout.MaxWidth(80), GUILayout.MaxHeight(32)))
            {
                MovementController[] objects = FindObjectsOfType<MovementController>();
                for (int i = 0; i < objects.Length; i++)
                {
                    objects[i].time = 0;
                    objects[i].transform.position = objects[i].defaultPosition;
                    objects[i].transform.rotation = objects[i].defaultRotation;
                    objects[i].transform.localScale = objects[i].defaultScale;

                    if(objects[i].transform.GetComponent<SpeedController>() != null)
                    {
                        objects[i].transform.GetComponent<SpeedController>().reset();
                    }
                }
            }*/
        }
    }
}
