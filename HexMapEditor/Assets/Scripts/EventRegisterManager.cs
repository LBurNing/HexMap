using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventRegisterManager
{
    private static EventRegisterManager _instance;

    public static EventRegisterManager instance
    {
        get
        {
            if (_instance == null)
                _instance = new EventRegisterManager();

            return _instance;
        }
    }

    public void Init()
    {
        Event<float>.Register(Event.MOUSE_SCROLLWHEEL, MouseScrollWheel);
        Event<bool>.Register(Event.F5, F5KeyDown);
        Event<bool>.Register(Event.F6, F6KeyDown);
    }

    public void MouseScrollWheel(float value)
    {
        HexGrid.instance.CallFunction(CellFunction.UpdateLayerUIScale);
    }

    public void F6KeyDown(bool firstClick)
    {
        if (firstClick)
        {
            HexGrid.instance.CallFunction(CellFunction.ShowGridPosUI);
        }
        else
        {
            HexGrid.instance.CallFunction(CellFunction.DestroyGridPosUI);
        }
    }

    public void F5KeyDown(bool firstClick)
    {
        if (firstClick)
        {
            HexGrid.instance.CallFunction(CellFunction.ShowLayerUI);
            HexGrid.instance.CallFunction(CellFunction.UpdateLayerUIScale);
        }
        else
        {
            HexGrid.instance.CallFunction(CellFunction.DestroyLayerUI);
        }
    }

    public void UnInit()
    {
        Event<float>.UnRegister(Event.MOUSE_SCROLLWHEEL, MouseScrollWheel);
        Event<bool>.UnRegister(Event.F5, F5KeyDown);
    }
}
