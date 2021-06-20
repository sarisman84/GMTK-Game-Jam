using System;
using System.Collections;
using System.Collections.Generic;
using Level.UI;
using Unity.VisualScripting;
using UnityEngine;

public class DoTweenAnimator : MonoBehaviour
{
    public bool playOnAwake;
    public List<GameObject> objectsToTween;
    public List<DoTweenAnimationClip> animationClips;
    private float accumilatedProcessTime = 0;

    public bool isPlaying { private set; get; }
    public float playTime { get; private set; }


    private void Awake()
    {
        if (playOnAwake)
            PlayAll();
    }

    public void PlayAll()
    {
        if (!isPlaying)
            StartCoroutine(TweenObjectsWithAllClips());
    }

    public void Play(DoTweenAnimationClip clip)
    {
        if (!isPlaying)
            StartCoroutine(TweenObjectWithClip(clip));
    }

    public IEnumerator TweenObjectsWithAllClips()
    {
        isPlaying = true;
        accumilatedProcessTime = 0;
        foreach (var animationClip in animationClips)
        {
            animationClip.PlayTween(objectsToTween, this);
            accumilatedProcessTime += animationClip.duration * objectsToTween.Count;
        }

        yield return new WaitForSeconds(accumilatedProcessTime);
        isPlaying = false;
    }

    public IEnumerator TweenObjectWithClip(DoTweenAnimationClip clip)
    {
        playTime = clip.duration;
        isPlaying = true;
        Debug.Log(clip.PlayTween(objectsToTween, this));
        yield return new WaitForSeconds(clip.duration);
        isPlaying = false;
        yield return new WaitForSeconds(0.2f);
    }


    public void AddObjectsToTween(GameObject owner)
    {
        if (objectsToTween == null)
            objectsToTween = new List<GameObject>();
        for (int i = 0; i < owner.transform.childCount; i++)
        {
            objectsToTween.Add(owner.transform.GetChild(i).gameObject);

            if (owner.transform.GetChild(i).childCount > 0)
            {
                AddObjectsToTween(owner.transform.GetChild(i).gameObject);
            }
        }
    }

    public void AddObjectsToTween(IEnumerable<GameObject> gameObjects)
    {
        if (objectsToTween == null)
            objectsToTween = new List<GameObject>();
        objectsToTween.AddRange(gameObjects);
    }
}