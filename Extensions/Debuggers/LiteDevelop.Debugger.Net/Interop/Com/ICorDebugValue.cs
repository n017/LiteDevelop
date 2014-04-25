using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using LiteDevelop.Debugger.Net.Interop.Wrappers;

namespace LiteDevelop.Debugger.Net.Interop.Com
{
    [ComImport, Guid("CC7BCAF7-8A68-11d2-983C-0000F808342D"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ICorDebugValue 
    {
	    /*
	     * GetType returns the simple type of the value.  If the object 
	     * has a more complex runtime type, that type may be examined through the
	     * appropriate subclasses (e.g. ICorDebugObjectValue can get the class of
	     * an object.)
	     */
        
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void GetType(out ElementType pType);

	    /*
	     * GetSize returns the size of the value in bytes. Note that for reference
	     * types this will be the size of the pointer rather than the size of
	     * the object.
	     */
        
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetSize(out uint pSize);

	    /*
         * GetAddress returns the address of the value in the debugee
	     * process.  This might be useful information for the debugger to
	     * show.
	     * 
	     * If the value is at least partly in registers, 0 is returned.
         */
        
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void GetAddress(out ulong pAddress); 

	    /*
	     * CreateBreakpoint creates a breakpoint which will be triggered when 
	     * the value is modified. 
         *
         * Note: not yet implemented.
         *
         * Not Implemented In-Proc
	     */
        
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void CreateBreakpoint([MarshalAs(UnmanagedType.Interface)] out ICorDebugValueBreakpoint ppBreakpoint);
    }

    [ComImport, Guid("5E0B54E7-D88A-4626-9420-A691E0A78B49"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ICorDebugValue2
    {       
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetExactType([MarshalAs(UnmanagedType.Interface)] out ICorDebugType ppType);
    }

    [ComImport, Guid("CC7BCAF8-8A68-11d2-983C-0000F808342D"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ICorDebugGenericValue : ICorDebugValue
    {	 
        
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new void GetType(out ElementType pType);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new void GetSize(out uint pSize);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new void GetAddress(out ulong pAddress);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new void CreateBreakpoint([MarshalAs(UnmanagedType.Interface)] out ICorDebugValueBreakpoint ppBreakpoint);
        
	    /*
	     * GetValue copies the value into the specified buffer.  The buffer should
	     * be the appropriate size for the simple type.
	     */

	    void GetValue([Out] IntPtr pTo);

	    /*
	     * SetValue copies a new value from the specified buffer. The buffer should
	     * be the approprirate size for the simple type.
         *
         * Not Implemented In-Proc
	     */

	    void SetValue([In] IntPtr pFrom); 
    };
    
    /*
     * ICorDebugReferenceValue is a subclass of ICorDebugValue which applies to 
     * a reference type. 
     */

        [ComImport, Guid("CC7BCAF9-8A68-11d2-983C-0000F808342D"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ICorDebugReferenceValue : ICorDebugValue
    {

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new void GetType(out ElementType pType);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new void GetSize(out uint pSize);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new void GetAddress(out ulong pAddress);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new void CreateBreakpoint([MarshalAs(UnmanagedType.Interface)] out ICorDebugValueBreakpoint ppBreakpoint);

	    /*
	     * IsNull tests whether the reference is null.
	     */

	    void IsNull(out int pbNull);

	    /*
	     * GetValue copies the value into the specified buffer.  The buffer should
	     * be the appropriate size for the simple type.
	     */

	    void GetValue(out ulong pValue);

	    /*
	     * SetValue copies a new value from the specified buffer. The buffer should
	     * be the appropriate size for the simple type.
         *
         * Not Implemented In-Proc
	     */

	    void SetValue([In] ulong value); 

	    /*
         * Dereference returns a ICorDebugValue representing the value
	     * referenced. If the resulting value is a garbage collected
	     * object, then the resulting value is a "weak reference" which
	     * will become invalid if the object is garbage collected.
         */
	
	    void Dereference([MarshalAs(UnmanagedType.Interface)] out ICorDebugValue ppValue);

	    /*
	     * DereferenceStrong returns a ICorDebugValue representing the value
         * referenced. If the resulting value is a garbage collected object,
         * then the resulting value is a "strong reference" which will cause
         * the object referenced to not be collect as long as it exists.
	     */
	
	    void DereferenceStrong([MarshalAs(UnmanagedType.Interface)] out ICorDebugValue ppValue);
    };

    /*
     * ICorDebugHeapValue is a subclass of ICorDebugValue which represents
     * a garbage collected object 
     */

    [ComImport, Guid("CC7BCAFA-8A68-11d2-983C-0000F808342D"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ICorDebugHeapValue : ICorDebugValue
    {

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new void GetType(out ElementType pType);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new void GetSize(out uint pSize);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new void GetAddress(out ulong pAddress);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new void CreateBreakpoint([MarshalAs(UnmanagedType.Interface)] out ICorDebugValueBreakpoint ppBreakpoint);

	    /*
	     * IsValid tests whether the object is valid.  (The object
	     * becomes invalid if the garbage collector reclaims the object.)
         *
         * Not Implemented In-Proc
	     */

	    void IsValid(out int pbValid);

	    /*
	     * CreateRelocBreakpoint creates a breakpoint which will be triggered when 
	     * the address in the reference changes due to a garbage collection.
         *
         * Note: not yet implemented.
         *
         * Not Implemented In-Proc
	     */

	    void CreateRelocBreakpoint(out 
								      ICorDebugValueBreakpoint ppBreakpoint);
    };

    /*
     * ICorDebugObjectValue is a subclass of ICorDebugValue which applies to 
     * values which contain an object.  
     */
    
    [ComImport, Guid("18AD3D6E-B7D2-11d2-BD04-0000F80849BD"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ICorDebugObjectValue : ICorDebugValue
    {

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new void GetType(out ElementType pType);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new void GetSize(out uint pSize);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new void GetAddress(out ulong pAddress);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new void CreateBreakpoint([MarshalAs(UnmanagedType.Interface)] out ICorDebugValueBreakpoint ppBreakpoint);

	    /*
	     * GetClass returns the runtime class of the object in the value. 
	     */
        
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void GetClass([MarshalAs(UnmanagedType.Interface)] out ICorDebugClass ppClass);

	    /*
         * GetFieldValue returns a value for the given field in the given
	     * class. The class must be on the class hierarchy of the object's
	     * class, and the field must be a field of that class.
         */
        
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void GetFieldValue([In, MarshalAs(UnmanagedType.Interface)] ICorDebugClass pClass,
                              [In] uint fieldDef,
						      [MarshalAs(UnmanagedType.Interface)] out ICorDebugValue ppValue);

	    /*
	     * GetVirtualMethod returns the most derived function
	     * for the given ref on this object.
         *
         * Note: not yet implemented.
	     */
        
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void GetVirtualMethod([In] uint memberRef,
							     [MarshalAs(UnmanagedType.Interface)] out ICorDebugFunction ppFunction);

	    /*
	     * GetContext returns the Common Language Runtime context for the object.
         *
         * Note: not yet implemented.
	     */
        
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
	    void GetContext([MarshalAs(UnmanagedType.Interface)] out ICorDebugContext ppContext);

        /*
         * IsValueClass returns true if the the class of this object is
         * a value class.
         */
        
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void IsValueClass(out int pbIsValueClass);

        /*
         * GetManagedCopy will return an IUnknown that is a managed copy
         * of a value class object. This can be used with Common Language Runtime Interop to
         * get info about the object, like calling System.Object::ToString
         * on it.
         *
         * Returns CORDB_E_OBJECT_IS_NOT_COPYABLE_VALUE_CLASS if the class of this
         * object is not a value class.
         *
         * Not Implemented In-Proc
         */
        
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetManagedCopy([MarshalAs(UnmanagedType.IUnknown)] out object ppObject);

        /*
         * SetFromManagedCopy will update a object's contents given a
         * managed copy of the object. This can be used after using
         * GetManagedCopy to update an object with a changed version.
         *
         * Returns CORDB_E_OBJECT_IS_NOT_COPYABLE_VALUE_CLASS if the class of this
         * object is not a value class.
         *
         * Not Implemented In-Proc
         *
         */
        
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetFromManagedCopy([In, MarshalAs(UnmanagedType.IUnknown)] object pObject);
    }


    /*
     * ICorDebugBoxValue is a subclass of ICorDebugValue which
     * represents a boxed value class object.  
     */

    [ComImport, Guid("CC7BCAFC-8A68-11d2-983C-0000F808342D"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ICorDebugBoxValue : ICorDebugHeapValue
    {

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new void GetType(out ElementType pType);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new void GetSize(out uint pSize);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new void GetAddress(out ulong pAddress);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new void CreateBreakpoint([MarshalAs(UnmanagedType.Interface)] out ICorDebugValueBreakpoint ppBreakpoint);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new void IsValid(out int pbValid);

                [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new void CreateRelocBreakpoint(out 
								      ICorDebugValueBreakpoint ppBreakpoint);
	    /*
	     * GetObject returns the value object which is in the box.
	     */

            void GetObject([MarshalAs(UnmanagedType.Interface)] out ICorDebugObjectValue ppObject);
    };

    /* 
     * ICorDebugStringValue is a subclass of ICorDebugValue which
     * applies to values which contain a string.  This interface
     * provides an easy way to get the string contents.  
     */

    [ComImport, Guid("CC7BCAFD-8A68-11d2-983C-0000F808342D"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ICorDebugStringValue : ICorDebugHeapValue
    {
        
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new void GetType(out ElementType pType);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new void GetSize(out uint pSize);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new void GetAddress(out ulong pAddress);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new void CreateBreakpoint([MarshalAs(UnmanagedType.Interface)] out ICorDebugValueBreakpoint ppBreakpoint);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new void IsValid(out int pbValid);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new void CreateRelocBreakpoint(out 
								      ICorDebugValueBreakpoint ppBreakpoint);
	    /* 
	     * GetLength returns the number of characters in the string.
	     */

	    void GetLength(out uint pcchString);

	    /* 
	     * GetString returns the contents of the string.
	     */

	    void GetString([In] uint cchString, 
					      out uint pcchString,
					     [MarshalAs(UnmanagedType.LPArray, SizeParamIndex=0)] char[] szString); 
    };

    /*
     * ICorDebugArrayValue is a subclass of ICorDebugValue which applies
     * to values which contain an array. This interface supports both
     * single and multidimension arrays.
     */
    [ComImport, Guid("0405B0DF-A660-11d2-BD02-0000F80849BD"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ICorDebugArrayValue : ICorDebugHeapValue
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new void GetType(out ElementType pType);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new void GetSize(out uint pSize);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new void GetAddress(out ulong pAddress);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new void CreateBreakpoint([MarshalAs(UnmanagedType.Interface)] out ICorDebugValueBreakpoint ppBreakpoint);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new void IsValid(out int pbValid);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        new void CreateRelocBreakpoint(out 
								      ICorDebugValueBreakpoint ppBreakpoint);
	    /*
	     * GetElementType returns the simple type of the elements in the 
	     * array. 
	     */

	    void GetElementType(out ElementType pType);

	    /*
	     * GetRank returns the number of dimensions in the array.  
	     */

	    void GetRank(out uint pnRank);

	    /*
	     * GetCount returns the number of elements in all dimensions of
         * the array.  
	     */

	    void GetCount(out uint pnCount);

	    /*
	     * GetDimensions returns the dimensions of the array.
	     */

	    void GetDimensions([In] uint cdim,
						   [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex=0)] uint[] dims);

        /*
         * HasBaseIndicies returns whether or not the array has base indicies.
         * If the answer is no, then all dimensions have a base index of 0.
         */

        void HasBaseIndicies(out int pbHasBaseIndicies);
    
	    /*
	     * GetBaseIndicies returns the base index of each dimension in 
	     * the array
	     */

	    void GetBaseIndicies([In] uint cdim,
						     [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex=0)] uint[] indicies);

	    /*
	     * GetElement returns a value representing the given element in the array.
	     */

	    void GetElement([In] uint cdim,
					       [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex=0)] uint[] indices,
					       [MarshalAs(UnmanagedType.Interface)] out ICorDebugValue ppValue);
	    /*
         * GetElementAtPosition returns a value representing the given
	     * element at the given position in the array. The position is
	     * over all dimensions of the array.
         */

	    void GetElementAtPosition([In] uint nPosition, 
                                     [MarshalAs(UnmanagedType.Interface)] out ICorDebugValue ppValue);
    };

}
