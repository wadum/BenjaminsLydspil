﻿using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class MelodyLevel : MonoBehaviour {

    public int CorrectMelody; // This could be private just to see that it take a new chord every time it plays
    public int LengthOfTuneSequence = 3;
    private bool _endingRunning; // Is the ending running

    private bool _intro = true; //Should the script give us a intro
    private bool _introRunning; // Is the intro running
    private bool _levelCompleted = true; // Used to make a new Chord to find 
    public int AmountOfLvlCompleted;

    public int _tuneprogression = 0;

    //Voice Clips
    public AudioClip EfterlignMelodien, ForkertMelodi;

    //list of the Hovers on the buttons
    public GameObject[] Hovers;

    public int TimesToCompleteLevel = 2;

    //Used to be able to play both sounds at once
    public AudioSource[] Tunes;

    //Playing voice sounds
    public AudioSource VoicePlayer;

    public Melody[] Melodies;

    private InputController _ic;

    // Looks if all the chords created in the inspector is filled and does not have the same tunes to play twise
    private void Start()
    {
        AmountOfLvlCompleted = 0;

        if (Melodies.Any(t => !Melodies.All(c => c.melody.All(a => a != null))))
        {
            throw new ApplicationException("Some Chords havent been set");
        }
        if (Melodies.Any(t => !Melodies.All(c => c.melody.Distinct().ToList().Count == c.melody.Length)))
        {
            throw new ApplicationException("Some Chords have 2 of the same tunes");
        }

        _ic = FindObjectOfType<InputController>();
    }

    private void Update()
    {
        //if we have completed the lvl enough times stop requesting the player to play it
        if (AmountOfLvlCompleted >= TimesToCompleteLevel)
        {
            GameManager.Game.NextLevel();
            return;
        }
        // If we have conpleted or started the level and need a new intro get it
        if (_intro && !_introRunning)
        {
            StartCoroutine(Intro());
        }
        // If the intro or ending is running dont do anything
        if (_introRunning || _endingRunning)
        {
            return;
        }

        var activeHoverCount = Hovers.Count(hover => hover.activeInHierarchy);

        if (activeHoverCount >= 2) //iff we press more than one button at a time we are doing it wrong
        {
            _tuneprogression = 0;
            return;
        }

        //if there is a hover active with the wrong name reset the melody stuff
        if (Hovers
            .Where(
                hover =>
                    hover.activeInHierarchy && hover.GetComponentInParent<AudioSource>().clip.name != Tunes[_tuneprogression].name).ToList().Count > 0)
        {
            _tuneprogression = 0;
        }

        //If we saw no errors and the correct number of hovers are active then we must have activated the correct buttons
        if (Hovers
            .Where(
                hover =>
                    hover.activeInHierarchy && hover.GetComponentInParent<AudioSource>().clip.name == Tunes[_tuneprogression].name).ToList().Count > 0)
        {
            _tuneprogression++;
        }


        if (_tuneprogression >= LengthOfTuneSequence)
        {
            StartCoroutine(Ending());
        }
    }

    //Sets up the game
    private IEnumerator Intro()
    {
        _ic.enabled = false;
        _introRunning = true;
        //Choose new Correct Chord
        if (_levelCompleted)
        {
            CorrectMelody = Random.Range(0, Melodies.Length);

            for (var i = 0; i < LengthOfTuneSequence; i++)
            {
                Tunes[i].clip = Melodies[CorrectMelody].melody[Random.Range(0, Melodies[CorrectMelody].melody.Length)]; //Choose at random the tunes from the correct melody section
                Tunes[i].Play();
                yield return new WaitForSeconds(1f);
                Tunes[i].Stop();
            }
            _levelCompleted = false;
        }
        else
        {
            for (var i = 0; i < Melodies[CorrectMelody].melody.Length; i++)
            {
                Tunes[i].Play();
                yield return new WaitForSeconds(1f);
                Tunes[i].Stop();
            }
        }

        

        VoicePlayer.clip = EfterlignMelodien;
        VoicePlayer.Play();

        yield return new WaitForSeconds(VoicePlayer.clip.length);
       
        _intro = false;
        _introRunning = false;
        _ic.enabled = true;
        yield return null;
    }

    // Ends the game and makes sure a new starts
    private IEnumerator Ending()
    {
        _ic.enabled = false;
        _endingRunning = true;
        yield return new WaitForSeconds(GameManager.Game.PlayCorrectSound());
        yield return new WaitForSeconds(GameManager.Game.PlayCorrect());
        _endingRunning = false;
        _ic.enabled = true;
        _levelCompleted = true;
        _intro = true;

        AmountOfLvlCompleted += 1;

        yield return null;
    }

    //Nested list with chords provided by benjamin
    [Serializable]
    public struct Melody
    {
        public AudioClip[] melody;
    }
}