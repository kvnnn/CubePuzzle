using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cell : MonoInheritance
{
// Scene
	public Game game;

// Parameter
	public IntVector2 position;
	public CellType currentType;
	public ItemType itemType;
	public Renderer tile;
	public Material tileMat {
		get {return tile.material;}
	}

	public void Init(IntVector2 position)
	{
		this.position = position;
		this.currentType = CellType.Normal;
		this.itemType = ItemType.None;
	}

	public bool IsAvailable()
	{
		return true;
	}

//----------------
// enum
//----------------
	public enum CellType {
		Normal,
		Colored,
		Goal,
		Item
	}
	public enum ItemType {
		None,
		TimeIncrease,
		PointIncrease,
	}
}
