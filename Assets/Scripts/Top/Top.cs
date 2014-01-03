using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Top : SingletonMonoBehaviour<Top>
{
// Scene
	public TopCube cube;

// NGUI
	public GameObject nguiTop;
	public GameObject mainGo;
	public GameObject helpGo;

	public void Show()
	{
		mainGo.SetActive(true);
		helpGo.SetActive(false);
		gameObject.SetActive(true);
		nguiTop.gameObject.SetActive(true);

		cube.OnShow();
	}

	public void Hide()
	{
		gameObject.SetActive(false);
		nguiTop.gameObject.SetActive(false);
		helpGo.SetActive(false);
	}

//----------------
// Touch Event
//----------------
	void StartEasyClick()
	{
		Hide();
		MasterManager.instance.StartCoroutine(Game.instance.Show(true));
	}

	void StartHardClick()
	{
		Hide();
		MasterManager.instance.StartCoroutine(Game.instance.Show(false));
	}

	void SettingClick()
	{
		Settings.instance.Show();
	}

	void RankingClick()
	{
		Ranking.instance.Show();
	}

	void HelpClick()
	{
		mainGo.SetActive(false);
		helpGo.SetActive(true);
	}

	void HelpCloseClick()
	{
		mainGo.SetActive(true);
		helpGo.SetActive(false);
	}
}
