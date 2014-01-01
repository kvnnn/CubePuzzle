using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cube : SingletonMonoBehaviour<Cube>
{
// Scene
	public Game game;
	public CellManager cellManager {get {return game.cellManager;}}
// Parameter
	public IntVector2 position;
	private Dictionary<SidePosition, string> sides = new Dictionary<SidePosition, string>();
	private float speed = 5.0f;
	private Transform rotator;
	private float halfCubeSize {
		get {return transform.localScale.x/2;}
	}
	private bool rotating = false;

	public IEnumerator OnShow()
	{
		// Set To Start Position
		Cell _startCell = cellManager.startCell;
		Vector3 _startPos = _startCell.transform.position;
		Vector3 _pos = transform.position;
		_pos.x = _startPos.x;
		_pos.z = _startPos.z;
		_pos.y = 10f;
		transform.position = _pos;
		_pos.y = halfCubeSize;

		InitSides();

		gameObject.SetActive(true);
		TweenPosition _tweenPos = TweenPosition.Begin(gameObject, 0.25f, _pos);
		_tweenPos.onFinished = (_tween)=>{
			game.StartGame();
			AdjustTo(_startCell);
			StartCube();
		};
		yield break;
	}

	public void OnHide()
	{
		StopCoroutine("ControlCoroutine");
		gameObject.SetActive(false);
	}

//----------------
// 操作
//----------------
	private void StartCube()
	{
		rotator = (new GameObject("Rotator")).transform;
		rotator.parent = game.transform;

		StartCoroutine("ControlCoroutine");
	}

	private IEnumerator ControlCoroutine()
  {
  	while (true) {
  		if (!rotating && game.isPlay) {
				if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
					RotateTo(Game.Direction.Right);
				} else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
					RotateTo(Game.Direction.Left);
				} else if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
					RotateTo(Game.Direction.Up);
				} else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
					RotateTo(Game.Direction.Down);
				}
			}
			yield return false;
  	}
	}

//----------------
// 回転・移動
//----------------
	private void RotateCube(Vector3 refPoint, Vector3 rotationAxis, Game.Direction direction)
	{
		rotating = true;
		StartCoroutine(RotateCubeCoroutine(refPoint, rotationAxis, direction));
	}

	private IEnumerator RotateCubeCoroutine(Vector3 refPoint, Vector3 rotationAxis, Game.Direction direction)
	{
		rotator.localRotation = Quaternion.identity;
		rotator.position = transform.position - Vector3.up * halfCubeSize + refPoint;
		transform.parent = rotator;
		float _angle = 0;
		while(_angle < 90.0f) {
			_angle += Time.deltaTime * 90.0f * speed;
			rotator.rotation = Quaternion.AngleAxis(Mathf.Min(_angle, 90.0f), rotationAxis);
			yield return false;
		}
		transform.parent = game.transform;

		AdjustTo(cellManager.GetDirectionPosition(position, direction));
		rotating = false;
	}

	public void RotateTo(Game.Direction direction)
	{
		if (!cellManager.IsAvailable(position, direction, NextMatColor(direction))) {
			return;
		}

		switch (direction) {
			case Game.Direction.Up:
				RotateCube(Vector3.forward * halfCubeSize, Vector3.right, direction);
			break;
			case Game.Direction.Down:
				RotateCube(-Vector3.forward * halfCubeSize, -Vector3.right, direction);
			break;
			case Game.Direction.Left:
				RotateCube(-Vector3.right * halfCubeSize, Vector3.forward, direction);
			break;
			case Game.Direction.Right:
				RotateCube(Vector3.right * halfCubeSize, -Vector3.forward, direction);
			break;
		}
	}

	private void AdjustTo(IntVector2 position)
	{
		AdjustTo(cellManager.cells[position]);
	}

	private void AdjustTo(Cell cell)
	{
		Vector3 _pos = transform.position;
		_pos.x = cell.transform.position.x;
		_pos.y = halfCubeSize;
		_pos.z = cell.transform.position.z;

		transform.position = _pos;
		position = cell.position;

		SetCurrentSide();
		cell.AutoChange();
	}

//----------------
// 面管理
//----------------
	private void SetCurrentSide()
	{
		foreach (Transform _trans in transform) {
			Vector3 _pos = _trans.position - transform.position;
			if (_pos.x > 0.45f) {
				sides[SidePosition.Right] = _trans.renderer.sharedMaterial.name;
			} else if (_pos.x < -0.45f) {
				sides[SidePosition.Left] = _trans.renderer.sharedMaterial.name;
			} else if (_pos.y > 0.45f) {
				sides[SidePosition.Up] = _trans.renderer.sharedMaterial.name;
			} else if (_pos.y < -0.45f) {
				sides[SidePosition.Down] = _trans.renderer.sharedMaterial.name;
			} else if (_pos.z > 0.45f) {
				sides[SidePosition.Back] = _trans.renderer.sharedMaterial.name;
			} else if (_pos.z < -0.45f) {
				sides[SidePosition.Front] = _trans.renderer.sharedMaterial.name;
			}
		}
	}

	private string NextMatColor(Game.Direction direction)
	{
		string _matName = "";
		switch (direction) {
			case Game.Direction.Up:
				_matName = sides[SidePosition.Front];
			break;
			case Game.Direction.Down:
				_matName = sides[SidePosition.Back];
			break;
			case Game.Direction.Left:
				_matName = sides[SidePosition.Right];
			break;
			case Game.Direction.Right:
				_matName = sides[SidePosition.Left];
			break;
		}
		return _matName;
	}

	private void InitSides()
	{
		foreach (Transform _trans in transform) {
			Vector3 _pos = _trans.position - transform.position;
			if (_pos.x > 0.45f) {
				_trans.renderer.sharedMaterial = game.rightMaterial;
			} else if (_pos.x < -0.45f) {
				_trans.renderer.sharedMaterial = game.leftMaterial;
			} else if (_pos.y > 0.45f) {
				_trans.renderer.sharedMaterial = game.upMaterial;
			} else if (_pos.y < -0.45f) {
				_trans.renderer.sharedMaterial = game.downMaterial;
			} else if (_pos.z > 0.45f) {
				_trans.renderer.sharedMaterial = game.backMaterial;
			} else if (_pos.z < -0.45f) {
				_trans.renderer.sharedMaterial = game.frontMaterial;
			}
		}
	}

//----------------
// enum
//----------------
	public enum SidePosition {
		Up,
		Down,
		Right,
		Left,
		Front,
		Back,
	}
}
