using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public static GameManager Game;

	void Start ()
	{
		if(Game)
			Destroy(gameObject);

		DontDestroyOnLoad(gameObject);
		Game = this;

	}
	
	public void PressedButton(SoundButtonController button)
	{
		button.Activate();
	}

	public void ReleasedButton(SoundButtonController button)
	{
		button.Deactivate();
	}
}
