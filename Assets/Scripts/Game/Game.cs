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
	public float currentTime = 0f;
	private float END_TIME = 180f;
	public Material[] materials;

	public IEnumerator Show()
	{
		gameObject.SetActive(true);
		nguiGame.gameObject.SetActive(true);
		status = GameStatus.End;

		yield return StartCoroutine(cellManager.OnShow());
		yield return StartCoroutine(cube.OnShow());
	}

	public void Hide()
	{
		gameObject.SetActive(false);
		nguiGame.gameObject.SetActive(false);
		status = GameStatus.End;

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
	}

	public void PuaseGame()
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
		UpdateTimeLabel();
	}

//----------------
// GUI
//----------------
	private void UpdateTimeLabel()
	{
		float _leftTime = END_TIME - currentTime;

		float minutes = Mathf.Floor(_leftTime/60);
		float seconds = Mathf.RoundToInt(_leftTime%60);
		seconds = Mathf.Max(0, seconds);

		minutesLabel.text = minutes.ToString("00");
		secondLabel.text = seconds.ToString("00");
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
