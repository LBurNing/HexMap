using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Event
{
    #region ÊÂ¼þÎ¨Ò»Id
    public static string BRUSH_MOUSE_CLICK = "BRUSH_MOUSE_CLICK";
    public static string MOUSE_CLICK = "MOUSE_CLICK";
    public static string RES_TITLE_CLICK = "RES_TITLE_CLICK";
    public static string CREATE_CELL_FINISH = "CREATE_CELL_FINISH";
    public static string ESC_INPUT = "ESC_INPUT";
    public static string OPEN_MAP_CONFIG_FILE = "OPEN_MAP_CONFIG_FILE";
    public static string UPDATE_MAP_INFO= "UPDATE_MAP_INFO";
    public static string UPDATE_MAP_AREA = "UPDATE_MAP_AREA";
    public static string UPDATE_X_Y_UI = "";
    public static string SELECT_FOLDER = "SELECT_FOLDER";
    public static string IMPORT_SUCCESS = "IMPORT_SUCCESS";
    public static string COMMAND_COMPLETED = "COMMAND_COMPLETED";
    public static string COMMAND_START = "COMMAND_START";
    public static string ADD_BRUSH_SCALE = "ADD_BRUSH_SCALE";
    public static string UPDATE_DROPDOWN = "UPDATE_DROPDOWN";
    public static string KEY_COPY = "KEY_COPY";
    public static string KEY_PASTE = "KEY_PASTE";
    public static string F5 = "F5";
    public static string F6 = "F6";
    public static string MOUSE_SCROLLWHEEL = "MOUSE_SCROLLWHEEL";
    public static string QUIT_SAVE = "QUIT_SAVE";
    public static string UPDATE_LAYER_INFO = "UPDATE_FUNC_UI";
    public static string CONFIG_LOAD_COMPLETE = "CONFIG_LOAD_COMPLETE";
    #endregion

    private static Dictionary<string, List<Action>> events = new Dictionary<string, List<Action>>();

    public static void Fire(string eventName)
    {
        List<Action> callBacks;
        if (!events.TryGetValue(eventName, out callBacks))
        {
            return;
        }

        foreach (Action callBack in callBacks)
        {
            callBack();
        }
    }

    public static void Register(string eventName, Action callBack)
    {
        List<Action> callBacks;
        if (!events.TryGetValue(eventName, out callBacks))
        {
            callBacks = new List<Action>();
            events.Add(eventName, callBacks);
        }

        callBacks.Add(callBack);
    }

    public static void UnRegister(string eventName, Action callBack)
    {
        List<Action> callBacks;
        if (!events.TryGetValue(eventName, out callBacks))
        {
            return;
        }

        callBacks.Remove(callBack);
    }
}

public static class Event<T>
{
    private static Dictionary<string, List<Action<T>>> eventTs = new Dictionary<string, List<Action<T>>>();

    public static void Fire(string eventName, T value)
    {
        List<Action<T>> callBacks;
        if (!eventTs.TryGetValue(eventName, out callBacks))
        {
            return;
        }

        foreach (Action<T> callBack in callBacks)
        {
            callBack(value);
        }
    }

    public static void Register(string eventName, Action<T> callBack)
    {
        List<Action<T>> callBacks;
        if (!eventTs.TryGetValue(eventName, out callBacks))
        {
            callBacks = new List<Action<T>>();
            eventTs.Add(eventName, callBacks);
        }

        callBacks.Add(callBack);
    }

    public static void UnRegister(string eventName, Action<T> callBack)
    {
        List<Action<T>> callBacks;
        if (!eventTs.TryGetValue(eventName, out callBacks))
        {
            return;
        }

        callBacks.Remove(callBack);
    }
}
