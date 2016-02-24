using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class InputController : MonoBehaviour {

	private SoundButtonController[] _buttons;

    private  SoundButtonController _currentMouseHover;
    private readonly Dictionary<int, SoundButtonController> _currentHovers = new Dictionary<int,SoundButtonController>();

    private void Start () {
		_buttons = FindObjectsOfType<SoundButtonController>();
	    Input.simulateMouseWithTouches = false;
	}

    private void Update () {
        HandleTouches();
        HandleMouse();
	}

    private void HandleTouches()
    {
        if (Input.touchCount <= 0)
        {
			foreach (var button in _currentHovers.Values)
            {
				GameManager.Game.ReleasedButton(button);
            }
            _currentHovers.Clear();
        }
        else
        {
            for (var i = 0; i < Input.touchCount; i++)
            {
                HandleTouch(i);
            }
            foreach (var unusedId in _currentHovers.Keys.Except(Input.touches.Select(t => t.fingerId)).ToArray())
            {
                SoundButtonController unusedButton;
                _currentHovers.TryGetValue(unusedId, out unusedButton);
                if (!unusedButton) continue;
				GameManager.Game.ReleasedButton(unusedButton);
            }
        }
    }

    private void HandleTouch(int id)
    {

        var touch = Input.GetTouch(id);
        SoundButtonController button;
        _currentHovers.TryGetValue(touch.fingerId, out button);

        if (button)
        {
            if (!IsPointInButton(touch.position, button))
            {
				GameManager.Game.ReleasedButton(button);
                _currentHovers.Remove(touch.fingerId);
            }
            else
            {
                {
                    return;
                }
            }

        }

        button = GetHoveredButton(touch.position);

        if (!button) return;
		GameManager.Game.PressedButton(button);
        _currentHovers.Add(touch.fingerId, button);

    }

    private void HandleMouse()
    {
        if (!Input.GetMouseButton(0))
        {
            if (!_currentMouseHover) return;
			GameManager.Game.ReleasedButton(_currentMouseHover);
            _currentMouseHover = null;
        }
        else
        {
            if (_currentMouseHover)
            {
                if (!IsPointInButton(Input.mousePosition, _currentMouseHover))
                {
					GameManager.Game.ReleasedButton(_currentMouseHover);
                    _currentMouseHover = null;
                }
                else
                {
                    return;
                }
            }

            var button = GetHoveredButton(Input.mousePosition);

            if (!button) return;

			GameManager.Game.PressedButton(button);
            _currentMouseHover = button;

        }
    }

    private SoundButtonController GetHoveredButton(Vector3 pos)
    {
        return _buttons.FirstOrDefault( b => IsPointInButton(pos, b) );
    }

    private static bool IsPointInButton(Vector3 pos, SoundButtonController button)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(button.GetComponent<RectTransform>(), pos);
    }
}
