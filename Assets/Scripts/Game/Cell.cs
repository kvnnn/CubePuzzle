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
	private Dictionary<string, Material> materials {
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
	public void AutoChange()
	{
		switch (currentType) {
			case CellType.Normal:
			break;
			case CellType.Colored:
				ToNormal();
			break;
			case CellType.Goal:
				ToNormal();
			break;
			case CellType.Item:
				ToNormal();
			break;
		}
	}

	public void ToNormal()
	{
		if (isNormal) {return;}
		toNormal();
		tile.transform.localScale = new Vector3();
		tile.gameObject.SetActive(false);
		icon.gameObject.SetActive(false);
	}

	public void ToColor(Material mat)
	{
		if (isColored) {return;}
		toColored();
		tile.gameObject.SetActive(true);
		tile.transform.localScale = new Vector3();
		tileMat = mat;
		icon.gameObject.SetActive(false);

		TweenScale.Begin(tile.gameObject, 1f, new Vector3(1f, 1f, 1f));
	}

	public void ToGoal(Material mat)
	{
		if (isGoal) {return;}
		toGoal();
		tile.gameObject.SetActive(true);
		tile.transform.localScale = new Vector3();
		tileMat = mat;
		icon.gameObject.SetActive(true);
		iconMat = materials["Star"];

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
	public bool isColored {get {return currentType == CellType.Colored;}}
	public bool isGoal {get {return currentType == CellType.Goal;}}
	public bool isItem {get {return currentType == CellType.Item;}}
	private void toNormal() {currentType = CellType.Normal;}
	private void toColored() {currentType = CellType.Colored;}
	private void toGoal() {currentType = CellType.Goal;}
	private void toItem() {currentType = CellType.Item;}
}
