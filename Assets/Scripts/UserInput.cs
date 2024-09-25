using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UserInput : MonoBehaviour
{
    public GameObject slot1;
    private Solitaire solitaire;
    private float timer;
    private const float doubleClickTime = 0.3f;
    private int clickCount = 0;

    void Start()
    {
        solitaire = FindObjectOfType<Solitaire>();
        slot1 = this.gameObject;
    }

    void Update()
    {
        HandleDoubleClick();
        GetMouseClick();
    }

    void HandleDoubleClick()
    {
        if (clickCount == 1)
        {
            timer += Time.deltaTime;
        }
        
        if (timer > doubleClickTime)
        {
            ResetClickCount();
        }
    }

    void ResetClickCount()
    {
        timer = 0;
        clickCount = 0;
    }

    void GetMouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            clickCount++;
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -10));
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (hit)
            {
                HandleHit(hit.collider.gameObject);
            }
        }
    }

    void HandleHit(GameObject hitObject)
    {
        if (hitObject.CompareTag("Deck"))
        {
            Deck();
        }
        else if (hitObject.CompareTag("Card"))
        {
            Card(hitObject);
        }
        else if (hitObject.CompareTag("Top"))
        {
            Top(hitObject);
        }
        else if (hitObject.CompareTag("Bottom"))
        {
            Bottom(hitObject);
        }
    }

    void Deck()
    {
        Debug.Log("Clicked on deck");
        solitaire.DealFromDeck();
        slot1 = this.gameObject;
    }

    void Card(GameObject selected)
    {
        Debug.Log("Clicked on Card");

        Selectable selectedComponent = selected.GetComponent<Selectable>();
        if (!selectedComponent.faceUp)
        {
            if (!Blocked(selected))
            {
                selectedComponent.faceUp = true;
                slot1 = this.gameObject;
            }
            return;
        }

        if (selectedComponent.inDeckPile && !Blocked(selected))
        {
            if (slot1 == selected)
            {
                if (DoubleClick())
                {
                    AutoStack(selected);
                }
            }
            else
            {
                slot1 = selected;
            }
            return;
        }

        HandleSelectedCard(selected);
    }

    void HandleSelectedCard(GameObject selected)
    {
        if (slot1 == this.gameObject)
        {
            slot1 = selected;
            return;
        }

        if (slot1 != selected)
        {
            if (Stackable(selected))
            {
                Stack(selected);
            }
            else
            {
                slot1 = selected;
            }
        }
        else if (DoubleClick())
        {
            AutoStack(selected);
        }
    }

    void Top(GameObject selected)
    {
        Debug.Log("Clicked on Top");
        if (slot1.CompareTag("Card") && slot1.GetComponent<Selectable>().value == 1)
        {
            Stack(selected);
        }
    }

    void Bottom(GameObject selected)
    {
        Debug.Log("Clicked on Bottom");
        if (slot1.CompareTag("Card") && slot1.GetComponent<Selectable>().value == 13)
        {
            Stack(selected);
        }
    }

    bool Stackable(GameObject selected)
    {
        Selectable s1 = slot1.GetComponent<Selectable>();
        Selectable s2 = selected.GetComponent<Selectable>();

        if (!s2.inDeckPile)
        {
            if (s2.top)
            {
                return (s1.suit == s2.suit || (s1.value == 1 && s2.suit == null)) && s1.value == s2.value + 1;
            }
            else
            {
                if (s1.value == s2.value - 1)
                {
                    bool card1Red = s1.suit == "H" || s1.suit == "D";
                    bool card2Red = s2.suit == "H" || s2.suit == "D";
                    return card1Red != card2Red; // True if they are alternate colors
                }
            }
        }
        return false;
    }

    void Stack(GameObject selected)
    {
        Selectable s1 = slot1.GetComponent<Selectable>();
        Selectable s2 = selected.GetComponent<Selectable>();
        float yOffset = s2.top || (s1.value == 13) ? 0 : 0.3f;

        slot1.transform.position = new Vector3(selected.transform.position.x, selected.transform.position.y - yOffset, selected.transform.position.z - 0.01f);
        slot1.transform.SetParent(selected.transform);

        UpdateCardTracking(s1, s2);
        ResetSlot1();
    }

    void UpdateCardTracking(Selectable s1, Selectable s2)
    {
        if (s1.inDeckPile)
        {
            solitaire.tripsOnDisplay.Remove(slot1.name);
        }
        else if (s1.top && s2.top && s1.value == 1)
        {
            solitaire.topPos[s1.row].GetComponent<Selectable>().value = 0;
            solitaire.topPos[s1.row].GetComponent<Selectable>().suit = null;
        }
        else if (s1.top)
        {
            solitaire.topPos[s1.row].GetComponent<Selectable>().value = s1.value - 1;
        }
        else
        {
            solitaire.bottoms[s1.row].Remove(slot1.name);
        }

        s1.inDeckPile = false;
        s1.row = s2.row;

        if (s2.top)
        {
            solitaire.topPos[s1.row].GetComponent<Selectable>().value = s1.value;
            solitaire.topPos[s1.row].GetComponent<Selectable>().suit = s1.suit;
            s1.top = true;
        }
        else
        {
            s1.top = false;
        }
    }

    void ResetSlot1()
    {
        slot1 = this.gameObject;
    }

    bool Blocked(GameObject selected)
    {
        Selectable s2 = selected.GetComponent<Selectable>();
        if (s2.inDeckPile)
        {
            return s2.name != solitaire.tripsOnDisplay.Last();
        }
        return s2.name != solitaire.bottoms[s2.row].Last();
    }

    bool DoubleClick()
    {
        if (timer < doubleClickTime && clickCount == 2)
        {
            Debug.Log("Double Click");
            return true;
        }
        return false;
    }

    void AutoStack(GameObject selected)
    {
        for (int i = 0; i < solitaire.topPos.Length; i++)
        {
            Selectable stack = solitaire.topPos[i].GetComponent<Selectable>();
            if (selected.GetComponent<Selectable>().value == 1)
            {
                if (stack.value == 0)
                {
                    slot1 = selected;
                    Stack(stack.gameObject);
                    break;
                }
            }
            else
            {
                if (stack.suit == slot1.GetComponent<Selectable>().suit && stack.value == slot1.GetComponent<Selectable>().value - 1 && HasNoChildren(slot1))
                {
                    slot1 = selected;
                    Stack(GameObject.Find(GetCardName(stack)));
                    break;
                }
            }
        }
    }

    string GetCardName(Selectable stack)
    {
        string lastCardname = $"{stack.suit}{stack.value}";
        if (stack.value == 1) return $"{stack.suit}A";
        if (stack.value == 11) return $"{stack.suit}J";
        if (stack.value == 12) return $"{stack.suit}Q";
        if (stack.value == 13) return $"{stack.suit}K";
        return lastCardname;
    }

    bool HasNoChildren(GameObject card)
    {
        return card.transform.childCount == 0;
    }
}
