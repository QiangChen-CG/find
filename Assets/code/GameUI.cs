using UnityEngine;
using System.Collections;

public class GameUI : MonoBehaviour {

	public static GameUI instance;
	public GameObject gameoverobj;
	public Animator gameoveranim;
	// Use this for initialization
	void Awake ()
	{
		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	
	public void GameOver()
	{
		gameoveranim.Play("restartin");
	}

	public void RestartGame()
	{
		GameData.instance.RestartGame();
		
		gameoveranim.Play("restartout");
	}

}
