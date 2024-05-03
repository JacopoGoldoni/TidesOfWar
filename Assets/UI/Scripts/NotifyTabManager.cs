using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotifyTabManager : MonoBehaviour
{
    public GameObject notificationPrefab;

    private VerticalLayoutGroup vlg;
    private Image notificationTabBackground;

    //NOTIFICATION
    EventBinding<NotificationEvent> notificationBinding;

    private void OnEnable()
    {
        //BIND NOTIFICATION BUS
        notificationBinding = new EventBinding<NotificationEvent>(HandleNotificationEvent);
        EventBus<NotificationEvent>.Register(notificationBinding);
    }

    private void OnDisable()
    {
        //UNBIND NOTIFICATION BUS
        EventBus<NotificationEvent>.Deregister(notificationBinding);
    }

    private void Awake()
    {
        vlg = GetComponent<VerticalLayoutGroup>();
        notificationTabBackground = GetComponent<Image>();
    }

    private void Start()
    {
        Check();
    }

    void HandleNotificationEvent(NotificationEvent notificationEvent)
    {
        AddNotify(notificationEvent.name, notificationEvent.description, notificationEvent.duration);
    }

    public void AddNotify(string name, string description, float duration)
    {
        GameObject newNotify = Instantiate(notificationPrefab);

        //SET NOTIFICATION PARAMS
        newNotify.GetComponent<NotificationManager>().Initialize(name, duration);

        newNotify.transform.SetParent(transform, false);

        Check();
    }
    
    public void Check()
    {
        if (transform.childCount != 0)
        {
            notificationTabBackground.enabled = true;
        }
        else
        {
            notificationTabBackground.enabled = false;
        }
    }
}
