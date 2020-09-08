using EnvDT.Model;
using System;

namespace EnvDT.UI.ViewModel
{
    public interface IProjectEditViewModel
    {
        void Load(Guid projectId);
        Project Project { get; }
    }
}
