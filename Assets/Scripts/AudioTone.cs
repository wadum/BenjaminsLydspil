using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class AudioTone : MonoBehaviour {

	public int position = 0;
	public int samplerate = 44100;
	public float frequency = 415.305f;
	void Start() {
		var myClip = AudioClip.Create("MySinusoid", samplerate * 2, 1, samplerate, true, OnAudioRead, OnAudioSetPosition);
		var aud = GetComponent<AudioSource>();
		aud.clip = myClip;
		aud.Play();
	}
	void OnAudioRead(float[] data) {
		var count = 0;
		while ( count < data.Length ) {
			data[count] = Mathf.Sign(Mathf.Sin(2 * Mathf.PI * frequency * position / samplerate));
			position++;
			count++;
		}
	}
	void OnAudioSetPosition(int newPosition) {
		position = newPosition;
	}
}
