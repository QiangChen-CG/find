using UnityEngine;
using System.Collections;

public class food : MonoBehaviour {


	public int foodadd = 1;
	public int soundFxId = -1;
	
	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "player")
		{
			GameData.instance.AddFood(foodadd);
			arcGameSoundPlayer.instance.PlayFX(soundFxId);
			Destroy(gameObject);
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
