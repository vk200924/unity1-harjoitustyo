using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckIfOutOfBounds : MonoBehaviour
{

    Vector3 startPos;
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(ResetPos());
        
    }

    //Tarkista sekunnin välein pari bugia enemy ja ball prefabseistä
    IEnumerator ResetPos()
    {
        yield return new WaitForSeconds(1);
        if (transform.position.x < -105 || transform.position.x > 105 || transform.position.z < -105 || transform.position.z > 105)
        {
            transform.position = startPos;
            Debug.Log("reset pos " + gameObject.name);
        }
        if (startPos.y - transform.position.y < -1 || startPos.y - transform.position.y > 1)
        {
            transform.position = new Vector3(transform.position.x, startPos.y, transform.position.z);
            //Debug.Log("reset y " + gameObject.name);
        }
    }
}
