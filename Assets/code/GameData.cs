using UnityEngine;
using System.Collections;

public class GameData : MonoBehaviour {

	public static GameData instance;

	public int randX = 8;
	public int randY = 8;
	public mazeGT randMaze = new mazeGT();

	public StepChar playerSC
	{
		get 
		{
			if (_playerSC == null)
			{
				_playerSC =  GameObject.Find("setpChar").GetComponent<StepChar>();
			}

			return _playerSC;
		}
	}
	
	StepChar _playerSC;
	
	public GameObject wallPrefab;
	public GameObject startPrefab;
	public GameObject goalPrefab;

	public bool UseRandMaze = true;
	public int currentLevel = 1;
	public int maxLevel = 4;
	public int restartLevel = 2;
	public GameObject currentLevelRoot;
	public GameObject currentLevelStart;

	public GameObject newLevelRoot;
	public GameObject newLevelStart;

	public int defaultFoods = 10;
	public int foods = 10;

	// Use this for initialization
	void Awake ()
	{
		if (instance == null)
		{
			randMaze.owner = gameObject;
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}

	// Use this for initialization
	void Start ()
	{
		LoadInitLevel();
	}

	public void LoadInitLevel()
	{
		if (currentLevel == 0)
		{
			GameData.instance.playerSC.CanCtrl = false;
			LoadNextLevel();
		}

	}

	// Update is called once per frame
	void Update ()
	{
	
	}

	/*
	void OnGUI()
	{		
		if (GUILayout.Button("level1"))
		{
			Application.LoadLevel("level1");
		}
	}*/

	public void LoadNextLevel()
	{
		if (UseRandMaze)
		{
			StartCoroutine("LoadLevelRand");
		}
		else
		{
			StartCoroutine("LoadLevel");
		}
	}

	
	IEnumerator LoadLevelRand()
	{
		currentLevelRoot = newLevelRoot;
		currentLevel++;
				
		
		int newlevelid = currentLevel;
		string newlevelname = "Level" + newlevelid.ToString();

		randMaze.GenMaze(randX, randY);

		GameObject levelobjRoot = new GameObject(newlevelname);
		
		for (int i = 0; i < randMaze.y; i++)
		{
			for (int j = 0; j < randMaze.x; j++)
			{
				yield return null;
				GameObject go = null;
				if (randMaze.cells[j, i].isStart)
				{
					go = GameObject.Instantiate<GameObject>(startPrefab);
					go.name = "start";
				}
				else if (randMaze.cells[j, i].isEnd)
				{
					go = GameObject.Instantiate<GameObject>(goalPrefab);
				}
				else if (randMaze.cells[j, i].celltype == mazeGTCell.CELLTYPE.path)
				{
				}
				else
				{
					go = GameObject.Instantiate<GameObject>(wallPrefab);
				}
				
				if (go != null)
				{					
					go.transform.position = new Vector3(j, 0, i);
					go.transform.parent = levelobjRoot.transform;
				}
			}
		}
		yield return null;


		
		newLevelRoot =  GameObject.Find(newlevelname);
		newLevelStart = GameObject.Find(newlevelname+"/start");





		Vector3 levelpos = playerSC.gameObject.transform.position - newLevelStart.transform.position;
		levelpos.y = newLevelRoot.transform.position.y;
		levelpos.x += newLevelRoot.transform.position.x;
		levelpos.z += newLevelRoot.transform.position.z;
		
		newLevelRoot.transform.position = levelpos;	
		
		if (currentLevel == 1)
		{
			arcGameSoundPlayer.instance.PlayFX(5);
		}
		else
		{
			arcGameSoundPlayer.instance.PlayFX(2);
		}
		
		if (currentLevelRoot != null)
		{
			StartCoroutine("HideCurrentLevel");
		}
		StartCoroutine("ShowNewLevel");
		
	}



	IEnumerator LoadLevel()
	{
		currentLevelRoot = newLevelRoot;
		currentLevel++;

		if (currentLevel > maxLevel)
		{
			currentLevel = restartLevel;
		}


		int newlevelid = currentLevel;
		string newlevelname = "Level" + newlevelid.ToString();
		
		AsyncOperation ao = Application.LoadLevelAdditiveAsync(newlevelname);
		yield return ao;

		newLevelRoot =  GameObject.Find(newlevelname);
		newLevelStart = GameObject.Find(newlevelname+"/start");
		//Vector3 levelpos = newLevelRoot.transform.position;
		
		Vector3 levelpos = playerSC.gameObject.transform.position - newLevelStart.transform.position;
		levelpos.y = newLevelRoot.transform.position.y;
		levelpos.x += newLevelRoot.transform.position.x;
		levelpos.z += newLevelRoot.transform.position.z;
		
		newLevelRoot.transform.position = levelpos;	

		if (currentLevel == 1)
		{
			arcGameSoundPlayer.instance.PlayFX(5);
		}
		else
		{
			arcGameSoundPlayer.instance.PlayFX(2);
		}

		if (currentLevelRoot != null)
		{
			StartCoroutine("HideCurrentLevel");
		}
		StartCoroutine("ShowNewLevel");

	}
	
	IEnumerator HideCurrentLevel()
	{
		if (currentLevelRoot == null)
		{
			return true;
		}

		float tweentime = 0;
		float starty = currentLevelRoot.transform.position.y;
		float endy = currentLevelRoot.transform.position.y - 1.0f;;
		while (tweentime < 1.0f)
		{
			if (currentLevelRoot == null)
			{
				return true;
			}

			float deltatime = arcGamePlayTimeMgr.GetDeltaTime("scene");
			tweentime += deltatime;
			
			float y = iTweenLite.Easing(starty, endy, tweentime, iTweenLite.EaseType.linear);
			Vector3 levelpos = currentLevelRoot.transform.position;
			levelpos.y = y;
			currentLevelRoot.transform.position = levelpos;

			yield return null;
		}

		GameObject.Destroy(currentLevelRoot);
		//currentLevelRoot.SetActive(false);
	}
	
	IEnumerator ShowNewLevel()
	{
		float tweentime = 0;
		float starty = newLevelRoot.transform.position.y - 1.0f;
		float endy = newLevelRoot.transform.position.y;
		while (tweentime < 1.0f)
		{
			float deltatime = arcGamePlayTimeMgr.GetDeltaTime("scene");
			tweentime += deltatime;
			
			float y = iTweenLite.Easing(starty, endy, tweentime, iTweenLite.EaseType.linear);
			Vector3 levelpos = newLevelRoot.transform.position;
			levelpos.y = y;
			newLevelRoot.transform.position = levelpos;
			
			yield return null;
		}

		GameData.instance.playerSC.CanCtrl = true;
	}

	public void AddFood(int add)
	{
		foods += add;
		//Debug.Log (foods);
	}

	public void RestartGame()
	{
		currentLevel = 0;
		Application.LoadLevel("level0");

		//LoadNextLevel();
		foods = defaultFoods;
		GameData.instance.playerSC.CanCtrl = false;

	}

}
