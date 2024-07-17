using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LayerWorldUI : MonoBehaviour
{
    private Canvas _canvas;
    private GameObject _templete;
    private Transform _content;

    void Awake()
    {
        _canvas = GetComponent<Canvas>();
        _canvas.renderMode = RenderMode.WorldSpace;
        _canvas.worldCamera = Camera.main;

        _templete = transform.Find("Panel/templete").gameObject;
        _content = transform.Find("Panel/LayerData/Viewport/Content");
    }

    public void UpdateScale()
    {
        transform.localScale = Vector3.one * (_canvas.worldCamera.orthographicSize / 10 + 0.5f);
    }

    public void UpdateLayerUI(HexCell hexCell)
    {
        //if (hexCell == null)
        //    return;

        //var layers = hexCell.layers;
        //for (int i = 0; i < _content.transform.childCount; i++)
        //{
        //    Destroy(_content.transform.GetChild(i).gameObject);
        //}

        //foreach (var value in layers)
        //{
        //    if (value.Key == LayerType.floor)
        //        continue;

        //    List<LayerData> layerData = hexCell.GetLayerData(value.Key);
        //    if (layerData == null 
        //        || string.IsNullOrEmpty(layerData.funcId.ToString()) 
        //        || string.IsNullOrEmpty(layerData.funcParams1))
        //    {
        //        continue;
        //    }

        //    GameObject templateGo = Instantiate(_templete);
        //    templateGo.SetActive(true);
        //    templateGo.transform.name = value.Key.ToString();
        //    templateGo.transform.parent = _content.transform;
        //    templateGo.transform.localRotation = Quaternion.identity;
        //    templateGo.transform.Find("title/layername").GetComponent<Text>().text = value.Key.ToString();
        //    Text funcId = templateGo.transform.Find("info/funcbg/funcid").GetComponent<Text>();
        //    Text funcParams1 = templateGo.transform.Find("info/parambg1/paramid").GetComponent<Text>();
        //    Text funcParams2 = templateGo.transform.Find("info/parambg2/paramid").GetComponent<Text>();

        //    if (layerData != null)
        //    {
        //        funcId.text = layerData.funcId.ToString();
        //        funcParams1.text = layerData.funcParams1;
        //        funcParams2.text = layerData.funcParams2;
        //    }
        //}
    }
}
