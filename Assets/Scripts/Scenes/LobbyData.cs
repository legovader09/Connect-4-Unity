using PlayerIOClient;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyData : MonoBehaviour
{
    private List<Message> msgList = new List<Message>(); //  Messsage queue implementation
    [SerializeField] private Button btnStart;
    [SerializeField] private GameObject txtWaiting;
    // Start is called before the first frame update
    void Start()
    {
        GameObject.Find("txtPlayer").GetComponent<Text>().text = PlayerPrefs.GetString("Username");

        Application.runInBackground = true;
        Debug.Log("Starting online room...");
        PlayerIO.Authenticate(
            "connect4-hor3eieh0ebariztrmdjq",
            "public",
            new Dictionary<string, string>
            {
            { "userId", PlayerPrefs.GetString("Username") },
            },
            null,
            delegate (Client client)
            {
                Debug.Log("Successfully connected to Player.IO");

                Debug.Log("Create ServerEndpoint");
                // Comment out the line below to use the live servers instead of your development server
                //client.Multiplayer.DevelopmentServer = new ServerEndpoint("localhost", 8184);
                Debug.Log("CreateJoinRoom");

                var joinCode = new System.Random().Next(100000, 999999).ToString();
                if (PlayerPrefs.GetString("LobbyCode").Trim() == "")
                {
                    Session.isHost = true;
                    PlayerPrefs.SetString("LobbyCode", joinCode);
                }
                else joinCode = PlayerPrefs.GetString("LobbyCode");

                //Create or join the room 
                client.Multiplayer.CreateJoinRoom(
                    joinCode,                    //Room id. If set to null a random roomid is used
                    "MainRoom",                   //The room type started on the server
                    false,                               //Should the room be visible in the lobby?
                    null,
                    new Dictionary<string, string>
                    {
                        { "isHost", "" + Session.isHost },
                    },
                    delegate (Connection connection)
                    {
                        Debug.Log("Joined Room.");
                        // We successfully joined a room so set up the message handler
                        Session.playerConn = connection;
                        Session.playerConn.OnMessage += handleMessage;
                        GameObject.Find("txtLobbyCode").GetComponent<TextMeshProUGUI>().text = $"Lobby Code: {joinCode}";
                        Session.playerConn.Send("PlayerData", Session.isHost);
                    },
                    delegate (PlayerIOError error)
                    {
                        Debug.Log("Error Joining Room: " + error.ToString());
                    }
                );
            },
            delegate (PlayerIOError error)
            {
                Debug.Log("Error connecting: " + error.ToString());
            }
        );
    }
    void FixedUpdate()
    {
        // process message queue
        foreach (Message m in msgList)
        {
            switch (m.Type)
            {
                case "PlayerJoined":
                    GameObject.Find("txtOpp").GetComponent<Text>().text = m.GetString(0);
                    PlayerPrefs.SetString("OppName", m.GetString(0));
                    btnStart.gameObject.SetActive(Session.isHost);
                    txtWaiting.SetActive(!Session.isHost);
                    break;
                case "PlayerLeft":
                    if (Session.isHost)
                    {
                        GameObject.Find("txtOpp").GetComponent<Text>().text = "Opponent";

                        btnStart.gameObject.SetActive(false);
                    }
                    else SceneManager.LoadScene("TitleScene");
                    break;
                case "GameStart":
                    SceneManager.LoadScene("GameScene");
                    break;
            }
        }

        // clear message queue after it's been processed
        msgList.Clear();
    }

    public void Leave()
    {
        Session.playerConn.Disconnect();

        SceneManager.LoadScene("TitleScene");
    }

    public void CopyCode()
    {
        GUIUtility.systemCopyBuffer = PlayerPrefs.GetString("LobbyCode");
    }

    public void startGame()
    {
        Session.playerConn.Send("GameStart");
    }

    void handleMessage(object sender, Message m)
    {
        msgList.Add(m);
    }
}
