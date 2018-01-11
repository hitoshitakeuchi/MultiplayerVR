using UnityEngine;
//using UnityEditor;
using System.Collections;

namespace MovementPlus
{
    [ExecuteInEditMode]
    public class MovementController : MonoBehaviour
    {
        public Vector3 defaultPosition;
        public Quaternion defaultRotation;

        public Color gizmoPositionColor = Color.blue;
        public bool positionAsOffset = true, simulate;

        private bool active = false;

        public float timeScale = 1f;

        public bool animatePosition, animateRotation;
        public int positionAnimIndex, rotationAnimIndex;

        public float posFrequency = 1, posPhaseShift;

        public Vector3 posA, posB;
        public int loopIndex = 1;
        private int reverseTime = 1;

        public Vector3 positionCircleCenter;
        public float positionCircleRadius = 1, positionCircleZenith, positionCircleAzimuth;

        public Transform positionTarget, rotationTarget;
        public float positionHardness, rotationHardness;
        public bool velocityApproach;
        public float maxVelocity, acceleration, snapRange, snapHardness;
        public float velocity;
        private Vector3 lastPosition;

        public BezierCurve bezierCurve;
        public bool bezierConstSpeed = false;
        public float bezierSpeed = 1f;
        public float bezierPhaseShift;

        public Vector3 perlinAmplitude = Vector3.one;
        public bool perlinSynchron = false;
        public Vector3 perlinRandomOffset;

        public Vector3 rotationAngles = new Vector3(1, 0, 0);

        public float curvePrediction;

        public float time = 0f;
        public float deltaTime = 0f;
        private float lastTimeStamp = 0f;

        void OnEnable()
        {

            //if (!Application.isPlaying) EditorApplication.update += EditorUpdate;
            simulate = false;
            active = true;
        }

        void OnDisable()
        {
            active = false;
            simulate = false;
            transform.localPosition = defaultPosition;
            transform.localRotation = defaultRotation;
        }

        void Start()
        {
            time = 0f;
            perlinRandomOffset = Vector3.zero;
        }

        void EditorUpdate()
        {
            if (!active) return;
            if (!Application.isPlaying)
            {
                if (simulate)
                {
                    if (transform.GetComponent<SpeedController>() != null)
                    {
                        transform.GetComponent<SpeedController>().simulate = true;
                    }
                    if ((positionAnimIndex == 0 || positionAnimIndex == 2 || positionAnimIndex == 4) && loopIndex == 2)
                    {
                        time += reverseTime * timeScale * (Time.realtimeSinceStartup - lastTimeStamp);
                    }
                    else
                    {
                        time += timeScale * (Time.realtimeSinceStartup - lastTimeStamp);
                        deltaTime = timeScale * (Time.realtimeSinceStartup - lastTimeStamp);
                    }

                    if (animatePosition)
                    {
                        calculatePosition();
                    }
                    if (animateRotation)
                    {
                        calculateRotation();
                    }

                }
                else
                {
                    defaultPosition = transform.localPosition;
                    defaultRotation = transform.rotation;
                }

                lastTimeStamp = Time.realtimeSinceStartup;
            }
        }

        void Update()
        {
            if (Application.isPlaying)
            {
                if ((positionAnimIndex == 0 || positionAnimIndex == 2 || positionAnimIndex == 4) && loopIndex == 2)
                {
                    time += reverseTime * timeScale * Time.deltaTime;
                }
                else
                {
                    time += timeScale * Time.deltaTime;
                }

                if (animatePosition)
                {
                    calculatePosition();
                }
                if (animateRotation)
                {
                    calculateRotation();
                }
            }

        }

        private void calculatePosition()
        {
            Vector3 animationPosition = Vector3.zero;

            if (positionAnimIndex == 0 || positionAnimIndex == 2 || positionAnimIndex == 4)
            {
                if (loopIndex == 1)
                {
                    if (time > 1) time = 0f;
                    else if (time < 0) time = 1f;
                }
                else if (loopIndex == 2)
                {
                    if (time > 1)
                    {
                        reverseTime = -reverseTime;
                        time = 1;
                    }
                    else if (time < 0)
                    {
                        reverseTime = -reverseTime;
                        time = 0;
                    }
                }
                else if (loopIndex == 0)
                {
                    if (time > 1)
                    {
                        time = 1;
                    }
                    else if (time < 0)
                    {
                        time = 0;
                    }
                }
            }

            switch (positionAnimIndex)
            {
                case 0://Linear
                    animationPosition = Vector3.Lerp(posA, posB, posFrequency * time);
                    break;
                case 1://Sine
                    Vector3 delta = posB - posA;
                    animationPosition = posA + delta * (0.5f + Mathf.Sin(2 * Mathf.PI * time + posPhaseShift * Mathf.PI) / 2f);
                    break;
                case 2://Circle
                    animationPosition = pointOnCircle(positionCircleCenter, positionCircleRadius, positionCircleZenith * Mathf.Deg2Rad, positionCircleAzimuth * Mathf.Deg2Rad, time * posFrequency * 2 * Mathf.PI + posPhaseShift * Mathf.PI);
                    break;
                case 3://Follow Target
                    if (positionTarget == null)
                    {
                        Debug.LogError(gameObject.name + ": Position Target is null!");
                        return;
                    }
                    if (!velocityApproach)
                    {
                        transform.position += positionHardness * (positionTarget.position - transform.position);
                    }
                    else
                    {
                        delta = positionTarget.position - transform.position;
                        Vector3 movement = Vector3.zero;

                        if(delta.magnitude <= snapRange)
                        {
                            if (velocity > 0) velocity = Mathf.Abs(Vector3.Magnitude(lastPosition - transform.position));
                            movement = snapHardness * delta * maxVelocity;
                        }
                        else
                        {
                            if (Application.isPlaying) velocity += acceleration * Time.deltaTime;
                            else velocity += acceleration * deltaTime;
                            if (velocity >= maxVelocity) velocity = maxVelocity;
                            movement = delta.normalized * velocity;
                        }
                        lastPosition = transform.position;
                        transform.position = transform.position + movement;
                    }
                    
                    return;
                case 4://Bezier
                    if (bezierCurve == null)
                    {
                        Debug.LogError(gameObject.name + ": Bezier Curve is null!");
                        return;
                    }
                    transform.position = bezierCurve.pointAt(time * bezierSpeed + bezierPhaseShift, bezierConstSpeed);
                    return;
                case 5://Perlin Noise
                    float synchron = 1;
                    if (perlinSynchron) synchron = 0;
                    if (perlinRandomOffset == Vector3.zero) perlinRandomOffset = new Vector3(Random.Range(0, 10000), Random.Range(0, 10000), Random.Range(0, 10000));
                    animationPosition.x = Mathf.PerlinNoise(synchron * perlinRandomOffset.x + time, 0) * perlinAmplitude.x - 0.5f * perlinAmplitude.x;
                    animationPosition.y = Mathf.PerlinNoise(0, synchron * perlinRandomOffset.y + time) * perlinAmplitude.y - 0.5f * perlinAmplitude.y;
                    animationPosition.z = Mathf.PerlinNoise(0, synchron * perlinRandomOffset.z + 10000 + time) * perlinAmplitude.z - 0.5f * perlinAmplitude.z;
                    break;
                default://Default
                    Debug.LogError("Unrecognized Option");
                    break;
            }
            if (positionAsOffset) animationPosition += defaultPosition;
            /*if (useParent)
            {
                if (transform.parent == null)
                {
                    Debug.LogError(transform.name + ": Cannot use Parent if parent is null!");
                }
                else animationPosition += transform.parent.position;
            }*/
            transform.localPosition = animationPosition;
        }

        private void calculateRotation()
        {
            Quaternion animationRotation = Quaternion.identity;
            switch (rotationAnimIndex)
            {
                case 0://Linear
                    animationRotation = defaultRotation * Quaternion.Euler(rotationAngles.x * time, rotationAngles.y * time, rotationAngles.z * time);
                    break;
                case 1://Look at Target
                    if (rotationTarget == null)
                    {
                        Debug.LogError(transform.name + ": Rotation Target is null!");
                        return;
                    }
                    Quaternion targetRotation = Quaternion.LookRotation(rotationTarget.position - transform.position, Vector3.up);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationHardness);
                    return;
                case 2://Bezier Nodes
                    if (bezierCurve == null)
                    {
                        Debug.LogError(transform.name + ": Bezier Curve is null!");
                        return;
                    }
                    transform.rotation = bezierCurve.rotationAt(time * bezierSpeed + bezierPhaseShift, bezierConstSpeed);
                    return;
                case 3://Look along curve
                    if (bezierCurve == null)
                    {
                        Debug.LogError(transform.name + ": Bezier Curve is null!");
                        return;
                    }
                    Vector3 target = bezierCurve.pointAt(time * bezierSpeed + bezierPhaseShift + curvePrediction / 10f, bezierConstSpeed);
                    targetRotation = Quaternion.LookRotation(target - transform.position, Vector3.up);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationHardness);
                    return;
                default://Default
                    Debug.LogError("Unrecognized Option!");
                    break;
            }
            /*if (!worldSpace) animationRotation += defaultPosition;
            if (useParent)
            {
                if (transform.parent == null) Debug.LogError("Cannot use Parent if parent is null!");
                else animationRotation += transform.parent.position;
            }
            if (gameObject == null) Debug.Log("Shit");*/
            transform.localRotation = animationRotation;
        }



        void OnDrawGizmos()
        {
            if (animatePosition)//Position
            {

                Gizmos.color = gizmoPositionColor;
                if (positionAnimIndex == 0 || positionAnimIndex == 1)//Linear | Sine
                {
                    Vector3 offset = Vector3.zero;
                    if (positionAsOffset)
                    {
                        offset += defaultPosition;
                    }
                    if (transform.parent != null)
                    {
                        Gizmos.DrawLine(transform.parent.TransformPoint(posA), transform.parent.TransformPoint(posB));
                        Gizmos.DrawWireCube(transform.parent.TransformPoint(posA), Vector3.one / 6f);
                        Gizmos.DrawWireCube(transform.parent.TransformPoint(posB), Vector3.one / 6f);
                        if (positionAnimIndex == 1)
                        {
                            Vector3 delta = posB - posA;
                            Vector3 pos0;
                            if (positionAnimIndex == 0) pos0 = posA + delta * (0.5f + Mathf.Sin(2 * Mathf.PI * posFrequency + posPhaseShift * Mathf.PI) / 2f);
                            else pos0 = posA + delta * (0.5f + Mathf.Sin(2 * Mathf.PI + posPhaseShift * Mathf.PI) / 2f);
                            Gizmos.DrawSphere(transform.parent.TransformPoint(pos0), 1 / 6f);
                        }
                    }
                    else
                    {
                        Gizmos.DrawLine(posA + offset, posB + offset);
                        Gizmos.DrawWireCube(posA + offset, Vector3.one / 6f);
                        Gizmos.DrawWireCube(posB + offset, Vector3.one / 6f);
                        if (positionAnimIndex == 1)
                        {
                            Vector3 delta = posB - posA;
                            Vector3 pos0;
                            if (positionAnimIndex == 0) pos0 = posA + delta * (0.5f + Mathf.Sin(2 * Mathf.PI * posFrequency + posPhaseShift * Mathf.PI) / 2f);
                            else pos0 = posA + delta * (0.5f + Mathf.Sin(2 * Mathf.PI + posPhaseShift * Mathf.PI) / 2f);
                            Gizmos.DrawSphere(pos0 + offset, 1 / 6f);
                        }
                    }
                }
                else if (positionAnimIndex == 2)//Circle
                {
                    Vector3 centerPos = positionCircleCenter;
                    if (positionAsOffset) centerPos += defaultPosition;
                    Quaternion parentRotation = Quaternion.identity;
                    if (transform.parent != null)
                    {
                        centerPos += transform.parent.position;
                        parentRotation = transform.parent.rotation;
                    }
                    //Handles.color = gizmoPositionColor;
                    Vector3 n = new Vector3(Mathf.Cos(positionCircleZenith * Mathf.Deg2Rad) * Mathf.Sin(positionCircleAzimuth * Mathf.Deg2Rad), Mathf.Sin(positionCircleAzimuth * Mathf.Deg2Rad) * Mathf.Sin(positionCircleZenith * Mathf.Deg2Rad), Mathf.Cos(positionCircleAzimuth * Mathf.Deg2Rad));
                    //Handles.DrawWireDisc(centerPos, parentRotation * n, positionCircleRadius);
                    Gizmos.color = gizmoPositionColor;
                    Gizmos.DrawLine(pointOnCircle(centerPos, positionCircleRadius, parentRotation * n, 0f), pointOnCircle(centerPos, positionCircleRadius, parentRotation * n, Mathf.PI));
                    Gizmos.DrawLine(pointOnCircle(centerPos, positionCircleRadius, parentRotation * n, 0.5f * Mathf.PI), pointOnCircle(centerPos, positionCircleRadius, parentRotation * n, 1.5f * Mathf.PI));
                    Gizmos.DrawSphere(pointOnCircle(centerPos, positionCircleRadius, parentRotation * n, posPhaseShift * Mathf.PI), 1 / 6f);
                }
                else if (positionAnimIndex == 3)//Follow Target
                {
                    if (positionTarget == null) return;

                    Gizmos.color = gizmoPositionColor;

                    DrawArrowGizmo(transform.position, (positionTarget.position - transform.position) * 0.9f, gizmoPositionColor, 1f);
                    Gizmos.DrawWireCube(transform.position, Vector3.one / 6f);
                    Gizmos.DrawWireSphere(positionTarget.position, 1 / 3f);
                }
                else if (positionAnimIndex == 4)//Bezier Curve
                {
                    if (bezierCurve == null) return;

                    Gizmos.color = gizmoPositionColor;

                    Gizmos.DrawSphere(bezierCurve.pointAt(bezierPhaseShift, bezierConstSpeed), 1 / 4f);

                    for (int i = 0; i < bezierCurve.points.Count; i++)
                    {
                        DrawArrowGizmo(bezierCurve.points[i].transform.position, bezierCurve.points[i].transform.forward * 3f, gizmoPositionColor, 1f);
                        Gizmos.DrawLine(bezierCurve.points[i].transform.position, bezierCurve.points[i].transform.position + bezierCurve.points[i].transform.up);
                    }

                }
                else if (positionAnimIndex == 5)//Perlin Noise
                {
                    Gizmos.color = gizmoPositionColor;
                    Vector3 offset = Vector3.zero;
                    if (positionAsOffset)
                    {
                        offset += defaultPosition;
                    }
                    if (transform.parent != null)
                    {
                        Gizmos.matrix = Matrix4x4.TRS(transform.parent.position, transform.parent.rotation, Vector3.one);
                        Gizmos.DrawWireCube(offset, perlinAmplitude);
                        Gizmos.matrix = Matrix4x4.identity;
                    }
                    else
                    {
                        Gizmos.DrawWireCube(offset, perlinAmplitude);
                    }

                    
                }
            }
            if (animateRotation)//Rotation
            {
                Gizmos.color = gizmoPositionColor;
                if (rotationAnimIndex == 0)//Linear
                {

                }
                else if (rotationAnimIndex == 1)//Look at target
                {
                    /*if (rotationTarget != null)
                    {
                        Handles.color = gizmoPositionColor;
                        Vector3 n = Vector3.Cross(rotationTarget.position - transform.position, rotationTarget.position - new Vector3(0, 1, 0) - transform.position);
                        Handles.DrawWireArc(transform.position, n, rotationTarget.position - transform.position, Mathf.Rad2Deg * Mathf.Atan2(transform.position.y - rotationTarget.position.y, transform.position.x - rotationTarget.position.x), Vector3.Distance(transform.position, rotationTarget.position));
                    }*/
                    Gizmos.color = gizmoPositionColor;

                    Gizmos.DrawLine(transform.position, rotationTarget.position);
                }
                else if(rotationAnimIndex == 3)//LookAlongCurve
                {
                    Gizmos.color = gizmoPositionColor;
                    Gizmos.DrawLine(transform.position, bezierCurve.pointAt(time * bezierSpeed + bezierPhaseShift + curvePrediction / 10f, bezierConstSpeed));
                }
            }
        }

        public Vector3 pointOnCircle(Vector3 center, float radius, float zenith, float azimuth, float time)
        {
            Vector3 n = new Vector3(Mathf.Cos(zenith) * Mathf.Sin(azimuth), Mathf.Sin(azimuth) * Mathf.Sin(zenith), Mathf.Cos(azimuth));
            Vector3 u = new Vector3(-Mathf.Sin(zenith), Mathf.Cos(zenith), 0);
            return radius * Mathf.Cos(time) * u + radius * Mathf.Sin(time) * Vector3.Cross(n, u) + center;
        }

        public Vector3 pointOnCircle(Vector3 center, float radius, Vector3 normal, float time)
        {
            Vector3 u = new Vector3(1, 1, (-normal.x - normal.y) / normal.z).normalized;
            return radius * Mathf.Cos(time) * u + radius * Mathf.Sin(time) * Vector3.Cross(normal, u) + center;
        }

        public static void DrawArrowGizmo(Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
        {
            if (direction == Vector3.zero) return;
            Gizmos.color = color;
            Gizmos.DrawRay(pos, direction);

            Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
            Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
        }

    }

}
