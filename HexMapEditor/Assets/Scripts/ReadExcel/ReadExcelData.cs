using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using UnityEngine;
using XLS2Config;


public static class ReadExcelData
{


    public static Dictionary<int, MethodCFG> methodDic = new Dictionary<int, MethodCFG>();
    public static Dictionary<string, MethodCFG> funcDic = new Dictionary<string, MethodCFG>();


    public static void IntiExcelData()
    {
        string tablepath = FileUtil.xlsPath + "副本功能表.xlsx";

        if (!CheckPostfi(tablepath))
            return;

        if (CheckPostfi(tablepath))
        {
            DataTable dataTable = NPOIExcelHelper.Excel2DataTable(tablepath);
            if (dataTable == null)
                return;

            if (dataTable.Rows.Count < 3)
                return;

            //字段名字
            object[] valueNames = dataTable.Rows[0].ItemArray;
            valueNames = valueNames.Take(valueNames.Count() - 1).ToArray();
            //遍历一张表
            for (int i = 2; i < dataTable.Rows.Count; i++)
            {
                object[] values = dataTable.Rows[i].ItemArray;
                MethodCFG methodCfg = new MethodCFG();

                int index = 0;
                foreach (var value in valueNames)
                {
                    string str = values[index].ToString();
                    switch (value)
                    {
                        case "ID":
                            methodCfg.ID = int.Parse(str);
                            break;
                        case "Layer":
                            methodCfg.Layer = str;
                            break;
                        case "Name":
                            methodCfg.Name = str;
                            break;
                        case "Arguments1":
                            methodCfg.Arguments1 = str;
                            break;
                        case "Arguments2":
                            methodCfg.Arguments2 = str;
                            break;
                        case "Desc":
                            methodCfg.Desc = str;
                            break;
                        case "Config":
                            methodCfg.Config = str;
                            break;
                        case "LayerType":
                            methodCfg.LayerType = int.Parse(str);
                            break;
                        case "":
                            break;
                    }

                    index++;
                }
                methodDic.Add(methodCfg.ID, methodCfg);
                funcDic.Add(methodCfg.Name, methodCfg);
            }
        }
    }

    private static bool CheckPostfi(string excelPath)
    {
        if (File.Exists(excelPath))
        {
            if (excelPath.EndsWith(".xls"))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(excelPath + "不支持.xls");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                return false;
            }

            if (excelPath.EndsWith(".xlsx"))
            {
                return true;
            }
        }

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(excelPath + "不存在");
        Console.ForegroundColor = ConsoleColor.DarkGray;
        return false;
    }
}
