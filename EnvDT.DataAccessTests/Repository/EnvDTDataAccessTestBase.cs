using EnvDT.DataAccess;
using EnvDT.Model.Entity;
using Microsoft.EntityFrameworkCore;
using System;

namespace EnvDT.DataAccessTests.Repository
{
    public class EnvDTDataAccessTestBase
    {
        protected EnvDTDataAccessTestBase(DbContextOptions<EnvDTDbContext> contextOptions)
        {
            ContextOptions = contextOptions;

            Seed();
        }

        protected DbContextOptions<EnvDTDbContext> ContextOptions { get; }

        private void Seed()
        {
            using var context = new EnvDTDbContext(ContextOptions);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            // *** Publications ***
            var publication1 = new Publication
            {
                PublicationId = new Guid("d50336ad-0c60-4f5e-8823-b7dd1ad3f9ff"),
                Title = "Publication1"
            }; // relevant

            var publication2 = new Publication
            {
                PublicationId = new Guid("506bc1d8-653b-492b-8598-e431e2a08155"),
                Title = "Publication2"
            };

            // *** RefValues ***
            var refValue1 = new RefValue
            {
                RefValueId = new Guid(),
                RValue = 10.0,
                PublParamId = new Guid("d50336ad-0c60-4f5e-8823-b7dd1ad3f9ff"), // publication1
                ValuationClassId = new Guid("0a85b540-c1f1-448e-a223-553a4a1ba217") // Publ1_VC1_No_No
            };

            var refValue2 = new RefValue
            {
                RefValueId = new Guid(),
                RValue = 25.0,
                PublParamId = new Guid("d50336ad-0c60-4f5e-8823-b7dd1ad3f9ff"), // publication1
                ValuationClassId = new Guid("714d87ee-cc23-41cc-93ff-92c5c139b061") // Publ1_VC2_Yes_No
            };

            var refValue3 = new RefValue
            {
                RefValueId = new Guid(),
                RValue = 30.0,
                PublParamId = new Guid("d50336ad-0c60-4f5e-8823-b7dd1ad3f9ff"), // publication1
                ValuationClassId = new Guid("0a85b540-c1f1-448e-a223-553a4a1ba217") // Publ1_VC1_No_No
            };

            var refValue4 = new RefValue
            {
                RefValueId = new Guid(),
                RValue = 31.5,
                PublParamId = new Guid("d50336ad-0c60-4f5e-8823-b7dd1ad3f9ff"), // publication1
                ValuationClassId = new Guid("42e2ac81-57df-4376-b115-9e00ac56df22") // Publ1_VC4_Yes_Yes
            };

            var refValue5 = new RefValue
            {
                RefValueId = new Guid(),
                RValue = 42.5,
                PublParamId = new Guid("d50336ad-0c60-4f5e-8823-b7dd1ad3f9ff"), // publication1
                ValuationClassId = new Guid("ae5a6d7f-0fac-40c0-990b-b912c822fb5a") // Publ1_VC3_No_Yes
            };

            var refValue6 = new RefValue
            {
                RefValueId = new Guid(),
                RValue = 35.0,
                PublParamId = new Guid("506bc1d8-653b-492b-8598-e431e2a08155"), // publication2
                ValuationClassId = new Guid("bcb05633-d0a7-47c4-9334-bddb77b00e85") // Publ2_VC1
            };

            // *** ValuationClasses ***
            var valuationClass1 = new ValuationClass
            {
                ValuationClassId = new Guid("0a85b540-c1f1-448e-a223-553a4a1ba217"),
                PublicationId = new Guid("d50336ad-0c60-4f5e-8823-b7dd1ad3f9ff"),
                ValuationClassName = "Publ1_VC1_No_No" //Publ_Name_UsesMedSubTypes_UsesConditions
            };

            var valuationClass2 = new ValuationClass
            {
                ValuationClassId = new Guid("714d87ee-cc23-41cc-93ff-92c5c139b061"),
                PublicationId = new Guid("d50336ad-0c60-4f5e-8823-b7dd1ad3f9ff"),
                ValuationClassName = "Publ1_VC2_Yes_No"
            };

            var valuationClass3 = new ValuationClass
            {
                ValuationClassId = new Guid("ae5a6d7f-0fac-40c0-990b-b912c822fb5a"),
                PublicationId = new Guid("d50336ad-0c60-4f5e-8823-b7dd1ad3f9ff"),
                ValuationClassName = "Publ1_VC3_No_Yes"
            };

            var valuationClass4 = new ValuationClass
            {
                ValuationClassId = new Guid("42e2ac81-57df-4376-b115-9e00ac56df22"),
                PublicationId = new Guid("d50336ad-0c60-4f5e-8823-b7dd1ad3f9ff"),
                ValuationClassName = "Publ1_VC4_Yes_Yes"
            };

            var valuationClass5 = new ValuationClass
            {
                ValuationClassId = new Guid("ed15aab4-bca0-4206-bec7-7008f9c6497f"),
                PublicationId = new Guid("d50336ad-0c60-4f5e-8823-b7dd1ad3f9ff"),
                ValuationClassName = "Publ1_VC5_No_No"
            };

            var valuationClass6 = new ValuationClass
            {
                ValuationClassId = new Guid("bcb05633-d0a7-47c4-9334-bddb77b00e85"),
                PublicationId = new Guid("506bc1d8-653b-492b-8598-e431e2a08155"),
                ValuationClassName = "Publ2_VC1"
            };

            // *** MediumSubTypes ***
            var mediumSubType1 = new MediumSubType
            {
                MedSubTypeId = new Guid("52a64f24-8958-4837-bf28-bf9730a44dba")
            }; // relevant

            var mediumSubType2 = new MediumSubType
            {
                MedSubTypeId = new Guid("3682a1e4-aabe-4a1d-aa4d-47493dfae583")
            };

            // *** Conditions ***
            var condition1 = new Condition
            {
                ConditionId = new Guid("6a808e55-b41e-4296-bb4f-ebd275032242")
            }; // relevant

            var condition2 = new Condition
            {
                ConditionId = new Guid("0fd3ccb6-7ac8-4b77-8ced-b293c300f7c9")
            };

            // *** ValuationClassMedSubTypes ***
            var valuationClassMedSubType1 = new ValuationClassMedSubType
            {
                ValuationClassId = new Guid("714d87ee-cc23-41cc-93ff-92c5c139b061"), // valuationClass2
                MedSubTypeId = new Guid("52a64f24-8958-4837-bf28-bf9730a44dba")
            };

            var valuationClassMedSubType2 = new ValuationClassMedSubType
            {
                ValuationClassId = new Guid("42e2ac81-57df-4376-b115-9e00ac56df22"), // valuationClass4
                MedSubTypeId = new Guid("52a64f24-8958-4837-bf28-bf9730a44dba")
            };

            // *** ValuationClassConditions ***
            var valuationClassCondition1 = new ValuationClassCondition
            {
                ValuationClassId = new Guid("ae5a6d7f-0fac-40c0-990b-b912c822fb5a"), // valuationClass3
                ConditionId = new Guid("6a808e55-b41e-4296-bb4f-ebd275032242")
            };

            var valuationClassCondition2 = new ValuationClassCondition
            {
                ValuationClassId = new Guid("42e2ac81-57df-4376-b115-9e00ac56df22"), // valuationClass4
                ConditionId = new Guid("6a808e55-b41e-4296-bb4f-ebd275032242")
            };

            // *** Samples ***
            var sample1 = new Sample
            {
                SampleId = new Guid("4d8e0575-00cb-4ca3-bcbc-5883557ff0da"), // relevant
                MediumSubTypeId = new Guid("52a64f24-8958-4837-bf28-bf9730a44dba") // mediumSubType1
            };

            var sample2 = new Sample
            {
                SampleId = new Guid("c1d6f33a-c3e7-4954-b676-8948ea2694c0"), // relevant
                ConditionId = new Guid("6a808e55-b41e-4296-bb4f-ebd275032242") // condition1
            };

            var sample3 = new Sample
            {
                SampleId = new Guid("6582467e-774e-45d6-95eb-a1821748d1ed"), // relevant
                MediumSubTypeId = new Guid("52a64f24-8958-4837-bf28-bf9730a44dba"), // mediumSubType1
                ConditionId = new Guid("6a808e55-b41e-4296-bb4f-ebd275032242") // condition1
            };

            var sample4 = new Sample
            {
                SampleId = new Guid("91395cf3-9a13-4b4e-bf0b-8680d337451c"),
                MediumSubTypeId = new Guid("3682a1e4-aabe-4a1d-aa4d-47493dfae583"), // mediumSubType2
                ConditionId = new Guid("0fd3ccb6-7ac8-4b77-8ced-b293c300f7c9") // condition2
            };

            var sample5 = new Sample
            {
                SampleId = new Guid("83a88086-ab7d-4950-88e0-bcc4859b028c")
            };

            context.AddRange
                (
                    publication1,
                    publication2,
                    refValue1,
                    refValue2,
                    refValue3,
                    refValue4,
                    refValue5,
                    refValue6,
                    valuationClass1,
                    valuationClass2,
                    valuationClass3,
                    valuationClass4,
                    valuationClass5,
                    valuationClass6,
                    mediumSubType1,
                    mediumSubType2,
                    condition1,
                    condition2,
                    valuationClassMedSubType1,
                    valuationClassMedSubType2,
                    valuationClassCondition1,
                    valuationClassCondition2,
                    sample1,
                    sample2,
                    sample3,
                    sample4,
                    sample5
                );

            context.SaveChanges();
        }
    }
}
