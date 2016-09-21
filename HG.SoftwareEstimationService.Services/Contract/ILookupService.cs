using System.Collections.Generic;
using HG.SoftwareEstimationService.Dto;

namespace HG.SoftwareEstimationService.Services.Contract
{
    public interface ILookupService
    {
        Dictionary<string, EnumItem[]> GetLookups(List<string> enumbersions);
    }
}