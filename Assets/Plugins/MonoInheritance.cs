using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonoInheritance : MonoBehaviour
{

	public bool isTouch
	{
		get {
			return (Input.GetMouseButtonDown (0) || (Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Began));
		}
	}

	public bool isTouchEnd
	{
		get {
			return (Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended));
		}
	}

	public bool isMove
	{
		get {
			return (Input.GetMouseButton(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved));
		}
	}

	public bool isUntouch
	{
		get {
			return !isTouch && !isMove && !isTouchEnd;
		}
	}

}
