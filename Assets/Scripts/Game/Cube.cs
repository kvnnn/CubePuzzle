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
	private IntVector2 position;
	private float speed = 5.0f;
	private Transform rotator;
	private float halfCubeSize {
		get {return transform.localScale.x/2;}
	}
	private bool rotating = false;

	public void OnShow()
	{
		MoveTo(cellManager.startPos);
		StartCube();
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
  		if (!rotating) {
				if (Input.GetKey(KeyCode.D)) {
					RotateCube(Vector3.right * halfCubeSize, -Vector3.forward);
				} else if (Input.GetKey(KeyCode.A)) {
					RotateCube(-Vector3.right * halfCubeSize, Vector3.forward);
				} else if (Input.GetKey(KeyCode.W)) {
					RotateCube(Vector3.forward * halfCubeSize, Vector3.right);
				} else if (Input.GetKey(KeyCode.S)) {
					RotateCube(-Vector3.forward * halfCubeSize, -Vector3.right);
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

	private void RotateCube(Vector3 refPoint, Vector3 rotationAxis)
	{
		rotating = true;
		StartCoroutine(RotateCubeCoroutine(refPoint, rotationAxis));
	}

	private IEnumerator RotateCubeCoroutine(Vector3 refPoint, Vector3 rotationAxis)
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
		rotating = false;
	}
}
