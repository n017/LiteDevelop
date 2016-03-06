using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using LiteDevelop.Framework;

namespace LiteDevelop.Debugger.Gui
{
    public partial class ThreadsControl : UserControl
    {
        private static readonly Dictionary<ThreadState, string> _threadStateMuiIdentifiers = new Dictionary<ThreadState, string>()
        {
            {ThreadState.Running, "ThreadsContent.ThreadStates.Running"},
            {ThreadState.StopRequested, "ThreadsContent.ThreadStates.StopRequested"},
            {ThreadState.SuspendRequested, "ThreadsContent.ThreadStates.SuspendRequested"},
            {ThreadState.Background, "ThreadsContent.ThreadStates.Background"},
            {ThreadState.Unstarted, "ThreadsContent.ThreadStates.Unstarted"},
            {ThreadState.Stopped, "ThreadsContent.ThreadStates.Stopped"},
            {ThreadState.WaitSleepJoin, "ThreadsContent.ThreadStates.WaitSleepJoin"},
            {ThreadState.Suspended, "ThreadsContent.ThreadStates.Suspended"},
            {ThreadState.AbortRequested, "ThreadsContent.ThreadStates.AbortRequested"},
            {ThreadState.Aborted, "ThreadsContent.ThreadStates.Aborted"},
        };

        private static string ThreadStateAsString(ThreadState state)
        {
            var values = (ThreadState[])Enum.GetValues(typeof(ThreadState));
            var extractedValues = new List<string>();

            foreach (var value in values)
            {
                if ((state & value) == value)
                {
                    extractedValues.Add(DebuggerBase.Instance.MuiProcessor.GetString(_threadStateMuiIdentifiers[value]));
                }
            }

            return string.Join(", ", extractedValues);
        }

        private readonly Dictionary<object, string> _componentMuiIdentifiers = new Dictionary<object, string>();
        private readonly ImageList _imageList;

        public ThreadsControl()
        {
            InitializeComponent();

            listView1.SmallImageList = _imageList = new ImageList()
            {
                ColorDepth = ColorDepth.Depth32Bit,
            };
            _imageList.Images.AddRange(new Image[]
            {
                new Bitmap(1,1),
                Properties.Resources.arrow_yellow,
            });

            _componentMuiIdentifiers = new Dictionary<object, string>()
            {
                {columnHeader1, "ThreadsContent.ListHeaders.Handle"},
                {columnHeader2, "ThreadsContent.ListHeaders.Id"},
                {columnHeader3, "ThreadsContent.ListHeaders.State"},
            };

            DebuggerBase.Instance.ExtensionHost.UILanguageChanged += ExtensionHost_UILanguageChanged;
            ExtensionHost_UILanguageChanged(null, null);
        }

        public void UpdateControl(IThread currentThread)
        {
            if (currentThread == null)
            {
                listView1.Items.Clear();
                return;
            }

            UpdateList(currentThread);
        }

        private void UpdateAllItems(IThread currentThread)
        {
            foreach (ListViewItem item in listView1.Items)
                UpdateThreadItem(item, currentThread);
        }

        private void UpdateList(IThread currentThread)
        {
            var provider = currentThread.Parent;
            var threads = provider.GetThreads().ToList();

            int index = 0;

            // remove threads that exited.
            while(index < listView1.Items.Count)
            {
                var thread = listView1.Items[index].Tag as IThread;
                if (!threads.Contains(thread))
                {
                    listView1.Items[index].Remove();
                    index--;
                }
                threads.Remove(thread);
                index++;
            }

            // add thread items that are not added to the list yet.
            foreach (var thread in threads)
                AddThread(thread);

            // update items.
            foreach (ListViewItem item in listView1.Items)
                UpdateThreadItem(item, currentThread);

        }

        private void AddThread(IThread thread)
        {
            var item = new ListViewItem(new string[3])
            {
                Tag = thread,
            };
           
            UpdateThreadItem(item, null);
       
            listView1.Items.Add(item);
        }
       
        private void RemoveThread(IThread thread)
        {
            foreach (ListViewItem item in listView1.Items)
            {
                if (item.Tag == thread)
                {
                    item.Remove();
                    break;
                }
            }
        }

        private void UpdateThreadItem(ListViewItem item, IThread currentThread)
        {
            var thread = item.Tag as IThread;
            item.SubItems[0].Text = thread.Handle.ToString();
            item.SubItems[1].Text = thread.Id.ToString();
            item.SubItems[2].Text = ThreadStateAsString(thread.State);
            item.ImageIndex = currentThread == thread ? 1 : 0;
        }

        private void ExtensionHost_UILanguageChanged(object sender, EventArgs e)
        {
            DebuggerBase.Instance.MuiProcessor.ApplyLanguageOnComponents(_componentMuiIdentifiers);
        }
    }
}
