using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cell : MonoInheritance
{
// Scene
	public Game game;
	public CellManager cellManager {get {return game.cellManager;}}
// Parameter
	public IntVector2 position;
	public CellType currentType;
	public ItemType itemType;
	public Renderer tile;
	private Material tileMat {
		get {return tile.sharedMaterial;}
		set {tile.sharedMaterial = value;}
	}
	private Color tileColor {
		get {return tile.sharedMaterial.color;}
	}
	public string matName {get {return tileMat.name;}}
	public Renderer icon;
	private Material iconMat {
		get {return icon.material;}
		set {icon.material = value;}
	}
	private Color iconColor {
		get {return icon.material.color;}
		set {icon.material.color = value;}
	}
// Materials
	private Dictionary<string, Material> materials_;
	private Dictionary<string, Material> materials {
		get {
			if (materials_ == null) {
				materials_ = new Dictionary<string, Material>();
				string[] _names = new string[]{"Bomb", "Heart", "Star", "Time"};
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
		this.currentType = CellType.None;
		this.itemType = ItemType.None;
		ToNormal();
	}

	public bool IsAvailable(string matName)
	{
		switch (currentType) {
			case CellType.Normal:
			case CellType.Bomb:
				return true;
			case CellType.Item:
				return false;
			case CellType.Colored:
			case CellType.Goal:
				return this.matName == matName;
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
			case CellType.Bomb:
			case CellType.Item:
			break;
			case CellType.Colored:
				clearCount[currentType]++;
				ToBomb();
			break;
			case CellType.Goal:
				clearCount[currentType]++;
				ToNormal();
				cellManager.BombSameColor(matName);
			break;
		}
	}

	public void ToNormal()
	{
		if (isNormal) {return;}
		toNormal();

		tile.transform.localScale = new Vector3();
		tile.enabled = false;
		tile.gameObject.SetActive(false);
		icon.gameObject.SetActive(false);
	}

	public void ToColor(Material mat)
	{
		if (isColored) {return;}
		toColored();

		tile.transform.localScale = new Vector3();
		tile.enabled = true;
		tile.gameObject.SetActive(true);
		tileMat = mat;

		icon.gameObject.SetActive(false);

		TweenScale.Begin(tile.gameObject, 1f, new Vector3(1f, 1f, 1f));
	}

	public void ToGoal(Material mat)
	{
		if (isGoal) {return;}
		toGoal();

		tile.transform.localScale = new Vector3();
		tile.enabled = false;
		tile.gameObject.SetActive(true);
		tileMat = mat;

		iconColor = tileColor;
		icon.gameObject.SetActive(true);
		iconMat = materials["Star"];
		iconColor = tileColor;

		TweenScale.Begin(tile.gameObject, 1f, new Vector3(1f, 1f, 1f));
	}

	public void ToBomb()
	{
		if (isBomb) {return;}
		toBomb();

		tile.transform.localScale = Vector3.one;
		tile.enabled = false;
		tile.gameObject.SetActive(true);

		icon.gameObject.SetActive(true);
		iconMat = materials["Bomb"];
		iconColor = tileColor;
	}

//----------------
// Count
//----------------
	private void SetCurrentCount(CellType to)
	{
		if (currentType != CellType.None) {
			currentCount[currentType]--;
		}
		currentCount[to]++;
	}

//----------------
// Effect
//----------------
	private void GoalEffect()
	{

	}

private bool isBombing = false;
	public void BombEffect()
	{
		if (isBombing) {return;}
		isBombing = true;
		Vector3 _original = icon.transform.localScale;
		TweenScale _tween1 = TweenScale.Begin(icon.gameObject, 0.3f, _original * 1.1f);
		_tween1.onFinished = (_t)=>{
			TweenScale _tween2 = TweenScale.Begin(icon.gameObject, 0.3f, _original * 0.9f);
			_tween2.onFinished = (__t)=>{
				TweenScale _tween3 = TweenScale.Begin(icon.gameObject, 0.3f, _original * 1.1f);
				_tween3.onFinished = (___t)=>{
					TweenScale _tween4 = TweenScale.Begin(icon.gameObject, 0.3f, _original * 0.9f);
					_tween4.onFinished = (____t)=>{
						ToNormal();
						cellManager.BombSurround(this);
						isBombing = false;
					};
				};
			};
		};
	}

//----------------
// enum
//----------------
	public enum CellType {
		None,
		Normal,
		Colored,
		Goal,
		Item,
		Bomb,
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
	public bool isBomb {get {return currentType == CellType.Bomb;}}
	private void toNormal() {
		SetCurrentCount(CellType.Normal);
		currentType = CellType.Normal;
	}
	private void toColored() {
		SetCurrentCount(CellType.Colored);
		currentType = CellType.Colored;
	}
	private void toGoal() {
		SetCurrentCount(CellType.Goal);
		currentType = CellType.Goal;
	}
	private void toItem() {
		SetCurrentCount(CellType.Item);
		currentType = CellType.Item;
	}
	private void toBomb() {
		SetCurrentCount(CellType.Bomb);
		currentType = CellType.Bomb;
	}

//----------------
// Static
//----------------
	public static Dictionary<CellType, int> currentCount;
	public static Dictionary<CellType, int> clearCount;
	public static void ResetCount()
	{
		currentCount = new Dictionary<CellType, int>(){
			{CellType.Normal, 0},
			{CellType.Colored, 0},
			{CellType.Goal, 0},
			{CellType.Item, 0},
			{CellType.Bomb, 0},
		};
		clearCount = new Dictionary<CellType, int>(){
			{CellType.Normal, 0},
			{CellType.Colored, 0},
			{CellType.Goal, 0},
			{CellType.Item, 0},
			{CellType.Bomb, 0},
		};
	}
	public static int normalCount {get {return currentCount[CellType.Normal];}}
	public static int coloredCount {get {return currentCount[CellType.Colored];}}
	public static int goalCount {get {return currentCount[CellType.Goal];}}
	public static int itemCount {get {return currentCount[CellType.Item];}}
	public static int bombCount {get {return currentCount[CellType.Bomb];}}
	public static int normalClearCount {get {return clearCount[CellType.Normal];}}
	public static int coloredClearCount {get {return clearCount[CellType.Colored];}}
	public static int goalClearCount {get {return clearCount[CellType.Goal];}}
	public static int itemClearCount {get {return clearCount[CellType.Item];}}
	public static int bombClearCount {get {return clearCount[CellType.Bomb];}}
	public static void DebugCount()
	{
		string _log = "Current" + "\n";
		foreach (KeyValuePair<CellType, int> kvp in currentCount) {
			_log += kvp.Key + " : " + kvp.Value;
			_log += "\n";
		}
		UnityEngine.Debug.Log(_log);

		_log = "Clear" + "\n";
		foreach (KeyValuePair<CellType, int> kvp in clearCount) {
			_log += kvp.Key + " : " + kvp.Value;
			_log += "\n";
		}
		UnityEngine.Debug.Log(_log);
	}
}
