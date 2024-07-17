using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static HexGrid;

public class BrushManager
{
    private CellsType _cellType;
    private HexCell _brushCell;
    public List<GridPos> obstaclePos;
    public List<GridPos> walkablePos;
    public List<GridPos> fogPos;
    private static BrushManager _instance;

    public bool bObs = false;
    public bool bWalk = false;
    public bool bFog = false;

    public bool bArea = false;
    public GameObject areaResObj;
    
    public static BrushManager instance
    {
        get
        {
            if (_instance == null)
                _instance = new BrushManager();

            return _instance;
        }
    }

    public BrushManager()
    {
        _cellType = CellsType.Walkable;
        obstaclePos = new List<GridPos>();
        walkablePos = new List<GridPos>();
        fogPos = new List<GridPos>();
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

        if (IsNull)
            return;

        GridPos gridPos = new GridPos(cell._hexCellData.x, cell._hexCellData.y);
        if (cellType == CellsType.Obstacle)
        {
            if (walkablePos != null)
            {
                RemoveWalkPos(gridPos.x, gridPos.y);
            }
                
            if (obstaclePos == null)
                obstaclePos = new List<GridPos>();
            
            if (!obstaclePos.Exists(t => t.x == gridPos.x && t.y == gridPos.y))
            {
                obstaclePos.Add(gridPos);
            }
            DrawBrushObs();
                
        }
        else if (cellType == CellsType.Walkable)
        {
            if (obstaclePos != null)
            {
                RemoveObsPos(gridPos.x, gridPos.y);
            }

            if (walkablePos == null)
                walkablePos = new List<GridPos>();

            if (!walkablePos.Exists(t => t.x == gridPos.x && t.y == gridPos.y))
            {
                walkablePos.Add(gridPos);
            }
            DrawBrushWalk();
        }

            

        cell.SetCellType(_cellType);
        cell.SetTitleColor(LayerType.map, _cellType);
        cell.SetTitleColor(LayerType.floor, _cellType);
    }
    
    public bool IsNull
    {
        get { return _cellType == CellsType.None; }
    }

    public CellsType cellType
    {
        get { return _cellType; }
        set 
        { 
            _cellType = value;
            if (_cellType == CellsType.None)
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

                Color color = Global.instance.GetColor(value);
                Material material = Global.instance.textureExMat;
                _brushCell.meshRenderer.material = material;
                _brushCell.meshRenderer.material.renderQueue = 3010;
                _brushCell.color = color;
            }
        }
    }

    public HexCell brush
    {
        get { return _brushCell; }
    }

    public void DrawBrushObs()
    {
        if (bWalk)
            DrawBrushWalk();

        if (!bObs)
            return;

        if (obstaclePos == null || obstaclePos.Count <= 0)
            return;

        Global.instance.brushColors = Global.instance.GetColor(CellsType.Obstacle);

        MeshCombineManager.Instance.DrawImage(obstaclePos, "Hex_Obstacle",2);
    }

    public void RemoveObsPos(int x, int y)
    {
        var index = obstaclePos.FindIndex(t => t.x == x && t.y == y);
        if (index == -1)
            return;

        obstaclePos.RemoveAt(index);

    }

    public void DrawBrushWalk()
    {
        if (bObs)
            DrawBrushObs();

        if (!bWalk)
            return;

        if (walkablePos == null || walkablePos.Count <= 0)
            return;

        Global.instance.brushColors = Global.instance.GetColor(CellsType.Walkable);
        MeshCombineManager.Instance.DrawImage(walkablePos, "Hex_Walkable",1);
    }

    public void RemoveWalkPos(int x, int y)
    {
        var index = walkablePos.FindIndex(t => t.x == x && t.y == y);
        if (index == -1)
            return;

        walkablePos.RemoveAt(index);
    }

    public void DrawBrushFog()
    {
        if (!bFog)
            return;

        if (fogPos == null || fogPos.Count <= 0)
            return;

        MeshCombineManager.Instance.DrawImage(fogPos, "Hex_Fog", 0);
    }

    public void RemoveFogPos(int x, int y)
    {
        var index = fogPos.FindIndex(t => t.x == x && t.y == y);
        if (index == -1)
            return;

        fogPos.RemoveAt(index);
    }

    public void Destroy()
    {
        if (_brushCell != null)
            _brushCell.Destroy();

        _cellType = CellsType.None;
        _brushCell = null;
    }

    //只需要切换地图的时候 才清除list
    public void ClearList()
    {
        walkablePos.Clear();
        obstaclePos.Clear();
        fogPos.Clear();
    }
}
