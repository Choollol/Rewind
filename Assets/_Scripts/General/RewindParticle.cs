using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewindParticle : MonoBehaviour
{
    [SerializeField] private float scaleChangeDuration;
    private float scaleChangeTimer = 0;

    private float startingScale;

    private void Start()
    {
        startingScale = transform.localScale.x;
    }

    void Update()
    {
        if (transform.localScale.x > 0)
        {
            transform.localScale = Vector2.one * Mathf.Lerp(startingScale, 0, scaleChangeTimer / scaleChangeDuration);
        }
        if (scaleChangeTimer < scaleChangeDuration)
        {
            scaleChangeTimer += Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
