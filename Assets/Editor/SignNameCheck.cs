using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine.UI;

public class SignNameCheck : MonoBehaviour {
   
    [MenuItem("SignCheck/ReadSignInfoXmlAndRecord")]
    public static void ReadSignInfoXmlAndRecord()
    {
        Object[] currentSelect = Selection.objects;
        for (int j = 0; j < currentSelect.Length; j++)
        {
            GameObject tempsign = currentSelect[j] as GameObject;

            List<string> RightNameArray = new List<string>();
            List<string> CurrentNameArray = new List<string>();

            //读取钉子文档信息

            XmlDocument SignInfo = new XmlDocument();
            SignInfo.Load(PublicClass.filePath + "boneSignInfoList" + ".xml");//资源列表进行
            XmlNodeList signs = SignInfo.SelectNodes("//Atlas[@AtlasID='" + tempsign.name + "']//SignElement");//获取对应图谱下的所有钉子信息

            for (int i = 0; i < signs.Count; i++)
            {
                SignInfo tempInfo = new SignInfo();
                tempInfo.ModelName = signs.Item(i).Attributes["SignName"].Value;
                tempInfo.Chinese = signs.Item(i).Attributes["SignChinese"].Value;
                tempInfo.English = signs.Item(i).Attributes["SignEnglish"].Value;
                tempInfo.Explain = signs.Item(i).Attributes["SignExplain"].Value;
                RightNameArray.Add(tempInfo.ModelName);//信息字典表
            }


            Transform[] tempArray = tempsign.transform.Find("PS").transform.GetComponentsInChildren<Transform>();
            for (int i = 1; i < tempArray.Length; i++)
            {
                CurrentNameArray.Add(tempArray[i].name);
            }

            DebugLog.DebugLogInfo(tempsign.name + "  signs count:" + CurrentNameArray.Count);
   
            DebugLog.DebugLogInfo("读取图谱小毛驴 AtlasTest.xml  atlasNameArray count:" + RightNameArray.Count);

            for (int i = 0; i < CurrentNameArray.Count; i++)
            {
                if (RightNameArray.Contains(CurrentNameArray[i]))
                    continue;
                else
                    Debug.LogError(CurrentNameArray[i] + "   name error");
            }
        }     
    }
}
