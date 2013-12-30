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
	}

	public void Hide()
	{
		gameObject.SetActive(false);
		nguiGame.gameObject.SetActive(false);

		cellManager.OnHide();
	}

//----------------
// Touch Event
//----------------
	void PauseClick()
	{

	}
}
