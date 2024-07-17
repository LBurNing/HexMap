using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MouseEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public GameObject IsShow;


    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("进入");
    }
    //鼠标离开
    public void OnPointerExit(PointerEventData eventData)
    {
        IsShow.SetActive(false);
    }
}
