using System;
using System.Collections.Generic;
using System.Linq;
using HG.SoftwareEstimationService.Dto;
using HG.SoftwareEstimationService.Repository;
using HG.SoftwareEstimationService.Services.Contract;
using HG.SoftwareEstimationService.SqliteData;

namespace HG.SoftwareEstimationService.Services
{
    public class LookupService : ILookupService
    {
        private readonly IRepositorySqlite<Enumeration> _enumerationRepo;

        public LookupService(IRepositorySqlite<Enumeration> enumerationRepo)
        {
            _enumerationRepo = enumerationRepo;
        }

        public Dictionary<string, EnumItem[]> GetLookups(List<string> enumerations)
        {
            return enumerations.ToDictionary(id => id, GetLookup);
        }

        private EnumItem[] GetLookup(string enumerationName)
        {
            Enumeration enumeration = _enumerationRepo
                .GetSingleOrDefault(e => string.Equals(e.Name, enumerationName, StringComparison.InvariantCultureIgnoreCase));

            if (enumeration != null)
            {
                return enumeration
                    .EnumerationItems.Select(ei => new EnumItem {Id = ei.EnumerationEnumerationId, Name = ei.Name})
                    .ToArray();
            }

            throw new ArgumentException(string.Format("{0} is not defined for Lookup.", enumerationName));
        }
    }
}
