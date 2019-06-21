using Assets.Scripts.DbDao;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//读取文件夹
public class FindFilePath : MonoBehaviour {

	void Start () {
        //DontDestroyOnLoad(this);
        //StartCoroutine(GetData());
    }
	
    public IEnumerator GetData()
    {
        var cache_resWeb = new CacheOption<GetResWeb>();
        cache_resWeb.setCacheJson("http://res.vesal.site/json/V310_GetResWeb.json");
        yield return null;
        var cache_resRelation = new CacheOption<GetResRelation>();
        cache_resRelation.setCacheJson("http://res.vesal.site/json/V310_GetResRelation.json");
        yield return null;
    }

    public Transform ImageArea;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
        JObject o = new JObject();
        o["url"] = "baidu.com";
        o["locaton"] = "妖姬/触发点.html";
        UnityMessageManager.Instance.SendMessageToWeb("date_name",o);
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            UnityMessageManager.Instance.PlayVideo("C:/Users/Raytine/Desktop/620x252_search.mp4");
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            UnityMessageManager.Instance.LoadImageInHtml("C:/Users/Raytine/AppData/Roaming/Vesal_unity_PC/Relation/newSpider/4895/CFD066_TTC.jpg", ImageArea.GetComponent<RectTransform>().rect);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            UnityMessageManager.Instance.CloseWebView();
        }
    }
}