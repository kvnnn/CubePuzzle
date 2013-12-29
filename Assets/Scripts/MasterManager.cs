using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MasterManager : SingletonMonoBehaviour<MasterManager>
{
	public Top topPage;

	protected override void Awake()
	{
		base.Awake();
	}

	void Start()
	{
		topPage.Show();
	}

}
