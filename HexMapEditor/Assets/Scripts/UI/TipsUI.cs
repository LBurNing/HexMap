using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TipsUI : MonoBehaviour
{
    private Action sureCallBack;
    private Action cancelCallBack;

    private Text HanderText;
    private Text BodyText;

    public void Awake()
    {
        HanderText = transform.Find("Main/Title/Text").GetComponent<Text>();
        BodyText = transform.Find("Main/Body/Text").GetComponent<Text>();
    }

    public void SetTips(string hander, string content, Action callBack = null)
    {
        HanderText.text = hander;
        BodyText.text = content;

        if (callBack != null)
            sureCallBack = callBack;
    }

    public void SetTips(string hander, string content, Action sureBack, Action cancelBack)
    {
        HanderText.text = hander;
        BodyText.text = content;

        if (sureBack != null)
            sureCallBack = sureBack;


        if (cancelBack != null)
            cancelCallBack = cancelBack;
    }

    public void OnCloseBtnClick()
    {
        ViewManager.instance.HideUI("TipsUI");

        if (cancelCallBack != null)
            cancelCallBack();

        cancelCallBack = null;
        sureCallBack = null;
    }

    public void OnSureBtnClick()
    {
        if (sureCallBack != null)
            sureCallBack();

        sureCallBack = null;
        cancelCallBack = null;
        ViewManager.instance.HideUI("TipsUI");
    }
}
