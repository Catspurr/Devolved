using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class EventHandler : MonoBehaviour
{
    public static EventHandler Current;

    private void Awake()
    {
        Current = this;
    }

    public event Action OnClientJoin;

    public void ClientJoin()
    {
        OnClientJoin?.Invoke();
    }

    public event Action OnClientLeave;

    public void ClientLeave()
    {
        OnClientLeave?.Invoke();
    }
}
