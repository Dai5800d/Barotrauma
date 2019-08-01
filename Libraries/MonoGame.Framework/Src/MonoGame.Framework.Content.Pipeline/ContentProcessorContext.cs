﻿// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Microsoft.Xna.Framework.Graphics;

namespace Microsoft.Xna.Framework.Content.Pipeline
{
    /// <summary>
    /// Provides access to custom processor parameters, methods for converting member data, and triggering nested builds.
    /// </summary>
    public abstract class ContentProcessorContext
    {
        /// <summary>
        /// Gets the name of the current content build configuration.
        /// </summary>
        public abstract string BuildConfiguration { get; }

        /// <summary>
        /// Gets the path of the directory that will contain any intermediate files generated by the content processor.
        /// </summary>
        public abstract string IntermediateDirectory { get; }

        /// <summary>
        /// Gets the logger interface used for status messages or warnings.
        /// </summary>
        public abstract ContentBuildLogger Logger { get; }

        /// <summary>
        /// Gets the ContentIdentity representing the source asset imported.
        /// </summary>
        public abstract ContentIdentity SourceIdentity { get; }

        /// <summary>
        /// Gets the output path of the content processor.
        /// </summary>
        public abstract string OutputDirectory { get; }

        /// <summary>
        /// Gets the output file name of the content processor.
        /// </summary>
        public abstract string OutputFilename { get; }

        /// <summary>
        /// Gets the collection of parameters used by the content processor.
        /// </summary>
        public abstract OpaqueDataDictionary Parameters { get; }

        /// <summary>
        /// Gets the current content build target platform.
        /// </summary>
        public abstract TargetPlatform TargetPlatform { get; }

        /// <summary>
        /// Gets the current content build target profile.
        /// </summary>
        public abstract GraphicsProfile TargetProfile { get; }

        /// <summary>
        /// Initializes a new instance of ContentProcessorContext.
        /// </summary>
        public ContentProcessorContext()
        {
        }

        /// <summary>
        /// Adds a dependency to the specified file. This causes a rebuild of the file, when modified, on subsequent incremental builds.
        /// </summary>
        /// <param name="filename">Name of an asset file.</param>
        public abstract void AddDependency(string filename);

        /// <summary>
        /// Add a file name to the list of related output files maintained by the build item. This allows tracking build items that build multiple output files.
        /// </summary>
        /// <param name="filename">The name of the file.</param>
        public abstract void AddOutputFile(string filename);

        /// <summary>
        /// Initiates a nested build of the specified asset and then loads the result into memory.
        /// </summary>
        /// <typeparam name="TInput">Type of the input.</typeparam>
        /// <typeparam name="TOutput">Type of the converted output.</typeparam>
        /// <param name="sourceAsset">Reference to the source asset.</param>
        /// <param name="processorName">Optional processor for this content.</param>
        /// <returns>Copy of the final converted content.</returns>
        /// <remarks>An example of usage would be a mesh processor calling BuildAndLoadAsset to build any associated textures and replace the original .tga file references with an embedded copy of the converted texture.</remarks>
        public TOutput BuildAndLoadAsset<TInput,TOutput>(
            ExternalReference<TInput> sourceAsset,
            string processorName
            )
        {
            return BuildAndLoadAsset<TInput, TOutput>(sourceAsset, processorName, null, null);
        }

        /// <summary>
        /// Initiates a nested build of the specified asset and then loads the result into memory.
        /// </summary>
        /// <typeparam name="TInput">Type of the input.</typeparam>
        /// <typeparam name="TOutput">Type of the converted output.</typeparam>
        /// <param name="sourceAsset">Reference to the source asset.</param>
        /// <param name="processorName">Optional processor for this content.</param>
        /// <param name="processorParameters">Optional collection of named values available as input to the content processor.</param>
        /// <param name="importerName">Optional importer for this content.</param>
        /// <returns>Copy of the final converted content.</returns>
        /// <remarks>An example of usage would be a mesh processor calling BuildAndLoadAsset to build any associated textures and replace the original .tga file references with an embedded copy of the converted texture.</remarks>
        public abstract TOutput BuildAndLoadAsset<TInput,TOutput>(
            ExternalReference<TInput> sourceAsset,
            string processorName,
            OpaqueDataDictionary processorParameters,
            string importerName
            );

        /// <summary>
        /// Initiates a nested build of an additional asset.
        /// </summary>
        /// <typeparam name="TInput">Type of the input.</typeparam>
        /// <typeparam name="TOutput">Type of the output.</typeparam>
        /// <param name="sourceAsset">Reference to the source asset.</param>
        /// <param name="processorName">Optional processor for this content.</param>
        /// <returns>Reference to the final compiled content. The build work is not required to complete before returning. Therefore, this file may not be up to date when BuildAsset returns but it will be available for loading by the game at runtime.</returns>
        /// <remarks>An example of usage for BuildAsset is being called by a mesh processor to request that any related textures used are also built, replacing the original TGA file references with new references to the converted texture files.</remarks>
        public ExternalReference<TOutput> BuildAsset<TInput,TOutput>(
            ExternalReference<TInput> sourceAsset,
            string processorName
            )
        {
            return BuildAsset<TInput, TOutput>(sourceAsset, processorName, null, null, null);
        }

        /// <summary>
        /// Initiates a nested build of an additional asset.
        /// </summary>
        /// <typeparam name="TInput">Type of the input.</typeparam>
        /// <typeparam name="TOutput">Type of the output.</typeparam>
        /// <param name="sourceAsset">Reference to the source asset.</param>
        /// <param name="processorName">Optional processor for this content.</param>
        /// <param name="processorParameters">Optional collection of named values available as input to the content processor.</param>
        /// <param name="importerName">Optional importer for this content.</param>
        /// <param name="assetName">Optional name of the final compiled content.</param>
        /// <returns>Reference to the final compiled content. The build work is not required to complete before returning. Therefore, this file may not be up to date when BuildAsset returns but it will be available for loading by the game at runtime.</returns>
        /// <remarks>An example of usage for BuildAsset is being called by a mesh processor to request that any related textures used are also built, replacing the original TGA file references with new references to the converted texture files.</remarks>
        public abstract ExternalReference<TOutput> BuildAsset<TInput,TOutput>(
            ExternalReference<TInput> sourceAsset,
            string processorName,
            OpaqueDataDictionary processorParameters,
            string importerName,
            string assetName
            );

        /// <summary>
        /// Converts a content item object using the specified content processor.
        /// </summary>
        /// <typeparam name="TInput">Type of the input content.</typeparam>
        /// <typeparam name="TOutput">Type of the converted output.</typeparam>
        /// <param name="input">Source content to be converted.</param>
        /// <param name="processorName">Optional processor for this content.</param>
        /// <returns>Reference of the final converted content.</returns>
        public TOutput Convert<TInput,TOutput>(
            TInput input,
            string processorName
            )
        {
            return Convert<TInput, TOutput>(input, processorName, new OpaqueDataDictionary());
        }

        /// <summary>
        /// Converts a content item object using the specified content processor.
        /// </summary>
        /// <typeparam name="TInput">Type of the input content.</typeparam>
        /// <typeparam name="TOutput">Type of the converted output.</typeparam>
        /// <param name="input">Source content to be converted.</param>
        /// <param name="processorName">Optional processor for this content.</param>
        /// <param name="processorParameters">Optional parameters for the processor.</param>
        /// <returns>Reference of the final converted content.</returns>
        public abstract TOutput Convert<TInput,TOutput>(
            TInput input,
            string processorName,
            OpaqueDataDictionary processorParameters
            );
    }
}