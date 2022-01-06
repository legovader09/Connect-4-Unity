using PlayerIOClient;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameHelper : MonoBehaviour
{
    [SerializeField] private GameObject btnRestart;
    private List<Message> msgList = new List<Message>();
    internal bool isPlayerTurn = false;
    private string winner = "";
    internal GameObject lastCheckerPlaced = null;

    private Color white;

    void Start()
    {
        //sample colour of the original tile.
        white = GameObject.Find("1,1").GetComponent<SpriteRenderer>().color;

        if (Session.playerConn != null)
        {
            Session.playerConn.OnMessage += handleMessage;
            Session.playerConn.Send("CheckStartingPlayer");
            if (!Session.isHost)
            {
                GameObject.Find("txtTurn").GetComponent<TextMeshProUGUI>().text = $"{PlayerPrefs.GetString("OppName")}'s Turn";
                GameObject.Find("txtTurn").GetComponent<TextMeshProUGUI>().color = Color.red;
            }
        }
        else
        {
            isPlayerTurn = true;
            GameObject.Find("txtTurn").GetComponent<TextMeshProUGUI>().text = $"Your Turn";
        }
    }

    void FixedUpdate()
    {
        // process message queue
        foreach (Message m in msgList)
        {
            switch (m.Type)
            {
                case "PlayerLeft":
                    Session.playerConn.Disconnect();
                    SceneManager.LoadScene("TitleScene");
                    break;
                case "UpdatePiece":
                    var c = GameObject.Find(m.GetInt(0) + "," + m.GetInt(1));
                    c.GetComponent<CheckerHelper>().isChecked = true;
                    if (lastCheckerPlaced == null)
                        c.GetComponent<SpriteRenderer>().color = Color.red;
                    else
                    {
                        lastCheckerPlaced.GetComponent<SpriteRenderer>().color = Color.red;
                        c.GetComponent<SpriteRenderer>().color = Color.cyan;
                    }
                    lastCheckerPlaced = c;
                    isPlayerTurn = true;
                    GameObject.Find("txtTurn").GetComponent<TextMeshProUGUI>().text = $"{PlayerPrefs.GetString("Username")}'s Turn";
                    GameObject.Find("txtTurn").GetComponent<TextMeshProUGUI>().color = Color.yellow;
                    break;
                case "StartingPlayer":
                    isPlayerTurn = true; 
                    GameObject.Find("txtTurn").GetComponent<TextMeshProUGUI>().text = $"{PlayerPrefs.GetString("Username")}'s Turn";
                    GameObject.Find("txtTurn").GetComponent<TextMeshProUGUI>().color = Color.yellow;
                    break;
                case "PlayerWin":
                    isPlayerTurn = false;
                    GameObject.Find("txtTurn").GetComponent<TextMeshProUGUI>().text = $"{m.GetString(0)} Wins!";
                    GameObject.Find("txtTurn").GetComponent<TextMeshProUGUI>().color = Color.red;
                    winner = m.GetString(0);
                    if (Session.isHost)
                        btnRestart.SetActive(true);
                    break;
                case "Restart":
                    lastCheckerPlaced = null;
                    foreach (GameObject g in GameObject.FindGameObjectsWithTag("Checker"))
                    {
                        if (g.GetComponent<CheckerHelper>().isChecked)
                        {
                            g.GetComponent<CheckerHelper>().isChecked = false;
                            g.GetComponent<SpriteRenderer>().color = white;
                        }
                    }
                    isPlayerTurn = !(PlayerPrefs.GetString("Username") == winner);
                    string s = isPlayerTurn ? PlayerPrefs.GetString("Username") : PlayerPrefs.GetString("OppName");
                    GameObject.Find("txtTurn").GetComponent<TextMeshProUGUI>().text = $"{s}'s Turn";
                    GameObject.Find("txtTurn").GetComponent<TextMeshProUGUI>().color = isPlayerTurn ? Color.yellow : Color.red;
                    winner = "";
                    btnRestart.SetActive(false);
                    break;
            }
        }

        // clear message queue after it's been processed
        msgList.Clear();
    }

    void handleMessage(object sender, Message m)
    {
        msgList.Add(m);
    }

    /// <summary>
    /// Sends the player's move to the game server.
    /// </summary>
    /// <param name="x">X-coordinate of checker.</param>
    /// <param name="y">Y-coordinate of checker.</param>
    internal void sendMove(int x, int y)
    {
        Session.playerConn.Send("SetPiece", x, y);
        GameObject.Find("txtTurn").GetComponent<TextMeshProUGUI>().text = $"{PlayerPrefs.GetString("OppName")}'s Turn";
        GameObject.Find("txtTurn").GetComponent<TextMeshProUGUI>().color = Color.red;

        // visually updates the last placed opponent checker back to red.
        if (lastCheckerPlaced != null) lastCheckerPlaced.GetComponent<SpriteRenderer>().color = Color.red;
    }


    /// <summary>
    /// Loop through the gameboard to check for a win.
    /// </summary>
    internal void checkWin()
    {
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Checker"))
        {
            if (!g.GetComponent<CheckerHelper>().isChecked) continue;

            if (g.GetComponent<WinConditions>().checkWinConditions(g.GetComponent<CheckerHelper>().coords)) handleWin();
        }
    }

    /// <summary>
    /// Visually show that the player has won, also broadcasts the win to the game server.
    /// </summary>
    private void handleWin()
    {
        Debug.Log("Win!");
        GameObject.Find("txtTurn").GetComponent<TextMeshProUGUI>().text = $"{PlayerPrefs.GetString("Username")} Wins!";
        winner = PlayerPrefs.GetString("Username");
        Session.playerConn.Send("PlayerWin", winner);
        if (Session.isHost)
            btnRestart.SetActive(true);
    }

    public void Leave()
    {
        SceneManager.LoadScene("TitleScene");
        if (Session.playerConn != null) Session.playerConn.Disconnect();
    }

    public void Restart()
    {
        if (Session.playerConn != null) Session.playerConn.Send("RestartGame");
    }
}