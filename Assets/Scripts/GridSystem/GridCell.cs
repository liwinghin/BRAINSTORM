using UnityEngine;
using UnityEngine.UI;

public class GridCell : MonoBehaviour
{
    public Text text;
    public Block root;

    public void SetRoot(Block r)
    {
        root = r;
    }
    public void SetCoordinate(string t)
    {
        text.text = t;
    }
}   