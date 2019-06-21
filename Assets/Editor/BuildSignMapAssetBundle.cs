using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Xml;
using System.Collections.Generic;

public class BuildSignMapAssetBundle : MonoBehaviour {

    [MenuItem("EditorObjectTool/PrintObjectFullName")]
    public static void PrintObjectFullName()
    {
        Object[] choosed = Selection.objects;

        for (int i = 0; i < choosed.Length; i++)
        {
            print(choosed[i].name);
        }
    }


    [MenuItem("EditorObjectTool/ChangeTargetFileName")]
    public static void ChangeTargetFileName()
    {

    }
    [MenuItem("EditorObjectTool/ToTargetFileName")]
    public static void ToTargetFileName()
    {
        target = Selection.objects;
        for (int i = 0; i < target.Length; i++)
        {
            for (int j = 0; j < resource.Length; j++)
            {
                if (target[i].name == resource[j].name.ToLower())
                {
                    //FileInfo.MoveTo();// target[i]
                }
            }
        }
    }
    static Object[] resource; static Object[] target;
    [MenuItem("EditorObjectTool/GetFileNames")]
    public static void GetFileNames()
    {
        resource = Selection.objects;
    }


    static List<Material> standardMaterials=new List<Material> ();

    [MenuItem("EditorObjectTool/GiveMaterial/GetStandardMaterial")]
    public static void GetStandardMaterial()
    {
        standardMaterials.Clear();
        object[] resources = Selection.objects;
        for (int i = 0; i < resources.Length; i++)
        {
            Material temp = resources[i] as Material;
            standardMaterials.Add(temp);
        }
        print("记录材质个数："+ standardMaterials.Count+"  请选中需要更改的钉子");
    }

    [MenuItem("EditorObjectTool/GiveMaterial/GiveMaterialToSignPrefab(simple) #a")]
    public static void GiveMaterialToSignPrefab()
    {
        object[] resources = Selection.objects;
        for (int i = 0; i < resources.Length; i++)
        {
            // ((GameObject)resources[i]).GetComponent<MeshRenderer>().material = standardMaterials[i];
            MeshCollider[] colliders=((GameObject)resources[i]).GetComponentsInChildren<MeshCollider>();
            MeshRenderer[] renders=((GameObject)resources[i]).GetComponentsInChildren<MeshRenderer>();
            for (int j = 0; j < colliders.Length; j++)
            {
                print(colliders.Length);
                if( !colliders[j].name.Contains("PS"))
                    DestroyImmediate(colliders[j]);
            
            }
            for (int j = 0; j < renders.Length; j++)
            {
                renders[j].material=null;
            }
        }
        print("更换材质");
    }


    [MenuItem("EditorObjectTool/GiveMaterial/GiveMaterialToSignPrefab(multi)")]
    public static void multiGiveMaterialToSignPrefab()
    {
        System. Random random_length=new System.Random ();
        object[] resources = Selection.objects;//选中所有钉子预制体
        for (int i = 0; i < resources.Length; i++)
        {
            Transform[] psName = ((GameObject)resources[i]).transform.Find("PS").transform.GetComponentsInChildren<Transform>();
            for (int j = 1; j < psName.Length; j++)
            {
                
                DestroyImmediate((psName[j]).GetComponent<MeshRenderer>());
                (psName[j]).gameObject.AddComponent<MeshRenderer>();
                int a=random_length.Next(0,standardMaterials.Count-1);
                (psName[j]).GetComponent<MeshRenderer>().material = standardMaterials[a];//Instantiate( standardMaterials[a]);

                print(a);
            }
        }
        print("批量更换材质");

        // for (int i = 0; i < resources.Length; i++)
        // {
        //     Transform[] allObject = ((GameObject)resources[i]).transform.GetComponentsInChildren<Transform>();
        //     for (int j = 1; j < allObject.Length; j++)
        //     {                
        //         if (!allObject[j].name.Contains("PS"))
        //         {
        //             allObject[j].GetComponent<MeshRenderer>().material = null;
        //         }
        //         else
        //         {
        //             if (allObject[j].name != "PS")
        //             {
        //                 if (allObject[j].GetComponent<MeshRenderer>() == null)
        //                     print(resources[i]+"  "+allObject[j].name+ "  没有片模型");
        //                 if (allObject[j].GetComponent<MeshCollider>() == null)
        //                     allObject[j].gameObject.AddComponent<MeshCollider>();
        //             }
        //         }
        //     }
        // }
        // print("添加碰撞体，删除模型材质");
    }

    [MenuItem("EditorObjectTool/GiveMaterial/ChangeAlphaValue")]
    public static void ChangeAlphaValue()
    {
        object[] resources = Selection.objects;
        for (int i = 0; i < resources.Length; i++)
        {
            Material temp = resources[i] as Material;
            temp.color = new Vector4(temp.GetColor("_Color").r, temp.GetColor("_Color").g
                , temp.GetColor("_Color").b, 150 / 255f);
        }
        print("更改透明度");
    }
}
