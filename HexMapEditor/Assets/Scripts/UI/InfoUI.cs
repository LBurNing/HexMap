using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoUI : MonoBehaviour
{
    private GameObject layerInfoTemplate;
    private GameObject layerInfoContent;
    private GameObject mapMethod;

    private void Awake()
    {
        Event<HexCell>.Register(Event.MOUSE_CLICK, MouseClickEvent);

        mapMethod = transform.Find("Body/Method").gameObject;
        layerInfoTemplate = transform.Find("Body/Template").gameObject;
        layerInfoContent = transform.Find("Body/Scroll View/Viewport/Content").gameObject;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MouseClickEvent(HexCell cell)
    {
        for (int i = 0; i < layerInfoContent.transform.childCount; i++)
        {
            Destroy(layerInfoContent.transform.GetChild(i).gameObject);
        }
        mapMethod.transform.Find("M1/Button").gameObject.GetComponent<Image>().color = new Color(0.44f, 0.45f, 0.45f);
        mapMethod.transform.Find("M2/Button").gameObject.GetComponent<Image>().color = new Color(0.44f, 0.45f, 0.45f);
        mapMethod.transform.Find("M3/Button").gameObject.GetComponent<Image>().color = new Color(0.44f, 0.45f, 0.45f);

        foreach (var item in cell._hexCellData.resTypeToResNames)
        {
            GameObject templateGo = Instantiate(layerInfoTemplate);
            templateGo.SetActive(true);

            string[] info = item.Value.Split(',');
            templateGo.transform.name = info[1];
            templateGo.transform.SetParent(layerInfoContent.transform);
            templateGo.transform.Find("D1").GetComponent<Text>().text = item.Key.ToString();
            templateGo.transform.Find("D2").GetComponent<Text>().text = info[0];
            templateGo.transform.Find("D3").GetComponent<Text>().text = info[1];
            templateGo.transform.Find("D4").GetComponent<Button>().onClick.AddListener(() =>
            {
                cell.RemoveResName((LayerType)item.Key);
                Destroy(cell.transform.Find(info[1]).gameObject);
                Event<HexCell>.Fire(Event.MOUSE_CLICK, cell);
            });
            templateGo.transform.localScale = Vector3.one;

            if (item.Key == 1)
            {
                mapMethod.transform.Find("M1/Button").gameObject.GetComponent<Image>().color = Color.white;
            }
            if (item.Key == 3)
            {
                mapMethod.transform.Find("M3/Button").gameObject.GetComponent<Image>().color = Color.white;
            }
            if (item.Key == 4)
            {
                mapMethod.transform.Find("M2/Button").gameObject.GetComponent<Image>().color = Color.white;
            }

        }


        if (cell._hexCellData.layerDatas.Count > 0)
        {
            mapMethod.transform.Find("M4/Button").gameObject.GetComponent<Image>().color = Color.white;
        }
        else
        {
            mapMethod.transform.Find("M4/Button").gameObject.GetComponent<Image>().color = new Color(0.44f, 0.45f, 0.45f);
        }
    }

    public void OnDestroy()
    {
        Event<HexCell>.UnRegister(Event.MOUSE_CLICK, MouseClickEvent);
    }
}
