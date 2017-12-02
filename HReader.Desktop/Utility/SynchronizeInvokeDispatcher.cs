using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace HReader.Utility
{
    internal class SynchronizeInvokeDispatcher : ISynchronizeInvoke
    {
        private class TaskWrapper : IAsyncResult
        {
            private readonly DispatcherOperation operation;

            public TaskWrapper(DispatcherOperation operation)
            {
                this.operation = operation;
            }
            
            public bool IsCompleted => operation.Task.IsCompleted;
            public WaitHandle AsyncWaitHandle => ((IAsyncResult)operation.Task).AsyncWaitHandle;
            public object AsyncState => operation.Task.AsyncState;
            public bool CompletedSynchronously => ((IAsyncResult)operation.Task).CompletedSynchronously;
            public object Result => operation.Result;
            public void Wait() => operation.Wait();
        }

        private readonly Dispatcher dispatcher;

        public SynchronizeInvokeDispatcher(Dispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }

        /// <inheritdoc />
        public IAsyncResult BeginInvoke(Delegate method, object[] args)
        {
            return new TaskWrapper(dispatcher.BeginInvoke(method, args));
        }

        /// <inheritdoc />
        public object EndInvoke(IAsyncResult result)
        {
            var wrapper = (TaskWrapper) result;
            wrapper.Wait();
            return wrapper.Result;
        }

        /// <inheritdoc />
        public object Invoke(Delegate method, object[] args)
        {
            return dispatcher.Invoke(method, args);
        }

        /// <inheritdoc />
        public bool InvokeRequired => !dispatcher.CheckAccess();
    }
}
