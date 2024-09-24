using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRewinder : MonoBehaviour
{
    [SerializeField] private List<GameObject> rewindShadows;

    private Vector2 startPos;
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
    }
    private void Rewind()
    {
        Instantiate(rewindShadows[0], transform.position, Quaternion.identity);
        transform.position = startPos;
        EventMessenger.TriggerEvent("PlayerRewinded");
    }
}
