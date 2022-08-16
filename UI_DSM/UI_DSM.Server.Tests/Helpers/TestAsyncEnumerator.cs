// --------------------------------------------------------------------------------------------------------
// <copyright file="TestAsyncEnumerator.cs" company="RHEA System S.A.">
//  Copyright (c) 2022 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft
// 
//  This file is part of UI-DSM.
//  The UI-DSM web application is used to review an ECSS-E-TM-10-25 model.
// 
//  The UI-DSM application is provided to the community under the Apache License 2.0.
// </copyright>
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.Server.Tests.Helpers
{
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> inner;

        public TestAsyncEnumerator(IEnumerator<T> inner)
        {
            this.inner = inner;
        }

        /// <summary>Advances the enumerator asynchronously to the next element of the collection.</summary>
        /// <returns>
        ///     A <see cref="T:System.Threading.Tasks.ValueTask`1" /> that will complete with a result of
        ///     <see langword="true" /> if the enumerator was successfully advanced to the next element, or
        ///     <see langword="false" /> if the enumerator has passed the end of the collection.
        /// </returns>
        public ValueTask<bool> MoveNextAsync()
        {
            return new ValueTask<bool>(this.inner.MoveNext());
        }

        public T Current => this.inner.Current;

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources asynchronously.</summary>
        /// <returns>A task that represents the asynchronous dispose operation.</returns>
        public ValueTask DisposeAsync()
        {
            return new ValueTask(Task.FromResult(() => this.inner.Dispose()));
        }

        public void Dispose()
        {
            this.inner.Dispose();
        }

        public Task<bool> MoveNext(CancellationToken cancellationToken)
        {
            return Task.FromResult(this.inner.MoveNext());
        }
    }
}
