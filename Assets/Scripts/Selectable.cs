using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selectable : MonoBehaviour
{
    public bool top = false;
    public string suit;
    public int value;
    public int row;
    public bool faceUp = false;
    public bool inDeckPile = false;

    void Start()
    {
        if (CompareTag("Card"))
        {
            InitializeCard();
        }
    }
    private void InitializeCard()
    {
        suit = transform.name[0].ToString(); 
        string valueString = transform.name.Substring(1); 
        value = GetCardValue(valueString); 
    }

    private int GetCardValue(string valueString)
    {
        switch (valueString)
        {
            case "A": return 1;
            case "2": return 2;
            case "3": return 3;
            case "4": return 4;
            case "5": return 5;
            case "6": return 6;
            case "7": return 7;
            case "8": return 8;
            case "9": return 9;
            case "10": return 10;
            case "J": return 11;
            case "Q": return 12;
            case "K": return 13;
            default: return 0; 
        }
    }
}
