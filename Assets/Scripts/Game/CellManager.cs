using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CellManager : SingletonMonoBehaviour<CellManager>
{
// Scene
	public Game game;
	public Cube cube {get {return game.cube;}}
	public GameObject cellPrefab;
// Parameter
	public Dictionary<IntVector2, Cell> cells;
	private IntVector2 maxSize = new IntVector2(12, 10);
	public Cell startCell {get {return GetRandomFreeCell();}}

	public IEnumerator OnShow()
	{
		Cell.ResetCount();
		yield return StartCoroutine(CreateStage(stages.Keys.ToArray()[Random.Range(0, stages.Count)]));
	}

	public void OnHide()
	{
		RemoveStage();
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
			return _pickabelCells[Random.Range(0, _pickabelCells.Count)];
		}
	}

	public bool IsStable()
	{
		foreach (Cell cell in cells.Values) {
			if (cell.isBombing) return false;
		}
		return true;
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

	public void BombSurround(IntVector2 pos)
	{
		for (int x = -1; x < 2; x++) {
			for (int y = -1; y < 2; y++) {
				if (x == 0 && y == 0) {continue;}
				IntVector2 _pos = new IntVector2(pos.x + x, pos.y + y);
				if (!cells.ContainsKey(_pos)) {continue;}
				Cell _cell = cells[_pos];
				_cell.BombChange();
			}
		}
	}

//----------------
// Stage
//----------------
	public IEnumerator CreateStage(int stageID)
	{
		// stageID = 3;
		IntVector2[] _stageData = stages[stageID];
		cells = new Dictionary<IntVector2, Cell>();
		Vector3 _scale = cellPrefab.transform.localScale;
		for (int x = 0; x < maxSize.x; x++) {
			for (int z = 0; z < maxSize.y; z++) {
				if (_stageData.Contains(new IntVector2(x, z))) {continue;}

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

	private Dictionary<int, IntVector2[]> stages_;
	private Dictionary<int, IntVector2[]> stages {
		get {
			if (stages_ == null) {
				stages_ = new Dictionary<int, IntVector2[]>(){
					{1, stage1},
					{2, stage2},
					{3, stage3},
				};
			}
			return stages_;
		}
	}
	private IntVector2[] stage1 = new IntVector2[]{
		new IntVector2(4,3), new IntVector2(4,4),
		new IntVector2(4,5), new IntVector2(4,6),
		new IntVector2(5,3), new IntVector2(5,4),
		new IntVector2(5,5), new IntVector2(5,6),
		new IntVector2(6,3), new IntVector2(6,4),
		new IntVector2(6,5), new IntVector2(6,6),
		new IntVector2(7,3), new IntVector2(7,4),
		new IntVector2(7,5), new IntVector2(7,6),
	};
	private IntVector2[] stage2 = new IntVector2[]{
		new IntVector2(0,0), new IntVector2(0,1),
		new IntVector2(0,2), new IntVector2(1,0),
		new IntVector2(1,1), new IntVector2(1,2),
		new IntVector2(2,0), new IntVector2(2,1),
		new IntVector2(2,2), new IntVector2(0,9),
		new IntVector2(0,8), new IntVector2(0,7),
		new IntVector2(1,9), new IntVector2(1,8),
		new IntVector2(1,7), new IntVector2(2,9),
		new IntVector2(2,8), new IntVector2(2,7),
		new IntVector2(9,0), new IntVector2(9,1),
		new IntVector2(9,2), new IntVector2(10,0),
		new IntVector2(10,1),new IntVector2(10,2),
		new IntVector2(11,0),new IntVector2(11,1),
		new IntVector2(11,2),new IntVector2(9,9),
		new IntVector2(9,8), new IntVector2(9,7),
		new IntVector2(10,9),new IntVector2(10,8),
		new IntVector2(10,7),new IntVector2(11,9),
		new IntVector2(11,8),new IntVector2(11,7),
	};
	private IntVector2[] stage3 = new IntVector2[]{
		new IntVector2(0,6), new IntVector2(0,5),
		new IntVector2(0,4), new IntVector2(0,3),
		new IntVector2(1,6), new IntVector2(1,5),
		new IntVector2(1,4), new IntVector2(1,3),
		new IntVector2(10,6),new IntVector2(10,5),
		new IntVector2(10,4),new IntVector2(10,3),
		new IntVector2(11,6),new IntVector2(11,5),
		new IntVector2(11,4),new IntVector2(11,3),
		new IntVector2(4,0), new IntVector2(4,1),
		new IntVector2(4,2), new IntVector2(5,0),
		new IntVector2(5,1), new IntVector2(5,2),
		new IntVector2(6,0), new IntVector2(6,1),
		new IntVector2(6,2), new IntVector2(7,0),
		new IntVector2(7,1), new IntVector2(7,2),
		new IntVector2(4,9), new IntVector2(4,8),
		new IntVector2(4,7), new IntVector2(5,9),
		new IntVector2(5,8), new IntVector2(5,7),
		new IntVector2(6,9), new IntVector2(6,8),
		new IntVector2(6,7), new IntVector2(7,9),
		new IntVector2(7,8), new IntVector2(7,7),
	};
// 0,9 1,9 2,9 3,9 4,9 5,9 6,9 7,9 8,9 9,9 10,9 11,9
// 0,8 1,8 2,8 3,8 4,8 5,8 6,8 7,8 8,8 9,8 10,8 11,8
// 0,7 1,7 2,7 3,7 4,7 5,7 6,7 7,7 8,7 9,7 10,7 11,7
// 0,6 1,6 2,6 3,6 4,6 5,6 6,6 7,6 8,6 9,6 10,6 11,6
// 0,5 1,5 2,5 3,5 4,5 5,5 6,5 7,5 8,5 9,5 10,5 11,5
// 0,4 1,4 2,4 3,4 4,4 5,4 6,4 7,4 8,4 9,4 10,4 11,4
// 0,3 1,3 2,3 3,3 4,3 5,3 6,3 7,3 8,3 9,3 10,3 11,3
// 0,2 1,2 2,2 3,2 4,2 5,2 6,2 7,2 8,2 9,2 10,2 11,2
// 0,1 1,1 2,1 3,1 4,1 5,1 6,1 7,1 8,1 9,1 10,1 11,1
// 0,0 1,0 2,0 3,0 4,0 5,0 6,0 7,0 8,0 9,0 10,0 11,0
}

