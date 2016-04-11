using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class AcoordLevel : MonoBehaviour
{
    public int CorrectMelody; // This could be private just to see that it take a new chord every time it plays
    private bool _endingRunning; // Is the ending running

    private bool _intro = true; //Should the script give us a intro
    private bool _introRunning; // Is the intro running
    private bool _levelCompleted = true; // Used to make a new Chord to find 
    public int AmountOfLvlCompleted;

    public Chord[] Chords;

    //Voice Clips
    public AudioClip FindTonerne, ForkerteToner;

    //list of the Hovers on the buttons
    public GameObject[] Hovers;

    public int TimesToCompleteLevel = 2;

    //Used to be able to play both sounds at once
    public AudioSource[] Tunes;

    //Playing voice sounds
    public AudioSource VoicePlayer;

    private InputController _ic;

    // Looks if all the chords created in the inspector is filled and does not have the same tunes to play twise
    private void Start()
    {
        AmountOfLvlCompleted = 0;

        if (Chords.Any(t => !Chords.All(c => c.chord.All(a => a != null))))
        {
            throw new ApplicationException("Some Chords havent been set");
        }
        if (Chords.Any(t => !Chords.All(c => c.chord.Distinct().ToList().Count == c.chord.Length)))
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

        //reset the active hover count
        var activeHoverCount = Hovers.Count(hover => hover.activeInHierarchy);

        Debug.Log(activeHoverCount);

        //If we have more active hovers then 2 then its wrong since there is only going to be 2 for any given chord
        if (activeHoverCount != Tunes.Length)
        {
            return;
        }
        //HER HVIS MAN HAR PRØVET FOR MANGE GANGE AFSPIL INTRO IGEN DEN VÆLGER IKKE EN NY MELODI DA _levelCompleted IKKE BLIVER SAT TIL true


        //If we saw no errors and the correct number of hovers are active then we must have activated the correct buttons
        if (Hovers
            .Where(
                hover =>
                    hover.activeInHierarchy).All(activeHover => Chords[CorrectMelody].chord.Any(
                        c => c.name == activeHover.GetComponentInParent<AudioSource>().clip.name)))
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
            CorrectMelody = Random.Range(0, Chords.Length);
            _levelCompleted = false;
        }

        // Play Tunes
        for (var i = 0; i < Chords[CorrectMelody].chord.Length; i++)
        {
            Tunes[i].clip = Chords[CorrectMelody].chord[i];
            Tunes[i].Play();
            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitForSeconds(2.5f);

        VoicePlayer.clip = FindTonerne;
        VoicePlayer.Play();

        if (VoicePlayer.isPlaying)
        {
            yield return null;
        }

        //Stop everything so the player can start playing
        foreach (var tune in Tunes)
            tune.Stop();

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
    public struct Chord
    {
        public AudioClip[] chord;
    }
}