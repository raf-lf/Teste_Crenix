using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;

public class Manager_Game : MonoBehaviour
{
    public GameObject vfxAllSlot;
    public static Manager_Game ManagerScript;
    public bool ItemHeld = false;
    public int[] inventorySlotId = { -1, -1, -1, -1, -1 };
    public int[] placementSlotId = { -1, -1, -1, -1, -1 };
    public GameObject[] inventorySlot = new GameObject[5];
    public GameObject[] placementSlot = new GameObject[5];
    public GameObject[] placedGear = new GameObject[5];

    IEnumerator DelayIntro()
    {
        yield return new WaitForSeconds(1);

        Nugget.nuggetScript.Animate(1);
        Nugget.nuggetScript.NuggetLine(0);
    }

    void Awake()
    {
        //Define esse script para ser referênciado por outros scripts
        ManagerScript = GetComponent<Manager_Game>();

        StartCoroutine(DelayIntro());
    }
    public void ToggleSlotCollision(bool turnOn)
    {
        for (int i = 0; i < placementSlot.Length; i++)
        {
            if (turnOn == true) placementSlot[i].GetComponent<Collider2D>().enabled = true;
            else placementSlot[i].GetComponent<Collider2D>().enabled = false;
        }

        for (int i = 0; i < inventorySlot.Length; i++)
        {
            if (turnOn == true) inventorySlot[i].GetComponent<Collider2D>().enabled = true;
            else inventorySlot[i].GetComponent<Collider2D>().enabled = false;
        }
    }

    public void AnimatePlacementSlots(bool dragging)
    {
        for(int i=0; i < placementSlot.Length;i++)
        {
            if (dragging==true)
            {
                if (placementSlotId[i] == -1) placementSlot[i].GetComponent<Animator>().SetInteger("state", 2);
                else placementSlot[i].GetComponent<Animator>().SetInteger("state", 1);
            }
            else placementSlot[i].GetComponent<Animator>().SetInteger("state",0);

        }

    }

    public void CheckSlots()
    {
        /*
        Debug.Log("Slots");
        for (int i = 0; i < inventorySlot.Length; i++)
        {
            //Desliga o bloqueio de raycast para os slots para não interferir com a busca das engrenagens
            //   CanvasGroup slotCanvasGroup = inventorySlot[i].GetComponent<CanvasGroup>();
            //  slotCanvasGroup.blocksRaycasts = false;

            //Raycast para cada slot de inventário
            RaycastHit2D rayInventory = Physics2D.Raycast(inventorySlot[i].transform.position, Vector2.zero);
            
            if (rayInventory.collider != null && rayInventory.collider.gameObject.CompareTag("Item") == true)
            {
                //Busca o Id da engrenagem naquele slot, e associa o ID dela a este slot do inventário
                Item_Pegar itemScript = rayInventory.collider.gameObject.GetComponent<Item_Pegar>();
                inventorySlotId[i] = itemScript.itemId;
            }
            else
            {
                inventorySlotId[i] = -1;
            }
            Debug.Log("Id of item in " + inventorySlot[i].name + ": " + inventorySlotId[i]);

            //Religa bloqueio de raycast dos slots
            //   slotCanvasGroup.blocksRaycasts = true;
        }
        */

        //Faz uma checagem por engrenagens nos slots de posicionamento
        for (int iB = 0; iB < placementSlot.Length; iB++)
        {
            placementSlot[iB].GetComponent<Collider2D>().enabled = false;
            RaycastHit2D rayPlacementHit = Physics2D.Raycast(placementSlot[iB].transform.position, Vector2.zero);
            placementSlot[iB].GetComponent<Collider2D>().enabled = true;

            if (rayPlacementHit == true)
            {
              //  Debug.Log("Detector do Slot " + iB + " colidiu com " + rayPlacementHit.collider.gameObject.name);

                if (rayPlacementHit.collider.gameObject.CompareTag("Item") == true)
                {
                    //Define o item como o colocado ali
                    placedGear[iB] = rayPlacementHit.collider.gameObject;

                    Item_Drag_Worldspace itemScript = rayPlacementHit.collider.gameObject.GetComponent<Item_Drag_Worldspace>();

                    if (itemScript != null) placementSlotId[iB] = itemScript.itemId;
                }
                else
                {
                    placementSlotId[iB] = -1;
                    placedGear[iB] = null;
                }
            }
            else
            {
                placementSlotId[iB] = -1;
                placedGear[iB] = null;
            }

        }

        CheckPlacedGears();
    }

    public void CheckPlacedGears()
    {
        int placedGearsCount=0;

        for (int i=0; i < placementSlot.Length; i++)
        {
            if (placementSlotId[i] != -1) placedGearsCount++;
        }

        if (placedGearsCount >= placementSlot.Length)
        {
            MoveGears(true);
            Nugget.nuggetScript.Animate(2);
            Nugget.nuggetScript.NuggetLine(2);
        }
        else
        {
            MoveGears(false);
        }
    }

    public void MoveGears(bool allGearsPlaced)
    {
        //Define todas as engrenagens que estão posicionadas

        for (int i=0; i < placedGear.Length; i++)
        {
            if (placedGear[i] != null)
            {
                Animator animator = placedGear[i].GetComponent<Animator>();

                if (allGearsPlaced == true)
                {
                    GameObject vfx = Instantiate(vfxAllSlot);
                    vfx.transform.position = placedGear[i].transform.position;

                    if (i <= 2) animator.SetInteger("spin", 1);
                    else animator.SetInteger("spin", -1);
                }
                else
                {
                    animator.SetInteger("spin", 0);
                    Nugget.nuggetScript.Animate(0);
                }
            }
        }

    }

}
