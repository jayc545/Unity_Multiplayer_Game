using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public int spawnerId;
    public bool hasItem;
    public MeshRenderer itenModel;

    public float itemRotationSpeed = 50F;
    public float itemBobSpeed = 2f;
    private Vector3 basePosition;

    private void Update()
    {
        if (hasItem)
        {
            transform.Rotate(Vector3.up, itemRotationSpeed * Time.deltaTime, Space.World);
            transform.position = basePosition + new Vector3(0f, 0.25F * Mathf.Sin(Time.time * itemBobSpeed), 0f);
        }
    }

    public void Initialize(int _spawnerid, bool _hasItem)
    {
        spawnerId = _spawnerid;
        hasItem = _hasItem;
        itenModel.enabled = _hasItem;

        basePosition = transform.position;
    }

    public void ItemSpawned()
    {
        hasItem = true;
        itenModel.enabled = true; 
    }

    public void ItemPickedUp()
    {
        hasItem = false;
        itenModel.enabled = false;
    }
}
