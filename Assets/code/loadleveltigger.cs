using UnityEngine;
using System.Collections;

public class loadleveltigger : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
		GameData.instance.LoadInitLevel();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI()
	{
		arcUtility.GUIMatrixAutoScale(300,400);
		if (GUILayout.Button("next level"))
		{
			GameData.instance.LoadNextLevel();
		}
	}
}
