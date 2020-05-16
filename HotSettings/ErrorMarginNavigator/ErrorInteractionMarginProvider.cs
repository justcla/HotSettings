namespace HotSettings.ErrorMarginNavigator
{
    using System;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;

    [Export(typeof(IWpfTextViewMarginProvider))]
    [Name(nameof(ErrorInteractionMarginProvider))]
    [Order(After = PredefinedMarginNames.OverviewError)]
    [ContentType("any")]
    [MarginContainer(PredefinedMarginNames.VerticalScrollBar)]
    [TextViewRole(PredefinedTextViewRoles.Interactive)]
    [DeferCreation(OptionName = DefaultTextViewHostOptions.ShowErrorsOptionName)]
    internal sealed class ErrorInteractionMarginProvider : IWpfTextViewMarginProvider
    {
        internal readonly IBufferTagAggregatorFactoryService TagAggregatorFactoryService;

        [ImportingConstructor]
        public ErrorInteractionMarginProvider(IBufferTagAggregatorFactoryService tagAggregatorFactoryService)
        {
            this.TagAggregatorFactoryService = tagAggregatorFactoryService
                ?? throw new ArgumentNullException(nameof(tagAggregatorFactoryService));
        }

        public IWpfTextViewMargin CreateMargin(IWpfTextViewHost wpfTextViewHost, IWpfTextViewMargin marginContainer)
        {
            return new ErrorInteractionMargin(wpfTextViewHost, marginContainer, this);
        }
    }
}
