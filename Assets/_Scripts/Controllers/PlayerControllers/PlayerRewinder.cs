using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor.Experimental.GraphView;

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

    private List<GameObject> rewindShadowList = new List<GameObject>();
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
    private void Update()
    {
        if (Input.GetButtonDown("Reset") || transform.position.y < -6)
        {
            ResetPosition();
        }
        if (Input.GetButtonDown("Undo") && rewindShadowList.Count > 0)
        {
            Undo();
        }
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
        EventMessenger.TriggerEvent("PlayerRewinded");
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
