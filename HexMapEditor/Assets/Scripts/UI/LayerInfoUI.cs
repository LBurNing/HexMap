using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LayerInfoUI : MonoBehaviour
{
    private Dropdown funcDropDown;
    private InputField funcParams1;
    private InputField funcParams2;
    private InputField dis;
    private Text funcName;
    private Toggle toBeActivatedUnit;
    private Toggle removeUnits;
    private Button ok;
    private int funcId = 0;
    public bool inited = false;

    public LayerData layerData { get; set; }

    public void Init()
    {
        if (inited) return;
        funcName = transform.Find("Body/Body_1/funcName/Label").GetComponent<Text>();
        funcDropDown = transform.Find("Body/Body_1/funcName").GetComponent<Dropdown>();
        funcParams1 = transform.Find("Body/Body_2/funcParams1").GetComponent<InputField>();
        funcParams2 = transform.Find("Body/Body_3/funcParams2").GetComponent<InputField>();
        dis = transform.Find("Body/Body_4/Distance").GetComponent<InputField>();
        toBeActivatedUnit = transform.Find("Body/Body_5/ToBeActivated").GetComponent<Toggle>();
        removeUnits = transform.Find("Body/Body_5/RemoveUnit").GetComponent<Toggle>();
        ok = transform.Find("Body/Body_6/Ok").GetComponent<Button>();
        ok.onClick.AddListener(SaveLayerUnit);
        funcDropDown.onValueChanged.AddListener(FuncClick);
        inited = true;
    }

    public void FuncClick(int index)
    {
        MethodCFG methodCFG = new MethodCFG();
        ReadExcelData.funcDic.TryGetValue(funcDropDown.options[index].text, out methodCFG);
        if (methodCFG == null)
            return;

        funcId = methodCFG.ID;
        funcParams1.transform.Find("Placeholder").GetComponent<Text>().text = methodCFG.Arguments1;
        funcParams2.transform.Find("Placeholder").GetComponent<Text>().text = methodCFG.Arguments2;
    }

    public void UpdateLayerInfo(LayerType layer)
    {
        //≥ı ºªØ
        funcParams1.text = "";
        funcParams2.text = "";
        dis.text = "";
        removeUnits.isOn = false;
        toBeActivatedUnit.isOn = false;

        funcDropDown.ClearOptions();
        foreach (var v in ReadExcelData.methodDic)
        {
            if (v.Value.LayerType == (int)layer)
            {
                Dropdown.OptionData data = new Dropdown.OptionData();
                data.text = v.Value.Name;
                funcDropDown.options.Add(data);
            }
        }

        funcDropDown.RefreshShownValue();
        if (layerData != null && layerData.funcId != 0)
        {
            List<Dropdown.OptionData> options = funcDropDown.GetComponent<Dropdown>().options;
            for (int i = 0; i < options.Count; i++)
            {
                if (options[i].text == layerData.funcName)
                {
                    funcDropDown.GetComponent<Dropdown>().value = i;
                }
            }

            funcName.text = layerData.funcName;
            funcParams1.text = layerData.funcParams1;
            funcParams2.text = layerData.funcParams2;
            removeUnits.isOn = layerData.removeUnit == 0 ? true : false;
            toBeActivatedUnit.isOn = layerData.active == 0 ? true : false;
            dis.text = layerData.dis.ToString();
        }
        else
        {
            funcDropDown.GetComponent<Dropdown>().value = 1;
            funcDropDown.GetComponent<Dropdown>().value = 0;
        }

        gameObject.SetActive(true);
    }

    public void SaveLayerUnit()
    {
        layerData.funcId = funcId;
        layerData.funcName = funcDropDown.captionText.text;
        layerData.funcParams1 = funcParams1.text;
        layerData.funcParams2 = funcParams2.text;
        layerData.removeUnit = removeUnits.isOn ? 0 : 1;
        layerData.active = toBeActivatedUnit.isOn ? 0 : 1;
        string distance = dis.text;
        if (string.IsNullOrEmpty(distance))
            distance = "0";

        layerData.dis = int.Parse(distance);
        gameObject.SetActive(false);
        Event<HexCell>.Fire(Event.UPDATE_LAYER_INFO, InputManager.Instance.ClickedCell);
    }
}
