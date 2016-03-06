using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using LiteDevelop.Debugger.Net.Interop.Com;

namespace LiteDevelop.Debugger.Net.Interop.Wrappers
{
    public class SourceStepper : DebuggerSessionObject 
    {
        private readonly RuntimeThread _thread;
        private readonly ICorDebugStepper _comStepper;

        internal SourceStepper(RuntimeThread thread, ICorDebugStepper comStepper)
        {
            _thread = thread;
            _comStepper = comStepper;
            _comStepper.SetRangeIL(1);
            _comStepper.SetUnmappedStopMask(CorDebugUnmappedStop.STOP_NONE);
        }

        internal ICorDebugStepper ComStepper
        {
            get { return _comStepper; }
        }

        public override NetDebuggerSession Session
        {
            get { return _thread.Session; }
        }

        public RuntimeThread Thread
        {
            get { return _thread; }
        }

        public bool IsActive
        {
            get
            {
                int isActive;
                _comStepper.IsActive(out isActive);
                return isActive == 1;
            }
        }

        public void Deactivate()
        {
            _comStepper.Deactivate();
        }

        public void StepIn()
        {
            Step(1);
        }

        public void StepOver()
        {
            Step(0);
        }

        private void Step(int stepIn)
        {
            // get sequence points
            var startOffset = _thread.CurrentFrame.GetOffset();
            var sequencePoint = _thread.CurrentFrame.Function.Symbols.GetSequencePoint(startOffset);
            var sequencePoints = _thread.CurrentFrame.Function.Symbols.GetIgnoredSequencePoints().ToList();
            sequencePoints.Insert(0, sequencePoint);
          
            int size = Marshal.SizeOf(sequencePoint.ByteRange);

            // write ranges to pointer.
            IntPtr rangesPtr = Marshal.AllocHGlobal(size * sequencePoints.Count);
            for (int i = 0; i < sequencePoints.Count; i++)
            {
                Marshal.StructureToPtr(sequencePoints[i].ByteRange, rangesPtr + (i * size), true);
            }

            // step
            _comStepper.StepRange(stepIn, rangesPtr, (uint)sequencePoints.Count);

            // free ranges
            Marshal.FreeHGlobal(rangesPtr);

            _thread.Process.Continue();
        }

        public void StepInInstruction()
        {
            StepInstruction(1);
        }

        public void StepOverInstruction()
        {
            StepInstruction(0);
        }

        private void StepInstruction(int stepIn)
        {
            _comStepper.Step(stepIn);
            _thread.Process.Continue();
        }

        public void StepOut()
        {
            _comStepper.StepOut();
            _thread.Process.Continue();
        }
        
    }

    public delegate void StepperEventHandler(object sender, StepperEventArgs e);

    public class StepperEventArgs : DebuggerEventArgs
    {
        public StepperEventArgs(RuntimeAppDomain domain, RuntimeThread thread, SourceStepper stepper)
            : base(domain, false)
        {
            Domain = domain;
            Thread = thread;
            Stepper = stepper;
        }

        public RuntimeThread Thread
        {
            get;
            private set;
        }

        public RuntimeAppDomain Domain
        {
            get;
            private set;
        }

        public SourceStepper Stepper
        {
            get;
            private set;
        }


    }
}
