using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TopCube : MonoInheritance
{
	public void OnShow()
	{
		RotateRandomly();
	}

	private void RotateRandomly()
	{
		TweenRotation _rotate = TweenRotation.Begin(gameObject, Random.Range(2f, 4f), Quaternion.Euler(new Vector3(Random.Range(-360, 360), Random.Range(-360, 360), Random.Range(-360, 360))));
		_rotate.onFinished = (_tween)=>{
			RotateRandomly();
		};
	}
}
