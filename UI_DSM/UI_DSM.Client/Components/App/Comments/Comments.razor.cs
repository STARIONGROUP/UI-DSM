// --------------------------------------------------------------------------------------------------------
// <copyright file="Comments.razor.cs" company="RHEA System S.A.">
//  Copyright (c) 2022 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft, Martin Risseeuw, Nabil Abbar
// 
//  This file is part of UI-DSM.
//  The UI-DSM web application is used to review an ECSS-E-TM-10-25 model.
// 
//  The UI-DSM application is provided to the community under the Apache License 2.0.
// </copyright>
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.Client.Components.App.Comments
{
    using System.Reactive.Linq;

    using Microsoft.AspNetCore.Components;

    using ReactiveUI;

    using UI_DSM.Client.Components.App.CommentCard;
    using UI_DSM.Client.ViewModels.App.CommentCard;
    using UI_DSM.Client.ViewModels.App.Comments;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     These component display <see cref="Comment" />s and provide capabilities to add/update <see cref="Comment" />s
    /// </summary>
    public partial class Comments : IDisposable
    {
        /// <summary>
        ///     A collection of <see cref="IDisposable" />
        /// </summary>
        private readonly List<IDisposable> disposables = new();

        /// <summary>
        ///     The currently selected <see cref="CommentCard" />
        /// </summary>
        private CommentCard selectedCommentCard;

        /// <summary>
        ///     The <see cref="ICommentsViewModel" />
        /// </summary>
        [Inject]
        public ICommentsViewModel ViewModel { get; set; }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.disposables.ForEach(x => x.Dispose());
        }

        /// <summary>
        ///     Method invoked when the component is ready to start, having received its
        ///     initial parameters from its parent in the render tree.
        /// </summary>
        protected override void OnInitialized()
        {
            this.disposables.Add(this.ViewModel.Comments.CountChanged.Subscribe(_ => this.InvokeAsync(this.StateHasChanged)));
            this.disposables.Add(this.ViewModel);
            this.disposables.Add(this.WhenAnyValue(x => x.ViewModel.IsOnCommentCreationMode).Subscribe(_ => this.InvokeAsync(this.StateHasChanged)));
            this.disposables.Add(this.WhenAnyValue(x => x.ViewModel.IsOnCommentUpdateMode).Subscribe(_ => this.InvokeAsync(this.StateHasChanged)));
            this.disposables.Add(this.WhenAnyValue(x => x.ViewModel.IsOnReplyCreationMode).Subscribe(_ => this.InvokeAsync(this.StateHasChanged)));
            this.disposables.Add(this.WhenAnyValue(x => x.ViewModel.IsOnReplyUpdateMode).Subscribe(_ => this.InvokeAsync(this.StateHasChanged)));
            
            this.disposables.Add(this.WhenAnyValue(x => x.ViewModel.ReplyConfirmCancelPopupViewModel.IsVisible)
                .Where(x => !x).Subscribe(_ => this.InvokeAsync(this.StateHasChanged)));

            this.disposables.Add(this.ViewModel.ErrorMessageViewModel.Errors.CountChanged.Subscribe(_ => this.InvokeAsync(this.StateHasChanged)));
            base.OnInitialized();
        }

        /// <summary>
        ///     Initializes a new <see cref="ICommentCardViewModel" />
        /// </summary>
        /// <param name="comment">The <see cref="Comment" /></param>
        /// <returns>The initialized <see cref="ICommentCardViewModel" /></returns>
        private ICommentCardViewModel CreateCommentCardViewModel(Comment comment)
        {
            return new CommentCardViewModel(comment, this.ViewModel.Participant, this.ViewModel.OnCommentEditCallback, this.ViewModel.OnDeleteCallback, 
                this.ViewModel.OnUpdateStatusCallback, this.ViewModel.OnReplyCallback, this.ViewModel.OnReplyEditContentCallback,
                this.ViewModel.OnDeleteReplyCallback);
        }

        /// <summary>
        ///     Handle the selection of a <see cref="CommentCard" />
        /// </summary>
        /// <param name="commentCard">The new selected <see cref="CommentCard" /></param>
        private void OnCommentCardClick(CommentCard commentCard)
        {
            this.selectedCommentCard?.Deselect();
            this.selectedCommentCard = commentCard;
            this.selectedCommentCard.Select();
            this.ViewModel.SetCurrentComment(this.selectedCommentCard.ViewModel.Comment);
        }
    }
}
