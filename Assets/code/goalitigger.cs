using UnityEngine;
using System.Collections;

public class goalitigger : MonoBehaviour {

	public Animator anim;

	// Use this for initialization
	void Start () {
	
		if (anim != null)
		{
			anim.Play("goalidle");
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "player")
		{
			if (anim != null)
			{
				anim.Play("goalon");
			}
			StartCoroutine("waitAnimAndLoadNextLevel");
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.tag == "player")
		{
			if (anim != null)
			{
				anim.Play("goaloff");
			}
		}
	}

	
	IEnumerator waitAnimAndLoadNextLevel()
	{
		
		arcGameSoundPlayer.instance.PlayFX(1);
		GameData.instance.playerSC.CanCtrl = false;
		yield return new WaitForSeconds(0.5f);
		GameData.instance.LoadNextLevel();
	}

}
