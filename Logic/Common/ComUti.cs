using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Data.SqlClient;
using System.Globalization;
using System.Threading;
using System.Text;
using System.Configuration;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.SqlServer.Server;
using ClosedXML.Excel;
using System.Web;
//using System.Web.Mvc;
using Microsoft.Extensions.Configuration;
using MalVirDetector_CLI_API;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using MalVirDetector_CLI_API.Web.Helpers;
using SelectPdf;

namespace MalVirDetector_CLI_API.Logic
{
    public class ComUti
    {
        public static string GetConnection()
        {
            return ClaimsModel.ConnectionString;// ConfigurationManager.ConnectionStrings["conn"].ConnectionString;
        }

        public static ApiResponse<T> Get_ApiResponse<T>(T data, string msg = "", bool isSuccess = true)
        {
            var res = new ApiResponse<T>
            {
                isSuccess = isSuccess,
                msg = msg,
                data = data
            };

            return res;
        }

        public static void MapObject(object sourceObject, object targetObject)
        {
            var sourceProp = sourceObject.GetType().GetProperties().ToDictionary(d => d.Name, d => d);
            var targetProp = targetObject.GetType().GetProperties();
            foreach (var item in targetProp)
            {
                if (sourceProp.ContainsKey(item.Name))
                {
                    var p = sourceProp[item.Name];
                    var val = p.GetValue(sourceObject);
                    if (p.PropertyType == item.PropertyType)
                    {
                        item.SetValue(targetObject, val);
                    }
                    else
                    {
                        item.SetValue(targetObject, val.To(item.PropertyType), null);
                    }
                }
            }
        }

        public static List<T> ConvertDataTable<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }
        private static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                        pro.SetValue(obj, dr[column.ColumnName], null);
                    else
                        continue;
                }
            }
            return obj;
        }

        #region Common Export
        public static string Export_Data(PDF_Table model)
        {
            string res = "";

            if (model.Type == "excel")
            {
                res = ExportHelper.Export_Excel_String(model);
            }
            else if (model.Type == "pdf")
            {
                res = ExportHelper.Export_PDF_Table(model);
            }
            else if (model.Type == "csv")
            {
                using (var ms = new MemoryStream())
                {
                    ExportHelper.ToCsv(model, ms);
                    res = Convert.ToBase64String(ms.ToArray());
                }
            }
            return res;
        }

        #endregion
    }

    public class DataManager
    {
        public static T Get<T, TKey>(Expression<Func<T, TKey>> expression, TKey value) where T : new()
        {
            Type t = typeof(T);
            MemberExpression memberExpression = null;

            if (expression.Body.NodeType == ExpressionType.Convert)
            {
                memberExpression = ((UnaryExpression)expression.Body).Operand as MemberExpression;
            }
            else if (expression.Body.NodeType == ExpressionType.MemberAccess)
            {
                memberExpression = expression.Body as MemberExpression;
            }
            if (memberExpression != null)
            {
                string name = memberExpression.Member.Name;
                string query = string.Format("SELECT * FROM {0} WHERE {1} = {2}", t.Name, name, value);
                var reader = SqlHelper.ExecuteReader(SqlHelper.GetConnection(), System.Data.CommandType.Text, query, null);
                var data = reader.MapToList<T>();
                return data[0];
            }
            else
            {
                return default(T);
            }
        }

        public static int ExecuteSPNonQeury<TParam>(TParam input, string ConnectionString = "")
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-IN");
            if (input != null)
            {
                try
                {
                    Type inType = typeof(TParam);
                    SqlProcedureAttribute attribute = inType.GetCustomAttributes(typeof(SqlProcedureAttribute), false).Cast<SqlProcedureAttribute>().FirstOrDefault();
                    if (attribute != null)
                    {
                        string spName = attribute.Name;
                        string connection = string.IsNullOrEmpty(ConnectionString) ? ComUti.GetConnection() : ConnectionString;
                        if (string.IsNullOrEmpty(connection))
                        {
                            return -1;
                        }
                        PropertyInfo[] propertyInfo = inType.GetProperties();
                        List<SqlParameter> sqlParameters = new List<SqlParameter>();
                        List<SqlParameter> outParameters = new List<SqlParameter>();
                        Dictionary<string, PropertyInfo> dictionary = new Dictionary<string, PropertyInfo>();
                        foreach (PropertyInfo propInfo in propertyInfo)
                        {
                            SqlParameterAttribute attr = propInfo.GetCustomAttribute(typeof(SqlParameterAttribute), false) as SqlParameterAttribute;
                            if (attr != null && !attr.NotBound && !string.IsNullOrEmpty(attr.Name))
                            {
                                SqlParameter sqlParameter = new SqlParameter();
                                sqlParameter.ParameterName = "@" + attr.Name;
                                sqlParameter.Value = propInfo.GetValue(input);
                                if (attr.IsOutput)
                                {
                                    dictionary.Add(attr.Name, propInfo);
                                    sqlParameter.Direction = System.Data.ParameterDirection.InputOutput;
                                    if (propInfo.PropertyType == typeof(string))
                                    {
                                        sqlParameter.Size = attr.Size;
                                    }
                                    outParameters.Add(sqlParameter);
                                }
                                sqlParameters.Add(sqlParameter);
                            }
                        }

                        int retVal = SqlHelper.ExecuteNonQuery(connection, System.Data.CommandType.StoredProcedure, spName, sqlParameters.ToArray());
                        foreach (SqlParameter sqlParameter in outParameters)
                        {
                            if (dictionary.ContainsKey(sqlParameter.ParameterName.Trim('@')))
                            {
                                PropertyInfo property = dictionary[sqlParameter.ParameterName.Trim('@')];
                                property.SetValue(input, sqlParameter.Value is DBNull ? null : sqlParameter.Value);
                            }
                        }
                        return retVal;
                    }
                }
                catch (Exception ex)
                {

                }
                return 0;
            }
            return 0;
        }

        public static void ExecuteSP<TParam>(TParam input, bool hasResult = true, string ConnectionString = "")
        {
            if (!string.IsNullOrEmpty(ConnectionString))
            {
                ExecuteProcedureWithConnectionString(input, hasResult, ConnectionString);
            }
            else
            {
                ExecuteProcedure(input, hasResult);
            }
        }

        public static T ExecuteSPGetSingle<T, TParam>(TParam input, bool hasResult = true, string ConnectionString = "")
        {
            T obj = default(T);
            if (!string.IsNullOrEmpty(ConnectionString))
            {
                var reader = ExecuteProcedureWithConnectionString(input, hasResult, ConnectionString); //ExecuteProcedure(input);
                if (reader != null)
                {
                    obj = reader.MapToSingle<T>();
                }
                reader.Close();
            }
            else
            {
                var reader = ExecuteProcedure(input);
                if (reader != null)
                {
                    obj = reader.MapToSingle<T>();
                }
                reader.Close();
            }

            return obj;
        }
        public static Tuple<List<T1>, T2> ExecuteSP_GridList<T1, T2, TParam>(TParam input)
        {
            List<T1> list1 = new List<T1>();
            T2 obj = default(T2);
            var reader = ExecuteProcedure(input);
            if (reader != null)
            {
                list1 = reader.MapToList<T1>();
                if (reader.NextResult())
                {
                    obj = reader.MapToSingle<T2>();
                }
                reader.Close();
            }
            Tuple<List<T1>, T2> tuple = new Tuple<List<T1>, T2>(list1, obj);
            return tuple;
        }
        public static Tuple<List<T1>, T2, List<T3>> ExecuteSP_GridList<T1, T2, T3, TParam>(TParam input)
        {
            List<T1> list1 = new List<T1>();
            T2 obj = default(T2);
            List<T3> list3 = new List<T3>();
            var reader = ExecuteProcedure(input);
            if (reader != null)
            {
                list1 = reader.MapToList<T1>();
                if (reader.NextResult())
                {
                    obj = reader.MapToSingle<T2>();
                    if (reader.NextResult())
                    {
                        list3 = reader.MapToList<T3>();
                    }
                }
                reader.Close();
            }

            Tuple<List<T1>, T2, List<T3>> tuple = new Tuple<List<T1>, T2, List<T3>>(list1, obj, list3);
            return tuple;
        }



        public static List<T> ExecuteSPGetList<T, TParam>(TParam input, string ConnectionString = "")
        {
            System.Data.IDataReader reader;
            if (string.IsNullOrEmpty(ConnectionString))
            {
                reader = ExecuteProcedure(input);
            }
            else
            {
                reader = ExecuteProcedureWithConnectionString(input, true, ConnectionString);
            }
            List<T> list = new List<T>();
            if (reader != null)
            {
                list = reader.MapToList<T>();
            }

            return list;
        }

        public static Tuple<List<T1>, List<T2>> ExecuteSPGetList<T1, T2, TParam>(TParam input, string ConnectionString = "")
        {
            List<T1> list1 = new List<T1>();
            List<T2> list2 = new List<T2>();
            if (!string.IsNullOrEmpty(ConnectionString))
            {
                var reader = ExecuteProcedureWithConnectionString(input, true, ConnectionString); //ExecuteProcedure(input);
                if (reader != null)
                {
                    list1 = reader.MapToList<T1>();
                    if (reader.NextResult())
                    {
                        list2 = reader.MapToList<T2>();
                    }
                    reader.Close();
                }
            }
            else
            {
                var reader = ExecuteProcedure(input);
                if (reader != null)
                {
                    list1 = reader.MapToList<T1>();
                    if (reader.NextResult())
                    {
                        list2 = reader.MapToList<T2>();
                    }
                    reader.Close();
                }
            }
            Tuple<List<T1>, List<T2>> tuple = new Tuple<List<T1>, List<T2>>(list1, list2);
            return tuple;
        }

        public static Tuple<List<T1>, List<T2>, List<T3>> ExecuteSPGetList<T1, T2, T3, TParam>(TParam input, string ConnectionString = "")
        {
            List<T1> list1 = new List<T1>();
            List<T2> list2 = new List<T2>();
            List<T3> list3 = new List<T3>();
            if (!string.IsNullOrEmpty(ConnectionString))
            {
                var reader = ExecuteProcedureWithConnectionString(input, true, ConnectionString);
                if (reader != null)
                {
                    list1 = reader.MapToList<T1>();
                    if (reader.NextResult())
                    {
                        list2 = reader.MapToList<T2>();
                        if (reader.NextResult())
                        {
                            list3 = reader.MapToList<T3>();
                        }
                    }
                    reader.Close();
                }
            }
            else
            {
                var reader = ExecuteProcedure(input);
                if (reader != null)
                {
                    list1 = reader.MapToList<T1>();
                    if (reader.NextResult())
                    {
                        list2 = reader.MapToList<T2>();
                        if (reader.NextResult())
                        {
                            list3 = reader.MapToList<T3>();
                        }
                    }
                    reader.Close();
                }
            }

            Tuple<List<T1>, List<T2>, List<T3>> tuple = new Tuple<List<T1>, List<T2>, List<T3>>(list1, list2, list3);
            return tuple;
        }

        public static Tuple<List<T1>, List<T2>, List<T3>, List<T4>> ExecuteSPGetList<T1, T2, T3, T4, TParam>(TParam input, string ConnectionString = "")
        {
            List<T1> list1 = new List<T1>();
            List<T2> list2 = new List<T2>();
            List<T3> list3 = new List<T3>();
            List<T4> list4 = new List<T4>();
            if (!string.IsNullOrEmpty(ConnectionString))
            {
                var reader = ExecuteProcedureWithConnectionString(input, true, ConnectionString);
                if (reader != null)
                {
                    list1 = reader.MapToList<T1>();
                    if (reader.NextResult())
                    {
                        list2 = reader.MapToList<T2>();
                        if (reader.NextResult())
                        {
                            list3 = reader.MapToList<T3>();
                            if (reader.NextResult())
                            {
                                list4 = reader.MapToList<T4>();
                            }
                        }
                    }
                    reader.Close();
                }
            }
            else
            {
                var reader = ExecuteProcedure(input);
                if (reader != null)
                {
                    list1 = reader.MapToList<T1>();
                    if (reader.NextResult())
                    {
                        list2 = reader.MapToList<T2>();
                        if (reader.NextResult())
                        {
                            list3 = reader.MapToList<T3>();
                            if (reader.NextResult())
                            {
                                list4 = reader.MapToList<T4>();
                            }
                        }
                    }
                    reader.Close();
                }
            }

            Tuple<List<T1>, List<T2>, List<T3>, List<T4>> tuple = new Tuple<List<T1>, List<T2>, List<T3>, List<T4>>(list1, list2, list3, list4);
            return tuple;
        }

        private static System.Data.IDataReader ExecuteProcedure<TParam>(TParam input, bool hasResult = true, string ConnectionString = "")
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-IN");
            if (input != null)
            {
                try
                {
                    Type inType = typeof(TParam);
                    SqlProcedureAttribute attribute = inType.GetCustomAttributes(typeof(SqlProcedureAttribute), false).Cast<SqlProcedureAttribute>().FirstOrDefault();
                    if (attribute != null)
                    {
                        string spName = attribute.Name;
                        string connection = string.IsNullOrEmpty(ConnectionString) ? ComUti.GetConnection() : ConnectionString;
                        PropertyInfo[] propertyInfo = inType.GetProperties();
                        List<SqlParameter> sqlParameters = new List<SqlParameter>();
                        List<SqlParameter> outParameters = new List<SqlParameter>();
                        Dictionary<string, PropertyInfo> dictionary = new Dictionary<string, PropertyInfo>();
                        foreach (PropertyInfo propInfo in propertyInfo)
                        {
                            SqlParameterAttribute attr = propInfo.GetCustomAttribute(typeof(SqlParameterAttribute), false) as SqlParameterAttribute;
                            if (attr != null && !attr.NotBound && !string.IsNullOrEmpty(attr.Name))
                            {
                                SqlParameter sqlParameter = new SqlParameter();
                                sqlParameter.ParameterName = "@" + attr.Name;
                                sqlParameter.Value = propInfo.GetValue(input);
                                if (attr.IsOutput)
                                {
                                    dictionary.Add(attr.Name, propInfo);
                                    sqlParameter.Direction = System.Data.ParameterDirection.InputOutput;
                                    if (propInfo.PropertyType == typeof(string))
                                    {
                                        sqlParameter.Size = attr.Size;
                                    }
                                    outParameters.Add(sqlParameter);
                                }
                                sqlParameters.Add(sqlParameter);
                            }
                        }
                        SqlDataReader sqlDataReader = null; int res = 0;
                        //connection.ConnectionTimeout = 180;//3 minuites
                        if (hasResult)
                            sqlDataReader = SqlHelper.ExecuteReader(connection, System.Data.CommandType.StoredProcedure, spName, sqlParameters.ToArray());
                        else
                            res = SqlHelper.ExecuteNonQuery(connection, System.Data.CommandType.StoredProcedure, spName, sqlParameters.ToArray());
                        foreach (SqlParameter sqlParameter in outParameters)
                        {
                            if (dictionary.ContainsKey(sqlParameter.ParameterName.Trim('@')))
                            {
                                PropertyInfo property = dictionary[sqlParameter.ParameterName.Trim('@')];
                                property.SetValue(input, sqlParameter.Value is DBNull ? null : sqlParameter.Value);
                            }
                        }
                        return sqlDataReader;
                    }
                }
                catch (Exception ex)
                {

                }
            }
            return null;
        }

        private static System.Data.IDataReader ExecuteProcedureWithConnectionString<TParam>(TParam input, bool hasResult = true, string ConnectionString = "")
        {
            if (input != null)
            {
                try
                {
                    Type inType = typeof(TParam);
                    SqlProcedureAttribute attribute = inType.GetCustomAttributes(typeof(SqlProcedureAttribute), false).Cast<SqlProcedureAttribute>().FirstOrDefault();
                    if (attribute != null)
                    {
                        string spName = attribute.Name;
                        PropertyInfo[] propertyInfo = inType.GetProperties();
                        List<SqlParameter> sqlParameters = new List<SqlParameter>();
                        List<SqlParameter> outParameters = new List<SqlParameter>();
                        Dictionary<string, PropertyInfo> dictionary = new Dictionary<string, PropertyInfo>();
                        foreach (PropertyInfo propInfo in propertyInfo)
                        {
                            SqlParameterAttribute attr = propInfo.GetCustomAttribute(typeof(SqlParameterAttribute), false) as SqlParameterAttribute;
                            if (attr != null && !attr.NotBound && !string.IsNullOrEmpty(attr.Name))
                            {
                                SqlParameter sqlParameter = new SqlParameter();
                                sqlParameter.ParameterName = "@" + attr.Name;
                                sqlParameter.Value = propInfo.GetValue(input);
                                if (attr.IsOutput)
                                {
                                    dictionary.Add(attr.Name, propInfo);
                                    sqlParameter.Direction = System.Data.ParameterDirection.InputOutput;
                                    if (propInfo.PropertyType == typeof(string))
                                    {
                                        sqlParameter.Size = attr.Size;
                                    }
                                    outParameters.Add(sqlParameter);
                                }
                                sqlParameters.Add(sqlParameter);
                            }
                        }
                        SqlDataReader sqlDataReader = null;
                        if (hasResult)
                            sqlDataReader = SqlHelper.ExecuteReader(ConnectionString, System.Data.CommandType.StoredProcedure, spName, sqlParameters.ToArray());
                        else
                            SqlHelper.ExecuteNonQuery(ConnectionString, System.Data.CommandType.StoredProcedure, spName, sqlParameters.ToArray());
                        foreach (SqlParameter sqlParameter in outParameters)
                        {
                            if (dictionary.ContainsKey(sqlParameter.ParameterName.Trim('@')))
                            {
                                PropertyInfo property = dictionary[sqlParameter.ParameterName.Trim('@')];
                                property.SetValue(input, sqlParameter.Value is DBNull ? null : sqlParameter.Value);
                            }
                        }
                        return sqlDataReader;
                    }
                }
                catch (Exception ex)
                {

                }
            }
            return null;
        }


        public static DataTable ExecuteSPDataTable<TParam>(TParam input, string ConnectionString = "")
        {
            DataTable dataTable = new DataTable();
            try
            {
                IDataReader dataReader;
                if (!string.IsNullOrEmpty(ConnectionString))
                {
                    dataReader = ExecuteProcedureWithConnectionString(input, true, ConnectionString);
                }
                else
                {
                    dataReader = ExecuteProcedure(input, true);
                }
                //dataReader = ExecuteProcedure(input, true);                
                dataTable.Load(dataReader);
            }
            catch (Exception ex)
            {

            }
            return dataTable;
        }

        public static DataSet ExecuteSPDataSet<TParam>(TParam input, string ConnectionString = "")
        {
            DataSet dataSet = new DataSet();
            try
            {
                IDataReader dataReader;
                if (!string.IsNullOrEmpty(ConnectionString))
                {
                    dataReader = ExecuteProcedureWithConnectionString(input, true, ConnectionString);
                }
                else
                {
                    dataReader = ExecuteProcedure(input, true);
                }
                while (!dataReader.IsClosed)
                    dataSet.Tables.Add().Load(dataReader);
            }
            catch (Exception ex)
            {

            }
            return dataSet;
        }

    }



    public static class ExportHelper
    {
        private const string Quote = "\"";
        private const string EscapedQuote = "\"\"";
        private static readonly char[] EscapableCharacters = { '"', ',', '\r', '\n' };

        //public static void ToCsv<T>(IEnumerable<T> collection, Stream stream, params string[] onlyFields) where T : class
        //{
        //    Dictionary<PropertyInfo, string> fieldNames = null;
        //    var sw = new StreamWriter(stream, Encoding.UTF8);

        //    foreach (var item in collection)
        //    {
        //        // Only on the first iteration we get the list of properties from the object type
        //        // We use a dictionary of <PropertyInfo, string> instead of just a list of PropertyInfo, 
        //        // because we extract the display name of the property (if exists) to use it as the "column" header
        //        if (fieldNames == null)
        //        {
        //            fieldNames = GetProperties(typeof(T), onlyFields);
        //            // Write the column headers
        //            WriteRow(sw, fieldNames.Select(v => v.Value));
        //        }

        //        var current = item;
        //        var valueList = fieldNames.Keys.Select(prop => prop.GetValue(current, null))
        //            .Select(Convert.ToString);

        //        WriteRow(sw, valueList);
        //    }

        //    // Reset the stream position to the beginning
        //    stream.Seek(0, SeekOrigin.Begin);
        //}

        public static void ToCsv(PDF_Table model, Stream stream)
        {
            var sw = new StreamWriter(stream, Encoding.UTF8);
            WriteRow(sw, model.Columns.Select(d => d.DisplayText));
            if (model.list != null)
            {
                foreach (var item in model.list)
                {
                    var valueList = new List<string>();
                    foreach (var column in model.Columns)
                    {
                        column.Type = column.Type.ToLower();
                        string val = item[column.ColumnName];
                        val = val == null ? "" : val.ToString();
                        if (!string.IsNullOrEmpty(val))
                        {
                            if (column.Is_Price) { val = String.Format("{0:n2}", Convert.ToDecimal(val)); }
                            if (column.Type == "date") { val = Convert.ToString(Convert.ToDateTime(item[column.ColumnName]).ToString("dd MMM yyyy")); }
                            if (column.Type == "datetime") { val = Convert.ToString(Convert.ToDateTime(item[column.ColumnName]).ToString("dd MMM yyyy hh:mm tt")); }
                            if (column.Type == "bool") { if (Convert.ToBoolean(val)) { val = "true"; } else { val = "false"; } }
                        }
                        valueList.Add(val);
                    }
                    WriteRow(sw, valueList);
                }
            }
            if (model.lstTotal != null)
            {
                foreach (var item in model.lstTotal)
                {
                    int j = 0;
                    var valueList = new List<string>();
                    foreach (var column in model.Columns)
                    {
                        if (column.Is_Sum || j == 0)
                        {
                            string val = item[column.ColumnName];
                            val = val == null ? "" : val.ToString();
                            if (!string.IsNullOrEmpty(val))
                            {
                                if (column.Is_Price) { val = String.Format("{0:n2}", Convert.ToDecimal(val)); }
                            }
                            if (j == 0) { valueList.Add("Total"); }
                            if (j > 0) { valueList.Add(val); }
                        }
                        else { valueList.Add(""); }
                        j++;
                    }
                    WriteRow(sw, valueList);
                }
            }

            // Reset the stream position to the beginning
            stream.Seek(0, SeekOrigin.Begin);
        }

        private static bool IsSimpleOrNullableType(Type t)
        {
            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                t = Nullable.GetUnderlyingType(t);
            }

            return IsSimpleType(t);
        }

        private static bool IsSimpleType(Type t)
        {
            return t.IsPrimitive || t.IsEnum || t == typeof(string) || t == typeof(Decimal) || t == typeof(DateTime) || t == typeof(Guid);
        }

        private static void WriteRow(TextWriter sw, IEnumerable<string> values)
        {
            int index = 0;
            foreach (var value in values)
            {
                if (index > 0)
                {
                    sw.Write(",");
                }

                WriteValue(sw, value);
                index++;
            }

            sw.Write(Environment.NewLine);
            sw.Flush();
        }

        private static void WriteValue(TextWriter sw, string value)
        {
            bool needsEscaping = value.IndexOfAny(EscapableCharacters) >= 0;

            if (needsEscaping)
            {
                sw.Write(Quote);
                sw.Write(value.Replace(Quote, EscapedQuote));
                sw.Write(Quote);
            }
            else
            {
                sw.Write(value);
            }
        }

        public static MemoryStream GetCSV(DataTable data)
        {
            string[] fieldsToExpose = new string[data.Columns.Count];
            for (int i = 0; i < data.Columns.Count; i++)
            {
                fieldsToExpose[i] = data.Columns[i].ColumnName;
            }

            return GetCSV(fieldsToExpose, data);
        }

        public static MemoryStream GetCSV(string[] fieldsToExpose, DataTable data)
        {
            MemoryStream stream = new MemoryStream();
            using (var writer = new StreamWriter(stream))
            {
                for (int i = 0; i < fieldsToExpose.Length; i++)
                {
                    if (i != 0) { writer.Write(","); }
                    writer.Write("\"");
                    writer.Write(fieldsToExpose[i].Replace("\"", "\"\""));
                    writer.Write("\"");
                }
                writer.Write("\n");

                foreach (DataRow row in data.Rows)
                {
                    for (int i = 0; i < fieldsToExpose.Length; i++)
                    {
                        if (i != 0) { writer.Write(","); }
                        writer.Write("\"");
                        writer.Write(row[fieldsToExpose[i]].ToString()
                            .Replace("\"", "\"\""));
                        writer.Write("\"");
                    }
                    writer.Write("\n");
                }
            }
            return stream;
        }

        public static string Export_Excel_String(PDF_Table model)
        {
            string res = "";
            try
            {
                //var property = typeof(T).GetProperties();
                using (XLWorkbook wb = new XLWorkbook())
                {
                    if (string.IsNullOrEmpty(model.FileName)) { model.FileName = "sheet"; }

                    var ws = wb.Worksheets.Add(model.FileName);

                    int skiprow = 1;//skip header row
                    int skipcol = 1;

                    // Merge a row
                    if (!string.IsNullOrEmpty(model.CompanyName))
                    {
                        var row = ws.Row(skiprow);
                        var cell = ws.Cell("A" + skiprow);
                        row.Height = 25;
                        row.Style.Font.Bold = true;
                        row.Style.Font.FontSize = 13;
                        cell.Value = "Company Name : " + model.CompanyName;
                        cell.Style.Alignment.WrapText = true;
                        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#f8f8f8");
                        ws.Range(skiprow, skipcol, skiprow, model.Columns.Count()).Row(1).Merge();//ws.Range("A1:D3").Row(1).Merge();                        
                        skiprow += 1;
                    }
                    if (!string.IsNullOrEmpty(model.PartyName))
                    {
                        var row = ws.Row(skiprow);
                        var cell = ws.Cell("A" + skiprow);
                        row.Height = 25;
                        row.Style.Font.Bold = true;
                        row.Style.Font.FontSize = 13;
                        cell.Value = "Party Name : " + model.PartyName;
                        cell.Style.Alignment.WrapText = true;
                        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#f8f8f8");

                        ws.Range(skiprow, skipcol, skiprow, model.Columns.Count()).Row(1).Merge();
                        skiprow += 1;
                    }
                    if (!string.IsNullOrEmpty(model.StartDate) && !string.IsNullOrEmpty(model.EndDate))
                    {
                        var row = ws.Row(skiprow);
                        var cell = ws.Cell("A" + skiprow);
                        row.Height = 25;
                        row.Style.Font.Bold = true;
                        row.Style.Font.FontSize = 13;

                        cell.Value = model.StartDate + " To " + model.EndDate;
                        cell.Style.Alignment.WrapText = true;
                        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#f8f8f8");
                        ws.Range(skiprow, 1, skiprow, model.Columns.Count()).Row(1).Merge();
                        skiprow += 1;
                    }

                    CreateColumnHeaders(ws, model.Columns, skiprow);
                    CreateWorksheetData(model, ws, skiprow + 1);

                    ws.Columns().AdjustToContents();
                    //save file to memory stream and return it as byte array
                    using (var ms = new MemoryStream())
                    {
                        //var path = HostingEnvironment.MapPath("~/fonts/test.xlsx");
                        //wb.SaveAs(path);
                        wb.SaveAs(ms);
                        res = Convert.ToBase64String(ms.ToArray());

                        //HttpContext.Current.Response.ContentType = "application/octet-stream";
                        //HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=" + model.FileName + ".xlsx");
                        //HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                        //HttpContext.Current.Response.BinaryWrite(ms.ToArray());
                        //HttpContext.Current.Response.End();
                        //HttpContext.Current.Response.Flush();
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return res;
        }

        private static void CreateColumnHeaders(IXLWorksheet worksheet, List<GridFilter> Columns, int skiprow)
        {
            var col = 1;
            //var row = worksheet.Row(skiprow);
            //row.Style.Font.FontSize = 12;
            foreach (var column in Columns)
            {
                worksheet.Row(skiprow).Height = 20;
                var cell = worksheet.Cell(skiprow, col);

                cell.Value = column.DisplayText;
                if (!string.IsNullOrEmpty(column.TextAlign) && column.TextAlign.Contains("right"))
                {
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                }
                else { cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center; }
                cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#e5f4ff");
                cell.Style.Font.Bold = true;
                //cell.Style.Font.FontSize = 12;
                col++;
            }
        }

        private static void CreateWorksheetData(PDF_Table model, IXLWorksheet worksheet, int skiprow)
        {
            try
            {

                var row = skiprow;
                if (model.list != null)
                {
                    foreach (var item in model.list)
                    {
                        var col = 1;
                        foreach (var column in model.Columns)
                        {
                            column.Type = column.Type.ToLower();
                            string val = item[column.ColumnName];
                            val = val == null ? "" : val.ToString();
                            var cell = worksheet.Cell(row, col);

                            if (!string.IsNullOrEmpty(val))
                            {
                                if (column.Is_Price) { val = String.Format("{0:n2}", Convert.ToDecimal(val)); }
                                if (column.Type == "date") { val = Convert.ToDateTime(item[column.ColumnName]).ToString("dd MMM yyyy"); }
                                if (column.Type == "datetime") { val = Convert.ToDateTime(item[column.ColumnName]).ToString("dd MMM yyyy hh:mm tt"); }
                                if (column.Type == "bool") { if (Convert.ToBoolean(val)) { val = "true"; } else { val = "false"; } }

                                cell.SetValue<string>(val);
                            }
                            else { cell.Value = val; }
                            if (!string.IsNullOrEmpty(column.TextAlign) && column.TextAlign.Contains("right"))
                            {
                                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                            }
                            col++;
                        }
                        row++;
                    }
                }
                var frow = model.list.Count + skiprow;
                if (model.lstTotal != null)
                {
                    foreach (var item in model.lstTotal)
                    {
                        var col = 1;
                        foreach (var column in model.Columns)
                        {
                            worksheet.Row(frow).Height = 20;
                            var cell = worksheet.Cell(frow, col);
                            if (column.Is_Sum || col == 1)
                            {
                                string val = item[column.ColumnName];
                                val = val == null ? "" : val.ToString();
                                if (!string.IsNullOrEmpty(val))
                                {
                                    if (column.Is_Price) { val = String.Format("{0:n2}", Convert.ToDecimal(val)); }
                                }

                                if (col == 1) { cell.Value = "Total"; }
                                if (col > 1) { cell.Value = val; }

                                if (!string.IsNullOrEmpty(column.TextAlign) && column.TextAlign.Contains("right"))
                                {
                                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                                }
                                else { cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center; }
                            }
                            else { cell.Value = ""; }
                            cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                            cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#f8f8f8");
                            cell.Style.Font.Bold = true;
                            col++;
                        }
                        frow++;
                    }
                }

                var cells = worksheet.Cells();
                cells.Style.Border.RightBorder = XLBorderStyleValues.Thin;
                cells.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                cells.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                cells.Style.Border.TopBorder = XLBorderStyleValues.Thin;
                cells.Style.Font.FontSize = 11;

            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public static string Export_PDF_Table(PDF_Table model)
        {
            string res = "";
            try
            {
                var newHtml = "";
                StringBuilder sb = new StringBuilder();
                sb.Append("<html><head><meta http-equiv='Content-Type' content='text/html; charset=utf-8'/></head><body>");
                sb.Append("<style>");
                sb.Append("body{padding:0;margin:0;}table {width:100%;page-break-inside: auto;}tr {page-break-inside: avoid;page-break-after: auto;}thead {display: table-header-group;}tfoot {display: table-footer-group;}.dvTable{width:100%;margin:0 auto;font-family: arial;color:#333;}.dvTable table{width:100%;border-collapse:separate;border-collapse:collapse;border-spacing:0;border:1px solid #333}table td,table th{border:1px solid #333;padding:6px;text-align:left;letter-spacing: 0.7px;}.totalrow td,table th{font-size:13px;background: #f8f8f8;color:#333;}table td{font-size:13px;}.center,.text-center{text-align:center}.right,.text-right{text-align:right}.bold{font-weight:700}.nowrap{white-space: nowrap;.white-space-normal, .white-space-normal a{white-space: normal !important;}}");
                sb.Append("</style>");
                sb.Append("<div class='dvTable'>");
                sb.Append("<table>");
                sb.Append("<thead>");
                if (!string.IsNullOrEmpty(model.CompanyName))
                {
                    sb.Append("<tr><th colspan='" + model.Columns.Count + "' class='center bold'>Company Name : " + model.CompanyName + "</th></tr>");
                }
                if (!string.IsNullOrEmpty(model.PartyName))
                {
                    sb.Append("<tr><th colspan='" + model.Columns.Count + "' class='center bold'>Party Name : " + model.PartyName + "</th></tr>");
                }
                if (!string.IsNullOrEmpty(model.StartDate) && !string.IsNullOrEmpty(model.EndDate))
                {
                    sb.Append("<tr><th colspan='" + model.Columns.Count + "' class='center bold'>" + model.StartDate + " To " + model.EndDate + "</th></tr>");
                }
                sb.Append("<tr>");
                foreach (var column in model.Columns)
                {
                    if (column.Width > 0) { sb.Append("<th class='" + column.TextAlign + "' width='" + column.Width + "%' >" + column.DisplayText + "</th>"); }
                    else { sb.Append("<th class='" + column.TextAlign + "'  >" + column.DisplayText + "</th>"); }
                }
                sb.Append("</tr>");
                sb.Append("</thead>");
                sb.Append("<tbody>");

                try
                {

                    int i = 0; string cls = "";
                    if (model.list != null)
                    {
                        foreach (var item in model.list)
                        {
                            sb.Append("<tr style='page-break-inside: avoid;'>");
                            foreach (var column in model.Columns)
                            {
                                column.Type = column.Type.ToLower();
                                string val = item[column.ColumnName];
                                val = val == null ? "" : val.ToString();
                                if (!string.IsNullOrEmpty(val))
                                {
                                    if (column.Is_Price) { val = String.Format("{0:n2}", Convert.ToDecimal(val)); }
                                    if (column.Type == "date") { val = Convert.ToDateTime(item[column.ColumnName]).ToString("dd MMM yyyy"); }
                                    if (column.Type == "datetime") { val = Convert.ToDateTime(item[column.ColumnName]).ToString("dd MMM yyyy hh:mm tt"); }
                                    if (column.Type == "bool") { if (Convert.ToBoolean(val)) { val = "true"; } else { val = "false"; } }
                                }

                                sb.Append("<td style='page-break-inside: avoid;' class='" + cls + " " + column.TextAlign + "'>" + val + "</td>");
                            }
                            i++;
                            sb.Append("</tr>");
                        }
                    }
                    if (model.lstTotal != null)
                    {
                        foreach (var item in model.lstTotal)
                        {
                            int j = 0;
                            sb.Append("<tr style='page-break-inside: avoid;' class='totalrow bold'>");
                            foreach (var column in model.Columns)
                            {
                                if (column.Is_Sum || j == 0)
                                {
                                    string val = item[column.ColumnName];
                                    val = val == null ? "" : val.ToString();
                                    if (!string.IsNullOrEmpty(val))
                                    {
                                        if (column.Is_Price) { val = String.Format("{0:n2}", Convert.ToDecimal(val)); }
                                    }
                                    if (j == 0) { sb.Append("<td>Total</td>"); }
                                    if (j > 0) { sb.Append("<td style='page-break-inside: avoid;' class='" + column.TextAlign + "'>" + val + "</td>"); }
                                }
                                else { sb.Append("<td></td>"); }
                                j++;
                            }
                            sb.Append("</tr>");
                        }
                    }

                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }
                sb.Append("</tbody></table></div></body></html>");
                newHtml = sb.ToString();

                // instantiate a html to pdf converter object
                HtmlToPdf converter = new HtmlToPdf();
                // set converter options
                if (model.Columns.Count > 11)
                {
                    converter.Options.PdfPageOrientation = PdfPageOrientation.Landscape;
                    converter.Options.PdfPageSize = PdfPageSize.A3;
                }
                else
                {
                    converter.Options.PdfPageSize = PdfPageSize.Letter;
                }
                converter.Options.AutoFitWidth = HtmlToPdfPageFitMode.AutoFit;
                converter.Options.MarginTop = 10; converter.Options.MarginBottom = 10;
                converter.Options.MarginLeft = 10; converter.Options.MarginRight = 10;

                PdfDocument doc = converter.ConvertHtmlString(newHtml);
                // save pdf document
                //doc.Save(Response, false, "Sample.pdf");
                using (var ms = new MemoryStream())
                {
                    doc.Save(ms);
                    res = Convert.ToBase64String(ms.ToArray());
                }

                // close pdf document
                doc.Close();

                ////NReco html to pdf
                //var htmlToPdf = new NReco.PdfGenerator.HtmlToPdfConverter();
                //htmlToPdf.LowQuality = true;//false;
                ////htmlToPdf.Quiet = false;
                //htmlToPdf.Margins = new NReco.PdfGenerator.PageMargins { Top = 5, Bottom = 0, Left = 5, Right = 5 };

                //if (model.Columns.Count > 10)
                //{
                //    htmlToPdf.Orientation = NReco.PdfGenerator.PageOrientation.Landscape;
                //    htmlToPdf.Zoom = (float)(1.50);
                //    htmlToPdf.Size = NReco.PdfGenerator.PageSize.A3;
                //}
                //else
                //{
                //    htmlToPdf.Zoom = (float)(1.25);
                //    htmlToPdf.Size = NReco.PdfGenerator.PageSize.Letter;
                //}
                //using (var ms = new MemoryStream())
                //{
                //    htmlToPdf.GeneratePdf(newHtml, "", ms);
                //    res = Convert.ToBase64String(ms.ToArray());

                //    //HttpContext.Current.Response.ContentType = "application/pdf";
                //    //HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=" + model.FileName + ".pdf");
                //    //HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                //    //HttpContext.Current.Response.BinaryWrite(ms.ToArray());
                //    //HttpContext.Current.Response.End();
                //    //HttpContext.Current.Response.Flush();
                //}

            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return res;
        }

        public static string Export_PDF_Html(string html, string FileName, bool Is_A3 = false)
        {
            string res = "";
            try
            {
                if (string.IsNullOrEmpty(FileName)) { FileName = "export_pdf"; }
                StringBuilder sb = new StringBuilder();
                sb.Append("<html><head><meta http-equiv='Content-Type' content='text/html; charset=utf-8'/></head><body>");
                sb.Append("<style>.doNotPrint_btn{display:none;}</style>");
                sb.Append(html);
                sb.Append("</body></html>");
                html = sb.ToString();
                //var newHtml = "";
                //var logo = HttpContext.Current.Server.MapPath("~/Documents/Profiles/cust_1.jpg");
                //sb.Append("<html><head><meta http-equiv='Content-Type' content='text/html; charset=utf-8'/></head><body style='margin: 0px;padding: 0px;'>");
                //sb.Append("<style> .tbl {border-collapse: collapse;border:1px solid #ddd;width:100%;} .tbl th,.tbl td {text-align: left;padding: 7px 4px;font-size:13px;border: 1px solid #ddd !important;}");
                //sb.Append(".btn{box-sizing: border-box;display: inline-block;padding: 6px 12px;margin-bottom: 0;font-size: 14px;font-weight: normal;line-height: 1.428571429;  }");
                //sb.Append("text-align: center;white-space: nowrap;vertical-align: middle;border: 1px solid transparent;border-radius: 4px;}");
                //sb.Append(".btn-custome-white{background-color: #fff}.btn-custome-red{background-color: #d9534f;}");
                //sb.Append(".btn-custome-orange{background-color: #f0ad4e;}.btn-custome-yellow{background-color: #f0ad4e;}");
                //sb.Append(".btn-custome-green{background-color: #5cb85c;}.btn-custome-circle{width: 20px;height: 20px;text-align: center;padding: 6px 0;font-size: 12px;border:1px solid #717171;");
                //sb.Append("line-height: 18px;border-radius: 50%;-moz-border-radius: 50%;-webkit-border-radius: 50%;}");
                //sb.Append("</style>");
                //sb.Append("<div class='tbl'>");
                //sb.Append("<img alt='' src='" + logo + "' /><br/><p style='color:red;font-size:30px;'></p>");
                //sb.Append("</table>");
                //sb.Append("</body></html>");
                //newHtml = sb.ToString();


                // instantiate a html to pdf converter object
                HtmlToPdf converter = new HtmlToPdf();
                // set converter options
                if (Is_A3) { converter.Options.PdfPageSize = PdfPageSize.A3; }
                else { converter.Options.PdfPageSize = PdfPageSize.A4; }
                converter.Options.AutoFitWidth = HtmlToPdfPageFitMode.AutoFit;
                converter.Options.MarginTop = 7; converter.Options.MarginBottom = 0;
                converter.Options.MarginLeft = 5; converter.Options.MarginRight = 5;
                //converter.Options.JpegCompressionEnabled = true;
                converter.Options.PdfCompressionLevel = PdfCompressionLevel.Normal;

                PdfDocument doc = converter.ConvertHtmlString(html);
                // save pdf document
                //doc.Save(Response, false, "Sample.pdf");
                using (var ms = new MemoryStream())
                {
                    doc.Save(ms);
                    res = Convert.ToBase64String(ms.ToArray());
                }

                // close pdf document
                doc.Close();

                //using (var ms = new MemoryStream())
                //{
                //    htmlToPdf.GeneratePdf(html, "", ms);
                //    res = Convert.ToBase64String(ms.ToArray());

                //    //HttpContext.Current.Response.ContentType = "application/pdf";
                //    //HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=" + FileName + ".pdf");
                //    //HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                //    //HttpContext.Current.Response.BinaryWrite(ms.ToArray());
                //    //HttpContext.Current.Response.End();
                //    //HttpContext.Current.Response.Flush();

                //    //need to alternate of HttpContext
                //}
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return res;
        }
    }

    //public class CAAuthorize : AuthorizeAttribute
    //{
    //    protected override bool AuthorizeCore(HttpContextBase httpContext)
    //    {
    //        //var authroized = base.AuthorizeCore(httpContext);
    //        //if (!authroized)
    //        //    return false;

    //        if (ClaimsModel.UserId == null && ClaimsModel.UserId <= 0)
    //            return false;
    //        return true;
    //    }
    //}

    public static class ClaimsModel
    {
        public static string ConnectionString
        {
            get
            {
                return HtmlHelpers.ClaimsModelService.GetConnectionString();
            }
        }

        public static long UserId
        {
            get
            {
                return HtmlHelpers.ClaimsModelService.GetUserId();
            }
        }
        public static string UserName
        {
            get
            {
                return HtmlHelpers.ClaimsModelService.GetUserName();
            }
        }
        public static string DisplayName
        {
            get
            {
                return HtmlHelpers.ClaimsModelService.GetDisplayName();
            }
        }
        public static string Email
        {
            get
            {
                return HtmlHelpers.ClaimsModelService.GetEmail();
                //var claimsIdentity = (ClaimsPrincipal)Thread.CurrentPrincipal;
                //var claim = claimsIdentity.Claims.FirstOrDefault(d => d.Type == Claim_Types.Email);
                //if (claim == null || string.IsNullOrEmpty(claim.Value))
                //    return "";
                //else
                //    return Convert.ToString(claim.Value);
            }
        }
        public static bool Is_Admin
        {
            get
            {
                return HtmlHelpers.ClaimsModelService.GetIs_Admin();
            }
        }
        public static bool Is_Agent
        {
            get
            {
                return HtmlHelpers.ClaimsModelService.GetIs_Agent();
            }
        }
        public static bool Is_Client
        {
            get
            {
                return HtmlHelpers.ClaimsModelService.GetIs_Client();
                //var claimsIdentity = (ClaimsPrincipal)Thread.CurrentPrincipal;
                //var claim = claimsIdentity.Claims.FirstOrDefault(d => d.Type == Claim_Types.Is_Customer);
                //if (claim == null || string.IsNullOrEmpty(claim.Value))
                //    return false;
                //else
                //    return Convert.ToBoolean(claim.Value);
            }
        }
        public static string API_Url
        {
            get
            {
                return HtmlHelpers.ClaimsModelService.GetAPI_Url();
            }
        }
    }

    public class GridFilter
    {
        public string ColumnName { get; set; }
        public string SortColumnName { get; set; }
        public string DisplayText { get; set; }
        public string Value { get; set; }
        public string Condition { get; set; }
        public string Type { get; set; }
        public bool Is_Visible { get; set; }
        public decimal Width { get; set; }
        public string TextAlign { get; set; }
        public bool Is_Sum { get; set; }
        public bool Is_Price { get; set; }
        public bool Is_Sort { get; set; }
    }

    public class PDF_Table
    {
        public List<dynamic> list { get; set; }
        public List<dynamic> lstTotal { get; set; }
        public List<GridFilter> Columns { get; set; }
        public string CompanyName { get; set; }
        public string PartyName { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Type { get; set; }
        public string FileName { get; set; }
    }

    public class Import_Result
    {
        public long res { get; set; }
        public long count { get; set; }
        public int inserted { get; set; }
        public int updated { get; set; }
        public int error { get; set; }
        public string logs { get; set; }
    }

    public class Custom_Paging_Model
    {
        public int page { get; set; }
        public int pageSize { get; set; }
        public string sortColumn { get; set; }
        public string sortOrder { get; set; }
        public string search { get; set; }
        public string where { get; set; }
        public List<GridFilter> Columns { get; set; }
        public int isAll { get; set; }
        public int isFooterRow { get; set; }
        public int isTotal { get; set; }

        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
    public class CommonGrid_List_Model<T>
    {
        public List<T> Rows { get; set; }
        public List<T> FooterRow { get; set; }
        public long Total { get; set; }
    }

    public enum SP_Action_Type
    {
        list, get, delete, update, listkeyvalue
    }

    public class ApiResponse<T>
    {
        public bool isSuccess { get; set; } = true;
        public string msg { get; set; }
        public T data { get; set; }
    }
    public class SMPTSetting
    {
        public string fromEmailAlias { get; set; }
        public string fromEmail { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public bool EnableSsl { get; set; }
        public bool DefaultCredentials { get; set; }
    }
}


