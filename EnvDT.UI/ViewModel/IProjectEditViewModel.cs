using EnvDT.UI.Wrapper;
using System;

namespace EnvDT.UI.ViewModel
{
    public interface IProjectEditViewModel
    {
        void Load(Guid projectId);
        ProjectWrapper Project { get; }
    }
}
