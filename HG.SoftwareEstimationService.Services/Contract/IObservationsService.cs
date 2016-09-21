using System.Collections.Generic;
using HG.SoftwareEstimationService.Dto;
using HG.SoftwareEstimationService.Dto.ViewModels;

namespace HG.SoftwareEstimationService.Services.Contract
{
    public interface IObservationsService
    {
        IEnumerable<ObservationsGrid> GetObservations(int ticketId);
        void AddObservation(ObservationsGrid observation);
        void UpdateObservation(ObservationsGrid observation);
        void DeleteObservation(int observationId);

        PropertyValuesDto GetObservationProperties(int observationId);
        long SaveObservationProperties(PropertyValuesDto propertyValues);
    }
}