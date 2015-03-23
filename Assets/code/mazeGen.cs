using UnityEngine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;


	public static class Extensions
	{
		public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, System.Random rng)
		{
			var e = source.ToArray();
			for (var i = e.Length - 1; i >= 0; i--)
			{
				var swapIndex = rng.Next(i + 1);
				yield return e[swapIndex];
				e[swapIndex] = e[i];
			}
		}
		
		public static CellState OppositeWall(this CellState orig)
		{
			return (CellState)(((int) orig >> 2) | ((int) orig << 2)) & CellState.Initial;
		}
	}
	
	[System.Flags]
	public enum CellState
	{
		Top = 1,
		Right = 2,
		Bottom = 4,
		Left = 8,
		Visited = 128,
		Initial = Top | Right | Bottom | Left,

	}


	public struct RemoveWallAction
	{
		public Vector2 Neighbour;
		public CellState Wall;
	}
	
	public class mazeGen
	{
		public readonly CellState[,] _cells;
		public readonly int _width;
		public readonly int _height;
		private readonly System.Random _rng;
		
		public mazeGen(int width, int height)
		{
			_width = width;
			_height = height;
			_cells = new CellState[width, height];
			for(var x=0; x<width; x++)
				for(var y=0; y<height; y++)
					_cells[x, y] = CellState.Initial;
		_rng = new System.Random();
			VisitCell(_rng.Next(width), _rng.Next(height));
		}

	
		bool hasFlag(CellState cs, CellState have)
		{
			return (cs | have) > 0;
		}

		public CellState this[int x, int y]
		{
			get { return _cells[x,y]; }
			set { _cells[x,y] = value; }
		}
		
	public IEnumerable<RemoveWallAction> GetNeighbours(Vector2 p)
		{
			if (p.x > 0) yield return new RemoveWallAction {Neighbour = new Vector2(p.x - 1, p.y), Wall = CellState.Left};
		if (p.y > 0) yield return new RemoveWallAction {Neighbour = new Vector2(p.x, p.y - 1), Wall = CellState.Top};
		if (p.x < _width-1) yield return new RemoveWallAction {Neighbour = new Vector2(p.x + 1, p.y), Wall = CellState.Right};
		if (p.y < _height-1) yield return new RemoveWallAction {Neighbour = new Vector2(p.x, p.y + 1), Wall = CellState.Bottom};
		}
		
		public void VisitCell(int x, int y)
		{
			this[x,y] |= CellState.Visited;
		foreach (var p in GetNeighbours(new Vector2((float)x, (float)y)).Shuffle(_rng).Where(z => !hasFlag((this[(int)z.Neighbour.x, (int)z.Neighbour.y]), CellState.Visited)))
			{
				this[x, y] -= p.Wall;
			this[(int)p.Neighbour.x, (int)p.Neighbour.y] -= p.Wall.OppositeWall();
			VisitCell((int)p.Neighbour.x, (int)p.Neighbour.y);
			}
		}
		
		public void Display()
		{
			var firstLine = string.Empty;
			for (var y = 0; y < _height; y++)
			{
				var sbTop = new StringBuilder();
				var sbMid = new StringBuilder();
				for (var x = 0; x < _width; x++)
				{
					sbTop.Append(hasFlag( this[x, y], CellState.Top) ? "+--" : "+  ");
					sbMid.Append(hasFlag( this[x, y], CellState.Left) ? "|  " : "   ");
				}
				if (firstLine == string.Empty)
					firstLine = sbTop.ToString();
				UnityEngine.Debug.Log(sbTop + "+");
				UnityEngine.Debug.Log(sbMid + "|");
				UnityEngine.Debug.Log(sbMid + "|");
			}
			UnityEngine.Debug.Log(firstLine);
		}
	}
	
//	class Program
//	{
//		static void Main(string[] args)
//		{
//			var maze = new Maze(20, 20);
//			maze.Display();
//		}
//	}
