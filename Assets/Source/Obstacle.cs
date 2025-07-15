// Copyright (c) 2025 by SoftEthix, Inc.
// All rights reserved.

using System;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField]
    public MeshRenderer obstacleRenderer;
    
    [SerializeField]
    public MeshFilter obstacleMesh;
    
    [SerializeField]
    public ObstacleGrid obstacleGrid;

    private void OnEnable()
    {
        obstacleMesh = GetComponent<MeshFilter>();
        obstacleRenderer = GetComponent<MeshRenderer>();
    }

    private void Awake()
    {
        Debug.Log($"Obstacle::{gameObject.name}.Awake(): grid: {obstacleGrid}");
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }

    public Obstacle Spawn(Vector3 position, Quaternion rotation)
    {
        Debug.Log($"Obstacle::{gameObject.name}.Spawn(): pos: {position}, rot: {rotation}");
        gameObject.transform.position = position;
        gameObject.transform.rotation = rotation;
        return this;
    }

    public void Enable()
    {
        enabled = true;
        gameObject.SetActive(true);
    }

    public void Disable()
    {
        Destroy(this);
    }
}
