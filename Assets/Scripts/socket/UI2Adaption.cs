using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class UI2Adaption : MonoBehaviour {

    public Text text;
    int high;
    int width;
    public bool isAdaption;
    public bool isHaveUIGrid = true;

    public GridLayoutGroup leftListGrid;
    public GridLayoutGroup rightListGrid;
    //public GridLayoutGroup topLeftGrid;
    //public GridLayoutGroup topRightGrid;
    //public GridLayoutGroup shareGrid;

    public Canvas MainUICanvas;
    public Camera UICamera;
    public Camera MainCamera;
    public RenderTexture dRender;
    public RawImage bg_rawimage;


    void Start () {
        //ChangeRenderTexture(dRender,new Vector2(ScreenData.instance.width, ScreenData.instance.high));
    }


	void ChangeRenderTexture(RenderTexture rt,Vector2 size)
	{
        //if (isAdaption)
        //{
        //    rt = new RenderTexture((int)size.x, (int)size.y, 24, RenderTextureFormat.ARGB32);
        //    rt.antiAliasing = 2;
        //    rt.name = "RT_A";
        //    rt.Create();
        //    MainCamera.targetTexture= rt;
        //    bg_rawimage.texture=rt;
        //}
	}

    float oldheight;
    public static int Maxwidth(Resolution[] array)
    {
        if (array == null) throw new Exception("数组空异常");
        int value = 0;
        bool hasValue = false;
        foreach (Resolution x in array)
        {
            if (hasValue)
            {
                if (x.width > value)
                    value = x.width;
            }
            else
            {
                value = x.width;
                hasValue = true;
            }
        }
        if (hasValue) return value;
        throw new Exception("没找到");
    }
    public static int Maxhight(Resolution[] array)
    {
        if (array == null) throw new Exception("数组空异常");
        int value = 0;
        bool hasValue = false;
        foreach (Resolution x in array)
        {
            if (hasValue)
            {
                if (x.height > value)
                    value = x.height;
            }
            else
            {
                value = x.height;
                hasValue = true;
            }
        }
        if (hasValue) return value;
        throw new Exception("没找到");
    }


    void LateUpdate()
    {
        if (MainConfig.isAdaptionUI)
        {
            high = (int)ScreenData.instance.high;
            width = (int)ScreenData.instance.width;
            if (high != oldheight)
        {
            oldheight = high;
            Debug.Log("--------------------------" + width + "x" + high);
            if (isAdaption)
            {
                float proportion = (high * 1f) / width;
                //横屏
                if (proportion < 1)
                {
                    // SetGridGroup(shareGrid, new RectOffset(36, 36,580, 0), new Vector2(150, 150), new Vector2(0, 20), 1, GridLayoutGroup.Constraint.FixedColumnCount);
                    // SetGridGroup(topLeftGrid, new RectOffset(36, 36,71, 0), new Vector2(150, 150), new Vector2(0, 20),2);
                    // SetGridGroup(topRightGrid, new RectOffset(-36, 36,71, 0), new Vector2(150, 150), new Vector2(0, 20),2);
                    SetGridGroup(leftListGrid, new RectOffset(-65, 0, 70, 0), new Vector2(90, 90), new Vector2(20, 20), 2, GridLayoutGroup.Constraint.FixedColumnCount);
                    SetGridGroup(rightListGrid, new RectOffset(0, 0, -40, 0), new Vector2(90, 90), new Vector2(20, 20), 2, GridLayoutGroup.Constraint.FixedColumnCount, GridLayoutGroup.Corner.UpperRight, TextAnchor.MiddleCenter);
                    //Muscle.GetComponent<RectTransform>().sizeDelta = new Vector2(Muscle.GetComponent<RectTransform>().sizeDelta.x, high / 2 - 100);
                    //view.localPosition = new Vector3(width / 2 - uifunc.GetComponent<RectTransform>().sizeDelta.x * 1.5f, uifunc.localPosition.y, 0);
                }
                else
                {
                    // SetGridGroup(shareGrid, new RectOffset(36, 36,392, 0), new Vector2(100, 100), new Vector2(0, 20),1, GridLayoutGroup.Constraint.FixedColumnCount);
                    // SetGridGroup(topLeftGrid, new RectOffset(36, 36, 25, 0), new Vector2(100, 100), new Vector2(0, 20), 1);
                    // SetGridGroup(topRightGrid, new RectOffset(31, 36, 25, 0), new Vector2(100, 100), new Vector2(0, 20), 1);
                    SetGridGroup(leftListGrid, new RectOffset(-30, 0, -300, 0), new Vector2(100, 100), new Vector2(0, 20), 1, GridLayoutGroup.Constraint.FixedColumnCount);
                    SetGridGroup(rightListGrid, new RectOffset(0, 0, -215, 0), new Vector2(100, 100), new Vector2(0, 20), 1, GridLayoutGroup.Constraint.FixedColumnCount, GridLayoutGroup.Corner.UpperRight, TextAnchor.UpperRight);
                    //Muscle.GetComponent<RectTransform>().sizeDelta = new Vector2(Muscle.GetComponent<RectTransform>().sizeDelta.x, 600);
                    //view.localPosition = new Vector3(uifunc.localPosition.x, uifunc.localPosition.y - uifunc.GetComponent<RectTransform>().sizeDelta.y + 200, 0);
                }
                //ChangeRenderTexture(dRender, new Vector2(width, high));
                //MainUICanvas.GetComponent<RectTransform>().sizeDelta = new Vector2(width, high);
                //UICamera.orthographicSize = high / 2f;
            }

        }
        }

    }


    public void SetGridGroup(GridLayoutGroup grid, RectOffset offset, Vector2 size, Vector2 space, int count,
        GridLayoutGroup.Constraint constr = GridLayoutGroup.Constraint.Flexible, 
        GridLayoutGroup.Corner starC= GridLayoutGroup.Corner.UpperLeft
        ,TextAnchor childA = TextAnchor.UpperLeft, GridLayoutGroup.Axis starA = GridLayoutGroup.Axis.Horizontal
        )
    {
        if (!isHaveUIGrid)
        {
            Debug.Log("not set grid");
            return;
        }
        Debug.Log("set grid");
        grid.padding = offset;
        grid.cellSize = size;
        grid.spacing = space;
        grid.startCorner = starC;
        grid.startAxis = starA;
        grid.childAlignment= childA;
        grid.constraint= constr;
        if(constr== GridLayoutGroup.Constraint.FixedColumnCount)
            grid.constraintCount = count;
    }
}
