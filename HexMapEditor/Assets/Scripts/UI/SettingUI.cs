using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingUI : MonoBehaviour
{
    private InputField unityPathInput;
    private InputField texturePackerInput;

    void Start()
    {
        transform.Find("BG/Title/Button").gameObject.GetComponent<Button>().onClick.AddListener(OnClose);

        unityPathInput = transform.Find("BG/Body/UnityPathInput").gameObject.GetComponent<InputField>();
        texturePackerInput = transform.Find("BG/Body/TexturePackerInput").gameObject.GetComponent<InputField>();

        transform.Find("BG/Body/Ok").gameObject.GetComponent<Button>().onClick.AddListener(OnSaveSetting);

        transform.Find("BG/Body/UnityPathInput/Select").gameObject.GetComponent<Button>().onClick.AddListener(OnSelectUnity);
        transform.Find("BG/Body/TexturePackerInput/Select").gameObject.GetComponent<Button>().onClick.AddListener(OnSelectTexture);
    }

    public void OnSelectUnity()
    {
        unityPathInput.text = FileUtil.OpenFilepath("��ѡ��Unity.exe·��:");
    }

    public void OnSelectTexture()
    {
        texturePackerInput.text = FileUtil.OpenFilepath("��ѡ��TexturePacker.exe·��:");
    }

    public void OnSaveSetting()
    {
        if (string.IsNullOrEmpty(unityPathInput.text) || string.IsNullOrEmpty(texturePackerInput.text))
        {
            Global.instance.tipsUI.SetTips("��ʾ", "·��������Ϊ��!", null);
            return;
        }

        FileUtil.unityExePath = unityPathInput.text;
        FileUtil.texturePackExePath = texturePackerInput.text;

        PlayerPrefs.SetString("unityExePath", unityPathInput.text);
        PlayerPrefs.SetString("texturePackExePath", texturePackerInput.text);
        PlayerPrefs.Save();

        Global.instance.systemTipsUI.AddSystemInfo("·������ɹ�!");
        OnClose();
    }

    public void OnClose()
    {
        ViewManager.instance.HideUI("SettingUI");
    }
}
