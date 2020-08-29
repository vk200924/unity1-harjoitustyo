using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject enemy;
    public GameObject weaponBox;

    private int enemyCount;
    public static int waveNumber = 3;
    private bool spawningEnemies = false;

    void Update()
    {
        if (!GameManager.GM.gameIsOver)
        {
            //Etsi pelistä EnemyScriptit
            enemyCount = FindObjectsOfType<EnemyScript>().Length;

            //Jos ei löydy, niin kutsu SpawnEnemies(waveNumber)-metodi
            //energy boost ja time on max, tyhjennä kenttä pickup prefabseistä, luo weaponLoot ja lisää yksi PickUp-jakajaan 
            if (enemyCount == 0 && !spawningEnemies)
            {
                //Lisää Object Pooler listoihin joka neljäs vihollisaalto
                if (waveNumber % 4 == 0)
                {
                    ObjectPooler.OP.AddPooledOjects();
                    Debug.Log("Added to Pooler wave: " + waveNumber);
                }

                //Uusi enemyaalto
                StartCoroutine(SpawnOEnemies(waveNumber));

                //Energy, boost ja time takasin 100:n
                GameManager.GM.energy = GameManager.GM.statMax;
                GameManager.GM.boost = GameManager.GM.statMax;
                GameManager.GM.time = GameManager.GM.statMax;
                GameManager.GM.pickUpDivider++;

                //PickUpit pois kentältä
                ObjectPooler.OP.DeActivatePooledObjects(ObjectPooler.OP.pooledEnergyPickupsList);
                ObjectPooler.OP.DeActivatePooledObjects(ObjectPooler.OP.pooledBoostPickupsList);
                ObjectPooler.OP.DeActivatePooledObjects(ObjectPooler.OP.pooledTimePickupsList);

                //Luo weaponLoot 
                Vector3 randomPos = new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
                Vector3 weaponBoxPos = weaponBox.transform.position + randomPos;
                Instantiate(weaponBox, weaponBoxPos, weaponBox.transform.rotation);

                GameManager.GM.pickUpDivider++;
                GameManager.gameSpeed += 0.01f;

            }
        }

    }

    //Arvo satunaisia paikkoja enemylle väliajoin enemyNumberin (waveNumber) verran ja lisää yksi waveNumberiin
    IEnumerator SpawnOEnemies(int enemyNumber)
    {
        waveNumber++;
        spawningEnemies = true;

        for (int i = 0; i < enemyNumber; i++)
        {
            yield return new WaitForSeconds(2f);

            int randomRange = Random.Range(-90, 90);
            Vector3 randomPos = new Vector3(randomRange, enemy.transform.position.y, randomRange);

            ObjectPooler.OP.ActivatePooledObject(ObjectPooler.OP.pooledEnemyList, randomPos, transform.rotation);
        }
        spawningEnemies = false;
    }

}
