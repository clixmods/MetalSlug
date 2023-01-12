using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class UIPointsPlusPanel : MonoBehaviour
{
    private static GameObject prefab;
   [SerializeField] private TextMeshProUGUI _text;
   [SerializeField] private CanvasGroup _canvasGroup;
    private int _score;
    float time = 0;
    float lifeTime = 0.75f;
    public int score
    {
        get => _score;
        set
        {
            _score = value;
            _text.text = "+" + _score;
        }

    }

    public static void CreateUIPointsPlus(GameObject canvasGameObject, Vector3 position, int scoreValue)
    {
       var objectui =  Instantiate(prefab,Camera.main.WorldToScreenPoint(position), Quaternion.identity, canvasGameObject.transform);
       var component = objectui.GetComponent<UIPointsPlusPanel>();
       objectui.SetActive(true);
       component.score = scoreValue;
    }
    // Start is called before the first frame update
    void Awake()
    {
        if (prefab == null)
        {
            prefab = gameObject;
            gameObject.SetActive(false);
        }
          
        
      
    }

    // Update is called once per frame
    void Update()
    {
        if(time < lifeTime)
        {   
            _canvasGroup.alpha = 1 - (time/lifeTime);
            transform.localScale = new Vector3(_canvasGroup.alpha,_canvasGroup.alpha,_canvasGroup.alpha);
            time += Time.deltaTime;
        }
        else
            Destroy(gameObject);
    }
}
