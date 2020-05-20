using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIControllerBase : MonoBehaviour
{

    public abstract Movement FindMovement(Board board);

}

public struct DangerEvaluation
{
    public DangerEvaluation(int i, int j)
    {
        row = i;
        col = j;

        total_horizontal_danger = 0;
        total_vertical_danger = 0;
        left_danger = 0;
        right_danger = 0;
        up_danger = 0;
        down_danger = 0;
        total_diagonal_negative_danger = 0;
        top_diagonal_neg_danger = 0;
        down_diagonal_neg_danger = 0;
        total_diagonal_positive_danger = 0;
        top_diagonal_pos_danger = 0;
        down_diagonal_pos_danger = 0;
    }

    public int row;
    public int col;
    public int total_horizontal_danger;
    public int left_danger;
    public int right_danger;
    public int total_vertical_danger;
    public int up_danger;
    public int down_danger;
    public int total_diagonal_negative_danger;
    public int top_diagonal_neg_danger;
    public int down_diagonal_neg_danger;
    public int total_diagonal_positive_danger;
    public int top_diagonal_pos_danger;
    public int down_diagonal_pos_danger;

    public int MaxRisk()
    {
        return Mathf.Max(total_horizontal_danger, total_vertical_danger, total_diagonal_negative_danger, total_diagonal_positive_danger);
    }

    public EDirection DangerDirection()
    {
        if (total_horizontal_danger >= total_vertical_danger && total_horizontal_danger >= total_diagonal_negative_danger && total_horizontal_danger >= total_diagonal_positive_danger)
            return EDirection.Horizontal;
        if (total_vertical_danger >= total_horizontal_danger && total_vertical_danger >= total_diagonal_negative_danger && total_vertical_danger >= total_diagonal_positive_danger)
            return EDirection.Vertical;
        if (total_diagonal_negative_danger >= total_vertical_danger && total_diagonal_negative_danger >= total_horizontal_danger && total_diagonal_negative_danger >= total_diagonal_positive_danger)
            return EDirection.DiagonalNeg;
        if (total_diagonal_positive_danger >= total_vertical_danger && total_diagonal_positive_danger >= total_horizontal_danger && total_diagonal_positive_danger >= total_diagonal_negative_danger)
            return EDirection.DiagonalPos;
        return EDirection.ItsAllFine;
    }
}

public enum EDirection
{
    Horizontal,
    Vertical,
    DiagonalNeg,
    DiagonalPos,
    ItsAllFine
}