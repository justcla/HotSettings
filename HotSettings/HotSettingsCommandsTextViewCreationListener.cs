using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Operations;
using System;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell.Settings;
using static HotSettings.Constants;
#pragma warning disable 0649

namespace HotSettings
{
    [Export(typeof(IVsTextViewCreationListener))]
    [ContentType("text")]
    [TextViewRole(PredefinedTextViewRoles.Editable)]
    internal sealed class HotSettingsCommandsTextViewCreationListener : IVsTextViewCreationListener
    {
        [Import]
        private IVsEditorAdaptersFactoryService EditorAdaptersFactoryService { get; set; }

        [Import]
        private IClassifierAggregatorService _aggregatorFactory;

        [Import]
        private SVsServiceProvider _globalServiceProvider;

        [Import(typeof(IEditorOperationsFactoryService))]
        private IEditorOperationsFactoryService _editorOperationsFactory;

        private IVsTextManager6 TextManager;
        IWpfTextView textView;
        HotSettingsCommandFilter commandFilter;

        private ShellSettingsManager SettingsManager;
        private WritableSettingsStore UserSettingsStore;


        public void VsTextViewCreated(IVsTextView textViewAdapter)
        {
            textView = EditorAdaptersFactoryService.GetWpfTextView(textViewAdapter);
            Guid langServiceGuid = GetLanguageServiceGuid(textView);

            TextManager = (IVsTextManager6)_globalServiceProvider.GetService(typeof(SVsTextManager));

            SettingsManager = new ShellSettingsManager(_globalServiceProvider);
            UserSettingsStore = SettingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);

            commandFilter = new HotSettingsCommandFilter(textView, langServiceGuid, TextManager, UserSettingsStore);
            textViewAdapter.AddCommandFilter(commandFilter, out IOleCommandTarget next);

            // Apply global settings to this editor window. ie. Sticky setting for Lightbulb margin
            ApplyInitialEditorMarginSettings();

            commandFilter.Next = next;
        }

        private Guid GetLanguageServiceGuid(IWpfTextView textView)
        {
            // Get the TextBuffer from the TextView.
            IVsTextBuffer textBuffer = EditorAdaptersFactoryService.GetBufferAdapter(textView.TextBuffer);
            // Handle NullReferenceException caused when Diff window (and others?) have no TextBuffer
            if (textBuffer == null) return Guid.Empty;

            // Fetch the language Guid
            textBuffer.GetLanguageServiceID(out Guid langServiceGuid);
            return langServiceGuid;
        }

        private void ApplyInitialEditorMarginSettings()
        {
            ApplyLightbulbMarginSetting();
        }

        private void ApplyLightbulbMarginSetting()
        {
            // Get the user's current sticky lightbulb margin setting
            // Note: First time fetch will be empty and should default to TRUE (ie. Show the lightbulb margin)
            bool showLightbulbMargin = UserSettingsStore.GetBoolean(HOT_SETTINGS_GROUP, SHOW_LIGHTBLUB_MARGIN, true);

            // Turn off the lightbulb if user set it OFF.
            // Note: Only worry about turning it OFF; it starts ON by default with a new editor.
            if (!showLightbulbMargin)
            {
                textView.Options.SetOptionValue("TextViewHost/SuggestionMargin", false);
            }
        }
    }
}
