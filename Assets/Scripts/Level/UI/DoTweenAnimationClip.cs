using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace Level.UI
{
    [CreateAssetMenu(fileName = "New Animation Clip", menuName = "GMTK/Animation/Create Clip", order = 0)]
    public class DoTweenAnimationClip : ScriptableObject
    {
        public float duration;
        public float delayBetweenEachTween;
        public TweenType tweenType;
        public float targetValue;
        public float startingValue;

        public enum TweenType
        {
            Fade
        }


        public string PlayTween(IEnumerable<GameObject> objectsToTween, DoTweenAnimator animator)
        {
            animator.StartCoroutine(ApplyTweenOnAllObjects(objectsToTween
                .ConvertTo(g => new RegistedGameObject(g, animator.transform.position)).ToHeap()));
            return $"Playing {name}";
        }

        private IEnumerator ApplyTweenOnAllObjects(Heap<RegistedGameObject> objectsToTween)
        {
            Debug.Log($"{name} -> Current object count: {objectsToTween.Count}");
            int previousObjHeapIndex = 0;
            while (objectsToTween.Count > 0)
            {
                RegistedGameObject obj = objectsToTween.RemoveFirst();
                ApplyTween(obj.obj);
                if (previousObjHeapIndex == obj.HeapIndex)
                {
                    continue;
                }

                previousObjHeapIndex = obj.HeapIndex;
                yield return new WaitForSeconds(delayBetweenEachTween);
            }

            yield return null;
        }

        private void ApplyTween(GameObject obj)
        {
            switch (tweenType)
            {
                case TweenType.Fade:
                    if (obj.GetComponent<TMP_Text>() is { } textPro)
                    {
                        textPro.alpha = startingValue;
                        Debug.Log($"{name} -> Reset value to starting value.");
                        textPro.DOFade(targetValue, duration);
                        Debug.Log($"{name} -> Doing a DOFade call.");
                    }

                    if (obj.GetComponent<Text>() is { } text)
                    {
                        text.color = new Color(text.color.r, text.color.g, text.color.b, startingValue);
                        Debug.Log($"{name} -> Reset value to starting value.");
                        text.DOFade(targetValue, duration);
                        Debug.Log($"{name} -> Doing a DOFade call.");
                    }

                    if (obj.GetComponent<Image>() is { } image)
                    {
                        image.color = new Color(image.color.r, image.color.g, image.color.b, startingValue);
                        Debug.Log($"{name} -> Reset value to starting value.");
                        image.DOFade(targetValue, duration);
                        Debug.Log($"{name} -> Doing a DOFade call.");
                    }

                    break;
                default:
                    break;
            }
        }


        public struct RegistedGameObject : IHeapItem<RegistedGameObject>
        {
            public GameObject obj;

            public RegistedGameObject(GameObject obj, Vector3 center)
            {
                this.obj = obj;
                HeapIndex = Mathf.RoundToInt(Vector3.Distance(obj.transform.position, center));
            }

            public int CompareTo(RegistedGameObject other)
            {
                return other.HeapIndex.CompareTo(HeapIndex);
            }

            public int HeapIndex { get; set; }
        }
    }
}