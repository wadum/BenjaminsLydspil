using UnityEngine;
using System.Collections;

public class SoundButtonController : MonoBehaviour
{

    private Coroutine _stopPlaying;
    private readonly float _timeToStopPlaying = 0.2f;

    public GameObject Hover
    {
        get
        {
            if (!_hover) _hover = gameObject.transform.FindChild("hover").gameObject;
            return _hover;
        }
    }

    public AudioSource Sound {
        get
        {
            if (!_sound) _sound = GetComponent<AudioSource>(); return _sound; 
        }
    }
    private AudioSource _sound;
    private GameObject _hover;


    public void Activate()
    {
        if (_stopPlaying != null)
        {
            RemoveStopPlaying();
        }
        Hover.SetActive(true);
        Sound.Play();
    }

    public void Deactivate()
    {
        Hover.SetActive(false);
        _stopPlaying = StartCoroutine(StopPlaying());
    }

    private IEnumerator StopPlaying()
    {
        var time = Time.time;

        while (time + _timeToStopPlaying > Time.time)
        {
            Sound.volume = 1 - ((Time.time - time) / _timeToStopPlaying);
            Debug.Log(Sound.volume);
            yield return null;
        }

        Sound.Stop();

        RemoveStopPlaying();
    }

    private void RemoveStopPlaying()
    {
        StopCoroutine(_stopPlaying);
        Sound.volume = 1;
        _stopPlaying = null;
    }

}
