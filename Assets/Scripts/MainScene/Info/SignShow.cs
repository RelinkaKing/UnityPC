using UnityEngine;
using System.Collections;

public class SignShow : MonoBehaviour {

    public bool isIndependent = false;
    public bool isEmpty = false;//是否是无具体模型物体
    public Material normalMat;//普通材质
    public Material translucentMat;//半透明材质
    public Material highlightMat;//高亮材质
    public bool isTranslucent = false;//是否是半透明材质
    public bool isActive = true;//是否显示
    public bool isSeleted = false;//是否被选中

    public void BecomeNormal()
    {
        gameObject.GetComponent<Renderer>().material = normalMat;//普通材质→当前材质
        isTranslucent = false;
        isSeleted = false;
        //this.GetComponent<HighlightableObject>().flashing = false;
        ChangeMaterial();
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
        //this.GetComponent<HighlightableObject>().flashing = true;
        ChangeMaterial();
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

    public void ChangeMaterial()
    {
        //GetComponent<HighlightableObject>().ReinitMaterials();
    }
}
