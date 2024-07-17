using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ImportUI : MonoBehaviour
{
    public GameObject _typeGo;
    public GameObject _typeParent;
    private string _layerType;

    void Start()
    {
        transform.Find("Main/BtnSelectFolder").GetComponent<Button>().onClick.AddListener(OnSelectFolder);
        transform.Find("Main/BtnClose").GetComponent<Button>().onClick.AddListener(OnClose);
        Event<string>.Register(Event.SELECT_FOLDER, SelectFolderCallBack);

        Create();
    }

    public void Create()
    {
        Destroy();
        foreach (var resType in Enum.GetNames(typeof(LayerType)))
        {
            GameObject go = Instantiate(_typeGo) as GameObject;
            go.transform.parent = _typeParent.transform;
            go.transform.localScale = Vector3.one;
            go.transform.name = resType;
            go.transform.Find("Label").GetComponent<Text>().text = resType;
            go.SetActive(true);

            go.GetComponent<Toggle>().onValueChanged.AddListener((isOn) =>
            {
                _layerType = resType;
            });
        }

        _typeParent.transform.Find(LayerType.map.ToString()).GetComponent<Toggle>().isOn = false;
        _typeParent.transform.Find(LayerType.map.ToString()).GetComponent<Toggle>().isOn = true;
    }

    public void SelectFolderCallBack(string folderPath)
    {
        Global.instance.maskUI.SetDesc("正在导入资源, 请稍等...");
        GameApp.CreateCoroutine(ImportRes(folderPath));
    }

    private IEnumerator ImportRes(string folderPath)
    {
        if (string.IsNullOrEmpty(folderPath))
        {
            Global.instance.maskUI.OnClose();
            yield break;
        }

        ViewManager.instance.HideUI(ViewDefine.IMPORT_UI);
        yield return new WaitForSeconds(0.2f);

        List<string> directories = FileUtil.GetDirectories(folderPath);
        foreach (string dir in directories)
        {
            string[] names = dir.Split('\\');
            string fileName = names[names.Length - 1];
            string folderName = names[names.Length - 2];
            List<string> filePath = FileUtil.CollectFilesByEnd(dir, ".png", ".jpg");

            if (filePath.Count == 0)
            {
                Global.instance.systemTipsUI.AddSystemInfo("空文件夹: " + dir);
                continue;
            }

            PackResManager.instance.SaveTextures(_layerType, folderName, fileName, dir);
        }

        Global.instance.maskUI.OnClose();
        Event<string>.Fire(Event.IMPORT_SUCCESS, _layerType);
        Global.instance.tipsUI.SetTips("导入资源", "资源导入完成");
    }

    private void Destroy()
    {
        for (int i = 0; i < _typeParent.transform.childCount; i++)
        {
            Destroy(_typeParent.transform.GetChild(i).gameObject);
        }
    }

    private void OnSelectFolder()
    {
        FileUtil.SelectFolder();
    }

    private void OnClose()
    {
        ViewManager.instance.HideUI("ImportUI");
    }

    private void OnDestroy()
    {
        Destroy();

    }
}
