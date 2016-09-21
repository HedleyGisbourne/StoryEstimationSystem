namespace HG.SoftwareEstimationService.Dto
{
    public class ApplicationConfigurationDto
    {
        public PartTypeDefinitionDto[] PartTypes { get; set; }
        public PropertyDto[] Properties { get; set; }
        public EnumerationDto[] Enumerations { get; set; }
    }

    public class PartTypeDefinitionDto
    {
        public long PartTypeId { get; set; }
        public string Name { get; set; }
        public long[] PropertyIds { get; set; }
    }

    public class PropertyDto
    {
        public long PropertyId { get; set; }
        public string Description { get; set; }
        public bool IsEnum { get { return EnumerationId != null; } }
        public long? EnumerationId { get; set; }
    }

    public class EnumerationDto
    {
        public long EnumerationId { get; set; }
        public EnumItem[] EnumItems { get; set; }
    }

    public class PropertyValuesDto
    {
        public long StoryId { get; set; }
        public long? StoryPartId { get; set; }
        public string Description { get; set; }
        public long PartTypeId { get; set; }
        public PropertyValue[] PropertyValues { get; set; }
    }
    public class PropertyValue
    {
        public long PropertyId { get; set; }
        public long Value { get; set; }
    }
}

