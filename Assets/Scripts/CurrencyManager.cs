using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurrencyManager : MonoBehaviour {
    public static CurrencyManager I;
    private float gold;
    private float dollar;
    private float caseCount;
    private float specialGiftCount;

    public TMP_Text _goldText;
    public TMP_Text _dollarText;
    public TMP_Text _caseText;
    public TMP_Text _specialGiftText;

    public RectTransform _toTheGoldUI;
    public RectTransform _toTheDollarUI;
    public RectTransform _toTheCaseUI;
    public RectTransform _toTheSpecialGiftUI;

    private void Start() {
        I = this;
    }

    private void Update() {
        SetText();
    }


    void SetText() {
        _goldText.text = "x" + gold;
        _dollarText.text = "x" + dollar;
        _caseText.text = "x" + caseCount;
        _specialGiftText.text = "x" + specialGiftCount;
    }
    
    // Coin going to UI.
    public void CoinPoolToGo(int quantity, Vector3 currentPos) {
        CurrencyPool.I.CurrencyAllocation(quantity, CurrencyPool.PoolType.Gold, _toTheGoldUI, currentPos);
    }
    
    // Dollar going to UI.
    public void DollarPoolToGo(int quantity, Vector3 currentPos) {
        CurrencyPool.I.CurrencyAllocation(quantity, CurrencyPool.PoolType.Dollar, _toTheDollarUI,  currentPos);
    }
    
    // Case going to UI.
    public void CasePoolToGo(int quantity, Vector3 currentPos) {
        CurrencyPool.I.CurrencyAllocation(quantity, CurrencyPool.PoolType.Case, _toTheCaseUI,  currentPos);
    }
    
    // SpecialGift going to UI.
    public void SpecialGiftPoolToGo(int quantity, Vector3 currentPos) {
        CurrencyPool.I.CurrencyAllocation(quantity, CurrencyPool.PoolType.SpecialGift, _toTheSpecialGiftUI,  currentPos);
    }
    
    public void EarnGold(float quantity) {
        gold += quantity;
    }
    
    public void EarnDollar(float quantity) {
        dollar += quantity;
    }

    public void EarnCase(float quantity) {
        caseCount += quantity;
    }

    public void EarnSpecialGift(float quantity) {
        specialGiftCount += quantity;
    }
    
    public void LoseGold(float quantity) {
        gold -= quantity;
    }
    
    public void LoseDollar(float quantity) {
        dollar -= quantity;
    }

    public void LoseCase(float quantity) {
        caseCount -= quantity;
    }

    public void LoseSpecialGift(float quantity) {
        specialGiftCount -= quantity;
    }

    public float GetGold() {
        return gold;
    }

    public float GetDollar() {
        return dollar;
    }

    public float GetCase() {
        return caseCount;
    }

    public float GetSpecialGift() {
        return specialGiftCount;
    }
}
