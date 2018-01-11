using UnityEngine;
using System.Collections;

public class Spawn : MonoBehaviour
{
    public GameObject obj;

    void Update()
    {
        Instantiate(obj, this.transform.position, Quaternion.identity);
    }
}
