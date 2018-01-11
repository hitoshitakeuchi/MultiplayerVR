#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using System.Collections;

namespace MovementPlus
{

    [CustomEditor(typeof(MovementController)), CanEditMultipleObjects]
    public class MovementEditor : Editor
    {

        public string[] positionAnimOptions = new string[] { "Linear", "Sine", "Circular", "Follow Target", "Bezier Curve", "Perlin Noise" };
        public string[] rotationAnimOptions = new string[] { "Linear", "Look at Target", "Bezier Nodes", "Look along Curve" };

        public string[] loopOptions = new string[] { "None", "Loop", "Bounce" };

        AnimBool showPositionFields;
        AnimBool showRotationFields;

        private static Texture logo;

        SerializedProperty gizmoPositionColorProp;
        SerializedProperty positionAsOffsetProp;

        SerializedProperty timeScaleProp;
        SerializedProperty animatePositionProp;
        SerializedProperty animateRotationProp;

        SerializedProperty positionAnimIndexProp;
        SerializedProperty rotationAnimIndexProp;

        SerializedProperty posAProp, posBProp;
        SerializedProperty posFrequencyProp;
        SerializedProperty posPhaseShiftProp;
        SerializedProperty loopIndexProp;

        SerializedProperty positionCircleCenterProp;
        SerializedProperty positionCircleRadiusProp;
        SerializedProperty positionCircleZenithProp;
        SerializedProperty positionCircleAzimuthProp;

        SerializedProperty positionTargetProp;
        SerializedProperty positionHardnessProp;
        SerializedProperty velocityApproachProp;
        SerializedProperty maxVelocityProp;
        SerializedProperty accelerationProp;
        SerializedProperty snapRangeProp;
        SerializedProperty snapHardnessProp;

        SerializedProperty bezierCurveProp;
        SerializedProperty bezierSpeedProp;
        SerializedProperty bezierConstSpeedProp;
        SerializedProperty bezierPhaseShiftProp;

        SerializedProperty perlinAmplitudeProp;
        SerializedProperty perlinSynchronProp;

        SerializedProperty rotationAnglesProp;
        SerializedProperty rotationTargetProp;
        SerializedProperty rotationHardnessProp;
        SerializedProperty curvePredictionProp;

        void OnEnable()
        {
            if (!logo) logo = (Texture)Resources.Load("MovementPlusLogo128");

            showPositionFields = new AnimBool(true);
            showPositionFields.valueChanged.AddListener(Repaint);

            showRotationFields = new AnimBool(true);
            showRotationFields.valueChanged.AddListener(Repaint);

            gizmoPositionColorProp = serializedObject.FindProperty("gizmoPositionColor");
            positionAsOffsetProp = serializedObject.FindProperty("positionAsOffset");
            timeScaleProp = serializedObject.FindProperty("timeScale");

            animatePositionProp = serializedObject.FindProperty("animatePosition");
            animateRotationProp = serializedObject.FindProperty("animateRotation");

            positionAnimIndexProp = serializedObject.FindProperty("positionAnimIndex");
            rotationAnimIndexProp = serializedObject.FindProperty("rotationAnimIndex");
            velocityApproachProp = serializedObject.FindProperty("velocityApproach");
            maxVelocityProp = serializedObject.FindProperty("maxVelocity");
            accelerationProp = serializedObject.FindProperty("acceleration");
            snapRangeProp = serializedObject.FindProperty("snapRange");
            snapHardnessProp = serializedObject.FindProperty("snapHardness");

            posAProp = serializedObject.FindProperty("posA");
            posBProp = serializedObject.FindProperty("posB");
            posFrequencyProp = serializedObject.FindProperty("posFrequency");
            posPhaseShiftProp = serializedObject.FindProperty("posPhaseShift");
            loopIndexProp = serializedObject.FindProperty("loopIndex");

            positionCircleCenterProp = serializedObject.FindProperty("positionCircleCenter");
            positionCircleRadiusProp = serializedObject.FindProperty("positionCircleRadius");
            positionCircleZenithProp = serializedObject.FindProperty("positionCircleZenith");
            positionCircleAzimuthProp = serializedObject.FindProperty("positionCircleAzimuth");

            positionTargetProp = serializedObject.FindProperty("positionTarget");
            positionHardnessProp = serializedObject.FindProperty("positionHardness");

            bezierCurveProp = serializedObject.FindProperty("bezierCurve");
            bezierSpeedProp = serializedObject.FindProperty("bezierSpeed");
            bezierConstSpeedProp = serializedObject.FindProperty("bezierConstSpeed");
            bezierPhaseShiftProp = serializedObject.FindProperty("bezierPhaseShift");

            perlinAmplitudeProp = serializedObject.FindProperty("perlinAmplitude");
            perlinSynchronProp = serializedObject.FindProperty("perlinSynchron");

            rotationAnglesProp = serializedObject.FindProperty("rotationAngles");
            rotationTargetProp = serializedObject.FindProperty("rotationTarget");
            rotationHardnessProp = serializedObject.FindProperty("rotationHardness");
            curvePredictionProp = serializedObject.FindProperty("curvePrediction");
        }

        public override void OnInspectorGUI()
        {
            //DrawDefaultInspector();

            EditorGUI.BeginChangeCheck();

            MovementController mc = (MovementController)target;
            serializedObject.Update();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.PrefixLabel("General Parameters:");
            EditorGUILayout.PropertyField(gizmoPositionColorProp, new GUIContent("Gizmo Color"));
            EditorGUILayout.PropertyField(positionAsOffsetProp, new GUIContent("Use position as offset"));
            EditorGUILayout.EndVertical();
            GUILayout.Label(logo, GUILayout.MaxWidth(64), GUILayout.MaxHeight(64));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.PropertyField(timeScaleProp, new GUIContent("Time Scale"));

            if (mc.simulate)
            {
                if (GUILayout.Button("Stop Simulation"))
                {
                    mc.simulate = false;
                    mc.time = 0f;
                    mc.transform.position = mc.defaultPosition;
                    mc.transform.rotation = mc.defaultRotation;
                }
            }
            else
            {
                if (GUILayout.Button("Start Simulation"))
                {
                    mc.simulate = true;
                    mc.time = 0f;
                    mc.defaultPosition = mc.transform.position;
                    mc.defaultRotation = mc.transform.rotation;
                }
            }

            EditorGUILayout.Space();

            //->POSITION
            animatePositionProp.boolValue = EditorGUILayout.ToggleLeft("Animate Position:", animatePositionProp.boolValue);
            showPositionFields.target = animatePositionProp.boolValue;
            if (EditorGUILayout.BeginFadeGroup(showPositionFields.faded))
            {
                EditorGUI.indentLevel++;

                if (!positionAnimIndexProp.hasMultipleDifferentValues)
                {
                    positionAnimIndexProp.intValue = EditorGUILayout.Popup("Animation Type", positionAnimIndexProp.intValue, positionAnimOptions);

                    switch (positionAnimIndexProp.intValue)
                    {
                        case 0://Linear
                            EditorGUILayout.PropertyField(posAProp, new GUIContent("Position A"));
                            EditorGUILayout.PropertyField(posBProp, new GUIContent("Position B"));
                            if (posFrequencyProp.hasMultipleDifferentValues)
                            {
                                EditorGUILayout.PropertyField(posFrequencyProp, new GUIContent("% of Path"));
                            }
                            else
                            {
                                posFrequencyProp.floatValue = EditorGUILayout.Slider("% of Path", posFrequencyProp.floatValue, 0, 1);
                            }

                            if (!loopIndexProp.hasMultipleDifferentValues)
                            {
                                loopIndexProp.intValue = EditorGUILayout.Popup("Loop Type", loopIndexProp.intValue, loopOptions);
                            }
                            else
                            {
                                EditorGUILayout.LabelField("Selection contains different Loop Types!");
                            }

                            break;
                        case 1://Sine
                            EditorGUILayout.PropertyField(posAProp, new GUIContent("Position A"));
                            EditorGUILayout.PropertyField(posBProp, new GUIContent("Position B"));
                            if (posPhaseShiftProp.hasMultipleDifferentValues)
                            {
                                EditorGUILayout.PropertyField(posPhaseShiftProp, new GUIContent("Phase Shift"));
                            }
                            else
                            {
                                posPhaseShiftProp.floatValue = EditorGUILayout.Slider("Phase Shift", posPhaseShiftProp.floatValue, 0, 1);
                            }
                            break;
                        case 2://Circle
                            EditorGUILayout.PropertyField(positionCircleCenterProp, new GUIContent("Center"));
                            EditorGUILayout.PropertyField(positionCircleRadiusProp, new GUIContent("Radius"));
                            if (positionCircleZenithProp.hasMultipleDifferentValues)
                            {
                                EditorGUILayout.PropertyField(positionCircleZenithProp, new GUIContent("Zenith Angle"));
                            }
                            else
                            {
                                positionCircleZenithProp.floatValue = EditorGUILayout.Slider("Zenith Angle", positionCircleZenithProp.floatValue, 0, 180f);
                            }

                            if (positionCircleAzimuthProp.hasMultipleDifferentValues)
                            {
                                EditorGUILayout.PropertyField(positionCircleAzimuthProp, new GUIContent("Azimuth Angle"));
                            }
                            else
                            {
                                positionCircleAzimuthProp.floatValue = EditorGUILayout.Slider("Azimuth Angle", positionCircleAzimuthProp.floatValue, 0, 180f);
                            }

                            if (posFrequencyProp.hasMultipleDifferentValues)
                            {
                                EditorGUILayout.PropertyField(posFrequencyProp, new GUIContent("% of Path"));
                            }
                            else
                            {
                                posFrequencyProp.floatValue = EditorGUILayout.Slider("% of Path", posFrequencyProp.floatValue, 0, 1);
                            }

                            if (!loopIndexProp.hasMultipleDifferentValues)
                            {
                                loopIndexProp.intValue = EditorGUILayout.Popup("Loop Type", loopIndexProp.intValue, loopOptions);
                            }
                            else
                            {
                                EditorGUILayout.LabelField("Selection contains different Loop Types!");
                            }

                            if (posPhaseShiftProp.hasMultipleDifferentValues)
                            {
                                EditorGUILayout.PropertyField(posPhaseShiftProp, new GUIContent("Phase Shift"));
                            }
                            else
                            {
                                posPhaseShiftProp.floatValue = EditorGUILayout.Slider("Phase Shift", posPhaseShiftProp.floatValue, 0, 1);
                            }
                            break;
                        case 3://Follow Target
                            EditorGUILayout.PropertyField(positionTargetProp, new GUIContent("Target Transform"));
                            EditorGUI.BeginDisabledGroup(velocityApproachProp.boolValue);
                            if (positionHardnessProp.hasMultipleDifferentValues)
                            {
                                EditorGUILayout.PropertyField(positionHardnessProp, new GUIContent("Hardness"));
                            }
                            else
                            {
                                positionHardnessProp.floatValue = EditorGUILayout.Slider("Hardness", positionHardnessProp.floatValue, 0, 1);
                            }
                            EditorGUI.EndDisabledGroup();

                            EditorGUILayout.PropertyField(velocityApproachProp, new GUIContent("Velocity Approach"));

                            EditorGUI.BeginDisabledGroup(!velocityApproachProp.boolValue);
                            EditorGUI.indentLevel++;

                            EditorGUILayout.PropertyField(maxVelocityProp, new GUIContent("Maximum Velocity"));
                            if (maxVelocityProp.floatValue < 0) maxVelocityProp.floatValue = 0f;
                            EditorGUILayout.PropertyField(accelerationProp, new GUIContent("Acceleration"));
                            if (accelerationProp.floatValue < 0) accelerationProp.floatValue = 0f;
                            EditorGUILayout.PropertyField(snapRangeProp, new GUIContent("Snap Range"));
                            if (snapRangeProp.floatValue < 0) snapRangeProp.floatValue = 0f;
                            if (snapHardnessProp.hasMultipleDifferentValues)
                            {
                                EditorGUILayout.PropertyField(snapHardnessProp, new GUIContent("Snap Hardness"));
                            }
                            else
                            {
                                snapHardnessProp.floatValue = EditorGUILayout.Slider("Snap Hardness", snapHardnessProp.floatValue, 0, 1);
                            }

                            EditorGUI.indentLevel--;
                            EditorGUI.EndDisabledGroup();

                            break;
                        case 4://Bezier
                            EditorGUILayout.PropertyField(bezierCurveProp, new GUIContent("Curve Object"));
                            if (bezierSpeedProp.hasMultipleDifferentValues)
                            {
                                EditorGUILayout.PropertyField(bezierSpeedProp, new GUIContent("% of Path"));
                            }
                            else
                            {
                                bezierSpeedProp.floatValue = EditorGUILayout.Slider("% of Path", bezierSpeedProp.floatValue, 0, 1);
                            }
                            EditorGUI.BeginDisabledGroup(true);
                            EditorGUILayout.PropertyField(bezierConstSpeedProp, new GUIContent("Constant Velocity"));
                            if (bezierConstSpeedProp.boolValue == true) bezierConstSpeedProp.boolValue = false;
                            EditorGUI.EndDisabledGroup();
                            if (!loopIndexProp.hasMultipleDifferentValues)
                            {
                                loopIndexProp.intValue = EditorGUILayout.Popup("Loop Type", loopIndexProp.intValue, loopOptions);
                            }
                            else
                            {
                                EditorGUILayout.LabelField("Selection contains different Loop Types!");
                            }

                            if (bezierPhaseShiftProp.hasMultipleDifferentValues)
                            {
                                EditorGUILayout.PropertyField(bezierPhaseShiftProp, new GUIContent("Phase Shift"));
                            }
                            else
                            {
                                bezierPhaseShiftProp.floatValue = EditorGUILayout.Slider("Phase Shift", bezierPhaseShiftProp.floatValue, 0, 1);
                            }
                            break;
                        case 5://Perlin Noise
                            EditorGUILayout.PropertyField(perlinAmplitudeProp, new GUIContent("Amplitude"));
                            EditorGUILayout.PropertyField(perlinSynchronProp, new GUIContent("Synchronized"));
                            break;
                        default://Default
                            Debug.LogError("Unrecognized Option");
                            break;
                    }
                }
                else
                {
                    EditorGUILayout.LabelField("Selection contains different Animation Types!");
                }
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndFadeGroup();
            EditorGUILayout.Space();

            //->ROTATION
            animateRotationProp.boolValue = EditorGUILayout.ToggleLeft("Animate Rotation:", animateRotationProp.boolValue);
            showRotationFields.target = animateRotationProp.boolValue;

            if (EditorGUILayout.BeginFadeGroup(showRotationFields.faded))
            {
                EditorGUI.indentLevel++;

                if (!rotationAnimIndexProp.hasMultipleDifferentValues)
                {

                    rotationAnimIndexProp.intValue = EditorGUILayout.Popup("Animation Type", rotationAnimIndexProp.intValue, rotationAnimOptions);

                    switch (rotationAnimIndexProp.intValue)
                    {
                        case 0://Linear
                            EditorGUI.BeginDisabledGroup(true);
                            EditorGUILayout.Toggle("Bound to Movement", true);
                            EditorGUI.EndDisabledGroup();

                            EditorGUILayout.PropertyField(rotationAnglesProp, new GUIContent("Angles"));
                            break;
                        case 1://LookAtTarget
                            EditorGUILayout.PropertyField(rotationTargetProp, new GUIContent("Target Transform"));
                            if (rotationHardnessProp.hasMultipleDifferentValues)
                            {
                                EditorGUILayout.PropertyField(rotationHardnessProp, new GUIContent("Hardness"));
                            }
                            else
                            {
                                rotationHardnessProp.floatValue = EditorGUILayout.Slider("Hardness", rotationHardnessProp.floatValue, 0, 1);
                            }
                            break;
                        case 2://Bezier Nodes
                            if (positionAnimIndexProp.intValue != 4)
                            {
                                GUI.contentColor = Color.red;
                                EditorGUILayout.LabelField("Position Animation must be set to Bezier Curve!", EditorStyles.whiteLabel);
                                GUI.contentColor = Color.white;
                            }
                            break;
                        case 3://LookAlongCurve
                            EditorGUILayout.PropertyField(curvePredictionProp, new GUIContent("Curve Prediction Amount"));
                            if (curvePredictionProp.floatValue <= 0) curvePredictionProp.floatValue = 0.0001f;
                            if (rotationHardnessProp.hasMultipleDifferentValues)
                            {
                                EditorGUILayout.PropertyField(rotationHardnessProp, new GUIContent("Hardness"));
                            }
                            else
                            {
                                rotationHardnessProp.floatValue = EditorGUILayout.Slider("Hardness", rotationHardnessProp.floatValue, 0, 1);
                            }
                            break;
                        default://Default
                            Debug.LogError("Unrecognized Option");
                            break;
                    }

                }
                else
                {
                    EditorGUILayout.LabelField("Selection contains different Animation Types!");
                }
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndFadeGroup();
            EditorGUILayout.Space();

            serializedObject.ApplyModifiedProperties();

            if (EditorGUI.EndChangeCheck())
            {
                SceneView.RepaintAll();
            }
        }

        public void OnSceneGUI()
        {
            MovementController mc = (MovementController)target;

            //serializedObject.Update();

            if (mc.animatePosition && (mc.positionAnimIndex == 0 || mc.positionAnimIndex == 1))
            {
                Vector3 offset = Vector3.zero;
                if (mc.positionAsOffset)
                {
                    offset += mc.defaultPosition;
                }
                if (mc.transform.parent != null)
                {
                    EditorGUI.BeginChangeCheck();
                    Vector3 pos1 = Vector3.zero;
                    if (Tools.current != Tool.Rotate)
                    {
                        pos1 = Handles.PositionHandle(mc.transform.parent.TransformPoint(mc.posA), Quaternion.identity);
                    }

                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(mc, "Move Handle A");
                        mc.posA = mc.transform.parent.InverseTransformPoint(pos1);
                    }


                    EditorGUI.BeginChangeCheck();
                    Vector3 pos2 = Vector3.zero;
                    if (Tools.current != Tool.Rotate)
                    {
                        pos2 = Handles.PositionHandle(mc.transform.parent.TransformPoint(mc.posB), Quaternion.identity);
                    }

                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(mc, "Move Handle B");
                        mc.posB = mc.transform.parent.InverseTransformPoint(pos2);
                    }
                }
                else
                {
                    EditorGUI.BeginChangeCheck();
                    Vector3 pos1 = Vector3.zero;
                    if (Tools.current != Tool.Rotate)
                    {
                        pos1 = Handles.PositionHandle(mc.posA + offset, Quaternion.identity);
                    }

                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(mc, "Move Handle A");
                        mc.posA = pos1 - offset;
                    }


                    EditorGUI.BeginChangeCheck();
                    Vector3 pos2 = Vector3.zero;
                    if (Tools.current != Tool.Rotate)
                    {
                        pos2 = Handles.PositionHandle(mc.posB + offset, Quaternion.identity);
                    }

                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(mc, "Move Handle B");
                        mc.posB = pos2 - offset;
                    }
                }



            }
        }

    }

}
#endif
