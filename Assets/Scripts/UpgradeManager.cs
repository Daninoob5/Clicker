using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    #region Properties
    public bool MaxUpgradeReached {  get; private set; }
    public int UpgradeIndex
    {
        get
        {
            return _upgradeIndex;
        }
        set
        {
            if(value < 0)
            {
                Debug.LogError("Error en la asignación de un índice de mejora");
            }
            else
            {
                _upgradeIndex = value;
            }
        }
    }
    #endregion
    #region Fields
    [SerializeField] private GameController _gameController;
    [SerializeField] private Inflator _inflator;

    [SerializeField] private enum _upgradeTypes { money, health, speed }
    [SerializeField] private _upgradeTypes _upgradeType;

    [SerializeField] private TextMeshProUGUI _priceText;
    [SerializeField] private long _price;

    [SerializeField] private TextMeshProUGUI _InfoText;

    [SerializeField] private int _upgradeIndex;
    [SerializeField] private int _maxUpgrade;
    [SerializeField] private int _multiplier;
    #endregion
    #region Unity Callbacks
    void Awake()
    {

    }
    void Start()
    {
        _upgradeIndex = 0;
        _multiplier = 1;
        UpdateUI();
    }
    void Update()
    {

    }
    #endregion
    #region Public Methods
    public void UpdateUI()
    {
        _priceText.text = _gameController.renameMoney(_price);
        _InfoText.text = "Actual: X" + _multiplier;
    }
    public void Upgrade(long money)
    {
        if(money >= _price)
        {
            _multiplier *= 2;
            ApplyMultiplier();
                _gameController.LoseMoney(Mathf.RoundToInt(_price));
            _price = Mathf.RoundToInt(_price * 2.5f);
            _upgradeIndex++;
            if (_upgradeIndex >= _maxUpgrade)
            {
                Debug.Log("Máxima mejora alcanzada");
                gameObject.SetActive(false);
                MaxUpgradeReached = true;
                _gameController.checkReachedUpgrades();
                
            }
            UpdateUI();
        }
        else
        {
            Debug.Log("No tienes suficiente dinero para realizar esta mejora");
            //Emitir sonido de error 
        }

    }
    public void NextTier()
    {
        MaxUpgradeReached = false;
        gameObject.SetActive(true);
        if (_upgradeType == _upgradeTypes.money)
        {
            _maxUpgrade += 5;
        }
        else
        {
            _multiplier = 1;
            ApplyMultiplier();
            _upgradeIndex = 0;
            UpdateUI();
        }
    }
    #endregion
    #region Private Methods
    private void ApplyMultiplier()
    {
        if (_upgradeType == _upgradeTypes.money)
        {
            _inflator.MoneyMultiplier = _multiplier;
        }
        else if (_upgradeType == _upgradeTypes.health)
        {
            _inflator.BalloonDurability = _multiplier * 10;
        }
        else if (_upgradeType == _upgradeTypes.speed)
        {
            _inflator.Speed = _multiplier;
        }
        else
        {
            Debug.LogError("Error buscando el tipo de mejora");
        }
    }
    #endregion
}
