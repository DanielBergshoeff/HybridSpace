using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Signal<T>
{
    private Action<T> action;

    public void AddListener(Action<T> listener)
    {
        action += listener;
    }

    public void RemoveListener(Action<T> listener)
    {
        action -= listener;
    }

    public void Dispatch(T value)
    {
        if (action != null)
        {
            action.Invoke(value);
        }
    }
}
