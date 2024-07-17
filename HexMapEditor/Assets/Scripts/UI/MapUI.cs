using Assets.Scripts.Utils;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SystemTipsInfo;
using UnityEngine;
using UnityEngine.UI;
using static HexGrid;

public class MapUI : MonoBehaviour
{
    private Text mapName;
    private Text positions;

    //地图菜单
    private Button btnMap;
    private GameObject mapMenu;
    private Button btnCreate;
    private Button btnOpen;
    private Button btnSave;
    //地图菜单结束

    //资源菜单
    private Button btnRes;
    private GameObject resMenu;
    private Button btnImport;
    private Button btnExport;
    //资源菜单结束

    //描边菜单
    private Button btnOutLine;
    private GameObject lineMenu;
    private Toggle walk;
    private Toggle obs;
    private Toggle fog;
    private Toggle off;
    //描边菜单结束

    private Button btnSetting;
    private Button btnHelp;

    private Toggle walkableToggle;
    private Toggle obsToggle;
    private Toggle dynamicObsToggle;

    private Dictionary<string, AreaType> areaTypes = new Dictionary<string, AreaType>();

    private InputField x;
    private InputField y;

    private void Awake()
    {
        InitUI();
        ResManager.instance.LoadRes();
        Event.Register(Event.ESC_INPUT, EscEvent);
        Event.Register(Event.UPDATE_MAP_INFO, UpdateMapInfo);
        Event.Register(Event.UPDATE_X_Y_UI, UpdateBrushAreaPos);
        Event.Register(Event.QUIT_SAVE, OnSaveMapConfig);
        Event.Register(Event.CONFIG_LOAD_COMPLETE, UpdateXY);
    }

    void Start()
    {
        AddEvent();
        Global.instance.systemTipsUI.AddSystemInfo("按F12查看功能说明");
    }

    void Update()
    {
    }

    public void OnDestroy()
    {
        Event.UnRegister(Event.ESC_INPUT, EscEvent);
        Event.UnRegister(Event.UPDATE_MAP_INFO, UpdateMapInfo);
        Event.UnRegister(Event.UPDATE_X_Y_UI, UpdateBrushAreaPos);
        Event.UnRegister(Event.QUIT_SAVE, OnSaveMapConfig);
        Event.UnRegister(Event.CONFIG_LOAD_COMPLETE, UpdateXY);
    }

    public void InitUI()
    {
        positions = transform.Find("HeadTools/Position").gameObject.GetComponent<Text>();
        mapName = transform.Find("HeadTools/MapName").gameObject.GetComponent<Text>();

        btnMap = transform.Find("Head/Map").GetComponent<Button>();
        mapMenu = transform.Find("Head/MapMenu").gameObject;
        btnCreate = transform.Find("Head/MapMenu/BtnCreate").GetComponent<Button>();
        btnOpen = transform.Find("Head/MapMenu/BtnOpen").GetComponent<Button>();
        btnSave = transform.Find("Head/MapMenu/BtnSave").GetComponent<Button>();

        btnRes = transform.Find("Head/Resource").GetComponent<Button>();
        resMenu = transform.Find("Head/ResMenu").gameObject;
        btnImport = transform.Find("Head/ResMenu/BtnImport").GetComponent<Button>();
        btnExport = transform.Find("Head/ResMenu/BtnExport").GetComponent<Button>();

        btnOutLine = transform.Find("Head/OutLine").GetComponent<Button>();
        lineMenu = transform.Find("Head/LineMenu").gameObject;
        walk = transform.Find("Head/LineMenu/Walk").GetComponent<Toggle>();
        obs = transform.Find("Head/LineMenu/Obs").GetComponent<Toggle>();
        fog = transform.Find("Head/LineMenu/Fog").GetComponent<Toggle>();
        off = transform.Find("Head/LineMenu/Off").GetComponent<Toggle>();

        btnSetting = transform.Find("Head/Setting").GetComponent<Button>();
        btnHelp = transform.Find("Head/Help").GetComponent<Button>();

        walkableToggle = transform.Find("HeadTools/Walkable").GetComponent<Toggle>();
        obsToggle = transform.Find("HeadTools/Obstacle").GetComponent<Toggle>();
        dynamicObsToggle = transform.Find("HeadTools/Dynamic").GetComponent<Toggle>();
        x = transform.Find("HeadTools/Point/x").GetComponent<InputField>();
        y = transform.Find("HeadTools/Point/y").GetComponent<InputField>();

        Global.instance.mapUI = this;
        string unityExePath = PlayerPrefs.GetString("unityExePath");
        string texturePackExePath = PlayerPrefs.GetString("texturePackExePath");

        if (string.IsNullOrEmpty(unityExePath) || string.IsNullOrEmpty(texturePackExePath))
        {
            ViewManager.instance.ShowView("SettingUI");
        }
        else
        {
            FileUtil.unityExePath = unityExePath;
            FileUtil.texturePackExePath = texturePackExePath;
        }

        UpdateXY();
    }

    private void UpdateXY()
    {
        x.text = Global.instance.x.ToString();
        y.text = Global.instance.y.ToString();
    }

    public void xChanged(string value)
    {
        Global.instance.x = int.Parse(value);
    }

    public void yChanged(string value)
    {
        Global.instance.y = int.Parse(value);
    }

    public void UpdateBrushAreaPos()
    {
        GridPos gridPos = Global.instance.gridPos;
        positions.text = "x = " + gridPos.x + " " + " y = " + gridPos.y;
    }

    private void AddEvent()
    {
        walkableToggle.onValueChanged.AddListener(WalkableToggleClick);
        obsToggle.onValueChanged.AddListener(ObsToggleClick);
        dynamicObsToggle.onValueChanged.AddListener(DynamicObsToggleClick);
        walk.onValueChanged.AddListener(WalkToggleClickOutline);
        obs.onValueChanged.AddListener(ObsToggleClickOutline);
        fog.onValueChanged.AddListener(FogToggleClickOutline);

        btnMap.onClick.AddListener(OpenMapMenu);
        btnRes.onClick.AddListener(OpenResMenu);
        btnOutLine.onClick.AddListener(OpenLineMenu);

        btnCreate.onClick.AddListener(OnOpenCreateUIClick);
        btnOpen.onClick.AddListener(OnOpenMap);
        btnSave.onClick.AddListener(OnSaveMapConfig);
        btnImport.onClick.AddListener(OnImport);
        btnExport.onClick.AddListener(OnExport);
        btnSetting.onClick.AddListener(OnSetting);
        x.onValueChanged.AddListener(xChanged);
        y.onValueChanged.AddListener(yChanged);

        btnHelp.onClick.AddListener(()=> 
        {
            lineMenu.SetActive(false);
            resMenu.SetActive(false);
            mapMenu.SetActive(false);
            ViewManager.instance.ShowView("HelpUI");
        });
    }

    public void OpenMapMenu()
    {
        mapMenu.SetActive(true);
        resMenu.SetActive(false);
        lineMenu.SetActive(false);
    }

    public void OpenResMenu()
    {
        resMenu.SetActive(true);
        mapMenu.SetActive(false);
        lineMenu.SetActive(false);
    }
    public void OpenLineMenu()
    {
        if (!string.IsNullOrEmpty(mapName.text))
        {
            lineMenu.SetActive(true);
            resMenu.SetActive(false);
            mapMenu.SetActive(false);
        }
        else
        {
            Global.instance.systemTipsUI.AddSystemInfo("没有打开中的地图!!!");
        }

    }

    private void ObsToggleClick(bool isOn)
    {
        if (isOn)
        {
            if (!string.IsNullOrEmpty(mapName.text))
            {
                BrushManager.instance.cellType = CellsType.Obstacle;
                ResBrushManager.instance.Destroy();
            }
            else
            {
                Global.instance.systemTipsUI.AddSystemInfo("没有打开中的地图!!!");
                obsToggle.isOn = false;
            }
           
        }
        else
        {
            BrushManager.instance.Destroy();
        }
    }

    private void WalkableToggleClick(bool isOn)
    {
        if (isOn)
        {
            if (!string.IsNullOrEmpty(mapName.text))
            {
                BrushManager.instance.cellType = CellsType.Walkable;
                ResBrushManager.instance.Destroy();
            }
            else
            {
                Global.instance.systemTipsUI.AddSystemInfo("没有打开中的地图!!!");
                walkableToggle.isOn = false;
            }
        }
        else
        {
            BrushManager.instance.Destroy();
        }
    }

    private void DynamicObsToggleClick(bool isOn)
    {
        if (isOn)
        {
            BrushManager.instance.cellType = CellsType.DynamicBlocking;
            ResBrushManager.instance.Destroy();
        }
        else
        {
            BrushManager.instance.Destroy();
        }
    }

    private void ObsToggleClickOutline(bool isOn)
    {
        BrushManager.instance.bObs = isOn;
        if (isOn)
        {
            BrushManager.instance.DrawBrushObs();
        }
        else
        {
            GameObject go = GameObject.Find("Hex_Obstacle");
            Destroy(go);
        }
    }

    private void WalkToggleClickOutline(bool isOn)
    {
        BrushManager.instance.bWalk = isOn;
        if (isOn)
        {
            BrushManager.instance.DrawBrushWalk();
        }
        else
        {
            GameObject go = GameObject.Find("Hex_Walkable");
            Destroy(go);
        }
    }

    private void FogToggleClickOutline(bool isOn)
    {
        BrushManager.instance.bFog = isOn;
        if (isOn)
        {
            BrushManager.instance.DrawBrushFog();
        }
        else
        {
            GameObject go = GameObject.Find("Hex_Fog");
            Destroy(go);
        }
    }

    public void OnOpenCreateUIClick()
    {
        ViewManager.instance.ShowView("CreatePopupUI");
    }

    public void OnOpenMap()
    {
        FileUtil.OpenMapConfigFile();
        mapName.text = MapConfigManager.instance.mapConfigName + "(" + MapConfigManager.instance.mapConfigSize + ")";
        //UpdateAreaList();
        ViewManager.instance.ShowView("ToolsUI");
        ViewManager.instance.ShowView("InfoUI");
    }

    private void EscEvent()
    {
        BrushManager.instance.Destroy();
        AreaBrushManager.instance.Destroy();
        ResBrushManager.instance.Destroy();
        //maskPanel.SetActive(false);
        GameObject areaGo = GameObject.Find(AreaManager.instance.selectAreaType.areaEName);
        Destroy(areaGo);
    }

    public void OnSaveMapConfig()
    {
        MapConfigManager.instance.SaveConfig();
    }

    public void OnImport()
    {
        ViewManager.instance.ShowView("ImportUI");
    }

    public void OnExport()
    {
        Global.instance.maskUI.SetDesc("正在打包资源, 请稍等...");
        StartCoroutine(BuildMap());
    }

    public void OnSetting()
    {
        lineMenu.SetActive(false);
        resMenu.SetActive(false);
        mapMenu.SetActive(false);
        ViewManager.instance.ShowView("SettingUI");
    }

    private IEnumerator BuildMap()
    {
        yield return new WaitForSeconds(0.5f);
        PackResManager.instance.BuildMap();
        Global.instance.tipsUI.SetTips("打包资源", "资源打包完成");
    }

    public void UpdateMapInfo()
    {
        mapName.text = MapConfigManager.instance.mapConfigName + "(" + MapConfigManager.instance.mapConfigSize + ")";
    }


}
