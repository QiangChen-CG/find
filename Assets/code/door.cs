using UnityEngine;
using System.Collections;

public class door : MonoBehaviour {

	public Animator anim;

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public void DoorOpen()
	{
		anim.Play("dooropen");
	}

}
