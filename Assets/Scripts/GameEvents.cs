using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameEvents : MonoBehaviour
{
    public static GameEvents current;

    // Set the current singleton.
    private void Awake()
    {
        current = this;
    }

    // What happens if the slot is clicked
    public event Action<int, int> onSlotClicked;
    public void SlotClicked(int row, int col)
    {
        if (onSlotClicked != null)
        {
            onSlotClicked(row, col);
        }
    }

    // What happens if the board is updated
    public event Action<int, int, Turn> onBoardUpdated;
    public void BoardUpdated(int row, int col, Turn player)
    {
        if (onBoardUpdated != null)
        {
            onBoardUpdated(row, col, player);
        }
    }

    // What happens if the turn changes
    public event Action<Turn> onUpdateTurn;
    public void UpdateTurn(Turn player)
    {
        if (onUpdateTurn != null)
        {
            onUpdateTurn(player);
        }
    }

    // What happens if a player wins
    public event Action<Turn> onVictoryAnounce;
    public void VictoryAnounce(Turn player)
    {
        if (onVictoryAnounce != null)
        {
            onVictoryAnounce(player);
        }
    }

}
