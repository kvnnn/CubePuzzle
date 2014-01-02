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
// Parameter
	private bool isEasy = true;
// NGUI
	public GameObject baseGo;
	public GameObject easyRankingGo;
	public UILabel easyScoreLabels;
	public UILabel easyComboLabels;
	public UILabel easyDateLabels;
	public GameObject hardRankingGo;
	public UILabel hardScoreLabels;
	public UILabel hardComboLabels;
	public UILabel hardDateLabels;
	public UISprite difficultyButtonSprite;

	protected override void Awake()
	{
		base.Awake();
		isEasy = true;
		baseGo.gameObject.SetActive(false);
		easy = new RankingData("easy_rankings");
		hard = new RankingData("hard_rankings");
	}

	public void Show()
	{
		baseGo.gameObject.SetActive(true);
		gameObject.SetActive(true);

		Top.instance.mainGo.SetActive(false);

		UpdateEasyRanking();
		UpdateHardRanking();
	}

	public void Hide()
	{
		gameObject.SetActive(false);
		baseGo.gameObject.SetActive(false);

		Top.instance.mainGo.SetActive(true);
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
// NGUI
//----------------
	void UpdateEasyRanking()
	{
		string _scores = "";
		string _combos = "";
		string _dates = "";
		foreach (RankingData.ScoreData _data in easy.scores) {
			_scores += _data.scoreString;
			_scores += "\n";
			_combos += _data.comboString;
			_combos += "\n";
			_dates += _data.date + " - " + _data.time;
			_dates += "\n";
		}
		easyScoreLabels.text = _scores;
		easyComboLabels.text = _combos;
		easyDateLabels.text = _dates;
	}

	void UpdateHardRanking()
	{
		string _scores = "";
		string _combos = "";
		string _dates = "";
		foreach (RankingData.ScoreData _data in hard.scores) {
			_scores += _data.scoreString;
			_scores += "\n";
			_combos += _data.comboString;
			_combos += "\n";
			_dates += _data.date + " - " + _data.time;
			_dates += "\n";
		}
		hardScoreLabels.text = _scores;
		hardComboLabels.text = _combos;
		hardDateLabels.text = _dates;
	}

//----------------
// Touch Event
//----------------
	void CloseClick()
	{
		Hide();
	}

	void DifficultyChangeClick()
	{
		Color _buttonColorTo;
		if (isEasy) {
			TweenAlpha _easyTween = TweenAlpha.Begin(easyRankingGo, 0.35f, 0f);
			TweenAlpha _hardTween = TweenAlpha.Begin(hardRankingGo, 0.35f, 1f);
			_buttonColorTo = new Color(0f, 200f/255f, 1f, 1f);
		} else {
			TweenAlpha _easyTween = TweenAlpha.Begin(easyRankingGo, 0.35f, 1f);
			TweenAlpha _hardTween = TweenAlpha.Begin(hardRankingGo, 0.35f, 0f);
			_buttonColorTo = new Color(1f, 0f, 200/255f, 1f);
		}
		TweenColor.Begin(difficultyButtonSprite.gameObject, 0.35f, _buttonColorTo);
		isEasy = !isEasy;
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
				DateTime _date = DateTime.Now;
				System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-US");

				date = _date.ToString("MMM dd", ci);;
				time = _date.ToString("HH:mm", ci);;
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
