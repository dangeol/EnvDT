using System;
using System.Collections.Generic;
using System.Text;

namespace EnvDT.Model
{
    public class Parameter
    {
        public int ParameterId { get; set; }
        public string ParamNameEn { get; set; }
        public string ParamNameDe { get; set; }
        public string CommentEn { get; set; }
        public string CommentDe { get; set; }

        public List<RefValueParam> RefValueParams { get; } = new List<RefValueParam>();
        public List<CAS> CASs { get; } = new List<CAS>();
    }
}
