﻿using Kephas.Services;

namespace Kephas.Model.Configuration
{
    /// <summary>
    /// Contract for model element configurators.
    /// </summary>
    public interface IElementConfigurator
    {
        /// <summary>
        /// Provides the model element to be configured.
        /// </summary>
        /// <param name="modelElement">The model element.</param>
        /// <returns>A configurator for the provided model element.</returns>
        IElementConfigurator With(IModelElement modelElement);

        /// <summary>
        /// Configures the model element provided with the.
        /// </summary>
        void Configure();
    }

    /// <summary>
    /// Contract for model element configurators for a specific native type.
    /// </summary>
    /// <typeparam name="TNativeElement">The type of the native element.</typeparam>
    /// <typeparam name="TRealConfigurator">The type of the real configurator.</typeparam>
    [SharedAppServiceContract(AllowMultiple = true, ContractType = typeof(IElementConfigurator),
        MetadataAttributes = new []{ typeof(ProcessingPriorityAttribute) })]
    public interface IElementConfigurator<TNativeElement, TRealConfigurator>
    {
    }
}