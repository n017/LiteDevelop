using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Text;
using LiteDevelop.Debugger.Net.Interop.Com.Symbols;
using LiteDevelop.Framework.FileSystem;

namespace LiteDevelop.Debugger.Net.Interop.Wrappers.Symbols
{

    public class MethodSymbols : IFunctionSymbols 
    {
        internal const int SpecialSequencePoint = 0xfeefee;
        private SequencePoint[] _sequencePoints;
        private LocalVariable[] _localVariables;
        
        internal MethodSymbols(RuntimeFunction function, ISymUnmanagedMethod symMethod)
        {
            Function = function;
            SymMethod = symMethod;
        }

        IFunction IFunctionSymbols.Function
        {
            get { return Function; }
        }

        IEnumerable<IVariable> IFunctionSymbols.GetVariables()
        {
            return LocalVariables;
        }

        IEnumerable<SequencePoint> IFunctionSymbols.GetSequencePoints()
        {
            return SequencePoints;
        }

        public RuntimeFunction Function
        {
            get;
            private set;
        }

        internal ISymUnmanagedMethod SymMethod
        {
            get;
            private set;
        }

        public LocalVariable[] LocalVariables
        {
            get
            {
                if (_localVariables == null)
                {
                    var localVariables = new List<LocalVariable>();
                    var scopes = new Stack<ISymUnmanagedScope>();
                    scopes.Push(SymMethod.GetRootScope());

                    while (scopes.Count > 0)
                    {
                        var currentScope = scopes.Pop();

                        int localsCount = currentScope.GetLocalCount();
                        var locals = new ISymUnmanagedVariable[localsCount];
                        currentScope.GetLocals(localsCount, out localsCount, locals);

                        localVariables.AddRange(locals.ToLocalVariables());

                        int childrenCount = 0;
                        currentScope.GetChildren(childrenCount, out childrenCount, new ISymUnmanagedScope[0]);

                        var children = new ISymUnmanagedScope[childrenCount];
                        currentScope.GetChildren(childrenCount, out childrenCount, children);

                        foreach (var child in children)
                            scopes.Push(child);
                    }

                    _localVariables = localVariables.ToArray();
                }

                return _localVariables;
            }
        }

        public SequencePoint[] SequencePoints
        {
            get
            {
                if (_sequencePoints == null)
                {
                    int pointsCount = SymMethod.GetSequencePointCount();
                    ISymUnmanagedDocument[] documents = new ISymUnmanagedDocument[pointsCount];
                    int[] startLines = new int[pointsCount];
                    int[] ilOffsets = new int[pointsCount];
                    int[] endLines = new int[pointsCount];
                    int[] startCols = new int[pointsCount];
                    int[] endCols = new int[pointsCount];
                    int outPoints;
                    SymMethod.GetSequencePoints(pointsCount, out outPoints, ilOffsets, documents, startLines, startCols, endLines, endCols);

                    StringBuilder sourceFileBuilder = new StringBuilder(512);
                    int sourceFilePathLength;
                    documents[0].GetURL(sourceFileBuilder.Capacity, out sourceFilePathLength, sourceFileBuilder);
                    var filePath = new FilePath(sourceFileBuilder.ToString());

                    var sequencePoints = new SequencePoint[pointsCount];
                    for (int i = 0; i < pointsCount; i++)
                    {
                        sequencePoints[i] = new SequencePoint(
                            filePath,
                            startLines[i],
                            startCols[i],
                            endLines[i],
                            endCols[i],
                            new ByteRange(
                                (uint)ilOffsets[i],
                                (uint)((i + 1) < pointsCount ? ilOffsets[i + 1] : (int)Function.IlCode.Size)));
                    }
                    _sequencePoints = sequencePoints;
                }
                return _sequencePoints;
            }
        }

        public SequencePoint GetSequencePoint(uint offset)
        {
            var points = SequencePoints.Where(p => p.Line != SpecialSequencePoint);

            var sequencePoint = points.FirstOrDefault(p => p.ByteRange.StartOffset <= offset && offset < p.ByteRange.EndOffset) ??
            points.FirstOrDefault(p => offset <= p.Offset);
            return sequencePoint;
        }

        public IEnumerable<SequencePoint> GetIgnoredSequencePoints()
        {
            return SequencePoints.Where(x => x.Line == SpecialSequencePoint);
        }

        public SequencePoint GetSequencePointByLine(int line)
        {
            return SequencePoints.Where(p => p.Line != SpecialSequencePoint).FirstOrDefault(x => x.Line >= line);
        }

    }
}
