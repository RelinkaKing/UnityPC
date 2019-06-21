using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Xml;
using System.Collections.Generic;
using UnityEngine.UI;


public class CheckFileWindows : MonoBehaviour {
    [MenuItem("Window/________CheckFile")]
    static void AddWindow_CheckFile()
    {
        //创建窗口
        Rect wr = new Rect(0, 0, 450, 800);
        InputPathWindows window = (InputPathWindows)EditorWindow.GetWindow(typeof(InputPathWindows), false, "ToolWindow");
        window.Show();
    }
	private string text;
    void OnGUI()
    {
		GUILayout.BeginVertical();
		GUI.skin.label.fontSize = 15;
		GUI.skin.label.alignment = TextAnchor.MiddleCenter;
		GUILayout.Label("FBX导入后处理");
		//输入框控件
		text = EditorGUILayout.TextField("输入模型解释xml路径:", text);
		GUILayout.Space(5);
		if (GUILayout.Button("选中模型后点击,进行模型错误检查", GUILayout.Width(400),GUILayout.Height(20)))
		{
			Transform[] currentSelect = Selection.transforms;
			XmlDocument SignInfo = new XmlDocument();
			SignInfo.Load(text);//资源列表进行
			XmlNodeList models = SignInfo.SelectNodes("//model");//获取对应图谱下的所有钉子信息
			List<string> CurrentNameArray = new List<string>();
			MeshRenderer[] allMeshs = currentSelect[0].GetComponentsInChildren<MeshRenderer>();
			for (int i = 0; i < allMeshs.Length; i++)
			{
				if (CurrentNameArray.Contains(allMeshs[i].gameObject.name))
					continue;
				else
					Debug.LogError(allMeshs[i].gameObject.name + "   name error");
			}
		}
	}
}
