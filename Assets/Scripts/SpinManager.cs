using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using WheelObjectsType;
using ToolSet;
using UnityEngine.SceneManagement;

public class SpinManager : MonoBehaviour {
    public MainObjects[] allMainObjects;
    public DetermineEveryZone _determineEveryZone;
    [Space(10)]
    [SerializeField]private Button _wheelSpinButton;
    [SerializeField]private Button _wheelSpinCloseTap;
    [SerializeField]private Button _restartButton;
    [SerializeField]private Button _reviveButton;
    [SerializeField]private Button _spinToWinButton;
    private float _aimNumberAsDegree = 0f;
    private float _wheelTime;
    private int selectedSection;
    private bool bSpin = false;
    private AnimationCurve wheelAnimationCurve;
    private int wheelSpinCount;
    private float totalWeights;
    private float PopUpTimer;
    private Dictionary<ObjectsType, MainObjects> TypeToProp = new Dictionary<ObjectsType, MainObjects>();

    public Transform _wheelSpin;
    public Transform _wheelSpinClose;
    public Transform _restart;
    public Transform _revive;
    public Transform _spinToWin;
    public RectTransform _wheelMiddle;
    public Sprite _defaultBG;
    public Sprite _goldBG;
    public Sprite _silverBG;
    public GameObject _whellBG;
    public GameObject _wheelBGParent;
    public TMP_Text _wheelTitle;
    public TMP_Text _totalSpinCount;
    public float reviveMultiplier;
    
    
    
    [SerializeField]private Transform[] detectedObject;
    [SerializeField]private float[] weights;
    
    public GameObject _bombUI;
    public GameObject _dontHaveEnoughCoinUI;

    List<int> _objectCount = new List<int>();

    private void OnValidate() {
        if (_wheelSpinButton == null) {
            _wheelSpinButton = _wheelSpin.GetComponent<Button>();
        }

        if (_wheelSpinCloseTap == null) {
            _wheelSpinCloseTap = _wheelSpinClose.GetComponent<Button>();
        }

        if (_restartButton == null) {
            _restartButton = _restart.GetComponent<Button>();
        }

        if (_reviveButton == null) {
            _reviveButton = _revive.GetComponent<Button>();
        }

        if (_spinToWinButton == null) {
            _spinToWinButton = _spinToWin.GetComponent<Button>();
        }
    }

    void Start()
    {
        for (int i = 0; i < allMainObjects.Length; i++) {
            TypeToProp.Add(allMainObjects[i]._objectType, allMainObjects[i]);
        }
        
        _wheelSpinButton.onClick.AddListener(() => {
            SpinWheel();
        });
        
        _wheelSpinCloseTap.onClick.AddListener(() => {
            _wheelBGParent.SetActive(false);
        });
        
        _restartButton.onClick.AddListener(() => {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        });
        
        _spinToWinButton.onClick.AddListener(() => {
            _wheelBGParent.SetActive(true);
        });
        SetImagesForEveryZone();
    }
    void Update() {
        WheelController();
        SpinAnimation();
        ControlPopUP();
    }

    
    public void ControlPopUP() {
        if (!_dontHaveEnoughCoinUI.activeSelf) return;
        PopUpTimer += Time.deltaTime;
        if (PopUpTimer >= 1f) {
            _dontHaveEnoughCoinUI.gameObject.SetActive(false);
            PopUpTimer = 0f;
        }
    }

    public void SetImagesForEveryZone() {
        int indexOfZone = wheelSpinCount;
        if (indexOfZone > _determineEveryZone._zoneProps.Length) {
            indexOfZone -= _determineEveryZone._zoneProps.Length;
        }
        for (int i = 0; i < detectedObject.Length; i++) {
            int objectLevel = _determineEveryZone._zoneProps[indexOfZone]._sliceProps[i]._levelOfObject;
            int objectCount = _determineEveryZone._zoneProps[indexOfZone]._sliceProps[i]._countOfObject;
            Sprite objectSprite = _determineEveryZone._zoneProps[indexOfZone]
                ._sliceProps[i]._mainObject._objects[objectLevel].GetComponent<SpriteRenderer>().sprite;

            float objectWeight = _determineEveryZone._zoneProps[indexOfZone]._sliceProps[i]._weightOfObject;
            
            //Determine Sprite
            detectedObject[i].GetChild(0).GetComponent<Image>().sprite = objectSprite;
            detectedObject[i].GetChild(0).name = objectSprite.name;

            //Determine object count
            detectedObject[i].GetChild(1).GetComponent<TMP_Text>().text = "x" + objectCount;
            
            //Determine object weights
            weights[i] = objectWeight;
            if (i > 0) weights[i] = weights[i-1] + objectWeight;
            totalWeights += objectWeight;
            _objectCount.Add(objectCount);
        }
    }

    void WheelController() {
        if (_whellBG.transform.parent.gameObject.activeSelf) _spinToWin.gameObject.SetActive(false);
        else _spinToWin.gameObject.SetActive(true);
        if (!bSpin) {
            _wheelSpin.gameObject.SetActive(true);
            _wheelSpinClose.gameObject.SetActive(true);
        }
        else {
            _wheelSpin.gameObject.SetActive(false);
            _wheelSpinClose.gameObject.SetActive(false);
        }
        _totalSpinCount.text = "Total Spin Count: " + wheelSpinCount;
        if (wheelSpinCount % 5 == 0 && wheelSpinCount % 30 != 0) {
            _whellBG.GetComponent<Image>().sprite = _silverBG;
            _wheelTitle.text = "SILVER SPIN";
            _wheelTitle.color = new Color(0.75f, 0.75f, 0.75f, 1f);
        }
        else if (wheelSpinCount % 30 == 0 && wheelSpinCount != 0) {
            _whellBG.GetComponent<Image>().sprite = _goldBG;
            _wheelTitle.text = "GOLDEN SPIN";
            _wheelTitle.color = new Color(1f, 0.75f, 0f, 1f);
        }

        else {
            _whellBG.GetComponent<Image>().sprite = _defaultBG;
            _wheelTitle.text = "NORMAL SPIN";
            _wheelTitle.color = new Color(0.75f, 0.25f, 0.75f, 1f);
        }
    }
    
    public void SpinWheel() {
        if (bSpin) return;
        selectedSection = GetRandomWeightedIndex();
        int totalWheelSpin = Random.Range(3, 5);
        _aimNumberAsDegree = Mathf.RoundToInt(totalWheelSpin * 360f + 360f + CalculateAngle(selectedSection));
        wheelAnimationCurve = new AnimationCurve();
        wheelAnimationCurve.AddKey(0, 0);
        wheelAnimationCurve.AddKey(_aimNumberAsDegree * Random.Range(60f, 80f) / 100f , _aimNumberAsDegree * Random.Range(80f, 101f) / 100f);
        wheelAnimationCurve.AddKey(_aimNumberAsDegree, _aimNumberAsDegree);
        bSpin = true;
    }
    
    void SpinAnimation() {
        if (bSpin) {
            _wheelTime += Time.deltaTime * _aimNumberAsDegree / 4f;
            _wheelMiddle.eulerAngles = Vector3.forward * wheelAnimationCurve.Evaluate(_wheelTime);
            if (_wheelTime >= _aimNumberAsDegree+100f) {
                wheelSpinCount++;
                bSpin = false;
                _wheelTime = 0;
                totalWeights = 0;
                GetRewards(detectedObject[selectedSection].GetChild(0).GetComponent<RectTransform>());
                _objectCount.Clear();
                SetImagesForEveryZone();
            }
        }
    }

    
    public void GetRewards(RectTransform rewardTransform) {
        
        
        ObjectsType type = Tools.StringToType(rewardTransform.name);
        int count = _objectCount[selectedSection];
        if (type == ObjectsType.Coin) CurrencyManager.I.CoinPoolToGo(count, rewardTransform.position);
        
        if (type == ObjectsType.Dollar) CurrencyManager.I.DollarPoolToGo(count, rewardTransform.position);
        
        if (type == ObjectsType.Case) CurrencyManager.I.CasePoolToGo(count, rewardTransform.position);
        
        if (type == ObjectsType.Bomb) {
            _wheelBGParent.SetActive(false);
            _bombUI.SetActive(true);
            float reviveCost = (wheelSpinCount + 1) * reviveMultiplier;
            _revive.transform.GetChild(2).GetComponent<TMP_Text>().text = reviveCost.ToString();
            bool bEnoughGold = CurrencyManager.I.GetGold() - reviveCost >= 0;
            _reviveButton.onClick.RemoveAllListeners();
            _reviveButton.onClick.AddListener(() => {
                if (bEnoughGold) {
                    CurrencyManager.I.LoseGold(reviveCost);
                    _bombUI.SetActive(false);
                    _wheelBGParent.SetActive(true);
                }
                else _dontHaveEnoughCoinUI.SetActive(true);
            });
        }
    }
    
    int GetRandomWeightedIndex() {
        float randomPoint = Random.Range(0, totalWeights);
        for (int i = 1; i < weights.Length; i++) {
            if (randomPoint >= weights[i - 1] && randomPoint < weights[i]) {
                return i;
            }
        }
        return 0;
    }
    
    float CalculateAngle(int sectionIndex) {
        float anglePerSection = 360f / detectedObject.Length;
        return 360f - (sectionIndex * anglePerSection);
    }
    
    
}
