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

	public CELLTYPE celltype = CELLTYPE.wall;
	public int x;
	public int y;
}

public class mazeGT : MonoBehaviour {

	public int x;
	public int y;

	public mazeGTCell[,] cells;

	public Stack pathRec = new Stack();

	// Use this for initialization
	void Start () {
	
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
		cells = new mazeGTCell[x, y];

		int startx = Random.Range(0, x);
		int starty = Random.Range(0, y);

		//step1.
		mazeGTCell startcell = cells[startx, starty];
		startcell.celltype = mazeGTCell.CELLTYPE.path;
		startcell.x = startx;
		startcell.y = starty;

		pathRec.Push(startcell);

		//step2.
		while (pathRec.Count > 0)
		{
			mazeGTCell next = pathRec.Pop();
			FindTheWay (next);
	
		}
	}

	bool IsNoPathNear(mazeGTCell cell, mazeGTCell parentcell)
	{
		//neberhood.
		List <mazeGTCell> nb = new List<mazeGTCell> ();
		//L.
		if (cell.x > 1)
		{
			mazeGTCell nc = cells[cell.x - 1, cell.y];
			if (nc.celltype == mazeGTCell.CELLTYPE.path && parentcell != nc)
			{
				return false;
			}
		}
		//R.
		if (cell.x < x - 1)
		{
			mazeGTCell nc = cells[cell.x + 1, cell.y];
			if (nc.celltype == mazeGTCell.CELLTYPE.path && parentcell != nc)
			{
				return false;
			}
		}
		
		//B.
		if (cell.y > 1)
		{
			mazeGTCell nc = cells[cell.x, cell.y - 1];
			if (nc.celltype == mazeGTCell.CELLTYPE.path && parentcell != nc)
			{
				return false;
			}
		}
		//U.
		if (cell.y < - 1)
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
		if (cell.x > 1)
		{
			mazeGTCell nc = cells[cell.x - 1, cell.y];
			if (nc.celltype == mazeGTCell.CELLTYPE.wall && IsNoPathNear(nc, cell))
			{
				nb.Add(nc);
			}
		}
		//R.
		if (cell.x < x - 1)
		{
			mazeGTCell nc = cells[cell.x + 1, cell.y];
			if (nc.celltype == mazeGTCell.CELLTYPE.wall && IsNoPathNear(nc, cell))
			{
				nb.Add(nc);
			}
		}

		//B.
		if (cell.y > 1)
		{
			mazeGTCell nc = cells[cell.x, cell.y - 1];
			if (nc.celltype == mazeGTCell.CELLTYPE.wall && IsNoPathNear(nc, cell))
			{
				nb.Add(nc);
			}
		}
		//U.
		if (cell.y < - 1)
		{
			mazeGTCell nc = cells[cell.x, cell.y + 1];
			if (nc.celltype == mazeGTCell.CELLTYPE.wall && IsNoPathNear(nc, cell))
			{
				nb.Add(nc);
			}
		}

		if (nb.Count > 0)
		{
			int randp = Random.Range (0, nb.Count);
			mazeGTCell rc = nb[randp];
			pathRec.Push(rc);
			FindTheWay(rc);

		}
		else
		{
			return false;
		}




		return true;
	}

	void OnGUI()
	{
		if (GUILayout.Button ("gen"))
		{
			GenMaze(x, y);
		}
	}
}
