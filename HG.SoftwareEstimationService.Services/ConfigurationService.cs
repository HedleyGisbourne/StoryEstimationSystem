using HG.SoftwareEstimationService.Dto;
using HG.SoftwareEstimationService.Repository;
using HG.SoftwareEstimationService.Services.Contract;

namespace HG.SoftwareEstimationService.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly IRepositorySqlite<SqliteData.Configuration> _configurationRepo;

        public ConfigurationService(IRepositorySqlite<SqliteData.Configuration> configurationRepo)
        {
            _configurationRepo = configurationRepo;
        }

        public DurationDefinition GetDurationDefinition()
        {
            return new DurationDefinition
            {
                HoursInWorkDay = _configurationRepo.GetById(1).Value,
                DaysInWorkWeek = _configurationRepo.GetById(2).Value,
                DaysInWorkMonth = _configurationRepo.GetById(3).Value,
                WeeksInWorkYear = _configurationRepo.GetById(4).Value,
                MinimumEstimation = _configurationRepo.GetById(5).Value,
                MaximumEstimation = _configurationRepo.GetById(6).Value,
            };
        }
    }
}
