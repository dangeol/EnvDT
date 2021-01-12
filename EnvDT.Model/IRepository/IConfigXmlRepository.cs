﻿using EnvDT.Model.Entity;
using System;

namespace EnvDT.Model.IRepository
{
    public interface IConfigXmlRepository : IGenericRepository<ConfigXml>
    {
        public ConfigXml GetByLaboratoryId(Guid? laboratoryId);
    }
}