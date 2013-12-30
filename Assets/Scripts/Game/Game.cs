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

	public void Show()
	{
		gameObject.SetActive(true);
		nguiGame.gameObject.SetActive(true);

		cellManager.OnShow();
		cube.OnShow();
	}

	public void Hide()
	{
		gameObject.SetActive(false);
		nguiGame.gameObject.SetActive(false);

		cellManager.OnHide();
		cube.OnHide();
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
}
