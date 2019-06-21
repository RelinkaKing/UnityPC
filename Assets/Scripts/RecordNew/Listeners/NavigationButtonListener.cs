using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
/// <summary>
/// 导航按钮监听类（弃用）
/// </summary>
public class NavigationButtonListener : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public int index;
    public void changeButtonColor(int index) {
        if (this.index == index)
        {
            transform.GetComponent<Image>().color = new Color(231 / 255f, 104 / 255f, 2 / 255f, 1);
        }
        else {
            transform.GetComponent<Image>().color = Color.white;
        }
    }
    public void DestroyButton() {
        if (index==0) {
            return;
        }
        Destroy(this.gameObject);
    }
    public static GameObject controller;
    // 定义事件代理
    public delegate void UIEventProxy(GameObject gb);

    // 鼠标点击事件
    public event UIEventProxy OnClick;

    // 鼠标进入事件
    public event UIEventProxy OnMouseEnter;

    // 鼠标滑出事件
    public event UIEventProxy OnMouseExit;

    public void OnPointerClick(PointerEventData eventData)
    {
        //if (OnClick != null)
        //    OnClick(this.gameObject);
        //Debug.Log(index);
        //controller.SendMessage("navButtonSelect",index); 
    }
    public void Click() {
        controller.SendMessage("navButtonSelect", index);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (OnMouseEnter != null)
            OnMouseEnter(this.gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (OnMouseExit != null)
            OnMouseExit(this.gameObject);
    }
}
