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

        bool victory = ValidateVictory(row, col);

        if (victory)
        {
            is_game_active = false;
            GameEvents.current.VictoryAnounce(current_turn);
            return;
        }

        //UpdateTurn();
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

    bool ValidateVictory(int row, int col)
    {
        if (HorizontalCheck(row, col))
            return true;

        if (VerticalCheck(row, col))
            return true;

        if (DiagonalPosCheck(row, col))
            return true;

        if (DiagonalNegCheck(row, col))
            return true;

        return false;
    }

    bool HorizontalCheck(int row, int col)
    {
        int begin = col - RANGE > 0 ? col - RANGE : 0;
        int end = col + RANGE <= MAX_INDEX ? col + RANGE : MAX_INDEX;
        int points = 0;
        int max = 0;
        ESlotStatus status = current_turn == Turn.Player ? ESlotStatus.Player : ESlotStatus.AI;

        for (int i = begin; i <= end; i++)
        {
            if (board[row, i] == status)
            {
                points++;
                max = Mathf.Max(max, points);
            }
            else
            {
                max = Mathf.Max(max, points);
                points = 0;
            }
        }
        return (max >= 5);
    }

    bool VerticalCheck(int row, int col)
    {
        int begin = row - RANGE > 0 ? row - RANGE : 0;
        int end = row + RANGE <= MAX_INDEX ? row + RANGE : MAX_INDEX;
        int points = 0;
        int max = 0;

        ESlotStatus status = current_turn == Turn.Player ? ESlotStatus.Player : ESlotStatus.AI;

        for (int i = begin; i <= end; i++)
        {
            if (board[i, col] == status)
            {
                points++;
                max = Mathf.Max(max, points);
            }
            else
            {
                max = Mathf.Max(max, points);
                points = 0;
            }
        }

        return (max >= 5);
    }

    bool DiagonalNegCheck(int row, int col)
    {
        int topX = col - RANGE >= 0 ? RANGE : col;
        int topY = row - RANGE >= 0 ? RANGE : row;
        int botX = col + RANGE <= MAX_INDEX ? RANGE : MAX_INDEX - col;
        int botY = row + RANGE <= MAX_INDEX ? RANGE : MAX_INDEX - row;

        int pos = Mathf.Min(topX, topY);
        int neg = Mathf.Min(botX, botY);

        ESlotStatus status = current_turn == Turn.Player ? ESlotStatus.Player : ESlotStatus.AI;
        int points = 0;
        int max = 0;

        for (int i = 0; i <= pos + neg; i++)
        {
            if (board[row - pos + i, col - pos + i] == status)
            {
                points++;
                max = Mathf.Max(max, points);
            }
            else
            {
                max = Mathf.Max(max, points);
                points = 0;
            }
        }

        return max >= 5;
    }

    bool DiagonalPosCheck(int row, int col)
    {
        int botX = col - RANGE >= 0 ? RANGE : col;
        int botY = row + RANGE <= MAX_INDEX ? RANGE : MAX_INDEX - row;
        int topX = col + RANGE <= MAX_INDEX ? RANGE : MAX_INDEX - col;
        int topY = row - RANGE >= 0 ? RANGE : row;

        int pos = Mathf.Min(botX, botY);
        int neg = Mathf.Min(topX, topY);

        ESlotStatus status = current_turn == Turn.Player ? ESlotStatus.Player : ESlotStatus.AI;
        int points = 0;
        int max = 0;

        for (int i = 0; i <= pos + neg; i++)
        {
            if (board[row + pos - i, col - pos + i] == status)
            {
                points++;
                max = Mathf.Max(max, points);
            }
            else
            {
                max = Mathf.Max(max, points);
                points = 0;
            }
        }

        return max >= 5;
    }

    void UpdateTurn()
    {
        current_turn = current_turn == Turn.Player ? Turn.AI : Turn.Player;
        GameEvents.current.UpdateTurn(current_turn);
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