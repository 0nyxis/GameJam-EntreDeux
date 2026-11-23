using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI texteUI;
    public List<string> dialogue;

    public float vitesse = 0.05f;
    public float tempEntreDialogue = 1.5f;
    public bool onePlay = true;
    private bool alreadyPlay = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (onePlay && alreadyPlay) return;

            alreadyPlay = true;
            StopAllCoroutines();
            StartCoroutine(PlayDialogue());
        }
    }

    private IEnumerator PlayDialogue()
    {
        foreach (string phrase in dialogue)
        {
            texteUI.text = phrase;
            texteUI.maxVisibleCharacters = 0;

            yield return null;

            int totalCaracteres = texteUI.textInfo.characterCount;

            for (int i = 0; i <= totalCaracteres; i++)
            {
                texteUI.maxVisibleCharacters = i;
                yield return new WaitForSeconds(vitesse);
            }

            yield return new WaitForSeconds(tempEntreDialogue);

        }

        texteUI.text = "";
        texteUI.gameObject.SetActive(false);
    }
}
