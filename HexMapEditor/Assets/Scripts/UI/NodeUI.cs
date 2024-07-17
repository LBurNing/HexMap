using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NodeUI : MonoBehaviour
{
    private GameObject add;
    private GameObject remove;
    private GameObject nodeRoot;
    private Text funcName;
    private List<GameObject> nodes;
    private List<RectTransform> lines;
    private float nodeCount = 0;
    private float depth = 0;

    public NodeUI nodeParentUI;
    public LayerInfoUI layerInfoUI { get; set; }
    public GameObject nodeTemplete { get; set; }
    public LayerData layerData { get; set; }
    public LayerType layerType { get; set; }

    private void Awake()
    {
        add = transform.Find("BtnGroup/BtnAdd").gameObject;
        remove = transform.Find("BtnGroup/BtnRemove").gameObject;

        nodeRoot = transform.Find("Node").gameObject;
        funcName = transform.Find("Text").GetComponent<Text>();
        lines = new List<RectTransform>()
        {
            transform.Find("LineLeft").GetComponent<RectTransform>(),
            transform.Find("LineRight").GetComponent<RectTransform>(),
        };

        BindBtnEvent();
        nodes = new List<GameObject>();
    }
    
    private void Start()
    {
    }

    private void Update()
    {
        if (layerData != null)
        {
            funcName.text = layerData.funcName;
            if (string.IsNullOrEmpty(layerData.funcName))
                funcName.text = "Œ¥≈‰÷√";
        }

        UpdateSpacing();
        UpdateLine();
    }

    private void UpdateLine()
    {
        for(int i = 0; i < lines.Count; i++)
        {
            lines[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < nodes.Count; i++)
        {
            GameObject node = nodes[i];
            RectTransform line = lines[i];
            Vector3 targetPos = node.transform.position;
            Vector3 curPos = transform.position;
            line.sizeDelta = new Vector2(4, Vector3.Distance(targetPos, curPos));

            double angle = Math.Atan2(targetPos.y - curPos.y, targetPos.x - curPos.x) * 180 / Math.PI;
            line.transform.rotation = Quaternion.Euler(0, 0, (float)angle + 270);
            line.gameObject.SetActive(true);
        }
    }

    private void GetNodeCount(List<LayerData> layers)
    {
        if (layers.Count == 0)
            nodeCount++;

        if (layers.Count == 2)
            depth++;

        foreach (var data in layers)
        {
            GetNodeCount(data.layerDatas);
        }
    }

    private void UpdateSpacing()
    {
        if (layerData == null)
            return;

        GetNodeCount(layerData.layerDatas);
        if (nodeCount < 3)
            nodeCount = 1;

        nodeCount = nodeCount + depth - 1;
        float spacing = 50f;
        nodeRoot.GetComponent<HorizontalLayoutGroup>().spacing = nodeCount * spacing;
        nodeCount = 0;
        depth = 0;
    }

    private void BindBtnEvent()
    {
        add.GetComponent<Button>().onClick.AddListener(AddNodeClick);
        remove.GetComponent<Button>().onClick.AddListener(RemoveNodeClick);
        gameObject.GetComponent<Button>().onClick.AddListener(NodeClick);
    }

    public void InitNode()
    {
        foreach(LayerData data in layerData.layerDatas)
        {
            data.parent = layerData;
            CreateNode(data);
        }
    }

    private void AddNodeClick()
    {
        if (layerData.layerDatas.Count >= 2)
            return;

        LayerData data = new LayerData();
        layerData.layerDatas.Add(data);
        data.parent = layerData;
        CreateNode(data);
    }

    private void CreateNode(LayerData data)
    {
        GameObject node = Instantiate(nodeTemplete);
        node.transform.SetParent(nodeRoot.transform);
        nodes.Add(node);

        NodeUI nodeUI = node.GetComponent<NodeUI>();
        nodeUI.nodeTemplete = nodeTemplete;
        nodeUI.layerData = data;
        nodeUI.layerType = layerType;
        nodeUI.layerInfoUI = layerInfoUI;
        nodeUI.nodeParentUI = this;
        nodeUI.InitNode();
    }

    private void RemoveNodeClick()
    {
        foreach(GameObject node in nodes)
        {
            Destroy(node.gameObject);
        }

        nodes.Clear();
        layerData.layerDatas.Clear();

        if (layerData.parent != null)
        {
            if (nodeParentUI != null)
                nodeParentUI.nodes.Remove(gameObject);

            GameObject.Destroy(gameObject);
            layerData.parent.layerDatas.Remove(layerData);
        }
    }

    private void NodeClick()
    {
        layerInfoUI.Init();
        layerInfoUI.layerData = layerData;
        layerInfoUI.UpdateLayerInfo(layerType);
    }
}
