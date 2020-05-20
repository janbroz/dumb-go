using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BoardDisplay : MonoBehaviour
{
    public GridLayoutGroup slot_grid;
    public GameObject slot_prefab;
    public GameObject victory_panel;
    public TextMeshProUGUI turn_display;

    // Start is called before the first frame update
    void Start()
    {
        InitializeGrid();
        GameEvents.current.onUpdateTurn += OnUpdateTurn;
        GameEvents.current.onVictoryAnounce += OnVictoryAnounce;
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

    void OnUpdateTurn(Turn player)
    {
        if(player == Turn.Player)
        {
            turn_display.text = "Human";
        }
        else
        {
            turn_display.text = "Mr. Roboto";
        }
    }

    void OnVictoryAnounce(Turn player)
    {
        if (player == Turn.Player)
        {
            turn_display.text = "Human victory";
        }
        else
        {
            turn_display.text = "Mr. Roboto victory";
        }
        victory_panel.gameObject.SetActive(true);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
