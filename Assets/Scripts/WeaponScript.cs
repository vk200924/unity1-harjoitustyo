using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponScript : MonoBehaviour
{
    private const float weaponSpeed = 250.0f;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * weaponSpeed * Time.deltaTime);
        StartCoroutine(CheckIfOutOfBounds());
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.CompareTag("Edge"))
        {
            gameObject.SetActive(false);
        }
    }

    IEnumerator CheckIfOutOfBounds()
    {
        yield return new WaitForSeconds(1);
        
        if (transform.position.x < -100 || transform.position.x > 100 || transform.position.z < -100 || transform.position.z > 100)
        {
            gameObject.SetActive(false);
        }
    }
}
