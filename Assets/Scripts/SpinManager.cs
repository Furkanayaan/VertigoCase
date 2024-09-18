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
    [SerializeField] private Button _takeYourRewardButton;
    private float _aimNumberAsDegree = 0f;
    private float _wheelTime;
    private int selectedSection;
    private bool bSpin = false;
    private AnimationCurve wheelAnimationCurve;
    private int wheelSpinCount;
    private float totalWeights;
    private float PopUpTimer;

    public Transform _wheelSpin;
    public Transform _wheelSpinClose;
    public Transform _restart;
    public Transform _revive;
    public Transform _spinToWin;
    public RectTransform _wheelMiddle;
    public Sprite _defaultBG;
    public Sprite _defaultArrow;
    public Sprite _goldBG;
    public Sprite _goldArrow;
    public Sprite _silverBG;
    public Sprite _silverArrow;
    public GameObject _whellBG;
    public GameObject _arrow;
    public GameObject _wheelBGParent;
    public TMP_Text _wheelTitle;
    public TMP_Text _totalSpinCount;
    public float reviveMultiplier;
    public GameObject _gainRewards;
    public Transform _rewardSprite;
    public Transform _takeYourReward;
    
    
    
    
    
    [SerializeField]private Transform[] detectedObject;
    [SerializeField]private float[] weights;
    
    public GameObject _bombUI;
    public GameObject _dontHaveEnoughCoinUI;

    List<int> _objectCount = new List<int>();
    private List<int> _levelCount = new List<int>();

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

        if (_takeYourRewardButton == null) {
            _takeYourRewardButton = _takeYourReward.GetComponent<Button>();
        }
    }

    void Start() {
        
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
        
        _takeYourRewardButton.onClick.AddListener(() => {
            _gainRewards.gameObject.SetActive(false);
        });
        SetImagesForEveryZone();
    }
    void Update() {
        WheelController();
        SpinAnimation();
        ControlPopUP();
    }

    
    public void ControlPopUP() {
        //Control of dont have enough coin ui
        if (!_dontHaveEnoughCoinUI.activeSelf) return;
        PopUpTimer += Time.deltaTime;
        if (PopUpTimer >= 1f) {
            _dontHaveEnoughCoinUI.gameObject.SetActive(false);
            PopUpTimer = 0f;
        }
    }

    public void SetImagesForEveryZone() {
        int indexOfZone = wheelSpinCount;
        while (indexOfZone >= _determineEveryZone._zoneProps.Length) {
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
            //Get every possible rewards count
            _objectCount.Add(objectCount);
            //Get every possible rewards level
            _levelCount.Add(objectLevel);
        }
    }

    void WheelController() {
        if (_wheelBGParent.activeSelf) _spinToWin.gameObject.SetActive(false);
        else _spinToWin.gameObject.SetActive(true);
        //Control of Spin button and close button
        if (bSpin || AnimationManager.I.ObjectsToAnim.Count > 0) {
            _wheelSpin.gameObject.SetActive(false);
            _wheelSpinClose.gameObject.SetActive(false);
        }
        else {
            _wheelSpin.gameObject.SetActive(true);
            _wheelSpinClose.gameObject.SetActive(true);
        }
        _totalSpinCount.text = "Current Spin Index:  " + (wheelSpinCount + 1);
        //Set golden spin here
        if (wheelSpinCount % 30 == 29) {
            _wheelMiddle.GetComponent<Image>().sprite = _goldBG;
            _arrow.GetComponent<Image>().sprite = _goldArrow;
            _wheelTitle.text = "GOLDEN SPIN";
            _wheelTitle.color = new Color(0.88f, 0.7f, 0.32f, 1f);
        }
        //Set silver spin here
        else if (wheelSpinCount % 5 == 4) {
            _wheelMiddle.GetComponent<Image>().sprite = _silverBG;
            _arrow.GetComponent<Image>().sprite = _silverArrow;
            _wheelTitle.text = "SILVER SPIN";
            _wheelTitle.color = new Color(0.5f, 0.5f, 0.5f, 1f);
        }
        //Set bronze spin here
        else {
            _wheelMiddle.GetComponent<Image>().sprite = _defaultBG;
            _arrow.GetComponent<Image>().sprite = _defaultArrow;
            _wheelTitle.text = "BRONZE SPIN";
            _wheelTitle.color = new Color(0.58f, 0.34f, 0.11f, 1f);
        }
    }
    
    public void SpinWheel() {
        if (bSpin) return;
        selectedSection = GetRandomWeightedIndex();
        int totalWheelSpin = Random.Range(3, 5);
        _aimNumberAsDegree = Mathf.RoundToInt(totalWheelSpin * 360f + 360f + CalculateAngle(selectedSection));
        wheelAnimationCurve = new AnimationCurve();
        wheelAnimationCurve.AddKey(0, 0);
        //The animation value and duration are being calculated.
        wheelAnimationCurve.AddKey(_aimNumberAsDegree * Random.Range(60f, 80f) / 100f , _aimNumberAsDegree * Random.Range(80f, 101f) / 100f);
        wheelAnimationCurve.AddKey(_aimNumberAsDegree, _aimNumberAsDegree);
        //The spin begin
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
                _levelCount.Clear();
                SetImagesForEveryZone();
            }
        }
    }

    
    //Reward acquisitions and currency purchases are being made.
    public void GetRewards(RectTransform rewardTransform) {
        ObjectsType type = Tools.StringToType(rewardTransform.name);
        int count = _objectCount[selectedSection];
        int level = _levelCount[selectedSection];
        if (type == ObjectsType.Coin) CurrencyManager.I.CoinPoolToGo(count, rewardTransform.position, 0);
        
        else if (type == ObjectsType.Dollar) CurrencyManager.I.DollarPoolToGo(count, rewardTransform.position);
        
        else if (type == ObjectsType.Case) CurrencyManager.I.CasePoolToGo(count, rewardTransform.position, level);
        
        else if (type == ObjectsType.Grenade) {
            _wheelBGParent.SetActive(false);
            _bombUI.SetActive(true);
            //Revive cost is being calculated
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

        else {
            
            AnimationManager.Type animationType = AnimationManager.Type.QuadraticThree;
            Vector3 currentPos = rewardTransform.position;

            GameObject cloneObj = Instantiate(rewardTransform.gameObject, currentPos, Quaternion.identity);
            cloneObj.name = cloneObj.GetComponent<Image>().name;
            cloneObj.transform.SetParent(_whellBG.transform);
            List<Vector3> poses = new List<Vector3>() {
                currentPos,
                new Vector3(0f, -Screen.height / 2f),
                new Vector3(Screen.width / 2f, Screen.height / 2f, 0f),
            };
            float speed = 1f;

            Action<Transform> endAction = (_transform) => {
                _gainRewards.SetActive(true);
                _rewardSprite.GetComponent<Image>().sprite = _transform.GetComponent<Image>().sprite;
                Destroy(_transform.gameObject);
                bSpin = false;
            };
            AnimationManager.SObjectsToAnim sObjectsToAnim = new();
            AnimationManager.AnimationProperties props = new(cloneObj.transform, animationType, poses, endAction, 0, 1,
                speed);
            sObjectsToAnim.Add(props);
            AnimationManager.I.ObjectsToAnim.Add(sObjectsToAnim);
        }
    }
    ////Random numbers are returned according to the total weight. The object in the range of the generated number is selected.
    int GetRandomWeightedIndex() {
        float randomPoint = Random.Range(0, totalWeights);
        for (int i = 1; i < weights.Length; i++) {
            if (randomPoint >= weights[i - 1] && randomPoint < weights[i]) {
                return i;
            }
        }
        return 0;
    }
    
    //Determine the rotation range according to the selected object
    float CalculateAngle(int sectionIndex) {
        float anglePerSection = 360f / detectedObject.Length;
        return 360f - (sectionIndex * anglePerSection);
    }
    
    
}
