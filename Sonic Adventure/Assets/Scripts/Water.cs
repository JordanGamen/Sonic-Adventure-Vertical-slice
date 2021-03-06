﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(Constants.Tags.playerCol))
        {
            StartCoroutine(other.transform.parent.GetComponent<PlayerDeath>().Die());
        }
    }
}
