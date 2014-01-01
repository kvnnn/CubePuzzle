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
	public UILabel pointLabel;
// Parameter
	public GameStatus status = GameStatus.End;
// Materials
	private Dictionary<string, Material> materials_;
	private Dictionary<string, Material> materials {
		get {
			if (materials_ == null) {
				materials_ = new Dictionary<string, Material>();
				string[] _names = new string[]{"Red", "Purple", "Blue", "Orange", "Yellow", "Green"};
				foreach (string matName in _names) {
					Material _mat = Resources.Load("Cubes/Materials/" + matName) as Material;
					materials_.Add(_mat.name, _mat);
				}
			}
			return materials_;
		}
	}
	private Material randomMaterial {
		get {return materials.Values.ToArray()[Random.Range(0, materials.Count - 1)];}
	}

	public IEnumerator Show()
	{
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
		// Manage Game
		ManageColoredCell();
		ManageGoalCell();

		// Debug
		// Cell.DebugCount();
	}

private int maxColoredCount = 80;
private int minColoredCount = 5;
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
		nextColoredTime = currentTime + Random.Range(3f, 5f);
	}

private int maxGoalCount = 3;
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

		minutesLabel.text = minutes.ToString("00");
		secondLabel.text = seconds.ToString("00");

		return _leftTime;
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
}
