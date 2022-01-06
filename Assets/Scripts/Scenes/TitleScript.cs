using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScript : MonoBehaviour
{
    [SerializeField] private GameObject dialogPrefab;
    // Start is called before the first frame update
    void Start()
    {
        Session.isHost = false;
        Session.playerConn = null;
    }
    public void ClickMultiplayer()
    {
        StartCoroutine(ShowUserDialog());
    }

    public void ClickSinglePlayer()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void ClickExitGame()
    {
        Application.Quit();
    }

    IEnumerator ShowUserDialog()
    {
        Dialog d = Instantiate(dialogPrefab).GetComponent<Dialog>();
        d.CreateDialog("Pick a Username", "Please choose a username that others can see when playing online.", true, true);
        GameObject.Find("txtInput").GetComponent<InputField>().text = PlayerPrefs.GetString("Username");
        yield return new WaitWhile(() => d.dialogResult == DialogResult.None);
        if (d.dialogResult == DialogResult.Confirm)
        {
            PlayerPrefs.SetString("Username", GameObject.Find("txtInput").GetComponent<InputField>().text);
            StartCoroutine(ShowLobbyDialog());
        }
        Destroy(d.gameObject);
    }

    IEnumerator ShowLobbyDialog()
    {
        Dialog d = Instantiate(dialogPrefab).GetComponent<Dialog>();
        d.CreateDialog("Enter Lobby Code", "Leave this blank if you are hosting the game.", true, false);
        yield return new WaitWhile(() => d.dialogResult == DialogResult.None);
        if (d.dialogResult == DialogResult.Confirm)
        {
            PlayerPrefs.SetString("LobbyCode", GameObject.Find("txtInput").GetComponent<InputField>().text);
            SceneManager.LoadScene("Lobby");
        }
        Destroy(d.gameObject);
    }
}
