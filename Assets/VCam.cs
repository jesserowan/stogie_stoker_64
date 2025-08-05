using System;
using Unity.Cinemachine;
using UnityEngine;

public class VCam : MonoBehaviour
{
    public CinemachineCamera vcam;
    public CinemachineRotationComposer rotation;
    public CinemachineFollow follow;

    private void Awake()
    {
        vcam = GetComponent<CinemachineCamera>();
        follow = GetComponent<CinemachineFollow>();
        rotation = GetComponent<CinemachineRotationComposer>();
    }

    private void Start()
    {
        Debug.Log($"VCAM START: current fov {vcam.Lens.FieldOfView}; target fov: {GameManager.CurrentDifficulty.fieldOfView}");
        vcam.Lens.FieldOfView = GameManager.CurrentDifficulty.fieldOfView;
        Debug.Log($"VCAM START: new fov: {vcam.Lens.FieldOfView}");
    }
}
