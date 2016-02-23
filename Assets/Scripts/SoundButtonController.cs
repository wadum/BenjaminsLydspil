using UnityEngine;
using System.Collections;

public class SoundButtonController : MonoBehaviour {

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
}
