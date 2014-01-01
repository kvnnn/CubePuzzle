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
	public GameObject scoreLabelsGo;
	public UILabel scoreLabel;
	public GameObject countLabelsGo;
	public UILabel normalCountLabel;
	public UILabel goalCountLabel;
	public UILabel bombCountLabel;
// Parameter
	public GameStatus status = GameStatus.End;
	private bool isEasyMode = false;

	public IEnumerator Show(bool isEasyMode)
	{
		this.isEasyMode = isEasyMode;

		gameObject.SetActive(true);
		nguiGame.gameObject.SetActive(true);
		status = GameStatus.End;

		// OnShow
		yield return StartCoroutine(cellManager.OnShow());
		yield return StartCoroutine(cube.OnShow());
	}

	public void Hide()
	{
		gameObject.SetActive(false);
		nguiGame.gameObject.SetActive(false);
		timeLabelsGo.gameObject.SetActive(false);
		scoreLabelsGo.gameObject.SetActive(false);
		countLabelsGo.gameObject.SetActive(false);
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
		currentTime = 0f;
		nextColoredTime = currentTime;

		status = GameStatus.Play;

		UpdateTimeLabel();
		timeLabelsGo.gameObject.SetActive(true);

		score = 0;
		UpdateScoreLabel();
		scoreLabelsGo.gameObject.SetActive(true);

		UpdateCountLabel();
		countLabelsGo.gameObject.SetActive(true);
	}

	public void EndGame()
	{
		status = GameStatus.End;
		Hide();
		Top.instance.Show();
	}

	public void PauseGame()
	{
		status = GameStatus.Pause;
		Time.timeScale = 0f;
	}

	public void ResumeGame()
	{
		Time.timeScale = 1f;
		status = GameStatus.Play;
	}

//----------------
// Manage Game
//----------------
private float currentTime = 0f;
private const float END_TIME = 180f;
	private void Update()
	{
		if (!isPlay) {return;}
		// Time Label
		currentTime += Time.deltaTime;
		float _leftTime = UpdateTimeLabel();
		if (_leftTime <= 0) {
			EndGame();
		}
		// Score Label
		UpdateScoreLabel();
		// Count Label
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
		if (isEasyMode) {
			nextColoredTime = currentTime + Random.Range(4f, 6f);
		} else {
			nextColoredTime = currentTime + Random.Range(3f, 5f);
		}
	}

private int maxGoalCount {
	get {
		int _max = Mathf.Min((Cell.goalClearCount/5) + 1, 8);
		if (isEasyMode) _max = Mathf.Min((Cell.goalClearCount/5) + 1, 5);
		return _max;
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

public int score {get; set;}
	private void UpdateScoreLabel()
	{
		scoreLabel.text = string.Format("{0:#,###0}", score);
	}

	private void UpdateCountLabel()
	{
		normalCountLabel.text = Cell.coloredClearCount.ToString();
		goalCountLabel.text = Cell.goalClearCount.ToString();
		bombCountLabel.text = Cell.bombClearCount.ToString();
	}

//----------------
// Touch Event
//----------------
	void PauseClick()
	{
		PauseGame();
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
		Play,
		Pause,
		End,
	}
	public bool isPlay {get {return status == GameStatus.Play;}}
	public bool isPause {get {return status == GameStatus.Pause;}}
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
		get {return materials.Values.ToArray()[Random.Range(0, materials.Count - 1)];}
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
