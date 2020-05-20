using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAI : AIControllerBase
{

    public override Movement FindMovement(Board board)
    {
        int row = Random.Range(0, 14);
        int col = Random.Range(0, 14);

        return new Movement(row, col);
    }

}
