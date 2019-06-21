using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
/// <summary>
/// 按钮监听类
/// </summary>
public class UIEventListenerPpt : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{

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
        if (OnClick != null)
            OnClick(this.gameObject);
        //SelectButton
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