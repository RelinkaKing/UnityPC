using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SuperGameManager : MonoBehaviour 
{
    public static SuperGameManager Instance;
	public bool autoLocate = true;//是否自动定位
	public bool blocksMode = false;
    public bool blockBaseSelected = false;
	public TransformTools.Type transformType = TransformTools.Type.Select;//默认为选择模式
    public Texture highLightTexture;
    public Vector3 core = Vector3.zero;//这个用来记录核心，包括单选和多选时的综合核心
    
    public Material lightning;
    public Material lightning_fade;

    public TouchMode touchMode = TouchMode.None;//初始为未判断
    public float time = 0f;//记录按下的时间
    public bool validClickRequest = true;
    public bool buttonPressed = false;
    public bool firstBlood = true;
    
    public float length = 16f;//大于这个距离的物体都隐藏
    
    public List<TouchableObject> objectsAll;
    public List<TouchableObject> objectsInScene;
    public List<TouchableObject> objectsInSceneTemp;
   
    public Shader transparent;
    public Shader diffuse;
    public Shader vertexLit;
    public List<Transform> selectedObjects;
    //----------------------------------------------------------------
    

    //Game Main Camera
    Camera gameMainCamera;
    //2D UI Camera
    Camera inGame2DUICamera;
	//3D UI Camera
	Camera inGame3DUICamera;


    public enum States
    {
        Show, //显示
        Fade, //透明
        Hide//隐藏
    }

    public enum TouchMode//用来标志双指操控为平移还是缩放
    {
        Pan,//平移
        Zoom,//缩放
        None//未判断
    }
    
    

    void Awake()
    {
        Instance = this;
        
      

        //Retrieve the game's Main Camera
        gameMainCamera = Camera.main;
        //Retrieve the 2D UI Camera
        inGame2DUICamera = NGUITools.FindCameraForLayer(9);
		//Retrieve the 3D UI Camrea
		inGame3DUICamera = NGUITools.FindCameraForLayer (10);

        //if (!MenuManager.Instance.autoHideToggle.value)
        {
            //length = 175f;
            //Debug.Log("为什么这里会没有执行");
            //Debug.Log("length现在的值是：" + length);
        }
        
    }

    void Update()
    {
        if (buttonPressed)
        {
            time += Time.deltaTime;

            if (time >= 0.5f)
            {
                CancelClickRequest();
            }
        }
    }
	
    void QuitNow()
    {
        Application.Quit();
    }
  
    public void ResetAllObjects()
    {
        if (selectedObjects.Count != 0)//说明存有元素
        {
            foreach (Transform obj in selectedObjects)
            {
                obj.GetComponent<TouchableObject>().Reset(true);
            }
            selectedObjects.Clear();
        }
        GameCamera.Inst.names1.Clear();
        GameCamera.Inst.names2.Clear();
    }
    
    
    public void ClearSelcetedObjects()
    {
        selectedObjects.Clear();
    }
    
   

    /**
    public void Init()
    {
        foreach(TouchableObject tou in preObjectsAll)
        {
            if (tou.isNode && tou.state != States.Hide &&tou.isRealTouchableObject)
            {
                objectsAll.Add(tou);
                objectsInScene.Add(tou);
                objectsInSceneTemp.Add(tou);
            }
        }
    }
    **/
    public void ControlRegion()//核心代码啊这是
    {
        
        List<TouchableObject> specialObjects = new List<TouchableObject>();
        //如果禁止计算区域，比如在骨单位场景就要这么做
        if (GameCamera.Inst.noControlRegion)
        {
            return;
        }
        foreach (TouchableObject obj in objectsInSceneTemp)
        {
            if (obj.CalculateDistance(core) > length)//大于这个距离，不管他本来是显示还是隐藏，都要隐藏掉
            {
                obj.SuperShow(false);
            }
            else//在距离内，也要检查他是否是hide状态，是的话，仍然不显示
            {
                if (!(obj.state == States.Hide))//如果他不是真正的隐藏状态，那么再进行下一步判断
                {
                    obj.SuperShow(true);
                    if (obj.isSpecial && !specialObjects.Contains(obj.m_transform.parent.GetComponent<TouchableObject>()))
                    {
                        //如果是特殊的，把它的父级物体先加进集合以备后用,但应该先检测下是否已经包含这个元素了
                        specialObjects.Add(obj.m_transform.parent.GetComponent<TouchableObject>());
                    }
                }
            }
        }
        foreach (TouchableObject obj in specialObjects)
        {
            for (int i = 0; i < obj.m_transform.childCount; i++)
            {
                obj.m_transform.GetChild(i).GetComponent<TouchableObject>().SuperShow(true);
            }
        }
        foreach (Transform obj in selectedObjects)
        {
            TouchableObject tobj = obj.GetComponent<TouchableObject>();
            if (tobj.isSpecial)
            {
                Transform tobjp = obj.parent;
                for (int i = 0; i < tobjp.childCount; i++)
                {
                    tobjp.GetChild(i).GetComponent<TouchableObject>().SuperShow(true);
                }
            }
            else
            {
                tobj.SuperShow(true);
            }
        }
    }

    public void ControlRegionTwo()//核心代码啊这是
    {
        if (GameCamera.Inst.noControlRegion)
        {
            return;
        }
        foreach (TouchableObject obj in objectsInSceneTemp)
        {
            if (obj.state == States.Hide)
            {
                Transform tempRoot = obj.m_transform;
                while (tempRoot.parent != null)
                {
                    int count = 0;
                    for (int i = 0; i < tempRoot.parent.childCount; i++)
                    {
                        if (tempRoot.parent.GetChild(i).GetComponent<TouchableObject>().state == SuperGameManager.States.Hide)
                        {
                            count++;
                        }
                    }
                    if (count == tempRoot.parent.childCount)
                    {
                        tempRoot.parent.GetComponent<TouchableObject>().state = States.Hide;
                    }
                    else
                    {
                        tempRoot.parent.GetComponent<TouchableObject>().state = States.Fade;
                    }
                    tempRoot = tempRoot.parent;
                }
            }
            else//obj.state != State.Hide
            {
                Transform tempRoot = obj.m_transform;
                while (tempRoot.parent != null)
                {
                    int count = 0;
                    for (int i = 0; i < tempRoot.parent.childCount; i++)
                    {
                        if (tempRoot.parent.GetChild(i).GetComponent<TouchableObject>().state == SuperGameManager.States.Show)
                        {
                            count++;
                        }
                    }
                    if (count == tempRoot.parent.childCount)
                    {
                        tempRoot.parent.GetComponent<TouchableObject>().state = States.Show;
                    }
                    else
                    {
                        tempRoot.parent.GetComponent<TouchableObject>().state = States.Fade;
                    }
                    tempRoot = tempRoot.parent;
                }
            }
        }
        //及时更新备份数据
        objectsInSceneTemp.Clear();
        foreach (TouchableObject tou in objectsInScene)
        {
            objectsInSceneTemp.Add(tou);
        }
        
    }

    public void HideSelected()//隐藏所选
    {
        foreach (Transform obj in selectedObjects)
        {
            TouchableObject tobj = obj.GetComponent<TouchableObject>();
            if (tobj.isSpecial)
            {
                Transform tobjp = obj.parent;
                for (int i = 0; i < tobjp.childCount; i++)
                {
                    tobjp.GetChild(i).GetComponent<TouchableObject>().Hide(false);
                }
            }
            else
            {
                tobj.Hide(false);
            }
        }
    }

    public void ShowAll()//全部取消隐藏
    {
        foreach (TouchableObject tobj in objectsInScene)
        {
            //tobj.Solid(false);
            tobj.SuperShow(true);
        }
    }

    public void HideOther()//隐藏未选定对象,先全部隐藏，再把选定的重新显示出来就行了
    {
        foreach (TouchableObject tobj in objectsInScene)
        {
            tobj.Hide(false);
        }
        foreach (Transform obj in selectedObjects)
        {
            TouchableObject tobj = obj.GetComponent<TouchableObject>();
            if (tobj.isSpecial)
            {
                Transform tobjp = obj.parent;
                for (int i = 0; i < tobjp.childCount; i++)
                {
                    //tobjp.GetChild(i).GetComponent<TouchableObject>().Solid(false);
                    tobjp.GetChild(i).GetComponent<TouchableObject>().SuperShow(true);
                }
            }
            else
            {
                //tobj.Solid(false);
                tobj.SuperShow(true);
            }
        }
    }

    
    public void CancelClickRequest()
    {
        validClickRequest = false;
    }
    
   
    
    

    
    

    
}
