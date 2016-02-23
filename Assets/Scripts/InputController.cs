using UnityEngine;
using System.Collections.Generic;

public class InputController : MonoBehaviour {

	private SoundButtonController[] _buttons;
	private readonly List<SoundButtonController> _currentHovers = new List<SoundButtonController>();
	private readonly List<SoundButtonController> _deleteHovers = new List<SoundButtonController>();

	// Use this for initialization
	void Start () {
		_buttons = FindObjectsOfType<SoundButtonController>();
	}
	
	// Update is called once per frame
	void Update () {
		foreach(SoundButtonController currentHover in _currentHovers){
			if(Input.GetMouseButton(0) && RectTransformUtility.RectangleContainsScreenPoint(currentHover.GetComponent<RectTransform>(),Input.mousePosition)){
				return;
			}

			currentHover.Hover.SetActive(false);
			currentHover.Sound.Stop();
			_deleteHovers.Add(currentHover);
		}
		_currentHovers.RemoveAll(c => _deleteHovers.Contains(c));
		_deleteHovers.Clear();
	    if (Input.touches.Length > 0)
	        return;  //TODO: do touch handling
	    else
	        CheckMouse();
	}

	private void CheckMouse(){
		if (Input.GetMouseButton(0)){
			foreach (SoundButtonController button in _buttons){
				if(RectTransformUtility.RectangleContainsScreenPoint(button.GetComponent<RectTransform>(),Input.mousePosition)){
					if(_currentHovers.Contains(button))
						return;
					button.Hover.SetActive(true);
					button.Sound.Play();
					_currentHovers.Add(button);
					return;
				}
			}
		}
	}
}
