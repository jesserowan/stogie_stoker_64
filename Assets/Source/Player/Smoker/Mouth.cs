using UnityEngine;

public class Mouth : MonoBehaviour
{
    public PositionVariable mouthPosition;

    private void FixedUpdate()
    {
        mouthPosition.Value = transform.position;
    }
}
