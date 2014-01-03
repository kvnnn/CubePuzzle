using UnityEngine;
using System;
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
	public ItemType currentItemType;
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
	public GameObject bombEffectPrefab;

	public void Init(IntVector2 position)
	{
		this.position = position;
		this.currentType = CellType.None;
		this.currentItemType = ItemType.None;
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
				AddClearCount(currentType);
				ToBomb();
			break;
			case CellType.Goal:
				AddClearCount(currentType);
				ToNormal();
				cellManager.BombSameColor(matName);
				game.AddItem();
				cellManager.GoalChangeSurround(position);
			break;
		}
	}

	public void GoalChange()
	{
		switch (currentType) {
			case CellType.Normal:
			case CellType.Bomb:
			case CellType.Item:
			case CellType.Goal:
			break;
			case CellType.Colored:
				AddClearCount(currentType);
				ToBomb();
			break;
		}
	}

	public void BombChange()
	{
		switch (currentType) {
			case CellType.Colored:
			case CellType.Goal:
				AutoChange();
			break;
			case CellType.Bomb:
				BombEffect();
			break;
			case CellType.Item:
				game.ActivateItem(currentItemType, position);
				ToNormal();
			break;
			case CellType.Normal:
			break;
		}
	}

	public void ToNormal()
	{
		if (isNormal) {return;}
		toNormal();

		tile.transform.localScale = Vector3.zero;
		tile.enabled = false;
		tile.gameObject.SetActive(false);
		icon.gameObject.SetActive(false);
	}

	public void ToColor(Material mat)
	{
		if (isColored) {return;}
		toColored();

		tile.transform.localScale = Vector3.zero;
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

		tile.transform.localScale = Vector3.zero;
		tile.enabled = false;
		tile.gameObject.SetActive(true);
		tileMat = mat;

		iconMat = materials["Star"];
		iconColor = tileColor;
		icon.gameObject.SetActive(true);

		TweenScale.Begin(tile.gameObject, 1f, Vector3.one);
	}

	public void ToBomb()
	{
		if (isBomb) {return;}
		toBomb();

		tile.transform.localScale = Vector3.one;
		tile.enabled = false;
		tile.gameObject.SetActive(true);

		iconMat = materials["Bomb"];
		iconColor = tileColor;
		icon.gameObject.SetActive(true);
	}

	public void ToRandomItem()
	{
		int _startIndex = 1;
		if (game.goalIncreaseCount > Game.MAX_GOAL_INCREASE) {
			_startIndex = 2;
		}
		int _enum = UnityEngine.Random.Range(_startIndex, Enum.GetNames(typeof(ItemType)).Length);
		ToItem((ItemType)_enum);
	}

	public void ToItem(ItemType itemType)
	{
		if (isItem) {return;}
		toItem(itemType);

		tile.transform.localScale = Vector3.zero;
		tile.enabled = false;
		tile.gameObject.SetActive(true);

		string _itemMat = "";
		switch (currentItemType) {
			case ItemType.TimeIncrease:
				_itemMat = "Time";
			break;
			case ItemType.PointIncrease:
				_itemMat = "Heart";
			break;
			case ItemType.GoalIncrease:
				_itemMat = "StarB";
			break;
			case ItemType.CrossBomb:
				_itemMat = "Cross2";
			break;
		}

		iconMat = materials[_itemMat];
		iconColor = Color.white;
		icon.gameObject.SetActive(true);

		TweenScale.Begin(tile.gameObject, 1f, Vector3.one);
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

	private void AddClearCount(CellType type)
	{
		clearCount[type]++;
		game.AddScore(type);
	}

//----------------
// Effect
//----------------
	private void GoalEffect()
	{

	}

public bool isBombing = false;
	public void BombEffect()
	{
		if (isBombing) {return;}
		isBombing = true;
		Vector3 _original = icon.transform.localScale;
		TweenScale _tween = TweenScale.Begin(icon.gameObject, 0.25f, _original * 1.1f);
		_tween.ignoreTimeScale = false;
		_tween.onFinished = (_t)=>{
			_tween = TweenScale.Begin(icon.gameObject, 0.25f, _original * 0.9f);
			_tween.ignoreTimeScale = false;
			_tween.onFinished = (__t)=>{
				_tween = TweenScale.Begin(icon.gameObject, 0.25f, _original * 1.1f);
				_tween.ignoreTimeScale = false;
				_tween.onFinished = (___t)=>{
					_tween = TweenScale.Begin(icon.gameObject, 0.25f, _original * 0.9f);
					_tween.ignoreTimeScale = false;
					_tween.onFinished = (____t)=>{
						_tween = TweenScale.Begin(icon.gameObject, 0.125f, _original);
						_tween.ignoreTimeScale = false;
						_tween.onFinished = (_____t)=>{
							GameObject _effectObject = Instantiate(bombEffectPrefab, transform.position, new Quaternion()) as GameObject;
							_effectObject.transform.parent = transform;
							_effectObject.transform.localScale = Vector3.one;
							_effectObject.transform.localPosition = Vector3.zero;
							BombEffect _effect = _effectObject.GetComponent<BombEffect>();
							_effect.StartEffect(tileMat,
								()=>{
									AddClearCount(currentType);
									ToNormal();
									cellManager.BombSurround(position);
									isBombing = false;
								}
							);
						};
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
		None = 0,
		GoalIncrease = 1,
		TimeIncrease = 2,
		PointIncrease = 3,
		CrossBomb = 4,
	}
	public bool isNormal {get {return currentType == CellType.Normal;}}
	public bool isColored {get {return currentType == CellType.Colored;}}
	public bool isGoal {get {return currentType == CellType.Goal;}}
	public bool isItem {get {return currentType == CellType.Item;}}
	public bool isBomb {get {return currentType == CellType.Bomb;}}
	private void toNormal() {
		SetCurrentCount(CellType.Normal);
		currentType = CellType.Normal;
		currentItemType = ItemType.None;
	}
	private void toColored() {
		SetCurrentCount(CellType.Colored);
		currentType = CellType.Colored;
		currentItemType = ItemType.None;
	}
	private void toGoal() {
		SetCurrentCount(CellType.Goal);
		currentType = CellType.Goal;
		currentItemType = ItemType.None;
	}
	private void toItem(ItemType itemType) {
		SetCurrentCount(CellType.Item);
		currentType = CellType.Item;
		this.currentItemType = itemType;
	}
	private void toBomb() {
		SetCurrentCount(CellType.Bomb);
		currentType = CellType.Bomb;
		currentItemType = ItemType.None;
	}

//----------------
// Materials
//----------------
	private Dictionary<string, Material> materials_;
	private Dictionary<string, Material> materials {
		get {
			if (materials_ == null) {
				materials_ = new Dictionary<string, Material>();
				string[] _names = new string[]{"Bomb", "Heart", "Star", "StarB", "Time", "Cross1", "Cross2"};
				foreach (string matName in _names) {
					Material _mat = Resources.Load("Cubes/Icons/" + matName) as Material;
					materials_.Add(_mat.name, _mat);
				}
			}
			return materials_;
		}
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
