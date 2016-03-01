using UnityEngine;

namespace Steps
{
    public class SingleInput : Step
    {
        public AudioClip Clip;

        public override bool HandleInput(SoundButtonController sb)
        {
            Debug.Log(sb.Sound.clip.name);
            return sb.Sound.clip.name == Clip.name;
        }

        public override void Play()
        {
            if (Clip) return;

            Debug.Log("Clip not existing");
            if (GameManager.Game.CurrentLevel.Current.CurrentStep.Current == this)
                GameManager.Game.CurrentLevel.Current.NextStep(false);
        }
    }
}