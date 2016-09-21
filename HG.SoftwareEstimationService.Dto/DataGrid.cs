using System.Collections.Generic;

namespace HG.SoftwareEstimationService.Dto
{
    public class DataGrid<T>
    {
        public IEnumerable<DataGridColumn> ColumnDefinition { get; set; }
        public IEnumerable<T> Data { get; set; }
    }
}
