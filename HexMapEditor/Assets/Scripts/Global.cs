using System;
using System.Collections.Generic;
using SystemTipsInfo;
using UnityEngine;
using static HexGrid;

public enum CellFunction
{
    None,
    ShowLayerUI,
    UpdateLayerUIScale,
    DestroyLayerUI,
    ShowGridPosUI,
    DestroyGridPosUI,
}

/// <summary>
/// 图片块制作规范
/// 图片高度 Global.instance._diameter * 100
/// 图片宽度 Global.instance._diameter * 100 * 0.866025404f
/// </summary>
public class Global
{
    private static int _x;
    private static int _y;
    private static int _lineCount;
    private static int _rowCount;
    private float _sprt = 1/*0.866025404f*/;
    private static float _diameter;
    private static LayerType _resType;
    private static Color _brushColors;
    private static HexCell _selectHexCell;

    private static Global _instance;

    private Dictionary<CellsType, Color> _colors = new Dictionary<CellsType, Color>()
    {
        [CellsType.Walkable] = new Color(0.4f, 0.9f, 0.85f, 1),
        [CellsType.Obstacle] = Color.red,
        [CellsType.DynamicBlocking] = Color.blue,
    };

    private static MapUI _mapUI;

    private static GameObject _loading;

    public static string TEX_NAME = "";
    public static string TEXT_NAME = "";
    public static readonly string[] WARP = new string[] { "\r\n" };
    public static int BRUSH_MAX_SCALE = 5;

    public static Global instance
    {
        get
        {
            if (_instance == null)
                _instance = new Global();

            return _instance;
        }
    }

    //外半径
    private float _outerRadius;
    //内半径
    public float _innerRadius;//outerRadius * 0.866025404f;

    public string GetResName(string fileName, string resName)
    {
        string name = string.Format("{0},{1}", fileName, resName);
        return name;
    }

    public int x
    {
        get { return _x; }
        set { _x = value; }
    }

    public int y
    {
        get { return _y; }
        set { _y = value; }
    }

    public float sqrt
    {
        get { return _sprt; }
        set { _sprt = value; }
    }

    public float outerRadius
    {
        get { return _outerRadius; }
        set { _outerRadius = value; }
    }

    public float innerRadius
    {
        get { return _innerRadius; }
        set { _innerRadius = value; }
    }

    public float diameter
    {
        get { return Mathf.Max(0.1f, _diameter); }
        set { _diameter = value; }
    }

    public LayerType resType
    {
        get { return _resType; }
        set { _resType = value; }
    }

    public int lineCount
    {
        get { return _lineCount; }
        set { _lineCount = value; }
    }

    public int rowCount
    {
        get { return _rowCount; }
        set { _rowCount = value; }
    }

    public Color brushColors
    {
        get 
        {
            if (_brushColors == null)
                _brushColors = Color.white;

            return _brushColors;
        }
        set
        {
            _brushColors = value;
        }
    
    }

    public GridPos gridPos { get; set; }
    public int textureWidth
    {
        get
        {
            int width = Mathf.RoundToInt(_diameter * 100);
            return width;
        }
    }

    public int textureHeight
    {
        get
        {
            int width = Mathf.RoundToInt(_diameter * 100 * _sprt);
            return width;
        }
    }

    public Material textureExMat
    {
        get
        {
            return Resources.Load<Material>("Material/TextureExMat"); ;
        }
    }

    public Material areaMat
    {
        get
        {
            return Resources.Load<Material>("Material/Area"); ;
        }
    }

    public Color GetColor(CellsType cellType)
    {
        return _colors[cellType];
    }

    public void SetDiameter(float diameter)
    {
        _diameter = diameter;
        _outerRadius = _diameter / 2;
        _innerRadius = _diameter / 2 * _sprt;
    }

    public void SetGridCount(int x, int y)
    {
        _lineCount = x;
        _rowCount = y;
    }

    public Vector3 GetPos(int x, int y)
    {
        Vector3 pos = new Vector3();
        pos.x = (x + y * 0.5f - y / 2) * (_innerRadius * 2f);
        pos.y = y * (_outerRadius * 1.5f);
        pos.z = 0;

        return pos;
    }

    public Vector3 GetMaxPos()
    {
        Vector3 pos = new Vector3();
        pos.x = (_lineCount + _rowCount * 0.5f - _rowCount / 2) * (_innerRadius * 2f);
        pos.y = _rowCount * (_outerRadius * 1.5f);
        pos.z = 0;

        return pos;
    }

    public Vector3 GetMinPos()
    {
        return new Vector3(-_innerRadius, -_outerRadius, 0);
    }

    public Vector2Int GetGridPos(Vector3 worldPos)
    {
        Vector3 min = GetMinPos();
        Vector3 max = GetMaxPos();

        if (worldPos.x < min.x || worldPos.x > max.x)
            return new Vector2Int(-1, -1);

        if (worldPos.y < min.y || worldPos.y > max.y)
            return new Vector2Int(-1, -1);

        HexCoordinates coordinates = HexCoordinates.FromPosition(worldPos);
        int index = coordinates.X + coordinates.Z * _lineCount + coordinates.Z / 2;

        int x = index;
        int y = 0;

        if(_lineCount == 0)
        {
            return new Vector2Int(-1, -1);
        }

        if (index >= _lineCount)
        {
            y = index / _lineCount;
            x = index - y * _lineCount;
        }

        return new Vector2Int(x, y);
    }

    public Vector3[] corners
    {
        get
        {
            Vector3[] _corners =
            {
                new Vector3(0f, 0f, _outerRadius),
                new Vector3(_innerRadius, 0f, 0.5f * _outerRadius),
                new Vector3(_innerRadius, 0f, -0.5f * _outerRadius),
                new Vector3(0f, 0f, -_outerRadius),
                new Vector3(-_innerRadius, 0f, -0.5f * _outerRadius),
                new Vector3(-_innerRadius, 0f, 0.5f * _outerRadius),
                new Vector3(0f, 0f, _outerRadius)
            };

            return _corners;
        }
    }

    public HexCell selectHexCell
    {
        get
        {
            return _selectHexCell;
        }
        set
        {
            _selectHexCell = value;
        }
    }

    public SystemTipsUI systemTipsUI
    {
        get 
        {
            GameObject go = ViewManager.instance.ShowView("SystemTipsUI");
            SystemTipsUI systemTips = go.GetComponent<SystemTipsUI>();
            return systemTips; 
        }
    }

    public TipsUI tipsUI
    {
        get 
        {
            GameObject go = ViewManager.instance.ShowView("TipsUI", ViewLayerType.Tips);
            TipsUI tips = go.GetComponent<TipsUI>();
            return tips; 
        }
    }

    public MaskUI maskUI
    {
        get
        {
            GameObject go = ViewManager.instance.ShowView("MaskUI", ViewLayerType.Mask);
            MaskUI mask = go.GetComponent<MaskUI>();
            return mask;
        }
    }

    public MapUI mapUI
    {
        get { return _mapUI; }
        set { _mapUI = value; }
    }

    public GameObject loading 
    {
        get { return _loading; }
        set { _loading = value; }
    }

    public void Reset()
    {
        _lineCount = 0;
        _rowCount = 0;
        _diameter = 0;
        selectHexCell = null;
    }
}