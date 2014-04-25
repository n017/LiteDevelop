using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using LiteDevelop.Debugger.Net.Interop.Wrappers;

namespace LiteDevelop.Debugger.Net.Interop.Com
{
    
	internal enum CorDebugIntercept
	{
        INTERCEPT_NONE = 0x0 ,
		INTERCEPT_CLASS_INIT = 0x01,
		INTERCEPT_EXCEPTION_FILTER	= 0x02,
		INTERCEPT_SECURITY = 0x04,
		INTERCEPT_CONTEXT_POLICY = 0x08,
		INTERCEPT_INTERCEPTION = 0x10,
		INTERCEPT_ALL = 0xffff
	}

	internal enum CorDebugUnmappedStop
	{
        STOP_NONE               = 0x0,
		STOP_PROLOG				= 0x01,
		STOP_EPILOG				= 0x02,
		STOP_NO_MAPPING_INFO	= 0x04,		
		STOP_OTHER_UNMAPPED		= 0x08,
		STOP_UNMANAGED          = 0x10,
		STOP_ALL				= 0xffff
	}

    [ComImport, Guid("CC7BCAEC-8A68-11d2-983C-0000F808342D"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ICorDebugStepper
    {
	    /*
	     * IsActive returns whether or not the stepper is active, that is, whether
	     * it is currently stepping.
	     *
	     * Any step action remains active until StepComplete is called.  Note that
	     * this automatically deactivates the stepper.
	     *
	     * A stepper may also be deactivated prematurely by calling
	     * Deactivate before a callback condition is reached.
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void IsActive(out int pbActive);

	    /*
	     * Deactivate causes a stepper to cancel the last stepping command it
	     * received.  A new stepping command may then be issued.
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void Deactivate();

	    /*
	     * SetInterceptMask controls which intercept code will be stepped
	     * into by the stepper. If the bit for an interceptor is set, the
	     * stepper will complete with reason STEPPER_INTERCEPT when the
	     * given type of intercept occurs.  If the bit is cleared, the 
	     * intercepting code will be skipped.
	     *
         * Note that SetInterceptMask may have unforeseen interactions
         * with SetUnmappedStopMask (from the user's point of view).  For
         * example, if the only visible (ie, non internal) portion of class
         * init code lacks mapping info (STOP_NO_MAPPING_INFO) and
         * STOP_NO_MAPPING_INFO isn't set, then we'll step over the class init.
         *
	     * By default, only INTERCEPT_NONE will be used.
	     */
        
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void SetInterceptMask([In] CorDebugIntercept mask);

	    /*
	     * SetUnmappedStopMask controls whether the stepper
	     * will stop in jitted code which is not mapped to IL.
	     * 
	     * If the given flag is set, then that type of unmapped code
	     * will be stopped in.  Otherwise stepping transparently continues.
         *
         * It should be noted that if one doesn't use a stepper to enter a
         * method (for example, the main() method of C++), then one
         * won't neccessarily step over prologs,etc.
         *
         * By default, STOP_OTHER_UNMAPPED will be used.
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void SetUnmappedStopMask([In] CorDebugUnmappedStop mask);

	    /*
	     * Step is called when a thread is to be single stepped.  The step
	     * will complete at the next managed instruction executed by the
	     * EE in the stepper's frame.
	     * 
	     * If bStepIn is TRUE, any function calls made during the step
	     * will be stepped into.  Otherwise they will be skipped.
	     *
	     * If Step is called on a stepper which is not in managed code,
	     * the step will complete when the next managed code is executed
	     * by the thread. (if bStepIn is FALSE, it will only complete 
	     * when managed code is returned to, not when it is stepped into.)
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void Step([In] int bStepIn);

	    /*
	     * StepRange works just like Step, except it will not complete
	     * until code outside the given range is reached.  This can be
	     * more efficient than stepping one instruction at a time.
	     * 
	     * Ranges are specified as a list of offset pairs [start, end)
	     * (note that end is exclusive) from the start of the stepper's
	     * frame's code.
	     *
	     * Ranges are in relative to the IL code of a method.  Call
	     * SetRangeIL(FALSE) to specify ranges relative to the native code
	     * of a method.
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void StepRange([In] int bStepIn,
                         [In] IntPtr ranges, 
                         [In] uint cRangeCount);

	    /*
	     * A StepOut operation will complete after the current frame is 
	     * returned from normally and the previous frame is reactivated.
	     * 
	     * If this is called when in unmanaged code, the step will complete
	     * when the calling managed code is returned to.
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void StepOut();

	    /*
	     * SetRangeIL is used to set whether the ranges passed StepRange are
	     * relative to the IL code or the native code for the method being 
	     * stepped in.
	     *
	     * By default the range is in IL.
	     */

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void SetRangeIL([In] int bIL);
    }
}
