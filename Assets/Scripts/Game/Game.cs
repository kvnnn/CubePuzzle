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

	protected override void Awake()
	{
		base.Awake();
		Hide();
	}

	public void Show()
	{
		gameObject.SetActive(true);
		nguiGame.gameObject.SetActive(true);

		cellManager.Show();
	}

	public void Hide()
	{
		gameObject.SetActive(false);
		nguiGame.gameObject.SetActive(false);
	}

//----------------
// Touch Event
//----------------
	void PauseClick()
	{

	}
}
