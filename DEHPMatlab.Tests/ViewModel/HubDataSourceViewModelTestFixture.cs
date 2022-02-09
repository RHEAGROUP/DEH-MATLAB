﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HubDataSourceViewModelTestFixture.cs" company="RHEA System S.A.">
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

namespace DEHPMatlab.Tests.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Concurrency;

    using Autofac;

    using CDP4Common.CommonData;
    using CDP4Common.EngineeringModelData;
    using CDP4Common.SiteDirectoryData;

    using CDP4Dal;
    using CDP4Dal.Permission;

    using DEHPCommon;
    using DEHPCommon.HubController.Interfaces;
    using DEHPCommon.Services.NavigationService;
    using DEHPCommon.UserInterfaces.ViewModels;
    using DEHPCommon.UserInterfaces.ViewModels.Interfaces;
    using DEHPCommon.UserInterfaces.ViewModels.PublicationBrowser;
    using DEHPCommon.UserInterfaces.ViewModels.Rows.ElementDefinitionTreeRows;
    using DEHPCommon.UserInterfaces.Views;

    using DEHPMatlab.DstController;
    using DEHPMatlab.ViewModel;
    using DEHPMatlab.ViewModel.Dialogs.Interfaces;
    using DEHPMatlab.Views.Dialogs;

    using Moq;

    using NUnit.Framework;

    using ReactiveUI;

    [TestFixture]
    public class HubDataSourceViewModelTestFixture
    {
        private HubDataSourceViewModel viewModel;
        private Mock<INavigationService> navigationService;
        private Mock<IHubController> hubController;
        private Mock<IHubSessionControlViewModel> sessionControl;
        private Mock<IHubBrowserHeaderViewModel> hubBrowserHeader;
        private Mock<IPublicationBrowserViewModel> publicationBrowser;
        private Mock<IObjectBrowserViewModel> objectBrowser;
        private Mock<IStatusBarControlViewModel> statusBar;
        private Mock<ISession> session;
        private Mock<IDstController> dstController;
        private DomainOfExpertise domain;
        private Person person;
        private Participant participant;
        private Iteration iteration;

        [SetUp]
        public void Setup()
        {
            RxApp.MainThreadScheduler = Scheduler.CurrentThread;
            this.navigationService = new Mock<INavigationService>();
            this.navigationService.Setup(x => x.ShowDialog<Login>());

            this.hubController = new Mock<IHubController>();
            this.hubController.Setup(x => x.IsSessionOpen).Returns(false);
            this.hubController.Setup(x => x.Close());

            this.session = new Mock<ISession>();

            Mock<IPermissionService> permissionService = new Mock<IPermissionService>();
            permissionService.Setup(x => x.Session).Returns(this.session.Object);
            permissionService.Setup(x => x.CanRead(It.IsAny<Thing>())).Returns(true);
            permissionService.Setup(x => x.CanWrite(It.IsAny<Thing>())).Returns(true);

            this.dstController = new Mock<IDstController>();

            this.session.Setup(x => x.PermissionService).Returns(permissionService.Object);

            this.domain = new DomainOfExpertise(Guid.NewGuid(), null, null)
            {
                Name = "t",
                ShortName = "e"
            };

            this.person = new Person(Guid.NewGuid(), null, null) { GivenName = "test", DefaultDomain = this.domain };

            this.session.Setup(x => x.ActivePerson).Returns(this.person);

            this.participant = new Participant(Guid.NewGuid(), null, null)
            {
                Person = this.person
            };

            var engineeringModelSetup = new EngineeringModelSetup(Guid.NewGuid(), null, null)
            {
                Participant = { this.participant },
                Name = "est"
            };

            this.iteration = new Iteration(Guid.NewGuid(), null, null)
            {
                IterationSetup = new IterationSetup(Guid.NewGuid(), null, null)
                {
                    IterationNumber = 23,
                    Container = engineeringModelSetup
                },
                Container = new EngineeringModel(Guid.NewGuid(), null, null)
                {
                    EngineeringModelSetup = engineeringModelSetup
                }
            };

            this.session.Setup(x => x.OpenIterations).Returns(
                new Dictionary<Iteration, Tuple<DomainOfExpertise, Participant>>()
                {
                    {
                        this.iteration,
                        new Tuple<DomainOfExpertise, Participant>(this.domain, this.participant)
                    }
                });

            this.hubController.Setup(x => x.Session).Returns(this.session.Object);

            this.objectBrowser = new Mock<IObjectBrowserViewModel>();
            this.objectBrowser.Setup(x => x.CanMap).Returns(new Mock<IObservable<bool>>().Object);
            this.objectBrowser.Setup(x => x.MapCommand).Returns(ReactiveCommand.Create());
            this.objectBrowser.Setup(x => x.Things).Returns(new ReactiveList<BrowserViewModelBase>());
            this.objectBrowser.Setup(x => x.SelectedThings).Returns(new ReactiveList<object>());

            this.publicationBrowser = new Mock<IPublicationBrowserViewModel>();
            this.hubBrowserHeader = new Mock<IHubBrowserHeaderViewModel>();
            this.sessionControl = new Mock<IHubSessionControlViewModel>();
            this.statusBar = new Mock<IStatusBarControlViewModel>();

            this.viewModel = new HubDataSourceViewModel(this.navigationService.Object, this.hubController.Object,
                this.sessionControl.Object, this.hubBrowserHeader.Object, this.publicationBrowser.Object,
                this.objectBrowser.Object, this.statusBar.Object, this.dstController.Object);
        }

        [Test]
        public void VerifyProperties()
        {
            Assert.IsNotNull(this.viewModel.SessionControl);
            Assert.IsNotNull(this.viewModel.ObjectBrowser);
            Assert.IsNotNull(this.viewModel.HubBrowserHeader);
            Assert.IsNotNull(this.viewModel.ConnectCommand);
            Assert.IsNotNull(this.viewModel.PublicationBrowser);
            Assert.IsNotNull(this.viewModel.StatusBar);
            Assert.AreEqual("Connect",this.viewModel.ConnectButtonText);
        }

        [Test]
        public void VerifyConnectCommand()
        {
            Assert.IsTrue(this.viewModel.ConnectCommand.CanExecute(null));
            this.hubController.Setup(x => x.IsSessionOpen).Returns(true);
            this.viewModel.ConnectCommand.Execute(null);
            this.hubController.Setup(x => x.IsSessionOpen).Returns(false);
            this.hubController.Setup(x => x.OpenIteration).Returns((Iteration)null);
            this.viewModel.ConnectCommand.Execute(null);
            this.hubController.Verify(x => x.Close(), Times.Once);
            this.navigationService.Verify(x => x.ShowDialog<Login>(), Times.Once);
        }

        [Test]
        public void TestMapCommandExecute()
        {
            var dialog = new Mock<IHubMappingConfigurationDialogViewModel>();

            dialog.Setup(x => x.Elements)
                .Returns(new ReactiveList<ElementDefinitionRowViewModel>());

            var container = new ContainerBuilder();
            container.RegisterInstance(dialog.Object).As<IHubMappingConfigurationDialogViewModel>();

            AppContainer.Container = container.Build();

            this.objectBrowser.Setup(x => x.SelectedThings)
                .Returns(new ReactiveList<object>());

            var elementDefinition = new ElementDefinition(Guid.NewGuid(), null, new Uri("t://s.t"))
            {
                Parameter =
                {
                    new Parameter(Guid.NewGuid(), null, new Uri("t://s.t"))
                    {
                        ParameterType = new DateTimeParameterType(Guid.NewGuid(), null, new Uri("t://s.t")),
                        ValueSet = { new ParameterValueSet(Guid.NewGuid(), null, new Uri("t://s.t")) }
                    }
                }
            };

            this.iteration.Element.Add(elementDefinition);

            var elementRow = new ElementDefinitionRowViewModel(elementDefinition,
                new DomainOfExpertise(), this.session.Object, null);

            this.viewModel.ObjectBrowser.SelectedThings.Add(
                elementRow);

            Assert.IsTrue(this.viewModel.ObjectBrowser.MapCommand.CanExecute(null));
            Assert.DoesNotThrow(() => this.viewModel.ObjectBrowser.MapCommand.Execute(null));

            this.navigationService.Verify(x =>
                    x.ShowDialog<HubMappingConfigurationDialog, IHubMappingConfigurationDialogViewModel>(It.IsAny<IHubMappingConfigurationDialogViewModel>()),
                Times.Once);
        }
    }
}
