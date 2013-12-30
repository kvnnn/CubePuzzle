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

//----------------
// Stage
//----------------
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
				_cell.Init(new IntVector2(x, z));

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

//----------------
// Cells
//----------------
	public IntVector2 GetDirectionPosition(IntVector2 position, Game.Direction direction)
	{
		switch (direction) {
			case Game.Direction.Up:
				position.y += 1;
			break;
			case Game.Direction.Down:
				position.y -= 1;
			break;
			case Game.Direction.Left:
				position.x -= 1;
			break;
			case Game.Direction.Right:
				position.x += 1;
			break;
		}
		return position;
	}

	public bool IsAvailable(IntVector2 position, Game.Direction direction)
	{
		IntVector2 _pos = GetDirectionPosition(position, direction);
		if (!cells.ContainsKey(_pos)) {return false;}
		Cell _cell = cells[_pos];

		return _cell.IsAvailable();
	}
}
