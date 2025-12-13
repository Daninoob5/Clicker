using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using Unity.VisualScripting;
using TMPro;

public class Inflator : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    #region Properties
    [SerializeField] public int MoneyMultiplier
    {
        get
        {
            return _moneyMultiplier;
        }
        set
        {
            if (value < 0)
            {
                Debug.LogError("Error al asignar el multiplicador de dinero");
            }
            else
            {
                _moneyMultiplier = value;
            }
        }
    }
    [SerializeField] public int Speed
    {
        get
        {
            return _speed;
        }
        set
        {
            if (value < 0)
            {
                Debug.LogError("Error al asignar la velocidad de crecimiento del globo");
            }
            else
            {
                _speed = value;
            }
        }
    }
    public float BalloonDurability
    {
        get
        {
            return _balloonDurablility;
        }
        set
        {
            if (value < 0)
            {
                Debug.LogError("Error al asignar la durabilidad del globo");
            }
            else
            {
                _balloonDurablility = value;
            }
        }
    }
    #endregion
    #region Fields
    [SerializeField] private GameController _gameController;
    [SerializeField] private GameObject _explodeParticles;
    [SerializeField] private Transform _balloonParentTransform;

    [Header("Ballon")]
    [SerializeField] private GameObject _currentBalloonModel;
    [SerializeField] private GameObject _balloonModel1;
    [SerializeField] private GameObject _balloonModel2;
    [SerializeField] private GameObject _balloonModel3;
    [SerializeField] private GameObject _currentBalloon;
    [SerializeField] private GameObject _previousBalloon;

    [Header("Upgrades")]
    [SerializeField] private int _moneyMultiplier;
    [SerializeField] private int _speed;
    [SerializeField] private float _balloonDurablility;

    [Header("Inflate")]
    [SerializeField] private int _flyForce;
    [SerializeField] private bool _isPressed = false;
    [SerializeField] private float _timePressed;
    [SerializeField] private float _balloonExplodeChance;

    [Header("Badges")]
    [SerializeField] private bool _first10Ballon;
    [SerializeField] private BadgeManager _ballon10BadgeManager;
    [SerializeField] private bool _first500KBallon;
    [SerializeField] private BadgeManager _ballon500KBadgeManager;
    [SerializeField] private BadgeManager _100BalloonsExplodedBadgeManager;
    [SerializeField] private BadgeManager _ballonExplodeBadgeManager;
    [SerializeField] private int _ballonsExploded;

    #endregion
    #region Unity Callbacks
    void Start()
    {
        _ballonsExploded = 0;
        _moneyMultiplier = 1;
        _speed = 1;
        _first10Ballon = false;
    }
    void Update()
    {
        if (_isPressed && _currentBalloon != null)
        {
            _timePressed += Time.deltaTime;
            //Debug.Log("Inflando...");
            _currentBalloon.transform.localScale = new Vector3(_timePressed*_speed/3, _timePressed*_speed/3, 1);
            _currentBalloon.GetComponentInChildren<TextMeshProUGUI>().text = "+" + Mathf.RoundToInt(_timePressed * _moneyMultiplier * _speed);
            float randomValue = Random.value;
            _balloonExplodeChance = Mathf.Pow(_timePressed*_speed / _balloonDurablility, 2f);
            if (randomValue < _balloonExplodeChance * Time.deltaTime * 2f)
            {
                Instantiate(_explodeParticles, _currentBalloon.transform.position, Quaternion.identity);
                Destroy(_currentBalloon);
                _ballonsExploded++;
                Debug.Log("El agente globo ha explotado!! Probabilidad de explosión: " + Mathf.RoundToInt(_balloonExplodeChance*100));
                if (_ballonsExploded==1)
                {
                    _ballonExplodeBadgeManager.BadgeAchieved();
                }
                else if (_ballonsExploded==100)
                {
                    _100BalloonsExplodedBadgeManager.BadgeAchieved();
                }
                //Partículas de explosion
            }
        }
    }
    #endregion
    #region Public Methods
    #endregion
    #region Private Methods
    public void OnPointerUp(PointerEventData eventData)
    {
        _isPressed = false;
        if(_currentBalloon != null)
        {
            if (_timePressed * _speed < 0.6)
            {
                Destroy(_currentBalloon);
            }
            else
            {
                _currentBalloon.transform.DOMoveY(_flyForce, 6);
                int money = Mathf.RoundToInt(_timePressed * _moneyMultiplier * _speed);
                _gameController.GainMoney(money);
                if (money >= 10 && !_first10Ballon)
                {
                    _ballon10BadgeManager.BadgeAchieved();
                    _first10Ballon=true;
                }
                if (money >= 500000 && !_first500KBallon)
                {
                    _ballon500KBadgeManager.BadgeAchieved();
                    _first500KBallon = true;
                }
                //Efecto de monedas cayendo
            }
        }    
    }

    public void OnPointerDown(PointerEventData eventData)
    {
       _isPressed = true;
       _timePressed = 0;

        if (_previousBalloon != null)
            Destroy(_previousBalloon);
        if(_currentBalloon != null)
            _previousBalloon = _currentBalloon;

        _currentBalloon = Instantiate(_currentBalloonModel, transform.position + new Vector3(350, 450, 0), Quaternion.identity, _balloonParentTransform);
    }
    public void UpgradeBalloon(int upgradeIndex)
    {
        if (upgradeIndex == 2)
            _currentBalloonModel = _balloonModel2;
        else if (upgradeIndex == 3)
            _currentBalloonModel = _balloonModel3;
        else
            _currentBalloonModel = _balloonModel1;
    }
    #endregion
}
