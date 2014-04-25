using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using LiteDevelop.Debugger.Net.Interop.Wrappers;

namespace LiteDevelop.Debugger.Net
{
    public enum CallMethod
    {
        Direct,
        Indirect,
    }

    public class MtaStaConnector : DebuggerSessionObject
    {
        private Thread _creationThread;
        private Queue<Action> _queuedActions = new Queue<Action>();
        private ManualResetEvent _hasQueuedActions = new ManualResetEvent(false);
        private Form _dummyForm;
        private IntPtr _dummyFormHandle;
        private NetDebuggerSession _session;

        public MtaStaConnector(NetDebuggerSession session)
        {
            _creationThread = Thread.CurrentThread;
            _dummyForm = new Form();
            _dummyFormHandle = _dummyForm.Handle;
            _session = session;
        }

        public override NetDebuggerSession Session
        {
            get { return _session; }
        }

        public CallMethod CallMethod
        {
            get;
            set;
        }

        public void PerformAllCalls()
        {
            while (!PerformNextCall()) ;
        }

        private void PerformAllCallsInternal()
        {
            switch (CallMethod)
            {
                case Net.CallMethod.Direct:
                    PerformAllCalls();
                    break;
                case Net.CallMethod.Indirect:
                    _dummyForm.BeginInvoke(new Action(PerformAllCalls));
                    break;
            }
        }

        public bool PerformNextCall()
        {
            if (_queuedActions.Count > 0)
            {
                _queuedActions.Dequeue()();
                return true;
            }

            _hasQueuedActions.Reset();
            return false;
        }

        public void Call(Action action)
        {
            var waitHandle = EnqueueAction(action);
            if (_creationThread == Thread.CurrentThread)
                PerformAllCalls();
            else
            {
                PerformAllCallsInternal();
                if (CallMethod == Net.CallMethod.Indirect)
                    waitHandle.WaitOne();
            }
        }

        private WaitHandle EnqueueAction(Action action)
        {
            var actionFinished = new ManualResetEvent(false);
            _queuedActions.Enqueue(new Action(() =>
                {
                    action();
                    actionFinished.Set();
                }));
            _hasQueuedActions.Set();
            return actionFinished;
        }

        public T MarshalAs<T>(object value)
        {
            return (T)MarshalAs(value, typeof(T));
        }

        public object MarshalAs(object value, Type target)
        {
            if (value is IntPtr)
                return MarshalAs((IntPtr)value, target);
            return value;
        }

        public T MarshalAs<T>(IntPtr value)
        {
            return (T)MarshalAs(value, typeof(T));
        }

        public object MarshalAs(IntPtr value, Type target)
        {
            if (target == typeof(IntPtr))
                return value;

            if (value == IntPtr.Zero)
                return null;

            if (target == typeof(string))
                return Marshal.PtrToStringAuto(value);

            object objectForIUnknown = Marshal.GetObjectForIUnknown(value);
            Session.ComInstanceCollector.AddComObject(objectForIUnknown);
            return objectForIUnknown;
        }
    }
}
