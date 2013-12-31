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
	private Material tileMat {
		get {return tile.sharedMaterial;}
		set {tile.sharedMaterial = value;}
	}
	private string matName {
		get {return tileMat.name;}
	}
	public Renderer icon;
	private Material iconMat {
		get {return icon.sharedMaterial;}
		set {icon.sharedMaterial = value;}
	}
// Materials
	private Dictionary<string, Material> materials_;
	public Dictionary<string, Material> materials {
		get {
			if (materials_ == null) {
				materials_ = new Dictionary<string, Material>();
				string[] _names = new string[]{"Bomb", "Heart", "HeartB", "Star", "StarB", "Time"};
				foreach (string matName in _names) {
					Material _mat = Resources.Load("Cubes/Icons/" + matName) as Material;
					materials_.Add(_mat.name, _mat);
				}
			}
			return materials_;
		}
	}

	public void Init(IntVector2 position)
	{
		this.position = position;
		this.currentType = CellType.Normal;
		this.itemType = ItemType.None;
	}

	public bool IsAvailable(string matName)
	{
		switch (currentType) {
			case CellType.Normal:
				return true;
			case CellType.Colored:
			case CellType.Goal:
				return this.matName == matName;
			case CellType.Item:
				return false;
		}
		return false;
	}

//----------------
// Change Type
//----------------
	public void ToNormal()
	{
		toNormal();
		tile.transform.localScale = new Vector3();
		tile.gameObject.SetActive(false);
	}

	public void ToColor(Material mat)
	{
		toColored();
		tile.gameObject.SetActive(true);
		tile.transform.localScale = new Vector3();
		tileMat = mat;

		TweenScale.Begin(tile.gameObject, 1f, new Vector3(1f, 1f, 1f));
	}

	public void ToGoal(Material mat)
	{
		toGoal();
		tile.gameObject.SetActive(true);
		tile.transform.localScale = new Vector3();
		tileMat = mat;

		TweenScale.Begin(tile.gameObject, 1f, new Vector3(1f, 1f, 1f));
	}

//----------------
// enum
//----------------
	public enum CellType {
		Normal,
		Colored,
		Goal,
		Item,
	}
	public enum ItemType {
		None,
		TimeIncrease,
		PointIncrease,
	}
	public bool isNormal {get {return currentType == CellType.Normal;}}
	private void toNormal() {currentType = CellType.Normal;}
	public bool isColored {get {return currentType == CellType.Colored;}}
	private void toColored() {currentType = CellType.Colored;}
	public bool isGoal {get {return currentType == CellType.Goal;}}
	private void toGoal() {currentType = CellType.Goal;}
	public bool isItem {get {return currentType == CellType.Item;}}
	private void toItem() {currentType = CellType.Item;}
}
