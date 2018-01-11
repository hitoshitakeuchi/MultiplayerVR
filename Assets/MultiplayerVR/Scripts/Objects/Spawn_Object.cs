using UnityEngine;
using System.Collections;

[System.Serializable]
public class Spawn_Object
{
	public GameObject GameObject;
	public float WaitTime;
	
	public Spawn_Object (GameObject gameObject, float waitTime)
	{
		this.GameObject = gameObject;
		this.WaitTime = waitTime;
	}
}
