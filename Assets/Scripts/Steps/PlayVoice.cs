using System.Collections;
using UnityEngine;

namespace Steps
{
    [RequireComponent(typeof (AudioSource))]
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
                StartCoroutine(NextStepWhenDone());
            }
            else
            {
                NextStep();
            }
        }

        private IEnumerator NextStepWhenDone()
        {
            Voice.Play();
            yield return new WaitForSeconds(Voice.clip.length);
            NextStep();
            StopAllCoroutines();
        }
    }
}