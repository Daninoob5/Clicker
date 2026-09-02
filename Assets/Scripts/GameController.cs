using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    #region Properties
    public long Money
    {
        get
        {
            return _money;
        }
        set
        {
            if (value < 0)
            {
                Debug.LogError("Error al asignar el valor del dinero");
            }
            else
            {
                if (!_10kMoneyAchieved && value >= 10000)
                {
                    _10kMoneyBadgeManager.BadgeAchieved()   ;
                    _10kMoneyAchieved = true;
                }
                _money = value;
            }
        }
    }
    #endregion
    #region Fields
    [SerializeField] private Transform _canvasTransform;

    [Header("Inflator")]
    [SerializeField] private GameObject _balloon;
    [SerializeField] Button _inflatorButton;

    [Header("Money")]
    [SerializeField] private long _money;
    [SerializeField] private TextMeshProUGUI _moneyText;

    [Header("Upgrades")]
    [SerializeField] private UpgradeManager _moneyUpgradeManager;
    [SerializeField] private UpgradeManager _healthUpgradeManager;
    [SerializeField] private UpgradeManager _speedUpgradeManager;

    [Header("Levels")]
    [SerializeField] private int _currentLevel;
    [SerializeField] private GameObject _levelUpgradeButton;
    [SerializeField] private long _levelUpgradePrice;
    [SerializeField] private TextMeshProUGUI _levelUpgradePriceText;
    [SerializeField] private GameObject _winScreen;

    [Header("Ambient")]
    [SerializeField] private Image _currentGameBackground;
    [SerializeField] private Sprite _background1;
    [SerializeField] private Sprite _background2;
    [SerializeField] private Sprite _background3;
    [SerializeField] private AudioClip _music1;
    [SerializeField] private AudioClip _music2;
    [SerializeField] private AudioClip _music3;

    [Header("Badges")]
    [SerializeField] private BadgeManager _levelUp1BadgeManager;
    [SerializeField] private BadgeManager _10kMoneyBadgeManager;
    [SerializeField] private bool _10kMoneyAchieved;

    [Header("Agents")]
    [SerializeField] private GameObject _ballonAgentPrefab;
    #endregion
    #region Unity Callbacks
    private void Awake()
    {
        _canvasTransform = FindFirstObjectByType<Canvas>().transform;
        _levelUpgradePrice = 0;
        _currentLevel = 0;
        UpgradeLevel();
    }
    void Start()
    {
        _10kMoneyAchieved = false;
    }
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
            Application.Quit();
    }
    #endregion
    #region Public Methods
    public void GainMoney(long moneyGained)
    {
        Money += moneyGained; 
        _moneyText.text = renameMoney(Money);

    }
    public void LoseMoney(long moneyLost)
    {
        Money -= moneyLost;
        _moneyText.text = renameMoney(Money);
    }
    public void MoneyUpgrade()
    {
        
        _moneyUpgradeManager.Upgrade(Money);
    }
    public void DurabilityUpgrade()
    {
        _healthUpgradeManager.Upgrade(Money);
    }
    public void SpeedUpgrade()
    {
        _speedUpgradeManager.Upgrade(Money);
    }
    public void UpgradeLevel()
    {
        if(Money >= _levelUpgradePrice)
        {
            _currentLevel++;
            _inflatorButton.gameObject.GetComponent<Inflator>().UpgradeBalloon(_currentLevel);
            LoseMoney(_levelUpgradePrice);
            _levelUpgradeButton.SetActive(false);
            if (_currentLevel == 1)
            {
                _currentGameBackground.sprite = _background1;
                SetMusic(_music1);
                _levelUpgradePrice = 30000;
            }
            else if (_currentLevel == 2)
            {
                _currentGameBackground.sprite = _background2;
                SetMusic(_music2);
                _levelUpgradePrice = 1000000;
                _moneyUpgradeManager.NextTier();
                _healthUpgradeManager.NextTier();
                _speedUpgradeManager.NextTier();
                _levelUp1BadgeManager.BadgeAchieved();
            }
            else if (_currentLevel == 3)
            {
                _currentGameBackground.sprite = _background3;
                SetMusic(_music3);
                _levelUpgradePrice = 50000000;
                _moneyUpgradeManager.NextTier();
                _healthUpgradeManager.NextTier();
                _speedUpgradeManager.NextTier();
            }
            else
            {
                _winScreen.SetActive(true);
                SetMusic(null);
            }
                _levelUpgradePriceText.text = renameMoney(_levelUpgradePrice);

        }
        else
        {
            Debug.Log("No tienes dinero suficiente para realizar una mejora");
            //Sonido de error
        }

    }
    public void checkReachedUpgrades()
    {
        if(_healthUpgradeManager.MaxUpgradeReached && _speedUpgradeManager.MaxUpgradeReached && _moneyUpgradeManager.MaxUpgradeReached)
        {
            Debug.Log("Todas las mejoras máximas alcanzadas");
            _levelUpgradeButton.SetActive(true);
        }
    }
    public string renameMoney(long money)
    {
        if (money < 1000)
        {
            return money.ToString();
        }
        else if (money < 1000000)
        {
            float renamedMoney = money / 1000f;
            return renamedMoney.ToString("F2") + "K";
        }
        else if (money < 1000000000)
        {
            float renamedMoney = money / 1000000f;
            return renamedMoney.ToString("F2") + "M";
        }
        else
        {
            float renamedMoney = money / 1000000000f;
            return renamedMoney.ToString("F2") + "B";
        }
    }
    public void CreateAgent(float speed, float durability, float moneyMultiplier, Sprite sprite, GameObject particles)
    {
        GameObject ballon = Instantiate(_ballonAgentPrefab, _canvasTransform);
        BalloonAgent ballonAgent = ballon.GetComponent<BalloonAgent>();
        ballonAgent.Speed = speed;
        ballonAgent.Durability = durability;   
        ballonAgent.MoneyMultiplier = moneyMultiplier;
        ballonAgent.ExplosionParticles = particles;
        ballon.GetComponent<Image>().sprite = sprite;
        ballonAgent.GameController = this;
        ballonAgent.MoveToRandomPosition();
    }
    public void SetMusic(AudioClip audio)
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.resource = audio;
        if (audio != null)
        {
            audioSource.Play();
        }
        
    }
    public void GameQuit()
    {
        Application.Quit();
    }
    #endregion
    #region Private Methods
    #endregion
}
