using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Top : SingletonMonoBehaviour<Top>
{
// Scene
	public TopCube cube;

// NGUI
	public GameObject nguiTop;

	public void Show()
	{
		gameObject.SetActive(true);
		nguiTop.gameObject.SetActive(true);

		cube.OnShow();
	}

	public void Hide()
	{
		gameObject.SetActive(false);
		nguiTop.gameObject.SetActive(false);
	}

//----------------
// Touch Event
//----------------
	void StartClick()
	{
		Hide();
		Game.instance.Show();
	}

	void SettingClick()
	{

	}

	void RankingClick()
	{

	}
}
