using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerRewinder : MonoBehaviour
{
    public enum RewindType
    {
        Static, Bounce
    }

    private SpriteRenderer spriteRenderer;
    private TextMeshPro rewindsText;

    [SerializeField] private List<GameObject> rewindShadows;

    private Vector2 startPos;
    [SerializeField] private RewindType startRewindType;

    private RewindType rewindType = RewindType.Static;

    public int rewinds;
    private void OnEnable()
    {
        EventMessenger.StartListening("Rewind", Rewind);
    }
    private void OnDisable()
    {
        EventMessenger.StopListening("Rewind", Rewind);
    }
    private void Start()
    {
        startPos = transform.position;

        spriteRenderer = GetComponent<SpriteRenderer>();
        rewindsText = transform.GetChild(0).GetComponent<TextMeshPro>();

        UpdateRewindsText();

        StartCoroutine(PreventRewind());
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
        Instantiate(rewindShadows[(int)rewindType], transform.position, Quaternion.identity);
        transform.position = startPos;
        GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        rewindType = startRewindType;
        UpdateColor();
        EventMessenger.TriggerEvent("PlayerRewinded");
        rewinds--;
        UpdateRewindsText();
        StartCoroutine(PreventRewind());
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
        if (rewindType != newRewindType)
        {
            AudioPlayer.PlaySound("Gate Pass Sound");
            rewindType = newRewindType;
        }
        UpdateColor();
    }
}
