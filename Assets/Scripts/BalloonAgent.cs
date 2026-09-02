using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class BalloonAgent : MonoBehaviour
{
    #region Properties
    public GameController GameController;
    public GameObject ExplosionParticles;
    public float Speed;
    public float MoneyMultiplier;
    public float Durability;
    #endregion
    #region Fields
    [SerializeField] private float _timeGrowing;
    [SerializeField] private float _balloonExplodeChance;
    #endregion
    #region Unity Callbacks
    void Start()
    {
        _timeGrowing = 0;
    }
    void Update()
    {
        _timeGrowing += Time.deltaTime;
        transform.localScale = new Vector3(_timeGrowing * Speed/3, _timeGrowing * Speed/3, 1);
        GetComponentInChildren<TextMeshProUGUI>().text = "+" + Mathf.RoundToInt(_timeGrowing * MoneyMultiplier * Speed);
        float randomValue = Random.value;
        _balloonExplodeChance = Mathf.Pow(_timeGrowing * Speed / Durability, 2f);
        if (randomValue < _balloonExplodeChance * Time.deltaTime * 2f)
        {
            Debug.Log("El agente globo ha explotado!! Probabilidad de explosión: " + _balloonExplodeChance);
            int money = Mathf.RoundToInt(_timeGrowing * MoneyMultiplier * Speed);
            GameController.GainMoney(money);
            _timeGrowing = 0;
            Instantiate(ExplosionParticles, transform.position, Quaternion.identity);
            MoveToRandomPosition();
        }
    }
    #endregion
    #region Public Methods
    public void MoveToRandomPosition()
    {
        float posX = Random.Range(0, 400);
        float posY = Random.Range(0, 100);
        Vector2 position = new Vector2(posX, posY);
        GetComponent<RectTransform>().anchoredPosition = position;
    }
    #endregion
    #region Private Methods
    #endregion
}
