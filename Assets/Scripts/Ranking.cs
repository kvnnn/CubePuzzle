using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PlayerPrefs = PreviewLabs.PlayerPrefs;

public class Ranking : SingletonMonoBehaviour<Ranking>
{
// RankingData
	public RankingData easy;
	public RankingData hard;
// NGUI

	protected override void Awake()
	{
		base.Awake();
		easy = new RankingData("easy_rankings");
		hard = new RankingData("hard_rankings");
	}

	public void SetScore(bool isEasy, int score, int maxCombo)
	{
		RankingData _ranking = isEasy ? easy : hard;
		_ranking.SetScore(score, maxCombo);
	}

	public bool IsMaxScore(bool isEasy, int score)
	{
		RankingData _ranking = isEasy ? easy : hard;
		return _ranking.IsMaxScore(score);
	}

//----------------
// Class
//----------------
	public class RankingData
	{
		public List<ScoreData> scores;
		private string key;
		public RankingData(string key)
		{
			this.key = key;
			scores = new List<ScoreData>();
			if (PlayerPrefs.HasKey(key)) {
				List<string> _stringList = PlayerPrefs.GetArray<string>(key);
				foreach (string str in _stringList) {
					scores.Add(new ScoreData(str));
				}
			}
		}

		public void SetScore(int score, int maxCombo)
		{
			scores.Add(new ScoreData(score, maxCombo));
			scores = scores.OrderByDescending(n => n.score).ToList();
			if (scores.Count > 10) {
				scores.RemoveRange(10, scores.Count - 10);
			}
			SaveData();
		}

		public void SaveData()
		{
			List<string> _stringList = new List<string>();
			foreach (ScoreData _data in scores) {
				_stringList.Add(_data.toString);
			}
			PlayerPrefs.SetArray<string>(key, _stringList);
			PlayerPrefs.Flush();

			// Debug;
			// PlayerPrefs.ShowDebug();
		}

		public bool IsMaxScore(int score)
		{
			if (scores.Count > 0) {
				return score > scores[0].score;
			}
			return true;
		}

		//----------------
		// Score Data Class
		//----------------
		public class ScoreData
		{
			public int score;
			public string scoreString {
				get {return string.Format("{0:#,###0}", score);}
			}
			public int combo;
			public string comboString {
				get {return combo.ToString();}
			}
			public string time;
			public string date;
			private const char SEPARATOR = '+';
			public string toString {
				get {
					return string.Format("{0}{1}{2}{3}{4}{5}{6}", score, SEPARATOR, combo, SEPARATOR, date, SEPARATOR, time);
				}
			}
			public ScoreData(int score, int combo)
			{
				this.score = score;
				this.combo = combo;
				date = string.Format("{0}", DateTime.Now.ToShortDateString());
				time = string.Format("{0}", DateTime.Now.ToShortTimeString());
			}
			public ScoreData(string str)
			{
				string[] _strArray = str.Split(SEPARATOR);
				this.score = int.Parse(_strArray[0]);
				this.combo = int.Parse(_strArray[1]);
				this.date = _strArray[2];
				this.time = _strArray[3];
			}
		}
	}
}
