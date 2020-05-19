using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{

    public const int WIDTH = 15;
    public const int HEIGTH = 15;
    public const int MAX_INDEX = WIDTH - 1;
    public const int RANGE = 4;

    // Board details
    public ESlotStatus[,] board;
    public int missing_slots = WIDTH * HEIGTH;
    public Turn current_turn;

    public ArrayList player_movements;
    public ArrayList ai_movements;
    public Movement last_player_movement;
    public Movement last_ai_movement;

    bool is_game_active = true;

    // Start is called before the first frame update
    void Start()
    {
        InitializeBoard();
        InitializeUI();

        GameEvents.current.onSlotClicked += OnSlotClicked;
    }

    void InitializeBoard()
    {
        board = new ESlotStatus[WIDTH, HEIGTH];
        for (int i = 0; i < WIDTH; i++)
        {
            for (int j = 0; j < HEIGTH; j++)
            {
                board[i, j] = ESlotStatus.Empty;
            }
        }
        current_turn = Turn.Player;
        player_movements = new ArrayList();
        ai_movements = new ArrayList();
    }

    void InitializeUI()
    {

    }

    void OnSlotClicked(int row, int col)
    {
        if(current_turn == Turn.Player && ValidPlay(row, col))
        {
            MakePlay(row, col);
        }
    }

    bool ValidPlay(int row, int col)
    {
        if (board[row, col] != ESlotStatus.Empty)
            return false;
        else
            return true;
    }

    void MakePlay(int row, int col)
    {
        if (!is_game_active)
            return;

        UpdateBoard(row, col);
        missing_slots--;

    }

    void UpdateBoard(int row, int col)
    {
        if(current_turn == Turn.Player)
        {
            board[row, col] = ESlotStatus.Player;
            last_player_movement = new Movement(row, col);
            player_movements.Add(last_player_movement);
        }
        else
        {
            board[row, col] = ESlotStatus.AI;
            last_ai_movement = new Movement(row, col);
            ai_movements.Add(last_ai_movement);
        }

        GameEvents.current.BoardUpdated(row, col, current_turn);
    }
}


public enum Turn
{
    Player,
    AI
}

public enum ESlotStatus
{
    Empty,
    Player,
    AI
}

public struct Movement
{
    public Movement(int i, int j)
    {
        row = i;
        col = j;
    }

    public int row;
    public int col;
}