﻿using UnityEngine;

public abstract class Step : MonoBehaviour
{
    public abstract bool HandleInput(SoundButtonController sb);

    public abstract void Play();


    public void NextStep()
    {
        GameManager.Game.CurrentLevel.Current.NextStep();
    }
}