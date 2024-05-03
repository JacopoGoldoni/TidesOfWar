using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationManager : MonoBehaviour
{
    private Text notificationText;

    CountdownTimer notificationTimer;

    private void Awake()
    {
        notificationText = GetComponentInChildren<Text>();    
    }

    public void Initialize(string text, float duration)
    {
        notificationText.text = text;
        notificationTimer = new CountdownTimer(duration);
        notificationTimer.Start();
    }

    private void Update()
    {
        notificationTimer.Tick(Time.deltaTime);

        if(notificationTimer.IsFinished)
        {
            NotifyTabManager ntb = transform.parent.GetComponent<NotifyTabManager>();
            transform.parent = null;
            ntb.Check();
            Destroy(this.gameObject);
        }
    }
}
