/*
    Implementation of ISynchronizeInvoke for Unity3D game engine.
    Can be used to invoke anything on main Unity thread.
    ISynchronizeInvoke is used extensively in .NET forms, it's is elegant and quite useful in Unity as well.
    I implemented it so i can use it with System.IO.FileSystemWatcher.SynchronizingObject.
    help from: http://www.codeproject.com/Articles/12082/A-DelegateQueue-Class
    example usage: https://gist.github.com/aeroson/90bf21be3fdc4829e631
    version: aeroson 2017-07-13 (author yyyy-MM-dd)
    license: WTFPL (http://www.wtfpl.net/)
*/

using System.Threading;
using System.ComponentModel;
using System.Collections.Generic;
using System;

public class DeferredSynchronizeInvoke : ISynchronizeInvoke
{
    private Queue<AsyncResult> toExecute = new Queue<AsyncResult>();
    private Thread mainThread;

    public bool InvokeRequired { get { return mainThread.ManagedThreadId != Thread.CurrentThread.ManagedThreadId; } }

    public DeferredSynchronizeInvoke()
    {
        mainThread = Thread.CurrentThread;
    }

    public IAsyncResult BeginInvoke(Delegate method, object[] args)
    {
        var asyncResult = new AsyncResult()
        {
            method = method,
            args = args,
            IsCompleted = false,
            manualResetEvent = new ManualResetEvent(false),
        };

        if (InvokeRequired)
        {
            lock (toExecute)
                toExecute.Enqueue(asyncResult);
        }
        else
        {
            asyncResult.Invoke();
            asyncResult.CompletedSynchronously = true;
        }

        return asyncResult;
    }

    public object EndInvoke(IAsyncResult result)
    {
        if (!result.IsCompleted)
            result.AsyncWaitHandle.WaitOne();

        return result.AsyncState;
    }

    public object Invoke(Delegate method, object[] args)
    {
        if (InvokeRequired)
        {
            var asyncResult = BeginInvoke(method, args);
            return EndInvoke(asyncResult);
        }
        else
        {
            return method.DynamicInvoke(args);
        }
    }

    public void ProcessQueue()
    {
        if (Thread.CurrentThread != mainThread)
            throw new Exception(
                "must be called from the same thread it was created on " +
                "(created on thread id: " + mainThread.ManagedThreadId + ", called from thread id: " + Thread.CurrentThread.ManagedThreadId
            );

        AsyncResult data = null;
        while (true)
        {
            lock (toExecute)
            {
                if (toExecute.Count == 0)
                    break;
                data = toExecute.Dequeue();
            }

            data.Invoke();
        }
    }

    private class AsyncResult : IAsyncResult
    {
        public Delegate method;
        public object[] args;
        public bool IsCompleted { get; set; }
        public WaitHandle AsyncWaitHandle { get { return manualResetEvent; } }
        public ManualResetEvent manualResetEvent;
        public object AsyncState { get; set; }
        public bool CompletedSynchronously { get; set; }

        public void Invoke()
        {
            AsyncState = method.DynamicInvoke(args);
            IsCompleted = true;
            manualResetEvent.Set();
        }
    }
}
