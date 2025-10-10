
using System;
using HolyWar.Units;

public class ActionWithEnd<T>
{
    private System.Action<T> OnEventStart;
    private System.Action<T> OnEventEnd;

    public void SubscribeToEvent(System.Action<T> eventStartListener, System.Action<T> eventEndListener)
    {
        OnEventStart += eventStartListener;
        OnEventEnd += eventEndListener;
    }

    public void UnsubscribeToEvent(System.Action<T> eventStartListener, System.Action<T> eventEndListener)
    {
        OnEventStart -= eventStartListener;
        OnEventEnd -= eventEndListener;
    }

    public void RaiseStartEvent(T args)
    {
        OnEventStart.Invoke(args);
    }

    public void RaiseEndEvent(T args)
    {
        OnEventEnd.Invoke(args);
    }
}

