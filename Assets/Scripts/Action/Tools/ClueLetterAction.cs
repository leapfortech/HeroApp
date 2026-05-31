using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class ClueLetterAction : MonoBehaviour
{
    [Header("UI")]
    [SerializeField]
    RectTransform lettersContainer;

    [SerializeField]
    GameObject letterPrefab;

    [Header("Layout")]
    [SerializeField]
    float spacing = 10f;

    private String currentWord = "";
    private List<Leap.UI.Elements.Text> txtLetters = new List <Leap.UI.Elements.Text>();

    
    public void DisplayWord(String word)
    {
        currentWord = word.ToUpper();

        foreach (Transform child in lettersContainer)
            Destroy(child.gameObject);

        txtLetters.Clear();

        HorizontalLayoutGroup layout = lettersContainer.GetComponent<HorizontalLayoutGroup>();

        if (layout != null)
            layout.spacing = spacing;

        for (int i = 0; i < currentWord.Length; i++)
        {
            GameObject item = Instantiate(letterPrefab, lettersContainer);

            Leap.UI.Elements.Text txtLetter = item.GetComponentInChildren<Leap.UI.Elements.Text>();

            if (txtLetter != null)
            {
                txtLetter.TextValue = "";
                txtLetters.Add(txtLetter);
            }

            LayoutElement layoutElement = item.GetComponent<LayoutElement>();

            if (layoutElement != null)
                layoutElement.flexibleWidth = 1;
        }
    }

    public void RevealLetter(int index)
    {
        if (index < 0 || index >= currentWord.Length)
            return;

        txtLetters[index].TextValue = currentWord[index].ToString();
    }

    public void RevealLetter(int index, char letter)
    {
        if (index < 0 || index >= txtLetters.Count)
            return;

        txtLetters[index].TextValue = letter.ToString();
    }

    public void RevealAll()
    {
        for (int i = 0; i < currentWord.Length; i++)
            txtLetters[i].TextValue = currentWord[i].ToString();
    }

    public void RevealRandomLetter()
    {
        List<int> hiddenIndexes = new List<int>();

        for (int i = 0; i < txtLetters.Count; i++)
        {
            if (String.IsNullOrEmpty(txtLetters[i].TextValue))
                hiddenIndexes.Add(i);
        }

        if (hiddenIndexes.Count == 0)
            return;

        int randomIndex = hiddenIndexes[UnityEngine.Random.Range(0, hiddenIndexes.Count)];

        txtLetters[randomIndex].TextValue = currentWord[randomIndex].ToString();
    }
}