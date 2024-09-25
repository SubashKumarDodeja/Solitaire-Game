using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateSprite : MonoBehaviour
{
    public Sprite cardFace;
    public Sprite cardBack;
    private SpriteRenderer spriteRenderer;
    private Selectable selectable;
    private Solitaire solitaire;
    private UserInput userInput;



    // Start is called before the first frame update
    void Start()
    {
        solitaire = FindObjectOfType<Solitaire>();
        userInput = FindObjectOfType<UserInput>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        selectable = GetComponent<Selectable>();
        cardFace = GetCardFaceByName(this.name);
    }

    private Sprite GetCardFaceByName(string cardName)
    {
        List<string> deck = Solitaire.GenerateDeck();
        int index = deck.IndexOf(cardName);
        return index >= 0 ? solitaire.cardFaces[index] : cardBack; // Default to cardBack if not found
    }

    void Update()
    {
        UpdateSpriteVisibility();
        UpdateSlotColor();
    }

    private void UpdateSpriteVisibility()
    {
        spriteRenderer.sprite = selectable.faceUp ? cardFace : cardBack;
    }

    private void UpdateSlotColor()
    {
        spriteRenderer.color = userInput.slot1 && name == userInput.slot1.name ? Color.yellow : Color.white;
    }
}
