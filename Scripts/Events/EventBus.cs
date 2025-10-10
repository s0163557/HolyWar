using System;
using System.Collections.Generic;
using UnityEngine;

public static class EventBus
{
    //Если нужно какое-то событие для классов, добавляем его сюда и по нему обращаемся.
    //Тут таблица для простых эвентов, которые можно без разбора записывать и выписывать.
    public enum EventsEnum
    {
        BattleStart = 0,
        BattleEnd = 1,
        PlayerGetsPoint = 2,
        GameEnd = 3,
    }

    private static Dictionary<EventsEnum, Action> _eventTable = new Dictionary<EventsEnum, Action>();

    public static void Subscribe(EventsEnum eventToSubscribe, Action listener)
    {
        if (_eventTable.ContainsKey(eventToSubscribe))
            _eventTable[eventToSubscribe] += listener;
        else
            _eventTable[eventToSubscribe] = listener;
    }

    public static void Unsubscribe(EventsEnum eventToUnsubscribe, Action listener)
    {
        if (_eventTable.ContainsKey(eventToUnsubscribe))
            _eventTable.Remove(eventToUnsubscribe);
    }

    public static void RaiseEvent(EventsEnum eventToRaise)
    {
        if (_eventTable.ContainsKey(eventToRaise))
            _eventTable[eventToRaise].Invoke();
    }

    //Стоит отедльно делать более тонкие ивенты с более точной настройкой и доступом.

    //Нужен для подсветки полей
    public static ActionWithEnd<GameObject> UnitSelection = new ActionWithEnd<GameObject>();
}

