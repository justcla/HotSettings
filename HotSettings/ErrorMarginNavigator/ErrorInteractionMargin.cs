namespace HotSettings.ErrorMarginNavigator
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text.Tagging;

    internal sealed class ErrorInteractionMargin : FrameworkElement, IWpfTextViewMargin
    {
        private readonly IWpfTextViewHost wpfTextViewHost;
        private readonly IWpfTextViewMargin marginContainer;
        private readonly ErrorInteractionMarginProvider factory;
        private readonly ToolTip tooltip = new ToolTip();

        private IWpfTextViewMargin errorMargin;
        private bool isDisposed;
        private ITagAggregator<IErrorTag> errorTagAggregator;

        public ErrorInteractionMargin(IWpfTextViewHost wpfTextViewHost, IWpfTextViewMargin marginContainer, ErrorInteractionMarginProvider factory)
        {
            this.wpfTextViewHost = wpfTextViewHost;
            this.marginContainer = marginContainer;
            this.factory = factory;

            marginContainer.VisualElement.MouseLeave += this.OnMouseLeave;
            marginContainer.VisualElement.MouseMove += this.OnMouseMove;
            marginContainer.VisualElement.PreviewMouseLeftButtonDown += this.OnLeftButtonDown;

            this.tooltip.PlacementTarget = marginContainer.VisualElement;

            // This margin only exists to hook UI input, so, hide it. Tried to avoid
            // creating an actual FrameworkElement, but VS complains if you try to create
            // a margin without one.
            this.Visibility = Visibility.Collapsed;
        }

        public FrameworkElement VisualElement => this;

        public double MarginSize => this.ActualWidth;

        // No need to respond to options, we use the DeferCreation attribute on our provider
        // to ensure that we are only created when the error margin is enabled.
        public bool Enabled => true;

        public void Dispose()
        {
            if (!this.isDisposed)
            {

                if (this.errorMargin != null)
                {
                    this.errorTagAggregator.Dispose();
                }

                this.marginContainer.VisualElement.MouseLeave -= this.OnMouseLeave;
                this.marginContainer.VisualElement.MouseMove -= this.OnMouseMove;
                this.marginContainer.VisualElement.PreviewMouseLeftButtonDown -= this.OnLeftButtonDown;

                this.isDisposed = true;
            }
        }

        public ITextViewMargin GetTextViewMargin(string marginName)
            => string.Equals(nameof(ErrorInteractionMarginProvider), marginName) ? this : null;

        private void OnMouseLeave(object sender, MouseEventArgs e) => this.Reset();

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (this.TryGetErrorTagsAtLineUnderCursor(e, out var lineTags))
            {
                this.marginContainer.VisualElement.Cursor = Cursors.Hand;
                this.tooltip.Content = string.Join("\r\n", lineTags.Select(tag => tag.Tag.ToolTipContent.ToString()));
                this.tooltip.PlacementRectangle = new Rect(
                    e.GetPosition(this.marginContainer.VisualElement),
                    new Size(0, 0));
                this.tooltip.IsOpen = true;
            }
            else
            {
                this.Reset();
            }
        }

        private void OnLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.TryGetErrorTagsAtLineUnderCursor(e, out var lineTags))
            {
                // Left-most error span start or the start of the line.
                var leftMostErrorStart = lineTags.Select(tag => tag.Span.Start.GetPoint(this.wpfTextViewHost.TextView.TextBuffer, PositionAffinity.Predecessor))
                    .Where(start => (start != null))
                    .OrderBy(start => start)
                    .First().Value;

                this.wpfTextViewHost.TextView.Caret.MoveTo(leftMostErrorStart);
            }
        }

        private void Reset()
        {
            this.marginContainer.VisualElement.Cursor = Cursors.Arrow;
            this.tooltip.IsOpen = false;
        }

        private bool TryGetErrorTagsAtLineUnderCursor(MouseEventArgs e, out IEnumerable<IMappingTagSpan<IErrorTag>> tags)
        {
            if (this.TryEnsureInitialized())
            {
                var mousePosition = e.GetPosition(this.errorMargin.VisualElement);
                if ((mousePosition.X >= 0)
                    && (mousePosition.X <= this.errorMargin.MarginSize)
                    && this.marginContainer.VisualElement is IVerticalScrollBar scrollBar)
                {
                    var hoveredBufferPosition = scrollBar.GetBufferPositionOfYCoordinate(mousePosition.Y);
                    var hoveredSnapshotLineExtent = hoveredBufferPosition.GetContainingLine().Extent;
                    tags = this.errorTagAggregator.GetTags(hoveredSnapshotLineExtent);

                    return tags.Any();
                }
            }

            tags = Enumerable.Empty<IMappingTagSpan<IErrorTag>>();
            return false;
        }

        private bool TryEnsureInitialized()
        {
            if (this.errorMargin == null)
            {
                this.errorMargin = this.wpfTextViewHost.GetTextViewMargin(PredefinedMarginNames.OverviewError);
                if (this.errorMargin == null)
                {
                    Debug.Fail("Failed to find error margin");
                    return false;
                }

                this.errorTagAggregator = this.factory.TagAggregatorFactoryService.CreateTagAggregator<IErrorTag>(this.wpfTextViewHost.TextView.TextBuffer);
            }

            return true;
        }
    }
}
