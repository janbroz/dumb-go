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

    public event Action<int, int> onSlotClicked;
    public void SlotClicked(int row, int col)
    {
        if (onSlotClicked != null)
        {
            onSlotClicked(row, col);
        }
    }

    public event Action<int, int, Turn> onBoardUpdated;
    public void BoardUpdated(int row, int col, Turn player)
    {
        if (onBoardUpdated != null)
        {
            onBoardUpdated(row, col, player);
        }
    }
}
