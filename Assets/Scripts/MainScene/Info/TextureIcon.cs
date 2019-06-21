using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;

public class TextureIcon : MonoBehaviour
{
    private float scaleParams;
    public float ScaleParams { get { return scaleParams; } set { scaleParams = value; } }
    public float distanceCamera;
    public XT_MouseFollowRotation mouseDistance;
    public List<Model> containsModels = new List<Model>();
    private ModelInfo info;
    public ModelInfo Info
    { get { return info; } set { info = value; } }

    public Shader myShader;
    public Material _matNormal;        //默认
    public Material _matHight;         //高亮

    public SignElement whichSign;

    public GameObject ParentModel;//父物体

    void Start()
    {
        //ParentModel = this.transform.parent.gameObject;                             //获取父物体
        mouseDistance = Camera.main.GetComponent<XT_MouseFollowRotation>();
    }

    public void PointerClick()
    {
        whichSign.IsChoosed(true);
        PublicClass.ThisSignElement = this.whichSign;
        if (PublicClass.LastSignElement == null)
        {
            //第一次点击
            PublicClass.LastSignElement = this.whichSign;
        }
        else
        {
            PublicClass.LastSignElement.IsChoosed(false);
        }
        PublicClass.LastSignElement = this.whichSign;

        ChangeMyTexture("SignHight/hight" + this.name);
        Camera.main.GetComponent<XT_MouseFollowRotation>().SetTarget(this.transform.gameObject);
    }

    //更换不同数字的普通材质图片  1 2 3 4
    public void ChangeMyTexture(string materialName)
    {
        Material thisMaterial = Resources.Load<Material>("SignMaterials/" + materialName);
        this.GetComponent<MeshRenderer>().material = thisMaterial;
        _matNormal = thisMaterial;
    }
    //高亮材质
    public void HightMaterial(string newMaterialName)
    {
        Material newMaterial = Resources.Load<Material>("SignMaterials/SignHight/" + newMaterialName);
        this.GetComponent<MeshRenderer>().material = newMaterial;
        _matHight = newMaterial;
    }
    //普通材质
    public void NormalMaterial()
    {
        Material newMaterial = Resources.Load<Material>("SignMaterials/" + this.name);
        this.GetComponent<MeshRenderer>().material = newMaterial;
        _matHight = newMaterial;
    }
}
public class SwapTexture : MonoBehaviour
{
    public Shader _shader;//"XM/Effect/SwapTexture” shader
    public Renderer _target;//目标对象
    [Range(0, 1)]
    public float _speed;//速度

    private Material _matOld;
    private Material _matNew;

    public void Swap(Texture tex, Action<bool> onComplete)
    {
        _matOld = _target.material;
        _matNew = new Material(_shader);
        _matNew.SetTexture("_MainTex", _target.material.GetTexture("_MainTex"));
        _matNew.SetTexture("_TargetTex", tex);
        StartCoroutine("_StartChange", onComplete);
    }

    private IEnumerator _StartChange(Action<bool> onComplete)
    {
        _target.material = _matNew;
        _matOld.SetTexture("_MainTex", _matNew.GetTexture("_TargetTex"));
        _target.material = _matOld;
        yield return 1;
    }
}
