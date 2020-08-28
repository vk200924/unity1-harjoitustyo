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

    //Käy enemyWeapon ja pickuppien prefab listat läpi, aktivoi ensimmäisen prefabin joka ei ole aktiivinen jq asettaa sen parametrina tuodun transformin paikkaan
    public void ActivatePooledObject(List<GameObject> pooledObjectList, Transform transform)
    {
        
        foreach (GameObject pooledObject in pooledObjectList)
        {

            if (!pooledObject.activeInHierarchy)
            {
                //transform.position muille kuin Player
                Vector3 pos = new Vector3(transform.position.x, 1f, transform.position.z);

                //transform.position Playerille tai muille
                pooledObject.transform.position = gameObject.CompareTag("Player") ? transform.position : pos;

                pooledObject.transform.rotation = transform.rotation;
                pooledObject.SetActive(true);

                Debug.Log(pooledObject.name + pooledObject.activeInHierarchy);
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
