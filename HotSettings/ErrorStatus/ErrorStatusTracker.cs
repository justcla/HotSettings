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
            var caretPosition = this.textView.Caret.Position.BufferPosition;

            // Get all error tags that intersect with the caret.
            var errorTagsAtCaret = this.errorTagAggregator.GetTags(new SnapshotSpan(caretPosition, 0));

            // Get first, highest priority error tag.
            foreach (var errorTypeDefinition in this.factory.OrderedErrorTypeDefinitions)
            {
                var firstMatchingTag = errorTagsAtCaret.FirstOrDefault(
                    tag => string.Equals(tag.Tag.ErrorType, errorTypeDefinition.Metadata.Name, StringComparison.OrdinalIgnoreCase));
                if (firstMatchingTag != null)
                {
                    this.UpdateStatusBarFromErrorTag(firstMatchingTag);

                    return;
                }
            }

            Marshal.ThrowExceptionForHR(this.factory.StatusBarService.Clear());
        }

        private void UpdateStatusBarFromErrorTag(IMappingTagSpan<IErrorTag> mappingTagSpan)
        {
            var errorTagContent = mappingTagSpan.Tag.ToolTipContent.ToString();
            Marshal.ThrowExceptionForHR(this.factory.StatusBarService.SetText(errorTagContent));
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
