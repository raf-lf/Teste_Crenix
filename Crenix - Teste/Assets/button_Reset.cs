using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class button_Reset : MonoBehaviour
{
    public void ResetButton()
    {
        Nugget.nuggetScript.NuggetLine(4);
        Nugget.nuggetScript.Animate(3);

        GameObject[] gears;
        gears = GameObject.FindGameObjectsWithTag("Item");
        if (gears != null)
        {
            foreach (GameObject gear in gears)
            {
                gear.GetComponent<Item_Drag_Worldspace>().ResetPosition();
                gear.GetComponent<Collider2D>().enabled = true;

            }
        }

        StartCoroutine(DelayedCheck());
    }

    IEnumerator DelayedCheck()
    {
        yield return new WaitForSeconds(.5f);
        Manager_Game.ManagerScript.CheckSlots();

    }
}
