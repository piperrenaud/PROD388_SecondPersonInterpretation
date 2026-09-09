using System.Collections;
using UnityEngine;

public class FlashText : MonoBehaviour
{
    [Header("Flash Settings")]
    [SerializeField] private GameObject objectToFlash;
    [SerializeField] private float flashInterval = 0.1f;

    private void Start()
    {
        StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        while (true) 
        {
            objectToFlash.SetActive(false);
            yield return new WaitForSeconds(flashInterval);

            objectToFlash.SetActive(true);
            yield return new WaitForSeconds(flashInterval);
        }
    }
}
