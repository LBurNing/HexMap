using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ResBrushManager
{
    private TextureData _texData;
    private HexCell _brushCell;
    private static ResBrushManager _instance;

    public static ResBrushManager instance
    {
        get
        {
            if (_instance == null)
                _instance = new ResBrushManager();

            return _instance;
        }
    }

    public ResBrushManager()
    {
        Event<HexCell>.Register(Event.BRUSH_MOUSE_CLICK, MouseClickEvent);
        Event<int>.Register(Event.ADD_BRUSH_SCALE, UpdateBrushScale);
    }

    public void Update()
    {
        if (_brushCell != null)
        {
            bool pointer = EventSystem.current.IsPointerOverGameObject();
            _brushCell.gameObject.SetActive(!pointer);

            if (!pointer)
            {
                Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                _brushCell.transform.position = worldMousePos;
            }
        }
    }

    private void UpdateBrushScale(int add)
    {
        if (_brushCell == null)
            return;

        if (Global.instance.resType == LayerType.floor)
            return;

        Vector3 scale = _brushCell.localScale + Vector3.one * add;
        if (scale.x >= Global.BRUSH_MAX_SCALE)
            scale = Vector3.one * Global.BRUSH_MAX_SCALE;

        if (scale.x <= 0)
            scale = Vector3.one;

        _brushCell.localScale = scale;
    }

    public HexCell brush
    {
        get { return _brushCell; }
    }

    public bool IsNull
    {
        get { return _texData == null; }
    }

    public void UpdateBrush(TextureData texData)
    {
        _texData = texData;
        if (_brushCell == null)
        {
            _brushCell = HexGrid.instance.CreateCell();
            GameObject.Destroy(_brushCell.gameObject.GetComponent<MeshCollider>());
        }

        _brushCell.SetResName(_texData._fileName, _texData._resName, Global.instance.resType, 1000);
    }

    private void MouseClickEvent(HexCell cell)
    {
        if (cell == null)
            return;

        if (IsNull)
            return;

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightAlt))
        {
            ResTitleRes(cell);
        }
        else
        {
            SetTitleRes(cell);
        }
    }

    private void ResTitleRes(HexCell cell)
    {
        cell.ResetResName();
    }

    private void SetTitleRes(HexCell cell)
    {
        if (_brushCell == null)
            return;

        if (Global.instance.resType == LayerType.fog)
        {
            if (!BrushManager.instance.fogPos.Exists(t => t.x == cell.hexCellData.x && t.y == cell.hexCellData.y))
            {
                HexGrid.GridPos gridPos = new HexGrid.GridPos(cell.hexCellData.x, cell.hexCellData.y);
                Global.instance.brushColors = new Color(0.21f, 0.21f, 0.21f, 1);
                BrushManager.instance.fogPos.Add(gridPos);
            }
            BrushManager.instance.DrawBrushFog();
        }

        cell.SetResName(_texData._fileName, _texData._resName, Global.instance.resType);
    }

    public void Destroy()
    {
        if (_brushCell != null)
            _brushCell.Destroy();

        _brushCell = null;
        _texData = null;
    }
}
