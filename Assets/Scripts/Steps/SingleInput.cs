using UnityEngine;

namespace Steps
{
    public class SingleInput : Step
    {
        public AudioClip Clip;

        private int _timesTried;
        public int WrongVoiceAfterTimes = 5;

        public override bool HandleInput(SoundButtonController sb)
        {
            if (sb.Sound.clip.name == Clip.name)
            {
                GameManager.Game.PlayCorrect();
                NextStep();
            }
            else
            {
                _timesTried++;
                if (_timesTried%WrongVoiceAfterTimes == 0)
                {
                    GameManager.Game.PlayWrong();
                }
            }
            return sb.Sound.clip.name == Clip.name;
        }

        public override void Play()
        {
            _timesTried = 0;
            if (Clip) return;

            Debug.Log("Clip not existing");
            NextStep();
        }
    }
}