using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Steps
{

    [RequireComponent(typeof(AudioSource))]
    public class PlayVoice : Step
    {

        public AudioSource Voice;

        public override bool HandleInput(SoundButtonController sb)
        {
            return false;
        }

        public override void Play()
        {
            if (Voice)
            {
                Voice.Play();
                StartCoroutine(NextStepWhenDone());
            }
            else
            {
                if(GameManager.Game.CurrentLevel.Current.CurrentStep.Current == this)
                    GameManager.Game.CurrentLevel.Current.NextStep(false);
            }
        }

        private IEnumerator NextStepWhenDone()
        {
            while (Voice.isPlaying)
            {
                yield return null;
            }
            if (GameManager.Game.CurrentLevel.Current.CurrentStep.Current == this)
                GameManager.Game.CurrentLevel.Current.NextStep(false);
        }
    }
}
