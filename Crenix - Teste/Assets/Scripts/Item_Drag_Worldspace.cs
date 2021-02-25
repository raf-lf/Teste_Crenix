using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Drag_Worldspace : MonoBehaviour
{
    public int itemId;

    //essas variáveis estão públicas para facilitar debug
    public bool mouseOver = false;
    public bool drag = false;
    public bool wasPlaced = false;

    private Vector2 returnPosition;
    private Vector2 beginPosition;
        
    public GameObject vfxSlotOk;
    public GameObject vfxSlotNope;
    public GameObject vfxSlotMeh;
    public GameObject vfxReset;
    public GameObject vfxPop;

    private void Awake()
    {
        //Define a posição inicial do item, pra quando o botão de reset for apertado
        beginPosition = transform.position;
    }

    private void OnMouseEnter()
    {
        //Quando o mouse está sobre o item, interações são possíveis. Ignora se o item já estiver sendo arrastado
        if (drag == false) mouseOver = true;
    }

    private void OnMouseExit()
    {
        //Quando o mouse sai do item, bloqueia interações. Ignora se o item já estiver sendo arrastado
        if (drag == false) mouseOver = false;
    }

    private void OnMouseDown()
    {
        //Acontece toda hora que o mouse for apertado (mas só quando o mouse está sobre o item)
        if (mouseOver == true)
        {
            drag = true;

            //Checa se o item estava em um slot de posicionamento. Se sim, lembra pra resolver um bug com falha no desencaixe mais pra frente
            if (GetComponent<Animator>().GetBool("inventory") == false) wasPlaced = true;
            else wasPlaced = false;

            //Arruma o animator pra animação de estar segurando a engrenagem
            GetComponent<Animator>().SetBool("drag", true);
            GetComponent<Animator>().SetBool("inventory", true);
            GetComponent<Animator>().SetInteger("spin", 0);

            //Define a posição de retorno caso o encaixe falhe
            returnPosition = transform.position;

            //Feedback para o jogador saber onde pode ter um encaixe
            Manager_Game.ManagerScript.AnimatePlacementSlots(true);

            //Desliga o colisor da engrenagem para que uma engrenagem sendo arrastada não seja detectada na leitura
            GetComponent<Collider2D>().enabled = false;
            Manager_Game.ManagerScript.CheckSlots();
            GetComponent<Collider2D>().enabled = true;

        }
    }

    private void OnMouseUp()
    {
        //Acontece toda hora que o mouse for solto (mas só quando o mouse está sobre o item)
        if (mouseOver == true)
        {
            drag = false;

            GetComponent<Animator>().SetBool("drag", false);

            //Desliga o feedback de encaixe
            Manager_Game.ManagerScript.AnimatePlacementSlots(false);

            //Liga o colisor de todos os slots pra poder tentar um encaixe. Normalmente estão desligados para evitar problemas na leitura das engrenagens
            Manager_Game.ManagerScript.ToggleSlotCollision(true);

            //Detecta se tem um slot na posição atual. Desativa o colisor da engrenagem pra não bloquear o raycast
            GetComponent<Collider2D>().enabled = false;
            RaycastHit2D rayHit = Physics2D.Raycast(transform.position, Vector2.zero);
            GetComponent<Collider2D>().enabled = true;

            //Agora que a tentativa de encontrar slots acabou, desliga os colisores dos slots para que eles não interfiram com a leitura das engrenagens
            Manager_Game.ManagerScript.ToggleSlotCollision(false);

            /*
            //Debug pra detectar com o que o item colidiu
            if (rayHit == true) Debug.Log("Colidiu com " + rayHit.collider.gameObject.name);
            else Debug.Log("Não colidiu");
            */
            

            //O item está sobre um slot de encaixe?
            if (rayHit == true && rayHit.collider.gameObject.CompareTag("Slot")==true)
            {
                Slot slotScript = rayHit.collider.gameObject.GetComponent<Slot>();

                //O slot é um slot de posicionamento?
                if (slotScript != null && slotScript.inventorySlot==false)
                {
                    //O slot está vazio?
                    if (Manager_Game.ManagerScript.placementSlotId[slotScript.slotId] == -1)
                    {
                        //  Debug.Log("Id do slot é: " + Manager_Game.ManagerScript.placementSlotId[slotScript.slotId]);


                        //Ajusta a engrenagem para o estado de posicionamento
                        GetComponent<Animator>().SetBool("inventory", false);

                        //Executa o processo de sucesso no encaixe, enviando a posição do slot e indicando que é um slot de posicionamento
                        SlotSuccess(rayHit.collider.gameObject.transform.position, true);
                    }
                    //Slot está ocupado
                    else SlotFail();
                }
                //O slot é um slot de inventário?
                else if (slotScript != null && slotScript.inventorySlot == true)
                {
                    //O slot está vazio?
                    if (Manager_Game.ManagerScript.inventorySlotId[slotScript.slotId] == -1)
                    {
                        //Executa o processo de sucesso no encaixe, enviando a posição do slot e indicando que não é um slot de posicionamento
                        SlotSuccess(rayHit.collider.gameObject.transform.position, false);
                    }
                    //Slot está ocupado
                    else SlotFail();

                }
                //Colidiu com algo que não era um slot
                else SlotFail();

            }
            //Não colidiu
            else SlotFail();

        }

    }

    public void SlotSuccess(Vector2 slotPosition, bool placementSlot)
    {

        //Posiciona a engrenagem no slot
        transform.position = slotPosition;

        //Não interrompe o Nugget não estiver pulando e comemorando a vitória!
        if (Nugget.nuggetScript.animator.GetInteger("state") != 2)
        {
            //Animação de fala do Nugget
            Nugget.nuggetScript.Animate(1);

            //Se a engrenagem for posicionada em um slot de posicionamento, o Nugget fica feliz
            if (placementSlot == true)
            {
                //Falas aleatórias de felicidade
                Nugget.nuggetScript.NuggetLine(1);

                //Feedback de que algo certo aconteceu
                GameObject vfx = Instantiate(vfxSlotOk);
                vfx.transform.position = transform.position;
            }
            //Se a engrenagem for pra um slot de inventário, o Nugget fica entediado
            else
            {
                //Falas aleatórias de tédio
                Nugget.nuggetScript.NuggetLine(3);

                //Feedback de que algo neutro aconteceu, já que o jogador só trocou a posição de engrenagens, ou voltou elas pro inventário
                GameObject vfx = Instantiate(vfxSlotMeh);
                vfx.transform.position = transform.position;
            }
        }

        //Faz uma checagem dos slots, depois de posicionar o item
       // Manager_Game.ManagerScript.CheckSlots();
        StartCoroutine(DelayCheck(.25f));
    }

    public void SlotFail()
    {
        //Feedback de sumiço do item
        GameObject vfx1 = Instantiate(vfxPop);
        vfx1.transform.position = transform.position;

        //Retorna o item a posiçaõ do clique
        transform.position = returnPosition;

        //Se a engrenagem já estava posicionada e o encaixe falhar, trata como se ela tivesse sido posicionada de novo
        if (wasPlaced == true)
        {
            GetComponent<Animator>().SetBool("inventory", false);

            SlotSuccess(returnPosition, true);
        }

        //Se ela não estava posicionada, executa normal
        else
        {
            //Feedback de que algo errado aconteceu
            GameObject vfx2 = Instantiate(vfxSlotNope);
            vfx2.transform.position = transform.position;

            //Faz uma checagem dos slots, depois de posicionar o item
            Manager_Game.ManagerScript.CheckSlots();
        }

    }

    //Essa função é executada quando o botão de reset é apertado. Ela é executada uma vez para todas as engrenagens presentes na cena
    public void ResetPosition()
    {
        //Ajusta as animações pra voltar a engrenagem para o inventário
        GetComponent<Animator>().SetBool("drag", false);
        GetComponent<Animator>().SetBool("inventory", true);

        //Feedback de sumiço do item
        GameObject vfx1 = Instantiate(vfxPop);
        vfx1.transform.position = transform.position;

        //Volta os itens para a posição inicial do jogo
        transform.position = beginPosition;

        //Feedback de que cada peça voltou para a posição original
        GameObject vfx2 = Instantiate(vfxReset);
        vfx2.transform.position = transform.position;

    }

    private void Update()
    {
        //Enquanto o item está sendo segurado, ajusta a posição dele para seguir o mouse
        if (drag == true)
        {
            Vector2 dragPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = Vector2.Lerp(transform.position,dragPosition,0.3f);
        }
        
    }

    IEnumerator DelayCheck(float delay)
    {
        yield return new WaitForSeconds(delay);

        Manager_Game.ManagerScript.CheckSlots();

    }
}
