using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace EnvDT
{
    class Program
    {
        static void Main()
        {
            using (var db = new EnvDTContext())
            {
                // Create
                Console.WriteLine("Inserting a new data");
                var condition = new Model.Condition { ConditionId = 13784738, Condition1 = "cond1" };
                var refValueName = new Model.RefValueName { RefValueNameId = 23782738 };
                var unit = new Model.Unit { UnitId = 33782738, UnitName = "µg/l" };
                var unit2 = new Model.Unit { UnitId = 52782738, UnitName = "mg/l" };
                db.Units.Add(unit2);
                var medium = new Model.Medium { MediumId = 43782738 };
                var publication = new Model.Publication { PublicationId = 53782738 };
                db.Add(new Model.RefValue { RefValueId = 193873837, 
                    Value = 0.1, 
                    Condition = condition, 
                    RefValueName = refValueName, 
                    Unit = unit, 
                    Medium = medium, 
                    Publication = publication });
                db.SaveChanges();

                // Read
                Console.WriteLine("Querying for a refValue");
                var refValue = db.RefValues
                    .OrderBy(u => u.RefValueId)
                    .First();

                // Update
                Console.WriteLine("Updating the value and unit");
                refValue.Value = 0.2;
                refValue.Unit = unit2;
                db.SaveChanges();

                // Delete
                Console.WriteLine("Delete a unit");
                db.Remove(unit);
                db.SaveChanges();
            }
        }
    }
}
