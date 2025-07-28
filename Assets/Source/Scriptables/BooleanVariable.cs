using UnityEngine;

[CreateAssetMenu(fileName = "BooleanVariable", menuName = "Scriptables/Boolean Variable")]
public class BooleanVariable : ScriptableObject
{
    [SerializeField] private bool value;

    public bool Value
    {
        get => value;
        set => this.value = value;
    }
}
