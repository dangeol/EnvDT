using EnvDT.Model.Core.HelperEntity;
using System;

namespace EnvDT.Model.Core
{
    public interface IFootnotes
    {
        public FootnoteResult IsFootnoteCondTrue(EvalArgs evalArgs, Guid? footnoteId);
    }
}