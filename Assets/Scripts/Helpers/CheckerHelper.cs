using UnityEngine;

public class CheckerHelper : MonoBehaviour
{
    public Vector2Int coords;
    public bool isChecked = false;

    private void OnMouseOver()
    {
        var eventSystem = GameObject.Find("EventSystem");

        if (isChecked || !eventSystem.GetComponent<GameHelper>().isPlayerTurn) return;

        if (Input.GetMouseButtonUp(0))
        {
            var c = findBottomUnoccupiedTile(6).GetComponent<CheckerHelper>();
            c.isChecked = true;
            c.GetComponent<SpriteRenderer>().color = Color.yellow;
            eventSystem.GetComponent<GameHelper>().isPlayerTurn = false;

            if (Session.playerConn != null)
            {
                eventSystem.GetComponent<GameHelper>().sendMove(c.coords.x, c.coords.y);
                eventSystem.GetComponent<GameHelper>().checkWin();
            }
        }
    }

    /// <summary>
    /// Recursively finds the bottom-most unoccupied space.
    /// </summary>
    /// <param name="depth">Initial depth to search</param>
    /// <returns>Checker GameObject</returns>
    private GameObject findBottomUnoccupiedTile(int depth)
    {
        var g = GameObject.Find(coords.x.ToString() + "," + depth);
        if (!g.GetComponent<CheckerHelper>().isChecked)
            return g;

        // if depth ends up being 1, it's the only tile left.
        if (depth == 1)
            return this.gameObject;
            
        return findBottomUnoccupiedTile(depth - 1);
    }
}