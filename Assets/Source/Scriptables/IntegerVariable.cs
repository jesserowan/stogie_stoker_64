using UnityEngine;

[CreateAssetMenu(fileName = "IntegerVariable", menuName = "Scriptables/Integer Variable")]
public class IntegerVariable : ScriptableObject
{
    [SerializeField] private int value;
    [SerializeField] private int defaultValue;

    public int Value
    {
        get => value;
        set => this.value = value;
    }

    public void Reset()
    {
        Value = defaultValue;
    }
}
