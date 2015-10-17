using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AsmResolver;
using AsmResolver.X86;
using LiteDevelop.Debugger.Net.Interop.Com;
using LiteDevelop.Debugger.Net.Interop.Wrappers;

namespace LiteDevelop.Debugger.Net.Disassembler.Gui
{
    public partial class X86InstructionsControl : UserControl
    {
        private IFunction _lastFunction;

        public X86InstructionsControl()
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
                    foreach (var item in instructionsListView.Items.Cast<X86InstructionListViewItem>())
                        item.UpdateItem(currentFrame);
                }
                else
                {
                    _lastFunction = currentFrame.Function;
                    
                    var code = ((RuntimeFunction) currentFrame.Function).NativeCode;
                    var mapping = code.GetILToNativeMapping().ToArray();
                    var bytes = code.GetBytes();
                    var reader = new MemoryStreamReader(bytes);
                    var disassembler = new X86Disassembler(reader, (long) code.Address);
                    while (reader.Position < reader.StartPosition + reader.Length)
                    {
                        try
                        {
                            int start = (int) reader.Position;
                            var instruction = disassembler.ReadNextInstruction();
                            int end = (int) reader.Position;

                            var instructionBytes = new byte[end - start];
                            Buffer.BlockCopy(bytes, start, instructionBytes, 0, end - start);
                            var item = new X86InstructionListViewItem(instruction, instructionBytes);
                            item.RelativeOffset = start;

                            item.Mapping = mapping.FirstOrDefault(
                                x => item.RelativeOffset >= x.NativeStartOffset
                                     && item.RelativeOffset < x.NativeEndOffset);
                            item.UpdateItem(currentFrame);
                            instructionsListView.Items.Add(item);
                        }
                        catch (IndexOutOfRangeException)
                        {
                            // HACK: ignore mnemonic choosing errors in disassembler
                        }
                    }
                }
            }
        }
    }

    public class X86InstructionListViewItem : ListViewItem
    {
        private static readonly FasmX86Formatter Formatter = new FasmX86Formatter();

        public X86InstructionListViewItem(X86Instruction instruction, byte[] bytes)
        {
            Instruction = instruction;
            Bytes = bytes;
        }

        public X86Instruction Instruction
        {
            get;
            private set;
        }

        public byte[] Bytes
        {
            get;
            set;
        }

        public int RelativeOffset
        {
            get;
            set;
        }

        public ILToNativeMap? Mapping
        {
            get;
            set;
        }

        public void UpdateItem(IFrame frame)
        {
            SubItems.Clear();
            Text = Instruction.Offset.ToString("X8");

            SubItems.AddRange(new string[]
            {
                string.Join(" ", Bytes.Select(x => x.ToString("X2"))),
                Formatter.FormatMnemonic(Instruction.Mnemonic),
                Formatter.FormatOperand(Instruction.Operand1),
                Formatter.FormatOperand(Instruction.Operand2),
            });

            BackColor = GetBackgroundColor(frame);
        }

        private Color GetBackgroundColor(IFrame frame)
        {
            if (Mapping == null)
            {
                if (frame.GetOffset() == Instruction.Offset)
                {
                    EnsureVisible();
                    return Color.Yellow;
                }
            }
            else if (frame.GetOffset() == Mapping.Value.ILOffset)
            {
                if (RelativeOffset == Mapping.Value.NativeStartOffset)
                {
                    EnsureVisible();
                    return Color.Yellow;
                }
                return Color.FromArgb(255, 255, 160);
            }

            return Color.Transparent;
        }
    }
}
