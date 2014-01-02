using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using PlayerPrefs = PreviewLabs.PlayerPrefs;

public class Settings : SingletonMonoBehaviour<Settings>
{
// Parameter
	private ControlType controllerType;
	private bool isSoundOn;
// NGUI
	public GameObject baseGo;
	public UILabel contollerLabel;
	public UIImageButton soundButton;
// db
	private const string KEY_CONTROLLER = "controller_type";
	private const string KEY_SOUND = "sound_on_off";

	protected override void Awake()
	{
		base.Awake();
		baseGo.gameObject.SetActive(false);

		if (PlayerPrefs.HasKey(KEY_CONTROLLER)) {
			controllerType = (ControlType)PlayerPrefs.GetInt(KEY_CONTROLLER);
		} else {
			controllerType = ControlType.Pad;
		}
		UpdateControllerLabel();

		if (PlayerPrefs.HasKey(KEY_SOUND)) {
			isSoundOn = PlayerPrefs.GetBool(KEY_SOUND);
		} else {
			isSoundOn = true;
		}
		UpdateSoundIcon();
	}

	public void Show()
	{
		baseGo.gameObject.SetActive(true);
		gameObject.SetActive(true);

		Top.instance.mainGo.SetActive(false);
	}

	public void Hide()
	{
		gameObject.SetActive(false);
		baseGo.gameObject.SetActive(false);

		Top.instance.mainGo.SetActive(true);
	}

//----------------
// Touch Event
//----------------
	void CloseClick()
	{
		Hide();
	}

	void ChangeControllerClick()
	{
		int _typeInt = (int)controllerType;
		_typeInt = (_typeInt+1)%3;
		controllerType = (ControlType)_typeInt;
		PlayerPrefs.SetInt(KEY_CONTROLLER, _typeInt);

		UpdateControllerLabel();
	}

	private void UpdateControllerLabel()
	{
		string _text = "";
		switch (controllerType) {
			case ControlType.Pad:
				_text = "Control Pad";
			break;
			case ControlType.Swipe:
				_text = "Swipe";
			break;
			case ControlType.Tilt:
				_text = "Tilt";
			break;
		}
		contollerLabel.text = _text;
	}

	void SoundClick()
	{
		isSoundOn = !isSoundOn;
		PlayerPrefs.SetBool(KEY_SOUND, isSoundOn);
		UpdateSoundIcon();
	}

private const string SOUND_ON_SPRITE = "btn_sound_on";
private const string SOUND_OFF_SPRITE = "btn_sound_off";
	private void UpdateSoundIcon()
	{
		string _spriteName = isSoundOn ? SOUND_ON_SPRITE : SOUND_OFF_SPRITE;
		soundButton.normalSprite = _spriteName;
		soundButton.hoverSprite = _spriteName;
		soundButton.pressedSprite = _spriteName;
		soundButton.disabledSprite = _spriteName;
	}

//----------------
// enum
//----------------
	public enum ControlType {
		Pad = 0,
		Swipe = 1,
		Tilt = 2,
	}
	public bool isPad {get {return controllerType == ControlType.Pad;}}
	public bool isSwipe {get {return controllerType == ControlType.Swipe;}}
	public bool isTilt {get {return controllerType == ControlType.Tilt;}}
}
