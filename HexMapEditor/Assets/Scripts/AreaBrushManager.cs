using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AreaBrushManager
{
    private AreaType _areaType;
    private HexCell _brushCell;
    private static AreaBrushManager _instance;

    public static AreaBrushManager instance
    {
        get
        {
            if (_instance == null)
                _instance = new AreaBrushManager();

            return _instance;
        }
    }

    public AreaBrushManager()
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
                _brushCell.transform.position = worldMousePos + new Vector3(0, 0, -1);
            }
        }
    }

    private void UpdateBrushScale(int add)
    {
        if (_brushCell == null)
            return;

        Vector3 scale = _brushCell.localScale + Vector3.one * add;
        if (scale.x >= Global.BRUSH_MAX_SCALE)
            scale = Vector3.one * Global.BRUSH_MAX_SCALE;

        if (scale.x <= 0)
            scale = Vector3.one;

        _brushCell.localScale = scale;
    }

    private void MouseClickEvent(HexCell cell)
    {
        if (cell == null)
            return;

        if (_areaType == null)
            return;

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightAlt))
        {
            AreaManager.instance.RemoveAreaPos(cell._hexCellData.x, cell._hexCellData.y);
        }
        else
        {
            AreaManager.instance.SetAreaPos(cell._hexCellData.x, cell._hexCellData.y);
        }
    }

    public bool IsNull
    {
        get { return _areaType == null; }
    }

    public HexCell brush
    {
        get { return _brushCell; }
    }

    public AreaType cellAreaType
    {
        get { return _areaType; }
        set
        {
            _areaType = value;
            if (_areaType == null)
            {
                Destroy();
            }
            else
            {
                if (_brushCell == null)
                {
                    _brushCell = HexGrid.instance.CreateCell();
                    GameObject.Destroy(_brushCell.gameObject.GetComponent<MeshCollider>());
                }


                Color color = AreaManager.instance.GetAreaColor(_areaType.areaType - 1);
                Material material = Global.instance.textureExMat;
                _brushCell.meshRenderer.material = material;
                _brushCell.meshRenderer.material.renderQueue = 3010;
                _brushCell.color = color;
            }
        }
    }

    public void Destroy()
    {
        if (_brushCell != null)
            _brushCell.Destroy();

        _areaType = null;
        _brushCell = null;
    }
}
