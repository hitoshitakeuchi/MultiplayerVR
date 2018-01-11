using UnityEngine;
using System.Collections.Generic;
using System.Linq;
//using UnityEditor;

namespace MovementPlus
{

    [ExecuteInEditMode]
    public class BezierCurve : MonoBehaviour
    {

        public List<GameObject> points;
        public Color curveColor = Color.white;
        public int resolution = 16;
        public bool closed = false;
        public bool preciseHandles = false;

        private float curveLength;

        //public void removePoint(SerializedProperty pointsProp, int index)
        //{
        //    if (index < 0 || index >= pointsProp.arraySize)
        //    {
        //        Debug.LogError("Error in " + gameObject.name + " -> BezierCurve Index: " + index + " is out of Bounds!");
        //        return;
        //    }
        //    GameObject go = (GameObject)pointsProp.GetArrayElementAtIndex(index).objectReferenceValue;
        //    DestroyImmediate(go);
        //    if (pointsProp.GetArrayElementAtIndex(index) != null) pointsProp.DeleteArrayElementAtIndex(index);
        //    //Undo.DestroyObjectImmediate(points[index].gameObject);
        //    pointsProp.DeleteArrayElementAtIndex(index);
        //    for (int i = index; i < pointsProp.arraySize; i++)
        //    {
        //        if (pointsProp.GetArrayElementAtIndex(i).objectReferenceValue == null) continue;
        //        GameObject g = (GameObject)pointsProp.GetArrayElementAtIndex(i).objectReferenceValue;
        //        g.name = "Point " + i;
        //    }
        //}

        //public void addPoint(SerializedProperty pointsProp, int index = -1)
        //{
        //    if (index < 0) index = pointsProp.arraySize;
        //    if (index > pointsProp.arraySize - 1) index = pointsProp.arraySize - 1;
        //    index++;

        //    GameObject g = new GameObject();
        //    //Undo.RegisterCreatedObjectUndo(g, "Created new Bezier Point");
        //    g.AddComponent<BezierPoint>();
        //    g.transform.SetParent(transform);
        //    g.name = "Point " + index;
        //    pointsProp.InsertArrayElementAtIndex(index);
        //    pointsProp.GetArrayElementAtIndex(index).objectReferenceValue = g;

        //    for (int i = index; i < pointsProp.arraySize; i++)
        //    {
        //        if (pointsProp.GetArrayElementAtIndex(i).objectReferenceValue == null) continue;
        //        GameObject go = (GameObject)pointsProp.GetArrayElementAtIndex(i).objectReferenceValue;
        //        go.name = "Point " + i;
        //    }
        //    GameObject before = (GameObject)pointsProp.GetArrayElementAtIndex(index - 1).objectReferenceValue;
        //    g.transform.position = before.transform.position + new Vector3(3, 0, 0);//pointAt(index / points.Count * 1f + 0.03f, false, true);
        //}

        //Not working!
        /*public void checkChildren(SerializedProperty pointsProp)
        {
            int numChildren = transform.childCount;
            for (int i = 0; i < numChildren; i++)
            {
                if (!transform.GetChild(i).name.Contains("Point")) {
                    DestroyImmediate(transform.GetChild(i).gameObject);
                    i--;
                    numChildren--;
                }
            }

            for(int j = 0; j < pointsProp.arraySize; j++)
            {
                pointsProp.DeleteArrayElementAtIndex(0);
            }

            for (int i = 0; i < numChildren; i++)
            {
                pointsProp.InsertArrayElementAtIndex(i);
                GameObject g = (GameObject)pointsProp.GetArrayElementAtIndex(i).objectReferenceValue;
                g = transform.GetChild(i).gameObject;

                if (transform.GetChild(i).GetComponent<BezierPoint>() == null)transform.GetChild(i).gameObject.AddComponent<BezierPoint>();
            }

        }*/

        public Vector3 pointAt(float t, bool constSpeed)
        {
            if (points == null) return Vector3.zero;
            if (points.Count < 2) return Vector3.zero;

            bool loop = true;

            if (t >= 1f)
            {
                if (loop)
                {
                    t -= Mathf.FloorToInt(t);
                }
                else
                {
                    t = 0.99999f;
                }
            }
            else if (t < 0f)
            {
                if (loop)
                {
                    t += Mathf.Abs(Mathf.FloorToInt(t));
                }
                else
                {
                    t = 0f;
                }
            }

            //if (t >= 1) t = 0.99999f;

            if (closed) points.Add(points[0]);

            t *= points.Count - 1;
            int i = Mathf.FloorToInt(t);
            t -= i;

            Vector3 p0 = points[i].transform.position;
            Vector3 p1 = points[i + 1].transform.position;
            Vector3 h0 = transform.TransformPoint(points[i].GetComponent<BezierPoint>().handle1 + points[i].transform.localPosition);
            Vector3 h1 = transform.TransformPoint(points[i + 1].GetComponent<BezierPoint>().handle0 + points[i + 1].transform.localPosition);

            /*Vector3 v1 = -3 * p0 + 9 * p0 - 9 * h1 + 3 * p1;
            Vector3 v2 = 6 * p0 - 12 * h0 + 6 * h1;
            Vector3 v3 = -3 * p0 + 3 * h0;*/

            if (closed) points.RemoveAt(points.Count - 1);

            if (!constSpeed)
            {
                Vector3 temp;
                cubicBezier(p0, p1, h0, h1, t, out temp);
                return temp;
            }
            else
            {
                Debug.LogError("Constant Bezier Velocity is not implemented  yet!");
            }

            return Vector3.zero;
        }

        public Quaternion rotationAt(float t, bool constSpeed)
        {
            bool loop = true;

            if (t >= 1f)
            {
                if (loop)
                {
                    t -= Mathf.FloorToInt(t);
                }
                else
                {
                    t = 0.99999f;
                }
            }
            else if (t < 0f)
            {
                if (loop)
                {
                    t += Mathf.Abs(Mathf.FloorToInt(t));
                }
                else
                {
                    t = 0f;
                }
            }

            //if (t >= 1) t = 0.99999f;

            if (closed) points.Add(points[0]);

            t *= points.Count - 1;
            int i = Mathf.FloorToInt(t);
            t -= i;

            /*Vector3 p0 = points[i].transform.position;
            Vector3 p1 = points[i + 1].transform.position;
            Vector3 h0 = transform.TransformPoint(points[i].GetComponent<BezierPoint>().handle1);
            Vector3 h1 = transform.TransformPoint(points[i + 1].GetComponent<BezierPoint>().handle0);
            */

            /*Vector3 v1 = -3 * p0 + 9 * p0 - 9 * h1 + 3 * p1;
            Vector3 v2 = 6 * p0 - 12 * h0 + 6 * h1;
            Vector3 v3 = -3 * p0 + 3 * h0;*/

            //Use curve length for const velocity? Or this http://gamedev.stackexchange.com/questions/27056/how-to-achieve-uniform-speed-of-movement-on-a-bezier-curve

            Quaternion newRotation = Quaternion.Slerp(points[i].transform.rotation, points[i + 1].transform.rotation, t);

            if (closed) points.RemoveAt(points.Count - 1);

            if (!constSpeed)
            {
                return newRotation;
            }
            else
            {

            }

            return Quaternion.identity;
        }

        void OnDrawGizmos()
        {
            if (closed) points.Add(points[0]);
            for (int i = 0; i < points.Count - 1; i++)
            {
                if (points[i] == null || points[i + 1] == null) continue;
                drawCubicBezier(points[i].transform.position, points[i + 1].transform.position, transform.TransformPoint(points[i].GetComponent<BezierPoint>().handle1 + points[i].transform.localPosition), transform.TransformPoint(points[i + 1].GetComponent<BezierPoint>().handle0 + points[i + 1].transform.localPosition));
            }
            if (closed) points.RemoveAt(points.Count - 1);
        }

        private void drawCubicBezier(Vector3 p0, Vector3 p1, Vector3 h0, Vector3 h1)
        {
            Vector3 start = Vector3.zero, end = Vector3.zero;
            Gizmos.color = curveColor;

            for (int i = 0; i < resolution; i++)
            {
                cubicBezier(p0, p1, h0, h1, i * 1f / resolution, out start);
                cubicBezier(p0, p1, h0, h1, (i + 1) * 1f / resolution, out end);
                Gizmos.DrawLine(start, end);
            }
        }

        public void updateCurveLength()
        {
            Vector3 p0, p1, h0, h1;

            float temp = 0;
            Vector3 start = Vector3.zero, end = Vector3.zero;

            if (closed) points.Add(points[0]);
            for (int i = 0; i < points.Count - 1; i++)
            {
                for (int r = 0; r < resolution; r++)
                {
                    if (points[i] == null || points[i + 1] == null)
                    {
                        curveLength = 0;
                        return;
                    }
                    p0 = points[i].transform.position;
                    p1 = points[i + 1].transform.position;
                    h0 = transform.TransformPoint(points[i].GetComponent<BezierPoint>().handle1 + points[i].transform.localPosition);
                    h1 = transform.TransformPoint(points[i + 1].GetComponent<BezierPoint>().handle0 + points[i + 1].transform.localPosition);

                    cubicBezier(p0, p1, h0, h1, r * 1f / resolution, out start);
                    cubicBezier(p0, p1, h0, h1, (r + 1) * 1f / resolution, out end);
                    temp += Vector3.Distance(start, end);
                }
            }
            if (closed) points.RemoveAt(points.Count - 1);

            curveLength = temp;
        }

        public float getCurveLength()
        {
            return curveLength;
        }

        private void cubicBezier(Vector3 p0, Vector3 p1, Vector3 h0, Vector3 h1, float t, out Vector3 pFinal)
        {
            pFinal.x = Mathf.Pow(1 - t, 3) * p0.x + Mathf.Pow(1 - t, 2) * 3 * t * h0.x + (1 - t) * 3 * t * t * h1.x + t * t * t * p1.x;
            pFinal.y = Mathf.Pow(1 - t, 3) * p0.y + Mathf.Pow(1 - t, 2) * 3 * t * h0.y + (1 - t) * 3 * t * t * h1.y + t * t * t * p1.y;
            pFinal.z = Mathf.Pow(1 - t, 3) * p0.z + Mathf.Pow(1 - t, 2) * 3 * t * h0.z + (1 - t) * 3 * t * t * h1.z + t * t * t * p1.z;
        }

        private GameObject findChild(Transform parent, string name)
        {
            int numChilds = parent.childCount;
            for (int i = 0; i < numChilds; i++)
            {
                if (parent.GetChild(i).name.Equals(name)) return parent.GetChild(i).gameObject;
            }
            return null;
        }

        private GameObject[] findChildren(Transform parent, string partialName)
        {
            int numChilds = parent.childCount;
            int count = 0;

            for (int i = 0; i < numChilds; i++)
            {
                if (parent.GetChild(i).name.Contains(partialName)) count++;
            }

            if (count == 0)
            {
                return null;
            }
            else
            {
                GameObject[] result = new GameObject[count];
                count = 0;
                for (int i = 0; i < numChilds; i++)
                {
                    if (parent.GetChild(i).name.Contains(partialName))
                    {
                        result[count] = parent.GetChild(i).gameObject;
                        count++;
                    }
                }
                return result;
            }
        }

    }

}