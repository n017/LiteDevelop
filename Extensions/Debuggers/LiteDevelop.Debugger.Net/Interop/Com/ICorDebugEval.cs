using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace LiteDevelop.Debugger.Net.Interop.Com
{
    [ComImport, Guid("CC7BCAF6-8A68-11d2-983C-0000F808342D"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ICorDebugEval
    {
	    /*
	     * CallFunction sets up a function call.  Note that the given function
	     * is called directly; there is no virtual dispatch.  
	     * (Use ICorDebugObjectValue::GetVirtualMethod to do virtual dispatch.)
	     */
        
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void CallFunction([In, MarshalAs(UnmanagedType.Interface)] ICorDebugFunction pFunction, 
                            [In] uint nArgs, 
                            [In, MarshalAs(UnmanagedType.LPArray)] ICorDebugValue[] ppArgs);

	    /*
	     * NewObject allocates and calls the constructor for an object.
	     */
        
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void NewObject([In, MarshalAs(UnmanagedType.Interface)] ICorDebugFunction pConstructor,
                         [In] uint nArgs,
                         [In, MarshalAs(UnmanagedType.LPArray)] ICorDebugValue[] ppArgs);

	    /*
         * NewObjectNoConstructor allocates a new object without
	     * attempting to call any constructor on the object.
         */
        
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void NewObjectNoConstructor([In, MarshalAs(UnmanagedType.Interface)] ICorDebugClass pClass);

	    /*
	     * NewString allocates a string object with the given contents.
	     */
        
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void NewString([In, MarshalAs(UnmanagedType.LPWStr)] string value);

	    /*
         * NewArray allocates a new array with the given element type and
	     * dimensions. If the elementType is a primitive, pElementClass
	     * may be NULL. Otherwise, pElementClass should be the class of
	     * the elements of the array. Note: lowBounds is optional. If
         * omitted, a zero lower bound for each dimension is assumed.
         */
        
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void NewArray([In] uint elementType,
                        [In, MarshalAs(UnmanagedType.Interface)] ICorDebugClass pElementClass, 
					    [In] uint rank,
                        [In] ref uint dims, 
                        [In] ref uint lowBounds);

	    /*
	     * IsActive returns whether or not the Eval has an active computation.
	     */
        
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void IsActive(out int pbActive);

	    /*
	     * Abort aborts the current computation.  Note that in the case of nested
	     * Evals, this may fail unless it is the most recent Eval.
	     */
        
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void Abort();

	    /*
         * GetResult returns the result of the evaluation.  This is only
	     * valid after the evaluation is completed.
	     *
	     * If the evaluation completes normally, the result will be the
	     * return value.  If it terminates with an exception, the result
	     * is the exception thrown. If the evaluation was for a new object,
         * the return value is the reference to the object.
         */
        
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void GetResult([MarshalAs(UnmanagedType.Interface)] out ICorDebugValue ppResult);

	    /* 
	     * GetThread returns the thread which this eval was created from.
	     */
        
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void GetThread([MarshalAs(UnmanagedType.Interface)] out ICorDebugThread ppThread);

        /*
         * CreateValue creates an ICorDebugValue of the given type for the
         * sole purpose of using it in a function evaluation. These can be
         * use to pass user constants as parameters. The value has a zero
         * or NULL initial value. Use ICorDebugValue::SetValue to
         * set the value.
         *
         * pElementClass is only required for value classes. Pass NULL
         * otherwise.
         *
         * If elementType == ELEMENT_TYPE_CLASS, then you get an
         * ICorDebugReferenceValue representing the NULL object reference.
         * You can use this to pass NULL to evals that have object reference
         * parameters. You cannot set the ICorDebugReferenceValue to
         * anything... it always remains NULL.
         */
    
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void CreateValue([In] uint elementType, 
                           [In, MarshalAs(UnmanagedType.Interface)] ICorDebugClass pElementClass, 
                           [MarshalAs(UnmanagedType.Interface)] out ICorDebugValue ppValue);
                
    }
}
