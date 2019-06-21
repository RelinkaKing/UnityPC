using UnityEngine;
using UnityEditor;

public class PrefabEditor : MonoBehaviour {

[MenuItem("Tools/PlayerPrefsDeleteAll")]
    public static void ClearPref()
    {
        PlayerPrefs.DeleteAll();
    }


    [MenuItem("Tools/BatchPrefab All Children")]
	public static void BatchPrefab()
    {
        Transform tParent = ((GameObject)Selection.activeObject).transform;
        Object tempPrefab;
        int i = 0;
        foreach (Transform t in tParent)
        {
            tempPrefab = PrefabUtility.CreateEmptyPrefab("Assets/Prefab/" + t.name + ".prefab");
            tempPrefab = PrefabUtility.ReplacePrefab(t.gameObject, tempPrefab);
            i++;
        }
    }

    [MenuItem("Tools/GetMat All Children")]
    public static void GetMat()
    {
        Transform tParent = ((GameObject)Selection.activeObject).transform;
        int i = 0;

        //foreach (Transform item in tParent)
        //{
        //    int j = 0;
            foreach (Transform t in tParent)
            {
                string matName = t.GetComponent<MeshRenderer>().materials[0].name;
                //string matName = t.GetComponent<Model>().normalMat.name;
                string topMatName = matName.Substring(0, matName.LastIndexOf("_"));
                //string topMatName = item.name;
                t.GetComponent<Model>().normalMat = Resources.Load<Material>("Material/" + topMatName + "_1");
                t.GetComponent<Model>().translucentMat = Resources.Load<Material>("Material/" + topMatName + "_2");
                t.GetComponent<Model>().highlightMat = Resources.Load<Material>("Material/" + topMatName + "_3");
                //t.GetComponent<Model>().signHighLightMat = Resources.Load<Material>("Material/SignHightLight");
                //try
                //{
                //    t.GetComponent<Model>().fenqu = Resources.Load<Material>("Material/" + topMatName + "_4");
                //}
                //catch
                //{
                //}
                t.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Material/" + topMatName + "_1");
            //    j++;
            //}
            i++;
        }
    }

    [MenuItem("Tools/Mutil-BatchPrefab All Children")]
    public static void MutilBatchPrefab()
    {
        GameObject[] objs = Selection.gameObjects;
        for (int j = 0; j < objs.Length; j++)
        {
            Transform tParent = objs[j].transform;
            Object tempPrefab;
            int i = 0;
            foreach (Transform t in tParent)
            {
                tempPrefab = PrefabUtility.CreateEmptyPrefab("Assets/Prefab/" + t.name + ".prefab");
                tempPrefab = PrefabUtility.ReplacePrefab(t.gameObject, tempPrefab);
                i++;
            }
        }        
    }
}
