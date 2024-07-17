using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class CreateAreaUI : MonoBehaviour
{

    private InputField areaNameInput;
    private InputField areaENameInput;
    
    void Start()
    {
        areaNameInput = transform.Find("Main/InputName").GetComponent<InputField>();
        areaENameInput = transform.Find("Main/InputPyName").GetComponent<InputField>();
        transform.Find("Main/BtnCreate").GetComponent<Button>().onClick.AddListener(OnSaveAreaType);
        transform.Find("Main/BtnClose").GetComponent<Button>().onClick.AddListener(OnClose);
    }

    public void OnSaveAreaType()
    {
        int areaType = 1;
        List<AreaType> areaTypeList = AreaManager.instance.GetAreaTypeList();
        if (areaTypeList != null)
        {
            areaType = areaTypeList.Count + 1;
        }

        AreaType area = new AreaType();
        area.areaType = areaType;
        area.areaName = areaNameInput.text;
        area.areaEName = areaENameInput.text;

        if (!string.IsNullOrEmpty(area.areaName) && !string.IsNullOrEmpty(area.areaEName))
        {
            AreaManager.instance.SaveAreaType(area);
        }
        // 如果文件夹存在 允许导出枚举
        if (Directory.Exists(FileUtil.areaRuleEnumClientPath))
            GenerateEnum();

        Event.Fire(Event.UPDATE_DROPDOWN);
        ViewManager.instance.HideUI("CreateAreaUI");
    }

    public static void GenerateEnum()
    {
        List<AreaType> areaTypeList = AreaManager.instance.GetAreaTypeList();
        if (areaTypeList == null)
            return;

        StringBuilder builder = new StringBuilder();
        //#region 导出服务器枚举
        //builder.Append("package global\n\n");
        //builder.Append("type EnumAreaType int\n\n");
        //builder.Append("const (\n");

        //AreaType areaType = null;
        //for (int i = 0; i < areaTypeList.Count; i++)
        //{
        //    areaType = areaTypeList[i];
        //    string line = "\tEART_" + areaType.areaEName + " EnumAreaType = " + areaType.areaType + " //" + areaType.areaName + "\n";
        //    builder.Append(line);
        //}
        //builder.Append(")");
        //File.WriteAllText(FileUtil.areaRuleEnumServerPath + "area_type_enum.go", builder.ToString());
        //#endregion

        #region 导出客户端枚举
        builder.Clear();
        builder.Append("-- 地图区域类型");
        builder.AppendLine();
        builder.Append("EmMapAreaType = \n{\n");
        for (int i = 0; i < areaTypeList.Count; i++)
        {
            string line = "\t" + areaTypeList[i].areaEName + " = " + areaTypeList[i].areaType + ",";
            builder.Append("\t-- " + areaTypeList[i].areaName);
            builder.AppendLine();
            builder.Append(line);
            builder.AppendLine();

            if (i < areaTypeList.Count - 1)
            {
                builder.AppendLine();
            }
        }
        builder.Append("}");
        File.WriteAllText(FileUtil.areaRuleEnumClientPath + "EmMapAreaType.lua", builder.ToString());
        #endregion
        Global.instance.tipsUI.SetTips("提示", "区域枚举导出成功\nEmMapAreaType.lua", null);
    }

    public void OnClose()
    {
        ViewManager.instance.HideUI("CreateAreaUI");
    }
}
