using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaskUI : MonoBehaviour
{
    private Text descText;

    void Awake()
    {
        descText = transform.Find("Main/Desc").GetComponent<Text>();
        Event.Register(Event.COMMAND_COMPLETED, OnClose);
    }

    public void SetDesc(string desc)
    {
        descText.text = desc;
    }

    public void OnClose()
    {
        ViewManager.instance.HideUI("MaskUI");
    }
}
