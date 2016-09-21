using System.Collections.Generic;
using HG.SoftwareEstimationService.Dto.ViewModels;

namespace HG.SoftwareEstimationService.Services.Contract
{
    public interface ISystemInDevelopmentService
    {
        IList<SystemInDevelopmentHomeGrid> GetAllSystemInDevelopmentReduced();
        IList<SystemInDevelopmentGrid> GetAllSystemInDevelopment();
        void AddSystem(SystemInDevelopmentGrid system);
        void UpdateSystem(SystemInDevelopmentGrid system);
        void DeleteSystem(int systemId);
    }
}