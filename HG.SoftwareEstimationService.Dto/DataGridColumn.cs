using HG.SoftwareEstimationService.Enums;

namespace HG.SoftwareEstimationService.Dto
{
    public class DataGridColumn
    {
        public string Name { get; set; }
        public bool PrimaryKey { get; set; }
        public bool HideProperty { get; set; }
        public string DisplayName { get; set; }
        public int Width { get; set; }
        public GridType GridType { get; set; }
        public bool Required { get; set; }
        public int MaxLength { get; set; }
        public Lookup? Lookup { get; set; }
        public bool Editable { get; set; }
    }
}
