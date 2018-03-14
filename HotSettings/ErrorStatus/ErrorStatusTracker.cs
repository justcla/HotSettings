using System.Collections.Generic;

namespace HotSettings.ErrorStatus
{
    using System;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Windows.Input;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text.Tagging;

    internal sealed class ErrorStatusTracker
    {
        private readonly IWpfTextView textView;
        private readonly ErrorStatusTextViewCreationListener factory;
        private readonly ITagAggregator<IErrorTag> errorTagAggregator;

        public static void Attach(IWpfTextView textView, ErrorStatusTextViewCreationListener factory) => new ErrorStatusTracker(textView, factory);

        private ErrorStatusTracker(
            IWpfTextView textView,
            ErrorStatusTextViewCreationListener factory)
        {
            this.textView = textView;
            this.factory = factory;

            this.errorTagAggregator = factory.TagAggregatorFactoryService.CreateTagAggregator<IErrorTag>(textView.TextBuffer);
            this.errorTagAggregator.BatchedTagsChanged += this.OnBatchedTagsChanged;

            textView.Closed += OnTextViewClosed;
            textView.Caret.PositionChanged += this.OnCaretPositionChanged;
            textView.VisualElement.GotKeyboardFocus += this.OnGotKeyboardFocus;
        }

        private void OnBatchedTagsChanged(object sender, BatchedTagsChangedEventArgs e) => this.ShowErrorTagContentAtCaret();

        private void OnCaretPositionChanged(object sender, CaretPositionChangedEventArgs e) => this.ShowErrorTagContentAtCaret();

        private void OnGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e) => this.ShowErrorTagContentAtCaret();

        private void ShowErrorTagContentAtCaret()
        {
            // Get all error tags that intersect with the caret.
            var caretPosition = this.textView.Caret.Position.BufferPosition;
            var errorTagsAtCaret = this.errorTagAggregator.GetTags(new SnapshotSpan(caretPosition, 0));

            // Optimisation: ErrorTags list is usually empty (or one). List of known error types is 6+ items.
            // Therefore, avoid iterating through the error type list where possible.
            // Convert the enum to list so we can easily see if it's empty or one.
            // Only where there are more than one ErrorTag do we need to sort by priority.
            var errorTagList = errorTagsAtCaret.ToList();
            switch (errorTagList.Count)
            {
                case 0:
                    // No error tag at caret location. Clear the status bar.
                    ClearStatusBarText();
                    return;
                case 1:
                    // Only one error tag at this location. Show it on the status bar.
                    this.UpdateStatusBarFromErrorTag(errorTagList[0]);
                    return;
                default:
                    // More than one error tag. Show highest priority error.
                    var highestPriorityErrorTag = GetHighestPriorityErrorTag(errorTagList);
                    this.UpdateStatusBarFromErrorTag(highestPriorityErrorTag);
                    return;
            }
        }

        private IMappingTagSpan<IErrorTag> GetHighestPriorityErrorTag(List<IMappingTagSpan<IErrorTag>> mappingTagSpans)
        {
            // Get first, highest priority error tag.
            return this.factory.OrderedErrorTypeDefinitions
                .Select(errorTypeDefinition => mappingTagSpans.FirstOrDefault(tag =>
                    string.Equals(tag.Tag.ErrorType, errorTypeDefinition.Metadata.Name,
                        StringComparison.OrdinalIgnoreCase)))
                .FirstOrDefault(firstMatchingTag => firstMatchingTag != null);
        }

        private void UpdateStatusBarFromErrorTag(IMappingTagSpan<IErrorTag> mappingTagSpan)
        {
            var errorTagContent = mappingTagSpan?.Tag?.ToolTipContent?.ToString();
            if (errorTagContent != null)
            {
                SetStatusBarText(errorTagContent);
            }
        }

        private void SetStatusBarText(string errorTagContent)
        {
            Marshal.ThrowExceptionForHR(this.factory.StatusBarService.SetText(errorTagContent));
        }

        private void ClearStatusBarText()
        {
            Marshal.ThrowExceptionForHR(this.factory.StatusBarService.Clear());
        }

        private void OnTextViewClosed(object sender, System.EventArgs e)
        {
            this.errorTagAggregator.BatchedTagsChanged -= this.OnBatchedTagsChanged;
            this.errorTagAggregator.Dispose();

            this.textView.Closed -= this.OnTextViewClosed;
            this.textView.Caret.PositionChanged -= this.OnCaretPositionChanged;
            textView.VisualElement.GotKeyboardFocus -= this.OnGotKeyboardFocus;
        }
    }
}
