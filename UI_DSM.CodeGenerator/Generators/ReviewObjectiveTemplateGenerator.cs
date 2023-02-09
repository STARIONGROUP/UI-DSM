// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewObjectiveTemplateGenerator.cs" company="RHEA System S.A.">
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

namespace UI_DSM.CodeGenerator.Generators
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.Json;
    using System.Threading.Tasks;

    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    using JsonSerializer = UI_DSM.Serializer.Json.JsonSerializer;

    /// <summary>
    ///     A generator to generate JSON template that will be used to create <see cref="ReviewObjective" /> and
    ///     <see cref="ReviewTask" />
    /// </summary>
    public class ReviewObjectiveTemplateGenerator : Generator
    {
        /// <summary>
        ///     The collection that will contain <see cref="ReviewObjective" />
        /// </summary>
        private readonly List<ReviewObjective> reviewObjectives;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ReviewObjectiveTemplateGenerator" /> class.
        /// </summary>
        public ReviewObjectiveTemplateGenerator()
        {
            this.reviewObjectives = new List<ReviewObjective>();
            this.CreateReviewObjectives();
        }

        /// <summary>
        ///     Generates code specific to the concrete implementation
        /// </summary>
        /// <param name="outputDirectory">
        ///     The target <see cref="DirectoryInfo" />
        /// </param>
        /// <returns>
        ///     An awaitable <see cref="Task" />
        /// </returns>
        public override async Task Generate(DirectoryInfo outputDirectory)
        {
            var dtos = new List<EntityDto>();

            foreach (var reviewObjective in this.reviewObjectives)
            {
                dtos.Add(reviewObjective.ToDto());
                dtos.AddRange(reviewObjective.ReviewTasks.Select(x => x.ToDto()));
            }

            var serializer = new JsonSerializer();

            using var stream = new MemoryStream();

            await serializer.SerializeAsync(dtos, stream, new JsonWriterOptions
            {
                Indented = true
            });

            stream.Position = 0;
            using var streamReader = new StreamReader(stream);
            var serialized = await streamReader.ReadToEndAsync();

            await Write(serialized, outputDirectory, "ReviewObjectives.json");
        }

        /// <summary>
        ///     Creates all <see cref="ReviewObjective" />s
        /// </summary>
        private void CreateReviewObjectives()
        {
            var prr1 = InitializeReviewObjective(ReviewObjectiveKind.Prr, 1,
                "Verify the completeness, adequacy and consistency of the preliminary management, design and development, product assurance and associated assembly, integration and verification (AIV) plan.",
                default, View.ProductBreakdownStructureView);

            prr1.ReviewTasks.Add(InitializeReviewTask("Review available/linked documentation.", 1, View.DocumentBased, prefilters:
                new List<string> { "product_assurance", "management", "aiv", "design_development" }));

            prr1.ReviewTasks.Add(InitializeReviewTask("Read related requirements.", 2, View.RequirementBreakdownStructureView
                , prefilters: new List<string> { "pa", "management", "aiv", "design_development" }));

            prr1.ReviewTasks.Add(InitializeReviewTask("Check consistency of the related requirements.", 3,
                View.RequirementVerificationControlView, hasPrimaryView: true));

            var prr2 = InitializeReviewObjective(ReviewObjectiveKind.Prr, 2, "Verify the proper translation and allocation of the ESA mission requirements into a set of system and subsystem technical specifications in terms of completeness, adequacy and consistency.",
                default, View.ProductBreakdownStructureView);

            prr2.ReviewTasks.Add(InitializeReviewTask("Read requirements inside the requirements table.", 1, View.RequirementBreakdownStructureView, hasPrimaryView: true));
            prr2.ReviewTasks.Add(InitializeReviewTask("Do completeness check.", 2, View.RequirementTraceabilityToRequirementView));
            prr2.ReviewTasks.Add(InitializeReviewTask("Check relationships between requirements and requirements flow down.", 3, View.RequirementTraceabilityToRequirementView));

            var prr3 = InitializeReviewObjective(ReviewObjectiveKind.Prr, 3, "Verify the completeness, adequacy and consistency of the preliminary design and compliance with the ESA requirements.",
                default, View.RequirementBreakdownStructureView, View.FunctionalBreakdownStructureView, View.ProductBreakdownStructureView, View.InterfaceView);

            prr3.ReviewTasks.Add(InitializeReviewTask("Check mass budgets.", 1, View.BudgetView));

            prr3.ReviewTasks.Add(InitializeReviewTask("Check if requirements are allocated to functions adequately.", 2,
                 View.RequirementTraceabilityToFunctionView, optionalView:View.RequirementTraceabilityToProductView, hasPrimaryView:true));

            prr3.ReviewTasks.Add(InitializeReviewTask("Check function allocation to products.", 3, View.FunctionalTraceabilityToProductView));
            prr3.ReviewTasks.Add(InitializeReviewTask("Check requirements verification.", 4, View.RequirementVerificationControlView));
            prr3.ReviewTasks.Add(InitializeReviewTask("Do completeness check", 5, View.RequirementTraceabilityToRequirementView));

            var prr4 = InitializeReviewObjective(ReviewObjectiveKind.Prr, 4, "To verify completeness, adequacy and consistency of the internal and external interfaces from system level, down to segment and subsystem level.", default);
            prr4.ReviewTasks.Add(InitializeReviewTask("Check adequacy and completeness of interfaces definition.", 1, View.InterfaceView, hasPrimaryView: true));
            prr4.ReviewTasks.Add(InitializeReviewTask("Check that all components are connected/have ports.", 2, View.InterfaceView, prefilters:new List<string>{ "product" }));
            prr4.ReviewTasks.Add(InitializeReviewTask("Check the consistency and completeness of a product interface with the physical architecture.", 3, View.InterfaceView, additionalView: View.PhysicalFlowView));

            var prr5 = InitializeReviewObjective(ReviewObjectiveKind.Prr, 5, "Verify the completeness and adequacy of the industry cost estimates report and its consistency with schedule and development plans and the organisational breakdown structure (for Cat. 1 reviews only).",
                new List<string> { "cost" }, View.ProductBreakdownStructureView, View.FunctionalBreakdownStructureView);

            prr5.ReviewTasks.Add(InitializeReviewTask("Review available/linked documentation.", 1,
                View.DocumentBased, hasPrimaryView: true, prefilters: new List<string> { "cost" }));

            var prr6 = InitializeReviewObjective(ReviewObjectiveKind.Prr, 6, "Verify the completeness of the risk management plan and that all technical and programmatic risks have been identified and that adequate mitigation actions are in place. This includes the identification of critical technologies, corresponding Technology Readiness Levels (TRL) and initiated pre-development activities.",
	            new List<string> { "trl" }, View.FunctionalBreakdownStructureView, View.RequirementTraceabilityToProductView, View.RequirementTraceabilityToFunctionView);

            prr6.ReviewTasks.Add(InitializeReviewTask("Check TRL “budget/report” inside the product tree.", 1, View.ProductBreakdownStructureView, View.BudgetView, View.TrlView, true));
            prr6.ReviewTasks.Add(InitializeReviewTask("Check critical technologies.", 2, View.FunctionalTraceabilityToProductView));

            prr6.ReviewTasks.Add(InitializeReviewTask("Check the risk management plan, that all technical and programmatic risks have been identified and that adequate mitigation actions are in place.", 3, View.DocumentBased
                , prefilters: new List<string> { "technical_risks" }));

            prr6.ReviewTasks.Add(InitializeReviewTask("Check documentation on initiated pre-development activities.", 4, View.DocumentBased
                , prefilters: new List<string> { "pre-development_activities" }));

            prr6.ReviewTasks.Add(InitializeReviewTask("Confirm validity and maturity of the spacecraft and payload design concept with the product tree.", 5, View.TrlView));

            var prr7 = InitializeReviewObjective(ReviewObjectiveKind.Prr, 7, "Verify the completeness and adequacy of the Product Assurance documents, in particular the critical item list, the qualification status list, and their compliance with the contractually applicable system requirements and the PA plan.", 
                default, View.RequirementVerificationControlView, View.FunctionalBreakdownStructureView, View.ProductBreakdownStructureView);

            prr7.ReviewTasks.Add(InitializeReviewTask("Review available/linked documentation.", 1, View.DocumentBased, hasPrimaryView: true
                , prefilters: new List<string> { "product_assurance" }));

            var prr8 = InitializeReviewObjective(ReviewObjectiveKind.Prr, 8, "Verify the completeness and adequacy of the Space Debris mitigation plan and its compliance with the ESA Space Debris Mitigation requirements."
                , default, View.ProductBreakdownStructureView);

            prr8.ReviewTasks.Add(InitializeReviewTask("Review available/linked documentation.", 1, View.DocumentBased, hasPrimaryView: true,
                prefilters: new List<string> { "space_debris" }));

            prr8.ReviewTasks.Add(InitializeReviewTask("Review related requirements (filter) inside the requirements table.", 2, View.RequirementBreakdownStructureView
                , prefilters: new List<string> { "space_debris" }));

            var prr9 = InitializeReviewObjective(ReviewObjectiveKind.Prr, 9, "Verify that the Project has taken all identified lessons learned of other Projects into account and propose potentially applicable recommendations to other programs. Such lessons learned items will be handled in the ESA Lessons Learned process.", default);

            prr9.ReviewTasks.Add(InitializeReviewTask("Review available/linked documentation.", 1, View.DocumentBased, hasPrimaryView: true
                , prefilters: new List<string> { "lessons_learned" }));

            var srr1 = InitializeReviewObjective(ReviewObjectiveKind.Srr, 1, "Verify the proper translation and allocation of the ESA mission requirements into a set of system and subsystem technical specifications, including analysis and justification documents in terms of completeness, adequacy and consistency.",
                default, View.ProductBreakdownStructureView);

            srr1.ReviewTasks.Add(InitializeReviewTask("Read requirements inside the requirements table.", 1, View.RequirementBreakdownStructureView, hasPrimaryView: true));
            srr1.ReviewTasks.Add(InitializeReviewTask("Do completeness check.", 2, View.RequirementTraceabilityToRequirementView));
            srr1.ReviewTasks.Add(InitializeReviewTask("Check relationships between requirements and requirements flow down.", 3, View.RequirementTraceabilityToRequirementView));
            srr1.ReviewTasks.Add(InitializeReviewTask("Check requirements traceability to products.", 4, View.RequirementTraceabilityToProductView));
            srr1.ReviewTasks.Add(InitializeReviewTask("Check requirements traceability to functions.", 5, View.RequirementTraceabilityToFunctionView));

            srr1.ReviewTasks.Add(InitializeReviewTask("Check additional documents (Justification & Analysis).", 6, View.DocumentBased,
                prefilters: new List<string> { "justifications" }));

            var srr2 = InitializeReviewObjective(ReviewObjectiveKind.Srr, 2,
                "Verify that the proposed system design and predicted performances meet the Agency’s requirements for all mission phases, including launcher compatibility, and that the allocation of the performances, budgets and margins to the various elements of the System are compliant with the system level requirements, including availability and reliability requirements.",
                default, View.RequirementBreakdownStructureView, View.FunctionalBreakdownStructureView, View.ProductBreakdownStructureView, View.InterfaceView);

            srr2.ReviewTasks.Add(InitializeReviewTask("Check mass budgets.", 1, View.BudgetView, hasPrimaryView: true));

            srr2.ReviewTasks.Add(InitializeReviewTask("Check if requirements are allocated to functions adequately.", 2,
                View.RequirementTraceabilityToFunctionView, View.RequirementTraceabilityToProductView));

            srr2.ReviewTasks.Add(InitializeReviewTask("Check function allocation to products.", 3, View.FunctionalTraceabilityToProductView));
            srr2.ReviewTasks.Add(InitializeReviewTask("Check requirements verification.", 4, View.RequirementVerificationControlView));
            srr2.ReviewTasks.Add(InitializeReviewTask("Do completeness check.", 5, View.RequirementTraceabilityToRequirementView));

            srr2.ReviewTasks.Add(InitializeReviewTask("Review available/linked documentation.", 6, View.DocumentBased,
                prefilters: new List<string> { "design_development" }));

            var srr3 = InitializeReviewObjective(ReviewObjectiveKind.Srr, 3, "Verify the technical feasibility of the preliminary system design and operations concepts.",
                default, View.InterfaceView);

            srr3.ReviewTasks.Add(InitializeReviewTask("Review available/linked documentation.", 1, View.DocumentBased,
                prefilters: new List<string> { "mission_analysis" }));

            srr3.ReviewTasks.Add(InitializeReviewTask("Check system design inside the function tree.", 2, View.FunctionalBreakdownStructureView, View.FunctionalTraceabilityToProductView,
                View.ProductBreakdownStructureView, hasPrimaryView: true));

            srr3.ReviewTasks.Add(InitializeReviewTask("Check mass budgets.", 3, View.BudgetView));

            var srr4 = InitializeReviewObjective(ReviewObjectiveKind.Srr, 4, "Verify the completeness, adequacy, and consistency of the preliminary management plan, design and development plan and associated assembly, integration and verification (AIV) plan with the development schedule, in particular long lead item procurement.", default);

            srr4.ReviewTasks.Add(InitializeReviewTask("Review available/linked documentation.", 1, View.DocumentBased, hasPrimaryView: true,
                prefilters: new List<string> { "product_assurance", "management", "aiv", "design_development" }));

            srr4.ReviewTasks.Add(InitializeReviewTask("Check requirement verification.", 2, View.RequirementVerificationControlView,
                prefilters: new List<string> { "pa", "management", "aiv", "design_development" }));

            var srr5 = InitializeReviewObjective(ReviewObjectiveKind.Srr, 5, "Verify the completeness and adequacy of the Product Assurance documents, in particular the critical item list, the qualification status list, and their compliance with the contractually applicable system requirements and the PA plan.", default,
                View.RequirementVerificationControlView, View.FunctionalBreakdownStructureView, View.ProductBreakdownStructureView);

            srr5.ReviewTasks.Add(InitializeReviewTask("Review available/linked documentation.", 1, View.DocumentBased, hasPrimaryView: true,
                prefilters: new List<string> { "product_assurance" }));

            var srr6 = InitializeReviewObjective(ReviewObjectiveKind.Srr, 6, "Verify completeness, adequacy, and consistency of the internal and external interfaces from system level, down to segment and subsystem level.", default);
            srr6.ReviewTasks.Add(InitializeReviewTask("Check adequacy and completeness of interfaces definition.", 1, View.InterfaceView, hasPrimaryView: true));
            srr6.ReviewTasks.Add(InitializeReviewTask("Check that all components are connected/have ports.", 2, View.InterfaceView, prefilters: new List<string> { "product" }));
            srr6.ReviewTasks.Add(InitializeReviewTask("Check the consistency and completeness of a product interface with the physical architecture.", 3, View.InterfaceView, additionalView: View.PhysicalFlowView));

            var srr7 = InitializeReviewObjective(ReviewObjectiveKind.Srr, 7, "Verify the completeness and adequacy of the industry cost estimates report and its consistency with schedule and development plans and the organisational breakdown structure (for Cat. 1 reviews only)."
                , new List<string> { "cost" }, View.ProductBreakdownStructureView, View.FunctionalBreakdownStructureView);

            srr7.ReviewTasks.Add(InitializeReviewTask("Review available/linked documentation.", 1, View.DocumentBased, hasPrimaryView: true
                , prefilters: new List<string> { "cost" }));

            var srr8 = InitializeReviewObjective(ReviewObjectiveKind.Srr, 8, "Verify the completeness of the risk management plan and that all technical and programmatic risks have been identified and that adequate mitigation actions are in place. This includes the identification of critical technologies, corresponding Technology Readiness Levels (TRL) and initiated pre-development activities.",
	            new List<string> { "trl" }, View.FunctionalBreakdownStructureView, View.RequirementTraceabilityToProductView, View.RequirementTraceabilityToFunctionView);

            srr8.ReviewTasks.Add(InitializeReviewTask("Check TRL “budget/report” inside the product tree.", 1, View.ProductBreakdownStructureView, View.BudgetView, View.TrlView, true));
            srr8.ReviewTasks.Add(InitializeReviewTask("Check critical technologies", 2, View.FunctionalTraceabilityToProductView));

            srr8.ReviewTasks.Add(InitializeReviewTask("Check the risk management plan, that all technical and programmatic risks have been identified and that adequate mitigation actions are in place.",
                3, View.DocumentBased, prefilters: new List<string> { "technical_risks" }));

            srr8.ReviewTasks.Add(InitializeReviewTask("Check documentation on initiated pre-development activities.", 4, View.DocumentBased,
                prefilters: new List<string> { "pre-development_activities" }));

            srr8.ReviewTasks.Add(InitializeReviewTask("Confirm validity and maturity of the spacecraft and payload design concept with the product tree.", 5, View.TrlView));

            var srr9 = InitializeReviewObjective(ReviewObjectiveKind.Srr, 9, "Verify the completeness and adequacy of the Space Debris mitigation plan and its compliance with the ESA Space Debris Mitigation requirements.",
                default, View.RequirementBreakdownStructureView, View.ProductBreakdownStructureView);

            srr9.ReviewTasks.Add(InitializeReviewTask("Review available/linked documentation.", 1, View.DocumentBased, hasPrimaryView: true
                , prefilters: new List<string> { "space_debris" }));

            var srr10 = InitializeReviewObjective(ReviewObjectiveKind.Srr, 10, "Verify that the Project has taken all identified lessons learned of other Projects into account and propose potentially applicable recommendations to other programs. Such lessons learned items will be handled in the ESA Lessons Learned process.", default);

            srr10.ReviewTasks.Add(InitializeReviewTask("Review available/linked documentation.", 1, View.DocumentBased, hasPrimaryView: true
                , prefilters: new List<string> { "lessons_learned" }));

            this.reviewObjectives.AddRange(new List<ReviewObjective>
            {
                prr1, prr2, prr3, prr4, prr5, prr6, prr7, prr8, prr9, srr1, srr2, srr3, srr4, srr5, srr6, srr7, srr8, srr9, srr10
            });
        }

        /// <summary>
        ///     Initializes a new <see cref="ReviewObjective" />
        /// </summary>
        /// <param name="kind">The <see cref="ReviewObjectiveKind" /></param>
        /// <param name="number">The review objective kind number</param>
        /// <param name="description">The description</param>
        /// <param name="view">An array of <see cref="View" /></param>
        /// <param name="additionnalColumns">The additionnal columns visible at start collection</param>
        /// <returns>The initializes <see cref="ReviewObjective" /></returns>
        private static ReviewObjective InitializeReviewObjective(ReviewObjectiveKind kind, int number, string description,
            List<string> additionnalColumns, params View[] view)
        {
            return new ReviewObjective
            {
                Id = Guid.NewGuid(),
                ReviewObjectiveKind = kind,
                ReviewObjectiveKindNumber = number,
                Description = description,
                RelatedViews = view.ToList(),
                AdditionnalColumnsVisibleAtStart = additionnalColumns ?? new List<string>()
            };
        }

        /// <summary>
        ///     Initializes a new <see cref="ReviewTask" />
        /// </summary>
        /// <param name="description">The title</param>
        /// <param name="taskNumber">The task number</param>
        /// <param name="mainView">The main <see cref="View" /></param>
        /// <param name="optionalView">The optional <see cref="View" /></param>
        /// <param name="additionalView">The additional <see cref="View" /></param>
        /// <param name="hasPrimaryView">The hasPrimaryView</param>
        /// <param name="prefilters">The prefilters collection</param>
        /// <returns>The initialized <see cref="ReviewTask" /></returns>
        private static ReviewTask InitializeReviewTask(string description, int taskNumber, View mainView, View optionalView = View.None,
            View additionalView = View.None, bool hasPrimaryView = false, List<string> prefilters = default)
        {
            return new ReviewTask
            {
                Id = Guid.NewGuid(),
                Description = description,
                TaskNumber = taskNumber,
                MainView = mainView,
                OptionalView = optionalView,
                AdditionalView = additionalView,
                HasPrimaryView = hasPrimaryView,
                Prefilters = prefilters ?? new List<string>()
            };
        }
    }
}
