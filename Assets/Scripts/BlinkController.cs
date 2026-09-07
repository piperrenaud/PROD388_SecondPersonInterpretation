using System.Collections;
using UnityEngine;

public class BlinkController : MonoBehaviour
{
    [Header("Head Material")]
    [SerializeField] private Renderer headRenderer;

    [Header("Textures")]
    [SerializeField] private Texture defaultTexture;
    [SerializeField] private Texture blinkTexture;

    [Header("Blink Timing")]
    [SerializeField] private float minBlinkTime = 2f;
    [SerializeField] private float maxBlinkTime = 6f;
    [SerializeField] private float blinkDurection = 0.1f;

    private Material headMaterial;
    private Texture currentExpression;

    private void Start()
    {
        //instance so original material isn't changed
        headMaterial = headRenderer.material;

        currentExpression = defaultTexture;
        headMaterial.mainTexture = currentExpression;

        StartCoroutine(BlinkRoutine());
    }

    private IEnumerator BlinkRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(minBlinkTime, maxBlinkTime);

            yield return new WaitForSeconds(waitTime);

            //blink
            headMaterial.mainTexture = blinkTexture;
            yield return new WaitForSeconds(blinkDurection);

            //return to normal
            headMaterial.mainTexture = currentExpression;
        }
    }

    public void SetExpression(Texture newExpression)
    {
        currentExpression = newExpression;

        headMaterial.mainTexture = currentExpression;
    }
}

