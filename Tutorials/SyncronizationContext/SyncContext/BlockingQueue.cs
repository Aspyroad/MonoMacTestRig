using System;
using System.Collections.Generic;
using System.Threading;


internal interface IQueueReader<t> : IDisposable
{
  t Dequeue();
  void ReleaseReader();
}

internal interface IQueueWriter<t> : IDisposable
{
  void Enqueue(t data);
}


internal class BlockingQueue<t> : IQueueReader<t>, IQueueWriter<t>, IDisposable 
{
  // use a .NET queue to store the data
  private Queue<t> mQueue = new Queue<t>();
  // create a semaphore that contains the items in the queue as resources.
  // initialize the semaphore to zero available resources (empty queue).
  private Semaphore mSemaphore = new Semaphore(0, int.MaxValue);
  // a event that gets triggered when the reader thread is exiting
  private ManualResetEvent mKillThread = new ManualResetEvent(false);
  // wait handles that are used to unblock a Dequeue operation.
  // Either when there is an item in the queue
  // or when the reader thread is exiting.
  private WaitHandle[] mWaitHandles;

  public BlockingQueue()
  {
        mWaitHandles = new WaitHandle[2] { mSemaphore, mKillThread };
  }
  public void Enqueue(t data)
  {
      lock (mQueue)
      {
          mQueue.Enqueue(data);
      }
    // add an available resource to the semaphore,
    // because we just put an item
    // into the queue.
    mSemaphore.Release();
  }

  public t Dequeue()
  {
    // wait until there is an item in the queue
    WaitHandle.WaitAny(mWaitHandles);
    lock (mQueue)
    {
        if (mQueue.Count > 0)
        {
            return mQueue.Dequeue();
        }
    }
    return default(t);
  }

  public void ReleaseReader()
  {
    mKillThread.Set();
  }


  void IDisposable.Dispose()
  {
    if (mSemaphore != null)
    {
      mSemaphore.Close();
      mQueue.Clear();
      mSemaphore = null;
    }
  }
}