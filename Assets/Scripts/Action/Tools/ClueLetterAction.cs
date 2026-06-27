using System;
using System.Collections.Generic;

using UnityEngine;
using UHorizontalLayoutGroup = UnityEngine.UI.HorizontalLayoutGroup;
using ULayoutElement = UnityEngine.UI.LayoutElement;

using Leap.UI.Elements;

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
    private List<Text> txtLetters = new List <Text>();

    private void Start()
    {
        lettersContainer.GetComponent<UHorizontalLayoutGroup>().spacing = spacing;
    }

    public void DisplayWord(String word)
    {
        currentWord = word.ToUpper();

        foreach (Transform child in lettersContainer)
            Destroy(child.gameObject);

        txtLetters.Clear();

        for (int i = 0; i < currentWord.Length; i++)
        {
            GameObject item = Instantiate(letterPrefab, lettersContainer);

            Text txtLetter = item.GetComponentInChildren<Text>();
            txtLetter.TextValue = "";
            txtLetters.Add(txtLetter);

            item.GetComponent<ULayoutElement>().flexibleWidth = 1;
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