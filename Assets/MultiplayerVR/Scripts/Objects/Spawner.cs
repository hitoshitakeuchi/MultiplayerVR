using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject spawnObject;
    public Vector3 throwForce;

    void Start()
    {
        StartCoroutine(createSpawnObject(spawnObject, 0.1f, 1));
    }

    IEnumerator createSpawnObject(GameObject gameObject, float wait, int count)
    {
        yield return new WaitForSeconds(wait);

        for (int i = 0; i < count; i++)
        {
            var spawn = Instantiate(gameObject, transform.position, Quaternion.identity) as GameObject;
            spawn.GetComponent<Rigidbody>().AddForce(throwForce, ForceMode.Impulse);
        }
    }
}
