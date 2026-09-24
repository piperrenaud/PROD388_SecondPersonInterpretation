using System.Collections;
using UnityEngine;

public class ExpressionController : MonoBehaviour
{
    public enum Expression
    {
        Happy,
        Sad
    }

    [Header("Material")]
    [SerializeField] private Renderer headRenderer;
    [SerializeField] private string textureProperty = "_MainTex";

    [Header("Expressions")]
    [SerializeField] private Texture happyTexture;
    [SerializeField] private Texture sadTexture;

    [Header("Blink")]
    [SerializeField] private Texture eyesClosedTexture;
    [SerializeField] private float blinkIntervalMin = 3f;
    [SerializeField] private float blinkIntervalMax = 6f;
    [SerializeField] private float blinkDuration = 0.12f;

    [Header("Talking")]
    [SerializeField] private Texture mouthClosedTexture;
    [SerializeField] private Texture mouthOpenTexture;
    [SerializeField] private float mouthChangeInterval = 0.12f;

    private Expression currentExpression = Expression.Sad;

    private bool isTalking;
    private bool isBlinking;

    private Coroutine blinkCoroutine;
    private Coroutine talkingCoroutine;

    private Material headMaterial;

    private void Awake()
    {
        if (headRenderer == null)
        {
            Debug.LogError($"{name}: Had Renderer has not been assigned");
            return;
        }

        headMaterial = headRenderer.material;

        ApplyNormalExpression();
    }

    private void Start()
    {
        blinkCoroutine = StartCoroutine(BlinkRoutine());
    }

    public void SetHappy()
    {
        currentExpression = Expression.Happy;

        if (!isBlinking && !isTalking)
        {
            ApplyNormalExpression();
        }
    }

    public void SetSad()
    {
        currentExpression = Expression.Sad;

        if (!isBlinking && !isTalking)
        {
            ApplyNormalExpression();
        }
    }

    public void SetExpression(Expression expression)
    {
        currentExpression = expression;

        if (!isBlinking && !isTalking)
        {
            ApplyNormalExpression();
        }
    }

    private void ApplyNormalExpression()
    {
        if (headMaterial == null) return;

        switch (currentExpression)
        {
            case Expression.Happy:
                SetTexture(happyTexture);
                break;

            case Expression.Sad:
                SetTexture(sadTexture);
                break;
        }
    }

    private IEnumerator BlinkRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(blinkIntervalMin, blinkIntervalMax);

            yield return new WaitForSeconds(waitTime);

            yield return StartCoroutine(Blink());
        }
    }

    private IEnumerator Blink()
    {
        if (eyesClosedTexture == null) yield break;

        isBlinking = true;

        SetTexture(eyesClosedTexture);

        yield return new WaitForSeconds(blinkDuration);

        isBlinking = false;

        RefreshFace();
    }

    public void StartTalking()
    {
        if (isTalking) return;

        isTalking = true;

        if (talkingCoroutine != null) StopCoroutine(talkingCoroutine);

        talkingCoroutine = StartCoroutine(TalkingRoutine());
    }   
    
    public void StopTalking()
    {
        isTalking = false;

        if (talkingCoroutine != null)
        {
            StopCoroutine(talkingCoroutine);
            talkingCoroutine = null;
        }

        RefreshFace();
    }

    private IEnumerator TalkingRoutine()
    {
        bool mouthOpen = false;

        while (isTalking)
        {
            if (!isBlinking)
            {
                mouthOpen = !mouthOpen;

                if (mouthOpen) SetTexture(mouthOpenTexture);
                else SetTexture(mouthClosedTexture);
            }

            yield return new WaitForSeconds(mouthChangeInterval);
        }
    }

    private void RefreshFace()
    {
        if (isBlinking)
        {
            SetTexture(eyesClosedTexture);
        }
        else if (isTalking)
        {
            SetTexture(mouthClosedTexture);
        }
        else
        {
            ApplyNormalExpression();
        }
    }

    private void SetTexture(Texture texture)
    {
        if (headMaterial == null || texture  == null) return;

        headMaterial.SetTexture(textureProperty, texture);
    }

    public bool IsTalking()
    {
        return isTalking;
    }

    public bool IsBlinking()
    {
        return isBlinking;
    }

    public Expression GetCurrentExpression()
    {
        return currentExpression;
    }


}
