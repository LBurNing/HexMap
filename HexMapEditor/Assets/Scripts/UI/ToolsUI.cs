using Assets.Scripts.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ToolsUI : MonoBehaviour
{

    private Button editLayerDataBtn;
    private Button btnSaveArea;
    private Button btnBrushArea;
    private Button btnAreaCanel;

    private Dropdown areaTypeDropDown;

    private GameObject layerData;
    private GameObject layerDataTemplate;
    private GameObject layerContent;
    private GameObject resUI;
    private GameObject areaUI;
    private GameObject createAreaUI;
    private GameObject maskPanel;
    private GameObject areaListContent;
    private GameObject areaListTemplate;
    private GameObject layerInfoTemplate;
    private GameObject layerInfoContent;
    private GameObject mapMethod;

    private Toggle mapToggle;
    private Toggle areaToggle;

    private Dictionary<string, AreaType> areaTypes = new Dictionary<string, AreaType>();

    private void Awake()
    {
        InitUI();
        UpdateAreaTypeDropDown();
        Event.Register(Event.UPDATE_DROPDOWN, UpdateAreaTypeDropDown);
        Event.Register(Event.UPDATE_MAP_AREA, UpdateAreaList);
        Event<HexCell>.Register(Event.MOUSE_CLICK, MouseClickEvent);
        Event<HexCell>.Register(Event.UPDATE_LAYER_INFO, UpdateLayerInfoEvent);
    }

    void Start()
    {
        AddClickEvent();
    }

    void Update()
    {
        
    }

    public void InitUI()
    {
        maskPanel = transform.Find("Body/MaskPanel").gameObject;
        layerData = transform.Find("Body/Layer/LayerData").gameObject;
        layerDataTemplate = transform.Find("Body/Layer/LayerData/Template").gameObject;
        layerContent = transform.Find("Body/Layer/LayerData/Viewport/Content").gameObject;

        editLayerDataBtn = transform.Find("Body/Layer/EditBtn").GetComponent<Button>();


        btnSaveArea = transform.Find("Body/MaskPanel/BtnSaveArea").GetComponent<Button>();
        btnBrushArea = transform.Find("Body/Area/Body_1/BtnBrushArea").GetComponent<Button>();
        btnAreaCanel = transform.Find("Body/MaskPanel/BtnCanel").GetComponent<Button>();

        resUI = transform.Find("Body/Res").gameObject;
        areaUI = transform.Find("Body/Area").gameObject;

        areaListContent = transform.Find("Body/Area/Scroll View/Viewport/Content").gameObject;
        areaListTemplate = transform.Find("Body/Area/Template").gameObject;

        areaTypeDropDown = transform.Find("Body/Area/Body_1/AreaDropdown").GetComponent<Dropdown>();

        mapToggle = transform.Find("Body/ToggleRes/Map").GetComponent<Toggle>();
        areaToggle = transform.Find("Body/ToggleRes/Area").GetComponent<Toggle>();
    }

    private void AddClickEvent()
    {
        mapToggle.onValueChanged.AddListener(delegate
        {
            Event.Fire(Event.ESC_INPUT);
        });
        areaToggle.onValueChanged.AddListener(delegate
        {
            UpdateAreaList();
            Event.Fire(Event.ESC_INPUT);
        });
        areaTypeDropDown.onValueChanged.AddListener((value) =>
        {
            if (areaUI.gameObject.activeSelf == true)
            {
                AreaType area = areaTypes[areaTypeDropDown.options[value].text];
                AreaManager.instance.selectAreaType = area;
                AreaManager.instance.areaId = 0;
            }

        });

        btnBrushArea.onClick.AddListener(() =>
        {
            AreaManager.instance.CreateArea();
            AreaBrushManager.instance.cellAreaType = AreaManager.instance.selectAreaType;
            Global.instance.brushColors = Color.yellow;
            maskPanel.SetActive(true);
            BrushManager.instance.Destroy();
            ResBrushManager.instance.Destroy();
        });

        btnSaveArea.onClick.AddListener(() =>
        {
            ViewManager.instance.ShowView("AreaAttrUI");
            maskPanel.SetActive(false);
            AreaBrushManager.instance.Destroy();
        });

        btnAreaCanel.onClick.AddListener(() =>
        {
            maskPanel.SetActive(false);
            AreaBrushManager.instance.Destroy();
            AreaManager.instance.Reset();
            string areaName = string.Format("{0}_{1}", AreaManager.instance.selectAreaType.areaEName, AreaManager.instance.selectAreaType.areaType);
            GameObject areaGo = GameObject.Find(areaName);
            Destroy(areaGo);
        });
        editLayerDataBtn.onClick.AddListener(()=> 
        {
            ViewManager.instance.ShowView("LayerUI");
        });
    }

    public void OnCreateAreaType()
    {
        ViewManager.instance.ShowView("CreateAreaUI");
    }

    public void MouseClickEvent(HexCell hexCell)
    {
        if (hexCell == null)
            return;

        if (!ResBrushManager.instance.IsNull || !BrushManager.instance.IsNull || !AreaBrushManager.instance.IsNull)
            return;

        Global.instance.selectHexCell = hexCell;

        UpdateLayerInfoUI(hexCell);
    }

    public void UpdateLayerInfoEvent(HexCell hexCell)
    {
        if (hexCell == null)
            return;

        UpdateLayerInfoUI(hexCell);
    }

    public void UpdateLayerInfoUI(HexCell hexCell)
    {
        for (int i = 0; i < layerContent.transform.childCount; i++)
        {
            Destroy(layerContent.transform.GetChild(i).gameObject);
        }

        foreach (var value in hexCell._hexCellData.layerDatas)
        {
            LayerData layer = value.Value;
            if (layer != null && !string.IsNullOrEmpty(layer.funcName))
            {
                layerData.SetActive(true);
                string name = Enum.GetName(typeof(LayerType), value.Key);
                ShowLayer(name, layer.funcId, layer.funcParams1, layer.funcParams2, layer.removeUnit == 0, layer.active == 0, layer.dis);

                if (layer.layerDatas.Count > 0)
                {
                    foreach (var it in layer.layerDatas)
                    {
                        ShowLayer(name, it.funcId, it.funcParams1, it.funcParams2, it.removeUnit == 0, it.active == 0, it.dis);
                    }
                }
            }
        }
    }

    public void ShowLayer(string tp, int funcId, string params1, string params2, bool removeUnit, bool active, int dis)
    {
        GameObject templateGo = Instantiate(layerDataTemplate);
        templateGo.SetActive(true);
        templateGo.transform.name = funcId.ToString();
        templateGo.transform.parent = layerContent.transform;

        templateGo.GetComponent<Image>().color = new Color(0.52f, 0.52f, 0.52f);

        Text MapResId = templateGo.transform.Find("Head/MapResId").GetComponent<Text>();
        templateGo.transform.Find("Head/LayerName").GetComponent<Text>().text = tp;

        Text funcParams1 = templateGo.transform.Find("Body_1/Arguments1").GetComponent<Text>();
        Text funcParams2 = templateGo.transform.Find("Body_1/Arguments2").GetComponent<Text>();

        Toggle ToBeActivated = templateGo.transform.Find("Body_2/ToBeActivated").GetComponent<Toggle>();
        ToBeActivated.interactable = false;
        Toggle RemoveUnit = templateGo.transform.Find("Body_2/RemoveUnit").GetComponent<Toggle>();
        RemoveUnit.interactable = false;
        Text Distance = templateGo.transform.Find("Body_3/Distance").GetComponent<Text>();
        Text func = templateGo.transform.Find("Body_1/FuncType").GetComponent<Text>();

        MethodCFG methodCFG = new MethodCFG();
        ReadExcelData.methodDic.TryGetValue(funcId, out methodCFG);
        func.text = "功    能：" + (methodCFG.Name == "" ? "未配置功能" : methodCFG.Name);
        funcParams1.text = "参数一：" + (params1 == "" ? "未配置参数" : params1);
        funcParams2.text = "参数二：" + (params2 == "" ? "未配置参数" : params2);
        RemoveUnit.isOn = removeUnit;
        ToBeActivated.isOn = active;
        Distance.text = dis.ToString();
        templateGo.transform.localScale = Vector3.one;
        string[] info = Global.instance.selectHexCell._hexCellData.resTypeToResNames[methodCFG.LayerType].Split(',');
        MapResId.text = string.Format("ID:<color=#FF0000>{0}</color>", info[0]);
    }


    public void UpdateAreaTypeDropDown()
    {
        List<AreaType> areaTypeList = AreaManager.instance.GetAreaTypeList();
        if (areaTypeList == null)
            return;

        areaTypeDropDown.ClearOptions();
        foreach (AreaType value in areaTypeList)
        {
            Dropdown.OptionData data = new Dropdown.OptionData();
            Color color = AreaManager.instance.GetAreaColor(value.areaType - 1);
            string colorStr = ColorUtility.ToHtmlStringRGBA(new Color(color.r, color.g, color.b, 1));
            string text = string.Format("<color=#{0}> {1}</color>", colorStr, string.Format("({0})", value.areaType) + value.areaName);
            data.text = text;
            areaTypeDropDown.options.Add(data);

            AreaType area = new AreaType();
            area.areaType = value.areaType;
            area.areaName = value.areaName;
            area.areaEName = value.areaEName;
            areaTypes[data.text] = area;
        }
        if (areaTypes.Count > 0)
        {
            areaTypeDropDown.value = 0;
            AreaManager.instance.selectAreaType = areaTypeList[0];
        }
        areaTypeDropDown.RefreshShownValue();
    }

    public void OnMapDeleteBtn(GameObject game)
    {
        string areaName = game.transform.Find("Item Label").GetComponent<Text>().text;
        areaTypeDropDown.Hide();
        int areaType = int.Parse(Tools.MidStrEx(areaName, "(", ")"));
        List<AreaType> areaTypeList = AreaManager.instance.GetAreaTypeList();

        foreach (AreaType v in areaTypeList)
        {
            if (v.areaType == areaType)
            {

                Global.instance.tipsUI.SetTips("提示", "是否删除笔刷", () => {
                    AreaManager.instance.RemoveAreaType(v);
                    UpdateAreaTypeDropDown();
                });
                return;
            }
        }

    }

    public void UpdateAreaList()
    {
        for (int i = 0; i < areaListContent.transform.childCount; i++)
        {
            Destroy(areaListContent.transform.GetChild(i).gameObject);
        }

        if (AreaManager.instance.areaDic == null || AreaManager.instance.areaDic.Count <= 0)
            return;

        foreach (var item in AreaManager.instance.areaDic)
        {
            GameObject templateGo = Instantiate(areaListTemplate);
            templateGo.SetActive(true);

            templateGo.transform.name = item.Value.areaName.ToString();
            templateGo.transform.SetParent(areaListContent.transform);
            templateGo.transform.Find("TexId").GetComponent<Text>().text = item.Key.ToString();
            Button btnArea = templateGo.transform.Find("BtnArea").GetComponent<Button>();
            Button btnRemove = templateGo.transform.Find("BtnRemove").GetComponent<Button>();
            btnArea.transform.Find("Text").GetComponent<Text>().text = item.Value.areaName.ToString();

            btnArea.onClick.AddListener(() =>
            {
                AreaType areaType = AreaManager.instance.GetAreaType(item.Value.areaType);
                AreaManager.instance.selectAreaType = areaType;
                AreaBrushManager.instance.cellAreaType = areaType;
                AreaManager.instance.drawAreaing = item.Value;
                AreaManager.instance.DrawArea(item.Key);
                AreaManager.instance.areaId = item.Key;
                maskPanel.SetActive(true);
                BrushManager.instance.Destroy();
                ResBrushManager.instance.Destroy();
            });

            btnRemove.onClick.AddListener(() =>
            {
                Global.instance.tipsUI.SetTips("提示", "确定删除此区域么", () =>
                {
                    AreaType areaType = AreaManager.instance.GetAreaType(item.Value.areaType);
                    AreaManager.instance.areaDic.Remove(item.Key);
                    UpdateAreaList();
                    string areaName = string.Format("{0}_{1}", areaType.areaEName, areaType.areaType);
                    GameObject go = GameObject.Find(areaName);
                    if (go != null)
                        Destroy(go);

                });
            });
            templateGo.transform.localScale = Vector3.one;

        }
    }

    public void GenerateEnum()
    {
        List<AreaType> areaTypeList = AreaManager.instance.GetAreaTypeList();
        if (areaTypeList == null)
            return;

        StringBuilder builder = new StringBuilder();
        //#region 导出服务器枚举
        //builder.Append("package global\n\n");
        //builder.Append("type EnumAreaType int\n\n");
        //builder.Append("const (\n");

        //AreaType areaType = null;
        //for (int i = 0; i < areaTypeList.Count; i++)
        //{
        //    areaType = areaTypeList[i];
        //    string line = "\tEART_" + areaType.areaEName + " EnumAreaType = " + areaType.areaType + " //" + areaType.areaName + "\n";
        //    builder.Append(line);
        //}
        //builder.Append(")");
        //File.WriteAllText(FileUtil.areaRuleEnumServerPath + "area_type_enum.go", builder.ToString());
        //#endregion

        #region 导出客户端枚举
        builder.Clear();
        builder.Append("-- 地图区域类型");
        builder.AppendLine();
        builder.Append("EmMapAreaType = \n{\n");
        for (int i = 0; i < areaTypeList.Count; i++)
        {
            string line = "\t" + areaTypeList[i].areaEName + " = " + areaTypeList[i].areaType + ",";
            builder.Append("\t-- " + areaTypeList[i].areaName);
            builder.AppendLine();
            builder.Append(line);
            builder.AppendLine();

            if (i < areaTypeList.Count - 1)
            {
                builder.AppendLine();
            }
        }
        builder.Append("}");
        File.WriteAllText(FileUtil.areaRuleEnumClientPath + "EmMapAreaType.lua", builder.ToString());
        #endregion
        Global.instance.tipsUI.SetTips("提示", "区域枚举导出成功\nEmMapAreaType.lua", null);
    }
}
