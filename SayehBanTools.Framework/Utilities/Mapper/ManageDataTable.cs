using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

namespace SayehBanTools.Framework.Utilities.Mapper
{
    public static class ManageDataTable
    {
        public static DataTable ConvertToDataTable<T>(IEnumerable<T> data, string tableName)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable(tableName);

            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);

            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }
    }
}
