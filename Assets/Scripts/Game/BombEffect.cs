using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class BombEffect : MonoInheritance
{
// Parameter
	public List<Renderer> renderers;
	private float cubeSize {
		get {return Cube.instance.transform.localScale.x;}
	}
	private System.Action callback;

	public void StartEffect(Material material, System.Action callback)
	{
		this.callback = callback;
		foreach (Renderer r in renderers) {
			r.sharedMaterial = material;
		}

		TweenPosition _tween = TweenPosition.Begin(gameObject, 0.2f, transform.localPosition + new Vector3(0f, 0.5f, 0f));
		_tween.ignoreTimeScale = false;
		_tween.onFinished = (_t)=>{
			TweenTo _tweenTo = TweenTo.UpLeft;
			bool _setCallback = true;
			foreach (Renderer r in renderers) {
				TweenCube(r.gameObject, _tweenTo, _setCallback);
				_setCallback = false;
				_tweenTo++;
			}
		};
	}

	private void TweenCube(GameObject cube, TweenTo tweenTo, bool setCallback)
	{
		Vector3 _pos = new Vector3();
		switch (tweenTo) {
			case TweenTo.UpLeft:
				_pos.x -= cubeSize;
				_pos.z += cubeSize;
			break;
			case TweenTo.UpCenter:
				_pos.z += cubeSize;
			break;
			case TweenTo.UpRight:
				_pos.x += cubeSize;
				_pos.z += cubeSize;
			break;
			case TweenTo.CenterLeft:
				_pos.x -= cubeSize;
			break;
			case TweenTo.CenterRight:
				_pos.x += cubeSize;
			break;
			case TweenTo.DownLeft:
				_pos.x -= cubeSize;
				_pos.z -= cubeSize;
			break;
			case TweenTo.DownCenter:
				_pos.z -= cubeSize;
			break;
			case TweenTo.DownRight:
				_pos.x += cubeSize;
				_pos.z -= cubeSize;
			break;
		}
		TweenPosition _tween = TweenPosition.Begin(cube, 0.2f, _pos);
		_tween.ignoreTimeScale = false;
		if (setCallback) {
			_tween.onFinished = (_t)=>{
				if (gameObject != null) {
					callback();
					Destroy(gameObject);
				}
			};
		}
	}

//----------------
// enum
//----------------
	private enum TweenTo {
		UpLeft = 0,
		UpCenter = 1,
		UpRight = 2,
		CenterLeft = 3,
		CenterRight = 4,
		DownLeft = 5,
		DownCenter = 6,
		DownRight = 7,
	}
}
