using UnityEngine;

[CreateAssetMenu(fileName = "FloatConstant", menuName = "Scriptables/Float Constant")]
public class FloatConstant : ScriptableObject
{
    [SerializeField] private float value;

    public float Value => value;
}
