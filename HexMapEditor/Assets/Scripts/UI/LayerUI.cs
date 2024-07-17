using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LayerUI : MonoBehaviour
{
    private Toggle map;
    private Toggle unit;
    private Toggle fog;
    private Button help;
    private Button ok;
    private Button canel;
    private Button close;
    private GameObject treeBtnTemp;
    private LayerInfoUI layerInfoUI;

    private LayerType layer;

    private void Awake()
    {
        InitUI();

    }

    void Start()
    {
        AddClickEvent();
        MapToggleClick(true);
    }

    public void InitUI()
    {
        layerInfoUI = transform.Find("LayerInfo").GetComponent<LayerInfoUI>();
        close = transform.Find("Body/Title/Canel").GetComponent<Button>();
        map = transform.Find("Body/ToggleGroup/map").GetComponent<Toggle>();
        unit = transform.Find("Body/ToggleGroup/unit").GetComponent<Toggle>();
        fog = transform.Find("Body/ToggleGroup/fog").GetComponent<Toggle>();
        ok = transform.Find("Floor/Ok").GetComponent<Button>();
        canel = transform.Find("Floor/Canel").GetComponent<Button>();
        help = transform.Find("Body/Method/Body/Body_2/ToBeActivated/Help").GetComponent<Button>();
        treeBtnTemp = transform.Find("TmpNode").gameObject;
    }

    private void UpdateNode(LayerType layerType)
    {
        layer = layerType;
        var hexCell = Global.instance.selectHexCell;
        var layerData = hexCell.GetLayerData(layer);
        NodeUI nodeUI = null;

        if (layerData == null)
        {
            layerData = new LayerData();
            hexCell.AddLayerData(layer, layerData);
        }

        Transform nodeParent = transform.Find("FuncTree/Viewport/Content");
        for (int i = 0; i < nodeParent.childCount; i++)
        {
            Destroy(nodeParent.GetChild(i).gameObject);
        }

        GameObject nodeGo = Instantiate(treeBtnTemp);
        nodeGo.transform.SetParent(nodeParent);
        nodeUI = nodeGo.GetComponent<NodeUI>();
        nodeUI.layerData = layerData;
        nodeUI.nodeTemplete = treeBtnTemp;
        nodeUI.layerType = layer;
        nodeUI.layerInfoUI = layerInfoUI;
        nodeUI.InitNode();
    }

    private void AddClickEvent()
    {
        map.onValueChanged.AddListener(MapToggleClick);
        unit.onValueChanged.AddListener(UnitToggleClick);
        fog.onValueChanged.AddListener(FogToggleClick);
        close.onClick.AddListener(OnClose);
        canel.onClick.AddListener(OnClose);
        ok.onClick.AddListener(SaveBtn);

        help.onClick.AddListener(() =>
        {
            Global.instance.tipsUI.SetTips("帮助", "勾选后单位在游戏中默认为隐藏状态，等待其他功能激活后才会显示并与玩家交互", null);
        });
    }

    public void MapToggleClick(bool isOn)
    {
        if (isOn)
        {
            UpdateNode(LayerType.map);
        }
    }

    public void UnitToggleClick(bool isOn)
    {
        if (isOn)
        {
            UpdateNode(LayerType.unit);
        }
    }

    public void FogToggleClick(bool isOn)
    {
        if (isOn)
        {
            UpdateNode(LayerType.fog);
        }
    }

    public void OnClose()
    {
        ViewManager.instance.HideUI("LayerUI");
        Event<HexCell>.Fire(Event.UPDATE_LAYER_INFO, InputManager.Instance.ClickedCell);
    }

    public void SaveBtn()
    {
        ViewManager.instance.HideUI("LayerUI");
        MapConfigManager.instance.SaveConfig();
        Event<HexCell>.Fire(Event.UPDATE_LAYER_INFO, InputManager.Instance.ClickedCell);
    }
}