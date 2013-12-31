using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

	private Dictionary<string, Material> materials_;
	public Dictionary<string, Material> materials {
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
	private float currentTime = 0f;
	private const float END_TIME = 180f;

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

	public void StartGame()
	{
		status = GameStatus.Play;
		currentTime = 0f;

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
		status = GameStatus.Play;
		Time.timeScale = 1f;
	}

	private void Update()
	{
		if (!isPlay) { return; }
		currentTime += Time.deltaTime;
		float _leftTime = UpdateTimeLabel();
		if (_leftTime <= 0) {
			EndGame();
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

		minutesLabel.text = minutes.ToString("00");
		secondLabel.text = seconds.ToString("00");

		return _leftTime;
	}

//----------------
// Touch Event
//----------------
	void PauseClick()
	{

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
	public bool isPlay {
		get { return status == GameStatus.Play; }
	}
	public bool isPause {
		get { return status == GameStatus.Pause; }
	}
	public bool isEnd {
		get { return status == GameStatus.End; }
	}
}
