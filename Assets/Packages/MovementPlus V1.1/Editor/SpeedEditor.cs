#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

namespace MovementPlus
{

    [CustomEditor(typeof(SpeedController)), CanEditMultipleObjects]
    public class SpeedEditor : Editor
    {

        public string[] tweenOptions = { "None", "Linear", "Sine", "Smoothstep x2", "Smoothstep x3" };

        SerializedProperty neutralSpeedProp;
        SerializedProperty targetSpeedProp;

        SerializedProperty tweenIndexProp;
        SerializedProperty tweenDurationProp;
        SerializedProperty intervalProp;

        SerializedProperty intervalTimeProp;
        SerializedProperty durationProp;

        void OnEnable()
        {
            neutralSpeedProp = serializedObject.FindProperty("neutralSpeed");
            targetSpeedProp = serializedObject.FindProperty("targetSpeed");
            tweenIndexProp = serializedObject.FindProperty("tweenIndex");
            tweenDurationProp = serializedObject.FindProperty("tweenDuration");
            intervalProp = serializedObject.FindProperty("interval");
            intervalTimeProp = serializedObject.FindProperty("intervalTime");
            durationProp = serializedObject.FindProperty("duration");
        }

        public override void OnInspectorGUI()
        {
            //DrawDefaultInspector();
            serializedObject.Update();

            SpeedController sc = (SpeedController)target;

            if (sc.transform.GetComponent<MovementController>() == null)
            {
                GUI.contentColor = Color.red;
                EditorGUILayout.LabelField("There must be a MovementController Script added!", EditorStyles.whiteLabel);
                GUI.contentColor = Color.white;
            }
            else
            {
                EditorGUI.BeginDisabledGroup(sc.interval == true);
                if (GUILayout.Button("Toggle"))
                {
                    sc.toggle();
                }
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.PropertyField(neutralSpeedProp, new GUIContent("Neutral Speed"));

                EditorGUILayout.PropertyField(targetSpeedProp, new GUIContent("Target Speed"));

                if (!tweenIndexProp.hasMultipleDifferentValues)
                {
                    tweenIndexProp.intValue = EditorGUILayout.Popup("Tween Type", tweenIndexProp.intValue, tweenOptions);
                }
                else
                {
                    EditorGUILayout.LabelField("Selection contains different Tween Types!");
                }


                if (tweenIndexProp.intValue != 0)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(tweenDurationProp, new GUIContent("Tween Duration"));
                    if (tweenDurationProp.floatValue < 0)
                    {
                        tweenDurationProp.floatValue = 0;
                    }
                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.PropertyField(intervalProp, new GUIContent("Interval"));
                if (intervalProp.boolValue)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(intervalTimeProp, new GUIContent("Neutral Duration"));
                    if (intervalTimeProp.floatValue < 0)
                    {
                        intervalTimeProp.floatValue = 0;
                    }
                    EditorGUILayout.PropertyField(durationProp, new GUIContent("Target Duration"));
                    if (durationProp.floatValue < 0)
                    {
                        durationProp.floatValue = 0;
                    }
                    EditorGUI.indentLevel--;
                }
            }

            serializedObject.ApplyModifiedProperties();

        }

    }
}
#endif
