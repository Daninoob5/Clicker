using UnityEngine;

public class BadgeManager : MonoBehaviour
{
    #region Properties
    #endregion
    #region Fields
    [SerializeField] private GameController _gameController;
    [Header("Reward")]
    [SerializeField] private long _moneyReward;
    [SerializeField] private enum _rewardTypes { money, agent}
    [SerializeField] private _rewardTypes _rewardType;

    [Header("Visuals")]
    [SerializeField] private GameObject _tick;
    [SerializeField] private GameObject _rewardOn;
    [SerializeField] private GameObject _rewardOff;

    [Header("Agent")]
    [SerializeField] private GameObject _agentExplosionParticles;
    [SerializeField] private float _agentSpeed;
    [SerializeField] private float _agentDurability;
    [SerializeField] private float _agentMoneyMultiplier;
    [SerializeField] private Sprite _balloonSprite;
    #endregion
    #region Unity Callbacks
    void Start()
    {

    }
    void Update()
    {

    }
    #endregion
    #region Public Methods
    public void BadgeAchieved()
    {
        _rewardOff.SetActive(false);
        _rewardOn.SetActive(true);
        _tick.SetActive(true);
    }
    public void GetReward()
    {
        if (_rewardType==_rewardTypes.money)
            _gameController.GainMoney(_moneyReward);
        else if (_rewardType == _rewardTypes.agent)
            _gameController.CreateAgent(_agentSpeed,_agentDurability,_agentMoneyMultiplier, _balloonSprite, _agentExplosionParticles);
        else
            Debug.LogError("Error en el tipo de recompensa del logro");
        _rewardOn.SetActive(false);
    }
    #endregion
    #region Private Methods
    #endregion
}
