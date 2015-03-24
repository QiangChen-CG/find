using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class mazeGTCell
{
	public enum CELLTYPE
	{
		path = 0,
		wall
	};

	public enum NEBERTYPE
	{
		U,
		B,
		L,
		R
	};

	public CELLTYPE celltype = CELLTYPE.wall;
	public int x = 9;
	public int y = 9;
	public bool isStart = false;
	public bool isEnd;
	//選路徑暫存用.
	public NEBERTYPE tempNeberType = NEBERTYPE.B;
	public int tempRandValue = 0;
}

public class mazeGTChoseWay
{
	public mazeGTCell.NEBERTYPE nebertype;
	public int randvalue;
	
}
public class mazeGT : MonoBehaviour
{
	public TextMesh debugText;

	public GameObject wallPrefab;
	public GameObject startPrefab;
	public GameObject goalPrefab;
	//選路的方式.
	public enum CHOSEWAYTYPE
	{
		//隨機.
		Rand,
		//自訂.
		CUSTOM,
	};
	//大小.
	public int x;
	public int y;
	//保留邊界.
	public bool wallAround = true;
	//選路的方式.
	public CHOSEWAYTYPE choseWayType = CHOSEWAYTYPE.Rand;

	public int choseRandR = 100;
	public int choseRandL = 100;
	public int choseRandU = 100;
	public int choseRandB = 100;

	//記錄最遠.
	public int statckMax = -1;
	public mazeGTCell farCell = null;

	public mazeGTCell[,] cells;
	Stack pathRec = new Stack();

	//保留邊界.
	int xkeep = 0;
	int ykeep = 0;



	// Use this for initialization
	void Start ()
	{
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void GenMaze(int x, int y)
	{
		if (cells != null)
		{
			System.Array.Clear(cells, 0, cells.Length);
		}
		statckMax = -1;
		farCell = null;

		cells = new mazeGTCell[x, y];

		for (int i = 0; i < y; i++)
		{
			for (int j = 0; j < x; j++)
			{
				mazeGTCell newmc = new mazeGTCell();
				newmc.x = j;
				newmc.y = i;
				newmc.celltype = mazeGTCell.CELLTYPE.wall;
				cells[j, i] = newmc;

			}
		}
		
		xkeep = 0;
		ykeep = 0;
		if (wallAround)
		{
			xkeep = 1;
			ykeep = 1;
		}
		int startx = Random.Range(0 + xkeep, x - xkeep);
		int starty = Random.Range(0 + ykeep, y - ykeep);

		//step1.
		mazeGTCell startcell = cells[startx, starty];
		startcell.isStart = true;

		pathRec.Push(startcell);

		//step2.
		while (pathRec.Count > 0)
		{
			mazeGTCell next = (mazeGTCell)pathRec.Peek();
			next.celltype = mazeGTCell.CELLTYPE.path;

			//記下最遠.
			if (pathRec.Count > statckMax)
			{
				statckMax = pathRec.Count;
				farCell = next;
			}

			//step3.
			if (!FindTheWay (next))
			{
				//step4.
				pathRec.Pop();
			}	
		}

		if (farCell != null)
		{
			farCell.isEnd = true;
		}
	}

	//週沒有路徑.建立一個 LOOP 的迷宮.
	bool IsNoPathNear(mazeGTCell cell, mazeGTCell parentcell)
	{
		//neberhood.
		List <mazeGTCell> nb = new List<mazeGTCell> ();
		//L.
		if (cell.x > xkeep)
		{
			mazeGTCell nc = cells[cell.x - 1, cell.y];
			if (nc.celltype == mazeGTCell.CELLTYPE.path && parentcell != nc)
			{
				return false;
			}
		}
		//R.
		if (cell.x < x - 1 - xkeep)
		{
			mazeGTCell nc = cells[cell.x + 1, cell.y];
			if (nc.celltype == mazeGTCell.CELLTYPE.path && parentcell != nc)
			{
				return false;
			}
		}
		
		//B.
		if (cell.y > ykeep)
		{
			mazeGTCell nc = cells[cell.x, cell.y - 1];
			if (nc.celltype == mazeGTCell.CELLTYPE.path && parentcell != nc)
			{
				return false;
			}
		}
		//U.
		if (cell.y < y - 1 - ykeep)
		{
			mazeGTCell nc = cells[cell.x, cell.y + 1];
			if (nc.celltype == mazeGTCell.CELLTYPE.path && parentcell != nc)
			{
				return false;
			}
		}
		return true;
	}

	bool FindTheWay(mazeGTCell cell)
	{
		//neberhood.
		List <mazeGTCell> nb = new List<mazeGTCell> ();
		//L.
		if (cell.x > xkeep)
		{
			mazeGTCell nc = cells[cell.x - 1, cell.y];
			if (nc.celltype == mazeGTCell.CELLTYPE.wall && IsNoPathNear(nc, cell))
			{
				nc.tempNeberType = mazeGTCell.NEBERTYPE.L;
				nc.tempRandValue = choseRandL;
				nb.Add(nc);
			}
		}
		//R.
		if (cell.x < x - 1 - xkeep)
		{
			mazeGTCell nc = cells[cell.x + 1, cell.y];
			if (nc.celltype == mazeGTCell.CELLTYPE.wall && IsNoPathNear(nc, cell))
			{
				nc.tempNeberType = mazeGTCell.NEBERTYPE.R;
				nc.tempRandValue = choseRandR;
				nb.Add(nc);
			}
		}

		//B.
		if (cell.y > ykeep)
		{
			mazeGTCell nc = cells[cell.x, cell.y - 1];
			if (nc.celltype == mazeGTCell.CELLTYPE.wall && IsNoPathNear(nc, cell))
			{
				nc.tempNeberType = mazeGTCell.NEBERTYPE.B;
				nc.tempRandValue = choseRandB;
				nb.Add(nc);
			}
		}
		//U.
		if (cell.y < y - 1 - ykeep)
		{
			mazeGTCell nc = cells[cell.x, cell.y + 1];
			if (nc.celltype == mazeGTCell.CELLTYPE.wall && IsNoPathNear(nc, cell))
			{
				nc.tempNeberType = mazeGTCell.NEBERTYPE.U;
				nc.tempRandValue = choseRandU;
				nb.Add(nc);
			}
		}

		if (nb.Count > 0)
		{
			ChoseWayCell(ref nb);
		}
		else
		{
			return false;
		}

		return true;
	}

	void ChoseWayCell(ref List <mazeGTCell> nb)
	{
		if (choseWayType == CHOSEWAYTYPE.Rand)
		{
			int randp = Random.Range (0, nb.Count);
			mazeGTCell rc = nb[randp];
			pathRec.Push(rc);
		}
		else
		{
			//總機率.
			int total = 0;
			foreach (mazeGTCell mc in nb)
			{
				total += mc.tempRandValue;
			}
			int randp = Random.Range(0, total);
			//算出落在那.
			int range = 0;
			foreach (mazeGTCell mc in nb)
			{
				range += mc.tempRandValue;
				if (randp < range)
				{
					pathRec.Push(mc);
					break;
				}
			}

		}
	}

	void DebugPrint()
	{
		string msg = "";
		for (int i = 0; i < y; i++)
		{
			for (int j = 0; j < x; j++)
			{
				if (cells[j, i].isStart)
				{
					msg += "Ｓ";
				}
				else if (cells[j, i].isEnd)
				{
					msg += "Ｅ";
				}
				else if (cells[j, i].celltype == mazeGTCell.CELLTYPE.path)
				{
					msg += "　";
				}
				else
				{
					msg += "Ｗ";
				}
			}
			msg += "\n";
		}

		Debug.Log(msg);
		debugText.text = msg;
	}

	void Awake()
	{
		GenMaze(x, y);
		//InstanceMaze();

	}

	void InstanceMaze()
	{		
		for (int i = 0; i < y; i++)
		{
			for (int j = 0; j < x; j++)
			{
				GameObject go = null;
				if (cells[j, i].isStart)
				{
					go = GameObject.Instantiate<GameObject>(startPrefab);
				}
				else if (cells[j, i].isEnd)
				{
					go = GameObject.Instantiate<GameObject>(goalPrefab);
				}
				else if (cells[j, i].celltype == mazeGTCell.CELLTYPE.path)
				{
				}
				else
				{
					go = GameObject.Instantiate<GameObject>(wallPrefab);
				}

				if (go != null)
				{					
					go.transform.position = new Vector3(j, 0, i);
					go.transform.parent = gameObject.transform;
				}
			}
		}
	}

	void OnGUI()
	{
		arcUtility.GUIMatrixAutoScale(600, 400);
		if (GUILayout.Button ("gen"))
		{
			GenMaze(x, y);
			DebugPrint();
		}
		if (GUILayout.Button ("DebugPrint"))
		{
			DebugPrint();
		}
		if (GUILayout.Button ("InstanceMaze"))
		{
			InstanceMaze();
		}
		if (GUILayout.Button ("disable shadow"))
		{
			Light[] ls = FindObjectsOfType(typeof(Light)) as Light[];
			foreach (Light l in ls) {
				l.shadows = LightShadows.None;
			}
		}
	}

}
