using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Xml;
using System;
using System.Xml.Serialization;
using System.IO;
using System.Text;

public class CreatXml : EditorWindow
{

    string path;
    Rect rect;

    [MenuItem("CreatSth/CreatXml")]
    static void Init()
    {
        EditorWindow.GetWindow(typeof(CreatXml));
    }

    [SerializeField]//必须要加
    protected List<Transform> _assetLst = new List<Transform>();

    //序列化对象
    protected SerializedObject _serializedObject;

    //序列化属性
    protected SerializedProperty _assetLstProperty;

    void OnGUI()
    {


        EditorGUILayout.LabelField("拖入预制体");

        rect = EditorGUILayout.GetControlRect(GUILayout.Width(600));
        //将上面的框作为文本输入框  
        //path = EditorGUI.TextField(rect, path);

        Transform[] transforms = Selection.transforms;
        //如果鼠标正在拖拽中或拖拽结束时，并且鼠标所在位置在文本输入框内  
        //if ((Event.current.type == EventType.DragUpdated
        //  || Event.current.type == EventType.DragExited)
        //  && rect.Contains(Event.current.mousePosition))
        //{
        //    DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
        //    //改变鼠标的外表  
        //    if (DragAndDrop.paths != null && DragAndDrop.paths.Length > 0)
        //    {
        //        path = DragAndDrop.paths[0];
        //    }
        //}

        //绘制对象
        GUILayout.Space(10);

        //更新
        _serializedObject.Update();

        //开始检查是否有修改
        EditorGUI.BeginChangeCheck();

        //显示属性
        //第二个参数必须为true，否则无法显示子节点即List内容
        EditorGUILayout.PropertyField(_assetLstProperty, true);

        //结束检查是否有修改
        if (EditorGUI.EndChangeCheck())
        {//提交修改
            _serializedObject.ApplyModifiedProperties();
        }

        GUILayout.Space(10);
        //if (path == null)
        //{
        //    return;
        //}
        //if (!path.EndsWith(".xml"))
        //{
        //    GUILayout.Label("请将待检查XML文件拖入路径框");
        //}

        if (_assetLst.Count == 0)
        {
            GUILayout.Label("请将待检查对象拖入列表");
        }

        if (_assetLst.Count != 0)
        {

            if (GUILayout.Button("生成xml"))
            {
                createXml(_assetLst);
            }

        }
    }
    public void createXml(List<Transform> _assetLst) {

        foreach (Transform tmpTfs in _assetLst) {
            Debug.Log(tmpTfs.name);
            AbDoc ad = new AbDoc();
            ad.name = tmpTfs.name;
            List<AbInfo> AbInfos = new List<AbInfo>();
            foreach (Transform sub in tmpTfs.GetComponentsInChildren<Transform>()) {
                if (sub.childCount == 0) {
                    AbInfos.Add(new AbInfo { name = sub.name});
                }
            }
            ad.count = AbInfos.Count;
            ad.AbInfos = AbInfos;
            ad.createTime = getTime();
            string xmlpath = Application.dataPath + "\\StreamingAssets\\xml\\" + tmpTfs.name + ".xml";
            Directory.CreateDirectory(get_dir_from_full_path(xmlpath));
            
            if (File.Exists(xmlpath)) {
                File.Delete(xmlpath);
            }
            SaveObject(xmlpath, ad);
        }
    }
    public static String get_dir_from_full_path(String path)
    {
        int pos = path.LastIndexOf("/");
        if (pos < 0)
        {
            pos = path.LastIndexOf("\\");
            if (pos < 0)
                return path;

            return path.Substring(0, pos + 1);
        }

        return path.Substring(0, pos + 1);
    }
    public static string getTime(string format = "yyyy_MM_dd_HH_mm_ss")
    {
        string time = Convert.ToDateTime(System.DateTime.Now).ToString(format);//24小时
        return time;
    }
    protected void OnEnable()
    {
        //使用当前类初始化
        _serializedObject = new SerializedObject(this);
        //获取当前类中可序列话的属性
        _assetLstProperty = _serializedObject.FindProperty("_assetLst");
    }

    public static bool SaveObject<T>(string path, T t) where T : class
    {
        XmlSerializer xmldes = new XmlSerializer(typeof(T));

        using (FileStream fs = new FileStream(path, FileMode.CreateNew, FileAccess.ReadWrite))
        {
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
            {
                xmldes.Serialize(sw, t, ns);
            }
        }

        return false;
    }

}
[Serializable]
[XmlRoot("AbDoc")]
public class AbDoc
{
    [XmlAttribute("name")]
    public string name;
    [XmlAttribute("count")]
    public float count;
    [XmlAttribute("createTime")]
    public string createTime;
    [XmlElement("AbInfos")]
    public List<AbInfo> AbInfos;
}
[Serializable]
public class AbInfo
{
    [XmlAttribute("name")]
    public string name;
}

