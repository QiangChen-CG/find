using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class uifood : MonoBehaviour {

	public Text foodtext;

	public int lastfood = -1;
	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update () {
	
		if (lastfood != GameData.instance.foods)
		{
			lastfood = GameData.instance.foods;
			foodtext.text =  lastfood.ToString();
		}
	}
}
