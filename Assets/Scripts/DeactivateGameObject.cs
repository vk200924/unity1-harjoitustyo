using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateGameObject : MonoBehaviour
{

    private void OnEnable()
    {
         StartCoroutine(WaitAndDestroyGameObject());
    }
    IEnumerator WaitAndDestroyGameObject()
    {
        yield return new WaitForSeconds(2.5f);
        gameObject.SetActive(false);
    }
}
