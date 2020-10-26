using EnvDT.Model.Entity;
using EnvDT.Model.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EnvDT.DataAccess.Repository
{
    public class PublParamRepository : GenericRepository<PublParam, EnvDTDbContext>,
        IPublParamRepository
    {
        public PublParamRepository(EnvDTDbContext context)
            :base(context)
        {
        }
    }
}