using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncMethodPattern
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }


    class AsyncMethods
    {
        private delegate void MyTaskWorkerDelegate(string[] files);
        public event AsyncCompletedEventHandler MyTaskCompleted;
        private readonly object _sync = new object();
        private bool _myTaskIsRunning = false;
        
        public bool IsBusy
        {
            get { return _myTaskIsRunning; }
        }

        // Async version of my actual work function
        public void MyTaskAsync(string[] files)
        {
            MyTaskWorkerDelegate worker = new MyTaskWorkerDelegate(MyTaskWorker);
            AsyncCallback completedCallback = new AsyncCallback(MyTaskCompletedCallback);

            lock (_sync)
            {
                if (_myTaskIsRunning)
                {
                    throw new InvalidOperationException("The control is currently busy.");
                }

                AsyncOperation async = AsyncOperationManager.CreateOperation(null);
                worker.BeginInvoke(files, completedCallback, async);
                _myTaskIsRunning = true;
            }
        }

        // Does the actual work
        private void MyTaskWorker(string[] files)
        {
            foreach (string file in files)
            {
                // a time consuming operation with a file (compression, encryption etc.)
                Thread.Sleep(1000);
            }
        }
        
        private void MyTaskCompletedCallback(IAsyncResult ar)
        {
            // get the original worker delegate and the AsyncOperation instance
            MyTaskWorkerDelegate worker = (MyTaskWorkerDelegate)((AsyncResult)ar).AsyncDelegate;
            AsyncOperation async = (AsyncOperation)ar.AsyncState;

            // finish the asynchronous operation
            worker.EndInvoke(ar);

            // clear the running task flag
            lock (_sync)
            {
                _myTaskIsRunning = false;
            }

            // raise the completed event
            AsyncCompletedEventArgs completedArgs = new AsyncCompletedEventArgs(null,
              false, null);
            async.PostOperationCompleted(
              delegate(object e) { OnMyTaskCompleted((AsyncCompletedEventArgs)e); },
              completedArgs);
        }

        protected virtual void OnMyTaskCompleted(AsyncCompletedEventArgs e)
        {
            if (MyTaskCompleted != null)
            {
                MyTaskCompleted(this, e);
            }
        }
    }

}
