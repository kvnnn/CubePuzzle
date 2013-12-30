using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CellManager : SingletonMonoBehaviour<CellManager>
{
// Scene
	public Game game;
	public GameObject cellPrefab;

// Parameter
	public Dictionary<IntVector2, Cell> cells;
	private IntVector2 maxSize = new IntVector2(12, 10);
	public IntVector2 startPos {
		get {
			return new IntVector2(maxSize.x/2, maxSize.y/2);
		}
	}

	public void OnShow()
	{
		CreateStage();
	}

	public void OnHide()
	{
		RemoveStage();
	}

	public void CreateStage(int id = 0)
	{
		cells = new Dictionary<IntVector2, Cell>();
		Vector3 _scale = cellPrefab.transform.localScale;
		for (int x = 0; x < maxSize.x; x++) {
			for (int z = 0; z < maxSize.y; z++) {
				GameObject _object = (GameObject)Instantiate(cellPrefab, new Vector3(), new Quaternion());
				_object.transform.parent = transform;
				_object.transform.localPosition = new Vector3(_scale.x * x, 0f, _scale.z * z);
				_object.name = string.Format("{0}:{1}", x, z);

				Cell _cell = _object.GetComponent<Cell>();
				_cell.game = game;
				_cell.position = new IntVector2(x, z);

				cells[_cell.position] = _cell;
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
