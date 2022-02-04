﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ElementDefinitionToMatlabVariableRule.cs" company="RHEA System S.A.">
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

namespace DEHPMatlab.MappingRules
{
    using System.Collections.Generic;
    using System.Linq;

    using DEHPCommon.MappingEngine;
    using DEHPCommon.MappingRules.Core;

    using DEHPMatlab.ViewModel.Row;

    /// <summary>
    /// The <see cref="ElementDefinitionToMatlabVariableRule"/> is a <see cref="IMappingRule"/> for the <see cref="MappingEngine"/>
    /// That takes a <see cref="List{T}"/> of <see cref="MappedElementDefinitionRowViewModel"/>
    /// as input and outputs a collection of <see cref="MatlabWorkspaceRowViewModel"/>
    /// </summary>
    public class ElementDefinitionToMatlabVariableRule : MappingRule<List<MappedElementDefinitionRowViewModel>, List<MatlabWorkspaceRowViewModel>>
    {
        /// <summary>
        /// Transform a <see cref="List{T}"/> of <see cref="MappedElementDefinitionRowViewModel"/> into an <see cref="MatlabWorkspaceRowViewModel"/>
        /// </summary>
        /// <param name="input">The <see cref="List{T}"/> of <see cref="MappedElementDefinitionRowViewModel"/></param>
        /// <returns>A collection of <see cref="MatlabWorkspaceRowViewModel"/></returns>
        public override List<MatlabWorkspaceRowViewModel> Transform(List<MappedElementDefinitionRowViewModel> input)
        {
            return input.Select(x =>
                new MatlabWorkspaceRowViewModel(x.SelectedMatlabVariable.Name, x.Value)
                {
                    ParentName = x.SelectedMatlabVariable.ParentName,
                    SelectedActualFiniteState = x.SelectedState,
                    SelectedOption = x.SelectedOption,
                    SelectedParameter = x.SelectedParameter
                }).ToList();
        }
    }
}
