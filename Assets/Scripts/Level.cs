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
        // Step was not completed this frame.
        if (!CurrentStep.Current.HandleInput(sb)) return false;

        NextStep(true);
        return true;
    }

    public void NextStep(bool Correct)
    {
        if (Correct)
        {
            GameManager.Game.PlayCorrect();
        }

        StartCoroutine(ContinueAfterCorrectVoice());
    }

    private IEnumerator ContinueAfterCorrectVoice()
    {
        while (GameManager.Game.CorrectVoice.isPlaying)
            yield return null;
        Debug.Log("Completed Step: " + CurrentStep.Current.gameObject.name);
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