using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Steps
{
    public class SingleInputGenerator : Step {

        public List<AudioClip> Clips
        {
            get {
                return _clips ??
                       (_clips =
                           FindObjectsOfType<SoundButtonController>()
                               .Select(sb => sb.GetComponent<AudioSource>().clip)
                               .ToList());
            }
        }
        public int TimesToComplete = 5;


        private AudioClip _currentClip;
        private int _timesCompleted;
        private List<AudioClip> _clips;

        private int _timesTried;
        public int WrongVoiceAfterTimes = 5;

        public override bool HandleInput(SoundButtonController sb)
        {
            if (sb.Sound.clip.name == _currentClip.name)
            {
                GameManager.Game.PlayCorrect();
                if (TimesToComplete <= _timesCompleted)
                {
                    NextStep();
                }
                else
                {
                    NewClip();
                }
            }
            else
            {
                _timesTried++;
                if (_timesTried % WrongVoiceAfterTimes == 0)
                {
                    GameManager.Game.PlayWrong();
                }
            }
            return sb.Sound.clip.name == _currentClip.name;
        }

        public override void Play()
        {
            if (Clips.Count > 0)
            {
                _timesCompleted = 0;
                NewClip();
                return;
            }

            Debug.Log("Clip not existing");
            NextStep();
        }

        private void NewClip()
        {
            _timesCompleted++;
            _timesTried = 0;
            _currentClip = Clips[Random.Range(0, Clips.Count)];
        }

    }
}
