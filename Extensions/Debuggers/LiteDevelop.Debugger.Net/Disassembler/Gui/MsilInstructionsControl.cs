using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AsmResolver;
using AsmResolver.Net.Metadata;
using AsmResolver.Net.Msil;
using AsmResolver.Net.Signatures;
using LiteDevelop.Debugger.Net.Interop.Wrappers;
using LiteDevelop.Framework.Debugging;
using LiteDevelop.Framework.Extensions;

namespace LiteDevelop.Debugger.Net.Disassembler.Gui
{
    public partial class MsilInstructionsControl : UserControl
    {
        private DebuggerSession _session;
        private IFunction _lastFunction;

        public MsilInstructionsControl()
        {
            InitializeComponent();
        }
        
        public void SetCurrentFrame(IFrame currentFrame)
        {
            if (currentFrame == null || currentFrame.Function != _lastFunction)
            {
                instructionsListView.Items.Clear();
                _lastFunction = null;
            }

            if (currentFrame != null)
            {
                if (currentFrame.Function == _lastFunction)
                {
                    foreach (var item in instructionsListView.Items.Cast<MsilInstructionListViewItem>()) 
                        item.UpdateItem(currentFrame);
                }
                else
                {
                    _lastFunction = currentFrame.Function;
                    var bytes = ((RuntimeFunction)currentFrame.Function).IlCode.GetBytes();
                    var disassembler = new MsilDisassembler(new MemoryStreamReader(bytes), new DefaultOperandResolver());
                    foreach (var instruction in disassembler.Disassemble())
                    {
                        var instructionBytes = new byte[instruction.Size];
                        Buffer.BlockCopy(bytes, instruction.Offset, instructionBytes, 0, instruction.Size);
                        var item = new MsilInstructionListViewItem(instruction, instructionBytes);
                        item.UpdateItem(currentFrame);
                        instructionsListView.Items.Add(item);
                    }
                }
            }
        }
    }

    public class MsilInstructionListViewItem : ListViewItem
    {
        public MsilInstructionListViewItem(MsilInstruction instruction, byte[] bytes)
        {
            Instruction = instruction;
            Bytes = bytes;
        }

        public MsilInstruction Instruction
        {
            get;
            private set;
        }

        public byte[] Bytes
        {
            get;
            set;
        }

        public void UpdateItem(IFrame frame)
        {
            SubItems.Clear();
            Text = "IL_" + Instruction.Offset.ToString("X4");
            SubItems.AddRange(new string[]
            {
                string.Join(" ", Bytes.Select(x => x.ToString("X2"))),
                Instruction.OpCode.Name,
                Instruction.OperandToString()
            });

            BackColor = GetBackgroundColor(frame);
        }

        private Color GetBackgroundColor(IFrame frame)
        {
            if (frame.GetOffset() == Instruction.Offset)
            {
                this.EnsureVisible();
                return Color.Yellow;
            }

            var symbols = frame.Function.Symbols;
            if (symbols != null)
            {
                var sequencePoint = symbols.GetSequencePoint(frame.GetOffset());
                if (Instruction.Offset >= sequencePoint.ByteRange.StartOffset
                    && Instruction.Offset < sequencePoint.ByteRange.EndOffset)
                    return Color.FromArgb(255, 255, 160);
            }

            return Color.Transparent;
        }
    }

    public class DefaultOperandResolver : IOperandResolver
    {
        public MetadataMember ResolveMember(MetadataToken token)
        {
            return null;
        }

        public string ResolveString(uint token)
        {
            return null;
        }

        public VariableSignature ResolveVariable(int index)
        {
            return null;
        }

        public ParameterSignature ResolveParameter(int index)
        {
            return null;
        }
    }
}
