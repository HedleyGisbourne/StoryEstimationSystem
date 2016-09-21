using System.Collections.Generic;
using System.Linq;
using HG.SoftwareEstimationService.Dto;
using HG.SoftwareEstimationService.Dto.ViewModels;
using HG.SoftwareEstimationService.Helpers;
using HG.SoftwareEstimationService.Repository;
using HG.SoftwareEstimationService.Services.Automapper;
using HG.SoftwareEstimationService.Services.Contract;
using HG.SoftwareEstimationService.SqliteData;

namespace HG.SoftwareEstimationService.Services
{
    public class ObservationsService : IObservationsService
    {
        private readonly IRepositorySqlite<StoryPart> _storypartRepo;
        private readonly IRepositorySqlite<StoryPartProperty> _storypartPropertyRepo;

        public ObservationsService(
            IRepositorySqlite<StoryPart> storypartRepo, 
            IRepositorySqlite<StoryPartProperty> storypartPropertyRepo)
        {
            _storypartRepo = storypartRepo;
            _storypartPropertyRepo = storypartPropertyRepo;
        }

        public IEnumerable<ObservationsGrid> GetObservations(int storyId)
        {
             return _storypartRepo
                .GetMany(x => x.StoryId == storyId)
                .Select(AutomapperRegistrar.Map<ObservationsGrid>)
                .ToList();
        }

        public void AddObservation(ObservationsGrid observation)
        {
            _storypartRepo.Add(AutomapperRegistrar.Map<StoryPart>(observation));
            _storypartRepo.SaveChanges();
        }

        public void UpdateObservation(ObservationsGrid observation)
        {
            StoryPart storyPart = AutomapperRegistrar.Map<StoryPart>(observation);
            _storypartRepo.Replace(o => o.StoryPartId, storyPart);
            _storypartRepo.SaveChanges();
        }

        public void DeleteObservation(int observationId)
        {
            _storypartRepo.Delete(_storypartRepo.GetById(observationId));
            _storypartPropertyRepo.DeleteMany(sp => sp.StoryPartId == observationId);
            _storypartRepo.SaveChanges();
            _storypartPropertyRepo.SaveChanges();
        }

        public PropertyValuesDto GetObservationProperties(int observationId)
        {
            StoryPart observation = _storypartRepo.GetById(observationId);
            StoryPartProperty[] properties = _storypartPropertyRepo
                .GetMany(s => s.StoryPartId == observationId)
                .ToArray();
            PropertyValue[] propertyValues = properties
                .Select(s => new PropertyValue { PropertyId = s.PropertyId, Value = s.PropertyValue.Value })
                .ToArray();
            long? partType = _storypartRepo.GetById(observationId).PartTypeId;
            if (partType == null)
            {
                return null;
            }
            
            return new PropertyValuesDto
            {
                StoryPartId = observationId,
                PartTypeId = partType.Value,
                PropertyValues = propertyValues,
                Description = observation.PartDescription
            };
        }

        public long SaveObservationProperties(PropertyValuesDto propertyValues)
        {
            if (propertyValues.StoryPartId == null)
            {
                propertyValues.StoryPartId = SaveNewObservationProperties(propertyValues);

            }
            else
            {
                SaveExistingObservationProperties(propertyValues.StoryPartId.Value, propertyValues.Description);
            }

            SaveObservationProperties(propertyValues.PropertyValues, propertyValues.StoryPartId.Value);
            _storypartRepo.SaveChanges();
            _storypartPropertyRepo.SaveChanges();

            return propertyValues.StoryPartId.Value;
        }

        private void SaveObservationProperties(PropertyValue[] propertyValues, long storyPartId){

            StoryPartProperty[] replacemenPartPropertiesRecords = _storypartPropertyRepo
                .GetMany(s => s.StoryPartId == storyPartId)
                .ToArray();

            int i = 0;
            foreach (PropertyValue propertyValue in propertyValues.Safe())
            {
                if (i >= replacemenPartPropertiesRecords.Length)
                {
                    _storypartPropertyRepo.Add(new StoryPartProperty
                    {
                        PropertyId = propertyValue.PropertyId,
                        PropertyValue = propertyValue.Value,
                        StoryPartId = storyPartId
                    });
                }
                else
                {
                    replacemenPartPropertiesRecords[i].PropertyId = propertyValue.PropertyId;
                    replacemenPartPropertiesRecords[i].PropertyValue = propertyValue.Value;
                    replacemenPartPropertiesRecords[i].StoryPartId = storyPartId;
                }

                i++;
            }

            for (int ii =i; ii < replacemenPartPropertiesRecords.Length; ii++)
            {
                _storypartPropertyRepo.Delete(replacemenPartPropertiesRecords[ii]);
            }
        }

        private void SaveExistingObservationProperties(long storyPartId, string description)
        {
            _storypartRepo.GetById(storyPartId).PartDescription = description;
        }

        private long SaveNewObservationProperties(PropertyValuesDto propertyValues)
        {
            StoryPart storyPart = new StoryPart
            {
                PartDescription = propertyValues.Description,
                StoryId = propertyValues.StoryId,
                PartTypeId = propertyValues.PartTypeId
            };
            
            _storypartRepo.Add(storyPart);
            _storypartRepo.SaveChanges();
            return storyPart.StoryPartId;
        }
    }
}
