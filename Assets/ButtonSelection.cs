using UnityEngine;
using UnityEngine.UI;

public class ButtonSelection : MonoBehaviour
{
    public Image lighterIndicator;
    public Button button;

    public void Select()
    {
        lighterIndicator.enabled = true;
    }

    public void Deselect()
    {
        lighterIndicator.enabled = true;
    }
}
