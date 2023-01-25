using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RhythmSystem;

public class FeedbackDisplayer : MonoBehaviour
{
    ObjectPool pool;
    public static FeedbackDisplayer Instance;

    private void Awake() 
    {
        if (Instance != null)
            return;

        Instance = this;
    }

    void Start()
    {
        pool = GetComponentInChildren<ObjectPool>();
    }

    public void CallFeedback(PrecisionScore precisionScore)
    {
        if (!pool)
            return;

        GameObject feedbackObject = pool.GetFromPool();
        FeedbackEffect feedbackEffect = feedbackObject.GetComponent<FeedbackEffect>();
        if (!feedbackEffect)
            return;

        feedbackEffect.Setup(precisionScore, transform.position);
    }
}
