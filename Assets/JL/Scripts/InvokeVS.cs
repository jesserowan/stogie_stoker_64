using Unity.VisualScripting;
using UnityEngine;

public class InvokeVS : MonoBehaviour
{
    public GameObject visualScript;
    public float fadeSpeed = 0f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            CustomEvent.Trigger(visualScript, "startScreen", 0, fadeSpeed);
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            CustomEvent.Trigger(visualScript, "startScreen", 1, fadeSpeed);
        }
    }
}
