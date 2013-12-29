using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MasterManager : SingletonMonoBehaviour<MasterManager>
{
	protected override void Awake()
	{
		base.Awake();
	}

	void Start()
	{
		Top.instance.Show();
		Game.instance.Hide();
	}

}
