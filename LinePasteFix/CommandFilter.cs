using System;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.TextManager.Interop;

namespace CMcG.LinePasteFix
{
    public class CommandFilter : IOleCommandTarget
    {
        static readonly Command cmdPaste      = new Command("5efc7975-14bc-11cf-9b2b-00aa00573819", 26),
                                cmdCut        = new Command("5efc7975-14bc-11cf-9b2b-00aa00573819", 16),
                                cmdLineDelete = new Command("1496a755-94de-11d0-8c3f-00c04fc2aae2", 62),
                                cmdLineCut    = new Command("1496a755-94de-11d0-8c3f-00c04fc2aae2", 61);

        readonly IWpfTextView m_view;

        public CommandFilter(IWpfTextView view)
        {
            m_view = view;
        }

        public IOleCommandTarget Next { get; set; }

        public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            int index      = -1;
            bool isFixable = (IsLinePaste     (pguidCmdGroup, nCmdID)
                          ||  cmdCut       .Is(pguidCmdGroup, nCmdID) && m_view.Selection.IsEmpty
                          ||  cmdLineDelete.Is(pguidCmdGroup, nCmdID)
                          ||  cmdLineCut   .Is(pguidCmdGroup, nCmdID));

            if (isFixable)
            {
                var caret = m_view.Caret.Position.BufferPosition;
                index     = m_view.GetTextViewLineContainingBufferPosition(caret).Start.Difference(caret)
                          + m_view.Caret.Position.VirtualSpaces;
            }

            int result = Next.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);

            if (isFixable)
            {
                var caret = m_view.Caret.Position.BufferPosition;
                if (m_view.GetTextViewLineContainingBufferPosition(caret).Start.Difference(caret) + m_view.Caret.Position.VirtualSpaces > 0)
                    return result;

                int lineLength    = m_view.Caret.Position.BufferPosition.GetContainingLine().Length;
                var virtualSpaces = (lineLength < index) ? index - lineLength : 0;
                index             = (lineLength < index) ? lineLength         : index;

                m_view.Caret.MoveTo(new VirtualSnapshotPoint(m_view.Caret.Position.BufferPosition.Add(index), virtualSpaces));
            }

            return result;
        }

        bool IsLinePaste(Guid pguidCmdGroup, uint nCmdID)
        {
            if (!cmdPaste.Is(pguidCmdGroup, nCmdID))
                return false;

            var text = System.Windows.Clipboard.GetText();
            return text.EndsWith(Environment.NewLine);
        }

        public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            return Next.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
        }

        public static void Register(IVsTextView textViewAdapter, CommandFilter filter)
        {
            if (ErrorHandler.Succeeded(textViewAdapter.AddCommandFilter(filter, out var next)))
                filter.Next = next;
        }
    }
}