// --------------------------------------------------------------------------------------------------------
// <copyright file="ModelPage.razor.cs" company="RHEA System S.A.">
//  Copyright (c) 2023 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft, Martin Risseeuw, Nabil Abbar
// 
//  This file is part of UI-DSM.
//  The UI-DSM web application is used to review an ECSS-E-TM-10-25 model.
// 
//  The UI-DSM application is provided to the community under the Apache License 2.0.
// </copyright>
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.Client.Pages.NormalUser.ModelPage
{
    using CDP4Common.EngineeringModelData;

    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.WebUtilities;

    using ReactiveUI;

    using UI_DSM.Client.Components.App.AnnotationLinker;
    using UI_DSM.Client.Components.App.Comments;
    using UI_DSM.Client.Components.App.SelectedItemCard;
    using UI_DSM.Client.Components.NormalUser.Views;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views.RowViewModel;
    using UI_DSM.Client.ViewModels.Pages.NormalUser.ModelPage;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Page to view an <see cref="Iteration" />
    /// </summary>
    public partial class ModelPage: IDisposable
    {
        /// <summary>
        ///     The collection of <see cref="IDisposable" />
        /// </summary>
        private readonly List<IDisposable> disposables = new();

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.disposables.ForEach(x => x.Dispose());
        }

        /// <summary>
        ///     The collection of <see cref="IDisposable" /> linked to a <see cref="BaseView" />
        /// </summary>
        private readonly List<IDisposable> viewDisposables = new();

        /// <summary>
        ///     Value indicating if the current page is loading
        /// </summary>
        private bool isLoading;

        /// <summary>
        ///     The <see cref="Project" /> id
        /// </summary>
        [Parameter]
        public string ProjectId { get; set; }

        /// <summary>
        ///     The <see cref="Review" /> id
        /// </summary>
        [Parameter]
        public string ReviewId { get; set; }

        /// <summary>
        ///     The <see cref="Model" /> id
        /// </summary>
        [Parameter]
        public string ModelId { get; set; }

        /// <summary>
        ///     The selected <see cref="View" />
        /// </summary>
        [Parameter]
        [SupplyParameterFromQuery(Name = "view")]
        public string SelectedView { get; set; } = View.RequirementBreakdownStructureView.ToString();

        /// <summary>
        ///     The id of the selected item
        /// </summary>
        [Parameter]
        [SupplyParameterFromQuery(Name = "id")]
        public string Id { get; set; }

        /// <summary>
        ///     The <see cref="IModelPageViewModel" />
        /// </summary>
        [Inject]
        public IModelPageViewModel ViewModel { get; set; }

        /// <summary>
        ///     The <see cref="NavigationManager" />
        /// </summary>
        [Inject]
        public NavigationManager NavigationManager { get; set; }

        /// <summary>
        ///     Reference to the <see cref="DynamicComponent" />
        /// </summary>
        public DynamicComponent BaseView { get; set; }

        /// <summary>
        ///     The <see cref="SelectedItemCard" />
        /// </summary>
        public SelectedItemCard SelectedItemCard { get; set; }

        /// <summary>
        ///     The <see cref="Comments" />
        /// </summary>
        public Comments Comments { get; set; }

        /// <summary>
        ///     The currently selected item in the BaseView
        /// </summary>
        public object SelectedItem { get; set; }

        /// <summary>
        ///     Method invoked when the component is ready to start, having received its
        ///     initial parameters from its parent in the render tree.
        /// </summary>
        protected override void OnInitialized()
        {
            this.isLoading = true;

            this.disposables.Add(this.WhenAnyValue(x => x.ViewModel.IsLinkerVisible)
                .Subscribe(async x => await this.OnLinkerVisibilityChanged(x)));

            base.OnInitialized();
        }

        /// <summary>
        ///     Handle the change of visible of the <see cref="AnnotationLinker" /> component
        /// </summary>
        /// <param name="value">The new visibility</param>
        /// <returns>A <see cref="Task" /></returns>
        private async Task OnLinkerVisibilityChanged(bool value)
        {
            if (!value && this.ViewModel.CurrentBaseViewInstance != null)
            {
                await this.ViewModel.CurrentBaseViewInstance.HasChanged();
                this.Comments.ViewModel.SelectedItem = null;
                this.Comments.ViewModel.SelectedItem = this.SelectedItem;
            }

            await this.InvokeAsync(this.StateHasChanged);
        }

        /// <summary>
        ///     Method invoked when the component has received parameters from its parent in
        ///     the render tree, and the incoming values have been assigned to properties.
        /// </summary>
        /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> representing any asynchronous operation.</returns>
        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            this.isLoading = true;
            this.BaseView = new DynamicComponent();

            if (Enum.TryParse(typeof(View), this.SelectedView, out var view) && view is View selectedView)
            {
                var projectId = Guid.Parse(this.ProjectId);
                var modelId = Guid.Parse(this.ModelId);
                var reviewId = Guid.Parse(this.ReviewId);
                await this.ViewModel.InitializeProperties(projectId, reviewId, modelId, selectedView);
                await this.InvokeAsync(this.StateHasChanged);
            }

            this.isLoading = false;
        }

        /// <summary>
        ///     Method invoked after each time the component has been rendered. Note that the component does
        ///     not automatically re-render after the completion of any returned <see cref="T:System.Threading.Tasks.Task" />, because
        ///     that would cause an infinite render loop.
        /// </summary>
        /// <param name="firstRender">
        ///     Set to <c>true</c> if this is the first time
        ///     <see cref="M:Microsoft.AspNetCore.Components.ComponentBase.OnAfterRender(System.Boolean)" /> has been invoked
        ///     on this component instance; otherwise <c>false</c>.
        /// </param>
        /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> representing any asynchronous operation.</returns>
        /// <remarks>
        ///     The <see cref="M:Microsoft.AspNetCore.Components.ComponentBase.OnAfterRender(System.Boolean)" /> and
        ///     <see cref="M:Microsoft.AspNetCore.Components.ComponentBase.OnAfterRenderAsync(System.Boolean)" /> lifecycle methods
        ///     are useful for performing interop, or interacting with values received from <c>@ref</c>.
        ///     Use the <paramref name="firstRender" /> parameter to ensure that initialization work is only performed
        ///     once.
        /// </remarks>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (this.BaseView?.Instance is BaseView baseView && this.ViewModel.ShouldInitializeBaseView)
            {
                this.ViewModel.ShouldInitializeBaseView = false;
                this.ViewModel.CurrentBaseViewInstance = baseView;
                this.DisposeViewDisposables();
                var projectId = new Guid(this.ProjectId);
                var reviewId = new Guid(this.ReviewId);

                await baseView.InitializeViewModel(this.ViewModel.Things, projectId, reviewId, 
                    Guid.Empty, new List<string>(), new List<string>(), null);

                await this.Comments.InitializesProperties(projectId, reviewId, this.ViewModel.CurrentView,
                    this.ViewModel.Participant, this.ViewModel.OnLinkCallback, null);

                if (Guid.TryParse(this.Id, out var itemId))
                {
                    var existingItem = baseView.GetAvailablesRows().OfType<IHaveThingRowViewModel>()
                        .FirstOrDefault(x => x.ThingId == itemId) ?? baseView.GetAvailablesRows().OfType<ElementBaseRowViewModel>()
                        .FirstOrDefault(x => x.ElementDefinitionId == itemId);

                    if (existingItem != null)
                    {
                        baseView.TrySetSelectedItem(existingItem);
                        await baseView.TryNavigateToItem(existingItem.Id);
                    }
                }

                this.viewDisposables.Add(this.Comments.ViewModel.Comments.CountChanged
                    .Subscribe(async _ => await OnCommentsCountChanged(baseView)));

                this.viewDisposables.Add(baseView.SelectedItemObservable.Subscribe(async x => await this.OnSelectedItemChanged(x)));
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        /// <summary>
        ///     Handle the count Changed event on the Comments collection
        /// </summary>
        /// <param name="baseView">The <see cref="BaseView" /></param>
        /// <returns>A <see cref="Task" /></returns>
        private static async Task OnCommentsCountChanged(BaseView baseView)
        {
            await baseView.HasChanged();
        }

        /// <summary>
        ///     Handle the changed of the baseView Selected Item
        /// </summary>
        /// <param name="newSelectedItem">The new selected Item</param>
        /// <returns>A <see cref="Task" /></returns>
        private async Task OnSelectedItemChanged(object newSelectedItem)
        {
            if (this.SelectedItemCard == null)
            {
                return;
            }

            this.SelectedItem = newSelectedItem;
            this.SelectedItemCard.ViewModel.SelectedItem = this.SelectedItem;
            this.Comments.ViewModel.SelectedItem = this.SelectedItem;

            if (this.ViewModel?.CurrentBaseViewInstance != null)
            {
                this.Comments.ViewModel.AvailableRows = this.ViewModel?.CurrentBaseViewInstance.GetAvailablesRows();
            }

            await this.InvokeAsync(this.StateHasChanged);
        }

        /// <summary>
        ///     Tries to navigate to a corresponding item
        /// </summary>
        /// <param name="itemName">The name of the item to navigate to</param>
        /// <returns>A <see cref="Task" /></returns>
        private Task TryNavigateToItem(string itemName)
        {
            return this.ViewModel.CurrentBaseViewInstance != null ? this.ViewModel.CurrentBaseViewInstance.TryNavigateToItem(itemName) : Task.CompletedTask;
        }

        /// <summary>
        ///     Dispose all <see cref="IDisposable" /> linked to the current view
        /// </summary>
        private void DisposeViewDisposables()
        {
            this.viewDisposables.ForEach(x => x.Dispose());
            this.viewDisposables.Clear();
        }

        /// <summary>
        /// Navigates to the selected View
        /// </summary>
        /// <param name="view">The selected View</param>
        private void OnViewSelect(View view)
        {
            var parameters = new Dictionary<string, string> { ["view"]= view.ToString() };

            if (this.SelectedItem is IHaveThingRowViewModel row)
            {
                parameters["id"] = row.ThingId.ToString();
            }

            var url = new Uri(this.NavigationManager.Uri).GetLeftPart(UriPartial.Path);
            this.NavigationManager.NavigateTo(QueryHelpers.AddQueryString(url, parameters));
        }
    }
}
