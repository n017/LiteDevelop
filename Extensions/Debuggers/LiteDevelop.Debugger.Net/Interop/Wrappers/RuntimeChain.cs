using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using LiteDevelop.Debugger.Net.Interop.Com;

namespace LiteDevelop.Debugger.Net.Interop.Wrappers
{
    public class RuntimeChain : DebuggerSessionObject, IChain
    {
        private ICorDebugChain _comChain;
        private RuntimeFrame _currentFrame;

        internal RuntimeChain (RuntimeThread thread, ICorDebugChain comChain)
        {
            _comChain = comChain;
            Thread = thread;
        }

        internal ICorDebugChain ComChain
        {
            get { return _comChain; }
        }

        public override NetDebuggerSession Session
        {
            get { return Thread.Session; }
        }

        #region IChain Members

        IThread IChain.Thread
        {
            get { return Thread; }
        }

        IFrame IChain.CurrentFrame
        {
            get { return CurrentFrame; }
        }

        IEnumerable<IFrame> IChain.GetFrames()
        {
            return GetFrames();
        }

        #endregion

        public RuntimeThread Thread
        {
            get;
            private set;
        }

        public RuntimeFrame CurrentFrame
        {
            get
            {
                ICorDebugFrame comFrame;
                _comChain.GetActiveFrame(out comFrame);

                if (comFrame == null)
                    return null;

                if (_currentFrame == null || comFrame != _currentFrame.ComFrame)
                {
                    _currentFrame = Session.ComInstanceCollector.GetWrapper<RuntimeFrame>(comFrame);
                    if (_currentFrame == null)
                    {
                        _currentFrame = new RuntimeFrame(Thread, comFrame);
                        Session.ComInstanceCollector.SetWrapper(comFrame, _currentFrame);
                    }
                }

                return _currentFrame;
            }
        }

        public IEnumerable<RuntimeFrame> GetFrames()
        {
            ICorDebugFrameEnum frameEnum;
            _comChain.EnumerateFrames(out frameEnum);
            uint count;
            frameEnum.GetCount(out count);

            var framePtrs = new IntPtr[count];
       
            frameEnum.Next(count, framePtrs, out count);
            for (int i = 0; i < count;i++)
            {
                var frame = Session.MtaStaConnector.MarshalAs<ICorDebugFrame>(framePtrs[i]);
                var frameWrapper = Session.ComInstanceCollector.GetWrapper<RuntimeFrame>(frame);
                if (frameWrapper == null)
                {
                    frameWrapper = new RuntimeFrame(Thread, frame);
                    Session.ComInstanceCollector.SetWrapper(frame, frameWrapper);
                }
                yield return frameWrapper;
            }

            yield break;
        }
    }
}
