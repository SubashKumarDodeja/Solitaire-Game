using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreKeeper : MonoBehaviour
{
    public Selectable[] topStacks;  
    public GameObject highScorePanel; 

    void Update()
    {
        if (HasWon())
        {
            Win();
        }
    }

    public bool HasWon()
    {
        int totalValue = 0; 

        
        foreach (Selectable topStack in topStacks)
        {
            totalValue += topStack.value;
        }
        return totalValue >= 52;
    }

    void Win()
    {
        highScorePanel.SetActive(true); 
        Debug.Log("You have won!"); // 
    }
}
