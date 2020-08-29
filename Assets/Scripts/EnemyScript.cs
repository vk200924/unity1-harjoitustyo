using System.Collections;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class EnemyScript : MonoBehaviour
{
    [SerializeField] private GameObject weaopnOnIndicator;
    [SerializeField] private GameObject explotionParticle;

    [SerializeField] private GameObject enemy;
    private Animator enemyAnim;

    private GameObject[] ballPrefabs;
    private Rigidbody enemyRb;

    private const float dodgeDistance = 20.0f;
    private const float dodgeSpeed = 15.0f;
    private const float followSpeed = 25.0f;
    private const float enemyWeaponRange = 100f;
    private const float lookRotationSpeed = 7f;
    private const float attacktDistance = 40f;
    private const float attackSpeed = 50f;

    private bool enemyIsActive = false;

    void Awake()
    {
        enemyRb = GetComponent<Rigidbody>();
        enemyAnim = enemy.GetComponent<Animator>();
        ballPrefabs = GameObject.FindGameObjectsWithTag("Ball");
    }

    private void FixedUpdate()
    {
        if (!GameManager.GM.gameIsOver && enemyIsActive)
        {
            DodgeBall();
            FollowPlayer(followSpeed);
            LookAtPlayer();
        }
    }

    private void Update()
    {
        if (!GameManager.GM.gameIsOver && enemyIsActive)
        {
            FireAndAttack();
        }
    }

    private void OnEnable()
    { 
        StartCoroutine(StartDelay());
    }

    IEnumerator StartDelay()
    {
        yield return new WaitForSeconds(2f);
        enemyIsActive = true;
        weaopnOnIndicator.SetActive(true);
    }

    void FollowPlayer(float speed)
    {
        Vector3 playerDirection = (PlayerControl.PC.transform.position - transform.position).normalized;
        enemyRb.AddForce(playerDirection * GameManager.gameSpeed * speed);
    }

    //Jos välimatka pelaajaan on enemmän kuin lookAtDistance käänny pelaaja kohti, muuten katso pelaajaan päin
    void LookAtPlayer()
    {
        Vector3 playerPos = new Vector3(PlayerControl.PC.transform.position.x, -7f, PlayerControl.PC.transform.position.z);

        if (Vector3.Distance(PlayerControl.PC.transform.position, transform.position) > attacktDistance)
        {
            Quaternion rotationTowardsPlayer = Quaternion.LookRotation((playerPos - transform.position).normalized);
            Quaternion currentRotation = transform.localRotation;

            transform.localRotation = Quaternion.Slerp(currentRotation, rotationTowardsPlayer, Time.deltaTime * lookRotationSpeed);
        }
        else
        {
            transform.LookAt(playerPos);
        }
    }

    //AddForce poispäin palloista jos matka alle dodgeDistance
    private void DodgeBall()
    {
        for (int i = 0; i < ballPrefabs.Length; i++)
        {
            if (Vector3.Distance(transform.position, ballPrefabs[i].transform.position) < dodgeDistance)
            {
                Vector3 projectileDirectionn = (ballPrefabs[i].transform.position - transform.position).normalized;
                enemyRb.AddForce(-projectileDirectionn * dodgeSpeed, ForceMode.Impulse);
            }
        }
    }

    void FireAndAttack()
    {

        //Jos välimatka pelaajaan on pienempi kuin enemyWeaponRange ja weaopnOnIndicator on aktiivinen, niin activoi ammus ja aloita WaitForWeapon()
        if (weaopnOnIndicator.activeInHierarchy && CheckIfRayCastHitPlayer())
        {
            Vector3 pos = transform.position + Vector3.up;

            ObjectPooler.OP.ActivatePooledObject(ObjectPooler.OP.pooledEnemyWeaponsList, pos, transform.rotation);
            weaopnOnIndicator.gameObject.SetActive(false);
            Debug.Log("Fire");

            StartCoroutine(WaitForWeapon());
        }
        else if (Vector3.Distance(transform.position, PlayerControl.PC.transform.position) < attacktDistance)
        {
            Debug.Log("Attack");
            FollowPlayer(attackSpeed);
            enemyAnim.SetTrigger("Attack");
        }

    }

    bool CheckIfRayCastHitPlayer()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, enemyWeaponRange) && hit.collider.gameObject.CompareTag("Player"))
        {
            return true;
        }
        return false;
        //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward * 100f), Color.green, 2.0f);
        //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward * 100f), Color.white);
    }

    IEnumerator WaitForWeapon()
    {
        yield return new WaitForSeconds(10);
        weaopnOnIndicator.gameObject.SetActive(true);
    }

    //Aktivoi Pickup mitä pelaajalla on vähiten
    void ActivePickUp()
    {
        Vector3 pickupPos = transform.position + new Vector3(0,5,0);
        
        switch (GameManager.GM.CalculatePickup())
        {
            case PickUp.Boost:
                ObjectPooler.OP.ActivatePooledObject(ObjectPooler.OP.pooledBoostPickupsList, pickupPos, transform.rotation);
                break;

            case PickUp.Energy:
                ObjectPooler.OP.ActivatePooledObject(ObjectPooler.OP.pooledEnergyPickupsList, pickupPos, transform.rotation);
                break;

            case PickUp.Time:
                ObjectPooler.OP.ActivatePooledObject(ObjectPooler.OP.pooledTimePickupsList, pickupPos, transform.rotation);
                break;
        }
    }

    void DeactivateEnemy()
    {
        //deaktivoi enemy
        enemyIsActive = false;
        gameObject.SetActive(false);
        weaopnOnIndicator.SetActive(false);
        enemyRb.velocity = Vector3.zero;

        //Activoi räjähdys
        ObjectPooler.OP.ActivatePooledObject(ObjectPooler.OP.pooledEnemyExplotionList, transform.position, transform.rotation);

    }

    //Jos törmää palloon InstantiateLoot(). Jos törmää seinään pomppaa vähän toiseen suuntaan
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            ActivePickUp();
            DeactivateEnemy();
            GameManager.GM.points++;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerWeapon"))
        {
            ActivePickUp();
            DeactivateEnemy();
            GameManager.GM.points++;
        }
    }

}

