using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CellManager : SingletonMonoBehaviour<CellManager>
{
// Scene
	public Game game;
	public Cube cube {get {return game.cube;}}
	public GameObject cellPrefab;
// Parameter
	public Dictionary<IntVector2, Cell> cells;
	private IntVector2 maxSize = new IntVector2(12, 10);
	public IntVector2 startPos {get {return new IntVector2(maxSize.x/2, maxSize.y/2);}}

	public IEnumerator OnShow()
	{
		Cell.ResetCount();
		yield return StartCoroutine(CreateStage());
	}

	public void OnHide()
	{
		RemoveStage();
	}

//----------------
// Stage
//----------------
	public IEnumerator CreateStage(int id = 0)
	{
		cells = new Dictionary<IntVector2, Cell>();
		Vector3 _scale = cellPrefab.transform.localScale;
		for (int x = 0; x < maxSize.x; x++) {
			for (int z = 0; z < maxSize.y; z++) {
				GameObject _object = (GameObject)Instantiate(cellPrefab, new Vector3(), new Quaternion());
				_object.transform.parent = transform;
				_object.transform.localPosition = new Vector3(_scale.x * x, 10f, _scale.z * z);
				_object.name = string.Format("{0}:{1}", x, z);

				Cell _cell = _object.GetComponent<Cell>();
				_cell.game = game;
				_cell.Init(new IntVector2(x, z));

				cells[_cell.position] = _cell;

				TweenPosition.Begin(_object, 0.25f, new Vector3(_scale.x * x, 0f, _scale.z * z));
				yield return StartCoroutine(WaitForSecondsIgnoreTimeScale(0.025f));
			}
		}

		yield return StartCoroutine(WaitForSecondsIgnoreTimeScale(0.05f));
	}

	public void RemoveStage()
	{
		foreach (Transform child in transform) {
			Destroy(child.gameObject);
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

	public bool IsMovable(IntVector2 position, Game.Direction direction)
	{
		IntVector2 _pos = GetDirectionPosition(position, direction);
		return cells.ContainsKey(_pos);
	}

	public bool IsAvailable(IntVector2 position, Game.Direction direction, string matName)
	{
		IntVector2 _pos = GetDirectionPosition(position, direction);
		if (!cells.ContainsKey(_pos)) {return false;}
		Cell _cell = cells[_pos];

		return _cell.IsAvailable(matName);
	}

	public Cell GetRandomFreeCell()
	{
		List<Cell> _pickabelCells = new List<Cell>();
		foreach (Cell cell in cells.Values) {
			if (cell.position.Equals(cube.position)) {continue;}
			if (cell.isNormal) {
				_pickabelCells.Add(cell);
			}
		}
		if (_pickabelCells.Count <= 0) {
			return null;
		} else {
			return _pickabelCells[Random.Range(0, _pickabelCells.Count - 1)];
		}
	}

//----------------
// Bomb
//----------------
	public void BombSameColor(string matName)
	{
		foreach (Cell cell in cells.Values) {
			if (cell.isBomb && cell.matName == matName) {
				cell.BombEffect();
			}
		}
	}

	public void BombSurround(Cell cell)
	{

	}
}
