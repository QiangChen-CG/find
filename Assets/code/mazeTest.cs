using UnityEngine;
using System.Collections;

public class mazeTest : MonoBehaviour {

	int foo;
	public GameObject wallprefab;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI()
	{
		if (GUILayout.Button ("createmaze")) 
		{
			mazeGen mg = new mazeGen(8, 8);

			for (int i = 0; i < mg._width; i++)
			{
				for (int j = 0; j < mg._height; j++)
				{
				//	mg._cells[i, j];
				}
			}
		}
	}
}
