using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public IEnumerator<Step> CurrentStep;
    public List<Step> Steps = new List<Step>();

    public void Play()
    {
        // If the level contains no steps, just skip it.
        if (Steps.Count == 0)
            CompletedLevel();

        CurrentStep = Steps.GetEnumerator();
        CurrentStep.MoveNext();
        CurrentStep.Current.Play();
    }

    public bool HandleInput(SoundButtonController sb)
    {
        return CurrentStep.Current.HandleInput(sb);
    }

    public void NextStep()
    {
        StartCoroutine(ContinueAfterCorrectVoice());
    }

    private IEnumerator ContinueAfterCorrectVoice()
    {
        yield return null;
        while (GameManager.Game.ControlVoicePlaying())
            yield return null;
        if (!CurrentStep.MoveNext())
        {
            // No more steps to play in level. Moving to next level.
            CompletedLevel();
        }

        // Start the next step.
        CurrentStep.Current.Play();
    }

    protected void CompletedLevel()
    {
        GameManager.Game.NextLevel();
    }
}