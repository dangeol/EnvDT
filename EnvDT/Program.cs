namespace EnvDT
{
    class Program
    {
        static void Main()
        {
            using (var db = new EnvDTContext())
            {
                db.SaveChanges();
            }
        }
    }
}
