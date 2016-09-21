using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Reflection;
using Castle.Core.Internal;
using HG.SoftwareEstimationService.Dto;
using HG.SoftwareEstimationService.Enums;
using HG.SoftwareEstimationService.Shared;

namespace HG.SoftwareEstimationService.Web.Helpers
{
    public class GridHelpers
    {
        public static List<DataGridColumn> GetColumnDefinitions(Type type)
        {
            return (from propertyInfo in type.GetProperties()
                let primaryKey = propertyInfo.GetAttribute<PrimaryKeyAttribute>() != null
                let required = propertyInfo.GetAttribute<RequiredAttribute>() != null
                let hideProperty = propertyInfo.GetAttribute<HideColumnAttribute>() != null
                let maxLengthAttribute = propertyInfo.GetAttribute<MaxLengthAttribute>()
                let maxLength = maxLengthAttribute == null ? 0 : maxLengthAttribute.Length
                let widthAttribute = propertyInfo.GetAttribute<ColumnWidthAttribute>()
                let width = widthAttribute != null ? widthAttribute.Width : 0
                let displayAttribute = propertyInfo.GetAttribute<DisplayAttribute>()
                let name = displayAttribute != null ? displayAttribute.Name : propertyInfo.Name
                let lookupAtt = propertyInfo.GetAttribute<LookupAttribute>()
                let lookup = lookupAtt == null ? null : (Lookup?) lookupAtt.Lookup
                let editableAtt = propertyInfo.GetAttribute<EditableAttribute>()
                let editable = !(editableAtt != null && !editableAtt.AllowEdit)
                select new DataGridColumn
                {
                    Name = propertyInfo.Name, 
                    PrimaryKey = primaryKey, 
                    HideProperty = hideProperty, 
                    Required = required, 
                    MaxLength = maxLength, 
                    GridType = GetDbType(propertyInfo), 
                    Width = width, 
                    DisplayName = name, 
                    Lookup = lookup, 
                    Editable = editable
                }).ToList();
        }

        private static GridType GetDbType(PropertyInfo propertyInfo)
        {
            if (propertyInfo.PropertyType == typeof(Int16))
                return GridType.Number;
            if (propertyInfo.PropertyType == typeof(Int32))
                return GridType.Number;
            if (propertyInfo.PropertyType == typeof(Int64))
                return GridType.Number;
            if (propertyInfo.PropertyType == typeof(Int16?))
                return GridType.Number;
            if (propertyInfo.PropertyType == typeof(Int32?))
                return GridType.Number;
            if (propertyInfo.PropertyType == typeof(Int64?))
                return GridType.Number;
            if (propertyInfo.PropertyType == typeof(string))
                return GridType.String;

            throw new DataException("Unsupported type in grid");
        }
    }
}