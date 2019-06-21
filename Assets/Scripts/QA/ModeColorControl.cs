using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// （旧模型颜色控制脚本，弃用）
/// </summary>
public class ModeColorControl : MonoBehaviour
{
    //public Model md;
    ////基础
    //public Material normalMat;
    ////高亮
    //public Material highlightMat;
    ////半透明
    //public Material translucentMat;


    //string objName;
    //string parentName;
    ////离相机最小距离，小于透明
    //static float nearDis;
    //static GameObject mainCamera;
    //MeshRenderer mr;

    ////是否需要高亮
    //public bool isNeedHL = false;
    ////是否需要保持基础色
    //public bool isNeedBasic = false;
    ////当前显示状态
    //bool isHighLight = false;
    //bool isbasic = false;
    ////该脚本Update执行总开关
    //public static bool mccKey = false;
    ////是图钉
    //public static bool isSign = false;
    ////高亮进入相机视野
    //public static bool isVisible = false;
    //MaterialMode currentMode = MaterialMode.Basic;
    
    //void Start()
    //{
        
    //    if (mainCamera == null)
    //    {
    //        try
    //        {
    //            mainCamera = transform.gameObject;
    //        }
    //        catch (SystemException e){
    //            Debug.Log(e.Message);
    //        }
    //    }
    //    name = this.transform.name;
    //    parentName = this.transform.parent.name;
    //    nearDis = 10f;
    //    mr = transform.GetComponent<MeshRenderer>();
    //    md = transform.GetComponent<Model>();
    //    if (md != null && ModelControl.type == Question.QuestionType.model)
    //    {
    //        normalMat = md.normalMat;
    //        highlightMat = md.highlightMat;
    //        translucentMat = md.translucentMat;
    //    }
    //    else if (mr != null && ModelControl.type == Question.QuestionType.thumbtack)
    //    {
    //        //normalMat = mr.material;
    //        //translucentMat = mr.material;
    //        //if (mr.material.name.StartsWith("PS"))
    //        //{
    //        //    Material tmpM = Material.Instantiate(mr.material);
    //        //    tmpM.color = new Color(0,255,0);
    //        //    highlightMat = tmpM;

    //        //}
    //        //else {
    //        //    highlightMat = mr.material;
    //        //}

    //        //highlightMat.color;
    //    }
    //    else {
    //        Debug.LogWarning("模型失效！");
    //        this.gameObject.SetActive(false);
    //    }

    //}
    //public void init() {

    //}
    //public enum MaterialMode
    //{
    //    Basic,
    //    Transparent,
    //    Translucence,
    //    HighLight,
    //}

    //void Update()
    //{
        
    //    if (mccKey)
    //    {
    //        isBecameVisible();
    //        if (isNeedHL && isHighLight)
    //        {
    //            if (count < 30)
    //            {
    //                count++;
    //            }
    //            else
    //            {
    //                count = 0;
    //                changAlpha();
    //            }
    //        }
    //        if (isSign)
    //        {
    //            if (isNeedHL && !isHighLight) {
    //                highlightMat = mr.material;
    //                isHighLight = true;
    //            }
    //            return;
    //        }
    //        if (mr == null)
    //        {
    //            return;
    //        }
    //        if (isNeedHL && !isHighLight)
    //        {
    //            isHighLight = true;
    //        }
    //        else if (isNeedBasic && !isbasic)
    //        {
    //            isbasic = true;
    //        }
    //        if (isHighLight)
    //        {
    //            if (!isNeedHL && currentMode == MaterialMode.HighLight)
    //            {
    //                SetMaterial(MaterialMode.Basic);
    //            }
    //            else if (currentMode != MaterialMode.HighLight)
    //            {
    //                SetMaterial(MaterialMode.HighLight);
    //            }
    //        }
    //        else if (isbasic)
    //        {
    //            if (currentMode != MaterialMode.Basic)
    //            {
    //                SetMaterial(MaterialMode.Basic);
    //            }
    //        }
    //        else
    //        {
    //            float dis = Vector3.Distance(this.transform.position, mainCamera.transform.position);
    //            if (dis > nearDis + 20 && currentMode != MaterialMode.Basic)
    //            {
    //                SetMaterial(MaterialMode.Basic);
    //            }
    //            else if (dis <= nearDis + 20 && dis > nearDis && currentMode != MaterialMode.Translucence)
    //            {
    //                SetMaterial(MaterialMode.Translucence);
    //            }
    //            else if (dis <= nearDis && currentMode != MaterialMode.Transparent)
    //            {
    //                SetMaterial(MaterialMode.Transparent);
    //            }
    //        }



    //    }
    //}
    //int count = 0;
    //public void changAlpha() {
    //    //Debug.Log("changAlpha");
    //    Color tmp = mr.material.color;
    //    if (tmp.r == 1) {
    //        if (GlobalVariable.tmpHighLightColor != Color.white) {
    //            tmp = GlobalVariable.tmpHighLightColor;
    //        }
    //        else if (isSign)
    //        {
    //            tmp = GlobalVariable.SignHighLightColor;
    //        }
    //        else {
    //            tmp = GlobalVariable.ModelHighLightColor;
    //        }
    //        //Debug.Log("changAlpha4");
    //    }
    //    else {
    //        tmp = Color.white;
    //        //Debug.Log("changAlpha3");
    //    }
    //    mr.material.color = tmp;
    //}
    ///// <summary>
    ///// 改变材质
    ///// </summary>
    ///// <param name="renderingMode"></param>
    //public void SetMaterial(MaterialMode renderingMode)
    //{
    //    //isHighLight = false;
    //    //this.gameObject.SetActive(true);
    //    mr.enabled = true;
    //    switch (renderingMode)
    //    {
    //        case MaterialMode.Basic:
    //            currentMode = MaterialMode.Basic;
    //            mr.material = normalMat;
    //            isHighLight = false;
    //            break;
    //        case MaterialMode.Translucence:
    //            currentMode = MaterialMode.Translucence;
    //            mr.material = translucentMat;
    //            isHighLight = false;
    //            break;
    //        case MaterialMode.Transparent:
    //            currentMode = MaterialMode.Transparent;
    //            mr.enabled = false;
    //            isHighLight = false;
    //            break;
    //        case MaterialMode.HighLight:
    //            #if UNITY_EDITOR
    //            highlightMat.shader = Shader.Find("Standard");
    //            #endif
    //            Material tmp = GameObject.Instantiate(highlightMat) as Material;
    //            tmp.color = GlobalVariable.ModelHighLightColor;
                
    //            mr.material = tmp;
    //                //highlightMat;
    //            isHighLight = true;
    //            currentMode = MaterialMode.HighLight;
    //            this.gameObject.SetActive(false);
    //            this.gameObject.SetActive(true);
    //            break;
    //    }

    //}

    ////private void OnBecameVisible()
    ////{
    ////    if (isNeedHL)
    ////    {
    ////        isVisible = true;
    ////    }
    ////}
    //public void isBecameVisible()
    //{
    //    //Debug.Log("isBecameVisible");
    //    //Debug.DrawLine(this.transform.position, transform.transform.position, Color.red);
    //    if (isNeedHL && !isVisible)
    //    {

    //        Ray tmpRay = new Ray(transform.transform.position, this.transform.position-transform.transform.position);
    //        RaycastHit tmpRh;
    //        if (Physics.Raycast(tmpRay,out tmpRh,500)) {
    //            //Debug.Log(tmpRh.transform.name);
    //            if (tmpRh.transform.name == this.transform.name) {
    //                isVisible = true;
    //                return;
    //            }
    //        };

    //        tmpRay = new Ray(this.transform.position, transform.transform.position - this.transform.position);
    //        RaycastHit[] tmpRhs = Physics.RaycastAll(tmpRay, 500);

    //        if (tmpRhs.Length==2)
    //        {
    //            for (int i = 0;i< tmpRhs.Length;i++) {
    //                if (tmpRhs[i].transform.name != this.transform.name && tmpRhs[i].transform.name != transform.transform.name)
    //                {
    //                    return;
    //                }
    //            }
    //            isVisible = true;
    //        }

    //    }
    //}
}
