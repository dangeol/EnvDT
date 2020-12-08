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
            using (var context = new EnvDTDbContext(ContextOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                // *** Publications ***
                var publication1 = new Publication(); // relevant
                publication1.PublicationId = new Guid("d50336ad-0c60-4f5e-8823-b7dd1ad3f9ff");
                publication1.Title = "Publication1";

                var publication2 = new Publication();
                publication2.PublicationId = new Guid("506bc1d8-653b-492b-8598-e431e2a08155");
                publication2.Title = "Publication2";

                // *** RefValues ***
                var refValue1 = new RefValue();
                refValue1.RefValueId = new Guid();
                refValue1.RValue = 10.0;
                refValue1.PublParamId = new Guid("d50336ad-0c60-4f5e-8823-b7dd1ad3f9ff"); // publication1
                refValue1.ValuationClassId = new Guid("0a85b540-c1f1-448e-a223-553a4a1ba217"); // Publ1_VC1_No_No

                var refValue2 = new RefValue();
                refValue2.RefValueId = new Guid();
                refValue2.RValue = 25.0;
                refValue2.PublParamId = new Guid("d50336ad-0c60-4f5e-8823-b7dd1ad3f9ff"); // publication1
                refValue2.ValuationClassId = new Guid("714d87ee-cc23-41cc-93ff-92c5c139b061"); // Publ1_VC2_Yes_No

                var refValue3 = new RefValue();
                refValue3.RefValueId = new Guid();
                refValue3.RValue = 30.0;
                refValue3.PublParamId = new Guid("d50336ad-0c60-4f5e-8823-b7dd1ad3f9ff"); // publication1
                refValue3.ValuationClassId = new Guid("0a85b540-c1f1-448e-a223-553a4a1ba217"); // Publ1_VC1_No_No

                var refValue4 = new RefValue();
                refValue4.RefValueId = new Guid();
                refValue4.RValue = 31.5;
                refValue4.PublParamId = new Guid("d50336ad-0c60-4f5e-8823-b7dd1ad3f9ff"); // publication1
                refValue4.ValuationClassId = new Guid("42e2ac81-57df-4376-b115-9e00ac56df22"); // Publ1_VC4_Yes_Yes

                var refValue5 = new RefValue();
                refValue5.RefValueId = new Guid();
                refValue5.RValue = 35.0;
                refValue5.PublParamId = new Guid("506bc1d8-653b-492b-8598-e431e2a08155"); // publication2
                refValue5.ValuationClassId = new Guid("bcb05633-d0a7-47c4-9334-bddb77b00e85"); // Publ2_VC1

                // *** ValuationClasses ***
                var valuationClass1 = new ValuationClass();
                valuationClass1.ValuationClassId = new Guid("0a85b540-c1f1-448e-a223-553a4a1ba217");
                valuationClass1.PublicationId = new Guid("d50336ad-0c60-4f5e-8823-b7dd1ad3f9ff");
                valuationClass1.ValuationClassName = "Publ1_VC1_No_No"; //Publ_Name_UsesMedSubTypes_UsesConditions

                var valuationClass2 = new ValuationClass();
                valuationClass2.ValuationClassId = new Guid("714d87ee-cc23-41cc-93ff-92c5c139b061");
                valuationClass2.PublicationId = new Guid("d50336ad-0c60-4f5e-8823-b7dd1ad3f9ff");
                valuationClass2.ValuationClassName = "Publ1_VC2_Yes_No";

                var valuationClass3 = new ValuationClass();
                valuationClass3.ValuationClassId = new Guid("ae5a6d7f-0fac-40c0-990b-b912c822fb5a");
                valuationClass3.PublicationId = new Guid("d50336ad-0c60-4f5e-8823-b7dd1ad3f9ff");
                valuationClass3.ValuationClassName = "Publ1_VC3_No_Yes";

                var valuationClass4 = new ValuationClass();
                valuationClass4.ValuationClassId = new Guid("42e2ac81-57df-4376-b115-9e00ac56df22");
                valuationClass4.PublicationId = new Guid("d50336ad-0c60-4f5e-8823-b7dd1ad3f9ff");
                valuationClass4.ValuationClassName = "Publ1_VC4_Yes_Yes";

                var valuationClass5 = new ValuationClass();
                valuationClass5.ValuationClassId = new Guid("ed15aab4-bca0-4206-bec7-7008f9c6497f");
                valuationClass5.PublicationId = new Guid("d50336ad-0c60-4f5e-8823-b7dd1ad3f9ff");
                valuationClass5.ValuationClassName = "Publ1_VC5_No_No";

                var valuationClass6 = new ValuationClass();
                valuationClass6.ValuationClassId = new Guid("bcb05633-d0a7-47c4-9334-bddb77b00e85");
                valuationClass6.PublicationId = new Guid("506bc1d8-653b-492b-8598-e431e2a08155");
                valuationClass6.ValuationClassName = "Publ2_VC1";

                // *** MediumSubTypes ***
                var mediumSubType1 = new MediumSubType(); // relevant
                mediumSubType1.MedSubTypeId = new Guid("52a64f24-8958-4837-bf28-bf9730a44dba");

                var mediumSubType2 = new MediumSubType();
                mediumSubType2.MedSubTypeId = new Guid("3682a1e4-aabe-4a1d-aa4d-47493dfae583");

                // *** Conditions ***
                var condition1 = new Condition(); // relevant
                condition1.ConditionId = new Guid("6a808e55-b41e-4296-bb4f-ebd275032242");

                var condition2 = new Condition();
                condition2.ConditionId = new Guid("0fd3ccb6-7ac8-4b77-8ced-b293c300f7c9");

                // *** ValuationClassMedSubTypes ***
                var valuationClassMedSubType1 = new ValuationClassMedSubType();
                valuationClassMedSubType1.ValuationClassId = new Guid("714d87ee-cc23-41cc-93ff-92c5c139b061"); // valuationClass2
                valuationClassMedSubType1.MedSubTypeId = new Guid("52a64f24-8958-4837-bf28-bf9730a44dba");

                var valuationClassMedSubType2 = new ValuationClassMedSubType();
                valuationClassMedSubType2.ValuationClassId = new Guid("42e2ac81-57df-4376-b115-9e00ac56df22"); // valuationClass4
                valuationClassMedSubType2.MedSubTypeId = new Guid("52a64f24-8958-4837-bf28-bf9730a44dba");

                // *** ValuationClassConditions ***
                var valuationClassCondition1 = new ValuationClassCondition();
                valuationClassCondition1.ValuationClassId = new Guid("ae5a6d7f-0fac-40c0-990b-b912c822fb5a"); // valuationClass3
                valuationClassCondition1.ConditionId = new Guid("6a808e55-b41e-4296-bb4f-ebd275032242");

                var valuationClassCondition2 = new ValuationClassCondition();
                valuationClassCondition2.ValuationClassId = new Guid("42e2ac81-57df-4376-b115-9e00ac56df22"); // valuationClass4
                valuationClassCondition2.ConditionId = new Guid("6a808e55-b41e-4296-bb4f-ebd275032242");

                context.AddRange
                    (
                        publication1,
                        publication2,
                        refValue1,
                        refValue2,
                        refValue3,
                        refValue4,
                        refValue5,
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
                        valuationClassCondition2
                    );

                context.SaveChanges();
            }
        }
    }
}
