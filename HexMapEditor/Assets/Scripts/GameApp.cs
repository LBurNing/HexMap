using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HexGrid;

public class GameApp : MonoBehaviour
{
    public static GameApp instance;
    private RaycastHit hitInfo;

    public void Awake()
    {
        string str = this.GetType().Assembly.Location;
        instance = this;
        EventRegisterManager.instance.Init();
        ResManager.instance.LoadRes();
        ReadExcelData.IntiExcelData();
        ViewManager.instance.Init();
        ViewManager.instance.ShowView("MapUI");
        CopyManager.instance.Init();

        Application.wantsToQuit += () =>
        {
            Event.Fire(Event.QUIT_SAVE);
            return true;
        };
    }

    void Start()
    {
        
    }

    void Update()
    {
        UpdateGridPos();
        BrushManager.instance.Update();
        AreaBrushManager.instance.Update();
        ResBrushManager.instance.Update();
    }

    public void UpdateGridPos()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
        Physics.Raycast(inputRay, out hitInfo);
        HexCell cell = HexGrid.instance.GetCell(hitInfo.point);
        if (cell != null)
        {
            Global.instance.gridPos = new GridPos(cell._hexCellData.x, cell._hexCellData.y);
            Event.Fire(Event.UPDATE_X_Y_UI);
        }
    }

    public static Coroutine CreateCoroutine(IEnumerator co)
    {
        return instance.StartCoroutine(co);
    }

    public static void DestoryCoroutine(Coroutine co)
    {
        try
        {
            instance.StopCoroutine(co);
        }
        catch (Exception e)
        {
            Debug.LogError("stop coroutine " + e.Message);
        }
    }

    public void OnDestroy()
    {
        EventRegisterManager.instance.UnInit();
    }
}
