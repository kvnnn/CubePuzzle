using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Game : SingletonMonoBehaviour<Game>
{
// Scene
	public CellManager cellManager;
	public Cube cube;
// NGUI
	public GameObject nguiGame;
	public GameObject timeLabelsGo;
	public UILabel minutesLabel;
	public UILabel secondLabel;
	public UILabel separatorLabel;
	public UILabel addedTimeLabel;
	public GameObject scoreLabelsGo;
	public UILabel scoreLabel;
	public UILabel comboLabel;
	public GameObject countLabelsGo;
	public UILabel coloredCountLabel;
	public UILabel goalCountLabel;
	public UILabel bombCountLabel;
	public GameObject pauseWindowGo;
	public Result resultWindow;
// Parameter
	public GameStatus status = GameStatus.Prepare;
	public bool isEasyMode = false;

	public IEnumerator Show(bool isEasyMode)
	{
		Time.timeScale = 1f;
		this.isEasyMode = isEasyMode;

		// show gameObject
		gameObject.SetActive(true);
		pauseWindowGo.gameObject.SetActive(false);
		resultWindow.gameObject.SetActive(false);
		nguiGame.gameObject.SetActive(true);
		status = GameStatus.Prepare;

		// reset label
		minutesLabel.text = "00";
		secondLabel.text = "00";
		scoreLabel.text = coloredCountLabel.text = goalCountLabel.text = bombCountLabel.text = "0";
		comboLabel.text = addedTimeLabel.text = "";

		// OnShow
		yield return StartCoroutine(cellManager.OnShow());
		yield return StartCoroutine(cube.OnShow());
	}

	public void Hide()
	{
		// hide gameObject
		gameObject.SetActive(false);
		nguiGame.gameObject.SetActive(false);
		pauseWindowGo.gameObject.SetActive(false);
		resultWindow.gameObject.SetActive(false);
		status = GameStatus.End;

		// OnHide
		cellManager.OnHide();
		cube.OnHide();
	}

//----------------
// Game Status
//----------------
	public void StartGame()
	{
		// reset param
		currentTime = 0f;
		nextColoredTime = currentTime;
		lastScoreAddedTime = -100f;
		score = combo = maxCombo = 0;

		status = GameStatus.Play;

		// reset label
		UpdateTimeLabel();
		UpdateScoreLabel();
		UpdateCountLabel();
	}

	public void EndGame()
	{
		status = GameStatus.End;
		Hide();
		Top.instance.Show();
	}

	public void PauseGame()
	{
		if (!isPlay) {return;}
		status = GameStatus.Pause;
		Time.timeScale = 0f;
		pauseWindowGo.gameObject.SetActive(true);
	}

	public void ResumeGame()
	{
		if (!isPause) {return;}
		pauseWindowGo.gameObject.SetActive(false);
		Time.timeScale = 1f;
		status = GameStatus.Play;
	}

	public void ShowResult()
	{
		status = GameStatus.End;
		resultWindow.Show();
	}

//----------------
// Manage Game
//----------------
private float currentTime = 0f;
private const float END_TIME = 180f;
	private void Update()
	{
		if (isPrepareEnd && cellManager.IsStable()) {
			ShowResult();
		}

		if (!isPlay) {return;}
		// Time Label
		currentTime += Time.deltaTime;
		CheckAddedTimeLabel();
		float _leftTime = UpdateTimeLabel();
		if (_leftTime <= 0) {
			status = GameStatus.PrepareEnd;
		}
		// Score Label
		UpdateScoreLabel();
		// Count Label
		CheckCombo();
		UpdateCountLabel();

		// Manage Game
		ManageColoredCell();
		ManageGoalCell();

		// Debug
		// Cell.DebugCount();
	}

private int maxColoredCount {
	get {return isEasyMode ? 50 : 70;}
}
private int minColoredCount {
	get {return isEasyMode ? 3 : 5;}
}
private float nextColoredTime = 0f;
	private void ManageColoredCell()
	{
		if (minColoredCount < Cell.coloredCount) {
			if (nextColoredTime > currentTime) {return;}
			if (Cell.coloredCount >= maxColoredCount) {return
				;}
		}
		Cell _cell = cellManager.GetRandomFreeCell();
		if (_cell == null) {return;}
		_cell.ToColor(randomMaterial);
		ColoredRandomNextTime();
	}

	private void ColoredRandomNextTime()
	{
		nextColoredTime = currentTime + Random.Range(1f, 3f);
	}

private int maxGoalCount {
	get {
		int _max = isEasyMode ? 5 : 10;
		return Mathf.Min((Cell.goalClearCount/5) + 1, _max);;
	}
}
	private void ManageGoalCell()
	{
		if (Cell.goalCount >= maxGoalCount) {return
			;}
		Cell _cell = cellManager.GetRandomFreeCell();
		if (_cell == null) {return;}
		_cell.ToGoal(randomMaterial);
	}

//----------------
// Item
//----------------
	public void AddItem()
	{
		if (Cell.goalClearCount%5 != 0) {
			if (Random.Range(0, 1000) > 25) {
				return;
			}
		}
		Cell _cell = cellManager.GetRandomFreeCell();
		_cell.ToRandomItem();
	}

	public void ActivateItem(Cell.ItemType itemType)
	{
		switch (itemType) {
			case Cell.ItemType.TimeIncrease:
				float _added = 20f;
				currentTime -= _added;
				UpdateAddedTimeLabel(_added);
			break;
			case Cell.ItemType.PointIncrease:
				CalculateScore(1000f);
			break;
		}
	}

//----------------
// Score
//----------------
public int score {get; private set;}
public int combo {get; private set;}
public int maxCombo {get; private set;}
private float comboRate {
	get {return isEasyMode ? 0.05f : 0.1f;}
}
private	float lastScoreAddedTime = -100f;
private const float comboTimer = 2f;
	public void AddScore(Cell.CellType type)
	{
		if (currentTime - lastScoreAddedTime <= comboTimer) {
			combo++;
			if (combo >= maxCombo) {
				maxCombo = combo;
			}
		} else {
			combo = 1;
		}
		lastScoreAddedTime = currentTime;

		float _baseScore = 0f;
		switch (type) {
			case Cell.CellType.Goal:
				_baseScore = 200f;
			break;
			case Cell.CellType.Colored:
				_baseScore = 50f;
			break;
			case Cell.CellType.Bomb:
				_baseScore = 20f;
			break;
			case Cell.CellType.Item:
			case Cell.CellType.Normal:
			break;
		}
		CalculateScore(_baseScore);
		lastScoreAddedTime = currentTime;
	}

	private void CalculateScore(float baseScore)
	{
		float _comboF = (float)combo;
		baseScore += baseScore * (_comboF * comboRate);
		score += (int)baseScore;
	}

	private void CheckCombo()
	{
		if (currentTime - lastScoreAddedTime > comboTimer) {
			combo = 1;
		}
	}

//----------------
// GUI
//----------------
	private float UpdateTimeLabel()
	{
		float _leftTime = END_TIME - currentTime;

		float minutes = Mathf.Floor(_leftTime/60);
		minutes = Mathf.Max(0, minutes);
		float seconds = Mathf.RoundToInt(_leftTime%60);
		seconds = Mathf.Max(0, seconds);

		if (minutes <= 0 && seconds <= 20f) {
			minutesLabel.color = Color.red;
			secondLabel.color = Color.red;
			separatorLabel.color = Color.red;
		} else {
			minutesLabel.color = Color.white;
			secondLabel.color = Color.white;
			separatorLabel.color = Color.white;
		}

		minutesLabel.text = minutes.ToString("00");
		secondLabel.text = seconds.ToString("00");

		return _leftTime;
	}

private float lastTimeAddedTime = 0f;
	private void CheckAddedTimeLabel()
	{
		if (currentTime - lastTimeAddedTime > 2f) {
			addedTimeLabel.text = "";
		}
	}

	private void UpdateAddedTimeLabel(float addTime)
	{
		addedTimeLabel.text = string.Format("+ {0}", addTime);
		lastTimeAddedTime = currentTime;
	}

	private void UpdateCountLabel()
	{
		coloredCountLabel.text = Cell.coloredClearCount.ToString();
		goalCountLabel.text = Cell.goalClearCount.ToString();
		bombCountLabel.text = Cell.bombClearCount.ToString();
	}

	private void UpdateScoreLabel()
	{
		scoreLabel.text = string.Format("{0:#,###0}", score);
		if (combo > 1) {
			if (combo >= 10) {
				int _index = (combo/10)%hardMaterials.Values.Count;
				Material _mat = hardMaterials.Values.ToArray()[_index];
				comboLabel.color = _mat.color;
			} else {
				comboLabel.color = Color.white;
			}
			comboLabel.text = string.Format("{0} COMBO !", combo);
		} else {
			comboLabel.text = "";
		}
	}


//----------------
// Touch Event
//----------------
	void PauseClick()
	{
		PauseGame();
	}

	void PlayClick()
	{
		ResumeGame();
	}

	void TitleClick()
	{
		EndGame();
	}

	void RestartClick()
	{
		status = GameStatus.Prepare;
		cellManager.OnHide();
		cube.OnHide();
		StartCoroutine(Show(isEasyMode));
	}

//----------------
// enum
//----------------
	public enum Direction {
		Up,
		Down,
		Right,
		Left,
	}
	public enum GameStatus {
		Prepare,
		Play,
		Pause,
		PrepareEnd,
		End,
	}
	public bool isPrepare {get {return status == GameStatus.Prepare;}}
	public bool isPlay {get {return status == GameStatus.Play;}}
	public bool isPause {get {return status == GameStatus.Pause;}}
	public bool isPrepareEnd {get {return status == GameStatus.PrepareEnd;}}
	public bool isEnd {get {return status == GameStatus.End;}}

//----------------
// Materials
//----------------
	private Dictionary<string, Material> materials {
		get {
			return isEasyMode ? easyMaterials : hardMaterials;
		}
	}
	private Dictionary<string, Material> easyMaterials_;
	private Dictionary<string, Material> easyMaterials {
		get {
			if (easyMaterials_ == null) {
				easyMaterials_ = new Dictionary<string, Material>();
				easyMaterials_.Add("Red", hardMaterials["Red"]);
				easyMaterials_.Add("Blue", hardMaterials["Blue"]);
				easyMaterials_.Add("Green", hardMaterials["Green"]);
			}
			return easyMaterials_;
		}
	}
	private Dictionary<string, Material> hardMaterials_;
	private Dictionary<string, Material> hardMaterials {
		get {
			if (hardMaterials_ == null) {
				hardMaterials_ = new Dictionary<string, Material>();
				string[] _names = new string[]{"Red", "Purple", "Blue", "LightBlue", "Yellow", "Green"};
				foreach (string matName in _names) {
					Material _mat = Resources.Load("Cubes/Materials/" + matName) as Material;
					hardMaterials_.Add(_mat.name, _mat);
				}
			}
			return hardMaterials_;
		}
	}
	private Material randomMaterial {
		get {return materials.Values.ToArray()[Random.Range(0, materials.Count)];}
	}
	public Material upMaterial {
		get {
			string _key = isEasyMode ? "Red" : "Red";
			return materials[_key];
		}
	}
	public Material downMaterial {
		get {
			string _key = isEasyMode ? "Red" : "Purple";
			return materials[_key];
		}
	}
	public Material leftMaterial {
		get {
			string _key = isEasyMode ? "Green" : "Green";
			return materials[_key];
		}
	}
	public Material rightMaterial {
		get {
			string _key = isEasyMode ? "Green" : "Yellow";
			return materials[_key];
		}
	}
	public Material backMaterial {
		get {
			string _key = isEasyMode ? "Blue" : "Blue";
			return materials[_key];
		}
	}
	public Material frontMaterial {
		get {
			string _key = isEasyMode ? "Blue" : "LightBlue";
			return materials[_key];
		}
	}
}
