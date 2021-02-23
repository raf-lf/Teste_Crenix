using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Drag_Worldspace : MonoBehaviour
{
    public int itemId;
    public bool mouseOver = false;
    public bool drag = false;
    private Vector2 returnPosition;
    private Vector2 beginPosition;

    public GameObject vfxSlotOk;
    public GameObject vfxSlotNope;
    public GameObject vfxSlotMeh;
    public GameObject vfxReset;
    public GameObject vfxPop;

    private void Awake()
    {
        beginPosition = transform.position;
    }

    private void OnMouseEnter()
    {
        if (drag == false) mouseOver = true;
    }

    private void OnMouseExit()
    {
        if (drag == false) mouseOver = false;
    }

    private void OnMouseDown()
    {
        if (mouseOver == true)
        {
            drag = true;
            GetComponent<Animator>().SetBool("drag", true);
            GetComponent<Animator>().SetBool("inventory", true);
            returnPosition = transform.position;
            GetComponent<Animator>().SetInteger("spin", 0);

            Manager_Game.ManagerScript.AnimatePlacementSlots(true);

            Manager_Game.ManagerScript.ToggleSlotCollision(true);

            GetComponent<Collider2D>().enabled = false;
            Manager_Game.ManagerScript.CheckSlots();
            GetComponent<Collider2D>().enabled = true;

        }
    }

    private void OnMouseUp()
    {
        if (mouseOver == true)
        {
            GetComponent<Animator>().SetBool("drag", false);
            drag = false;

            Manager_Game.ManagerScript.AnimatePlacementSlots(false);

            //Raycast pra detectar slot de posicionamento. Colisor do item é temporáriamente ativado pra não interferir.
            Collider2D collider = GetComponent<Collider2D>();
            collider.enabled = false;
            RaycastHit2D rayHit = Physics2D.Raycast(transform.position, Vector2.zero);
            collider.enabled = true;

            /*
            //Debug pra detectar com o que o item colidiu.
            if (rayHit == true) Debug.Log("Colidiu com " + rayHit.collider.gameObject.name);
            else Debug.Log("Não colidiu");
            */

            //Detecta se item está sobre um slot
            if (rayHit == true && rayHit.collider.gameObject.CompareTag("Slot")==true)
            {
                Slot slotScript = rayHit.collider.gameObject.GetComponent<Slot>();

                if (slotScript != null && slotScript.inventorySlot==false)
                {
                    //Posiciona o item, mas apenas se o slot estiver vazio (Id -1, que é guardado no Manager_Game)
                    if (Manager_Game.ManagerScript.placementSlotId[slotScript.slotId] == -1)
                    {
                        //  Debug.Log("Id do slot é: " + Manager_Game.ManagerScript.placementSlotId[slotScript.slotId]);

                        GetComponent<Animator>().SetBool("inventory", false);
                        SlotSuccess(rayHit.collider.gameObject.transform.position, true);
                    }
                    else SlotFail();
                }
                else if (slotScript != null && slotScript.inventorySlot == true)
                {
                    if (Manager_Game.ManagerScript.inventorySlotId[slotScript.slotId] == -1)
                    {
                        SlotSuccess(rayHit.collider.gameObject.transform.position, false);
                    }
                    else SlotFail();

                }
                else SlotFail();

            }
            else SlotFail();

            Manager_Game.ManagerScript.ToggleSlotCollision(false);
        }

    }

    public void SlotSuccess(Vector2 slotPosition, bool placementSlot)
    {
        transform.position = slotPosition;
        GetComponent<Collider2D>().enabled = true;
        Manager_Game.ManagerScript.CheckSlots();


        if (Nugget.nuggetScript.animator.GetInteger("state") != 2)
        {
            Nugget.nuggetScript.Animate(1);

            if (placementSlot == true)
            {
                Nugget.nuggetScript.NuggetLine(1);

                GameObject vfx = Instantiate(vfxSlotOk);
                vfx.transform.position = transform.position;
            }
            else
            {
                Nugget.nuggetScript.NuggetLine(3);
                GameObject vfx = Instantiate(vfxSlotMeh);
                vfx.transform.position = transform.position;
            }
        }
    }

    public void SlotFail()
    {
        GameObject vfx1 = Instantiate(vfxPop);
        vfx1.transform.position = transform.position;

        transform.position = returnPosition;
        GetComponent<Collider2D>().enabled = true;
        Manager_Game.ManagerScript.CheckSlots();

        GameObject vfx2 = Instantiate(vfxSlotNope);
        vfx2.transform.position = transform.position;
    }

    public void ResetPosition()
    {
        GetComponent<Animator>().SetBool("drag", false);
        GetComponent<Animator>().SetBool("inventory", true);

        GameObject vfx1 = Instantiate(vfxPop);
        vfx1.transform.position = transform.position;

        transform.position = beginPosition;

        GameObject vfx2 = Instantiate(vfxReset);
        vfx2.transform.position = transform.position;

    }

    private void Update()
    {
        if (drag == true)
        {
            Vector2 dragPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = Vector2.Lerp(transform.position,dragPosition,0.3f);
        }
        
    }
}
