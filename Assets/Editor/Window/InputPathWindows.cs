using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Xml;
using System.Collections.Generic;
using UnityEngine.UI;

public class InputPathWindows : EditorWindow
{
    static int windowIndex;
    static void OpenWindow(int index)
    {
        windowIndex = index;
    }


    [MenuItem("Window/________MianModelXmlInputPathWindows")]
    static void AddWindow()
    {
        //创建窗口
        Rect wr = new Rect(0, 0, 450, 800);
        //InputPathWindows window = (InputPathWindows)EditorWindow.GetWindowWithRect(typeof(InputPathWindows), wr, false, "模型检错和预制体操作");
        InputPathWindows window = (InputPathWindows)EditorWindow.GetWindow(typeof(InputPathWindows), false, "ToolWindow");
        window.Show();
        OpenWindow(1);
    }
    [MenuItem("Window/________CreateABFiles")]
    static void CreateABFilesWindow()
    {
        //创建窗口
        Rect wr = new Rect(0, 0, 1500, 300);
        InputPathWindows window = (InputPathWindows)EditorWindow.GetWindow(typeof(InputPathWindows), false, "打包assetbundle");
        window.Show();
        OpenWindow(2);

    }
    //输入文字的内容
    private string text;
    //选择贴图的对象
    private string materialPath;
    GameObject prefab;
    private string description;
    private bool showBtn;
    public void Awake()
    {
        //在资源中读取一张贴图
    }

    //绘制窗口时调用
    void OnGUI()
    {
        switch (windowIndex)
        {
            case 1:
                GUILayout.BeginVertical();
                GUI.skin.label.fontSize = 15;
                GUI.skin.label.alignment = TextAnchor.MiddleCenter;
                GUILayout.Label("FBX导入后处理");
                //输入框控件
                text = EditorGUILayout.TextField("输入模型解释xml路径:", text);
                GUILayout.Space(5);
                if (GUILayout.Button("选中模型后点击,进行模型错误检查", GUILayout.Width(400),GUILayout.Height(20)))
                {
                    //打开一个通知栏
                    //this.ShowNotification(new GUIContent("This is a Notification"));
                    if (text == string.Empty)
                        this.ShowNotification(new GUIContent("xml路径不能位空"));
                    Transform[] currentSelect = Selection.transforms;
                    if (currentSelect.Length == 0)
                        this.ShowNotification(new GUIContent("请选中模型"));

                    XmlDocument SignInfo = new XmlDocument();
                    SignInfo.Load(text);//资源列表进行
                    XmlNodeList models = SignInfo.SelectNodes("//model");//获取对应图谱下的所有钉子信息
                    List<string> CurrentNameArray = new List<string>();

                    for (int i = 0; i < models.Count; i++)
                    {
                        ModelInfo tempInfo = new ModelInfo();
                        tempInfo.ModelName = ((XmlElement)models[i]).GetAttribute("pinyin");
                        tempInfo.Chinese = ((XmlElement)models[i]).GetAttribute("chinese");
                        tempInfo.English = ((XmlElement)models[i]).GetAttribute("english");
                        tempInfo.Note = ((XmlElement)models[i]).GetAttribute("notes");
                        CurrentNameArray.Add(tempInfo.ModelName);//信息字典表
                    }
                    Debug.Log("记录表长度：" + CurrentNameArray.Count);

                    MeshRenderer[] allMeshs = currentSelect[0].GetComponentsInChildren<MeshRenderer>();
                    Debug.Log("模型长度：" + allMeshs.Length);
                    for (int i = 0; i < allMeshs.Length; i++)
                    {
                        if (CurrentNameArray.Contains(allMeshs[i].gameObject.name))
                            continue;
                        else
                            Debug.LogError(allMeshs[i].gameObject.name + "   name error");
                    }
                }

                GUILayout.Space(20);
                GUI.skin.label.fontSize = 15;
                GUI.skin.label.alignment = TextAnchor.MiddleCenter;
                GUILayout.Label("对材质球进行处理");
                GUILayout.Space(5);
                EditorGUILayout.LabelField("             将‘材质编辑工具’拖动到导入文件中的材质文件夹下，双击启动"); 
                //description = EditorGUILayout.TextArea(description, GUILayout.MaxHeight(75));

                GUILayout.Space(20);
                GUI.skin.label.fontSize = 15;
                GUI.skin.label.alignment = TextAnchor.MiddleCenter;
                GUILayout.Label("制作模型预置体");
                GUILayout.Space(5);
                if (GUILayout.Button("选中模型附加 mesh 组件", GUILayout.Width(400)))
                {
                    Transform[] currentSelect = Selection.transforms;
                    if (currentSelect.Length == 0)
                        this.ShowNotification(new GUIContent("请选中模型"));
                    MeshRenderer[] allMeshs = currentSelect[0].GetComponentsInChildren<MeshRenderer>();
                    for (int i = 0; i < allMeshs.Length; i++)
                    {
                        if (allMeshs[i].gameObject.GetComponent<MeshCollider>() == null)
                        {
                            allMeshs[i].gameObject.AddComponent<MeshCollider>();
                        }
                        if (allMeshs[i].gameObject.GetComponent<Model>() == null)
                        {
                            allMeshs[i].gameObject.AddComponent<Model>();
                        }
                    }
                    //System.Diagnostics.Process.Start(Application .streamingAssetsPath);
                    //System.Diagnostics.Process.Start(Application.streamingAssetsPath);
                }
                GUILayout.Space(5);
                EditorGUILayout.LabelField("输入材质地址:");
                materialPath = EditorGUILayout.TextField(new GUIContent("Assets/Resources/"), materialPath, GUILayout.Width(400));
                if (GUILayout.Button("选中模型，更换材质", GUILayout.Height(20), GUILayout.Width(400)))
                {
                    Transform[] currentSelect = Selection.transforms;
                    if (currentSelect.Length == 0)
                        this.ShowNotification(new GUIContent("请选中模型"));

                    foreach (Transform item in currentSelect[0])
                    {
                        foreach (Transform t in item)
                        {
                            string topMatName = item.name;
                            t.GetComponent<Model>().normalMat = Resources.Load<Material>(materialPath + topMatName + "1");
                            t.GetComponent<Model>().translucentMat = Resources.Load<Material>(materialPath + topMatName + "2");
                            t.GetComponent<Model>().highlightMat = Resources.Load<Material>(materialPath + topMatName + "3");
                            // t.GetComponent<Model>().signHighLightMat = Resources.Load<Material>("SignHightLight");
                            try
                            {
                                // t.GetComponent<Model>().fenqu = Resources.Load<Material>(materialPath + topMatName + "4");
                            }
                            catch
                            {

                            }
                            t.GetComponent<MeshRenderer>().material = Resources.Load<Material>(materialPath + topMatName + "1");
                        }
                    }
                }
                GUILayout.Space(20);
                EditorGUILayout.LabelField("将模型手动拖到 Prefab 文件夹下，完成预置体制作");
                GUILayout.Space(20);
                GUI.skin.label.fontSize = 15;
                GUI.skin.label.alignment = TextAnchor.MiddleCenter;
                GUILayout.Label("调整材质");
                GUILayout.Space(20);
                GUI.skin.label.fontSize = 5;
                GUI.skin.label.alignment = TextAnchor.UpperLeft;
                GUILayout.Label("。。。。。。。。。。。。。。。。。。。。。。。。。。");
                GUILayout.Space(5);
                GUILayout.Space(20);
                GUI.skin.label.fontSize = 15;
                GUI.skin.label.alignment = TextAnchor.MiddleCenter;
                GUILayout.Label("打包AB文件");
                GUILayout.Space(5);
                if (GUILayout.Button("打包Prefab文件夹下的预置体", GUILayout.Width(400)))
                {
                    // AssetBundleBuilder.AutoBuildAll();      //自动打包
                }
                GUILayout.EndVertical();
                break;
            case 2:
                
                break;
            case 3:
                EditorGUILayout.LabelField("制作  预制体:");
                prefab = EditorGUILayout.ObjectField("添加贴图", prefab, typeof(GameObject), true) as GameObject;
                if (GUILayout.Button("", GUILayout.Width(400)))
                {

                }
                EditorGUILayout.LabelField("打包 AB 文件:");
                if (GUILayout.Button("", GUILayout.Width(400)))
                {
                    // AssetBundleBuilder.AutoBuildAll();      //自动打包
                }
                GUILayout.Space(10);
                GUILayout.BeginHorizontal();
                GUILayout.Label("Description", GUILayout.MaxWidth(80));
                description = EditorGUILayout.TextArea(description, GUILayout.MaxHeight(75));
                GUILayout.EndHorizontal();

                showBtn = EditorGUILayout.Toggle("Show Button", showBtn);
                if (showBtn)
                {  //开关点开
                    if (GUILayout.Button("Close"))
                    { //绘制按钮
                        this.Close(); //关闭面板
                    }
                }
                break;
            default:
                break;
        }
        
       

        /*
         * 
         * if (GUILayout.Button("打开通知", GUILayout.Width(200)))
        {
            //打开一个通知栏
            this.ShowNotification(new GUIContent("This is a Notification"));
        }

        if (GUILayout.Button("关闭通知", GUILayout.Width(200)))
        {
            //关闭通知栏
            this.RemoveNotification();
        }

        //文本框显示鼠标在窗口的位置
        EditorGUILayout.LabelField("鼠标在窗口的位置", Event.current.mousePosition.ToString());

        //选择贴图
        texture = EditorGUILayout.ObjectField("添加贴图", texture, typeof(Texture), true) as Texture;

        if (GUILayout.Button("关闭窗口", GUILayout.Width(200)))
        {
            //关闭窗口
            this.Close();
        }
         */


    }

    //更新
    void Update()
    {

    }

    void OnFocus()
    {
        //Debug.Log("当窗口获得焦点时调用一次");
    }

    void OnLostFocus()
    {
        //Debug.Log("当窗口丢失焦点时调用一次");
    }

    void OnHierarchyChange()
    {
        //Debug.Log("当Hierarchy视图中的任何对象发生改变时调用一次");
    }

    void OnProjectChange()
    {
        //Debug.Log("当Project视图中的资源发生改变时调用一次");
    }

    void OnInspectorUpdate()
    {
        //Debug.Log("窗口面板的更新");
        //这里开启窗口的重绘，不然窗口信息不会刷新
        this.Repaint();
    }

    void OnSelectionChange()
    {
        //当窗口出去开启状态，并且在Hierarchy视图中选择某游戏对象时调用
        //foreach (Transform t in Selection.transforms)
        //{
        //    //有可能是多选，这里开启一个循环打印选中游戏对象的名称
        //    Debug.Log("OnSelectionChange" + t.name);
        //}
    }

    void OnDestroy()
    {
        //Debug.Log("当窗口关闭时调用");
    }
}
