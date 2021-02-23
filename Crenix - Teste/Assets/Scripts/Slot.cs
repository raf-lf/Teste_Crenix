using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class Slot : MonoBehaviour//,IDropHandler, IPointerEnterHandler,IPointerExitHandler
{
    public int slotId = 0;
    public bool inventorySlot;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    /*
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            Item_Pegar droppedItemScript = eventData.pointerDrag.GetComponent<Item_Pegar>();

            if (Manager_Game.ManagerScript.inventorySlotId[slotId] == -1)
            {
                //Transforma a posição do item arrastado para a mesma deste objeto
                eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
                eventData.pointerDrag.GetComponent<Transform>().position = GetComponent<Transform>().position;
                animator.SetBool("slotOk", true);
            }
            else animator.SetBool("slotOk", false);

            animator.SetInteger("state", 2);

        }

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(eventData.pointerDrag != null)
        {
            if(Manager_Game.ManagerScript.inventorySlotId[slotId] == -1) animator.SetBool("slotOk", true);
            else animator.SetBool("slotOk", false);
            animator.SetInteger("state", 1);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        animator.SetInteger("state",0);
    }

    public void SetAnimationState(int state)
    {
        animator.SetInteger("state", state);
    }
    */
}
