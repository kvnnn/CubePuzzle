using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cube : SingletonMonoBehaviour<Cube>
{
// Scene
	public Game game;
// Parameter
	private float speed = 5.0f;
	private Transform rotator;
	private float halfCubeSize {
		get {return transform.localScale.x/2;}
	}
	private bool rotating = false;

	private void Start()
	{
		rotator = (new GameObject("Rotator")).transform;
		rotator.parent = game.transform;
	}

	// TODO : 操作
	private void Update()
  {
		if (!rotating) {
			if (Input.GetKey(KeyCode.D)) {
				rotating = true;
				RotateCube(Vector3.right * halfCubeSize, -Vector3.forward);
			} else if (Input.GetKey(KeyCode.A)) {
				rotating = true;
				RotateCube(-Vector3.right * halfCubeSize, Vector3.forward);
			} else if (Input.GetKey(KeyCode.W)) {
				rotating = true;
				RotateCube(Vector3.forward * halfCubeSize, Vector3.right);
			} else if (Input.GetKey(KeyCode.S)) {
				rotating = true;
				RotateCube(-Vector3.forward * halfCubeSize, -Vector3.right);
			}
		}
	}

//----------------
// 回転処理
//----------------
	private void RotateCube(Vector3 refPoint, Vector3 rotationAxis)
	{
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
