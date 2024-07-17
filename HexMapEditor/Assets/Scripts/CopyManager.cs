using System.Collections.Generic;

public class CopyManager
{
    private static CopyManager _instance;

    private HexCell _cacheCell;

    public void Init()
    {
        Event<HexCell>.Register(Event.KEY_COPY,CopyCell);
        Event<HexCell>.Register(Event.KEY_PASTE, PasteCell);
    }

    public static CopyManager instance
    {
        get
        {
            if (_instance == null)
                _instance = new CopyManager();

            return _instance;
        }
    }

    public void CopyCell(HexCell cell)
    {
        _cacheCell = cell;
    }

    public void PasteCell(HexCell cell)
    {
        if (_cacheCell == null)
            return;

        Dictionary<int, LayerData> dict = new Dictionary<int, LayerData>(_cacheCell.hexCellData.layerDatas);

        cell.hexCellData.cellType = _cacheCell.hexCellData.cellType;
        cell.hexCellData.layerDatas = dict;
        cell.hexCellData.orderInLayer = _cacheCell.hexCellData.orderInLayer;

        foreach (var item in _cacheCell.hexCellData.resTypeToResNames)
        {
            string[] filename = item.Value.Split(',');
            cell.SetResName(filename[0], filename[1], (LayerType)item.Key);
        }

    }


}
