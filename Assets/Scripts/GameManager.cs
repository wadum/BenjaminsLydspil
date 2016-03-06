using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Game;

    public AudioSource CorrectVoice;
    public AudioSource WrongVoice;

    public AudioSource ErrorSound;
    public AudioSource CorrectSound;

    public IEnumerator<Level> CurrentLevel;

    public List<Level> Levels = new List<Level>();

    private void Start()
    {
        if (Game)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        Game = this;
        if (Levels.Count == 0)
            throw new ApplicationException("No levels to play!");
        if (!CorrectVoice)
            throw new ApplicationException("Need to set correct voice.");
        if (!WrongVoice)
            throw new ApplicationException("Need to set wrong voice.");
        if (!ErrorSound)
            throw new ApplicationException("Need to set error sound.");
        if (!CorrectSound)
            throw new ApplicationException("Need to set correct sound.");
        StartGame();
    }

    public void PlayCorrect()
    {
        if (CorrectVoice)
            CorrectVoice.Play();
    }

    public void PlayWrong()
    {
        if (WrongVoice)
            WrongVoice.Play();
    }

    public void PlayErrorSound()
    {
        if (ErrorSound)
            ErrorSound.Play();
    }

    public void PlayCorrectSound()
    {
        if (CorrectSound)
            CorrectSound.Play();
    }

    public bool ControlVoicePlaying()
    {
        return (CorrectVoice && CorrectVoice.isPlaying) || (WrongVoice && WrongVoice.isPlaying);
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
        if (!ControlVoicePlaying()) CurrentLevel.Current.HandleInput(button);
        button.Activate();
    }

    public void ReleasedButton(SoundButtonController button)
    {
        button.Deactivate();
    }
}