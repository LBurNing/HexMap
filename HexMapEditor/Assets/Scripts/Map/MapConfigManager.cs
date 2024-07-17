using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using System;

public class MapConfig
{
    public string mapName;
    public float diameter;
    public int colCount;
    public int rowCount;
    public int x;
    public int y;
    public Dictionary<int, Area> area = new Dictionary<int, Area>();
    public List<short[]> blocks = new List<short[]>();
    public List<int[]> orderInLayers = new List<int[]>(); // 1
    public List<List<Dictionary<int, string>>> res = new List<List<Dictionary<int, string>>>(); // 2 
    public List<List<Dictionary<int, LayerData>>> layerDatas = new List<List<Dictionary<int, LayerData>>>();

    public void Reset()
    {
        area?.Clear();
        blocks?.Clear();
        orderInLayers?.Clear();
        res?.Clear();
        layerDatas?.Clear();
    }
}

public class MapConfigManager
{
    private MapConfig _mapConfig;

    private string dataPath;

    private static MapConfigManager _instance;
    public string mapConfigName { get; set; }
    public string mapConfigSize { get; set; }

    public static MapConfigManager instance
    {
        get
        {
            if (_instance == null)
                _instance = new MapConfigManager();

            return _instance;
        }
    }

    public MapConfigManager()
    {
        _mapConfig = new MapConfig();
    }

    public MapConfig GetMapConfig()
    {
        if(_mapConfig != null)
            return _mapConfig;

        return null;
    }

    public string mapConfigPath
    {
        get
        {
            string path = Path.GetFullPath(FileUtil.mapConfigPathRoot + "/" + mapConfigName + ".json");
            return path;
        }
    }

    public string mapConfigServerPath
    {
        get
        {
            string path = Path.GetFullPath(FileUtil.mapConfigServerPathRoot + "/" + mapConfigName + ".json");
            return path;
        }
    }

    public bool Exists(string mapConfigName)
    {
        string path = Path.GetFullPath(FileUtil.mapConfigPathRoot + "/" + mapConfigName + ".json");
        bool exist = File.Exists(path);
        return exist;
    }

    public void RecoveryMapConfig()
    {

        for (int x = 0; x < _mapConfig.colCount; x++)
        {
            for(int y = 0; y < _mapConfig.rowCount; y++)
            {
                HexCellData hexCellData = new HexCellData();
                hexCellData.cellType = (CellsType)_mapConfig.blocks[x][y];
                //hexCellData.orderInLayer = _mapConfig.orderInLayers[x][y];
                hexCellData.layerDatas = _mapConfig.layerDatas[x][y];
                hexCellData.layerDatas = _mapConfig.layerDatas[x][y];
                hexCellData.x = x;
                hexCellData.y = y;
                foreach(var res in _mapConfig.res[x][y])
                {
                    hexCellData.resTypeToResNames.Add((int)res.Key,res.Value);
                }
                

                HexCell hexCell = HexGrid.instance.GetCell(x, y);
                hexCell.SetHexCellData(hexCellData);
                hexCell.SetCellType(hexCellData.cellType);
                HexGrid.GridPos gridPos = new HexGrid.GridPos(x,y);
                if (hexCellData.cellType == CellsType.Obstacle)
                {
                    BrushManager.instance.obstaclePos.Add(gridPos);
                }else if(hexCellData.cellType == CellsType.Walkable)
                {
                    BrushManager.instance.walkablePos.Add(gridPos);
                }
               
                hexCell.color = Global.instance.GetColor(hexCellData.cellType);

                foreach (var value in _mapConfig.res[x][y])
                {
                    string[] ress = value.Value.Split(',');
                    hexCell.SetResName(ress[0], ress[1], (LayerType)value.Key);
                    if ((LayerType)value.Key == LayerType.fog)
                    {
                        BrushManager.instance.fogPos.Add(gridPos);
                    }
                }
            }
        }
    }

    public void LoadConfig()
    {
        if (!Exists(mapConfigName))
            return;

        if (string.IsNullOrEmpty(mapConfigPath))
            return;

        StreamReader streamReader = new StreamReader(mapConfigPath);
        if (streamReader == null)
            return;

        string json = streamReader.ReadToEnd();
        if (json.Length <= 0)
            return;

        _mapConfig = JsonConvert.DeserializeObject<MapConfig>(json);
        mapConfigName = _mapConfig.mapName;
        mapConfigSize = _mapConfig.colCount +"X"+ _mapConfig.rowCount;
        Global.instance.x = _mapConfig.x;
        Global.instance.y = _mapConfig.y;
        Global.instance.SetDiameter(_mapConfig.diameter);
        Global.instance.SetGridCount(_mapConfig.colCount, _mapConfig.rowCount);
        if(_mapConfig.area == null)
        {
            AreaManager.instance.areaDic = new Dictionary<int, Area>();
        }
        else
        {
            AreaManager.instance.areaDic = _mapConfig.area;
        }

        HexGrid.instance.CreateMapCell();
        RecoveryMapConfig();
        streamReader.Close();
        streamReader.Dispose();
        Event.Fire(Event.CONFIG_LOAD_COMPLETE);
        //Global.instance.loading.SetActive(false);
    }

    public void NewConfig()
    {
        if (Exists(mapConfigName))
            return;

        if (string.IsNullOrEmpty(mapConfigPath))
            return;

        SaveConfig(false);
    }

    public void SaveConfig(bool showTips = true)
    {
        if (string.IsNullOrEmpty(mapConfigPath))
            return;

        if (string.IsNullOrEmpty(mapConfigName))
        {
            Global.instance.systemTipsUI.AddSystemInfo("没有打开中的地图!");
            return;
        }

        _mapConfig.Reset();
        _mapConfig.mapName = mapConfigName;
        _mapConfig.diameter = Global.instance.diameter;
        _mapConfig.colCount = Global.instance.lineCount;
        _mapConfig.rowCount = Global.instance.rowCount;
        _mapConfig.x = Global.instance.x;
        _mapConfig.y = Global.instance.y;
        _mapConfig.area = AreaManager.instance.areaDic;

        for (int x = 0; x < _mapConfig.colCount; x++)
        {
            short[] blocks = new short[_mapConfig.rowCount];
            //int[] orderInLayers = new int[_mapConfig.rowCount];
            List<Dictionary<int, string>> res = new List<Dictionary<int, string>>();
            List<Dictionary<int, LayerData>> layerDatas = new List<Dictionary<int, LayerData>>();

            for (int y = 0; y < _mapConfig.rowCount; y++)
            {
                HexCell hexCell = HexGrid.instance.GetCell(x, y);
                blocks[y] = (short)hexCell._hexCellData.cellType;
                //orderInLayers[y] = (int)hexCell._hexCellData.orderInLayer;
                res.Add(hexCell._hexCellData.resTypeToResNames);
                layerDatas.Add(hexCell._hexCellData.layerDatas);
                hexCell._hexCellData.x = x;
                hexCell._hexCellData.y = y;
            }

            _mapConfig.blocks.Add(blocks);
            //if (_mapConfig.orderInLayers == null)
            //    _mapConfig.orderInLayers = new List<int[]>();

            //_mapConfig.orderInLayers.Add(orderInLayers);
            _mapConfig.res.Add(res);
            _mapConfig.layerDatas.Add(layerDatas);
        }

        WriteConfig();
        WriteServerConfig();
    }

    public void WriteConfig(bool showTips = true)
    {
        StreamWriter streamWriter = null;
        try
        {
            streamWriter = new StreamWriter(mapConfigPath);
            if (streamWriter == null)
                return;

            var settings = new JsonSerializerSettings();
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            string json = JsonConvert.SerializeObject(_mapConfig, settings);
            streamWriter.WriteLine(json);

            if (showTips)
                Global.instance.tipsUI.SetTips("提示", "地图数据保存成功", null);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            Global.instance.tipsUI.SetTips("提示", "地图数据保存失败,请联系相关人员,进行查看!", null);
        }
        finally
        {
            streamWriter.Close();
            streamWriter.Dispose();
        }
    }

    public void WriteServerConfig(bool showTips = true)
    {
        StreamWriter streamServerWriter = null;
        try
        {
            streamServerWriter = new StreamWriter(mapConfigServerPath);
            if (streamServerWriter == null)
                return;

            MapConfig mapConfig = new MapConfig();
            mapConfig.area = _mapConfig.area;
            mapConfig.blocks = _mapConfig.blocks;
            mapConfig.colCount = _mapConfig.colCount;
            mapConfig.rowCount = _mapConfig.rowCount;
            mapConfig.mapName = _mapConfig.mapName;
            mapConfig.diameter = _mapConfig.diameter;
            mapConfig.layerDatas = _mapConfig.layerDatas;
            mapConfig.x = Global.instance.x;
            mapConfig.y = Global.instance.y;
            mapConfig.orderInLayers = null;
            mapConfig.res = null;

            var serverSettings = new JsonSerializerSettings();
            serverSettings.NullValueHandling = NullValueHandling.Ignore;
            string serverjson = JsonConvert.SerializeObject(mapConfig, serverSettings);
            streamServerWriter.WriteLine(serverjson);

            if (showTips)
                Global.instance.tipsUI.SetTips("提示", "地图数据保存成功", null);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            Global.instance.tipsUI.SetTips("提示", "地图数据保存失败,请联系相关人员,进行查看!", null);
        }
        finally
        {
            streamServerWriter.Close();
            streamServerWriter.Dispose();
        }
    }


    public void Destroy()
    {
        _mapConfig.Reset();
    }

}