using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public class FileUtil
{
    public static string dataPath = Application.dataPath.Replace("\\", "/");
    public static string toolRootPath = dataPath + "/../../";

#if UNITY_EDITOR
    public static string projectPath = toolRootPath + "BuildProject/";
    public static string areaJsonPath = dataPath + "/../AreaType.json";
    public static string resPath = toolRootPath + "Product/resources/";
    public static string resABPath = toolRootPath + "Product/assetbundle/";
    public static string mapConfigPathRoot = toolRootPath + "Product/settings/client/map/";
    public static string mapConfigServerPathRoot = toolRootPath + "Product/settings/server/map/";
    public static string areaRuleEnumClientPath = toolRootPath + "Product/enums/client/";
    public static string areaRuleEnumServerPath = toolRootPath + "Product/enums/server";
    public static string xlsPath = toolRootPath + "Product/xls/";
#else
    public static string projectPath = toolRootPath + "BuildProject/";
    public static string areaJsonPath = dataPath + "/../AreaType.json";
    public static string resPath = toolRootPath + "Product/resources/";
    public static string resABPath = toolRootPath + "Product/assetbundle/";
    public static string mapConfigPathRoot = toolRootPath + "Product/settings/client/map/";
    public static string mapConfigServerPathRoot = toolRootPath + "Product/settings/server/map/";
    public static string areaRuleEnumClientPath = toolRootPath + "Product/enums/client/";
    public static string areaRuleEnumServerPath = toolRootPath + "Product/enums/server";
    public static string xlsPath = toolRootPath + "Product/xls/";
#endif

    private static string _unityExePath = "";
    private static string _texturePackExePath = toolRootPath + "TexturePacker/TexturePacker.exe";
    private static string _logFile = projectPath + "/unity_log.log";


    public static string logFile
    {
        get { return _logFile; }
        set { _logFile = value; }
    }

    public static string texturePackExePath
    {
        get { return _texturePackExePath; }
        set { _texturePackExePath = value; }
    }

    public static string unityExePath
    {
        get{ return _unityExePath; }
        set { _unityExePath = value; }
    }

    /// <summary>
    /// 从外部指定文件中加载图片
    /// </summary>
    /// <returns></returns>
    public static Texture2D LoadTextureByIO(string path)
    {
        FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
        fs.Seek(0, SeekOrigin.Begin);//游标的操作，可有可无
        byte[] bytes = new byte[fs.Length];

        try
        {
            fs.Read(bytes, 0, bytes.Length);

        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        fs.Close();

        int width = 512;
        int height = 512;
        Texture2D texture = new Texture2D(width, height);

        if (texture.LoadImage(bytes))
        {
            return texture;

        }
        else
        {
            return null;
        }
    }

    public static void CopyFile(string sourceFileName, string destFileName)
    {
        if (!File.Exists(sourceFileName))
            return;

        if (File.Exists(destFileName))
            return;

        File.Copy(sourceFileName, destFileName, true);
    }

    private void CopyDir(string srcPath, string aimPath)
    {
        try
        {
            // 检查目标目录是否以目录分割字符结束如果不是则添加之
            if (aimPath[aimPath.Length - 1] != System.IO.Path.DirectorySeparatorChar)
            {
                aimPath += System.IO.Path.DirectorySeparatorChar;
            }

            // 判断目标目录是否存在如果不存在则新建之
            if (!System.IO.Directory.Exists(aimPath))
            {
                System.IO.Directory.CreateDirectory(aimPath);
            }

            // 得到源目录的文件列表，该里面是包含文件以及目录路径的一个数组
            // 如果你指向copy目标文件下面的文件而不包含目录请使用下面的方法
            // string[] fileList = Directory.GetFiles(srcPath);
            string[] fileList = System.IO.Directory.GetFileSystemEntries(srcPath);

            // 遍历所有的文件和目录
            foreach (string file in fileList)
            {
                // 先当作目录处理如果存在这个目录就递归Copy该目录下面的文件
                if (System.IO.Directory.Exists(file))
                {
                    CopyDir(file, aimPath + System.IO.Path.GetFileName(file));
                }

                // 否则直接Copy文件
                else
                {
                    CopyFile(file, aimPath + System.IO.Path.GetFileName(file));
                }
            }
        }
        catch (Exception e)
        {
            throw;
        }
    }

    public static List<string> GetDirectories(string path)
    {
        List<string> dcirectories = new List<string>(Directory.GetDirectories(path));
        return dcirectories;
    }

    public static List<string> CollectFilesByEnd(string directory, params string[] ends)
    {
        if (!Directory.Exists(directory))
        {
            Debug.LogError("CollectAllFile error, directory not exist:" + directory);
            return null;
        }

        if (ends == null || ends.Length < 1)
        {
            Debug.LogError("CollectAllFile error, ends == null || ends.Length < 1:" + directory);
            return null;
        }

        string[] paths = Directory.GetFiles(directory, "*", SearchOption.AllDirectories);
        List<string> files = new List<string>();

        foreach (string path in paths)
        {
            string p = path.ToLower();
            foreach (var extension in ends)
            {
                if (p.EndsWith(extension.ToLower()))
                {
                    files.Add(path);
                    break;
                }
            }
        }

        return files;
    }

    public static string BrowseFile(string path, string title, string filter, string defExt)
    {
        FileOpenDialog dialog = new FileOpenDialog();
        dialog.structSize = Marshal.SizeOf(dialog);
        dialog.title = title;
        dialog.initialDir = path;
        dialog.filter = filter;
        dialog.defExt = defExt;
        dialog.file = new string(new char[256]);
        dialog.maxFile = dialog.file.Length;
        dialog.fileTitle = new string(new char[64]);
        dialog.maxFileTitle = dialog.fileTitle.Length;
        //OFN_EXPLORER|OFN_FILEMUSTEXIST|OFN_PATHMUSTEXIST| OFN_ALLOWMULTISELECT|OFN_NOCHANGEDIR
        //of.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;
        dialog.flags = 0x00000008; //只能单选
        if (DialogShow.GetOFN(dialog))
        {
            return dialog.file;
        }

        return null;
    }

    public static string OpenMapFilepath()
    {
        FileOpenDialog dialog = new FileOpenDialog();
        dialog.structSize = Marshal.SizeOf(dialog);
        dialog.title = "选择要打开的文件:";
        dialog.initialDir = Path.GetFullPath(mapConfigPathRoot);
        //dialog.filter = "scene files\0*.scene\0All Files\0*.*\0\0";
        dialog.file = new string(new char[256]);
        dialog.maxFile = dialog.file.Length;
        dialog.fileTitle = new string(new char[64]);
        dialog.maxFileTitle = dialog.fileTitle.Length;

        dialog.flags = 0x00000800; //只能单选
        if (DialogShow.GetOpenFileName(dialog))
        {
            string fileName = Path.GetFileNameWithoutExtension(dialog.file);
            return fileName;
        }
        return "";
    }

    public static string OpenFilepath(string title)
    {
        FileOpenDialog dialog = new FileOpenDialog();
        dialog.structSize = Marshal.SizeOf(dialog);
        dialog.title = title;
        dialog.file = new string(new char[256]);
        dialog.filter = "files\0*.exe";
        dialog.maxFile = dialog.file.Length;
        dialog.fileTitle = new string(new char[64]);
        dialog.maxFileTitle = dialog.fileTitle.Length;

        dialog.flags = 0x00000800; //只能单选

        if (DialogShow.GetOpenFileName(dialog))
        {
            return dialog.file;
        }
        return "";
    }

    public static void SelectFolder()
    {
        string path = DialogShow.GetPathFromWindowsExplorer("");
        Event<string>.Fire(Event.SELECT_FOLDER, path);
    }

    public static void OpenMapConfigFile()
    {
        FileOpenDialog dialog = new FileOpenDialog();
        dialog.structSize = Marshal.SizeOf(dialog);
        dialog.title = "选择要打开的文件:";
        dialog.initialDir = Path.GetFullPath(mapConfigPathRoot);
        dialog.filter = "scene files\0*.json\0All Files\0*.*\0\0";
        dialog.file = new string(new char[256]);
        dialog.maxFile = dialog.file.Length;
        dialog.fileTitle = new string(new char[64]);
        dialog.maxFileTitle = dialog.fileTitle.Length;
        dialog.defExt = "scene";

        dialog.flags = 0x00000008; //只能单选
        if (DialogShow.GetOpenFileName(dialog))
        {
            #region 数据重置
            Global.instance.Reset();
            HexGrid.instance.Destroy();
            MapConfigManager.instance.Destroy();
            BrushManager.instance.Destroy();
            BrushManager.instance.ClearList();
            ResBrushManager.instance.Destroy();
            AreaBrushManager.instance.Destroy();
            #endregion

            string fileName = Path.GetFileNameWithoutExtension(dialog.file);
            MapConfigManager.instance.mapConfigName = fileName;
            MapConfigManager.instance.LoadConfig();

            Event.Fire(Event.OPEN_MAP_CONFIG_FILE);
        }
    }

}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
public class FileOpenDialog
{
    public int structSize = 0;
    public IntPtr dlgOwner = IntPtr.Zero;
    public IntPtr instance = IntPtr.Zero;
    public String filter = null;
    public String customFilter = null;
    public int maxCustFilter = 0;
    public int filterIndex = 0;
    public String file = null;
    public int maxFile = 0;
    public String fileTitle = null;
    public int maxFileTitle = 0;
    public String initialDir = null;
    public String title = null;
    public int flags = 0;
    public short fileOffset = 0;
    public short fileExtension = 0;
    public String defExt = null;
    public IntPtr custData = IntPtr.Zero;
    public IntPtr hook = IntPtr.Zero;
    public String templateName = null;
    public IntPtr reservedPtr = IntPtr.Zero;
    public int reservedInt = 0;
    public int flagsEx = 0;
}


[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
public class OpenDialogDir
{
    public IntPtr hwndOwner = IntPtr.Zero;
    public IntPtr pidlRoot = IntPtr.Zero;
    public String pszDisplayName = null;
    public String lpszTitle = null;
    public UInt32 ulFlags = 0;
    public IntPtr lpfn = IntPtr.Zero;
    public IntPtr lParam = IntPtr.Zero;
    public int iImage = 0;
}

public class DialogShow
{
    [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    public static extern bool GetOpenFileName([In, Out] FileOpenDialog dialog);  //这个方法名称必须为GetOpenFileName
    public static bool GetOFN([In, Out] FileOpenDialog ofn)
    {
        return GetOpenFileName(ofn);
    }

    [DllImport("shell32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    public static extern IntPtr SHBrowseForFolder([In, Out] OpenDialogDir ofn);

    [DllImport("shell32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    public static extern bool SHGetPathFromIDList([In] IntPtr pidl, [In, Out] char[] fileName);

    [DllImport("shell32.dll", ExactSpelling = true)]
    public static extern int SHOpenFolderAndSelectItems(IntPtr pidlList, uint cild, IntPtr children, uint dwFlags);

    public static string GetPathFromWindowsExplorer(string dialogtitle = "请选择下载路径")
    {
        try
        {
            OpenDialogDir ofn2 = new OpenDialogDir();
            ofn2.pszDisplayName = new string(new char[2048]);// 存放目录路径缓冲区  
            ofn2.lpszTitle = dialogtitle; // 标题  
            ofn2.ulFlags = 0x00000040; // 新的样式,带编辑框  
            IntPtr pidlPtr = SHBrowseForFolder(ofn2);
            char[] charArray = new char[2048];
            for (int i = 0; i < 2048; i++)
            {
                charArray[i] = '\0';
            }
            SHGetPathFromIDList(pidlPtr, charArray);
            // SHOpenFolderAndSelectItems(pidlPtr, 0, IntPtr.Zero, 0);
            string res = new string(charArray);
            res = res.Substring(0, res.IndexOf('\0'));
            return res;
        }
        catch (Exception e)
        {

        }

        return string.Empty;
    }
}
