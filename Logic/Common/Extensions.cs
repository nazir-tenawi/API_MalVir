using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Security.Claims;

namespace MalVirDetector_CLI_API.Logic
{
    public static class DataExtensions
    {
        public enum Conditions { IsEqual, IsNotEqual, Contains, GraterThan, LessThan, GreaterThanOrEqual, LessThanOrEqual, DoesNotContains, StartsWith, EndsWith, After, Before, InMemoryContains, ImMemoryDoesnotContains }

        public static List<T> MapToList<T>(this IDataReader dr)
        {
            if (dr != null && dr.FieldCount > 0)
            {
                var type = typeof(T);
                var entities = new List<T>();
                var propDict = new Dictionary<string, PropertyInfo>();
                var props = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                propDict = props.ToDictionary(p =>
                {
                    var attr = p.GetCustomAttribute<PropertyBinding>();
                    if (attr != null)
                    {
                        return attr.Name;
                    }
                    return p.Name;
                }, p => p);

                if (type.IsPrimitive || type.IsValueType || type == typeof(string))
                {
                    while (dr.Read())
                    {
                        T obj = (T)dr.GetValue(0);
                        entities.Add(obj);
                    }
                }
                else if (type == typeof(object))
                {
                    while (dr.Read())
                    {
                        dynamic newObject = new ExpandoObject();
                        var obj = newObject as IDictionary<string, object>;
                        for (int index = 0; index < dr.FieldCount; index++)
                        {
                            obj[dr.GetName(index)] = dr.GetValue(index);
                        }
                        entities.Add(newObject);
                    }
                }
                else
                {
                    while (dr.Read())
                    {
                        T newObject = Activator.CreateInstance<T>();
                        for (int index = 0; index < dr.FieldCount; index++)
                        {
                            if (propDict.ContainsKey(dr.GetName(index)))
                            {
                                var info = propDict[dr.GetName(index)];
                                if ((info != null) && info.CanWrite)
                                {
                                    var val = dr.GetValue(index);
                                    if (val.GetType() == info.PropertyType)
                                    {
                                        info.SetValue(newObject, (val == DBNull.Value) ? null : val, null);
                                    }
                                    else
                                    {
                                        info.SetValue(newObject, (val == DBNull.Value) ? null : val.To(info.PropertyType), null);
                                    }
                                }
                            }
                        }
                        entities.Add(newObject);
                    }
                }
                return entities;
            }
            return null;
        }

        public static T MapToSingle<T>(this IDataReader dr)
        {
            if (dr != null && dr.FieldCount > 0)
            {
                Type type = typeof(T);
                var propDict = new Dictionary<string, PropertyInfo>();
                var props = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                propDict = props.ToDictionary(p =>
                {
                    var attr = p.GetCustomAttribute<PropertyBinding>();
                    if (attr != null)
                    {
                        return attr.Name;
                    }
                    return p.Name;
                }, p => p);

                while (dr.Read())
                {
                    if (type.IsPrimitive || type.IsValueType || type == typeof(string))
                    {
                        if (type == typeof(Int64))
                        {
                            return (T)Convert.ChangeType(dr.GetValue(0), typeof(T));
                        }
                        else
                        {
                            T obj = (T)dr.GetValue(0);
                            return obj;
                        }
                    }
                    else
                    {

                        T newObject = Activator.CreateInstance<T>();
                        for (int index = 0; index < dr.FieldCount; index++)
                        {
                            if (propDict.ContainsKey(dr.GetName(index)))
                            {
                                var info = propDict[dr.GetName(index)];
                                if ((info != null) && info.CanWrite)
                                {
                                    var val = dr.GetValue(index);
                                    if (val.GetType() == info.PropertyType)
                                    {
                                        info.SetValue(newObject, (val == DBNull.Value) ? null : val, null);
                                    }
                                    else
                                    {
                                        info.SetValue(newObject, (val == DBNull.Value) ? null : val.To(info.PropertyType), null);
                                    }
                                }
                            }
                        }
                        return newObject;
                    }
                }
            }
            return default(T);
        }

        public static List<T> MapToList<T>(this DataTable dt) where T : new()
        {
            if (dt != null)
            {
                var entity = typeof(T);
                var entities = new List<T>();
                var propDict = new Dictionary<string, PropertyInfo>();
                var props = entity.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                propDict = props.ToDictionary(p => p.Name, p => p);
                foreach (DataRow row in dt.Rows.Cast<DataRow>())
                {
                    T newObject = new T();
                    foreach (DataColumn column in dt.Columns)
                    {
                        if (propDict.ContainsKey(column.ColumnName))
                        {
                            var info = propDict[column.ColumnName];
                            if ((info != null) && info.CanWrite)
                            {
                                var val = row[column];
                                info.SetValue(newObject, (val == DBNull.Value) ? null : val, null);
                            }
                        }
                    }
                    entities.Add(newObject);
                }
                return entities;
            }
            return null;
        }
        public static List<T> MapToList<T>(this DataTable dt, int pageNo, int? pageSize, bool isSpecial = false) where T : new()
        {
            if (dt != null)
            {
                int skip = pageNo * (pageSize.HasValue ? pageSize.Value : 0);
                int take = (pageSize.HasValue ? pageSize.Value : int.MaxValue);
                var entity = typeof(T);
                var entities = new List<T>();
                var propDict = new Dictionary<string, PropertyInfo>();
                var props = entity.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                propDict = props.ToDictionary(p => p.Name, p => p);
                foreach (DataRow row in dt.Rows.Cast<DataRow>().Skip(skip).Take(take))
                {
                    T newObject = new T();
                    foreach (DataColumn column in dt.Columns)
                    {
                        if (propDict.ContainsKey(column.ColumnName))
                        {
                            var info = propDict[column.ColumnName];
                            if ((info != null) && info.CanWrite)
                            {
                                var val = row[column];
                                info.SetValue(newObject, (val == DBNull.Value) ? null : val, null);
                            }
                        }
                    }
                    entities.Add(newObject);
                }
                return entities;
            }
            return null;
        }

        public static Expression<Func<T, bool>> BuildPredicate<T>(string member, object value, Conditions condition)
        {
            var p = Expression.Parameter(typeof(T), "d");
            Expression body = p;
            Expression<Func<T, bool>> ret = d => false;
            Type t = null;
            var prop = typeof(T).GetProperty(member);
            if (prop != null)
            {
                t = prop.PropertyType;
            }
            else
            {
                return ret;
            }
            if (value != null && t != value.GetType())
            {
                var a = Nullable.GetUnderlyingType(t) ?? t;
                value = Convert.ChangeType(value, a, System.Globalization.CultureInfo.InvariantCulture);
            }
            foreach (var subMember in member.Split('.'))
            {
                body = Expression.PropertyOrField(body, subMember);
            }

            if (t == typeof(int) || t == typeof(int?) || t == typeof(decimal) || t == typeof(decimal?) || t == typeof(double) || t == typeof(double?) || t == typeof(float) || t == typeof(float?) || t == typeof(long) || t == typeof(long?))
            {
                switch (condition)
                {
                    case Conditions.IsEqual:
                        ret = Expression.Lambda<Func<T, bool>>(Expression.Equal(body, Expression.Constant(value, body.Type)), p);
                        break;
                    case Conditions.IsNotEqual:
                        ret = Expression.Lambda<Func<T, bool>>(Expression.NotEqual(body, Expression.Constant(value, body.Type)), p);
                        break;
                    case Conditions.LessThan:
                        ret = Expression.Lambda<Func<T, bool>>(Expression.LessThan(body, Expression.Constant(value, body.Type)), p);
                        break;
                    case Conditions.LessThanOrEqual:
                        ret = Expression.Lambda<Func<T, bool>>(Expression.LessThanOrEqual(body, Expression.Constant(value, body.Type)), p);
                        break;
                    case Conditions.GraterThan:
                        ret = Expression.Lambda<Func<T, bool>>(Expression.GreaterThan(body, Expression.Constant(value, body.Type)), p);
                        break;
                    case Conditions.GreaterThanOrEqual:
                        ret = Expression.Lambda<Func<T, bool>>(Expression.GreaterThanOrEqual(body, Expression.Constant(value, body.Type)), p);
                        break;
                }
            }
            else if (t == typeof(string))
            {
                switch (condition)
                {
                    case Conditions.IsEqual:
                        ret = Expression.Lambda<Func<T, bool>>(Expression.Equal(body, Expression.Constant(value, body.Type)), p);
                        break;
                    case Conditions.IsNotEqual:
                        ret = Expression.Lambda<Func<T, bool>>(Expression.NotEqual(body, Expression.Constant(value, body.Type)), p);
                        break;
                    case Conditions.Contains:
                        ret = Expression.Lambda<Func<T, bool>>(Expression.Call(
                                 body, "Contains", null, Expression.Constant(value, body.Type)), p);
                        break;
                    case Conditions.DoesNotContains:
                        ret = Expression.Lambda<Func<T, bool>>(Expression.Not(Expression.Call(
                                 body, "Contains", null, Expression.Constant(value, body.Type))), p);
                        break;
                    case Conditions.StartsWith:
                        ret = Expression.Lambda<Func<T, bool>>(Expression.Call(
                                body, "StartsWith", null, Expression.Constant(value, body.Type)), p);
                        break;
                    case Conditions.EndsWith:
                        ret = Expression.Lambda<Func<T, bool>>(Expression.Call(
                                body, "EndsWith", null, Expression.Constant(value, body.Type)), p);
                        break;
                    case Conditions.InMemoryContains:
                        ret = Expression.Lambda<Func<T, bool>>(Expression.GreaterThanOrEqual(Expression.Call(
                                 body, "IndexOf", null, Expression.Constant(value, body.Type), Expression.Constant(StringComparison.InvariantCultureIgnoreCase)), Expression.Constant(0)), p);
                        break;
                    case Conditions.ImMemoryDoesnotContains:
                        ret = Expression.Lambda<Func<T, bool>>(Expression.LessThan(Expression.Call(
                                 body, "IndexOf", null, Expression.Constant(value, body.Type), Expression.Constant(StringComparison.InvariantCultureIgnoreCase)), Expression.Constant(0)), p);
                        break;
                }
            }
            else if (t == typeof(DateTime) || t == typeof(DateTime?))
            {
                switch (condition)
                {
                    case Conditions.IsEqual:
                    case Conditions.Contains:
                        ret = Expression.Lambda<Func<T, bool>>(Expression.Equal(body, Expression.Constant(value, body.Type)), p);
                        break;
                    case Conditions.IsNotEqual:
                        ret = Expression.Lambda<Func<T, bool>>(Expression.NotEqual(body, Expression.Constant(value, body.Type)), p);
                        break;
                    case Conditions.After:
                        ret = Expression.Lambda<Func<T, bool>>(Expression.GreaterThan(body, Expression.Constant(value, body.Type)), p);
                        break;
                    case Conditions.Before:
                        ret = Expression.Lambda<Func<T, bool>>(Expression.LessThan(body, Expression.Constant(value, body.Type)), p);
                        break;
                }
            }
            else if (t == typeof(bool) || t == typeof(bool?))
            {
                ret = Expression.Lambda<Func<T, bool>>(Expression.Equal(body, Expression.Constant(value, body.Type)), p);
            }
            return ret;
        }

        public static Expression<Func<T, bool>> AndAlso<T>(this Expression<Func<T, bool>> lambda, string member, object value, Conditions condition)
        {
            var lambda2 = BuildPredicate<T>(member, value, condition);
            ParameterExpression p = lambda.Parameters[0];

            SubstExpressionVisitor visitor = new SubstExpressionVisitor();
            visitor.subst[lambda2.Parameters[0]] = p;

            Expression body = Expression.AndAlso(lambda.Body, visitor.Visit(lambda2.Body));
            var exp = Expression.Lambda<Func<T, bool>>(body, p);
            return exp;
        }

        public static Expression<Func<T, bool>> AndAlso<T>(this Expression<Func<T, bool>> lambda, Expression<Func<T, bool>> lambda2)
        {
            //var lambda2 = BuildPredicate<T>(member, value, condition);
            ParameterExpression p = lambda.Parameters[0];

            SubstExpressionVisitor visitor = new SubstExpressionVisitor();
            visitor.subst[lambda2.Parameters[0]] = p;

            Expression body = Expression.AndAlso(lambda.Body, visitor.Visit(lambda2.Body));
            var exp = Expression.Lambda<Func<T, bool>>(body, p);
            return exp;
        }
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> lambda, Expression<Func<T, bool>> lambda2)
        {
            //var lambda2 = BuildPredicate<T>(member, value, condition);
            ParameterExpression p = lambda.Parameters[0];

            SubstExpressionVisitor visitor = new SubstExpressionVisitor();
            visitor.subst[lambda2.Parameters[0]] = p;

            Expression body = Expression.And(lambda.Body, visitor.Visit(lambda2.Body));
            var exp = Expression.Lambda<Func<T, bool>>(body, p);
            return exp;
        }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> lambda, string member, object value, Conditions condition)
        {
            var lambda2 = BuildPredicate<T>(member, value, condition);
            ParameterExpression p = lambda.Parameters[0];

            SubstExpressionVisitor visitor = new SubstExpressionVisitor();
            visitor.subst[lambda2.Parameters[0]] = p;

            Expression body = Expression.Or(lambda.Body, visitor.Visit(lambda2.Body));
            var exp = Expression.Lambda<Func<T, bool>>(body, p);
            return exp;
        }

        public static Expression<Func<T, bool>> OrElse<T>(this Expression<Func<T, bool>> lambda, string member, object value, Conditions condition)
        {
            var lambda2 = BuildPredicate<T>(member, value, condition);
            ParameterExpression p = lambda.Parameters[0];

            SubstExpressionVisitor visitor = new SubstExpressionVisitor();
            visitor.subst[lambda2.Parameters[0]] = p;

            Expression body = Expression.OrElse(lambda.Body, visitor.Visit(lambda2.Body));
            var exp = Expression.Lambda<Func<T, bool>>(body, p);
            return exp;
        }

        //public static IQueryable<dynamic> SelectDynamic<T>(this IQueryable<T> source, IEnumerable<string> fieldNames)
        //{
        //    Dictionary<string, PropertyInfo> sourceProperties = fieldNames.ToDictionary(name => name, name => source.ElementType.GetProperty(name));
        //    Type dynamicType = MyTypeBuilder.CompileResultType(sourceProperties);
        //    ParameterExpression para = Expression.Parameter(typeof(T), "d");
        //    IEnumerable<MemberBinding> bindings = dynamicType.GetProperties().Select(p => Expression.Bind(p, Expression.Property(para, sourceProperties[p.Name]))).OfType<MemberBinding>();
        //    var selector = Expression.Lambda<Func<T, dynamic>>(Expression.MemberInit(
        //        Expression.New(dynamicType), bindings), para);
        //    return source.Select(selector);
        //}

        public static IQueryable<T> Select<T>(this IQueryable<T> source, IEnumerable<string> fieldNames)
        {
            Dictionary<string, PropertyInfo> sourceProperties = fieldNames.ToDictionary(name => name, name => source.ElementType.GetProperty(name));
            Type dynamicType = typeof(T);
            ParameterExpression sourceItem = Expression.Parameter(source.ElementType, "t");
            var ci = typeof(T).GetConstructor(Type.EmptyTypes);
            IEnumerable<MemberBinding> bindings = fieldNames.Select(p => Expression.Bind(sourceProperties[p], Expression.Property(sourceItem, sourceProperties[p]))).OfType<MemberBinding>();

            var selector = Expression.Lambda<Func<T, T>>(Expression.MemberInit(
                Expression.New(typeof(T)), bindings), sourceItem);

            return source.Select(selector);
        }

        //public static Expression<Func<TSource, dynamic>> SelectExpression<TSource>(IEnumerable<string> propertyNames)
        //{
        //    Dictionary<string, PropertyInfo> sourceProperties = propertyNames.ToDictionary(name => name, name => typeof(TSource).GetProperty(name));
        //    Type dynamicType = MyTypeBuilder.CompileResultType(sourceProperties);
        //    ParameterExpression para = Expression.Parameter(typeof(TSource), "d");
        //    IEnumerable<MemberBinding> bindings = dynamicType.GetProperties().Select(p => Expression.Bind(p, Expression.Property(para, sourceProperties[p.Name]))).OfType<MemberBinding>();
        //    var selector = Expression.Lambda<Func<TSource, dynamic>>(Expression.MemberInit(
        //        Expression.New(dynamicType), bindings), para);
        //    return selector;
        //}

        public static bool HasColumn(this IDataRecord dr, string columnName)
        {
            for (int i = 0; i < dr.FieldCount; i++)
            {
                if (dr.GetName(i).Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }
            return false;
        }


        //public static IEnumerable DynamicSqlQuery(this Database database, string sql, params object[] parameters)
        //{
        //    TypeBuilder builder = createTypeBuilder(
        //            "MyDynamicAssembly", "MyDynamicModule", "MyDynamicType");

        //    using (System.Data.IDbCommand command = database.Connection.CreateCommand())
        //    {
        //        try
        //        {
        //            database.Connection.Open();
        //            command.CommandText = sql;
        //            command.CommandTimeout = command.Connection.ConnectionTimeout;
        //            foreach (var param in parameters)
        //            {
        //                command.Parameters.Add(param);
        //            }

        //            using (System.Data.IDataReader reader = command.ExecuteReader())
        //            {
        //                var schema = reader.GetSchemaTable();

        //                foreach (System.Data.DataRow row in schema.Rows)
        //                {
        //                    string name = (string)row["ColumnName"];
        //                    //var a=row.ItemArray.Select(d=>d.)
        //                    Type type = (Type)row["DataType"];
        //                    if (type != typeof(string) && (bool)row.ItemArray[schema.Columns.IndexOf("AllowDbNull")])
        //                    {
        //                        type = typeof(Nullable<>).MakeGenericType(type);
        //                    }
        //                    createAutoImplementedProperty(builder, name, type);
        //                }
        //            }
        //        }
        //        finally
        //        {
        //            database.Connection.Close();
        //            command.Parameters.Clear();
        //        }
        //    }

        //    Type resultType = builder.CreateType();

        //    return database.SqlQuery(resultType, sql, parameters);
        //}

        //private static TypeBuilder createTypeBuilder(string assemblyName, string moduleName, string typeName)
        //{
        //    TypeBuilder typeBuilder = AppDomain
        //        .CurrentDomain
        //        .DefineDynamicAssembly(new AssemblyName(assemblyName),
        //                               AssemblyBuilderAccess.Run)
        //        .DefineDynamicModule(moduleName)
        //        .DefineType(typeName, TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Serializable);
        //    typeBuilder.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);
        //    //typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Any, Type.EmptyTypes);
        //    return typeBuilder;
        //}

        private static void createAutoImplementedProperty(TypeBuilder builder, string propertyName, Type propertyType)
        {
            const string PrivateFieldPrefix = "m_";
            const string GetterPrefix = "get_";
            const string SetterPrefix = "set_";

            // Generate the field.
            FieldBuilder fieldBuilder = builder.DefineField(
                string.Concat(PrivateFieldPrefix, propertyName),
                              propertyType, FieldAttributes.Private);

            // Generate the property
            PropertyBuilder propertyBuilder = builder.DefineProperty(
                propertyName, System.Reflection.PropertyAttributes.HasDefault, propertyType, null);

            // Property getter and setter attributes.
            MethodAttributes propertyMethodAttributes =
                MethodAttributes.Public | MethodAttributes.SpecialName |
                MethodAttributes.HideBySig;

            // Define the getter method.
            MethodBuilder getterMethod = builder.DefineMethod(
                string.Concat(GetterPrefix, propertyName),
                propertyMethodAttributes, propertyType, Type.EmptyTypes);

            // Emit the IL code.
            // ldarg.0
            // ldfld,_field
            // ret
            ILGenerator getterILCode = getterMethod.GetILGenerator();
            getterILCode.Emit(OpCodes.Ldarg_0);
            getterILCode.Emit(OpCodes.Ldfld, fieldBuilder);
            getterILCode.Emit(OpCodes.Ret);

            // Define the setter method.
            MethodBuilder setterMethod = builder.DefineMethod(
                string.Concat(SetterPrefix, propertyName),
                propertyMethodAttributes, null, new Type[] { propertyType });

            // Emit the IL code.
            // ldarg.0
            // ldarg.1
            // stfld,_field
            // ret
            ILGenerator setterILCode = setterMethod.GetILGenerator();
            setterILCode.Emit(OpCodes.Ldarg_0);
            setterILCode.Emit(OpCodes.Ldarg_1);
            setterILCode.Emit(OpCodes.Stfld, fieldBuilder);
            setterILCode.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getterMethod);
            propertyBuilder.SetSetMethod(setterMethod);
        }

        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string property)
        {
            return ApplyOrder<T>(source, property, "OrderBy");
        }

        public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string property)
        {
            return ApplyOrder<T>(source, property, "OrderByDescending");
        }

        public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> source, string property)
        {
            return ApplyOrder<T>(source, property, "ThenBy");
        }

        public static IOrderedQueryable<T> ThenByDescending<T>(this IOrderedQueryable<T> source, string property)
        {
            return ApplyOrder<T>(source, property, "ThenByDescending");
        }

        static IOrderedQueryable<T> ApplyOrder<T>(IQueryable<T> source, string property, string methodName)
        {
            string[] props = property.Split('.');
            Type type = typeof(T);
            ParameterExpression arg = Expression.Parameter(type, "x");
            Expression expr = arg;
            foreach (string prop in props)
            {
                // use reflection (not ComponentModel) to mirror LINQ
                PropertyInfo pi = type.GetProperty(prop.Trim());
                expr = Expression.Property(expr, pi);
                type = pi.PropertyType;
            }
            Type delegateType = typeof(Func<,>).MakeGenericType(typeof(T), type);
            LambdaExpression lambda = Expression.Lambda(delegateType, expr, arg);

            object result = typeof(Queryable).GetMethods().Single(
                    method => method.Name == methodName
                            && method.IsGenericMethodDefinition
                            && method.GetGenericArguments().Length == 2
                            && method.GetParameters().Length == 2)
                    .MakeGenericMethod(typeof(T), type)
                    .Invoke(null, new object[] { source, lambda });
            return (IOrderedQueryable<T>)result;
        }

        internal class SubstExpressionVisitor : System.Linq.Expressions.ExpressionVisitor
        {
            public Dictionary<Expression, Expression> subst = new Dictionary<Expression, Expression>();

            protected override Expression VisitParameter(ParameterExpression node)
            {
                Expression newValue;
                if (subst.TryGetValue(node, out newValue))
                {
                    return newValue;
                }
                return node;
            }
        }
    }

    public static class SystemExtensions
    {
        public static T To<T>(this object input, T defaultValue = default(T))
        {
            T ret = defaultValue;
            if (input == null || input == DBNull.Value) return ret;
            try
            {
                Type t = typeof(T);
                t = Nullable.GetUnderlyingType(t) ?? t;
                ret = (T)Convert.ChangeType(input, t, System.Globalization.CultureInfo.InvariantCulture);
            }
            catch { }
            return ret;
        }

        public static object To(this object input, Type t)
        {
            if (input == null || input == DBNull.Value) return input;
            try
            {
                t = Nullable.GetUnderlyingType(t) ?? t;
                return Convert.ChangeType(input, t, System.Globalization.CultureInfo.InvariantCulture);
            }
            catch { }
            return null;
        }

        public static TSource ExactOrFirst<TSource>(this IEnumerable<TSource> source, Expression<Func<TSource, bool>> predicate) where TSource : class
        {
            var queryable = source.AsQueryable();
            if (queryable.Any(predicate))
                return queryable.First(predicate);
            else
                return queryable.FirstOrDefault();
        }

        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            foreach (T item in enumeration)
            {
                action(item);
            }
        }

        public static dynamic EnumerableFirst(this IEnumerable enumeration)
        {
            var enumerator = enumeration.GetEnumerator();
            if (enumerator.MoveNext())
                return enumerator.Current;
            else
                return null;
        }
        public static dynamic EnumerableToList(this IEnumerable enumeration)
        {
            var list = new List<dynamic>();
            var enumerator = enumeration.GetEnumerator();
            while (enumerator.MoveNext())
            {
                list.Add(enumerator.Current);
            }
            return list;
        }

    }

    public class ObjectComparer<T> : IEqualityComparer<T>
    {
        public bool Equals(T x, T y)
        {
            var ret = false;
            if (x != null && y != null)
            {
                var propX = x.GetType().GetProperties();
                var propY = y.GetType().GetRuntimeProperties();
                if (propX.Count() == propY.Count())
                {
                    ret = true;
                    foreach (var item in propX)
                    {
                        var yProp = propY.FirstOrDefault(d => d.Name == item.Name && d.PropertyType == item.PropertyType);
                        if (yProp != null)
                        {
                            if (item.GetValue(x) != yProp.GetValue(y))
                            {
                                ret = false;
                                break;
                            }
                        }
                    }
                }
            }
            return ret;
        }

        public int GetHashCode(T obj)
        {
            return obj.GetHashCode();
        }
    }
}