public interface IEvent { }

public class NotificationEvent : IEvent
{
    public string name;
    public string description;

    public float duration;
}
