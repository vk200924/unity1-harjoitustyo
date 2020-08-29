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
    public List<GameObject> pooledEnemyExplotionList = new List<GameObject>();
    public List<GameObject> pooledEnemyList = new List<GameObject>();
    public List<GameObject> pooledWeaponBoxList = new List<GameObject>();


    [SerializeField] private GameObject enemyWeaponToPool;
    [SerializeField] private GameObject playerWeaponToPool;
    [SerializeField] private GameObject energyPickUpToPool;
    [SerializeField] private GameObject boostPickUpToPool;
    [SerializeField] private GameObject timePickUpToPool;
    [SerializeField] private GameObject explotionToPool;
    [SerializeField] private GameObject enemyToPool;
    [SerializeField] private GameObject weaponBoxToPool;

    private const int ammountToPool = 50;

    void Awake()
    {
        OP = this;

        MakePooledObjectsList(pooledEnemyList, enemyToPool);
        MakePooledObjectsList(pooledWeaponBoxList, weaponBoxToPool);
        MakePooledObjectsList(pooledEnemyWeaponsList, enemyWeaponToPool);
        MakePooledObjectsList(pooledPlayerWeaponsList, playerWeaponToPool);
        MakePooledObjectsList(pooledEnergyPickupsList, energyPickUpToPool);
        MakePooledObjectsList(pooledBoostPickupsList, boostPickUpToPool);
        MakePooledObjectsList(pooledTimePickupsList, timePickUpToPool);
        MakePooledObjectsList(pooledEnemyExplotionList, explotionToPool);
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

    //Käy prefab lista läpi ja aktivoi ensimmäisen prefabin joka ei ole aktiivinen
    //Aseta sen paikka parametrina tuodun pos ja rotation mukaan
    public void ActivatePooledObject(List<GameObject> pooledObjectList, Vector3 pos, Quaternion rotation)
    {
        foreach (GameObject pooledObject in pooledObjectList)
        {
            if (!pooledObject.activeInHierarchy)
            {
                pooledObject.transform.position = pos;
                pooledObject.transform.rotation = rotation;

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
