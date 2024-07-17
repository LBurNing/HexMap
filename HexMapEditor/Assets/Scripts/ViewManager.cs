using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ViewLayerType
{
    Begin,
    Mask,
    Default,
    Tips,
    End
}

public class ViewManager
{
    private static ViewManager _instance;
    private string _rootName = "UIRoot";
    private GameObject _uiRoot;

    private Dictionary<ViewLayerType, RectTransform> _dicLayerToLayerRoot;
    private Dictionary<string, GameObject> _openUI;

    public static ViewManager instance
    {
        get
        {
            if (_instance == null)
                _instance = new ViewManager();

            return _instance;
        }
    }

    public ViewManager()
    {
        _dicLayerToLayerRoot = new Dictionary<ViewLayerType, RectTransform>();
        _openUI = new Dictionary<string, GameObject>();
    }

    public void Init()
    {
        _uiRoot = new GameObject(_rootName);
        for (int i = (int)ViewLayerType.Begin + 1; i < (int)ViewLayerType.End; i++)
        {
            ViewLayerType type = (ViewLayerType)i;
            GameObject layerGo = new GameObject(type.ToString(), typeof(RectTransform));
            RectTransform rectTransform = layerGo.transform as RectTransform;
            rectTransform.SetParent(_uiRoot.transform);
            _dicLayerToLayerRoot[type] = rectTransform;
        }
    }

    public GameObject ShowView(string viewName, ViewLayerType type = ViewLayerType.Default)
    {
        GameObject uiGo;
        if (_openUI.TryGetValue(viewName, out uiGo))
            return uiGo;

        uiGo = LoadUI(viewName);
        RectTransform root = _dicLayerToLayerRoot[type];
        uiGo.transform.SetParent(root.transform);

        _openUI.Add(viewName, uiGo);

        return uiGo;
    }

    public void HideUI(string viewName)
    {
        if (!_openUI.ContainsKey(viewName))
            return;

        GameObject.Destroy(_openUI[viewName]);
        _openUI.Remove(viewName);
    }
    public void UIActive(string viewName)
    {
        if (!_openUI.ContainsKey(viewName))
        {
            ShowView(viewName);
            return;
        }
        GameObject.Destroy(_openUI[viewName]);
        _openUI.Remove(viewName);
    }

    public GameObject LoadUI(string name)
    {
        return GameObject.Instantiate(Resources.Load<GameObject>("UIPrefabs/" + name));
    }
}
