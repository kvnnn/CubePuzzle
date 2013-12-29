using UnityEngine;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;

public class Yaml
{
	public static Regex YAML_IGNORE = new System.Text.RegularExpressions.Regex("(#.*$| )");
	public Dictionary<string, string> values {get; private set;}

	public void Load(string file)
	{
		if (values == null) {
			values = new Dictionary<string, string>();
		}

		TextAsset _textAsset = null;
		_textAsset = (TextAsset)Resources.Load(file);
		//Debug.Log(_textAsset);
		if (_textAsset != null) {
			string result = _textAsset.text;

			string[] lines = result.Split("\n" [0]);
			foreach(string line in lines)
			{
				var data = YAML_IGNORE.Replace(line,"");
				var item = data.Split(":"[0]);
				if (item[0] != string.Empty) {
					values[item[0]] = item[1];
				}
			}
		}
		Resources.UnloadAsset(_textAsset);
	}

	public void setDefault(string key, string v)
	{
		if (values == null) {
			values = new Dictionary<string, string>();
		}
		if (!values.ContainsKey(key)) {
			values[key] = v;
		}
	}

	public string v (string key, string defaultValue = null)
	{
		if (!values.ContainsKey(key)) {
			return defaultValue;
		}
		return values[key];
	}

	// for enum
	public T v<T>(string key, T defaultValue)
	{
		string _value = v(key);
		if (_value == null) {
			return defaultValue;
		} else {
			int _num;
			if (int.TryParse(_value, out _num)) {
				return (T)(object)_num;
			}

			return (T)System.Enum.Parse(typeof(T), _value);
		}
	}

	public int i(string key, int defaultValue)
	{
		string _value = v(key);
		if (_value == null) {
			return defaultValue;
		} else {
			return int.Parse(_value);
		}
	}

	public float f(string key, float defaultValue)
	{
		string _value = v(key);
		if (_value == null) {
			return defaultValue;
		} else {
			return float.Parse(_value);
		}
	}

	public bool b(string key, bool defaultValue)
	{
		string _value = v(key);
		if (_value == null) {
			return defaultValue;
		} else {
			return bool.Parse(_value);
		}
	}

	public void ShowDebug()
	{
		string _text = "";
		foreach (KeyValuePair<string, string> p in values) {
			_text += p.Key + " : " + p.Value.ToString() + "\n";
		}
		UnityEngine.Debug.Log(_text);
	}
}
