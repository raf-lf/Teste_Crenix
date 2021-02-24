using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;

public class Manager_Game : MonoBehaviour
{
    public static Manager_Game ManagerScript;

    public GameObject vfxAllSlot;

    //Valores de -1 em um slot são interpretados como um slot vazio. Qualquer outra coisa significa que o slot está preenchido
    public int[] inventorySlotId = { -1, -1, -1, -1, -1 };
    public int[] placementSlotId = { -1, -1, -1, -1, -1 };

    public GameObject[] inventorySlot = new GameObject[5];
    public GameObject[] placementSlot = new GameObject[5];
    public GameObject[] placedGear = new GameObject[5];


    void Awake()
    {
        //Define esse script para ser referênciado por outros scripts
        ManagerScript = GetComponent<Manager_Game>();

        //Atrasa um pouco a explicação pra que a cena carregue e não interfira
        StartCoroutine(DelayIntro());
    }

    IEnumerator DelayIntro()
    {
        yield return new WaitForSeconds(1);
        
        //Anima o nugget pra falar
        Nugget.nuggetScript.Animate(1);
        
        //Fala de introdução
        Nugget.nuggetScript.NuggetLine(0);
    }

    //Função que liga ou desliga a colisão dos slots de encaixe. Serve para evitar que atrapalhem a leitura das engrenagens. Chamada no script das engrenagens
    public void ToggleSlotCollision(bool turnOn)
    {
        //Slots de posicionamento e inventário são separados, pois quero que a quantidade destes slots seja independente
        for (int i = 0; i < placementSlot.Length; i++)
        {
            placementSlot[i].GetComponent<Collider2D>().enabled = turnOn;
        }

        for (int i = 0; i < inventorySlot.Length; i++)
        {
            inventorySlot[i].GetComponent<Collider2D>().enabled = turnOn;
        }
    }

    //Função que lida com o feedback do jogador quando ele está posicionando uma engrenagem. Chamada no script das engrenagens
    public void AnimatePlacementSlots(bool dragging)
    {
        for(int i=0; i < placementSlot.Length;i++)
        {
            if (dragging==true)
            {
                //Se o slot de posicionamento está vazio, indica que uma engrenagem pode ser posta ali
                if (placementSlotId[i] == -1) placementSlot[i].GetComponent<Animator>().SetInteger("state", 2);

                //Se o slot de posicionamento está preenchido, escurece o slot para indicar que ali não tem lugar
                else placementSlot[i].GetComponent<Animator>().SetInteger("state", 1);
            }
            //Remove o feedback quando a engrenagem for posicionada
            else placementSlot[i].GetComponent<Animator>().SetInteger("state",0);
        }
    }

    //Função responsável pela leitura de todos os slots do jogo, de inventário e de posicionamento
    public void CheckSlots()
    {
        //Desliga a colisão de todos os slots para evitar tretas na checagem. Isso ta aqui por precaução, pois já se espera que já estejam desligadas
        ToggleSlotCollision(false);

        //Para cada slot do inventário, detecta se tem uma engrenagem naquela posição
        for (int i = 0; i < inventorySlot.Length; i++)
        {
            RaycastHit2D rayInventoryHit = Physics2D.Raycast(inventorySlot[i].transform.position, Vector2.zero);

            //Tem algo aqui nesse slot?
            if (rayInventoryHit == true)
            {
                //  Debug.Log("Detector do Slot " + iB + " colidiu com " + rayPlacementHit.collider.gameObject.name);

                //Tem algo aqui. É uma engrenagem?
                if (rayInventoryHit.collider.gameObject.CompareTag("Item") == true)
                {
                    //Pega o valor do Id da engrenagem, e substitui pelo valor desse slot
                    inventorySlotId[i] = rayInventoryHit.collider.gameObject.GetComponent<Item_Drag_Worldspace>().itemId;
                }
                //Não é uma engrenagem
                else inventorySlotId[i] = -1;
            }
            //Não tem nada nesse slot
            else inventorySlotId[i] = -1;
        }

        //Para cada slot de posicionamento, detecta se tem uma engrenagem naquela posição
        for (int i = 0; i < placementSlot.Length; i++)
        {
            RaycastHit2D rayPlacementHit = Physics2D.Raycast(placementSlot[i].transform.position, Vector2.zero);

            //Tem algo aqui nesse slot?
            if (rayPlacementHit == true)
            {
                //  Debug.Log("Detector do Slot " + iB + " colidiu com " + rayPlacementHit.collider.gameObject.name);

                //Tem algo aqui. É uma engrenagem?
                if (rayPlacementHit.collider.gameObject.CompareTag("Item") == true)
                {
                    //Define a engrenagem que está posicionada nesse slot na lista das engrenagens posicionadas. É usado pra contar quantas ja foram posicionadas
                    placedGear[i] = rayPlacementHit.collider.gameObject;

                    //Pega o valor do Id da engrenagem, e substitui pelo valor desse slot
                    placementSlotId[i] = rayPlacementHit.collider.gameObject.GetComponent<Item_Drag_Worldspace>().itemId;
                }
                //Não é uma engrenagem
                else
                {
                    //Desvincula este slot de uma engrenagem
                    placedGear[i] = null;
                    placementSlotId[i] = -1;
                }
            }
            //Não tem nada nesse slot
            else
            {
                placedGear[i] = null;
                placementSlotId[i] = -1;
            }

        }

        //Depois da leitura das engrenagens, faz uma contagem pra saber quais engrenagens estão posicionadas
        CountPlacedGears();
    }

    public void CountPlacedGears()
    {
        int placedGearsCount=0;

        //Conta todos os slots de posicionamento que não estejam vazios
        for (int i=0; i < placementSlot.Length; i++)
        {
            if (placementSlotId[i] != -1) placedGearsCount++;
        }

        //Se a contagem atingir a quantidade de slots (todos os slots cheios), inicia a vitória, caso contrário, para as engreanagens
        if (placedGearsCount >= placementSlot.Length)
        {
            //Inicia o movimento das engrenagens
            MoveGears(true);
            
            //Nugget faz a animação e inicia a fala de vitória
            Nugget.nuggetScript.Animate(2);
            Nugget.nuggetScript.NuggetLine(2);
        }
        else
        {
            //Para as engrenagens
            MoveGears(false);
        }
    }

    //Função que controla a rotação das engrenagens posicionadas. O bool vem verdadeiro se todas estiverem no lugar
    public void MoveGears(bool allGearsPlaced)
    {
        //Executa uma vez para cada engrenagem que esteja posicionada
        for (int i=0; i < placedGear.Length; i++)
        {
            //Só executa se uma engrenagem for encontrada
            if (placedGear[i] != null)
            {
                //Define o controlador de animações daquela engrenagem
                Animator animator = placedGear[i].GetComponent<Animator>();
                
                //Todas as engrenagens estão posicionadas?
                if (allGearsPlaced == true)
                {
                    //Feedback de que tudo deu certo!
                    GameObject vfx = Instantiate(vfxAllSlot);
                    vfx.transform.position = placedGear[i].transform.position;

                    //Slots de posicionamento 0, 1 e 2 fazem a engrenagem girar no sentido horário
                    if (i <= 2) animator.SetInteger("spin", 1);

                    //Os outros fazem ela girar no sentido anti-horário
                    else animator.SetInteger("spin", -1);
                }

                //Nem todas engrenagens estão posicionadas
                else
                {
                    //Para todas as engreangens
                    animator.SetInteger("spin", 0);
                    
                    //Interrompe a animação do Nugget, pra que ele pare de ficar pulando com a vitória :'(
                    if (Nugget.nuggetScript.animator.GetInteger("state") == 2) Nugget.nuggetScript.Animate(0);
                }
            }
        }
    }
}
