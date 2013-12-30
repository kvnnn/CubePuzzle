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




	public enum CellType {
		Empty,
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
