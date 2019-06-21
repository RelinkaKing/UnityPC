using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AutoCamTest : MonoBehaviour {

    GameObject obj;
    public GameObject Center;
    Transform[] subTfs;

    Transform current;
    GameObject gmOb;

    Vector3 initCamPos;
    // Use this for initialization
    void Start () {
        initCamPos = Camera.main.transform.position;
        string path = "C:\\VesalDigital\\PPT\\Data\\SA0101008.assetbundle";
        AssetBundle abc = AssetBundle.LoadFromMemory(File.ReadAllBytes(path));
        obj = abc.LoadAsset<GameObject>("SA0101008") as GameObject;
        obj = GameObject.Instantiate(obj);
        obj.transform.position = Vector3.zero;

        obj = obj.transform.Find("SA0101008").gameObject;
        //Camera.main.transform.LookAt(obj.transform);

        subTfs = obj.GetComponentsInChildren<Transform>();
        List<Transform> tmpList = new List<Transform>();
        foreach (Transform tmp in subTfs) {
            if (tmp.childCount == 0) {
                tmpList.Add(tmp);
                tmp.gameObject.SetActive(false);
            }
        }
        subTfs = tmpList.ToArray();
        max = subTfs.Length;
        
    }
    int max;
    public void getRandomObj() {
        int tmp = Random.Range(0, 50);
        List<int> tmpList = new List<int>();
        destoryAll();
        GameObject tmpObj = new GameObject("center");
        current = tmpObj.transform;
        
        for (int i =0;i<=tmp;i++) {
            int tmpIndex = Random.Range(0, max);
            if (tmpList.Contains(tmpIndex)) {
                continue;
            }
            tmpList.Add(tmpIndex);
            Transform tmpTf= GameObject.Instantiate(subTfs[tmpIndex],current,true);
            tmpTf.gameObject.SetActive(true);
            //subTfs[tmpIndex].SetParent(current);
        }

        gmOb = current.gameObject;
        gmOb.SetActive(true);
        initGmPos();
    }
    public void destoryAll() {
        if (current == null) {
            return;
        }
        Destroy(current.gameObject);
    }
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.F1)) {
            getRandomObj();
        }
	}

    //依据Center参考大小与距离 ， 计算高亮物体最佳缩放距离
    public float calculateFitDis()
    {
        //GameObject obj = hLObj;//GameObject.Find(highLithName);
        GameObject obj = gmOb;
        Debug.Log(obj.name);
        Bounds centerBounds = Center.transform.GetComponent<Renderer>().bounds;
        Renderer tmpRr = obj.GetComponent<Renderer>();
        Bounds objBounds;
        if (tmpRr == null)
        {
            Renderer[] renders = obj.GetComponentsInChildren<Renderer>();
            objBounds = new Bounds(obj.transform.position, Vector3.zero);
            for (int i = 0; i < renders.Length; i++)
            {

                objBounds.Encapsulate(renders[i].bounds);
            }
        }
        else
        {
            objBounds = tmpRr.bounds;
        }

        float tmp = Vector3.Distance(centerBounds.max, centerBounds.center) / Vector3.Distance(objBounds.max, objBounds.center);
        Debug.Log(initCamPos + "-=-=-=" + Center.transform.position);
        float fitDis = Vector3.Distance(initCamPos, Center.transform.position) * 0.95f / tmp;
        

        return Mathf.Max(fitDis, 20);
    }

    //计算模型的中心点
    public void initGmPos()
    {

        Transform parent = current;
        Renderer[] renders = parent.GetComponentsInChildren<Renderer>();
        Vector3 postion = parent.position;
        Quaternion rotation = parent.rotation;
        Vector3 scale = parent.localScale;
        parent.position = Vector3.zero;
        parent.rotation = Quaternion.Euler(Vector3.zero);
        parent.localScale = Vector3.one;
        Vector3 center = Vector3.zero;


        for (int i = 0; i < renders.Length; i++)
        {
            center += renders[i].bounds.center;
        }

        center /= parent.GetComponentsInChildren<Renderer>().Length;

        Bounds bounds = new Bounds(center, Vector3.zero);

        for (int i = 0; i < renders.Length; i++)
        {
            bounds.Encapsulate(renders[i].bounds);
        }

        parent.localScale = scale;


        for (int i = 0; i < parent.childCount; i++)
        {
            parent.GetChild(i).position -= center;
        }

        Bounds centerBounds = Center.transform.GetComponent<Renderer>().bounds;

        //物体距离中心点最远比 估算 缩放比
        float tmp = Vector3.Distance(centerBounds.max, centerBounds.center) * 0.7f / Vector3.Distance(bounds.max, bounds.center);
        current.localScale = Vector3.one * tmp;
        current.position = Center.transform.position;
        //绕Y180转正，可有可无
        Debug.Log(rotation.eulerAngles + "!!!!!!!!!!!!!!!!!!!!!");
        if ((rotation.eulerAngles.x > 260 && rotation.eulerAngles.x < 280))
        {
            current.rotation = Quaternion.Euler(new Vector3(-90, 0, -180));
        }
        else
        {
            current.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
        }
       
    }

    //计算模型的中心点
    public Vector3 initGmPos(Transform parent)
    {
        Renderer[] renders = parent.GetComponentsInChildren<Renderer>();

        Vector3 center = Vector3.zero;
        for (int i = 0; i < renders.Length; i++)
        {
            center += renders[i].bounds.center;
        }
        center /= parent.GetComponentsInChildren<Renderer>().Length;
        return center;
    }
}
