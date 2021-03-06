﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Game;
    public AudioSource CorrectSound;

    public AudioSource CorrectVoice;

    public IEnumerator<NeedHelp> CurrentLevel;

    public AudioSource ErrorSound;

    public AudioSource WelcomeToSoundGame;

    public List<NeedHelp> Levels = new List<NeedHelp>();
    public AudioSource WrongVoice;

    private InputController _ic;

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

        _ic = FindObjectOfType<InputController>();

        StartCoroutine(StartGame());
    }

    public float PlayCorrect()
    {
        if (CorrectVoice)
            CorrectVoice.Play();
        return CorrectVoice.clip.length;
    }

    public float PlayWrong()
    {
        if (WrongVoice)
            WrongVoice.Play();
        return WrongVoice.clip.length;
    }

    public float PlayErrorSound()
    {
        if (ErrorSound)
            ErrorSound.Play();
        return ErrorSound.clip.length;
    }

    public float PlayCorrectSound()
    {
        if (CorrectSound)
            CorrectSound.Play();
        return CorrectSound.clip.length;
    }

    public bool ControlVoicePlaying()
    {
        return (CorrectVoice && CorrectVoice.isPlaying) || (WrongVoice && WrongVoice.isPlaying);
    }

    public void NextLevel()
    {
        CurrentLevel.Current.gameObject.SetActive(false);
        if (!CurrentLevel.MoveNext())
        {
            // No more levels to play. Restarting.
            StartCoroutine(StartGame());
        }

        // Start the next level.
        CurrentLevel.Current.gameObject.SetActive(true);
    }

    public IEnumerator StartGame()
    {
        _ic.enabled = false;
        foreach (var level in Levels)
        {
            level.gameObject.SetActive(false);
        }

        WelcomeToSoundGame.Play();

        yield return new WaitForSeconds(WelcomeToSoundGame.clip.length+0.5f);

        CurrentLevel = Levels.GetEnumerator();
        CurrentLevel.MoveNext();
        CurrentLevel.Current.gameObject.SetActive(true);

        _ic.enabled = true;

        yield return null;
    }

    public void PressedButton(SoundButtonController button)
    {
        button.Activate();
    }

    public void ReleasedButton(SoundButtonController button, float delay = 0)
    {
        if (delay > 0)
            StartCoroutine(DelayedRelease(button, delay));
        else
            button.Deactivate();
    }

    private static IEnumerator DelayedRelease(SoundButtonController button, float delay)
    {
        yield return new WaitForSeconds(delay);
        button.Deactivate();
    }

    public void PlayHelp()
    {
        StartCoroutine(CurrentLevel.Current.PlayHelp());
    }
}