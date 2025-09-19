using System;
using System.Collections.Generic;
using UnityEngine;

public class UnityMainThreadDispatcher : MonoBehaviour
{
    private static readonly Queue<Action> executionQueue = new Queue<Action>();
    private static UnityMainThreadDispatcher instance;

    public static UnityMainThreadDispatcher Instance()
    {
        if (instance == null)
        {
            // 새로운 GameObject를 만들어 붙임
            var obj = new GameObject("UnityMainThreadDispatcher");
            instance = obj.AddComponent<UnityMainThreadDispatcher>();
            DontDestroyOnLoad(obj);
        }
        return instance;
    }

    void Update()
    {
        lock (executionQueue)
        {
            while (executionQueue.Count > 0)
            {
                executionQueue.Dequeue().Invoke();
            }
        }
    }

    /// <summary>
    /// 메인 스레드에서 실행될 작업을 큐에 추가
    /// </summary>
    public void Enqueue(Action action)
    {
        if (action == null)
            return;

        lock (executionQueue)
        {
            executionQueue.Enqueue(action);
        }
    }
}
