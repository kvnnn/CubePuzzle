using UnityEngine;
using System.Collections;

public class SingletonMonoBehaviour<T> : MonoInheritance
	where T : MonoInheritance
{
	protected static T classInstance;

	public static T instance {
		get {
			if (classInstance == null) {
				classInstance = FindObjectOfType (typeof(T)) as T;
				if (classInstance == null) {
					Debug.LogError (typeof(T) + " is nothing");
				}
			}
			return classInstance;
		}
	}

	protected virtual void Awake()
	{
		CheckInstance();
	}

	protected bool CheckInstance()
	{
		if ( this == instance ) {
			return true;
		}
		DestroyImmediate(this);
		return false;
	}

	protected void OnDestroy()
	{
		Destroy(gameObject);
	}

}
