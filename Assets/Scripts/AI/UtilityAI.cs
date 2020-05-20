using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This is an initial approach to utility AI.
// We are going to score based on:
// - Do i need to place a piece on the board?
// - Do I need to block the player victory?
// - Do I need to make a play for victory?

public class UtilityAI : AIControllerBase
{
    const int SIZE = 15;
    const int MAX_INDEX = SIZE - 1;
    const int SEARCH_RANGE = 4;

    Board playing_board;

    DangerEvaluation last_evaluation;
    DangerEvaluation last_self_evaluation;

    // AI scorers
    public int placement_score;
    public int attemp_block_sore;
    public int victory_movement_score;

    public override Movement FindMovement(Board board)
    {
        playing_board = board;
        placement_score = ScorePlacement();
        attemp_block_sore = ScoreBlocker();
        victory_movement_score = ScoreVictoryTry();

        if (placement_score > attemp_block_sore && placement_score > victory_movement_score)
        {
            // Place a piece on the table
            return GetPlacingMovement();
        }

        if (attemp_block_sore > placement_score && attemp_block_sore > victory_movement_score)
        {
            // Attempts to block the player
            return GetBlockingMovement();
        }

        if (victory_movement_score > placement_score && victory_movement_score > attemp_block_sore)
        {
            // Push for the victory
            return GetWinningMovement();
        }


        // Why am i here? Do a random move and run away!
        int row = Random.Range(0, MAX_INDEX);
        int col = Random.Range(0, MAX_INDEX);
        Movement movement = new Movement(row, col);

        int row_t = board.last_player_movement.row;
        int col_t = board.last_player_movement.col;


        return movement;
    }

    public int ScoreBlocker()
    {
        DangerEvaluation evaluation = new DangerEvaluation(playing_board.last_player_movement.row, playing_board.last_player_movement.col);

        evaluation = HorizontalDanger(playing_board.last_player_movement.row, playing_board.last_player_movement.col, evaluation, ESlotStatus.Player);
        evaluation = VerticalDanger(playing_board.last_player_movement.row, playing_board.last_player_movement.col, evaluation, ESlotStatus.Player);
        evaluation = DiagonalNegativeDanger(playing_board.last_player_movement.row, playing_board.last_player_movement.col, evaluation, ESlotStatus.Player);
        evaluation = DiagonalPositiveDanger(playing_board.last_player_movement.row, playing_board.last_player_movement.col, evaluation, ESlotStatus.Player);

        last_evaluation = evaluation;

        int total_danger = last_evaluation.MaxRisk();
        bool can_be_blocked = CanBeBlocked(last_evaluation);

        if (!can_be_blocked)
        {
            total_danger -= 100;
        }

        switch (total_danger)
        {
            case 1:
                return 33;
            case 2:
                return 50;
            case 3:
                return 99;
            case 4:
                return 130;
            default:
                return 10;
        }
    }

    Movement GetBlockingMovement()
    {
        int row = last_evaluation.row;
        int col = last_evaluation.col;

        EDirection direction_to_block = last_evaluation.DangerDirection();
        switch (direction_to_block)
        {
            case EDirection.Horizontal:
                // This needs to be extracted and deal with a more complex logic.
                // Its being repeated at make movement
                return GetHorizontalMovement(col, row);
            case EDirection.Vertical:
                if (ValidIndex(row + 1) && playing_board.board[row + 1, col] == ESlotStatus.Empty)
                    return new Movement(row + 1, col);
                if (ValidIndex(row - 1) && playing_board.board[row - 1, col] == ESlotStatus.Empty)
                    return new Movement(row - 1, col);
                break;
            case EDirection.DiagonalNeg:
                if ((ValidIndex(row - 1) && ValidIndex(col - 1)) && playing_board.board[row - 1, col - 1] == ESlotStatus.Empty)
                    return new Movement(row - 1, col - 1);
                if ((ValidIndex(row + 1) && ValidIndex(col + 1)) && playing_board.board[row + 1, col + 1] == ESlotStatus.Empty)
                    return new Movement(row + 1, col + 1);
                break;
            case EDirection.DiagonalPos:
                if ((ValidIndex(row + 1) && ValidIndex(col - 1)) && playing_board.board[row + 1, col - 1] == ESlotStatus.Empty)
                    return new Movement(row + 1, col - 1);
                if ((ValidIndex(row - 1) && ValidIndex(col + 1)) && playing_board.board[row - 1, col + 1] == ESlotStatus.Empty)
                    return new Movement(row - 1, col + 1);
                break;
            case EDirection.ItsAllFine:
                break;
            default:
                break;
        }
        return GetWinningMovement();
    }

    Movement GetHorizontalMovement(int col, int row)
    {
        if (ValidIndex(col + 1) && playing_board.board[row, col + 1] == ESlotStatus.Empty)
            return new Movement(row, col + 1);
        if (ValidIndex(col - 1) && playing_board.board[row, col - 1] == ESlotStatus.Empty)
            return new Movement(row, col - 1);

        return GetWinningMovement();
    }

    Movement GetVerticalMovement(int col, int row)
    {
        if (ValidIndex(row + 1) && playing_board.board[row + 1, col] == ESlotStatus.Empty)
            return new Movement(row + 1, col);
        if (ValidIndex(row - 1) && playing_board.board[row - 1, col] == ESlotStatus.Empty)
            return new Movement(row - 1, col);

        return new Movement(0, 0);
    }

    bool CanBeBlocked(DangerEvaluation evaluation)
    {
        // Need to implement this function :(

        return true;
    }

    Movement GetPlacingMovement()
    {
        // Pending to implement
        int row_mod = Random.Range(-3, 3);
        int col_mod = Random.Range(-3, 3);
        Movement movement = new Movement(SIZE / 2 + row_mod, SIZE / 2 + col_mod);

        return movement;
    }

    Movement GetWinningMovement()
    {
        // What this should do:
        // Iterate over our movements, calculate the danger of the movement and
        // select the one that is going to give us the most for our buck!


        foreach (Movement current_movement in playing_board.ai_movements)
        {
            DangerEvaluation evaluation = EvaluateMovement(current_movement);
            if (last_self_evaluation.MaxRisk() < evaluation.MaxRisk())
                last_self_evaluation = evaluation;
        }

        EDirection direction_to_move = last_self_evaluation.DangerDirection();
        int row = playing_board.last_ai_movement.row;
        int col = playing_board.last_ai_movement.col;
        Movement movement = new Movement(row, col);

        switch (direction_to_move)
        {
            case EDirection.Horizontal:
                // This needs to be extracted and deal with a more complex logic.
                // Its being repeated at make movement
                if (ValidIndex(col + 1) && playing_board.board[row, col + 1] == ESlotStatus.Empty)
                    return new Movement(row, col + 1);
                if (ValidIndex(col - 1) && playing_board.board[row, col - 1] == ESlotStatus.Empty)
                    return new Movement(row, col - 1);
                break;
            case EDirection.Vertical:
                if (ValidIndex(row + 1) && playing_board.board[row + 1, col] == ESlotStatus.Empty)
                    return new Movement(row + 1, col);
                if (ValidIndex(row - 1) && playing_board.board[row - 1, col] == ESlotStatus.Empty)
                    return new Movement(row - 1, col);
                break;
            case EDirection.DiagonalNeg:
                if ((ValidIndex(row - 1) && ValidIndex(col - 1)) && playing_board.board[row - 1, col - 1] == ESlotStatus.Empty)
                    return new Movement(row - 1, col - 1);
                if ((ValidIndex(row + 1) && ValidIndex(col + 1)) && playing_board.board[row + 1, col + 1] == ESlotStatus.Empty)
                    return new Movement(row + 1, col + 1);
                break;
            case EDirection.DiagonalPos:
                if ((ValidIndex(row + 1) && ValidIndex(col - 1)) && playing_board.board[row + 1, col - 1] == ESlotStatus.Empty)
                    return new Movement(row + 1, col - 1);
                if ((ValidIndex(row - 1) && ValidIndex(col + 1)) && playing_board.board[row - 1, col + 1] == ESlotStatus.Empty)
                    return new Movement(row - 1, col + 1);
                break;
            case EDirection.ItsAllFine:
                break;
            default:
                break;
        }

        return GetRandomMovement();
    }

    DangerEvaluation EvaluateMovement(Movement movement)
    {
        DangerEvaluation evaluation = new DangerEvaluation(movement.row, movement.col);

        evaluation = HorizontalDanger(movement.row, movement.col, evaluation, ESlotStatus.Player);
        evaluation = VerticalDanger(movement.row, playing_board.last_player_movement.col, evaluation, ESlotStatus.Player);
        evaluation = DiagonalNegativeDanger(movement.row, movement.col, evaluation, ESlotStatus.Player);
        evaluation = DiagonalPositiveDanger(movement.row, movement.col, evaluation, ESlotStatus.Player);

        return evaluation;
    }

    Movement GetRandomMovement()
    {
        int row = Random.Range(0, MAX_INDEX);
        int col = Random.Range(0, MAX_INDEX);
        Movement movement = new Movement(row, col);

        return movement;
    }

    DangerEvaluation HorizontalDanger(int row, int col, DangerEvaluation evaluation, ESlotStatus status)
    {
        int t_min = col - SEARCH_RANGE;
        int t_max = col + SEARCH_RANGE;
        int min = ValidIndex(t_min) ? t_min : 0;
        int max = ValidIndex(t_max) ? t_max : MAX_INDEX;

        // Evaluate the left side of the piece
        int begin = col - 1;

        int left_danger = 0;
        bool at_corner = begin < 0;
        if (!at_corner)
        {
            for (int i = begin; i >= min; i--)
            {
                if (playing_board.board[row, i] == status)
                    left_danger++;

                if (playing_board.board[row, i] == Enemy(status))
                    break;
            }
        }

        // Evaluate the right side of the piece
        int r_begin = col + 1;
        int right_danger = 0;
        at_corner = r_begin > MAX_INDEX;
        if (!at_corner)
        {
            for (int i = r_begin; i <= max; i++)
            {
                if (playing_board.board[row, i] == status)
                    right_danger++;
                if (playing_board.board[row, i] == Enemy(status))
                    break;
            }
        }
        evaluation.total_horizontal_danger = (left_danger + right_danger + 1);
        evaluation.left_danger = left_danger;
        evaluation.right_danger = right_danger;

        return evaluation;
    }

    DangerEvaluation VerticalDanger(int row, int col, DangerEvaluation evaluation, ESlotStatus status)
    {
        int t_min = row - SEARCH_RANGE;
        int t_max = row + SEARCH_RANGE;
        int min = ValidIndex(t_min) ? t_min : 0;
        int max = ValidIndex(t_max) ? t_max : MAX_INDEX;

        // Evaluate the left side of the piece
        int top = row - 1;
        int top_danger = 0;
        bool at_corner = top < 0;
        if (!at_corner)
        {
            for (int i = top; i >= min; i--)
            {
                if (playing_board.board[i, col] == status)
                    top_danger++;
                if (playing_board.board[i, col] == Enemy(status))
                    break;
            }
        }

        // Evaluate the right side of the piece
        int bot = row + 1;
        int down_danger = 0;
        at_corner = bot > MAX_INDEX;
        if (!at_corner)
        {
            for (int i = bot; i <= max; i++)
            {
                if (playing_board.board[i, col] == status)
                    down_danger++;
                if (playing_board.board[i, col] == Enemy(status))
                    break;
            }
        }

        evaluation.total_vertical_danger = (top_danger + down_danger + 1);
        evaluation.up_danger = top_danger;
        evaluation.down_danger = down_danger;

        return evaluation;
    }

    DangerEvaluation DiagonalNegativeDanger(int row, int col, DangerEvaluation evaluation, ESlotStatus status)
    {
        // Left max
        int y_min = row - SEARCH_RANGE;
        int x_min = col - SEARCH_RANGE;
        int l_y_min = ValidIndex(y_min) ? SEARCH_RANGE : row;
        int l_x_min = ValidIndex(x_min) ? SEARCH_RANGE : col;
        int left_range = Mathf.Min(l_y_min, l_x_min);


        // Do the left side
        bool at_corner = row == 0 || col == 0;
        int top_danger = 0;
        if (!at_corner)
        {
            int new_row = 0;
            int new_col = 0;
            for (int i = 0; i < left_range; i++)
            {
                new_row = row - 1 - i;
                new_col = col - 1 - i;
                if (playing_board.board[new_row, new_col] == status)
                    top_danger++;
                if (playing_board.board[new_row, new_col] == Enemy(status))
                    break;
            }
        }

        int y_max = row + SEARCH_RANGE;
        int x_max = col + SEARCH_RANGE;
        int l_y_max = ValidIndex(y_max) ? SEARCH_RANGE : MAX_INDEX - row;
        int l_x_max = ValidIndex(x_max) ? SEARCH_RANGE : MAX_INDEX - col;
        int rigth_range = Mathf.Min(l_y_max, l_x_max);

        at_corner = row == MAX_INDEX || col == MAX_INDEX;
        // Do the right side
        int down_danger = 0;
        if (!at_corner)
        {
            int new_row = 0;
            int new_col = 0;
            for (int i = 0; i < rigth_range; i++)
            {
                new_row = row + 1 + i;
                new_col = col + 1 + i;
                if (playing_board.board[new_row, new_col] == status)
                    down_danger++;
                if (playing_board.board[new_row, new_col] == Enemy(status))
                    break;
            }
        }

        evaluation.total_diagonal_negative_danger = (top_danger + down_danger + 1);
        evaluation.up_danger = top_danger;
        evaluation.down_danger = down_danger;

        return evaluation;
    }

    DangerEvaluation DiagonalPositiveDanger(int row, int col, DangerEvaluation evaluation, ESlotStatus status)
    {
        // Left max
        int y_min = row + SEARCH_RANGE;
        int x_min = col - SEARCH_RANGE;
        int l_y_min = ValidIndex(y_min) ? SEARCH_RANGE : MAX_INDEX - row;
        int l_x_min = ValidIndex(x_min) ? SEARCH_RANGE : col;
        int left_range = Mathf.Min(l_y_min, l_x_min);


        // Do the left side
        bool at_corner = row == MAX_INDEX || col == 0;
        int top_danger = 0;
        if (!at_corner)
        {
            int new_row = 0;
            int new_col = 0;
            for (int i = 0; i < left_range; i++)
            {
                new_row = row + 1 + i;
                new_col = col - 1 - i;
                if (playing_board.board[new_row, new_col] == status)
                    top_danger++;
                if (playing_board.board[new_row, new_col] == Enemy(status))
                    break;
            }
        }

        int y_max = row - SEARCH_RANGE;
        int x_max = col + SEARCH_RANGE;
        int l_y_max = ValidIndex(y_max) ? SEARCH_RANGE : row;
        int l_x_max = ValidIndex(x_max) ? SEARCH_RANGE : MAX_INDEX - col;
        int rigth_range = Mathf.Min(l_y_max, l_x_max);

        at_corner = row == 0 || col == MAX_INDEX;
        // Do the right side
        int down_danger = 0;
        if (!at_corner)
        {
            int new_row = 0;
            int new_col = 0;
            for (int i = 0; i < rigth_range; i++)
            {
                new_row = row - 1 - i;
                new_col = col + 1 + i;
                if (playing_board.board[new_row, new_col] == status)
                    down_danger++;
                if (playing_board.board[new_row, new_col] == Enemy(status))
                    break;
            }
        }

        evaluation.total_diagonal_positive_danger = (top_danger + down_danger + 1);
        evaluation.up_danger = top_danger;
        evaluation.down_danger = down_danger;

        return evaluation;
    }


    bool ValidIndex(int i)
    {
        return (i >= 0 && i <= MAX_INDEX);
    }


    public int ScorePlacement()
    {
        if (playing_board.ai_movements.Count < 1)
            return 200;

        return 10;
    }



    public int ScoreVictoryTry()
    {
        // This is going to do the same danger(close to victory) for our last movement
        DangerEvaluation evaluation = new DangerEvaluation(playing_board.last_ai_movement.row, playing_board.last_ai_movement.col);

        evaluation = HorizontalDanger(playing_board.last_ai_movement.row, playing_board.last_ai_movement.col, evaluation, ESlotStatus.AI);
        evaluation = VerticalDanger(playing_board.last_ai_movement.row, playing_board.last_ai_movement.col, evaluation, ESlotStatus.AI);
        evaluation = DiagonalNegativeDanger(playing_board.last_ai_movement.row, playing_board.last_ai_movement.col, evaluation, ESlotStatus.AI);
        evaluation = DiagonalPositiveDanger(playing_board.last_ai_movement.row, playing_board.last_ai_movement.col, evaluation, ESlotStatus.AI);
        last_self_evaluation = evaluation;

        int risk = last_self_evaluation.MaxRisk();

        switch (risk)
        {
            case 1:
                return 60;
            case 2:
                return 80;
            case 3:
                return 110;
            case 4:
                return 200;
            default:
                break;
        }

        return 0;
    }

    ESlotStatus Enemy(ESlotStatus status)
    {
        if (status == ESlotStatus.Player)
            return ESlotStatus.AI;
        else
            return ESlotStatus.Player;
    }
}
