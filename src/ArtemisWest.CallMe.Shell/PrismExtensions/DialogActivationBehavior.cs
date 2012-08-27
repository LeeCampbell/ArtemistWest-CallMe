using System;
using System.Collections.Specialized;
using System.Windows;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.Regions.Behaviors;

namespace ArtemisWest.CallMe.Shell.PrismExtensions
{
    /// <summary>
    /// Defines a behavior that creates a Dialog to display the active view of the target <see cref="IRegion"/>.
    /// </summary>
    public abstract class DialogActivationBehavior : RegionBehavior, IHostAwareRegionBehavior
    {
        /// <summary>
        /// The key of this behavior
        /// </summary>
        public const string BehaviorKey = "DialogActivation";

        private IWindow _contentDialog;

        /// <summary>
        /// Gets or sets the <see cref="DependencyObject"/> that the <see cref="IRegion"/> is attached to.
        /// </summary>
        /// <value>A <see cref="DependencyObject"/> that the <see cref="IRegion"/> is attached to.
        /// This is usually a <see cref="FrameworkElement"/> that is part of the tree.</value>
        public DependencyObject HostControl { get; set; }

        /// <summary>
        /// Performs the logic after the behavior has been attached.
        /// </summary>
        protected override void OnAttach()
        {
            this.Region.ActiveViews.CollectionChanged += this.ActiveViews_CollectionChanged;
        }

        /// <summary>
        /// Override this method to create an instance of the <see cref="IWindow"/> that 
        /// will be shown when a view is activated.
        /// </summary>
        /// <returns>
        /// An instance of <see cref="IWindow"/> that will be shown when a 
        /// view is activated on the target <see cref="IRegion"/>.
        /// </returns>
        protected abstract IWindow CreateWindow();

        private void ActiveViews_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    this.CloseContentDialog();
                    this.PrepareContentDialog(e.NewItems[0]);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    this.CloseContentDialog();
                    break;
                default:
                    //throw new NotSupportedException(string.Format("An action of {0} is not supported for the ActiveViews Collection of a dialog region", e.Action));
                    break;
            }
        }

        private Style GetStyleForView()
        {
            return this.HostControl.GetValue(RegionPopupBehaviors.ContainerWindowStyleProperty) as Style;
        }

        private void PrepareContentDialog(object view)
        {
            this._contentDialog = this.CreateWindow();
            this._contentDialog.Content = view;
            this._contentDialog.Owner = this.HostControl;
            this._contentDialog.Closed += this.ContentDialogClosed;
            this._contentDialog.Style = this.GetStyleForView();
            this._contentDialog.Show();
        }

        private void CloseContentDialog()
        {
            if (this._contentDialog != null)
            {
                this._contentDialog.Closed -= this.ContentDialogClosed;
                this._contentDialog.Close();
                this._contentDialog.Content = null;
                this._contentDialog.Owner = null;
            }
        }

        private void ContentDialogClosed(object sender, System.EventArgs e)
        {
            this.Region.Deactivate(this._contentDialog.Content);
            this.CloseContentDialog();
        }
    }
}