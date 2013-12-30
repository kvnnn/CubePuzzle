using UnityEngine;
using System.Collections;

public struct IntVector2
{
	private int coordinateX, coordinateY;
	private bool hasValue_;
	public bool hasValue {
		get { return hasValue_; }
	}
	public string toString {
		get {
			return coordinateX + ", " + coordinateY;
		}
	}
	public string toDataString {
		get {
			return coordinateX + ":" + coordinateY;
		}
	}
	public const char SEPARATOR = ':';

	public IntVector2(int coordinateX, int coordinateY)
	{
		this.coordinateX = coordinateX;
		this.coordinateY = coordinateY;
		this.hasValue_ = true;
	}

	public IntVector2(bool isNull = true)
	{
		this.hasValue_ = !isNull;
		this.coordinateX = 0;
		this.coordinateY = 0;
	}

	public IntVector2(IntVector2 original)
	{
		this.coordinateX = original.x;
		this.coordinateY = original.y;
		this.hasValue_ = true;
	}

	public IntVector2(string inDataString)
	{
		string[] _data = inDataString.Split(SEPARATOR);
		this.coordinateX = int.Parse(_data[0]);
		this.coordinateY = int.Parse(_data[1]);
		this.hasValue_ = true;
	}

	public int x
	{
		set {
			coordinateX = value;
		}
		get {
			return coordinateX;
		}
	}

	public int y
	{
		set {
			coordinateY = value;
		}
		get {
			return coordinateY;
		}
	}

	public bool Equals(IntVector2 vector2)
	{
		return (x==vector2.x && y==vector2.y);
	}

}
