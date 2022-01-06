using UnityEngine;

public class LayoutGenerator : MonoBehaviour
{
    [SerializeField] private GameObject circle;
    public Vector2Int boardSize;
    // Start is called before the first frame update
    void Start()
    {
        var baseX = -3.75f;
        var baseY = 3.1f;
        for (int y = 1; y <= boardSize.y; y++)
        {
            for (int x = 1; x <= boardSize.x; x++)
            {
                var c = Instantiate(circle);
                c.transform.parent = GameObject.Find("GameBoard").transform;
                c.transform.position = new Vector3(baseX, baseY, -1);
                c.name = $"{x},{y}";
                c.GetComponent<CheckerHelper>().coords = new Vector2Int(x, y);
                baseX += 1.25f;
            }
            baseX = -3.75f;
            baseY -= 1.25f;
        }
    }
}