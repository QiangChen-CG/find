using UnityEngine;
using System.Collections;

public class GameRule : MonoBehaviour
{

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void LateUpdate()
	{
		if (GameData.instance.foods <= 0 && GameData.instance.playerSC.CanCtrl && GameData.instance.playerSC.state == StepChar.State.idle)
		{
			GameData.instance.foods = 0;
			GameData.instance.playerSC.Dead();

			GameUI.instance.GameOver();
		}
		
	}
}
