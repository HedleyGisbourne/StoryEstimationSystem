using System.Collections.Generic;
using System.Linq;
using HG.SoftwareEstimationService.Dto;
using HG.SoftwareEstimationService.Repository;
using HG.SoftwareEstimationService.Services.ApplicationConfiguration.Contract;
using HG.SoftwareEstimationService.Services.Automapper;
using HG.SoftwareEstimationService.SqliteData;

namespace HG.SoftwareEstimationService.Services.ApplicationConfiguration
{
    public class ApplicationConfigurationService : IApplicationConfigurationService
    {
        private readonly IRepositorySqlite<EnumerationItem> _enumerationItemRepository;
        private readonly IRepositorySqlite<Property> _propertyRepository;
        private readonly IRepositorySqlite<PartType> _partTypeRepository;

        public ApplicationConfigurationService(
            IRepositorySqlite<EnumerationItem> enumerationItemRepository, 
            IRepositorySqlite<Property> propertyRepository, 
            IRepositorySqlite<PartType> partTypeRepository)
        {
            _enumerationItemRepository = enumerationItemRepository;
            _propertyRepository = propertyRepository;
            _partTypeRepository = partTypeRepository;
        }

        public ApplicationConfigurationDto GetApplicationConfiguration()
        {
            return new ApplicationConfigurationDto
            {
                Enumerations = GetEnumnerationItems(),
                PartTypes = GetPartTypeDefinition(),
                Properties = GetProperties()
            };
        }

        private EnumerationDto[] GetEnumnerationItems()
        {
            IEnumerable<IGrouping<long, EnumerationItem>> groupedEnumerations = _enumerationItemRepository.GetAll()
                .GroupBy(ei => ei.EnumerationId);

            return groupedEnumerations
                .Select(e =>
                    new EnumerationDto
                    {
                        EnumerationId = e.Key,
                        EnumItems = e.Select(ei => new EnumItem { Name = ei.Name, Id = ei.EnumerationEnumerationId }).ToArray()
                    }).ToArray();
        }

        private PropertyDto[] GetProperties()
        {
            return _propertyRepository.GetAll()
                .Select(AutomapperRegistrar.Map<PropertyDto>).ToArray();
        }

        private PartTypeDefinitionDto[] GetPartTypeDefinition()
        {
            return _partTypeRepository.GetAll()
                .Select(pt => new PartTypeDefinitionDto
                {
                    Name = pt.Name,
                    PartTypeId = pt.PartTypeId,
                    PropertyIds = pt.PartTypeProperties.Select(p => p.PropertyId).ToArray()
                }).ToArray();
        }
    }
}
