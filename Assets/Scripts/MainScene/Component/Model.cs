using UnityEngine;
using System.Collections;

public class Model : MonoBehaviour
{
    public bool isIndependent = false;
    public bool isEmpty = false;//是否是无具体模型物体
    public Material normalMat;//普通材质
    public Material translucentMat;//半透明材质
    public Material highlightMat;//高亮材质
                                 //    public Material signHighLightMat;  //图钉高亮
                         //    public Material fenqu;  //分区
    public Material sspMat;
    public bool isTranslucent = false;//是否是半透明材质
    public bool isActive = true;//是否显示
    public bool isSeleted = false;//是否被选中
    public bool isSignShow = false;//是否被选中
    //public bool isACU;
    // private ModelInfo info=new ModelInfo();
    //public ModelInfo Info;
        // { get { return info; } set { info = value; } }

//    public void BecomeSignHighLight()
//    {
////        gameObject.GetComponent<Renderer>().material = signHighLightMat;//普通材质→当前材质
//        isTranslucent = false;
//        isSeleted = true;
//    }

    public void BecomeNormal()
    {
        if (gameObject.GetComponent<Renderer>() != null)
        {
        gameObject.GetComponent<Renderer>().material = normalMat;//普通材质→当前材质
        isTranslucent = false;
        isSeleted = false;
        }
    }

   

    public void BecomeTranslucent()
    {
        gameObject.GetComponent<Renderer>().material = translucentMat;//半透明材质→当前材质
        isTranslucent = true;
    }
    public void BecomeHight()
    {
        gameObject.GetComponent<Renderer>().material = highlightMat;        
        isSeleted = true;
        isTranslucent = false;
    }
    public void BecomeDisplay()
    {
        gameObject.SetActive(true);
        isActive = true;
    }
    public void BecomeHide()
    { 
        gameObject.SetActive(false);
        isActive = false;
    }
    public Transform parent;
    public Vector3 pos;
    public Vector3 CamBoxPos;
    public void BeSignShow()
    {
        parent = this.transform.parent;
     //   pos = this.transform.position;
      //  CamBoxPos = Camera.main.transform.parent.localPosition;
        isSignShow = true;
    }
    public void setParent()
    {    
        this.transform.SetParent(parent);
        this.transform.position = pos;
      //  Camera.main.transform.parent.localPosition = CamBoxPos;
    }
 

    public void set()
    {
        parent = this.transform.parent;
        pos = this.transform.position;
        isSignShow = true;
    }

    //    public void BecomeFenQu()
    //    {
    ////        gameObject.GetComponent<Renderer>().material = fenqu;
    //        isTranslucent = false;
    //        isSeleted = true;
    //    }

    public void BecomeMouseColor()
    {
        Material mouseMaterial=Instantiate(Resources.Load<Material>("Materials/GreenMaterial"));
        gameObject.GetComponent<Renderer>().material=mouseMaterial;       
    }

    //肌肉起始点标注功能

    public void ChangeMaterial()
    {

        gameObject.GetComponent<MeshRenderer>().material = sspMat;
    }

    public void ChangeMaterial(Material mat)
    {

        gameObject.GetComponent<MeshRenderer>().material = mat;
    }



    public void ChangeShader(Texture2D maintex, Texture2D masktex)
    {
        gameObject.GetComponent<MeshRenderer>().material.shader = Shader.Find("MyShader/SignSelect");
        gameObject.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", maintex);
        gameObject.GetComponent<MeshRenderer>().material.SetTexture("_MaskTex", masktex);

    }
    public void ChangeChoseColor(Color chosCol)
    {
        Material tmp = gameObject.GetComponent<MeshRenderer>().material;
       
            tmp.SetColor("_ChooseColor", chosCol);
           
        
    }

    public void ChangeResColor(Color resCol)
    {
        Material tmp = gameObject.GetComponent<MeshRenderer>().material;


        tmp.SetColor("_ResponseColor", resCol);

    }

    public void ChangeHightColor(Color hightCol)
    {
        Material tmp = gameObject.GetComponent<MeshRenderer>().material;

        tmp.SetColor("_SelectedColor",hightCol);

    }

}
