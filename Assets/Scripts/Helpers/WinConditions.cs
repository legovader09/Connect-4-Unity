using System;
using UnityEngine;

public class WinConditions : MonoBehaviour
{
    Vector2Int coords;

    internal bool checkWinConditions(Vector2Int checkerCoords)
    {
        coords = checkerCoords;
        if (GameObject.Find((coords.x).ToString() + "," + (coords.y).ToString()).GetComponent<SpriteRenderer>().color == Color.red)
            return false;

        return (checkSideWin() ||
                checkUpDownWin() ||
                checkDiagUpWin() ||
                checkDiagDownWin());
    }

    private bool checkDiagDownWin()
    {
        if (coords.y > 3)
            return false;

        if (coords.x > 4)
            return false;

        for (int i = 1; i < 4; i++)
        {
            var g = GameObject.Find((coords.x + i).ToString() + "," + (coords.y + i).ToString());
            if (!g.GetComponent<CheckerHelper>().isChecked)
                return false;

            if (g.GetComponent<SpriteRenderer>().color == Color.red)
                return false;
        }
            
        return true;
    }

    private bool checkDiagUpWin()
    {
        if (coords.y < 3)
            return false;

        if (coords.x > 4)
            return false;

        for (int i = 1; i < 4; i++)
        {
            var g = GameObject.Find((coords.x + i).ToString() + "," + (coords.y - i).ToString());
            if (!g.GetComponent<CheckerHelper>().isChecked)
                return false;

            if (g.GetComponent<SpriteRenderer>().color == Color.red)
                return false;
        }
            
        return true;
    }

    private bool checkUpDownWin()
    {
        if (coords.y > 3)
            return false;

        for (int i = 1; i < 4; i++)
        {
            var g = GameObject.Find((coords.x).ToString() + "," + (coords.y + i).ToString());
            if (!g.GetComponent<CheckerHelper>().isChecked)
                return false;

            if (g.GetComponent<SpriteRenderer>().color == Color.red)
                return false;
        }
            
        return true;
    }

    private bool checkSideWin()
    {
        if (coords.x > 4)
            return false;

        for (int i = 1; i < 4; i++)
        {
            var g = GameObject.Find((coords.x + i).ToString() + "," + (coords.y).ToString());
            if (!g.GetComponent<CheckerHelper>().isChecked)
                return false;

            if (g.GetComponent<SpriteRenderer>().color == Color.red)
                return false;
        }
            
        return true;
    }
}