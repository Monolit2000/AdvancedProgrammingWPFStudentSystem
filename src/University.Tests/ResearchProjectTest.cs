using Castle.DynamicProxy.Generators;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using University.Data;
using University.Interfaces;
using University.Models;
using University.Services;
using University.ViewModels;

namespace University.Tests
{
    [TestClass]
    public class ResearchProjectTests
    {
        private IDialogService _dialogService;
        private DbContextOptions<UniversityContext> _options;

        [TestInitialize()]
        public void Initialize()
        {
            _options = new DbContextOptionsBuilder<UniversityContext>()
                .UseInMemoryDatabase(databaseName: "UniversityTestDB")
                .Options;
            SeedTestDB();
            _dialogService = new DialogService();
        }

        private void SeedTestDB()
        {
            using UniversityContext context = new UniversityContext(_options);
            {
                context.Database.EnsureDeleted();

                List<ResearchProject> researchProjects = new List<ResearchProject>
                {
                    new ResearchProject
                    {
                        ResearchProjectId = 1,
                        Title = "Example Research Project 1",
                        Description = "This is an example research project description 1.",
                        Supervisor = "Dr. Smith",
                        StartDate = new DateTime(2024, 5, 1),
                        EndDate = new DateTime(2025, 5, 1),
                        Budget = 10000.0f
                    },
                    new ResearchProject
                    {
                        ResearchProjectId = 2,
                        Title = "Example Research Project 2",
                        Description = "This is an example research project description 2.",
                        Supervisor = "Dr. Johnson",
                        StartDate = new DateTime(2024, 6, 1),
                        EndDate = new DateTime(2025, 6, 1),
                        Budget = 15000.0f
                    },
                    new ResearchProject
                    {
                        ResearchProjectId = 3,
                        Title = "Example Research Project 3",
                        Description = "This is an example research project description 3.",
                        Supervisor = "Dr. Lee",
                        StartDate = new DateTime(2024, 7, 1),
                        EndDate = new DateTime(2025, 7, 1),
                        Budget = 12000.0f
                    }
                };


                context.ResearchProjects.AddRange(researchProjects);
                context.SaveChanges();
            }
        }

        #region Add Tests

        [TestMethod]
        public void Show_all_research_projects()
        {
            using UniversityContext context = new UniversityContext(_options);
            {
                ResearchProjectViewModel researchProjectsViewModel = new ResearchProjectViewModel(context, _dialogService);
                bool hasData = researchProjectsViewModel.Projects.Any();
                Assert.IsTrue(hasData);
            }
        }

        [TestMethod]
        public void Add_research_project_with_valid_data()
        {
            using UniversityContext context = new UniversityContext(_options);
            {
                AddResearchProjectViewModel addResearchProjectViewModel = new AddResearchProjectViewModel(context, _dialogService)
                {
                    Title = "Project without Supervisor",
                    Description = "Description without title",
                    Supervisor = "Dr. NoTitle",
                    StartDate = new DateTime(2024, 8, 1),
                    EndDate = new DateTime(2025, 8, 1),
                    Budget = 30000.0f
                };
                addResearchProjectViewModel.Save.Execute(null);

                bool newResearchProjectExists = context.ResearchProjects.Any(rp => rp.Title == "Project without Supervisor" && rp.Supervisor == "Dr. NoTitle");
                Assert.IsTrue(newResearchProjectExists);
            }
        }

        [TestMethod]
        public void Add_research_project_without_title()
        {
            using UniversityContext context = new UniversityContext(_options);
            {
                AddResearchProjectViewModel addResearchProjectViewModel = new AddResearchProjectViewModel(context, _dialogService)
                {
                    Description = "Description without title",
                    Supervisor = "Dr. NoTitle",
                    StartDate = new DateTime(2024, 8, 1),
                    EndDate = new DateTime(2025, 8, 1),
                    Budget = 30000.0f
                };
                addResearchProjectViewModel.Save.Execute(null);

                bool newResearchProjectExists = context.ResearchProjects.Any(rp => rp.Description == "Description without title" && rp.Supervisor == "Dr. NoTitle");
                Assert.IsFalse(newResearchProjectExists);
            }
        }

        [TestMethod]
        public void Add_research_project_without_supervisor()
        {
            using UniversityContext context = new UniversityContext(_options);
            {
                AddResearchProjectViewModel addResearchProjectViewModel = new AddResearchProjectViewModel(context, _dialogService)
                {
                    Title = "Project without Supervisor",
                    Description = "Description for project without supervisor",
                    StartDate = new DateTime(2024, 8, 1),
                    EndDate = new DateTime(2025, 8, 1),
                    Budget = 35000.0f
                };
                addResearchProjectViewModel.Save.Execute(null);

                bool newResearchProjectExists = context.ResearchProjects.Any(rp => rp.Title == "Project without Supervisor" && rp.Description == "Description for project without supervisor");
                Assert.IsTrue(newResearchProjectExists);
            }
        }

        [TestMethod]
        public void Add_research_project_without_description()
        {
            using UniversityContext context = new UniversityContext(_options);
            {
                AddResearchProjectViewModel addResearchProjectViewModel = new AddResearchProjectViewModel(context, _dialogService)
                {
                    Title = "Project without Description",
                    Supervisor = "Dr. NoDescription",
                    StartDate = new DateTime(2024, 8, 1),
                    EndDate = new DateTime(2025, 8, 1),
                    Budget = 40000.0f
                };
                addResearchProjectViewModel.Save.Execute(null);

                bool newResearchProjectExists = context.ResearchProjects.Any(rp => rp.Title == "Project without Description" && rp.Supervisor == "Dr. NoDescription");
                Assert.IsFalse(newResearchProjectExists);
            }
        }

        #endregion

        #region Edit Tests
        [TestMethod]
        public void Edit_research_project_with_valid_data()
        {
            using UniversityContext context = new UniversityContext(_options);
            {
                EditResearchProjectViewModel editResearchProjectViewModel = new EditResearchProjectViewModel(context, _dialogService)
                {
                    ResearchProjectId = 1,
                    Title = "Updated Research Project 1",
                    Description = "Updated description for research project 1",
                    StartDate = new DateTime(2024, 5, 1),
                    EndDate = new DateTime(2025, 5, 1),
                    Budget = 20000.0f
                };
                editResearchProjectViewModel.Save.Execute(null);

                var updatedResearchProject = context.ResearchProjects.FirstOrDefault(rp => rp.ResearchProjectId == 1);

                Assert.IsNotNull(updatedResearchProject);
                Assert.AreEqual("Updated Research Project 1", updatedResearchProject.Title);
                Assert.AreEqual("Updated description for research project 1", updatedResearchProject.Description);
                Assert.AreEqual(20000.0f, updatedResearchProject.Budget);
            }
        }

        [TestMethod]
        public void Edit_research_project_without_title()
        {
            using UniversityContext context = new UniversityContext(_options);
            {
                EditResearchProjectViewModel editResearchProjectViewModel = new EditResearchProjectViewModel(context, _dialogService)
                {
                    ResearchProjectId = 2,
                    Title = "",
                    Description = "Updated description without title",
                    StartDate = new DateTime(2024, 6, 1),
                    EndDate = new DateTime(2025, 6, 1),
                    Budget = 25000.0f
                };
                editResearchProjectViewModel.Save.Execute(null);

                var updatedResearchProject = context.ResearchProjects.FirstOrDefault(rp => rp.ResearchProjectId == 2);
                Assert.IsNotNull(updatedResearchProject);
                Assert.AreNotEqual("Updated description without title", updatedResearchProject.Description);
            }
        }

        [TestMethod]
        public void Edit_research_project_without_description()
        {
            using UniversityContext context = new UniversityContext(_options);
            {
                EditResearchProjectViewModel editResearchProjectViewModel = new EditResearchProjectViewModel(context, _dialogService)
                {
                    ResearchProjectId = 3,
                    Title = "Updated Research Project 3",
                    Description = "",
                    StartDate = new DateTime(2024, 7, 1),
                    EndDate = new DateTime(2025, 7, 1),
                    Budget = 18000.0f
                };
                editResearchProjectViewModel.Save.Execute(null);

                var updatedResearchProject = context.ResearchProjects.FirstOrDefault(rp => rp.ResearchProjectId == 3);
                Assert.IsNotNull(updatedResearchProject);
                Assert.AreNotEqual(18000.0f, updatedResearchProject.Budget);
            }
        }

        #endregion

        #region Remove Tests

        [TestMethod]
        public void Remove_ResearchProject_With_Valid_Id()
        {
            using (var context = new UniversityContext(_options))
            {
                var initialCount = context.ResearchProjects.Count();
                var dialogService = new TestDialogService(true);
                var viewModel = new ResearchProjectViewModel(context, dialogService);

                viewModel.Remove.Execute((long)1);

                var researchProjectExists = context.ResearchProjects.Any(c => c.ResearchProjectId == 1);
                Assert.IsFalse(researchProjectExists);
                Assert.AreEqual(initialCount - 1, context.ResearchProjects.Count());
            }
        }

        [TestMethod]
        public void Remove_ResearchProject_Without_Confirmation()
        {
            using (var context = new UniversityContext(_options))
            {
                var initialCount = context.ResearchProjects.Count();
                var dialogService = new TestDialogService(false);
                var viewModel = new ResearchProjectViewModel(context, dialogService);

                viewModel.Remove.Execute((long)1);

                var researchProjectExists = context.ResearchProjects.Any(c => c.ResearchProjectId == 1);
                Assert.IsTrue(researchProjectExists);
                Assert.AreEqual(initialCount, context.ResearchProjects.Count());
            }
        }

        [TestMethod]
        public void Remove_ResearchProject_With_Invalid_Id()
        {
            using (var context = new UniversityContext(_options))
            {
                var initialCount = context.ResearchProjects.Count();
                var dialogService = new TestDialogService(true);
                var viewModel = new ResearchProjectViewModel(context, dialogService);

                viewModel.Remove.Execute((long)9999);

                var researchProjectExists = context.ResearchProjects.Any(c => c.ResearchProjectId == 9999);
                Assert.IsFalse(researchProjectExists);
                Assert.AreEqual(initialCount, context.ResearchProjects.Count());
            }
        }
        #endregion

    }
}
