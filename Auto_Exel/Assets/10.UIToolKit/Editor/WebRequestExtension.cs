using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Networking;

public struct UnityWebRequestAwaiter : INotifyCompletion
{
    private UnityWebRequestAsyncOperation _asyncOP;
    private Action _Continuation;

    public UnityWebRequestAwaiter(UnityWebRequestAsyncOperation asyncOP)
    {
        _asyncOP = asyncOP;
        _Continuation = null;
    }

    public bool IsCompleted { get => _asyncOP.isDone; }

    public void GetResult() { }

    public void OnCompleted(Action continuation)
    {
        _Continuation = continuation;
        _asyncOP.completed += OnRequestCompleted;
    }

    private void OnRequestCompleted(AsyncOperation operation)
    {
        _Continuation?.Invoke();
    }
}

public static class WebRequestExtensions
{
    public static UnityWebRequestAwaiter GetAwaiter(this UnityWebRequestAsyncOperation asyncOP)
    {
        return new UnityWebRequestAwaiter(asyncOP);
    }
}