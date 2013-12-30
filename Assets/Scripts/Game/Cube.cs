using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cube : SingletonMonoBehaviour<Cube>
{
// Scene
	public Game game;
	public CellManager cellManager {
		get {
			return game.cellManager;
		}
	}

// Parameter
	public Renderer upperSide;
	public Material upperMat {
		get {return upperSide.material;}
	}
	public Renderer downSide;
	public Material downMat {
		get {return downSide.material;}
	}
	public Renderer rightSide;
	public Material rightMat {
		get {return rightSide.material;}
	}
	public Renderer leftSide;
	public Material leftMat {
		get {return leftSide.material;}
	}
	public Renderer behindSide;
	public Material behindMat {
		get {return upperSide.material;}
	}
	public Renderer backSide;
	public Material backMat {
		get {return backSide.material;}
	}

	private IntVector2 position;
	private float speed = 5.0f;
	private Transform rotator;
	private float halfCubeSize {
		get {return transform.localScale.x/2;}
	}
	private bool rotating = false;

	public IEnumerator OnShow()
	{
		Vector3 _startPos = cellManager.cells[cellManager.startPos].transform.position;
		Vector3 _pos = transform.position;
		_pos.x = _startPos.x;
		_pos.z = _startPos.z;

		_pos.y = 10f;
		transform.position = _pos;
		_pos.y = halfCubeSize;

		gameObject.SetActive(true);
		TweenPosition _tweenPos = TweenPosition.Begin(gameObject, 0.25f, _pos);
		_tweenPos.onFinished = (_tween)=>{
			game.StartGame();
			MoveTo(cellManager.startPos);
			StartCube();
		};
		yield break;
	}

	public void OnHide()
	{
		StopCoroutine("ControlCoroutine");
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
				if (Input.GetKey(KeyCode.D)) {
					Move(Game.Direction.Right);
				} else if (Input.GetKey(KeyCode.A)) {
					Move(Game.Direction.Left);
				} else if (Input.GetKey(KeyCode.W)) {
					Move(Game.Direction.Up);
				} else if (Input.GetKey(KeyCode.S)) {
					Move(Game.Direction.Down);
				}
			}
			yield return false;
  	}
	}

	private void MoveTo(IntVector2 position)
	{
		MoveTo(cellManager.cells[position]);
	}

	private void MoveTo(Cell cell)
	{
		Vector3 _pos = transform.position;
		_pos.x = cell.transform.position.x;
		_pos.y = halfCubeSize;
		_pos.z = cell.transform.position.z;

		transform.position = _pos;
		position = cell.position;
	}

//----------------
// 回転
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

		MoveTo(cellManager.GetDirectionPosition(position, direction));

		rotating = false;
	}

	public void Move(Game.Direction direction)
	{
		if (!cellManager.IsAvailable(position, direction)) {return;}

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
}
