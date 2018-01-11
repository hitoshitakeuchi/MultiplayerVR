using UnityEngine;
using System.Collections;

public class AutoRotate : MonoBehaviour
{
    public float rot = -0.06f;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, Time.smoothDeltaTime * rot);

        transform.position += new Vector3(0f, 0.00002f, 0f);
    }
}
