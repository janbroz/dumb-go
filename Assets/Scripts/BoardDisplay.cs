using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardDisplay : MonoBehaviour
{
    public GridLayoutGroup slot_grid;
    public GameObject slot_prefab;

    // Start is called before the first frame update
    void Start()
    {
        InitializeGrid();

        GameEvents.current.onBoardUpdated += OnBoardUpdated;
    }

    // Update is called once per frame
    void InitializeGrid()
    {
        if (slot_grid && slot_prefab)
        {
            for (int i = 0; i < 225; i++)
            {
                int col = i / 15;
                int row = i % 15;

                GameObject go = Instantiate(slot_prefab);
                go.transform.SetParent(slot_grid.transform);
                go.transform.localScale = new Vector3(1, 1, 1);
                Slot slot = go.GetComponent<Slot>();
                slot.pos_x = col;
                slot.pos_y = row;
            }
        }
    }

    void OnBoardUpdated(int row, int col, Turn player)
    {

    }
}
