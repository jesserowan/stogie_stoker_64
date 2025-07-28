using UnityEngine;

[CreateAssetMenu(fileName = "IntegerVariable", menuName = "Scriptables/Integer Variable")]
public class IntegerVariable : ScriptableObject
{
    [SerializeField] private int value;

    public int Value
    {
        get => value;
        set => this.value = value;
    }
}
