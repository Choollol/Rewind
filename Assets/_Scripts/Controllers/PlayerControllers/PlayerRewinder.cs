using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerRewinder : MonoBehaviour
{
    public enum RewindType
    {
        Static, Bounce, Launch
    }

    private SpriteRenderer spriteRenderer;
    private TextMeshPro rewindsText;

    [SerializeField] private List<GameObject> rewindShadows;

    private Vector2 startPos;
    [SerializeField] private RewindType startRewindType;

    private RewindType rewindType = RewindType.Static;

    public int rewinds;

    [SerializeField] private GameObject rewindParticle;
    private bool doInstantiateRewindParticle;
    [SerializeField] private float rewindParticleInterval;
    [SerializeField] private float rewindParticleIntervalDecrement;
    private float rewindParticleTimer;

    private List<GameObject> rewindShadowList = new List<GameObject>();
    private void OnEnable()
    {
        EventMessenger.StartListening("Restart", StopRewind);
    }
    private void OnDisable()
    {
        EventMessenger.StopListening("Restart", StopRewind);
    }
    private void Start()
    {
        startPos = transform.position;

        spriteRenderer = GetComponent<SpriteRenderer>();
        rewindsText = transform.GetChild(0).GetComponent<TextMeshPro>();

        UpdateRewindsText();

        StartCoroutine(PreventRewind());
    }
    private void Update()
    {
        if (GameManager.isGameActive)
        {
            if (Input.GetButtonDown("Reset") || transform.position.y < -5.3)
            {
                ResetPosition();
            }
            if (Input.GetButtonDown("Undo") && rewindShadowList.Count > 0)
            {
                Undo();
            }
            if (Input.GetButtonDown("Rewind") && GameManager.canRewind && rewinds > 0)
            {
                StartCoroutine(StartRewind());
            }
        }
    }
    private IEnumerator HandleRewindParticles()
    {
        rewindParticleTimer = rewindParticleInterval;
        while (doInstantiateRewindParticle) {
            Instantiate(rewindParticle, transform.position, Quaternion.identity);
            yield return new WaitForSeconds(rewindParticleTimer);
            rewindParticleTimer -= rewindParticleIntervalDecrement * Time.deltaTime;
        }
        yield break;
    }
    private void StopRewind()
    {
        StopAllCoroutines();
        EventMessenger.TriggerEvent("UnfreezeTime");
        StartCoroutine(PreventRewind());
    }
    private IEnumerator StartRewind()
    {
        doInstantiateRewindParticle = true;
        StartCoroutine(HandleRewindParticles());
        GameManager.canRewind = false;
        AudioPlayer.PlaySound("Rewind Sound");
        EventMessenger.TriggerEvent("FreezeTime");
        EventMessenger.TriggerEvent("FreezePlayer");
        while (AudioPlayer.IsSoundPlaying("Rewind Sound"))
        {
            if (AudioPlayer.GetProgress("Rewind Sound") > 0.9f)
            {
                doInstantiateRewindParticle = false;
            }
            yield return null;
        }
        Rewind();
        EventMessenger.TriggerEvent("UnfreezeTime");
        EventMessenger.TriggerEvent("UnfreezePlayer");
        EventMessenger.TriggerEvent("ResetPlayerDirection");
        EventMessenger.TriggerEvent("PlayerRewinded");
        yield break;
    }
    private void Undo()
    {
        ResetPosition();
        UpdateRewinds(rewinds + 1);
        Destroy(rewindShadowList[rewindShadowList.Count - 1]);
        rewindShadowList.RemoveAt(rewindShadowList.Count - 1);
    }
    private IEnumerator PreventRewind()
    {
        GameManager.canRewind = false;
        yield return new WaitForSeconds(0.1f);
        GameManager.canRewind = true;
        yield break;
    }
    private void Rewind()
    {
        rewindShadowList.Add(Instantiate(rewindShadows[(int)rewindType], transform.position, Quaternion.identity));
        UpdateRewinds(rewinds - 1);
    }
    private void UpdateRewinds(int newRewinds)
    {
        rewinds = newRewinds;
        UpdateRewindsText();
        StartCoroutine(PreventRewind());
        ResetPosition();
    }
    private void ResetPosition()
    {
        rewindType = startRewindType;
        transform.position = startPos;
        GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        UpdateColor();
        StartCoroutine(PreventRewind());
        EventMessenger.TriggerEvent("ResetPlayerDirection");
    }
    private void UpdateRewindsText()
    {
        rewindsText.text = rewinds.ToString();
        if (rewinds == 0)
        {
            rewindsText.text = "";
        }
    }
    private void UpdateColor()
    {
        Color newColor = rewindShadows[(int)rewindType].GetComponent<SpriteRenderer>().color;
        spriteRenderer.color = new Color(newColor.r, newColor.g, newColor.b, 1);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        RewindType newRewindType = rewindType;
        if (collision.gameObject.CompareTag("StaticGate"))
        {
            newRewindType = RewindType.Static;
        }
        else if (collision.gameObject.CompareTag("BounceGate"))
        {
            newRewindType = RewindType.Bounce;
        }
        else if (collision.gameObject.CompareTag("LaunchGate"))
        {
            newRewindType = RewindType.Launch;
        }
        if (rewindType != newRewindType)
        {
            AudioPlayer.PlaySound("Gate Pass Sound");
            rewindType = newRewindType;
        }
        UpdateColor();
    }
}
