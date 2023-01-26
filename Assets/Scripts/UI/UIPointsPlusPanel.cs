using System.Collections;
using System.Collections.Generic;
using AudioAliase;
using UnityEngine;
using TMPro;
public class UIPointsPlusPanel : MonoBehaviour
{
    public delegate void EventScoreMultiplier(int amount);

    public static event EventScoreMultiplier OnMultiplierSend;
    private static GameObject prefab;
    public static UIPointsPlusPanel Instance;
   [SerializeField] private TextMeshProUGUI _text;
   [SerializeField] private CanvasGroup _canvasGroup;
    private int _score;
    private int _multiplier;
    float time = 0;
    float lifeTime = 5f;

    public Vector3 positionWorld;
    
    [SerializeField] [Aliase] private string AliasOn10;
    [SerializeField] [Aliase] private string AliasOn20;
    [SerializeField] [Aliase] private string AliasOn30;
    [SerializeField] [Aliase] private string AliasOn40;
    [SerializeField] [Aliase] private string AliasOn50;
    [SerializeField] [Aliase] private string AliasOn60;
    [SerializeField] [Aliase] private string AliasOn70;
    [SerializeField] [Aliase] private string AliasOn80;
    [SerializeField] [Aliase] private string AliasOn90;
    [SerializeField] [Aliase] private string AliasOn100;
    [SerializeField] [Aliase] private string LoseMultiplier;
    public int score
    {
        get => _score;
        set
        {
            _score = value;
            _text.text = "+" + _score;
            if(_multiplier > 1)
                _text.text +=" x"+_multiplier;
        }

    }

    public static void CreateUIPointsPlus(GameObject canvasGameObject, Vector3 position, int scoreValue)
    {
       // var objectui =  Instantiate(prefab,position.GetPositionInWorldToScreenPoint(), Quaternion.identity, canvasGameObject.transform);
       // var component = objectui.GetComponent<UIPointsPlusPanel>();
       Instance.gameObject.SetActive(true);
       Instance.score += scoreValue;
       Instance.positionWorld = position;
       Instance.time = 0;
       Instance._multiplier++;
       switch (Instance._multiplier)
       {
           case 20: AudioManager.PlaySoundAtPosition(Instance.AliasOn10); break;
           case 50: AudioManager.PlaySoundAtPosition(Instance.AliasOn20); break;
           case 80: AudioManager.PlaySoundAtPosition(Instance.AliasOn30); break;
           case 100: AudioManager.PlaySoundAtPosition(Instance.AliasOn40); break;
           case 120: AudioManager.PlaySoundAtPosition(Instance.AliasOn50); break;
           case 150: AudioManager.PlaySoundAtPosition(Instance.AliasOn60); break;
           case 180: AudioManager.PlaySoundAtPosition(Instance.AliasOn70); break;
           case 200: AudioManager.PlaySoundAtPosition(Instance.AliasOn80); break;
           case 250: AudioManager.PlaySoundAtPosition(Instance.AliasOn90); break;
           case 300: AudioManager.PlaySoundAtPosition(Instance.AliasOn100); break;
           default : break;
       }
       
    }
    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        PlayerInstance.eventPlayerDeath += PlayerInstanceOneventPlayerDeath;
        if (prefab == null)
        {
            prefab = gameObject;
            gameObject.SetActive(false);
        }
    }

    private void PlayerInstanceOneventPlayerDeath(PlayerInstance newplayer)
    {
        if (LevelManager.GetAlivePlayers.Count > 1)
        {
            _multiplier /= 2;
        }
        else
        {
            ResetMultiplierPoint();
        }
    }

    private void ResetMultiplierPoint()
    {
        time = 0;
        score = 0;
        _multiplier = 0;
        gameObject.SetActive(false);
        AudioManager.PlaySoundAtPosition(LoseMultiplier);
    }
    // Update is called once per frame
    void Update()
    {
        if (LevelManager.Instance.State == State.InGame)
        {
            if(time < lifeTime)
            {   
                //transform.position = positionWorld.GetPositionInWorldToScreenPoint();
                _canvasGroup.alpha = 1 - (time/lifeTime);
                transform.localScale = new Vector3(_canvasGroup.alpha,_canvasGroup.alpha,_canvasGroup.alpha);
                time += Time.deltaTime;
            }
            else
            {
                OnMultiplierSend?.Invoke(10 * _multiplier);
                ResetMultiplierPoint();
            }
        }
    }
}
