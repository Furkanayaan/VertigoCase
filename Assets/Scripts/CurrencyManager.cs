using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class CurrencyManager : MonoBehaviour {
    public static CurrencyManager I;

    [Serializable]
    public class CCases {
        public float[] casesCount;
        public TMP_Text[] _casesText;
        public RectTransform[] _casesUI;
    }
    private float gold;
    private float dollar;

    public TMP_Text _goldText;
    public TMP_Text _dollarText;

    public RectTransform _toTheGoldUI;
    public RectTransform _toTheDollarUI;

    public CCases _cases;

    private void Start() {
        Array.Resize(ref _cases.casesCount, _cases._casesText.Length );
        I = this;
    }

    private void Update() {
        SetText();
    }


    void SetText() {
        _goldText.text = "x" + gold;
        _dollarText.text = "x" + dollar;
        for (int i = 0; i < _cases._casesText.Length; i++) {
            _cases._casesText[i].text = "x" + _cases.casesCount[i];
        }
        
    }
    
    // Coin going to UI.
    public void CoinPoolToGo(int quantity, Vector3 currentPos, int index = 0) {
        CurrencyPool.I.CurrencyAllocation(quantity, CurrencyPool.PoolType.Gold, _toTheGoldUI, currentPos, null, index);
    }
    
    // Dollar going to UI.
    public void DollarPoolToGo(int quantity, Vector3 currentPos, int index = 0) {
        CurrencyPool.I.CurrencyAllocation(quantity, CurrencyPool.PoolType.Dollar, _toTheDollarUI,  currentPos, null, index);
    }
    
    // Case going to UI.
    public void CasePoolToGo(int quantity, Vector3 currentPos, int index) {
        CurrencyPool.I.CurrencyAllocation(quantity, CurrencyPool.PoolType.Case, _cases._casesUI[index],  currentPos, null, index);
    }
    
    public void EarnGold(float quantity) {
        gold += quantity;
    }
    
    public void EarnDollar(float quantity) {
        dollar += quantity;
    }

    public void EarnCase(float quantity, int index) {
        _cases.casesCount[index] += quantity;
    }
    
    public void LoseGold(float quantity) {
        gold -= quantity;
    }

    public float GetGold() {
        return gold;
    }
}
