using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler OP;

    public List<GameObject> pooledEnemyWeaponsList = new List<GameObject>();
    public List<GameObject> pooledPlayerWeaponsList = new List<GameObject>();
    public List<GameObject> pooledEnergyPickupsList = new List<GameObject>();
    public List<GameObject> pooledBoostPickupsList = new List<GameObject>();
    public List<GameObject> pooledTimePickupsList = new List<GameObject>();

    [SerializeField] private GameObject enemyWeaponToPool;
    [SerializeField] private GameObject playerWeaponToPool;
    [SerializeField] private GameObject energyPickUpToPool;
    [SerializeField] private GameObject boostPickUpToPool;
    [SerializeField] private GameObject timePickUpToPool;

    private const int ammountToPool = 20;

    void Awake()
    {
        OP = this;

        MakePooledObjectsList(pooledEnemyWeaponsList, enemyWeaponToPool);
        MakePooledObjectsList(pooledPlayerWeaponsList, playerWeaponToPool);
        MakePooledObjectsList(pooledEnergyPickupsList, energyPickUpToPool);
        MakePooledObjectsList(pooledBoostPickupsList, boostPickUpToPool);
        MakePooledObjectsList(pooledTimePickupsList, timePickUpToPool);
    }

    void MakePooledObjectsList(List<GameObject> pooledObjectList, GameObject pooledObject)
    {
        for (int i = 0; i < ammountToPool; i++)
        {
            GameObject obj = Instantiate(pooledObject);
            obj.SetActive(false);
            pooledObjectList.Add(obj);
            obj.transform.SetParent(this.transform);
        }
    }

    //Käy enemyWeapon, playerWeapon ja pickuppien prefab listat läpi, ja aktivoi ensimmäisen prefabin joka ei ole aktiivinen
    //Aseta sen paikka parametrina tuodun gameObjectin mukaan riippuen onko se Player vai Enemy. 
    public void ActivatePooledObject(List<GameObject> pooledObjectList, GameObject theObject)
    {
        
        foreach (GameObject pooledObject in pooledObjectList)
        {

            if (!pooledObject.activeInHierarchy)
            {
                //transform.position eneemylle
                Vector3 pos = new Vector3(theObject.transform.position.x, 0.5f, theObject.transform.position.z);

                //transform.position Playerille tai enemylle
                pooledObject.transform.position = theObject.CompareTag("Player") ? theObject.transform.position : pos;
                pooledObject.transform.rotation = theObject.transform.rotation;
                pooledObject.SetActive(true);

                break;
            }
        }
    }



    public void DeActivatePooledObjects(List<GameObject> pooledObjectList)
    {
        foreach (GameObject pooledObject in pooledObjectList)
        {
            if (pooledObject.activeInHierarchy)
            {
                pooledObject.SetActive(false);
            }
        }
    }


}
