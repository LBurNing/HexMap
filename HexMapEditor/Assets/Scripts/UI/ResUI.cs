using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ResUI : MonoBehaviour
{
    private GridLayoutGroup resContent;
    public ResTitle resTitle;

    private GameObject BtnPanel;
    private GameObject BtnTemplate;

    private string curSelectType;

    private void Awake()
    {
        InitUI();
    }

    private void Start()
    {
        CreateResBtn();
        Event<string>.Register(Event.IMPORT_SUCCESS, ImportSuccess);
    }

    void Update()
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }


    private void InitUI()
    {
        resContent = transform.Find("Scroll View/Viewport/ResContent").gameObject.GetComponent<GridLayoutGroup>();
        BtnTemplate = transform.Find("BtnTemplate").gameObject;
        BtnPanel = transform.Find("BtnList/BtnPanel").gameObject;
    }

    private void ImportSuccess(string layerType)
    {
        if (string.IsNullOrEmpty(curSelectType))
            return;

        if (curSelectType.Equals(layerType))
        {
            ResManager.instance.UpdateRes();
            CreateImage(curSelectType);
        }
    }

    private void CreateResBtn()
    {
        var resType = ResManager.instance.GetRess();

        foreach(var ress in resType)
        {
            //只是为了处理一个报错 虽然那个报错 无伤大雅 但是不喜欢飘红
            if (BtnTemplate == null)
                return;

            GameObject newItem = Instantiate(BtnTemplate.gameObject);
            
            newItem.name = $"Btn_{ress.Key}";
            newItem.GetComponentInChildren<Text>().text = ResManager.GetDescription(ress.Key);
            
            newItem.transform.GetComponent<Toggle>().onValueChanged.AddListener((isOn)=> 
            {
                if (isOn)
                {
                    CreateImage(ress.Key);
                    Global.instance.resType = (LayerType)Enum.Parse(typeof(LayerType), ress.Key);
                    ResBrushManager.instance.Destroy();
                }
            });

            newItem.transform.SetParent(BtnPanel.transform, false);
        }

        //启动时加载第一个
        //BtnPanel.transform.GetChild(0).gameObject.GetComponent<Toggle>().Select();
        CreateImage(resType.First().Key);
        Global.instance.resType = (LayerType)Enum.Parse(typeof(LayerType), resType.First().Key);
    }

    private void CreateImage(string key)
    {
        curSelectType = key;
        if (resContent != null)
        {
            foreach(Transform tf in resContent.transform)
            {
                Destroy(tf.gameObject);
            }
        }

        var resType = ResManager.instance.GetRess();
        if (resType == null)
            return;

       var resList = resType[key];
        foreach (var res in resList)
        {
            ResTitle tempResTitle = Instantiate<ResTitle>(resTitle);
            tempResTitle.transform.SetParent(resContent.transform);
            tempResTitle.GetComponent<ResTitle>().SetTexture(res);
        }
    }
}
