using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class mazeEventData
{
	public GameObject target;
	public string message;
}

public class mazeEvent : MonoBehaviour
{

	public mazeEventData[] eventdatas;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
	
	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "player")
		{
			foreach (mazeEventData md in eventdatas)
			{
				md.target.SendMessage(md.message);
			}

		}
	}

}
