﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using FastColoredTextBoxNS;
using LiteDevelop.Essentials.CodeEditor.Gui.Styles;
using LiteDevelop.Essentials.CodeEditor.Syntax;
using LiteDevelop.Framework;
using LiteDevelop.Framework.Debugging;
using LiteDevelop.Framework.FileSystem;
using LiteDevelop.Framework.FileSystem.Projects;
using LiteDevelop.Framework.Languages;

namespace LiteDevelop.Essentials.CodeEditor.Gui
{
    public partial class CodeEditorControl : UserControl
    {
        private static readonly MarkerStyle _sameWordsStyle = new MarkerStyle(new SolidBrush(Color.FromArgb(40, Color.Gray)));

        private readonly Dictionary<object, string> _componentMuiIdentifiers;
        private readonly OpenedFile _file;
        private readonly CodeEditorInstructionPointer _instructionPointer;
        private AutocompleteMenu _autoCompleteMenu;
        private AutoCompletionMap _autoCompletionMap;
        private InternalAutoCompletionMap _itemEnumerator;
        private EditorLayoutData _layoutData;
        private readonly CodeEditorContent _content;
        private readonly CodeEditorExtension _extension;
        private bool _justCompletedBrace;
        private Range _lastIpRange;
        private readonly ErrorStyle _errorStyle;
        private readonly WarningStyle _warningStyle;
        
        public CodeEditorControl(CodeEditorContent content, OpenedFile file)
        {
            InitializeComponent();

            _content = content;
            _extension = (CodeEditorExtension)content.ParentExtension;
            _file = file;

            SetupTextBox();
            _errorStyle = new ErrorStyle(_extension.StyleMap.DefaultText.Description);
            _warningStyle = new WarningStyle(_extension.StyleMap.DefaultText.Description);

            contextMenuStrip1.Renderer = _extension.ExtensionHost.ControlManager.MenuRenderer;

            _componentMuiIdentifiers = new Dictionary<object, string>()
            {
                {this.cutToolStripMenuItem, "CodeEditorControl.ContextMenu.Cut"},
                {this.copyToolStripMenuItem, "CodeEditorControl.ContextMenu.Copy"},
                {this.pasteToolStripMenuItem, "CodeEditorControl.ContextMenu.Paste"},
                {this.selectAllToolStripMenuItem, "CodeEditorControl.ContextMenu.SelectAll"},
            };

            _extension.ExtensionHost.UILanguageChanged += ExtensionHost_UILanguageChanged;
            _extension.ExtensionHost.BookmarkManager.Bookmarks.InsertedItem += Bookmarks_InsertedItem;
            _extension.ExtensionHost.BookmarkManager.Bookmarks.RemovedItem += Bookmarks_RemovedItem;
            _extension.ExtensionHost.ErrorManager.Errors.InsertedItem += Errors_InsertedItem;
            _extension.ExtensionHost.ErrorManager.Errors.RemovedItem += Errors_RemovedItem;
            _extension.ExtensionHost.ErrorManager.Errors.ClearedCollection += Errors_ClearedCollection;
            _extension.ExtensionHost.DebugStarted += ExtensionHost_DebugStarted;
            _extension.ExtensionHost.DebugStopped += ExtensionHost_DebugStopped;

            _instructionPointer = new CodeEditorInstructionPointer(TextBox, _extension.StyleMap.InstructionPointerStyle);

            if (_extension.ExtensionHost.IsDebugging)
                ExtensionHost_DebugStarted(null, null);

            ExtensionHost_UILanguageChanged(null, null);

            _extension.AppliedSettings += extension_AppliedSettings;
            extension_AppliedSettings(null, null);

            file.HasUnsavedDataChanged += file_HasUnsavedDataChanged;

        }

        public FastColoredTextBox TextBox
        {
            get { return fastColoredTextBox1; }
        }

        #region Utilities

        private void SetupTextBox()
        {
            this.TextBox.BeginUpdate();
            
            this.TextBox.Font = new Font("Consolas", 9.75F);
            this.TextBox.Text = _file.GetContentsAsString();
            this.TextBox.AllowDrop = true;
 
            InitializeEditorLayout();
            AddRegisteredBookmarks();

            this.TextBox.ClearUndo();
            this.TextBox.EndUpdate();

            this.TextBox.IsChanged = false;
            this.TextBox.Update();

            AddTextBoxEventHandlers();

            _extension.SetCurrentLocation(1, 1);
        }

        private void AddRegisteredBookmarks()
        {
            foreach (var bookmark in _extension.ExtensionHost.BookmarkManager.GetBookmarks(_file.FilePath))
            {
                TextBox.Bookmarks.Add(CreateBookmark(bookmark));
            }
        }
        
        private void AddTextBoxEventHandlers()
        {
            this.TextBox.MouseUp += TextBox_MouseUp;
            this.TextBox.DragEnter += TextBox_DragEnter;
            this.TextBox.DragDrop += TextBox_DragDrop;
            this.TextBox.SelectionChanged += TextBox_SelectionChanged;
            this.TextBox.SelectionChangedDelayed += this.TextBox_SelectionChangedDelayed;
            this.TextBox.TextChanged += this.TextBox_TextChanged;
            this.TextBox.KeyDown += this.TextBox_KeyDown;
            this.TextBox.KeyPressing += this.TextBox_KeyPressing;
            this.TextBox.DelayedEventsInterval = 500;
            this.TextBox.DelayedTextChangedInterval = 1000;
            this.TextBox.TextChangedDelayed += this.TextBox_TextChangedDelayed;
            this.TextBox.ScrollbarsUpdated += TextBox_ScrollbarsUpdated;
            this.TextBox.ZoomChanged += TextBox_ZoomChanged;
            this.TextBox.LineInserted += TextBox_LineCollectionChanged;
            this.TextBox.LineRemoved += TextBox_LineCollectionChanged;
            this.TextBox.ToolTipNeeded += TextBox_ToolTipNeeded;
        }
        
        private CodeEditorBookmark CreateBookmark(Framework.FileSystem.Bookmark bookmark)
        {
            var breakpoint = bookmark as BreakpointBookmark;
            if (breakpoint != null)
            {
                return new CodeEditorBreakpoint(TextBox, breakpoint, _extension.StyleMap.BreakpointStyle);
            }

            return new CodeEditorBookmark(TextBox, bookmark, _extension.StyleMap.BookmarkStyle);
        }

        private void UpdateBookmarks()
        {
            foreach (var bookmark in this.TextBox.Bookmarks)
            {
                var codeEditorBookmark = bookmark as CodeEditorBookmark;
                if (codeEditorBookmark != null)
                {
                    codeEditorBookmark.InnerBookmark.Location = new SourceLocation(_file.FilePath, codeEditorBookmark.LineIndex + 1, 0);
                }
            }
        }

        private void InitializeEditorLayout()
        {
            _autoCompleteMenu = new AutocompleteMenu(this.TextBox);
            
            var language = LanguageDescriptor.GetLanguageByPath(_file.FilePath);
            _layoutData = EditorLayoutData.GetLayout(language);

            if (_layoutData != null)
            {
                _autoCompletionMap = _layoutData.CreateAutoCompletionMap(_autoCompleteMenu);
            }

            if (_autoCompletionMap != null)
            {
                this.TextBox_TextChangedDelayed(this, null);
                _autoCompleteMenu.Items.SetAutocompleteItems(_itemEnumerator = new InternalAutoCompletionMap(_autoCompletionMap));
                _autoCompleteMenu.AppearInterval = 10;
                _autoCompleteMenu.MinFragmentLength = 1;
                _autoCompleteMenu.Opening += _autoCompleteMenu_Opening;
                _autoCompleteMenu.Opened += _autoCompleteMenu_Opened;                
                _autoCompleteMenu.SearchPattern = _autoCompletionMap.SearchPattern;
                _autoCompleteMenu.Selecting += _autoCompleteMenu_Selecting;
            }
            else
            {
                _itemEnumerator = new InternalAutoCompletionMap(null);
            }

        }

        private void InitializeBookmarks()
        {
            foreach (var bookmark in _extension.ExtensionHost.BookmarkManager.GetBookmarks(_file.FilePath))
            {
                if (bookmark is BreakpointBookmark)
                {
                    this.TextBox.Bookmarks.Add(new CodeEditorBreakpoint(TextBox, bookmark as BreakpointBookmark, _extension.StyleMap.BreakpointStyle));
                }
                else
                {
                    this.TextBox.Bookmarks.Add(new CodeEditorBookmark(TextBox, bookmark, null));
                }
            }
        }

        private void ToggleBreakpoint(int line)
        {
            bool add = true;
            foreach (var bookmark in TextBox.Bookmarks)
            {
                if (bookmark.LineIndex == line - 1)
                {
                    _extension.ExtensionHost.BookmarkManager.Bookmarks.Remove((bookmark as CodeEditorBookmark).InnerBookmark);
                    add = false;
                    break;
                }
            }

            if (add)
            {
                _extension.ExtensionHost.BookmarkManager.Bookmarks.Add(new BreakpointBookmark(new SourceLocation(_file.FilePath, line, 1)));
            }
        }

        private void HighlightSyntax(Range range)
        {
            if (CodeEditorExtension.Instance.Settings.GetValue<bool>("General.SyntaxHighlighting"))
            {
                TextBox.SyntaxHighlighter.KeywordStyle = _extension.StyleMap.KeywordStyle;
                TextBox.SyntaxHighlighter.StringStyle = _extension.StyleMap.StringStyle;
                TextBox.SyntaxHighlighter.ClassNameStyle = _extension.StyleMap.TypeDefinitionStyle;
                TextBox.SyntaxHighlighter.NumberStyle = _extension.StyleMap.NumberStyle;
                TextBox.SyntaxHighlighter.CommentStyle = _extension.StyleMap.CommentStyle;
                TextBox.SyntaxHighlighter.AttributeStyle = _extension.StyleMap.AttributeStyle;

                if (_layoutData.TextBoxSyntaxDescriptor != null)
                    this.TextBox.SyntaxHighlighter.HighlightSyntax(_layoutData.TextBoxSyntaxDescriptor, range);
                else
                    this.TextBox.SyntaxHighlighter.HighlightSyntax(_layoutData.TextBoxLanguage, range);
            }
        }

        private void UpdateSnapshotAsync()
        {
			var snapshotProvider = _autoCompletionMap as ISnapshotProvider;
            if (snapshotProvider != null)
            {
                string currentSource = TextBox.Text;
                new Thread(() =>
                {
                    snapshotProvider.UpdateSnapshot(currentSource);
                }) { IsBackground = true }.Start();
            }
        }

        private AutocompleteItem GetLastUsedAutoCompleteItem()
        {
            if (_autoCompleteMenu != null)
            {
                var visibleItems = _autoCompleteMenu.Items.VisibleItems;
                for (int i = 0; i < _extension.LastUsedItems.Count; i++)
                {
                    for (int j = 0; j < visibleItems.Count; j++)
                    {
                        if (visibleItems[j].MenuText == _extension.LastUsedItems[i])
                        {
                            return visibleItems[j];
                        }
                    }
                }
            }

            return null;
        }

        private void SelectAutoCompleteItem(AutocompleteItem item)
        {
            if (_autoCompleteMenu != null)
            {
                _autoCompleteMenu.Items.FocussedItem = item;
                _autoCompleteMenu.Items.DoSelectedVisible();
            }
        }

        internal void CommentSelected()
        {
            TextBox.InsertLinePrefix(_layoutData.Language.CommentPrefix);
            InsertLineSuffix(_layoutData.Language.CommentSuffix);
        }

        internal void UncommentSelected()
        {
            TextBox.RemoveLinePrefix(_layoutData.Language.CommentPrefix);
            RemoveLineSuffix(_layoutData.Language.CommentSuffix);
        }

        internal void ForceShowSuggestions()
        {
            if (_autoCompleteMenu != null && CodeEditorExtension.Instance.Settings.GetValue<bool>("AutoCompletion.AutoComplete"))
            {
                _itemEnumerator.CanYieldItems = true;
                _autoCompleteMenu.Show(true);
            }
        }

        private void AdjustScrollbar(ScrollBar scrollBar, int max, int value, int clientSize)
        {
            scrollBar.LargeChange = clientSize / 3;
            scrollBar.SmallChange = clientSize / 11;
            scrollBar.Maximum = max + scrollBar.LargeChange;
            scrollBar.Enabled = max > 0;
            scrollBar.Value = Math.Min(scrollBar.Maximum, value);
        }

        private void UpdateWrapper()
        {
            if (documentMap1.Visible)
            {
                int documentMapWidth = (int)Math.Min(textBoxWrapperPanel.Width * documentMap1.Scale, 150);
                fastColoredTextBox1.Width = textBoxWrapperPanel.Width - documentMapWidth;
                documentMap1.Width = documentMapWidth;
                documentMap1.Left = textBoxWrapperPanel.Width - documentMap1.Width;
            }
            else
                fastColoredTextBox1.Width = textBoxWrapperPanel.Width;

            fastColoredTextBox1.Height = textBoxWrapperPanel.Height;
        }

        private void InsertLineSuffix(string suffix)
        {
            Range old = TextBox.Selection.Clone();
            int from = Math.Min(TextBox.Selection.Start.iLine, TextBox.Selection.End.iLine);
            int to = Math.Max(TextBox.Selection.Start.iLine, TextBox.Selection.End.iLine);
            TextBox.BeginUpdate();
            TextBox.Selection.BeginUpdate();
            TextBox.TextSource.Manager.BeginAutoUndoCommands();
            TextBox.TextSource.Manager.ExecuteCommand(new SelectCommand(TextBox.TextSource));

            for (int i = from; i <= to; i++)
            {
                TextBox.Selection.Start = new Place(TextBox.Lines[i].Length, i);
                TextBox.TextSource.Manager.ExecuteCommand(new InsertTextCommand(TextBox.TextSource, suffix));
            }

            TextBox.Selection.Start = new Place(0, from);
            TextBox.Selection.End = new Place(TextBox.Lines[to].Length, to);
            //needRecalc = true;
            TextBox.TextSource.Manager.EndAutoUndoCommands();
            TextBox.Selection.EndUpdate();
            TextBox.EndUpdate();
            TextBox.Invalidate();
        }

        private void RemoveLineSuffix(string suffix)
        {
            Range old = TextBox.Selection.Clone();
            int from = Math.Min(TextBox.Selection.Start.iLine, TextBox.Selection.End.iLine);
            int to = Math.Max(TextBox.Selection.Start.iLine, TextBox.Selection.End.iLine);
            TextBox.BeginUpdate();
            TextBox.Selection.BeginUpdate();
            TextBox.TextSource.Manager.BeginAutoUndoCommands();
            TextBox.TextSource.Manager.ExecuteCommand(new SelectCommand(TextBox.TextSource));
            for (int i = from; i <= to; i++)
            {
                string text = TextBox.TextSource[i].Text;
                string trimmedText = text.TrimEnd();
                if (trimmedText.EndsWith(suffix))
                {
                    TextBox.Selection.Start = new Place(trimmedText.Length - suffix.Length, i);
                    TextBox.Selection.End = new Place(trimmedText.Length, i);
                    TextBox.ClearSelected();
                }
            }
            TextBox.Selection.Start = new Place(0, from);
            TextBox.Selection.End = new Place(TextBox.Lines[to].Length, to);
            //TextBox.needRecalc = true;
            TextBox.TextSource.Manager.EndAutoUndoCommands();
            TextBox.Selection.EndUpdate();
            TextBox.EndUpdate();
        }

        private void HighlightRange(SourceRange sourceRange, TextStyle style)
        {
            var range = new Range(TextBox, 
                sourceRange.Column - 1, 
                sourceRange.Line - 1,
                sourceRange.EndColumn - 1, 
                sourceRange.EndLine - 1);

            // remove last ip highlighting
            ClearLastHighlighting(style);

            // remove syntax highlighting and add ip highlighting
            range.ClearStyle(StyleIndex.All);
            range.SetStyle(style);
            
            _lastIpRange = range;
        }

        private void ClearLastHighlighting(TextStyle style)
        {
            if (_lastIpRange != null)
            {
                // clear ip highlighting
                _lastIpRange.ClearStyle(style);
                // add syntax highlighting
                HighlightSyntax(_lastIpRange);
            }
        }

        
        private Range GetErrorRange(SourceLocation errorLocation)
        {
            return GetErrorRange(new Place(errorLocation.Column - 1, errorLocation.Line - 1));
        }
        
        private Range GetErrorRange(Place location)
        {
            var range = TextBox.GetRange(location, new Place(location.iChar + 1, location.iLine));
            var fragment = range.GetFragment(@"\w");
            if (fragment.Text.Length == 0)
                return range;
            return fragment;
        }

        #endregion

        #region Event handlers

        #region Text box event handlers

        private void TextBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left && e.X < TextBox.LeftPadding &&
                _extension.ExtensionHost.CurrentSolution != null &&
                _extension.ExtensionHost.CurrentSolution.HasDebuggableProjects(_extension.ExtensionHost.ExtensionManager))
            {
                int line = TextBox.PointToPlace(e.Location).iLine;
                ToggleBreakpoint(line + 1);
            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Space && _autoCompleteMenu != null && CodeEditorExtension.Instance.Settings.GetValue<bool>("AutoCompletion.AutoComplete"))
            {
                // force show autocompletion with ctrl + space.
                _itemEnumerator.CanYieldItems = true;
                _autoCompleteMenu.Show(true);
            }

            if (_justCompletedBrace)
            {
                _justCompletedBrace = false;

                if (e.KeyCode == Keys.Enter)
                {
                    Place start = TextBox.Selection.Start;

                    TextBox.InsertText(Environment.NewLine + Environment.NewLine);

                    Place end = TextBox.Selection.End;

                    for (int i = start.iLine; i <= end.iLine; i++)
                        TextBox.DoAutoIndent(i);

                    do
                    {
                        TextBox.Selection.GoLeft();
                    }
                    while (TextBox.Selection.CharAfterStart != '\n');

                    e.Handled = true;
                }
            }
        }

        private void TextBox_KeyPressing(object sender, KeyPressEventArgs e)
        {
            var settings = CodeEditorExtension.Instance.Settings;
            _itemEnumerator.CanYieldItems = _autoCompleteMenu.Visible || settings.GetValue<bool>("AutoCompletion.ShowSuggestionsWhenTyping");

            if (_autoCompleteMenu != null && _autoCompleteMenu.Visible)
            {
                // auto complete when a key specified in the settings has been pressed.
                if ((settings.GetValue<bool>("AutoCompletion.AutoCompleteCommitOnSpaceBar") && e.KeyChar == ' ') 
                    || (settings.GetValue<bool>("AutoCompletion.AutoCompleteCommitOnTab") && e.KeyChar == '\t') 
                    || (settings.GetValue<string>("AutoCompletion.AutoCompleteCommitChars").Contains(e.KeyChar)))
                {
                    _autoCompleteMenu.OnSelecting();

                    var selectedItem = _autoCompleteMenu.Items.FocussedItem as CodeEditorAutoCompleteItem;
                    if (selectedItem != null)
                    {
                        // do not append space when surpressed by autocomplete item
                        if (e.KeyChar == ' ' && selectedItem.SurpressSpaceBar)
                        {
                            e.Handled = true;
                            return;
                        }

                        if (selectedItem is CodeEditorMethodAutoCompleteItem)
                        {
                            // do not append extra parantheses for methods when the paranthese key is pressed.
                            if (settings.GetValue<bool>("AutoCompletion.AutoCompleteMethodParantheses") && (selectedItem as CodeEditorMethodAutoCompleteItem).Parantheses[0] == e.KeyChar)
                            {
                                e.Handled = true;
                                return;
                            }
                        }
                    }
                }

            }

            if (_autoCompletionMap is ICodeBlockCompleter && this.TextBox.SelectionStart < this.TextBox.Text.Length)
            {
                var styles = this.TextBox.GetStylesOfChar(this.TextBox.Selection.Start);

                var blockCompleter = _autoCompletionMap as ICodeBlockCompleter;
                var startIdentifiers = blockCompleter.BlockIdentifiers.Keys;
                var endIdentifiers = blockCompleter.BlockIdentifiers.Values;
                string currentCharAsString = this.TextBox.Text[this.TextBox.SelectionStart].ToString();
                string nextCharAsString = this.TextBox.Text.Length < this.TextBox.SelectionStart ? this.TextBox.Text[this.TextBox.SelectionStart + 1].ToString() : null;
                string keyCharAsString = e.KeyChar.ToString();

                // do not handle ending blocks when they are already present
                if (endIdentifiers.Contains(currentCharAsString) && currentCharAsString == keyCharAsString)
                {
                    e.Handled = true;
                    this.TextBox.SelectionStart++;
                }
                // auto complete code blocks
                else if (settings.GetValue<bool>("AutoCompletion.AutoCompleteCodeBlocks") && !styles.Contains(this.TextBox.SyntaxHighlighter.CommentStyle) && !styles.Contains(this.TextBox.SyntaxHighlighter.StringStyle) && startIdentifiers.Contains(keyCharAsString))
                {
                    this.TextBox.InsertText(keyCharAsString + blockCompleter.BlockIdentifiers[keyCharAsString]);
                    this.TextBox.SelectionStart -= blockCompleter.BlockIdentifiers[keyCharAsString].Length;
                    e.Handled = true;
                    _justCompletedBrace = true;
                }

            }
        }
   
        private void TextBox_DragEnter(object sender, DragEventArgs e)
        {
            //redirect drag enter event
            _content.DispatchDragEnterEvent(e);
        }

        private void TextBox_DragDrop(object sender, DragEventArgs e)
        {
            //redirect drag drop event
            _content.DispatchDragDropEvent(e);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            HighlightSyntax(e.ChangedRange);
            _file.GiveUnsavedData();
        }

        private void TextBox_SelectionChangedDelayed(object sender, EventArgs e)
        {
            this.TextBox.Range.ClearStyle(_sameWordsStyle);
            string currentWord = this.TextBox.Selection.GetFragment("\\w").Text;
            var ranges = this.TextBox.Range.GetRanges("\\b" + currentWord + "\\b").ToArray();

            foreach (var r in ranges)
                r.SetStyle(_sameWordsStyle);
        }

        private void TextBox_SelectionChanged(object sender, EventArgs e)
        {
            var location = TextBox.Selection.Start;
            _extension.SetCurrentLocation(location.iLine + 1, location.iChar + 1);
        }

        private void TextBox_ScrollbarsUpdated(object sender, EventArgs e)
        {
            AdjustScrollbar(vScrollBar1, TextBox.VerticalScroll.Maximum, TextBox.VerticalScroll.Value, TextBox.ClientSize.Height);
            AdjustScrollbar(hScrollBar1, TextBox.HorizontalScroll.Maximum, TextBox.HorizontalScroll.Value, TextBox.ClientSize.Width);
        }

        private void TextBox_TextChangedDelayed(object sender, TextChangedEventArgs e)
        {
            UpdateSnapshotAsync();
        }

        private void TextBox_ZoomChanged(object sender, EventArgs e)
        {
            label1.Text = this.TextBox.Zoom + "%";
        }

        private void TextBox_LineCollectionChanged(object sender, EventArgs e)
        {
            UpdateBookmarks();
        }

        #endregion

        #region Auto completion menu event handlers

        private void _autoCompleteMenu_Selecting(object sender, SelectingEventArgs e)
        {
            _extension.AddLastUsedItem(e.Item.MenuText);

			var autoCompleteItem = e.Item as CodeEditorMethodAutoCompleteItem;
            if (autoCompleteItem != null)
            {
				autoCompleteItem.AppendParantheses = CodeEditorExtension.Instance.Settings.GetValue<bool>("AutoCompletion.AutoCompleteMethodParantheses");
            }
        }

        private void _autoCompleteMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!CodeEditorExtension.Instance.Settings.GetValue<bool>("AutoCompletion.AutoComplete"))
            {
                e.Cancel = true;
            }
            else
            {
                // prevent opening autocomplete menu when inside string or comment.
                var styles = this.TextBox.GetStylesOfChar(_autoCompleteMenu.Fragment.Start);

                if (styles.Contains(this.TextBox.SyntaxHighlighter.CommentStyle) || styles.Contains(this.TextBox.SyntaxHighlighter.StringStyle))
                {
                    e.Cancel = true;
                    return;
                }
            }
        }

        private void _autoCompleteMenu_Opened(object sender, EventArgs e)
        {
            var lastUsedItem = GetLastUsedAutoCompleteItem();
            if (lastUsedItem != null)
                SelectAutoCompleteItem(lastUsedItem);
        }

        #endregion

        #region Context menu event handlers

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TextBox.Cut();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TextBox.Copy();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TextBox.Paste();
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TextBox.SelectAll();
        }

        #endregion

        #region Extension and host event handlers

        private void extension_AppliedSettings(object sender, EventArgs e)
        {
            var settings = _extension.Settings;

            this.TextBox.BeginUpdate();

            this.TextBox.WordWrap = settings.GetValue<bool>("General.WordWrap");
            this.TextBox.ShowLineNumbers = settings.GetValue<bool>("General.LineNumbers");
            this.TextBox.CurrentLineColor = settings.GetValue<bool>("General.HighlightCurrentLine") ? _extension.StyleMap.CurrentLineStyle.Description.BackColor : Color.Transparent;
            this.TextBox.ChangedLineColor = settings.GetValue<bool>("General.TrackUnsavedChanges") ? _extension.StyleMap.ChangedLineStyle.Description.BackColor : Color.Transparent;
            this.TextBox.SelectionColor = _extension.StyleMap.SelectionStyle.Description.BackColor;
            _autoCompleteMenu.AllowTabKey = settings.GetValue<bool>("AutoCompletion.AutoCompleteCommitOnTab");
            var appearance = _extension.StyleMap.DefaultText.Description;
            this.TextBox.BackColor = this.documentMap1.BackColor = appearance.BackColor;
            this.TextBox.ForeColor = appearance.ForeColor;
            this.TextBox.Font = new System.Drawing.Font(this.TextBox.Font, appearance.FontStyle);

            appearance = _extension.StyleMap.LineNumbers.Description;
            this.TextBox.LineNumberColor = appearance.ForeColor;
            this.TextBox.IndentBackColor = appearance.BackColor;
            // TODO: set line number font somehow... (maybe edit in fastcoloredtextbox?)

            this.documentMap1.Visible = settings.GetValue<bool>("General.ShowDocumentMiniMap");

            if (settings.GetValue<bool>("General.SyntaxHighlighting") && _layoutData != null)
            {
                this.TextBox.Language = _layoutData.TextBoxLanguage;
                HighlightSyntax(this.TextBox.Range);
            }
            else
            {
                this.TextBox.Language = Language.Custom;
                this.TextBox.ClearStylesBuffer();
            }

            this.TextBox.EndUpdate();
        }
       
        private void Bookmarks_InsertedItem(object sender, CollectionChangedEventArgs e)
        {
            var bookmark = e.TargetObject as Framework.FileSystem.Bookmark;
            if (bookmark.Location.FilePath == _file.FilePath)
                TextBox.Bookmarks.Add(CreateBookmark(bookmark));
        }

        private void Bookmarks_RemovedItem(object sender, CollectionChangedEventArgs e)
        {
            var bookmark = e.TargetObject as Framework.FileSystem.Bookmark;
            if (bookmark.Location.FilePath == _file.FilePath)
            {
                foreach (var tbBookmark in TextBox.Bookmarks)
                    if (tbBookmark.LineIndex == bookmark.Location.Line - 1)
                    {
                        TextBox.Bookmarks.Remove(tbBookmark);
                        break;
                    }
            }
        }

        private void ExtensionHost_DebugStarted(object sender, EventArgs e)
        {
            _extension.ExtensionHost.CurrentDebuggerSession.Paused += CurrentDebuggerSession_Paused;
            _extension.ExtensionHost.CurrentDebuggerSession.Resumed += CurrentDebuggerSession_Resumed;
            CurrentDebuggerSession_Paused(_extension.ExtensionHost.CurrentDebuggerSession, EventArgs.Empty);
            fastColoredTextBox1.ReadOnly = true;
        }

        private void ExtensionHost_DebugStopped(object sender, EventArgs e)
        {
            _extension.ExtensionHost.CurrentDebuggerSession.Paused -= CurrentDebuggerSession_Paused;
            fastColoredTextBox1.ReadOnly = false;
            TextBox.Bookmarks.Remove(_instructionPointer);
            ClearLastHighlighting(_instructionPointer.Style);
        }

        private void CurrentDebuggerSession_Resumed(object sender, EventArgs e)
        {
            TextBox.Bookmarks.Remove(_instructionPointer);
            ClearLastHighlighting(_instructionPointer.Style);
        }

        private void CurrentDebuggerSession_Paused(object sender, EventArgs e)
        {
            var session = sender as DebuggerSession;
            var range = session.CurrentSourceRange;
            if (range != null && range.FilePath == _file.FilePath)
            {
                _instructionPointer.InnerBookmark.Location = range;
                HighlightRange(range, _instructionPointer.Style);

                if (!TextBox.Bookmarks.Contains(_instructionPointer))
                    TextBox.Bookmarks.Add(_instructionPointer);

                _instructionPointer.DoVisible();
            }
            else
            {
                ClearLastHighlighting(_instructionPointer.Style);
                TextBox.Bookmarks.Remove(_instructionPointer);
            }
        }

        private void Errors_ClearedCollection(object sender, EventArgs e)
        {
            TextBox.Range.ClearStyle(_errorStyle, _warningStyle);
        }

        private void Errors_RemovedItem(object sender, CollectionChangedEventArgs e)
        {
            var error = e.TargetObject as BuildError;
            if (error.Location.FilePath == this._file.FilePath)
            {
                GetErrorRange(error.Location).ClearStyle(error.Severity == MessageSeverity.Error ? (Style)_errorStyle : _warningStyle);
            }
        }

        private void Errors_InsertedItem(object sender, CollectionChangedEventArgs e)
        {
            var error = e.TargetObject as BuildError;
            if (error.Location.FilePath == this._file.FilePath)
            {
                GetErrorRange(error.Location).SetStyle(error.Severity == MessageSeverity.Error ? (Style)_errorStyle : _warningStyle);
            }
        }

        private void ExtensionHost_UILanguageChanged(object sender, EventArgs e)
        {
            _extension.MuiProcessor.ApplyLanguageOnComponents(_componentMuiIdentifiers);
            var location = TextBox.Selection.Start;
            _extension.SetCurrentLocation(location.iLine + 1, location.iChar + 1);
        }

        private void TextBox_ToolTipNeeded(object sender, ToolTipNeededEventArgs e)
        {
            foreach (var error in _extension.ExtensionHost.ErrorManager.Errors)
            {
                if (error.Location.FilePath == _file.FilePath &&
                    GetErrorRange(error.Location).Start == GetErrorRange(e.Place).Start)
                {
                    switch (error.Severity)
                    {
                        case MessageSeverity.Error: e.ToolTipIcon = ToolTipIcon.Error; break;
                        case MessageSeverity.Warning: e.ToolTipIcon = ToolTipIcon.Warning; break;
                        default: e.ToolTipIcon = ToolTipIcon.Info; break;
                    }

                    e.ToolTipTitle = error.Severity.ToString();
                    e.ToolTipText = error.Message;
                    return;
                }
            }
        }
        
        #endregion

        #region Other event handlers

        private void ScrollBars_Scroll(object sender, ScrollEventArgs e)
        {
            TextBox.OnScroll(e, e.Type != ScrollEventType.ThumbTrack && e.Type != ScrollEventType.ThumbPosition);
        }

        private void textBoxWrapperPanel_SizeChanged(object sender, EventArgs e)
        {
            UpdateWrapper();
        }

        private void documentMap1_VisibleChanged(object sender, EventArgs e)
        {
            documentMap1.Target = Visible ? TextBox : null;
            UpdateWrapper();
        }

        private void file_HasUnsavedDataChanged(object sender, EventArgs e)
        {
            TextBox.IsChanged = (sender as OpenedFile).HasUnsavedData;
            TextBox.Invalidate();
        }

        #endregion

        #endregion

        private sealed class InternalAutoCompletionMap : IEnumerable<AutocompleteItem>
        {
            private AutoCompletionMap _map;

            public InternalAutoCompletionMap(AutoCompletionMap map)
            {
                _map = map;
            }

            public bool CanYieldItems
            {
                get;
                set;
            }

            public IEnumerator<AutocompleteItem> GetEnumerator()
            {
                if (CanYieldItems && _map != null)
                    return _map.GetEnumerator();

                return Enumerable.Empty<AutocompleteItem>().GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        internal void BookmarkCurrentLine()
        {
            if (_extension.ExtensionHost.BookmarkManager.GetBookmarksOnLine(_file.FilePath, TextBox.Selection.Start.iLine).Count() == 0)
            {
                _extension.ExtensionHost.BookmarkManager.Bookmarks.Add(
                    new Framework.FileSystem.Bookmark(new SourceLocation(_file.FilePath, TextBox.Selection.Start.iLine + 1, 1)));
            }
        }

        internal void UnbookmarkCurrentLine()
        {
            _extension.ExtensionHost.BookmarkManager.ClearBookmarksOnLine(_file.FilePath, TextBox.Selection.Start.iLine + 1);
        }
    }
}
