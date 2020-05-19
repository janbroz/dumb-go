using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    // Start is called before the first frame update
    public int pos_x;
    public int pos_y;
    bool is_used = false;

    public Image icon;
    public Sprite player_sprite;
    public Sprite ai_sprite;

    void Start()
    {
        GameEvents.current.onBoardUpdated += OnUpdateSlotUI;
    }

    public void OnUpdateSlotUI(int row, int col, Turn playing_turn)
    {
        if(pos_x == row && pos_y == col)
        {
            is_used = true;

            if (playing_turn == Turn.Player)
            {
                icon.sprite = player_sprite;
            }
            else
            {
                icon.sprite = ai_sprite;
            }
        }
    }

    private void OnSlotClicked()
    {
        GameEvents.current.SlotClicked(pos_x, pos_y);
    }
    
}
