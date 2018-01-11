#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace MovementPlus
{

    [CustomEditor(typeof(BezierCurve)), CanEditMultipleObjects]
    public class BezierCurveEditor : Editor
    {

        SerializedProperty pointsProp;
        SerializedProperty closedProp;
        SerializedProperty curveColorProp;
        SerializedProperty resolutionProp;
        SerializedProperty preciseHandlesProp;

        void OnEnable()
        {
            pointsProp = serializedObject.FindProperty("points");
            closedProp = serializedObject.FindProperty("closed");
            curveColorProp = serializedObject.FindProperty("curveColor");
            resolutionProp = serializedObject.FindProperty("resolution");
            preciseHandlesProp = serializedObject.FindProperty("preciseHandles");
        }

        public override void OnInspectorGUI()
        {
            //DrawDefaultInspector();

            BezierCurve bc = (BezierCurve)target;

            serializedObject.Update();

            EditorGUILayout.PropertyField(curveColorProp, new GUIContent("Curve Color"));

            if (resolutionProp.hasMultipleDifferentValues)
            {
                EditorGUILayout.PropertyField(resolutionProp, new GUIContent("Resolution"));
            }
            else
            {
                resolutionProp.intValue = EditorGUILayout.IntSlider("Resolution", resolutionProp.intValue, 0, 64);
            }
            bc.updateCurveLength();
            EditorGUILayout.LabelField("Curve Length: " + bc.getCurveLength());
            EditorGUILayout.PropertyField(closedProp, new GUIContent("Closed"));
            EditorGUILayout.PropertyField(preciseHandlesProp, new GUIContent("Precise Handles"));
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Curve Points:");
            EditorGUI.indentLevel++;

            if (!pointsProp.hasMultipleDifferentValues)
            {
                for (int i = 0; i < pointsProp.arraySize; i++)
                {
                    if (pointsProp.GetArrayElementAtIndex(i).objectReferenceValue == null) continue;
                    EditorGUILayout.BeginHorizontal();
                    GameObject point = (GameObject)pointsProp.GetArrayElementAtIndex(i).objectReferenceValue;
                    point.transform.position = EditorGUILayout.Vector3Field("Point " + i, point.transform.position);
                    if (GUILayout.Button("+", GUILayout.MaxWidth(24), GUILayout.MaxHeight(20)))
                    {
                        //bc.addPoint(pointsProp, i);
                        serializedObject.ApplyModifiedPropertiesWithoutUndo();
                        serializedObject.Update();
                    }
                    if (pointsProp.arraySize <= 2) EditorGUI.BeginDisabledGroup(true);
                    if (GUILayout.Button("-", GUILayout.MaxWidth(24), GUILayout.MaxHeight(20)))
                    {
                        //bc.removePoint(pointsProp, i);
                        serializedObject.ApplyModifiedPropertiesWithoutUndo();
                        serializedObject.Update();
                    }
                    if (pointsProp.arraySize <= 2) EditorGUI.EndDisabledGroup();

                    EditorGUILayout.EndHorizontal();
                }

                //if (GUILayout.Button("Check Children", GUILayout.MaxWidth(100), GUILayout.MaxHeight(24))) bc.checkChildren(pointsProp);

            }
            else
            {
                EditorGUILayout.LabelField("Point Array does not support Multi Object Editing!");
            }

            serializedObject.ApplyModifiedProperties();
            EditorGUI.indentLevel--;
        }

        void OnDrawGizmos()
        {
            BezierCurve bc = (BezierCurve)target;
            Handles.color = bc.curveColor;
            Gizmos.color = bc.curveColor;

            for (int i = 0; i < bc.points.Count; i++)
            {
                Handles.CubeCap(1, bc.points[i].transform.position, bc.points[i].transform.rotation, 0.4f);
            }
        }

        public void OnSceneGUI()
        {
            /*BezierCurve bc = (BezierCurve)target;

            for (int i = 0; i < bc.points.Count; i++)
            {
                if (bc.points[i] == null) return;
                pointGUI(bc.points[i].GetComponent<BezierPoint>());
            }*/

            if (pointsProp == null) return;
            for (int i = 0; i < pointsProp.arraySize; i++)
            {
                GameObject point = (GameObject)pointsProp.GetArrayElementAtIndex(i).objectReferenceValue;
                if (point == null) continue;
                pointGUI(point.GetComponent<BezierPoint>());
            }

        }

        public void pointGUI(BezierPoint bp)
        {
            BezierCurve bc = (BezierCurve)target;

            Quaternion rot = bp.transform.rotation;

            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.white;

            Handles.Label(bp.transform.position + new Vector3(0, 2, 0), bp.gameObject.name, style);

            Handles.color = bc.curveColor;

            EditorGUI.BeginChangeCheck();
            Vector3 pos = bc.transform.position;
            if (bc.preciseHandles)
            {
                pos = Handles.PositionHandle(bp.transform.position, bp.transform.rotation);
                if (Tools.current == Tool.Rotate)
                {
                    rot = Handles.RotationHandle(bp.transform.rotation, bp.transform.position);
                }
            }
            else
            {
                pos = Handles.FreeMoveHandle(bp.transform.position, bp.transform.rotation, 0.4f, new Vector3(.5f, .5f, .5f), Handles.SphereCap);
            }
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(bp.transform, "Free Move Point");
                bp.transform.position = pos;
                bp.transform.rotation = rot;
            }

            Handles.DrawDottedLine(bp.transform.position, bc.transform.TransformPoint(bp.handle0 + bp.transform.localPosition), 5f);
            Handles.DrawDottedLine(bp.transform.position, bc.transform.TransformPoint(bp.handle1 + bp.transform.localPosition), 5f);

            EditorGUI.BeginChangeCheck();
            Vector3 pos1 = Vector3.zero;
            if (bc.preciseHandles)
            {
                if (Tools.current != Tool.Rotate)
                {
                    pos1 = Handles.PositionHandle(bc.transform.TransformPoint(bp.handle0 + bp.transform.localPosition), Quaternion.identity);
                }
            }
            else
            {
                pos1 = Handles.FreeMoveHandle(bc.transform.TransformPoint(bp.handle0 + bp.transform.localPosition), Quaternion.identity, 0.2f, new Vector3(.5f, .5f, .5f), Handles.CircleCap);
            }
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(bp, "Free Move Handle 0");
                bp.handle0 = bc.transform.InverseTransformPoint(pos1) - bp.transform.localPosition;
                bp.handle1 = -bp.handle0;
            }

            EditorGUI.BeginChangeCheck();
            Vector3 pos2 = Vector3.zero;
            if (bc.preciseHandles)
            {
                if (Tools.current != Tool.Rotate)
                {
                    pos2 = Handles.PositionHandle(bc.transform.TransformPoint(bp.handle1 + bp.transform.localPosition), Quaternion.identity);
                }
            }
            else
            {
                pos2 = Handles.FreeMoveHandle(bc.transform.TransformPoint(bp.handle1 + bp.transform.localPosition), Quaternion.identity, 0.2f, new Vector3(.5f, .5f, .5f), Handles.CircleCap);
            }
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(bp, "Free Move Handle 1");
                bp.handle1 = bc.transform.InverseTransformPoint(pos2) - bp.transform.localPosition;
                bp.handle0 = -bp.handle1;
            }
        }

    }
}
#endif
