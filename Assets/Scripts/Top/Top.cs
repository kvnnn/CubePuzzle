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
		cube.OnShow();
		nguiTop.gameObject.SetActive(true);
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
