using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private GameObject previousSelection;

    public void OnPointerEnter(PointerEventData eventData)
    {
        previousSelection = EventSystem.current.currentSelectedGameObject;
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (previousSelection != null)
        {
            EventSystem.current.SetSelectedGameObject(previousSelection);
            previousSelection = null;
        }
    }
}
