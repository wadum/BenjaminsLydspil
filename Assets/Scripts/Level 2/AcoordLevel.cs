using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcoordLevel : MonoBehaviour {

    //Used to be able to play both sounds at once
    public AudioSource Tune1, Tune2;

    //Playing voice sounds
    public AudioSource VoicePlayer;

    //Voice Clips
    public AudioClip FindTonerne, ForkerteToner;

    //Nested list with chords provided by benjamin
    [System.Serializable]
    public struct Chord
    {
        public AudioClip[] chord;
    }

    public Chord[] Chords;

    //list of the Hovers on the buttons
    public GameObject[] Hovers;


    public int _correctMelody; // This could be private just to see that it take a new chord every time it plays

    public int TimesToCompleteLevel = 2;
    public int AmountOfLvlCompleted = 0;
    private bool _levelCompleted = true; // Used to make a new Chord to find 
    private bool _intro = true; //Should the script give us a intro
    private bool _introRunning = false; // Is the intro running
    private bool _endingRunning = false; // Is the ending running

    private bool _error = false; //Did the player make an error e.g. holding the wrong button

    // Looks if all the chords created in the inspector is filled and does not have the same tunes to play twise
    void Start () {

        AmountOfLvlCompleted = 0;

        for (int i = 0; i < Chords.Length; i++)
        {
            if (!(Chords[i].chord[0] && Chords[i].chord[1]))
            {
                throw new System.ApplicationException("Some Chords havent been set");
            }
            if (Chords[i].chord[0] == Chords[i].chord[1])
            {
                throw new System.ApplicationException("Some Chords have 2 of the same tunes");
            }
        }
	}
	
	void Update () {
        //if we have completed the lvl enough times stop requesting the player to play it
        if(AmountOfLvlCompleted >= TimesToCompleteLevel)
        {
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
        int _activeHoverCount = 0;

        //Counting the amount of active hovers
        foreach (GameObject hover in Hovers)
        {
            if (hover == isActiveAndEnabled)
            {
                _activeHoverCount++;
            }
        }
        //If we have more active hovers then 2 then its wrong since there is only going to be 2 for any given chord
        if (_activeHoverCount != 2)
        {
            return;
        }
        //HER HVIS MAN HAR PRØVET FOR MANGE GANGE AFSPIL INTRO IGEN DEN VÆLGER IKKE EN NY MELODI DA _levelCompleted IKKE BLIVER SAT TIL true


        //Check if the enabled hovers have the correct audio files
        foreach (GameObject hover in Hovers)
        {
            if(hover == isActiveAndEnabled)
            {
                // The hover which is active has a parent and that parents audiosource must have the name of either the correct tunes if not then the wrong button must be active
                if (!(transform.parent.GetComponent<AudioSource>().name == Chords[_correctMelody].chord[0].name || transform.parent.GetComponent<AudioSource>().name == Chords[_correctMelody].chord[1].name))
                {
                    _error = true;
                }
            }
        }

        //If we saw no errors and the correct number of hovers are active then we must have activated the correct buttons
        if(!_error && _activeHoverCount == 2)
        {
            StartCoroutine(Ending());
        }
	}

    //Sets up the game
    private IEnumerator Intro()
    {
        _introRunning = true;
        //Choose new Correct Chord
        if (_levelCompleted)
        {
            _correctMelody = Random.Range(0, Chords.Length);
            _levelCompleted = false;
        }

        // Play first tune
        Tune1.clip = Chords[_correctMelody].chord[0];
        Tune1.Play();
        yield return new WaitForSeconds(0.5f);

        //Play secound tune on top of the first
        Tune2.clip = Chords[_correctMelody].chord[1];
        Tune2.Play();
        yield return new WaitForSeconds(3f);

        VoicePlayer.clip = FindTonerne;
        VoicePlayer.Play();

        if(VoicePlayer.isPlaying)
        {
            yield return null;
        }
        //Stop everything so the player can start playing
        Tune1.Stop();
        Tune2.Stop();
        _intro = false;
        _introRunning = false;
        yield return null;
    }

    // Ends the game and makes sure a new starts
    private IEnumerator Ending()
    {
        _endingRunning = true;
        GameManager.Game.PlayCorrectSound();
        if (GameManager.Game.CorrectSound.isPlaying)
        {
            yield return null;
        }
        GameManager.Game.PlayCorrect();
        if (GameManager.Game.ControlVoicePlaying())
        {
            yield return null;
        }
        _endingRunning = false;
        _levelCompleted = true;
        _intro = true;

        AmountOfLvlCompleted += 1;

        yield return null;
    }
}

