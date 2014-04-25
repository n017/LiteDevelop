using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace LiteDevelop.Debugger.Net.Interop.Com
{
    [ComImport, Guid("6DC3FA01-D7CB-11d2-8A95-0080C792E5D8"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ICorDebugEditAndContinueSnapshot
    {
	    /*
	     * CopyMetaData saves a copy of the executing metadata from the debuggee
	     * for this snapshot to the output stream.  The stream implementation must
	     * be supplied by the caller and will typically either save the copy to
	     * memory or to disk.  Only the IStream::Write method will be called by
	     * this method.  The MVID value returned is the unique metadata ID for
	     * this copy of the metadata.  It may be used on subsequent edit and 
	     * continue operations to determine if the client has the most recent
	     * version already (performance win to cache).
	     */
	    void CopyMetaData([In, MarshalAs(UnmanagedType.Interface)] System.Runtime.InteropServices.ComTypes.IStream pIStream, out Guid pMvid);
	
	    /*
	     * GetMvid will return the currently active metadata ID for the executing
	     * process.  This value can be used in conjunction with CopyMetaData to
	     * cache the most recent copy of the metadata and avoid expensive copies.
	     * So for example, if you call CopyMetaData once and save that copy,
	     * then on the next E&C operation you can ask for the current MVID and see
	     * if it is already in your cache.  If it is, use your version instead of
	     * calling CopyMetaData again.
	     */
	    void GetMvid(out Guid pMvid);

	    /*
	     * GetRoDataRVA returns the base RVA that should be used when adding new
	     * static read only data to an existing image.  The EE will guarantee that
	     * any RVA values embedded in the code are valid when the delta PE is
	     * applied with new data.  The new data will be added to a page that is
	     * marked read only.
	     */
	    void GetRoDataRVA(out uint pRoDataRVA);

	    /*
	     * GetRwDataRVA returns the base RVA that should be used when adding new
	     * static read/write data to an existing image.  The EE will guarantee that
	     * any RVA values embedded in the code are valid when the delta PE is
	     * applied with new data.  The ew data will be added to a page that is 
	     * marked for both read and write access.
	     */
	    void GetRwDataRVA(out uint pRwDataRVA);


	    /*
	     * SetPEBytes gives the snapshot object a reference to the delta PE which was
	     * based on the snapshot.  This reference will be AddRef'd and cached until
	     * CanCommitChanges and/or CommitChanges are called, at which point the 
	     * engine will read the delta PE and remote it into the debugee process where
	     * the changes will be checked/applied.
	     */
	    void SetPEBytes([In, MarshalAs(UnmanagedType.Interface)] System.Runtime.InteropServices.ComTypes.IStream pIStream);

	    /*
	     * SetILMap is called once for every method being replace that has
	     * active instances on a call stack on a thread in the target process.
	     * It is up to the caller of this API to determine this case exists.
	     * One should halt the target process before making this check and
	     * calling this method.
	     */
        void SetILMap([In] uint mdFunction, [In] uint cMapSize,
                        [In] ref COR_IL_MAP map);

        /*
         * SetPESymbolBytes gives the snapshot object a reference to the
         * updated symbols for the delta PE. This reference will be AddRef'd
         * and cached until CanCommitChanged and/or CommitChanges are called,
         * at which point the engine will read the delta and remote it into
         * the debuggee process where the changes will be checked/applied.
         */
        void SetPESymbolBytes([In, MarshalAs(UnmanagedType.Interface)] System.Runtime.InteropServices.ComTypes.IStream pIStream);
    }
}
