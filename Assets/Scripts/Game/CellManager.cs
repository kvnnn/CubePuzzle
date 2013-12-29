using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CellManager : SingletonMonoBehaviour<CellManager>
{
// Scene
	public Game game;
	public GameObject cellPrefab;

	private int MAX_SIZE_X = 12;
	private int MAX_SIZE_Y = 10;

	public void Show()
	{
		CreateStage();
	}

	public void CreateStage(int id = 0)
	{
		Vector3 _scale = cellPrefab.transform.localScale;
		for (int x = 0; x < MAX_SIZE_X; x++) {
			for (int z = 0; z < MAX_SIZE_Y; z++) {
				GameObject _object = (GameObject)Instantiate(cellPrefab, new Vector3(), new Quaternion());
				_object.transform.parent = transform;
				_object.transform.localPosition = new Vector3(_scale.x * x, 0f, _scale.z * z);
				_object.name = string.Format("{0}:{1}", x, z);
			}
		}
	}

	public void RemoveStage()
	{
		foreach (Transform child in transform) {
			Destroy(child);
		}
	}
}
