using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
        public static AnimationManager I;

    public enum Type {
        Linear,
        QuadraticThree,
        QuadraticFour
    }

    public class AnimationProperties {
        public Transform TargetTransform; 
        public Type AnimType;
        public List<Vector3> Posses;
        public Action<Transform> Action;
        public int AnimCurvePos;
        public int AnimCurveScale;
        public float Speed = 1f;
        public float Time = 0f;

        public AnimationProperties(Transform targetTransform, Type animType, List<Vector3> posses, Action<Transform> action, int animCurvePos, int animCurveScale, float speed = 1f, float time = 0f) {
            TargetTransform = targetTransform;
            AnimType = animType;
            Posses = posses;
            Action = action;
            AnimCurvePos = animCurvePos;
            AnimCurveScale = animCurveScale;
            Speed = speed;
            Time = time;
        }
    }
    
    [Serializable]
    public struct SObjectsToAnim {


        public List<Transform> Transforms;

        public List<List<Vector3>> Posses; // length == 2; 0 start, 1 end +++++ length == 4; 0 start, 1 first middle , 2 second middle, 3 end
        public List<Vector3> StartScales;

        public List<float> Times;
        public List<float> Speeds;

        public List<Type> AnimTypes;
        public List<int> animationCurvePoses;
        public List<int> animationCurveScales;

        public List<Action<Transform>> Actions;

        public SObjectsToAnim(int i) {
            Transforms = new();
            Posses = new();
            StartScales = new();
            Times = new();
            Speeds = new();
            AnimTypes = new();

            animationCurvePoses = new();
            animationCurveScales = new();

            Actions = new();
        }

        public void Add(AnimationProperties animationProperties) {
            if(animationProperties == null || animationProperties.TargetTransform == null) {
                return;
            }

            Transforms.Add(animationProperties.TargetTransform);
            Posses.Add(animationProperties.Posses);
            StartScales.Add(animationProperties.TargetTransform.localScale);
            Actions.Add(animationProperties.Action);
            Speeds.Add(animationProperties.Speed);

            animationCurvePoses.Add(animationProperties.AnimCurvePos);
            animationCurveScales.Add(animationProperties.AnimCurveScale);
            AnimTypes.Add(animationProperties.AnimType);

            Times.Add(animationProperties.Time);
        }
        public void Remove(Transform targetTransform) {
            int index = Transforms.IndexOf(targetTransform);
            
            if(index == -1) return;

            try {
                if(Actions[index] != null) 
                    Actions[index].Invoke(targetTransform);
            }
            catch (System.Exception e) {
                string error = e.ToString();
            }
                
            Transforms.RemoveAt(index);
            Posses.RemoveAt(index);
            StartScales.RemoveAt(index);
            Times.RemoveAt(index);
            Speeds.RemoveAt(index);
            animationCurvePoses.RemoveAt(index);
            animationCurveScales.RemoveAt(index);
            AnimTypes.RemoveAt(index);
            Actions.RemoveAt(index);
        }
        
        public void RemoveAt(int index) {
            if(index < 0) return;

            try {
                if(Actions[index] != null && Transforms[index] != null) 
                    Actions[index].Invoke(Transforms[index]);
            }
            catch (System.Exception e) {
                string error = e.ToString();
            }
                
            Transforms.RemoveAt(index);
            Posses.RemoveAt(index);
            StartScales.RemoveAt(index);
            Times.RemoveAt(index);
            Speeds.RemoveAt(index);
            animationCurvePoses.RemoveAt(index);
            animationCurveScales.RemoveAt(index);
            AnimTypes.RemoveAt(index);
            Actions.RemoveAt(index);
        }
    }
    
    internal SObjectsToAnim ObjectsToAnim = new(0);

    [SerializeField] private AnimationCurve[] curvesPos;
    [SerializeField] private AnimationCurve[] curvesScale;

    public void Awake() {
        I = this;
    }

    public void Update() {
        OneTimeAnimations();
    }


    public void OneTimeAnimations() {
        if(ObjectsToAnim.Transforms.Count == 0) return; 
        
        for (int i = ObjectsToAnim.Transforms.Count - 1; i >= 0; i--) {
            if (ObjectsToAnim.Transforms[i] == null) {
                ObjectsToAnim.RemoveAt(i);
                return;
            }
            if(!ObjectsToAnim.Transforms[i].gameObject.activeSelf) continue;

            float evaluated = curvesPos[ObjectsToAnim.animationCurvePoses[i]].Evaluate(ObjectsToAnim.Times[i]);
            float evaluatedScale = curvesScale[ObjectsToAnim.animationCurveScales[i]].Evaluate(ObjectsToAnim.Times[i]);

            ObjectsToAnim.Transforms[i].position = PosReturner(i, evaluated);
            ObjectsToAnim.Transforms[i].localScale = ObjectsToAnim.StartScales[i] + Vector3.one * evaluatedScale;

            ObjectsToAnim.Times[i] += Time.smoothDeltaTime / ObjectsToAnim.Speeds[i];
            
            if (ObjectsToAnim.Times[i] > 1.0f) {
                ObjectsToAnim.Transforms[i].position = PosReturner(i, curvesPos[ObjectsToAnim.animationCurvePoses[i]].keys[^1].value);
                ObjectsToAnim.Transforms[i].localScale = ObjectsToAnim.StartScales[i] + Vector3.one * curvesScale[ObjectsToAnim.animationCurveScales[i]].keys[^1].value;

                ObjectsToAnim.Remove(ObjectsToAnim.Transforms[i]);
            }
        }
    }

    
    public Vector3 PosReturner(int index, float time) {
        switch (ObjectsToAnim.AnimTypes[index]) {
            default:
                return new Vector3();
            case Type.Linear:
                return Linear(index, time);
            
            case Type.QuadraticThree:
                return QuadraticThree(index, time);
            
            case Type.QuadraticFour:
                return QuadraticFour(index, time);
        }
    }

    public Vector3 Linear(int index, float t) {
        return ObjectsToAnim.Posses[index][0] + t * (ObjectsToAnim.Posses[index][1] - ObjectsToAnim.Posses[index][0]);
    }
    
    public Vector3 QuadraticThree(int index, float t) {
        Vector3 p = (1f - t) * ((1 - t) * ObjectsToAnim.Posses[index][0] + t * ObjectsToAnim.Posses[index][1]) + t * ((1 - t) * ObjectsToAnim.Posses[index][1] + t * ObjectsToAnim.Posses[index][2]);
        return p;
    }
    
    public Vector3 QuadraticFour(int index, float t) {
        Vector3 p = Mathf.Pow(1f - t, 3f) * ObjectsToAnim.Posses[index][0] + 
                    3f * Mathf.Pow(1f - t, 2f) * t * ObjectsToAnim.Posses[index][1] + 
                    3f * (1f - t) * t * t * ObjectsToAnim.Posses[index][2] + 
                    Mathf.Pow(t, 3f) * ObjectsToAnim.Posses[index][3];
        
        return p;
    }
}
