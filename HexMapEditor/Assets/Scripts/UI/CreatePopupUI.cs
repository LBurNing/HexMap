using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CreatePopupUI : MonoBehaviour
{

    private string mapName;
    private string mapPath;
    private float mapDiameter;
    private int mapLine;
    private int mapRow;

    private InputField inputPath;

    private List<GameObject> inputList;
    EventSystem system;

    // Start is called before the first frame update
    void Start()
    {

        transform.Find("Main/InputName").GetComponent<InputField>().onValueChanged.AddListener((text) => {
            mapName = text;
        });
        inputPath = transform.Find("Main/InputPath").GetComponent<InputField>();
        
        transform.Find("Main/InputDiameter").GetComponent<InputField>().onValueChanged.AddListener((text) => {
            mapDiameter = float.Parse(text);
        });
        transform.Find("Main/InputLine").GetComponent<InputField>().onValueChanged.AddListener((text) => {
            mapLine = int.Parse(text);
        });
        transform.Find("Main/InputRow").GetComponent<InputField>().onValueChanged.AddListener((text) => {
            mapRow = int.Parse(text);
        });

        inputPath.onValueChanged.AddListener((text) => {
            mapPath = text;
        });

        system = EventSystem.current;
        inputList = new List<GameObject>();
        InputField[] array = transform.GetComponentsInChildren<InputField>();
        for (int i = 0; i < array.Length; i++)
        {
            inputList.Add(array[i].gameObject);
        }
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (inputList.Contains(system.currentSelectedGameObject))
            {
                GameObject next = NextInput(system.currentSelectedGameObject);
                system.SetSelectedGameObject(next);
            }
        }
    }

    public void OnBtnCreateClick()
    {
        if (mapLine <= 0)
            mapLine = 1;

        if (mapRow <= 0)
            mapRow = 1;

        if (string.IsNullOrEmpty(mapName))
        {
            Global.instance.systemTipsUI.AddSystemInfo("地图名称不能为空!");
            return;
        }
        if (mapDiameter <= 0.0f)
        {
            Global.instance.systemTipsUI.AddSystemInfo("格子直径不能为空!");
            return;
        }

        bool exist = MapConfigManager.instance.Exists(mapName);
        if (exist)
        {
            Global.instance.systemTipsUI.AddSystemInfo("地图已存在");
            return;
        }

        #region 重置
        HexGrid.instance.Destroy();
        MapConfigManager.instance.Destroy();
        BrushManager.instance.Destroy();
        ResBrushManager.instance.Destroy();
        Global.instance.Reset();
        #endregion

        Global.instance.SetDiameter(mapDiameter);
        Global.instance.SetGridCount(mapLine, mapRow);
        HexGrid.instance.CreateMapCell();

        MapConfigManager.instance.mapConfigName = mapName;
        MapConfigManager.instance.mapConfigSize = mapLine + "X" + mapRow;
        AreaManager.instance.areaDic = new Dictionary<int, Area>();
        MapConfigManager.instance.NewConfig();
        gameObject.SetActive(false);
        ViewManager.instance.ShowView("ToolsUI");
        ViewManager.instance.ShowView("InfoUI");
        Event.Fire(Event.UPDATE_MAP_INFO);
    }

    public void OnBtnSelectClick()
    {
        inputPath.text = FileUtil.OpenMapFilepath();
    }

    public void OnCloseBtnClick()
    {
        ViewManager.instance.HideUI("CreatePopupUI");
    }

    private GameObject NextInput(GameObject input)
    {
        int indexNow = IndexNow(input);
        if (indexNow + 1 < inputList.Count)
        {
            return inputList[indexNow + 1].gameObject;
        }
        else
        {
            return inputList[0].gameObject;
        }
    }

    //获取当前选中物体的序列
    private int IndexNow(GameObject input)
    {
        int indexNow = 0;
        for (int i = 0; i < inputList.Count; i++)
        {
            if (input == inputList[i])
            {
                indexNow = i;
                break;
            }
        }
        return indexNow;
    }
}
