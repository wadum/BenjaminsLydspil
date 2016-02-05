using UnityEngine;
using System.Collections.Generic;

public class InputController : MonoBehaviour {

	private SoundButtonController[] buttons;
	private List<SoundButtonController> currentHovers = new List<SoundButtonController>();
	private List<SoundButtonController> deleteHovers = new List<SoundButtonController>();

	// Use this for initialization
	void Start () {
		buttons = GameObject.FindObjectsOfType<SoundButtonController>();
	}
	
	// Update is called once per frame
	void Update () {
		
		foreach(SoundButtonController currentHover in currentHovers){
			if(Input.GetMouseButton(0) && RectTransformUtility.RectangleContainsScreenPoint(currentHover.GetComponent<RectTransform>(),Input.mousePosition)){
				return;
			} else {
				currentHover.hover.SetActive(false);
				currentHover.sound.Stop();
				deleteHovers.Add(currentHover);
			}
		}
		currentHovers.RemoveAll(c => deleteHovers.Contains(c));
		deleteHovers.Clear();
		CheckMouse();
	}

	void CheckMouse(){
		if (Input.GetMouseButton(0)){
			foreach (SoundButtonController button in buttons){
				if(RectTransformUtility.RectangleContainsScreenPoint(button.GetComponent<RectTransform>(),Input.mousePosition)){
					if(currentHovers.Contains(button))
						return;
					button.hover.SetActive(true);
					button.sound.Play();
					currentHovers.Add(button);
					return;
				}
			}
		}
	}
}
