using UnityEngine;
using System.Collections;

public class Destroy : MonoBehaviour
{

    public float lifetime = 90f;

    void Start()
    {
        GameObject.Destroy(gameObject, lifetime);
    }
}
