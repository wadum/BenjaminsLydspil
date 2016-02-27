using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{

    public List<Level> Levels = new List<Level>(); 

	public static GameManager Game;

    public AudioSource CorrectVoice;

    public IEnumerator<Level> CurrentLevel; 

	private void Start ()
	{
		if(Game)
			Destroy(gameObject);

		DontDestroyOnLoad(gameObject);
		Game = this;
        if(Levels.Count == 0)
            throw new ApplicationException("No levels to play!");
        if (!CorrectVoice)
            throw new ApplicationException("Need to set correct voice.");
        StartGame();

	}

    public void PlayCorrect()
    {
        if(CorrectVoice)
            CorrectVoice.Play();
    }

    public void NextLevel()
    {

        if (!CurrentLevel.MoveNext())
        {
            // No more levels to play. Restarting.
            StartGame();
        }

        // Start the next level.
        CurrentLevel.Current.Play();

    }

    public void StartGame()
    {
        CurrentLevel = Levels.GetEnumerator();
        CurrentLevel.MoveNext();
        CurrentLevel.Current.Play();
    }

    public void PressedButton(SoundButtonController button)
	{
        if(CorrectVoice && !CorrectVoice.isPlaying) CurrentLevel.Current.HandleInput(button);
		button.Activate();
	}

	public void ReleasedButton(SoundButtonController button)
	{
		button.Deactivate();
	}
}
