using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

public class PackResManager
{
    private static PackResManager _instance;
    public static string _buildAssetBundleCmd = "-batchmode -quit -executeMethod {0}";
    private static string BUILD_MAP = "BuildAssetBundle.BuildMap";
    private static string tpCmd = "--sheet {0}.png --data {1}.json --format json-array --force-squared  --trim-mode CropKeepPos --pack-mode Best  --algorithm MaxRects --max-size 4096 --size-constraints POT  --disable-rotation --scale 1 {2}";

    public static PackResManager instance
    {
        get
        {
            if (_instance == null)
                _instance = new PackResManager();

            return _instance;
        }
    }

    public PackResManager()
    {
    }

    public void SaveTextures(string type, string folderName, string fileName, string textureSourcePath)
    {
        string outPath = string.Format("{0}/{1}/{2}/{3}/", FileUtil.resPath, type, folderName, fileName);
        if (!Directory.Exists(outPath))
            Directory.CreateDirectory(outPath);

        string texPath = string.Format("{0}/{1}", outPath, fileName + Global.TEX_NAME);
        string textPath = string.Format("{0}/{1}", outPath, fileName + Global.TEXT_NAME);
        ProcessCommand(FileUtil.texturePackExePath, string.Format(tpCmd, texPath, textPath, textureSourcePath));

    }

    public void BuildMap()
    {
        Build(BUILD_MAP);
    }

    private void Build(string func)
    {
        string abCmd = string.Format(_buildAssetBundleCmd, func);
        abCmd += " -projectPath " + Path.GetFullPath(FileUtil.projectPath);
        abCmd += " -logFile " + Path.GetFullPath(FileUtil.logFile);
        abCmd += " -resPath " + FileUtil.resPath;

        ProcessCommand(FileUtil.unityExePath, abCmd);
    }

    public static void ProcessCommand(string command, string argument = "", bool shell = false)
    {
        Event.Fire(Event.COMMAND_START);
        System.Diagnostics.ProcessStartInfo start = new System.Diagnostics.ProcessStartInfo(command);
        start.Arguments = argument;
        start.CreateNoWindow = false;
        start.ErrorDialog = true;
        start.UseShellExecute = shell;

        if (shell)
        {
            start.RedirectStandardOutput = false;
            start.RedirectStandardError = false;
            start.RedirectStandardInput = false;
        }
        else
        {
            start.RedirectStandardOutput = true;
            start.RedirectStandardError = true;
            start.RedirectStandardInput = true;
            start.StandardOutputEncoding = System.Text.UTF8Encoding.UTF8;
            start.StandardErrorEncoding = System.Text.UTF8Encoding.UTF8;
        }

        System.Diagnostics.Process p = System.Diagnostics.Process.Start(start);
        string result = p.StandardOutput.ReadToEnd();

        p.WaitForExit();
        p.Close();
        Event.Fire(Event.COMMAND_COMPLETED);
    }
}
