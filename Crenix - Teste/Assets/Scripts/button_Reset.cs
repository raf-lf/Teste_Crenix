using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class button_Reset : MonoBehaviour
{
    public void ResetButton()
    {
        //Faz o Nugget surtar e girar :O
        Nugget.nuggetScript.NuggetLine(4);
        Nugget.nuggetScript.Animate(3);

        //Busca todas as engrenagens na cena e as reseta
        GameObject[] gears;
        gears = GameObject.FindGameObjectsWithTag("Item");
        if (gears != null)
        {
            foreach (GameObject gear in gears)
            {
                gear.GetComponent<Item_Drag_Worldspace>().ResetPosition();

            }
        }

        //Faz uma checagem, mas um pouco atrasada, pra dar tempo de todas as peças voltarem pros seus respectivos lugares
        StartCoroutine(DelayedCheck());
    }

    IEnumerator DelayedCheck()
    {
        yield return new WaitForSeconds(.25f);
        Manager_Game.ManagerScript.CheckSlots();

    }
}
