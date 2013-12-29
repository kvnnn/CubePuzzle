using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace PreviewLabs
{
	public static class PlayerPrefs
	{
		private static Hashtable playerPrefsHashtable = new Hashtable();

		private static bool hashTableChanged = false;
		private static string serializedOutput = "";
		private static string serializedInput = "";

		private const string PARAMETERS_SEPARATOR = ";";
		private const string KEY_VALUE_SEPARATOR = ":";

		private const string ARRAY_MARK = ".A";
		private const string ARRAY_INDEX_MARK = "A.";

		private static readonly string fileName = Application.persistentDataPath + "/PlayerPrefs.txt";

		static PlayerPrefs()
		{
			//load previous settings
			StreamReader fileReader = null;

			if (File.Exists(fileName)) {
				fileReader = new StreamReader(fileName);
				serializedInput = fileReader.ReadLine();
				Deserialize();
				fileReader.Close();
			}
		}

		public static bool HasKey(string key, bool check = true)
		{
			if (check && playerPrefsHashtable.ContainsKey(key + ARRAY_MARK)) return true;
			return playerPrefsHashtable.ContainsKey(key);
		}

		public static void SetString(string key, string value)
		{
			if (!playerPrefsHashtable.ContainsKey(key))
			{
				playerPrefsHashtable.Add(key, value);
			} else {
				playerPrefsHashtable[key] = value;
			}

			hashTableChanged = true;
		}

		public static void SetInt(string key, int value)
		{
			if (!playerPrefsHashtable.ContainsKey(key)) {
				playerPrefsHashtable.Add(key, value);
			} else {
				playerPrefsHashtable[key] = value;
			}

			hashTableChanged = true;
		}

		public static void SetFloat(string key, float value)
		{
			if (!playerPrefsHashtable.ContainsKey(key)) {
				playerPrefsHashtable.Add(key, value);
			} else {
				playerPrefsHashtable[key] = value;
			}

			hashTableChanged = true;
		}

		public static void SetBool(string key, bool value)
		{
			if (!playerPrefsHashtable.ContainsKey(key)) {
				playerPrefsHashtable.Add(key, value);
			} else {
				playerPrefsHashtable[key] = value;
			}

			hashTableChanged = true;
		}

// For Array
		public static void SetArray<T>(string key, T[] value)
		{
			int _length = value.Length;
			SetInt(key + ARRAY_MARK, _length);

			for (int i = 0; i < _length; i++) {
				string _key = ARRAY_INDEX_MARK + key + i;
				if (!playerPrefsHashtable.ContainsKey(_key)) {
					playerPrefsHashtable.Add(_key, value[i]);
				} else {
					playerPrefsHashtable[_key] = value[i];
				}
			}

			hashTableChanged = true;
		}

		public static void SetArray<T>(string key, List<T> value)
		{
			SetArray<T>(key, value.ToArray());
		}

		public static string GetString(string key)
		{
			if (playerPrefsHashtable.ContainsKey(key)) {
				return playerPrefsHashtable[key].ToString();
			}

			return null;
		}

		public static string GetString(string key, string defaultValue)
		{
			if (playerPrefsHashtable.ContainsKey(key)) {
				return playerPrefsHashtable[key].ToString();
			} else {
				playerPrefsHashtable.Add(key, defaultValue);
				hashTableChanged = true;
				return defaultValue;
			}
		}

		public static int GetInt(string key)
		{
			if (playerPrefsHashtable.ContainsKey(key)) {
				return (int) playerPrefsHashtable[key];
			}

			return 0;
		}

		public static int GetInt(string key, int defaultValue)
		{
			if (playerPrefsHashtable.ContainsKey(key)) {
				return (int) playerPrefsHashtable[key];
			} else {
				playerPrefsHashtable.Add(key, defaultValue);
				hashTableChanged = true;
				return defaultValue;
			}
		}

		public static float GetFloat(string key)
		{
			if (playerPrefsHashtable.ContainsKey(key)) {
				return (float) playerPrefsHashtable[key];
			}

			return 0.0f;
		}

		public static float GetFloat(string key, float defaultValue)
		{
			if (playerPrefsHashtable.ContainsKey(key)) {
				return (float) playerPrefsHashtable[key];
			} else {
				playerPrefsHashtable.Add(key, defaultValue);
				hashTableChanged = true;
				return defaultValue;
			}
		}

		public static bool GetBool(string key)
		{
			if (playerPrefsHashtable.ContainsKey(key)) {
				return (bool) playerPrefsHashtable[key];
			}

			return false;
		}

		public static bool GetBool(string key, bool defaultValue)
		{
			if (playerPrefsHashtable.ContainsKey(key)) {
				return (bool) playerPrefsHashtable[key];
			} else {
				playerPrefsHashtable.Add(key, defaultValue);
				hashTableChanged = true;
				return defaultValue;
			}
		}

		public static List<T> GetArray<T>(string key)
		{
			int _count = GetInt(key + ARRAY_MARK, -1);
			if (_count == -1) return null;

			List<T> _value = new List<T>();
			for (int i = 0; i < _count; i++) {
				string _key = ARRAY_INDEX_MARK + key + i;
				_value.Add( (T)playerPrefsHashtable[_key] );
			}

			return _value;
		}

		public static List<T> GetArray<T>(string key, List<T> defaultValue)
		{
			int _count = GetInt(key + ARRAY_MARK, -1);
			if (_count == -1) {
				SetArray<T>(key, defaultValue);
				return defaultValue;
			}

			return GetArray<T>(key);
		}

		public static void DeleteKey(string key, bool check = true)
		{
			if (check && HasKey(ARRAY_INDEX_MARK + key + "0")) DeleteArray(key);
			playerPrefsHashtable.Remove(key);
			hashTableChanged = true;
		}

		private static void DeleteArray(string key)
		{
			int _count = GetInt(key + ARRAY_MARK);
			for (int i = 0; i < _count; i++) {
				string _key = ARRAY_INDEX_MARK + key + i;
				DeleteKey(_key, false);
			}
		}

		public static void DeleteAll()
		{
			playerPrefsHashtable.Clear();

			hashTableChanged = true;
		}

		public static void Flush()
		{
			if (hashTableChanged) {
				Serialize();

				StreamWriter fileWriter = null;
				fileWriter = File.CreateText(fileName);

				if (fileWriter == null) {
					Debug.LogWarning("PlayerPrefs::Flush() opening file for writing failed: " + fileName);
				}

				fileWriter.WriteLine(serializedOutput);
				fileWriter.Close();
				serializedOutput = "";
			}
		}

		private static void Serialize()
		{
			IDictionaryEnumerator myEnumerator = playerPrefsHashtable.GetEnumerator();

			while ( myEnumerator.MoveNext() ) {
				if (serializedOutput != "") {
					serializedOutput += " " + PARAMETERS_SEPARATOR + " ";
				}
				serializedOutput += EscapeNonSeperators(myEnumerator.Key.ToString()) + " " + KEY_VALUE_SEPARATOR + " " + EscapeNonSeperators(myEnumerator.Value.ToString()) + " " + KEY_VALUE_SEPARATOR + " " + myEnumerator.Value.GetType();
			}
		}

		private static void Deserialize()
		{
			if (serializedInput == "") return;

			string[] parameters = serializedInput.Split(new string[] {" " + PARAMETERS_SEPARATOR + " "}, StringSplitOptions.None);

			foreach(string parameter in parameters)
			{
				string[] parameterContent = parameter.Split(new string[]{" " + KEY_VALUE_SEPARATOR + " "}, StringSplitOptions.None);

				playerPrefsHashtable.Add(DeEscapeNonSeperators(parameterContent[0]), GetTypeValue(parameterContent[2], DeEscapeNonSeperators(parameterContent[1])));

				if (parameterContent.Length > 3) {
					Debug.LogWarning("PlayerPrefs::Deserialize() parameterContent has " + parameterContent.Length + " elements");
				}
			}
		}

		private static string EscapeNonSeperators(string inputToEscape)
		{
			inputToEscape = inputToEscape.Replace(KEY_VALUE_SEPARATOR,"\\" + KEY_VALUE_SEPARATOR);
			inputToEscape = inputToEscape.Replace(PARAMETERS_SEPARATOR,"\\" + PARAMETERS_SEPARATOR);
			return inputToEscape;
		}

		private static string DeEscapeNonSeperators(string inputToDeEscape)
		{
			inputToDeEscape = inputToDeEscape.Replace("\\" + KEY_VALUE_SEPARATOR, KEY_VALUE_SEPARATOR);
			inputToDeEscape = inputToDeEscape.Replace("\\" + PARAMETERS_SEPARATOR, PARAMETERS_SEPARATOR);
			return inputToDeEscape;
		}

		public static object GetTypeValue(string typeName, string value)
		{
			if (typeName == "System.String") {
				return (object)value.ToString();
			}
			if (typeName == "System.Int32") {
				return (object)System.Convert.ToInt32(value);
			}
			if (typeName == "System.Boolean") {
				return (object)System.Convert.ToBoolean(value);
			}
			if (typeName == "System.Single"){
				// -> single = float
				return (object)System.Convert.ToSingle(value);
			} else {
				Debug.LogError("Unsupported type: " + typeName);
			}

			return null;
		}

		public static void ShowDebug()
		{
			string _console = "";
			_console += "--- PlayerPrefs (Key : Value) ---\n";

			foreach (DictionaryEntry pair in playerPrefsHashtable) {
				string key = pair.Key.ToString();
				string value = (pair.Value).ToString();
				if (key.StartsWith(ARRAY_INDEX_MARK)) continue;
				if (key.EndsWith(ARRAY_MARK)) {
					string _arrayValue = "[ ";
					for (int i = 0; i < System.Convert.ToInt32(value); i++) {
						_arrayValue += playerPrefsHashtable[ARRAY_INDEX_MARK + key.Replace(ARRAY_MARK, "") + i].ToString();
						if (i < System.Convert.ToInt32(value) - 1) _arrayValue += ", ";
					}
					_arrayValue += " ]";

					_console += key + " : size " + value + " " + _arrayValue;
				} else {
					_console += key + " : " + value;
				}

				_console += "\n";
			}

			UnityEngine.Debug.Log(_console);
		}

	}
}
