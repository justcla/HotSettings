//------------------------------------------------------------------------------
// <copyright file="SettingsMargin.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Shell;
using System.Windows.Input;
using Microsoft.VisualStudio.Shell.Interop;

namespace HotSettings
{
    /// <summary>
    /// Margin's canvas and visual definition including both size and content
    /// </summary>
    internal class SettingsMargin : Canvas, IWpfTextViewMargin
    {
        /// <summary>
        /// Margin name.
        /// </summary>
        public const string MarginName = "SettingsMargin";

        /// <summary>
        /// A value indicating whether the object is disposed.
        /// </summary>
        private bool isDisposed;
        private SVsServiceProvider serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsMargin"/> class for a given <paramref name="textView"/>.
        /// </summary>
        /// <param name="textView">The <see cref="IWpfTextView"/> to attach the margin to.</param>
        public SettingsMargin(IWpfTextView textView, SVsServiceProvider sp)
        {
            this.serviceProvider = sp;
            this.Width = 10; // Margin height sufficient to have the label
            this.ClipToBounds = true;
            this.Background = new SolidColorBrush(Colors.LightCyan);

            // Add a green colored label that says "Hello SettingsMargin"
            var label = new Label
            {
                Background = new SolidColorBrush(Colors.LightGreen),
                Content = "Hello SettingsMargin",
            };

            //this.Children.Add(label);
        }

        #region IWpfTextViewMargin

        /// <summary>
        /// Gets the <see cref="Sytem.Windows.FrameworkElement"/> that implements the visual representation of the margin.
        /// </summary>
        /// <exception cref="ObjectDisposedException">The margin is disposed.</exception>
        public FrameworkElement VisualElement
        {
            // Since this margin implements Canvas, this is the object which renders
            // the margin.
            get
            {
                this.ThrowIfDisposed();
                return this;
            }
        }

        #endregion

        #region ITextViewMargin

        /// <summary>
        /// Gets the size of the margin.
        /// </summary>
        /// <remarks>
        /// For a horizontal margin this is the height of the margin,
        /// since the width will be determined by the <see cref="ITextView"/>.
        /// For a vertical margin this is the width of the margin,
        /// since the height will be determined by the <see cref="ITextView"/>.
        /// </remarks>
        /// <exception cref="ObjectDisposedException">The margin is disposed.</exception>
        public double MarginSize
        {
            get
            {
                this.ThrowIfDisposed();

                // Since this is a vertical margin, its height will be bound to the height of the text view.
                // Therefore, its size is its width.
                return this.ActualWidth;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the margin is enabled.
        /// </summary>
        /// <exception cref="ObjectDisposedException">The margin is disposed.</exception>
        public bool Enabled
        {
            get
            {
                this.ThrowIfDisposed();

                // The margin should always be enabled
                return true;
            }
        }

        /// <summary>
        /// Gets the <see cref="ITextViewMargin"/> with the given <paramref name="marginName"/> or null if no match is found
        /// </summary>
        /// <param name="marginName">The name of the <see cref="ITextViewMargin"/></param>
        /// <returns>The <see cref="ITextViewMargin"/> named <paramref name="marginName"/>, or null if no match is found.</returns>
        /// <remarks>
        /// A margin returns itself if it is passed its own name. If the name does not match and it is a container margin, it
        /// forwards the call to its children. Margin name comparisons are case-insensitive.
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="marginName"/> is null.</exception>
        public ITextViewMargin GetTextViewMargin(string marginName)
        {
            return string.Equals(marginName, SettingsMargin.MarginName, StringComparison.OrdinalIgnoreCase) ? this : null;
        }

        /// <summary>
        /// Disposes an instance of <see cref="SettingsMargin"/> class.
        /// </summary>
        public void Dispose()
        {
            if (!this.isDisposed)
            {
                GC.SuppressFinalize(this);
                this.isDisposed = true;
            }
        }

        #endregion

        /// <summary>
        /// Checks and throws <see cref="ObjectDisposedException"/> if the object is disposed.
        /// </summary>
        private void ThrowIfDisposed()
        {
            if (this.isDisposed)
            {
                throw new ObjectDisposedException(MarginName);
            }
        }

        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonUp(e);

            ShowContextMenu(e);
        }

        private void ShowContextMenu(MouseButtonEventArgs e)
        {
            const string guidVSPackageContextMenuCmdSet = "c75f116c-9249-4984-8d82-d3c6025afb17";
            const int MyContextMenuId = 0x1100;

            IVsUIShell uiShell;
            System.Guid contextMenuGuid = new System.Guid(guidVSPackageContextMenuCmdSet);
            System.Windows.Point relativePoint;
            System.Windows.Point screenPoint;
            POINTS point;
            POINTS[] points;

            uiShell = serviceProvider.GetService(typeof(SVsUIShell)) as IVsUIShell;

            if (uiShell != null)
            {
                relativePoint = e.GetPosition(this);
                screenPoint = this.PointToScreen(relativePoint);

                point = new POINTS();
                point.x = (short)screenPoint.X;
                point.y = (short)screenPoint.Y;

                points = new[] { point };

                // TODO: error handling
                uiShell.ShowContextMenu(0, ref contextMenuGuid, MyContextMenuId, points, null);
            }
        }

    }
}
