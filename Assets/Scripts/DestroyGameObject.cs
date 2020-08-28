using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyGameObject : MonoBehaviour
{

    // Skripti explotion prefabin tuhoamiseen
    void Start()
    {
         StartCoroutine(WaitAndDestroyGameObject());
    }
    
    IEnumerator WaitAndDestroyGameObject()
    {
        yield return new WaitForSeconds(2.5f);
        Destroy(gameObject);
    }
}
