using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PlayerPrefs = PreviewLabs.PlayerPrefs;

public class MasterManager : SingletonMonoBehaviour<MasterManager>
{
	protected override void Awake()
	{
		base.Awake();
		Time.timeScale = 1f;
	}

	void Start()
	{
		Top.instance.Show();
		Game.instance.Hide();
		Ranking.instance.Hide();
	}

	void OnApplicationQuit()
	{
		PlayerPrefs.Flush();
	}
}
