using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HexGrid;

public class MeshCombineManager
{
    private static MeshCombineManager instance;
    public static MeshCombineManager Instance
    {
        get
        {
            if (instance == null)
                instance = new MeshCombineManager();

            return instance;
        }
    }


    public void DrawImage(List<GridPos> gridPos,string name,int color)
    {
        Mesh mesh = CombineMesh(gridPos);

        GameObject go = GameObject.Find(name);
        MeshRenderer meshRenderer;
        MeshFilter meshFilter;
        if (go == null)
        {
            go = new GameObject(name);
            meshRenderer = go.AddComponent<MeshRenderer>();
            meshFilter = go.AddComponent<MeshFilter>();
            var ol = go.AddComponent<cakeslice.Outline>();
            ol.color = color;
        }
        else
        {
            meshRenderer = go.GetComponent<MeshRenderer>();
            meshFilter = go.GetComponent<MeshFilter>();
        }

        meshFilter.mesh = mesh;
        meshRenderer.sharedMaterial = Global.instance.areaMat;
        Color c = Global.instance.brushColors;
        c.a = 0.2f;
        meshRenderer.material.color = c;
    }


    private Mesh CombineMesh(List<GridPos> gridPos)
    {
        CombineInstance[] combines = new CombineInstance[gridPos.Count];
        for (int i = 0; i < gridPos.Count; i++)
        {
            GridPos pos = gridPos[i];
            HexCell hexCell = HexGrid.instance.GetCell(pos.x, pos.y);
            combines[i].mesh = hexCell.sharedMesh;
            combines[i].transform = hexCell.transform.localToWorldMatrix;
        }

        Mesh mesh = new Mesh();
        mesh.CombineMeshes(combines, true);
        return mesh;
    }
}
