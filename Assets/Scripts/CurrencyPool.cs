using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using Random = Unity.Mathematics.Random;
using UnityEngine.UI;

using DG.Tweening;

public class CurrencyPool : MonoBehaviour
{
     public enum PoolType {
        Gold,
        Dollar,
        Case,
    }
    [Serializable]
    public class Pool {
        public GameObject[] Parent;
        public PoolType[] PoolType;

        public Transform[] Deactives;
        public Transform[] Actives;

        public void SetPool(ref Dictionary<PoolType, int> parentDic) {
            Array.Resize(ref Actives, Parent.Length);
            Array.Resize(ref Deactives, Parent.Length);

            for(int i = 0; i < Parent.Length; i++) {
                Deactives[i] = Parent[i].transform.GetChild(0);
                Actives[i] = Parent[i].transform.GetChild(1);

                Parent[i].transform.GetChild(0).gameObject.SetActive(false);

                parentDic.Add(PoolType[i], i);
            }
        }
    } public Pool pool = new();
	
    public static CurrencyPool I;
    
    private Dictionary<PoolType, int> _pools = new ();
    private Dictionary<Transform, PoolType> _uiTypes = new ();
	

    private void Start() {
        I = this;
        pool.SetPool(ref _pools);
    }
    
    public void CurrencyAllocation(int count, PoolType type, Transform targetParent, Vector3 currentPos, Action endAction = null) {
        if (count == 0) return;
        int totalObj = 0;
        if (count <= 50) totalObj = count;
        else {
            totalObj = 50;
            int remain = count - totalObj;
            if (type == PoolType.Gold) CurrencyManager.I.EarnGold(remain);
            if (type == PoolType.Dollar) CurrencyManager.I.EarnDollar(remain);
            if(type == PoolType.Case) CurrencyManager.I.EarnCase(remain);
        }

        for (int i = 0; i < totalObj; i++) {
            // check object pool, if there is enough object
            if(GetDeactivePool(type).childCount == 1) {
                string parentName = GetDeactivePool(type).GetChild(0).name;
                Transform instantiated = Instantiate(GetDeactivePool(type).GetChild(0));
                instantiated.transform.SetParent(GetDeactivePool(type));
                instantiated.name = parentName;
            }

            Transform target = GetDeactivePool(type).GetChild(0);
            target.localScale = Vector3.one;
            Vector3 defaultScale = target.localScale;

            Transform deactiveParent = GetDeactivePool(type);

            target.SetParent(GetActivePool(type));

            Action<Transform> end = (_transform) => {
                _transform.SetParent(deactiveParent);
                _transform.localScale = defaultScale;
                if (type == PoolType.Gold) CurrencyManager.I.EarnGold(1);
                if (type == PoolType.Dollar) CurrencyManager.I.EarnDollar(1);
                if(type == PoolType.Case) CurrencyManager.I.EarnCase(1);
                targetParent.DOScale(new Vector3(1.3f,1.3f,1.3f), 0.2f).OnComplete(() => {
                    targetParent.DOScale(Vector3.one, 0.25f);
                });
                if (endAction != null) endAction();
            };
            Vector3 pos0 = currentPos;
            Vector3 pos1 = new Vector3(Screen.currentResolution.width / 2f, Screen.currentResolution.height / 2f, 0f);
            Vector3 pos2 = targetParent.position;
        
            List<Vector3> poses = new List<Vector3>() { 
                pos0,
                pos1,
                pos2
            };

            AnimationManager.Type animationType = AnimationManager.Type.QuadraticThree;

            AnimationManager.AnimationProperties props = new(target, animationType, poses, end, 0, 0, UnityEngine.Random.Range(0.6f, 0.8f));
            AnimationManager.I.ObjectsToAnim.Add(props);
        }

    }

	
    private Transform GetDeactivePool(PoolType type) {
        return pool.Deactives[_pools[type]];
    }
	

    private Transform GetActivePool(PoolType type) {
        return pool.Actives[_pools[type]];
    }

	
    public Dictionary<Transform,PoolType> GetUITypes() {
        return _uiTypes;
    }

    private void OnValidate() {
        
    }
}
