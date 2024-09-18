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
        Case
    }
    [Serializable]
    public class Pool {
        public GameObject[] Parent;
        public PoolType[] PoolType;

        public Transform[] Deactives;
        public Transform[] Actives;

        public void SetPool(ref Dictionary<PoolType, List<int>> parentDic) {
            Array.Resize(ref Actives, Parent.Length);
            Array.Resize(ref Deactives, Parent.Length);

            for(int i = 0; i < Parent.Length; i++) {
                Deactives[i] = Parent[i].transform.GetChild(0);
                Actives[i] = Parent[i].transform.GetChild(1);

                Parent[i].transform.GetChild(0).gameObject.SetActive(false);
                if (!parentDic.ContainsKey(PoolType[i])) {
                    parentDic.Add(PoolType[i], new List<int>());
                }
                parentDic[PoolType[i]].Add(i);
            }
        }
    } public Pool pool = new();
	
    public static CurrencyPool I;
    
    private Dictionary<PoolType, List<int>> _pools = new ();
	

    private void Start() {
        I = this;
        pool.SetPool(ref _pools);
    }
    
    public void CurrencyAllocation(int count, PoolType type, Transform targetParent, Vector3 currentPos, Action endAction = null, int parentIndex = 0) {
        if (count == 0) return;
        int totalObj = 0;
        if (count <= 50) totalObj = count;
        else {
            totalObj = 50;
            int remain = count - totalObj;
            if (type == PoolType.Gold) CurrencyManager.I.EarnGold(remain);
            if (type == PoolType.Dollar) CurrencyManager.I.EarnDollar(remain);
            if(type == PoolType.Case) CurrencyManager.I.EarnCase(remain, parentIndex);
        }

        for (int i = 0; i < totalObj; i++) {
            // check object pool, if there is enough object
            if(GetDeactivePool(type, parentIndex).childCount == 1) {
                string parentName = GetDeactivePool(type, parentIndex).GetChild(0).name;
                Transform instantiated = Instantiate(GetDeactivePool(type, parentIndex).GetChild(0));
                instantiated.transform.SetParent(GetDeactivePool(type, parentIndex));
                instantiated.name = parentName;
            }

            Transform target = GetDeactivePool(type, parentIndex).GetChild(0);
            target.localScale = Vector3.one;
            Vector3 defaultScale = target.localScale;

            Transform deactiveParent = GetDeactivePool(type, parentIndex);

            target.SetParent(GetActivePool(type, parentIndex));

            Action<Transform> end = (_transform) => {
                _transform.SetParent(deactiveParent);
                _transform.localScale = defaultScale;
                if (type == PoolType.Gold) CurrencyManager.I.EarnGold(1);
                if (type == PoolType.Dollar) CurrencyManager.I.EarnDollar(1);
                if(type == PoolType.Case) CurrencyManager.I.EarnCase(1, parentIndex);
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
            AnimationManager.SObjectsToAnim sObjectsToAnim = new();
            AnimationManager.AnimationProperties props = new(target, animationType, poses, end, 0, 0, UnityEngine.Random.Range(0.6f, 0.8f));
            sObjectsToAnim.Add(props);
            AnimationManager.I.ObjectsToAnim.Add(sObjectsToAnim);
        }

    }

	
    private Transform GetDeactivePool(PoolType type, int index) {
        return pool.Deactives[_pools[type][index]];
    }
	

    private Transform GetActivePool(PoolType type, int index) {
        return pool.Actives[_pools[type][index]];
    }
}
