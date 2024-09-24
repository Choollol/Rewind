using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class EventMessenger : MonoBehaviour
{

    private Dictionary<string, UnityEvent> eventDictionary;

    private static EventMessenger instance;

    public static EventMessenger Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType(typeof(EventMessenger)) as EventMessenger;

                if (!instance)
                {
                    Debug.LogError("There needs to be one active EventMessenger script on a GameObject in your scene.");
                }
                else
                {
                    instance.Init();
                }
            }

            return instance;
        }
    }

    void Init()
    {
        if (eventDictionary == null)
        {
            eventDictionary = new Dictionary<string, UnityEvent>();
        }
    }

    public static void StartListening(string eventName, UnityAction listener)
    {
        UnityEvent thisEvent = null;
        if (Instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent();
            thisEvent.AddListener(listener);
            Instance.eventDictionary.Add(eventName, thisEvent);
        }
    }

    public static void StopListening(string eventName, UnityAction listener)
    {
        if (instance == null) return;
        UnityEvent thisEvent = null;
        if (Instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    public static void TriggerEvent(string eventName)
    {
        UnityEvent thisEvent = null;
        if (Instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke();
        }
        else
        {
            //Debug.Log("EventMessenger does not contain " + eventName);
        }
    }
}