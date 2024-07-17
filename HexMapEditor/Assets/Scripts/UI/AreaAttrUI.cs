using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static HexGrid;

public class AreaAttrUI : MonoBehaviour
{
    public Button saveAreaDataBtn;

    private Text Title;

    private Toggle DeliveredTo;
    private InputField X;
    private InputField Y;

    private Toggle MusicToggle;
    private InputField MusicText;

    private Toggle SEToggle;
    private InputField SEText;

    private Toggle RegionToggle;
    private GameObject RegionView;
    private HorizontalLayoutGroup RegionContent;
    public ResTitle resTitle;

    private Toggle ChangeOfState;
    private Toggle Toggle_1; //可通行
    private Toggle Toggle_2; //障碍
    private Toggle Toggle_3; //特殊通行
    private Button BtnHelp; //特殊通行说明按钮

    private Toggle ToggleText;
    private InputField JuQingId;

    private List<int> areaRules = new List<int>();

    // Start is called before the first frame update
    void Start()
    {
        init();
        saveAreaDataBtn.onClick.AddListener(OnSaveAreaAttrBtn);
        UpdateAreaAttr();
    }

    public void init()
    {
        Title = transform.Find("Area/Head/Title").GetComponent<Text>();

        DeliveredTo = transform.Find("Area/Body/Method/Transmit/DeliveredTo").GetComponent<Toggle>();
        X = transform.Find("Area/Body/Method/Transmit/Coordinate/X").GetComponent<InputField>();
        Y = transform.Find("Area/Body/Method/Transmit/Coordinate/Y").GetComponent<InputField>();

        MusicToggle = transform.Find("Area/Body/Method/BGM/Music/MusicToggle").GetComponent<Toggle>();
        MusicText = transform.Find("Area/Body/Method/BGM/Music/MusicText").GetComponent<InputField>();

        SEToggle = transform.Find("Area/Body/Method/BGM/SoundEffect/SEToggle").GetComponent<Toggle>();
        SEText = transform.Find("Area/Body/Method/BGM/SoundEffect/SEText").GetComponent<InputField>();

        RegionToggle = transform.Find("Area/Body/Method/Region/RegionToggle").GetComponent<Toggle>();
        RegionView = transform.Find("Area/Body/Method/Region/RegionView/Viewport/Content").gameObject;
        RegionContent = transform.Find("Area/Body/Method/Region/RegionView/Viewport/Content").gameObject.GetComponent<HorizontalLayoutGroup>();

        ChangeOfState = transform.Find("Area/Body/Method/Pass/ChangeOfState").GetComponent<Toggle>();
        Toggle_1 = transform.Find("Area/Body/Method/Pass/StateGroup/Toggle_1").GetComponent<Toggle>();
        Toggle_2 = transform.Find("Area/Body/Method/Pass/StateGroup/Toggle_2").GetComponent<Toggle>();
        Toggle_3 = transform.Find("Area/Body/Method/Pass/StateGroup/Toggle_3").GetComponent<Toggle>();
        BtnHelp = transform.Find("Area/Body/Method/Pass/StateGroup/Help").GetComponent<Button>();

        ToggleText = transform.Find("Area/Body/Method/Dialogue/ToggleText").GetComponent<Toggle>();
        JuQingId = transform.Find("Area/Body/Method/Dialogue/JuQingId").GetComponent<InputField>();

        DeliveredTo.onValueChanged.AddListener((value) =>
        {
            if (value == true)
            {
                if (RegionToggle.isOn)
                {
                    Global.instance.tipsUI.SetTips("提示", "本勾选项与“改变区域”冲突，是否取消“改变区域”的选择？", ()=> 
                    {
                        RegionToggle.isOn = false;
                    },
                    () => {
                        DeliveredTo.isOn = false;
                        return;
                    });
                }
                X.readOnly = false;
                Y.readOnly = false;
            }
        });

        MusicToggle.onValueChanged.AddListener((value) => 
        {
            if (value == true)
            {
                MusicText.readOnly = false;
            }
            else
            {
                MusicText.readOnly = true;
            }
        });


        SEToggle.onValueChanged.AddListener((value) =>
        {
            if (value == true)
            {
                SEText.readOnly = false;
            }
            else
            {
                SEText.readOnly = true;
            }
        });

        RegionToggle.onValueChanged.AddListener((value) => 
        {
            if (value == true)
            {
                if (DeliveredTo.isOn)
                {
                    Global.instance.tipsUI.SetTips("提示", "本勾选项与“传送”冲突，是否取消“传送”的选择？", () =>
                    {
                        DeliveredTo.isOn = false;
                        X.text = "";
                        Y.text = "";
                        X.readOnly = true;
                        Y.readOnly = true;
                    },
                    ()=> {
                        RegionToggle.isOn = false;
                        return;
                    });
                }

                    var resType = ResManager.instance.GetRess();
                if (resType == null)
                    return;

                //渲染贴图
                foreach (var rt in resType)
                {
                    foreach (var res in rt.Value)
                    {
                        ResTitle tempResTitle = Instantiate<ResTitle>(resTitle);
                        tempResTitle.transform.SetParent(RegionContent.transform);
                        tempResTitle.transform.name = res._resName;
                        tempResTitle.GetComponent<ResTitle>().SetTexture(res);
                        tempResTitle.sharedMesh = CreateMesh();
                        tempResTitle.GetComponent<RectTransform>().sizeDelta = new Vector2(70,70);
                    }
                }
                BrushManager.instance.bArea = true;
            }
            else
            {
                for (int i = 0; i < RegionView.transform.childCount; i++)
                {
                    Destroy(RegionView.transform.GetChild(i).gameObject);
                }
                BrushManager.instance.bArea = false;
            }
        });


        ChangeOfState.onValueChanged.AddListener((value) =>
        {
            if (value == true)
            {
                Toggle_1.interactable = true;
                Toggle_2.interactable = true;
                Toggle_3.interactable = true;

            }
            else
            {
                Toggle_1.interactable = false;
                Toggle_2.interactable = false;
                Toggle_3.interactable = false;
            }
        });

        BtnHelp.onClick.AddListener(()=> 
        {
            Global.instance.tipsUI.SetTips("帮助","玩家需要变化形态（挂载Buff）后才可通行，例如乘船行于水面地图", null);
        });

        ToggleText.onValueChanged.AddListener((value)=> 
        { 
            if(value == true)
            {
                JuQingId.readOnly = false;
            }
            else
            {
                JuQingId.readOnly = true;
            }
        });
    }

    public void UpdateAreaAttr()
    {
        if (AreaManager.instance.drawAreaing != null)
        {
            int areaType = AreaManager.instance.drawAreaing.areaType;
            string areaName = AreaManager.instance.drawAreaing.areaName;
            Title.text = string.Format("({0}){1}", areaType, areaName);

            AreaAttr areaAttr = AreaManager.instance.drawAreaing.areaAttr;
            if (areaAttr != null)
            {
                if (areaAttr.x != 0 && areaAttr.y != 0)
                {
                    DeliveredTo.isOn = true;
                    X.text = areaAttr.x.ToString();
                    Y.text = areaAttr.y.ToString();
                }
                if (!string.IsNullOrEmpty(areaAttr.musicName))
                {
                    MusicToggle.isOn = true;
                    MusicText.text = areaAttr.musicName;
                }
                if (!string.IsNullOrEmpty(areaAttr.soundEffectName))
                {
                    SEToggle.isOn = true;
                    SEText.text = areaAttr.soundEffectName;
                }
                if (!string.IsNullOrEmpty(areaAttr.juQingId))
                {
                    ToggleText.isOn = true;
                    JuQingId.text = areaAttr.juQingId;
                }

                if (areaAttr.walkable == 0 || areaAttr.obstacle == 0 || areaAttr.specialPassage == 0)
                {
                    ChangeOfState.isOn = true;
                    Toggle_1.isOn = areaAttr.walkable == 0 ? true : false;
                    Toggle_2.isOn = areaAttr.obstacle == 0 ? true : false;
                    Toggle_3.isOn = areaAttr.specialPassage == 0 ? true : false;
                }

                if (!string.IsNullOrEmpty(areaAttr.resName))
                {
                    GameObject obj = RegionView.transform.Find(areaAttr.resName).gameObject;
                    BrushManager.instance.areaResObj = obj;
                    obj.AddComponent<Outline>();
                    obj.GetComponent<Outline>().effectColor = Color.green;
                    obj.GetComponent<Outline>().effectDistance = new Vector2(3, 3);
                }

            }
        }
    }

    //保存区域属性
    public void OnSaveAreaAttrBtn()
    {
        AreaAttr areaAttr = new AreaAttr();
        if (DeliveredTo.isOn)
        {
            if (!string.IsNullOrEmpty(X.text) && !string.IsNullOrEmpty(Y.text))
            {
                int x = int.Parse(X.text);
                int y = int.Parse(Y.text);
                HexCell hc = HexGrid.instance.GetCell(x, y);
                if (hc == null)
                {
                    Global.instance.tipsUI.SetTips("提示", "所填写坐标值不符合地图尺寸，请检查!", null);
                    return;
                }
                else if (hc.hexCellData.cellType.Equals(CellsType.Obstacle))
                {
                    Global.instance.tipsUI.SetTips("提示", "所填写坐标类型为“障碍”，请更换坐标或更改该地格类型为“可通行”", null);
                    return;
                }
                //else if (!hc.hexCellData.cellType.Equals(CellsType.SpecialPassage))
                //{
                //    Global.instance.tipsUI.SetTips("提示", "所填写坐标类型与当前笔刷区域类型不同，请更换坐标或该地格类型!(当前坐标类型为“特殊通行”)", null);
                //    return;
                //}
                areaAttr.x = x;
                areaAttr.y = y;
            }
            else
            {
                Global.instance.systemTipsUI.AddSystemInfo("传送目标地格未填写");
                return;
            }
        }

        if (MusicToggle.isOn)
        {
            if (!string.IsNullOrEmpty(MusicText.text))
            {
                areaAttr.musicName = MusicText.text;
            }
            else
            {
                Global.instance.systemTipsUI.AddSystemInfo("音乐资源名未填写");
                return;
            }
        }

        if (SEToggle.isOn)
        {
            if (!string.IsNullOrEmpty(SEText.text))
            {
                areaAttr.soundEffectName = SEText.text;
            }
            else
            {
                Global.instance.systemTipsUI.AddSystemInfo("音效资源名未填写");
                return;
            }
        }

        if (RegionToggle.isOn)
        {
            areaAttr.resName = BrushManager.instance.areaResObj.name;
        }

        if (ChangeOfState.isOn)
        {
            if (Toggle_1.isOn || Toggle_2.isOn || Toggle_3.isOn)
            {
                areaAttr.walkable = Toggle_1.isOn ? 0 : 1;
                areaAttr.obstacle = Toggle_2.isOn ? 0 : 1;
                areaAttr.specialPassage = Toggle_3.isOn ? 0 : 1;
            }
            else
            {
                Global.instance.systemTipsUI.AddSystemInfo("未选择通行状态");
                return;
            }
        }

        if (ToggleText.isOn)
        {
            if (!string.IsNullOrEmpty(JuQingId.text))
            {
                areaAttr.juQingId = JuQingId.text;
            }
            else
            {
                Global.instance.systemTipsUI.AddSystemInfo("剧情对话组ID未填写");
                return;
            }

        }

        AreaManager.instance.SetAreaDic(areaAttr);
        AreaManager.instance.Reset();
        AreaManager.instance.areaId = 0;
        Event.Fire(Event.UPDATE_MAP_AREA);
        ViewManager.instance.HideUI("AreaAttrUI");

        string areaName = string.Format("{0}_{1}", AreaManager.instance.selectAreaType.areaEName, AreaManager.instance.selectAreaType.areaType);
        GameObject go = GameObject.Find(areaName);
        if (go != null)
            Destroy(go);

        Global.instance.systemTipsUI.AddSystemInfo("格子区域数据保存成功!");
        BrushManager.instance.bArea = false;

    }

    private Mesh CreateMesh()
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

}
