using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Level_3
{
    public class MelodyLevel : NeedHelp
    {
        private bool _endingRunning; // Is the ending running

        private InputController _ic;

        private bool _intro = true; //Should the script give us a intro
        private bool _introRunning; // Is the intro running
        private bool _levelCompleted = true; // Used to make a new Chord to find 

        public int Tuneprogression;
        public int AmountOfLvlCompleted;

        public int CorrectMelody; // This could be private just to see that it take a new chord every time it plays

        //Voice Clips
        public AudioClip EfterlignMelodien, ForkertMelodi, Runde2;

        //list of the Hovers on the buttons
        public GameObject[] Hovers;
        public int LengthOfTuneSequence = 3;

        public Melody[] Melodies;

        //public int TimesToCompleteLevel = 2;

        //Used to be able to play both sounds at once
        public AudioSource[] Tunes;

        //Playing voice sounds
        public AudioSource VoicePlayer;

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
            /*if (AmountOfLvlCompleted >= TimesToCompleteLevel)
            {
                GameManager.Game.NextLevel();
                return;
            }*/
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
                Tuneprogression = 0;
                return;
            }

            //if there is a hover active with the wrong name reset the melody stuff
            if (Hovers
                .Where(
                    hover =>
                        hover.activeInHierarchy && Tuneprogression > 0 &&
                        hover.GetComponentInParent<AudioSource>().clip.name == Tunes[Tuneprogression-1].clip.name).ToList().Count >
                0)
            {
                // pass
            } else if (Hovers
               .Where(
                   hover =>
                       hover.activeInHierarchy &&
                       hover.GetComponentInParent<AudioSource>().clip.name != Tunes[Tuneprogression].clip.name).ToList().Count >
               0)
            {
                Tuneprogression = 0;
            }

            //If we saw no errors and the correct number of hovers are active then we must have activated the correct buttons
            if (Hovers
                .Where(
                    hover =>
                        hover.activeInHierarchy &&
                        hover.GetComponentInParent<AudioSource>().clip.name == Tunes[Tuneprogression].clip.name).ToList().Count >
                0)
            {
                Tuneprogression++;
            }


            if (Tuneprogression >= LengthOfTuneSequence)
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

            if (AmountOfLvlCompleted == 0 && Runde2 != null)
            {
                VoicePlayer.clip = Runde2;
                VoicePlayer.Play();

                yield return new WaitForSeconds(VoicePlayer.clip.length + 0.5f);
                VoicePlayer.Stop();
            }

            VoicePlayer.clip = EfterlignMelodien;
            VoicePlayer.Play();

            yield return new WaitForSeconds(VoicePlayer.clip.length+0.5f);

            if (_levelCompleted)
            {
                CorrectMelody = Random.Range(0, Melodies.Length);

                for (var i = 0; i < LengthOfTuneSequence; i++)
                {
                    Tunes[i].clip = Melodies[CorrectMelody].melody[Random.Range(0, Melodies[CorrectMelody].melody.Length)];
                        //Choose at random the tunes from the correct melody section
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
            yield return new WaitForSeconds(1);
            yield return new WaitForSeconds(GameManager.Game.PlayCorrectSound());
            yield return new WaitForSeconds(GameManager.Game.PlayCorrect());
            _endingRunning = false;
            _ic.enabled = true;
            _levelCompleted = true;
            _intro = true;

            Tuneprogression = 0;
            AmountOfLvlCompleted += 1;
            if (LengthOfTuneSequence < 6)
            {
                LengthOfTuneSequence += 1;
            }

            yield return null;
        }

        public override IEnumerator PlayHelp()
        {
            _ic.enabled = false;
            _introRunning = true;

            for (var i = 0; i < LengthOfTuneSequence; i++)
            {
                //Choose at random the tunes from the correct melody section
                Tunes[i].Play();
                yield return new WaitForSeconds(1f);
                Tunes[i].Stop();
            }

            _ic.enabled = true;
            _introRunning = false;
        }

        //Nested list with chords provided by benjamin
        [Serializable]
        public struct Melody
        {
            public AudioClip[] melody;
        }
    }
}