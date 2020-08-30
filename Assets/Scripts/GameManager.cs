using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public enum PickUp { Boost, Energy, Time };

public class GameManager : MonoBehaviour
{
    public static GameManager GM;
    public static float gameSpeed = 1.0f;

    public Button retryButton;
    public GameObject explotion;

    public bool gameIsOver = false;

    [SerializeField] private TextMeshProUGUI boostText;
    [SerializeField] private TextMeshProUGUI energyText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI weaponsText;
    [SerializeField] private TextMeshProUGUI pointsText;
    [SerializeField] private TextMeshProUGUI gameOverText;
    
    [SerializeField] private TextMeshProUGUI fpsText;


    [NonSerialized] public int points = 0;
    [NonSerialized] public int weapons = 3;
    [NonSerialized] public int pickUpDivider = 1;

    [NonSerialized] public float time = 100.0f;
    [NonSerialized] public float boost = 100.0f;
    [NonSerialized] public float energy = 100f;
    readonly public float statMax = 100.0f;

    
    private int frame = 0;
    private float fpsTimer = 0;
    private int framerate = 0;

    void Awake()
    {
        GM = this;
        retryButton.onClick.AddListener(LoadScene);
    }

    void Update()
    {
        Timer();
        //Päivitä tekstit kun pelaaja on elossa
        if (!PlayerIsDead())
        {
            pointsText.text = "Points: " + points;
            energyText.text = "Energy: " + Mathf.Ceil(energy);
            boostText.text = "Boost: " + Mathf.Ceil(boost);
            weaponsText.text = "Weapons: " + weapons;
            timeText.text = "Time: " + Mathf.Ceil(time);

            ChangeTextColorIfUnder30(boostText, boost);
            ChangeTextColorIfUnder30(energyText, energy);
            ChangeTextColorIfUnder30(timeText, time);

        }
        //Loppu toiminnot jos pelaaja kuolee
        else if (!gameIsOver)
        {
            gameIsOver = true;

            StartCoroutine(ClearGameObjects());

            //Näytä teksti kumpi meni nollaan energy vai time
            if (GameManager.GM.energy <= 0)
            {
                energyText.text = "Energy: 0";
                timeText.gameObject.SetActive(false);
            }
            else
            {
                timeText.text = "Time: 0";
                energyText.gameObject.SetActive(false);
            }

            //Piilota loput, mäytä retry nappi, enemiesKilled ja GameOver teksti
            boostText.gameObject.SetActive(false);
            weaponsText.gameObject.SetActive(false);
            retryButton.gameObject.SetActive(true);
            gameOverText.gameObject.SetActive(true);
            pointsText.gameObject.SetActive(true);
        }

        //FrameRate. Ei mitään tekemistä pelin kanssa kiinnosti vain tietää kun peli tökki
        FPS();

    }

    IEnumerator ClearGameObjects()
    {
        //Poista PickUpit kentältä
        ObjectPooler.OP.DeActivatePooledObjects(ObjectPooler.OP.pooledEnergyPickupsList);
        ObjectPooler.OP.DeActivatePooledObjects(ObjectPooler.OP.pooledBoostPickupsList);
        ObjectPooler.OP.DeActivatePooledObjects(ObjectPooler.OP.pooledTimePickupsList);

        //Poista pallot kentältä
        GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball");
        foreach (GameObject ball in balls)
        {
            Destroy(ball);
        }

        //Poista enemyt kentältä
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            yield return new WaitForSeconds(0.5f);
            Instantiate(explotion, enemy.transform.position, enemy.transform.rotation);
            Destroy(enemy);
        }
    }

    //Käytetään EnemyScriptin InstantiatePickup-metodissa
    public PickUp CalculatePickup()
    {
        if (boost / statMax < energy / statMax && boost / statMax < time / statMax)
        {
            return PickUp.Boost;
        }
        else if (energy / statMax < boost / statMax && energy / statMax < time / statMax)
        {
            return PickUp.Energy;
        }
        else
        {
            return PickUp.Time;
        }
    }

    void Timer()
    {
        float delsTime = Time.deltaTime;
        time -= delsTime;
    }

    void ChangeTextColorIfUnder30(TextMeshProUGUI textMesh, float stat)
    {
        textMesh.color = stat > 30f ? Color.white : Color.red;
    }

    public bool PlayerIsDead()
    {
        if (energy <= 0 || time <= 0 || (PlayerControl.PC.transform.position.y <= -100 && boost <= 0))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void LoadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    //Framerate
    void FPS()
    {
        frame++;
        fpsTimer += Time.deltaTime;
        if (Mathf.Floor(fpsTimer) >= 1f)
        {
            framerate = frame;
            fpsTimer = 0;
            frame = 0;
        }
        fpsText.text = "FPS: " + framerate;
    }
}

