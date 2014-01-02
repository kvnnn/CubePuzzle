using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Result : MonoInheritance
{
// Scene
	public Game game;
// NGUI
	public UILabel difficultyLabel;
	public UILabel coloredCountLabel;
	public UILabel goalCountLabel;
	public UILabel bombCountLabel;
	public UILabel maxComboLabel;
	public UILabel scoreLabel;
	public GameObject newIconGo;

	public void Show()
	{
		if (Ranking.instance.IsMaxScore(game.isEasyMode, game.score)) {
			newIconGo.gameObject.SetActive(true);
		} else {
			newIconGo.gameObject.SetActive(false);
		}
		SaveScore();

		if (game.isEasyMode) {
			difficultyLabel.text = "Easy";
			difficultyLabel.color = new Color(0f, 200f/255f, 1f, 1f);
		} else {
			difficultyLabel.text = "Hard";
			difficultyLabel.color = new Color(200/255f, 0f, 1f, 1f);
		}

		coloredCountLabel.text = Cell.coloredClearCount.ToString();
		goalCountLabel.text = Cell.goalClearCount.ToString();
		bombCountLabel.text = Cell.bombClearCount.ToString();

		maxComboLabel.text = game.maxCombo.ToString();
		scoreLabel.text = string.Format("{0:#,###0}", game.score);

		gameObject.SetActive(true);
	}

	public bool SaveScore()
	{
		Ranking.instance.SetScore(game.isEasyMode, game.score, game.maxCombo);
		return false;
	}

}
