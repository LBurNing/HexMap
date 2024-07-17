using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexGrid 
{
    public class GridPos
    {
        public int x;
        public int y;

        public GridPos(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    private static HexGrid _instance;
    public static HexGrid instance
    {
        get
        {
            if (_instance == null)
                _instance = new HexGrid();

            return _instance;
        }
    }
 
    private Vector3 _offset;
	public Dictionary<int, Dictionary<int, HexCell>> _cells;
    private List<OBBRect> _oBBRects;
    private GameObject _gridParant;
    private GameObject _cellParent;
    private string _gridParentName = "Grid Parent";


    public HexGrid()
    {
        _cells = new Dictionary<int, Dictionary<int, HexCell>>();
        _oBBRects = new List<OBBRect>();

        InitGridParent();
        LoadPerfab();
    }

    public void InitGridParent()
    {
        _gridParant = GameObject.Find(_gridParentName);
        if (_gridParant == null)
            _gridParant = new GameObject(_gridParentName);

        _gridParant.transform.position = Vector3.zero;
        _gridParant.transform.rotation = Quaternion.identity;
        _gridParant.transform.localScale = Vector3.one;
    }

    private void LoadPerfab()
    {
        _cellParent = Resources.Load<GameObject>("UIPrefabs/Hex Cell");
    }

    public void CreateMapCell()
    {
        int x = Mathf.RoundToInt(Global.instance.lineCount * 2 * Global.instance.sqrt);
        int y = Mathf.RoundToInt(Global.instance.rowCount * 2 * Global.instance.sqrt);

        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                Vector3 pos = new Vector3(i * Global.instance.outerRadius, j * Global.instance.innerRadius, 0) + new Vector3(_offset.x, _offset.y, 0);
                Vector2Int gridPos = Global.instance.GetGridPos(pos);
                if (gridPos.x >= Global.instance.lineCount || gridPos.y >= Global.instance.rowCount)
                    continue;

                if (gridPos.x < 0 || gridPos.y < 0)
                    continue;

                Dictionary<int, HexCell> hexCells;
                if (!_cells.TryGetValue(gridPos.x, out hexCells))
                {
                    hexCells = new Dictionary<int, HexCell>();
                    _cells[gridPos.x] = hexCells;
                }

                HexCell cell;
                if (!hexCells.TryGetValue(gridPos.y, out cell))
                {
                    cell = CreateCell(gridPos.x, gridPos.y);
                    hexCells[gridPos.y] = cell;
                }

                cell.coordinates = new HexCoordinates(gridPos.x, gridPos.y);
            }
        }

        Event.Fire(Event.CREATE_CELL_FINISH);
    }

    public void CallFunction(CellFunction function)
    {
        int x = Global.instance.lineCount;
        int y = Global.instance.rowCount;

        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                HexCell hexCell = GetCell(i, j);
                hexCell?.Function(function);
            }
        }
    }

    public HexCell GetCell(int x, int y)
    {
        Dictionary<int, HexCell> hexCells;
        if (!_cells.TryGetValue(x, out hexCells))
            return null;

        HexCell cell;
        if (!hexCells.TryGetValue(y, out cell))
            return null;

        return cell;
    }

    public HexCell GetCell(Vector3 position)
    {
        Vector2Int gridPos = Global.instance.GetGridPos(position);
        HexCell cell = GetCell(gridPos.x, gridPos.y);
        return cell;
    }

    public HexCell CreateCell (int x, int y) 
	{
		Vector3 position;
		position.x = (x + y * 0.5f - y / 2) * (Global.instance.innerRadius * 2f);
		position.y = y * (Global.instance.outerRadius * 1.5f);
		position.z = 0;
        
        HexCell hexCell = CreateCell();
        _cells[x][y] = hexCell;

        hexCell.transform.SetParent(_gridParant.transform, false);
        hexCell.transform.localPosition = position - new Vector3(_offset.x, _offset.y, 0);
        hexCell.coordinates = HexCoordinates.FromOffsetCoordinates(x, y);
        hexCell.transform.name = string.Format("Grid Pos, x:{0}, y:{1}", x, y);

        return hexCell;
    }

    public HexCell CreateCell()
    {
        GameObject go = GameObject.Instantiate(_cellParent);
        HexCell hexCell = go.GetComponent<HexCell>();
        hexCell.transform.rotation = Quaternion.Euler(-90, 0, 0);
        Mesh mesh = CreateMesh(hexCell);
        hexCell.sharedMesh = mesh;

        OBBRect obbRect  = new OBBRect(go.transform);
        _oBBRects.Add(obbRect);
        return hexCell;
    }

    public List<Transform> GetGridPosList(Transform transform)
    {
        List<Transform> trans = new List<Transform>();
        OBBRect obb = new OBBRect(transform);

        foreach (OBBRect value in _oBBRects)
        {
            if (value.intersects(obb))
            {
                trans.Add(value.m_Transform);
            }
        }

        return trans;
    }

	private Mesh CreateMesh(HexCell cell)
    {
		Mesh mesh = new Mesh();
		Vector3 center = Vector3.zero;
		List<Vector3> vertices = new List<Vector3>();
		List<int> triangles = new List<int>();

        for (int i = 0; i < 6; i++)
        {
			Vector3 v1 = center;
			Vector3 v2 = center + Global.instance.corners[i];
			Vector3 v3 = center + Global.instance.corners[i + 1];
       
            int vertexIndex = vertices.Count;
            vertices.Add(v1);
            vertices.Add(v2);
            vertices.Add(v3);
            triangles.Add(vertexIndex);
            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 2);
        }

        Vector2[] uvs = new Vector2[vertices.Count];
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = Vector3.Normalize(vertices[i]);
        }

        mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();

        //重新计算法线
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        mesh.RecalculateBounds();
        return mesh;
    }

    public void Destroy()
    {
        foreach(var value in _cells)
        {
            foreach (var value1 in value.Value)
            {
                value1.Value.Destroy();
            }
        }

        _cells.Clear();
    }
}