﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDstController.cs" company="RHEA System S.A.">
// Copyright (c) 2020-2022 RHEA System S.A.
// 
// Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Théate.
// 
// This file is part of DEHPMatlab
// 
// The DEHPMatlab is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 3 of the License, or (at your option) any later version.
// 
// The DEHPMatlab is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with this program; if not, write to the Free Software Foundation,
// Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace DEHPMatlab.DstController
{
    using DEHPMatlab.Services.MatlabConnector;
    using DEHPMatlab.ViewModel.Row;

    using ReactiveUI;

    /// <summary>
    /// Interface defintion for <see cref="DstController"/>
    /// </summary>
    public interface IDstController
    {
        /// <summary>
        /// Assert the <see cref="MatlabConnector"/> is connected to the application
        /// </summary>
        bool IsSessionOpen { get; set; }

        /// <summary>
        /// The name of the current loaded Matlab script
        /// </summary>
        string LoadedScriptName { get; set; }

        /// <summary>
        /// Gets or sets whether a script is loaded
        /// </summary>
        bool IsScriptLoaded { get; set; }

        /// <summary>
        /// Gets the collection of <see cref="MatlabWorkspaceInputRowViewModels"/> detected as Input
        /// </summary>
        ReactiveList<MatlabWorkspaceRowViewModel> MatlabWorkspaceInputRowViewModels { get; }

        /// <summary>
        /// Connects the adapter to a Matlab Instance
        /// </summary>
        void Connect();

        /// <summary>
        /// Disconnects the adapter to the Matlab Instance
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Load a Matlab Script
        /// </summary>
        /// <param name="scriptPath">The path of the script to load</param>
        void LoadScript(string scriptPath);

        /// <summary>
        /// Unload the Matlab Script
        /// </summary>
        void UnloadScript();

        /// <summary>
        /// Runs the currently loaded Matlab Script
        /// </summary>
        void RunMatlabScript();
    }
}
