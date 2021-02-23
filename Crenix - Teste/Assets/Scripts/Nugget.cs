using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Nugget : MonoBehaviour
{
    public Animator animator;
    public AudioSource audioSource;

    public static Nugget nuggetScript;

    public float textDelay = 0.1f;
    public Text textBox;
    private string currentText;

    public AudioClip clip;
        
    private string textLibrary(int index)
    {
        switch(index)
        {
            case (0):
                return "ENCAIXE AS ENGRENAGENS EM QUALQUER ORDEM.";
            case (1):
                int stringRoll = Random.Range(0,6);
                if (stringRoll == 0) return "VOCÊ É MUITO BOM NISSO!";
                else if (stringRoll == 1) return "OH! ESSA VAI AÍ? LEGAL!";
                else if (stringRoll == 2) return "SUPIMPA!";
                else if (stringRoll == 3) return "ENCAIXOU DIREITINHO!";
                else if (stringRoll == 4) return "PARABÉNS AOS ENVOLVIDOS!";
                else return ":3";

            case (2):
                return "YAY! PARABÉNS! TAREFA CONCLUÍDA!";

            case (3):
                int stringRoll2 = Random.Range(0, 6);
                if (stringRoll2 == 0) return "AH...";
                else if (stringRoll2 == 1) return "OK, MAS QUANDO VOCÊ VAI ENCAIXAR BONITINHO?";
                else if (stringRoll2 == 2) return "TO FICANDO COM SONO...";
                else if (stringRoll2 == 3) return "AI AI... SERÁ QUE TEM ALGO PRA COMER POR AQUI?";
                else if (stringRoll2 == 4) return "QUE LINDA, LINDA. TA LINDA, LINDA.";

                else return ":/";

            case (4):
                return "ABRGLAEBRLUGELRH! É O QUE QUE TA ACONTECENDO?!";

            default:
                return "?";

        }
    }

    private void Awake()
    {
        nuggetScript = GetComponent<Nugget>();
    }

    public void Animate(int state)
    {
        animator.SetInteger("state",state);
    }

    public void NuggetLine(int lineIndex)
    {
        textBox.GetComponentInParent<Animator>().SetBool("on",true);
        StopAllCoroutines();
        StartCoroutine(Typewrite(textLibrary(lineIndex)));
        StartCoroutine(CloseBaloon(textLibrary(lineIndex)));
    }

    IEnumerator Typewrite(string fullText)
    {
        for (int i = 0; i < fullText.Length; i++)
        {
            currentText = fullText.Substring(0, i + 1);
            textBox.text = currentText;

            float pitchRandom = Random.Range(-0.15f, 0.15f);
            audioSource.pitch = 1 + pitchRandom;
            audioSource.PlayOneShot(clip);

            yield return new WaitForSeconds(textDelay);
        }
    }

    IEnumerator CloseBaloon(string line)
    {
        yield return new WaitForSeconds(line.Length * textDelay);

        if (animator.GetInteger("state") != 2 || animator.GetInteger("state") != 3)
        {
            //textBox.GetComponentInParent<Animator>().SetBool("on", false);
            animator.SetInteger("state", 0);
        }
    }

}
