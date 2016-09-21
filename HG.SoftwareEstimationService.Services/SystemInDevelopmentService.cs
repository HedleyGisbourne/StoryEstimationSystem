using System.Collections.Generic;
using System.Linq;
using HG.SoftwareEstimationService.Dto.ViewModels;
using HG.SoftwareEstimationService.Repository;
using HG.SoftwareEstimationService.Services.Automapper;
using HG.SoftwareEstimationService.Services.Contract;
using HG.SoftwareEstimationService.SqliteData;

namespace HG.SoftwareEstimationService.Services
{
    public class SystemInDevelopmentService : ISystemInDevelopmentService
    {
        private readonly IRepositorySqlite<SystemInDevelopment> _systemInDevelopmentRepo;
        
        public SystemInDevelopmentService(IRepositorySqlite<SystemInDevelopment> systemInDevelopmentRepo)
        {
            _systemInDevelopmentRepo = systemInDevelopmentRepo;
        }

        public IList<SystemInDevelopmentHomeGrid> GetAllSystemInDevelopmentReduced()
        {
            return _systemInDevelopmentRepo
                .GetMany(x => !x.LogicalDelete)
                .Select(AutomapperRegistrar.Map<SystemInDevelopmentHomeGrid>)
                .ToList();
        }

        public IList<SystemInDevelopmentGrid> GetAllSystemInDevelopment()
        {
            return _systemInDevelopmentRepo
                .GetMany(x => !x.LogicalDelete)
                .Select(AutomapperRegistrar.Map<SystemInDevelopmentGrid>)
                .ToList();
        }

        public void AddSystem(SystemInDevelopmentGrid system)
        {
            _systemInDevelopmentRepo.Add(AutomapperRegistrar.Map<SystemInDevelopment>(system));
            _systemInDevelopmentRepo.SaveChanges();
        }

        public void UpdateSystem(SystemInDevelopmentGrid system)
        {
            _systemInDevelopmentRepo.Replace(x => x.SystemId, AutomapperRegistrar.Map<SystemInDevelopment>(system));
            _systemInDevelopmentRepo.SaveChanges();
        }

        public void DeleteSystem(int systemId)
        {
            _systemInDevelopmentRepo.GetSingleOrDefault(x => x.SystemId == systemId).LogicalDelete = true;
            _systemInDevelopmentRepo.SaveChanges();
        }
    }
}
