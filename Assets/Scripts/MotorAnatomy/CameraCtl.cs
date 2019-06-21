using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using XLua;

[Hotfix]
public class CameraCtl : MonoBehaviour
{
    public TouchCtrl touchCtrl;
    FingerTouchState FingerState = FingerTouchState.NonFingerTouch;
    public bool isDragSlide;
    public Transform emptyBox;
    public Interaction interaction;
    // Use this for initialization
    [LuaCallCSharp]
    public void Init()
    {
        Interaction.instance.maxDistance = 18;
        interaction.setParamValue2();
        Interaction.AnimationRotateY = 0.5f;
    }


    // Update is called once per frame
    [LuaCallCSharp]
    void Update()
    {
        if (Input.touchCount >= 1 && !isDragSlide)
        {

            if ((Input.touchCount > 0 && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)))
            {

                //新的操控模式
                if (Input.touchCount > 1)
                {
                    if (FingerState != FingerTouchState.TwoFingerTouch)
                    {
                        //enter_special_two_finger_state();
                        FingerState = FingerTouchState.TwoFingerTouch;
                    }
                    touchCtrl.TouchTwoFinger();

                }
                else if (Input.touchCount == 1)
                {
                    if (FingerState == FingerTouchState.TwoFingerTouch)
                    {
                        //leave_special_two_finger_state();
                        FingerState = FingerTouchState.OneFingerTouch;
                    }
                    touchCtrl.TouchOneFinger();
                }
            }
        }

#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        else
        {
            if (!isDragSlide && !EventSystem.current.IsPointerOverGameObject())
            {
                if (FingerState == FingerTouchState.TwoFingerTouch)
                {
                    //leave_special_two_finger_state();
                    FingerState = FingerTouchState.NonFingerTouch;
                }
                touchCtrl.MouseClick();
                touchCtrl.MouseDrag();
                touchCtrl.MouseRollWheel();
            }

        }
#endif
    }

    int rotateCount = 0;
    [LuaCallCSharp]
    public void rotateRight()
    {
        isDragSlide = true;
        float y = emptyBox.rotation.eulerAngles.y;
        rotateCount = (int)((y + 90) / 90);
        iTween.RotateTo(emptyBox.gameObject, iTween.Hash("rotation", new Vector3(0, 90 * rotateCount, 0), "time", 1));
        StartCoroutine(savePos());

        //iTween.RotateTo(emptyBox.gameObject, new Vector3(0, 90 * rotateCount + 1, 0), 1);
    }
    [LuaCallCSharp]
    public void rotateUP()
    {
        isDragSlide = true;
        float x = emptyBox.rotation.eulerAngles.x;
        float y = emptyBox.rotation.eulerAngles.y;
        if (x < 0.001 && y == 180)
            rotateCount = 3;
        else
            rotateCount = (int)((x + 90) / 90);

        iTween.RotateTo(emptyBox.gameObject, iTween.Hash("rotation", new Vector3(90 * rotateCount, 0, 0), "time", 1));
        StartCoroutine(savePos2());

        //iTween.RotateTo(emptyBox.gameObject, new Vector3(0, 90 * rotateCount + 1, 0), 1);
    }
    [LuaCallCSharp]
    public void review()
    {
        isDragSlide = true;
        // iTween.MoveTo(gameObject, iTween.Hash("position", new Vector3(0, 0, transform.localPosition.z), "time", 0));
        transform.localPosition = new Vector3(0, 0, transform.localPosition.z);
        iTween.RotateTo(emptyBox.gameObject, iTween.Hash("rotation", new Vector3(0, 0, 0), "time", 1));

        StartCoroutine(savePos());
    }
    [LuaCallCSharp]
    IEnumerator savePos()
    {
        yield return new WaitForSeconds(1);

        interaction.setParamValue();
        isDragSlide = false;
    }
    [LuaCallCSharp]
    IEnumerator savePos2()
    {
        yield return new WaitForSeconds(1);
        interaction.setParamValue2();
        isDragSlide = false;
    }

}
