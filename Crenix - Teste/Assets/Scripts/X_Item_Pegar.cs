using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class X_Item_Pegar : MonoBehaviour, IPointerEnterHandler,IPointerExitHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerClickHandler
{
    public int itemId;
    private Animator animator;
    private Vector2 returnPosition;

    private void Awake()
    {
        //Define os componentes presentes no objeto
        animator = GetComponent<Animator>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Faz a animação de hover, mas apenas se o o item estiver no idle
     //   if (animator.GetInteger("state")==0) animator.SetInteger("state", 1);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Volta para o idle, mas apenas se o o item estiver no hover
   //     if (animator.GetInteger("state") == 1) animator.SetInteger("state", 0);
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        Manager_Game.ManagerScript.CheckSlots();

        //Animação do item sendo arrastado
        animator.SetInteger("state", 2);
        returnPosition = transform.position;

        animator.SetBool("placement", false);

        Nugget.nuggetScript.Animate(1);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        RaycastHit2D ray = Physics2D.Raycast(transform.position, Vector2.zero);

        if (ray.collider != null)
        {
            Debug.Log(ray.collider.gameObject.name);

            if (ray.collider.gameObject.CompareTag("Slot") == true)
            {
                Slot slotScript = ray.collider.gameObject.GetComponent<Slot>();

                if (slotScript.inventorySlot == true)
                {
                    if (Manager_Game.ManagerScript.placementSlotId[slotScript.slotId] == -1)
                    {
                        animator.SetBool("placement", true);
                    }
                    else
                    {
                        animator.SetBool("slotOk", false);
                        animator.SetInteger("state", 3);
                    }

                }
                else
                {
                    if (Manager_Game.ManagerScript.inventorySlotId[slotScript.slotId] == -1)
                    {
                        animator.SetBool("slotOk", true);
                        animator.SetInteger("state", 3);
                    }
                    else
                    {
                        animator.SetBool("slotOk", false);
                        animator.SetInteger("state", 3);
                    }

                }
            }
            else
            {
                animator.SetBool("slotOk", false);
                animator.SetInteger("state", 3);
            }
        }

        else
        {
            animator.SetBool("slotOk", false);
            animator.SetInteger("state", 3);
        }

    }

    public void OnDrag(PointerEventData eventData)
    {
        //Atualiza a posição do item a cada frame
        transform.position = Input.mousePosition;
    }

    public void ResetPosition()
    {
        transform.position = returnPosition;
    }

    public void SetAnimationState(int state)
    {
        animator.SetInteger("state", state);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //Animação do item sendo arrastado
        animator.SetInteger("state", 2);

        Vector2 dragPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = Vector2.Lerp(transform.position, dragPosition, 0.1f);

        animator.SetBool("placement", true);
    }


    /*  public void OnPointerDown(PointerEventData eventData)
      {
          //Tenta pegar o item apenas se não estiver segurando outro
          if (Manager_Game.ItemHeld == -1)
          {
              //Coloca o id do item segurado pra o id deste item
              Manager_Game.ItemHeld = id;
              animator.SetInteger("state", 2);

          }
      }


      public void OnPointerUp(PointerEventData eventData)
      {
          //Larga o Item, tirando ele do cursor
          Manager_Game.ItemHeld = -1;
          //Inicia a amação de reset
          animator.SetInteger("state", 0);
      }

    */
}
