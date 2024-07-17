using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using System.ComponentModel;
using Newtonsoft.Json;

public enum CellsType
{
    None = -1,
    Walkable,
    Obstacle,
    DynamicBlocking,
    SpecialPassage, //特殊通行
}

public class LayerData
{
    public int funcId;
    public string funcName;
    public bool funBool = true;
    public string funcParams1;
    public string funcParams2;
    public int removeUnit;
    public int active;
    public int dis;
    [JsonIgnore]
    public LayerData parent;
    public List<LayerData> layerDatas = new List<LayerData>();
}

public class AreaData
{
    public int areaId;
    public string areaName;
}

public class HexCellData
{
    public int orderInLayer;
    public Dictionary<int, string> resTypeToResNames;
    public Dictionary<int, LayerData> layerDatas;
    public CellsType cellType;
    public int x;
    public int y;

    public Dictionary<int, LayerData> GetLayerDatas()
    {
        Dictionary<int, LayerData> datas = new Dictionary<int, LayerData>();
        foreach (var data in layerDatas)
        {
            if(!string.IsNullOrEmpty(data.Value.funcName))
            {
                datas.Add(data.Key, data.Value);
            }
        }

        return datas;
    }

    public HexCellData()
    {
        cellType = CellsType.Walkable;
        resTypeToResNames = new Dictionary<int, string>();
        layerDatas = new Dictionary<int, LayerData>();  
    }
}

public class HexCell : MonoBehaviour
{
	public HexCoordinates coordinates;
	private MaterialPropertyBlock _block;
    private MeshRenderer _meshRenderer;
    private MeshFilter _meshFilter;
    private MeshCollider _meshCollider;
    private Vector3 _position;

    public HexCellData _hexCellData;
    private Dictionary<CellFunction, Action> _functions;
    private Dictionary<LayerType, SpriteAnimation> _layers;
    private AreaType _areaType;
    private LayerWorldUI _layerWorldUI;
    private TextMesh _gridPosText;

    private void Awake()
    {
        _layers = new Dictionary<LayerType, SpriteAnimation>();
        _functions = new Dictionary<CellFunction, Action>()
        {
            [CellFunction.ShowLayerUI] = ShowLayerUI,
            [CellFunction.UpdateLayerUIScale] = UpdateLayerUIScale,
            [CellFunction.DestroyLayerUI] = DestroyLayerUI,
            [CellFunction.ShowGridPosUI] = ShowGridPosUI,
            [CellFunction.DestroyGridPosUI] = DestroyGridPosUI,
        };

        _hexCellData = new HexCellData();
        _block = new MaterialPropertyBlock();
        _areaType = new AreaType();
    }

    private void Update()
    {
        foreach (var val in _layers)
        {
            val.Value?.UpdateFrame();
        }
    }

    public void Function(CellFunction function)
    {
        Action action;
        if (!_functions.TryGetValue(function, out action))
            return;

        action.Invoke();
    }

    private void ShowLayerUI()
    {
        //foreach (var val in _layers)
        //{
        //    List<LayerData> layerData = GetLayerData(val.Key);
        //    if (layerData == null
        //        || string.IsNullOrEmpty(layerData.funcId.ToString())
        //        || string.IsNullOrEmpty(layerData.funcParams1))
        //    {
        //        continue;
        //    }

        //    GameObject layerWorld = ViewManager.instance.LoadUI("LayerWorldUI");
        //    layerWorld.transform.SetParent(transform);
        //    layerWorld.transform.localPosition = Vector3.zero;
        //    layerWorld.transform.localRotation = Quaternion.Euler(90, 0, 0);
        //    _layerWorldUI = layerWorld.AddComponent<LayerWorldUI>();
        //    _layerWorldUI.UpdateLayerUI(this);
        //    return;
        //}
    }

    private void ShowGridPosUI()
    {
        if (_gridPosText == null)
        {
            GameObject go = new GameObject("Pos");
            go.transform.parent = transform;
            go.transform.localScale = Vector3.one * 0.6f;
            go.transform.localRotation = Quaternion.Euler(90, 0, 0);
            go.transform.localPosition = Vector3.zero;
            _gridPosText = go.AddComponent<TextMesh>();
            _gridPosText.anchor = TextAnchor.MiddleCenter;
        }

        _gridPosText.text = coordinates.X + "_" + coordinates.Z;
        _gridPosText.gameObject.SetActive(true);
    }

    private void DestroyGridPosUI()
    {
        if (_gridPosText == null)
            return;

        _gridPosText.gameObject.SetActive(false);
    }

    private void UpdateLayerUIScale()
    {
        _layerWorldUI?.UpdateScale();
    }

    private void DestroyLayerUI()
    {
        if (_layerWorldUI != null && _layerWorldUI.gameObject)
        {
            Destroy(_layerWorldUI.gameObject);
            _layerWorldUI = null;
        }
    }

    public void AddLayerData(LayerType resType, LayerData layerData)
    {
        if(!_hexCellData.layerDatas.ContainsKey((int)resType))
        {
            _hexCellData.layerDatas.Add((int)resType, layerData);
        }
        else
        {
            _hexCellData.layerDatas[(int)resType] = layerData;
        }
    }

    public LayerData GetLayerData(LayerType resType)
    {
        if (_hexCellData.layerDatas.ContainsKey((int)resType))
            return _hexCellData.layerDatas[(int)resType];

        return null;
    }

    public void SetHexCellData(HexCellData hexCellData)
    {
        _hexCellData = hexCellData;
    }

    public void SetResName(string fileName, string resName, LayerType resType, int addLayer = 0)
    {
        TextureData texData = ResManager.instance.GetTextureData(resType.ToString(), fileName, resName);
        _hexCellData.resTypeToResNames[(int)resType] = Global.instance.GetResName(fileName, resName);
        if (string.IsNullOrEmpty(texData._resName))
            return;

        SpriteAnimation animator = null;
        if (!_layers.TryGetValue(resType, out animator))
        {
            animator = new SpriteAnimation(texData._resName);
            animator.spriteMgr.parent = transform;
            animator.spriteMgr.localRotation = Quaternion.Euler(90, 0, 0);
            _layers.Add(resType, animator);
        }

        animator.Reset();
        animator.SetTextureData(texData);
        animator.SetSprite();
        animator.Play();

        if (resType == LayerType.floor)
        {
            var z = (Global.instance.diameter / 2) * -1;
            animator.spriteMgr.localPosition = new Vector3(0, 0, z);
        }

        int layer = 0;
        if (resType != LayerType.floor)
            layer = (int)resType; //_tileRes.Count;

        int orderInLayer = Global.instance.rowCount - coordinates.Z + layer;
        _hexCellData.orderInLayer = orderInLayer;
        animator.spriteMgr.orderInLayer = orderInLayer;
          
        SetTitleColor(resType, _hexCellData.cellType);
        animator.spriteMgr.SetFogAlpha(resType);
    }

    public void SetTitleColor(LayerType resType, CellsType cellType)
    {
        if (cellType == CellsType.None)
            return;

        Color cellColor = Global.instance.GetColor(cellType);
        color = cellColor;
    }

    public void SetCellType(CellsType cellType)
    {
        _hexCellData.cellType = cellType;
    }

    public void SetRenderQueue(CellsType cellType)
    {
        meshRenderer.material.renderQueue = 3000 + (int)cellType;
    }

    public void ResetResName()
    {

        if (_layers == null)
            return;

        for (int i = 0; i < _layers.Count; i++)
        {
            if (_layers == null)
                continue;

            var value= _layers.ElementAt(i);
            if (value.Key == Global.instance.resType)
            {
                value.Value.Dispose();
                _layers.Remove(value.Key);
                _hexCellData.resTypeToResNames.Remove((int)value.Key);
            }

            if (value.Key == LayerType.fog)
            {
                BrushManager.instance.RemoveFogPos(_hexCellData.x, _hexCellData.y);
            }
        }
        
        if (ResBrushManager.instance.IsNull)
        {
            foreach (var value in _layers)
            {
                value.Value?.Dispose();
            }

            _layers.Clear();
            _hexCellData.resTypeToResNames.Clear();
        }
    }

    public void RemoveResName(LayerType resType)
    {

        if (_layers == null)
            return;

        for (int i = 0; i < _layers.Count; i++)
        {
            if (_layers == null)
                continue;

            var value = _layers.ElementAt(i);
            if (value.Key == resType)
            {
                value.Value.Dispose();
                _layers.Remove(value.Key);
                _hexCellData.resTypeToResNames.Remove((int)value.Key);
            }
        }
    }

    public GameObject GetOutlineObj()
    {
        SpriteAnimation animation;
        if (!_layers.TryGetValue(LayerType.map, out animation))
            return gameObject;

        return animation.spriteMgr.gameobject;
    }

    public Dictionary<LayerType, SpriteAnimation> layers
    {
        get { return _layers; }
    }

    public Vector3 position
    {
        set
        {
            _position = value;
        }

        get
        {
            return _position;
        }
    }

    public MeshFilter meshFilter
    {
        get
        {
            if(_meshFilter == null)
                _meshFilter = GetComponent<MeshFilter>();

            return _meshFilter;
        }
    }

    public MeshRenderer meshRenderer
    {
        get
        {
            if (_meshRenderer == null)
                _meshRenderer = GetComponent<MeshRenderer>();

            return _meshRenderer;
        }
    }

    public MeshCollider meshCollider
    {
        get
        {
            if (_meshCollider == null)
                _meshCollider = GetComponent<MeshCollider>();

            return _meshCollider;
        }
    }

    public HexCellData hexCellData
    {
        get
        {
            return _hexCellData;
        }
    }

    public Mesh sharedMesh
    {
        set
        {
            if (value == null)
                return;

            meshFilter.sharedMesh = value;
            meshCollider.sharedMesh = value;
        }

        get
        {
            return meshFilter.sharedMesh;
        }
    }

    public Vector3 localScale
    {
        get { return transform.localScale; }
        set { transform.localScale = value; }
    }

    public Color color
    {
        set
        {
            meshRenderer.GetPropertyBlock(_block);
            _block.SetColor("_Color", value);
            meshRenderer.SetPropertyBlock(_block);
        }
    }

    public void Destroy()
    {
        _block = null;
        _hexCellData = null;

        Destroy(gameObject);
    }
}