using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CellManager : SingletonMonoBehaviour<CellManager>
{
// Scene
	public Game game;
	public GameObject cellPrefab;

	private int MAX_SIZE = 10;

	public void Show()
	{
		Vector3 _scale = cellPrefab.transform.localScale;
		for (int x = 0; x < MAX_SIZE; x++) {
			for (int z = 0; z < MAX_SIZE; z++) {
				GameObject _object = (GameObject)Instantiate(cellPrefab, new Vector3(), new Quaternion());
				_object.transform.parent = transform;
				_object.transform.localPosition = new Vector3(_scale.x * x, 0f, _scale.z * z);
			}
		}
	}
}
