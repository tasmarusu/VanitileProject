using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CustomStandaloneInputModule : StandaloneInputModule
{
    public override void Process()
    {
        if (!eventSystem.isFocused && this.CustomShouldIgnoreEventsOnNoFocus())
            return;

        bool usedEvent = SendUpdateEventToSelectedObject();

        // case 1004066 - touch / mouse events should be processed before navigation events in case
        // they change the current selected gameobject and the submit button is a touch / mouse button.

        // touch needs to take precedence because of the mouse emulation layer
        //if (!CustomProcessTouchEvents() && input.mousePresent)
        //    ProcessMouseEvent();

        if (eventSystem.sendNavigationEvents)
        {
            if (!usedEvent)
                usedEvent |= SendMoveEventToSelectedObject();

            if (!usedEvent)
                SendSubmitEventToSelectedObject();
        }
    }

    private bool CustomShouldIgnoreEventsOnNoFocus()
    {
#if UNITY_EDITOR
        return !UnityEditor.EditorApplication.isRemoteConnected;
#else
            return true;
#endif
    }

    private bool CustomProcessTouchEvents()
    {
        for (int i = 0; i < input.touchCount; ++i)
        {
            Touch touch = input.GetTouch(i);

            if (touch.type == TouchType.Indirect)
                continue;

            bool released;
            bool pressed;
            var pointer = GetTouchPointerEventData(touch, out pressed, out released);

            ProcessTouchPress(pointer, pressed, released);

            if (!released)
            {
                ProcessMove(pointer);
                ProcessDrag(pointer);
            }
            else
                RemovePointerData(pointer);
        }
        return input.touchCount > 0;
    }
}
