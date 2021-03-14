﻿using EnvDT.Model.Entity;
using System.Collections.Generic;

namespace EnvDT.Model.Core.HelperEntity
{
    public class FootnoteResult
    {
        public bool Result { get; set; }
        public HashSet<PublParam> MissingParams { get; set; }
        public HashSet<string> TakingAccountOf { get; set; }
    }
}
