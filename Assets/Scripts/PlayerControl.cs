using UnityEngine;


public class PlayerControl : MonoBehaviour
{
    public static PlayerControl PC;

    [SerializeField] private GameObject playerWeapon;
    [SerializeField] private GameObject dragon;
    [SerializeField] private Animator dragonAnim;
    [SerializeField] private GameObject behindPlayer;

    private Rigidbody playerRb;

    private const float speed = 60.0f;
    private const float turnSpeed = 165.0f;
    private const float floatForce = 15.0f;
    private const float boostSpeed = 20.0f;
    private const float ballForce = 70.0f;

    private float horizontalInput;
    private float forwardInput;

    private bool colliderIsGrounded;

    void Awake()
    {
        PC = this;
        Physics.gravity = new Vector3(0, -45f, 0);
        playerRb = GetComponent<Rigidbody>();
        dragonAnim = dragon.GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        if (!GameManager.GM.gameIsOver)
        {
            MoveThePlayer();
            FlyAndGlide();
        }
    }

    void Update()
    {
        if (!GameManager.GM.gameIsOver)
        {
            FireWeapon();
        }
    }

    private void LateUpdate()
    {
        if (!GameManager.GM.gameIsOver)
        {
            PlayerAnimations();
        }
        else
        {
            dragonAnim.SetTrigger("Die");
        }
    }

    void MoveThePlayer()
    {
        // player input
        horizontalInput = Input.GetAxis("Horizontal");
        forwardInput = Input.GetAxis("Vertical");

        // Move the player forward
        transform.Translate(Vector3.forward * Time.deltaTime * speed * forwardInput * GameManager.gameSpeed);

        // Turn the player
        transform.Rotate(Vector3.up, Time.deltaTime * turnSpeed * horizontalInput);
    }

    void FlyAndGlide()
    {
        //Fly
        if (Input.GetKey(KeyCode.C) && GameManager.GM.boost > 0)
        {
            playerRb.AddForce(Vector3.up * floatForce, ForceMode.Impulse);
            GameManager.GM.boost -= UseBoost(10f);
        }

        //Glide
        if (Input.GetKey(KeyCode.Z) && GameManager.GM.boost > 0)
        {
            playerRb.AddRelativeForce(Vector3.forward * boostSpeed * GameManager.gameSpeed, ForceMode.Impulse);
            GameManager.GM.boost -= UseBoost(5f);
        }

    }

    void PlayerAnimations()
    {
        //Animaatiot jos pelaajan collider osuu maahan
        if (colliderIsGrounded)
        {
            if (Input.GetKey(KeyCode.DownArrow))
            {
                dragonAnim.SetBool("IsOnGround", false);
                dragonAnim.SetTrigger("FlyIdle");
                //Debug.Log("FlyIdle");
            }
            else if (GameManager.GM.boost > 0 && Input.GetKeyDown(KeyCode.C))
            {
                dragonAnim.SetBool("IsOnGround", false);
                dragonAnim.SetTrigger("Fly");
                //Debug.Log("Glide");
            }            
            else if (GameManager.GM.boost > 0 && (Input.GetKeyDown(KeyCode.Z) || Input.GetKey(KeyCode.Z)))
            {
                dragonAnim.SetBool("IsOnGround", true);
                dragonAnim.SetTrigger("Run");
                //Debug.Log("Glide");
            }
            else if (Input.GetKey(KeyCode.UpArrow))
            {
                dragonAnim.SetBool("IsOnGround", true);
                dragonAnim.SetTrigger("Walk");
                //Debug.Log("Run");
            }
            else
            {
                dragonAnim.SetBool("IsOnGround", true);
                dragonAnim.SetTrigger("Idle");
                //Debug.Log("Idle");
            }
        }
        else if(!colliderIsGrounded)
        {
            if (Input.GetKeyUp(KeyCode.C) || GameManager.GM.boost <= 0)
            {
                dragonAnim.SetTrigger("Glide");
                dragonAnim.SetBool("IsOnGround", false);

            }
            else if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.C) && GameManager.GM.boost >= 0)
            {
                //
                dragonAnim.SetTrigger("Fly");
                dragonAnim.SetBool("IsOnGround", false);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                dragonAnim.SetTrigger("FlyIdle");
                dragonAnim.SetBool("IsOnGround", false);
            }
        }

        if (Input.GetKeyDown(KeyCode.A)) Debug.Log(colliderIsGrounded);

    }

    float UseBoost(float usedBoost)
    {
        return Time.deltaTime * usedBoost;
    }

    void FireWeapon()
    {
        //Aktivoi ammus ja poista yks weapons
        if (GameManager.GM.weapons > 0 && Input.GetKeyDown(KeyCode.Space))
        {
            ObjectPooler.OP.ActivatePooledObject(ObjectPooler.OP.pooledPlayerWeaponsList, transform.position, transform.rotation);
            GameManager.GM.weapons--;
        }
    }

    void AddPickupAndCheckIfItGoesOverMax(ref float pickUp)
    {
        float pickUpMax = 100;
        pickUp += pickUpMax / GameManager.GM.pickUpDivider;
        if (pickUp > pickUpMax)
        {
            pickUp = pickUpMax;
        }
    }



    private void OnCollisionEnter(Collision collision)
    {
        //Ammu pallo taaksepäin jos painetaan x
        if (collision.gameObject.CompareTag("Ball") && Input.GetKey(KeyCode.X))
        {
            collision.gameObject.transform.position = new Vector3(behindPlayer.transform.position.x, collision.gameObject.transform.position.y, behindPlayer.transform.position.z);
            collision.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * -ballForce / 4, ForceMode.Impulse);
        }
        //Ammu pallo eteenpäin
        else if (collision.gameObject.CompareTag("Ball"))
        {
            collision.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * ballForce, ForceMode.Impulse);
        }

        //Enemy törmätessä vähennetään energy ja AddForce enemystä poispäin
        if (collision.gameObject.CompareTag("Enemy") && GameManager.GM.energy > 0)
        {
            GameManager.GM.energy -= 5;
            Vector3 direction = collision.transform.position - transform.position;
            playerRb.AddForce(-direction * 30f, ForceMode.Impulse);
            dragonAnim.SetTrigger("GetHit");
        }

        //WeaponLoot törmätessä lisätää weapons
        if (collision.gameObject.CompareTag("WeaponLoot"))
        {
            Destroy(collision.gameObject);
            GameManager.GM.weapons++;
        }

        if (collision.gameObject.CompareTag("Ground"))
        {
            colliderIsGrounded = true;
        }

    }


    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            colliderIsGrounded = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Lisää boost, energy tai time
        if (other.gameObject.CompareTag("Boost"))
        {
            AddPickupAndCheckIfItGoesOverMax(ref GameManager.GM.boost);
            other.gameObject.SetActive(false);
            Debug.Log("Boost");
        }
        else if (other.gameObject.CompareTag("Energy"))
        {
            AddPickupAndCheckIfItGoesOverMax(ref GameManager.GM.energy);
            other.gameObject.SetActive(false);
        }
        else if (other.gameObject.CompareTag("Time"))
        {
            AddPickupAndCheckIfItGoesOverMax(ref GameManager.GM.time);
            other.gameObject.SetActive(false);
        }

        //Jos EnemyWeapon osuu poista energy ja piilota EnemyWeapon
        if (other.gameObject.CompareTag("EnemyWeapon"))
        {
            GameManager.GM.energy -= 5;
            other.gameObject.SetActive(false);
        }
    }


}