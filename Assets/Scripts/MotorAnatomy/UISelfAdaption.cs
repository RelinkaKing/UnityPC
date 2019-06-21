using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;
[Hotfix]
public class UISelfAdaption : MonoBehaviour {
    public Transform Muscle;
    public Transform Musclefather;
    public Transform view;
    public Transform uifunc;
    float high;
    public float luahigh =1200;
    [LuaCallCSharp]
    private void Start()
    {
        Muscle.GetComponent<RectTransform>().sizeDelta = new Vector2(Muscle.GetComponent<RectTransform>().sizeDelta.x, 350);
        //high = Screen.height;
    }
    //[LuaCallCSharp]
    //private void Update()
    //{
    //    if (high != Screen.height)
    //    {
    //        high = Screen.height;
    //        float width = Screen.width;
    //        float proportion = high / width;
    //        if (high < luahigh)
    //        {
    //            //if (Musclefather.childCount <= 4)
    //            //    Muscle.GetComponent<RectTransform>().sizeDelta = new Vector2(Muscle.GetComponent<RectTransform>().sizeDelta.x, Musclefather.childCount*100);
    //            //else
    //                Muscle.GetComponent<RectTransform>().sizeDelta = new Vector2(Muscle.GetComponent<RectTransform>().sizeDelta.x, 300);
    //            float y = Muscle.GetComponent<RectTransform>().sizeDelta.y;
             
    //            view.localPosition = new Vector3(width / 2 - uifunc.GetComponent<RectTransform>().sizeDelta.x * 1.5f, uifunc.localPosition.y, 0);
    //        }
    //        else
    //        {
    //            Muscle.GetComponent<RectTransform>().sizeDelta = new Vector2(Muscle.GetComponent<RectTransform>().sizeDelta.x, 300);
    //            float y = Muscle.GetComponent<RectTransform>().sizeDelta.y;

    //            view.localPosition = new Vector3(width / 2 - uifunc.GetComponent<RectTransform>().sizeDelta.x * 1.5f, uifunc.localPosition.y, 0);

    //            //if(Musclefather.childCount<=6)
    //            //    Muscle.GetComponent<RectTransform>().sizeDelta = new Vector2(Muscle.GetComponent<RectTransform>().sizeDelta.x, Musclefather.childCount*100);
    //            //else               
    //            //Muscle.GetComponent<RectTransform>().sizeDelta = new Vector2(Muscle.GetComponent<RectTransform>().sizeDelta.x, 650);
    //            // MuscleButton.GetComponent<RectTransform>().sizeDelta = new Vector2(MuscleButton.GetComponent<RectTransform>().sizeDelta.x, y);
    //            //view.localPosition = new Vector3(uifunc.localPosition.x, uifunc.localPosition.y - uifunc.GetComponent<RectTransform>().sizeDelta.y + 200, 0);
    //        }
    //    }
    //}
}
