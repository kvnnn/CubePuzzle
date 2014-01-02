using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PlayerPrefs = PreviewLabs.PlayerPrefs;

public class MasterManager : SingletonMonoBehaviour<MasterManager>
{
	public Camera nguiCamera;

	protected override void Awake()
	{
		base.Awake();

		// Debug
		// PlayerPrefs.DeleteAll();

		Time.timeScale = 1f;
	}

	void Start()
	{
		Top.instance.Show();
		Game.instance.Hide();
		Ranking.instance.Hide();
		Settings.instance.Hide();
	}

	void OnApplicationQuit()
	{
		PlayerPrefs.Flush();
	}
}
