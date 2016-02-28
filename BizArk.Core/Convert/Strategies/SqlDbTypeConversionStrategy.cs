using System;
using System.Collections.Generic;
using System.Data;
using BizArk.Core.Extensions.TypeExt;

namespace BizArk.Core.Convert.Strategies
{

    /// <summary>
    /// Converts a .Net type to a SqlDBType.
    /// </summary>
    public class SqlDBTypeConversionStrategy
        : IConvertStrategy
    {

        /// <summary>
        /// Changes the type of the value.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="value"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public object Convert(Type from, Type to, object value, IFormatProvider provider)
        {
            if (from == typeof(SqlDbType))
                return DbTypeMap.ToNetType((SqlDbType)value);
            else
            {
                var type = (Type)value;
                // check for Nullable enums. 
                // Null values should be handled by DefaultValueConversionStrategy, but we need to be able
                // to get the actual type of the enum here.
                if (type.IsDerivedFromGenericType(typeof(Nullable<>)))
                    type = type.GetGenericArguments()[0];
                return DbTypeMap.ToSqlDbType(type);
            }
        }

        /// <summary>
        /// Determines whether this converter can convert the value.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public bool CanConvert(Type from, Type to)
        {
            if (from.IsDerivedFrom(typeof(Type)) && to == typeof(SqlDbType)) return true;
            if (to.IsDerivedFrom(typeof(Type)) && from == typeof(SqlDbType)) return true;
            return false;
        }

    }

    /// <summary>
    /// Map between different datatypes.
    /// </summary>
    public static class DbTypeMap
    {

        #region Initialization and Destruction

        static DbTypeMap()
        {
            sMap = new List<DbTypeMapEntry>();

            sMap.Add(new DbTypeMapEntry(typeof(bool), DbType.Boolean, SqlDbType.Bit));
            sMap.Add(new DbTypeMapEntry(typeof(byte), DbType.Byte, SqlDbType.TinyInt));
            sMap.Add(new DbTypeMapEntry(typeof(byte?), DbType.Byte, SqlDbType.TinyInt));
            sMap.Add(new DbTypeMapEntry(typeof(byte[]), DbType.Binary, SqlDbType.Binary));
            sMap.Add(new DbTypeMapEntry(typeof(DateTime), DbType.DateTime, SqlDbType.DateTime));
            sMap.Add(new DbTypeMapEntry(typeof(DateTime?), DbType.DateTime, SqlDbType.DateTime));
            sMap.Add(new DbTypeMapEntry(typeof(decimal), DbType.Decimal, SqlDbType.Decimal));
            sMap.Add(new DbTypeMapEntry(typeof(decimal?), DbType.Decimal, SqlDbType.Decimal));
            sMap.Add(new DbTypeMapEntry(typeof(double), DbType.Double, SqlDbType.Float));
            sMap.Add(new DbTypeMapEntry(typeof(double?), DbType.Double, SqlDbType.Float));
            sMap.Add(new DbTypeMapEntry(typeof(Guid), DbType.Guid, SqlDbType.UniqueIdentifier));
            sMap.Add(new DbTypeMapEntry(typeof(Guid?), DbType.Guid, SqlDbType.UniqueIdentifier));
            sMap.Add(new DbTypeMapEntry(typeof(Int16), DbType.Int16, SqlDbType.SmallInt));
            sMap.Add(new DbTypeMapEntry(typeof(Int16?), DbType.Int16, SqlDbType.SmallInt));
            sMap.Add(new DbTypeMapEntry(typeof(Int32), DbType.Int32, SqlDbType.Int));
            sMap.Add(new DbTypeMapEntry(typeof(Int32?), DbType.Int32, SqlDbType.Int));
            sMap.Add(new DbTypeMapEntry(typeof(Int64), DbType.Int64, SqlDbType.BigInt));
            sMap.Add(new DbTypeMapEntry(typeof(Int64?), DbType.Int64, SqlDbType.BigInt));
            sMap.Add(new DbTypeMapEntry(typeof(object), DbType.Object, SqlDbType.Variant));
            sMap.Add(new DbTypeMapEntry(typeof(string), DbType.String, SqlDbType.VarChar));
        }

        #endregion

        #region Fields and Properties

        private static List<DbTypeMapEntry> sMap;

        #endregion

        #region Methods

        /// <summary>
        /// Converts the value.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type ToNetType(SqlDbType type)
        {
            var entry = Find(type);
            if (entry == null)
                throw new InvalidCastException(string.Format("SqlDbType.{0} cannot be converted to a .Net type.", type.ToString()));
            return entry.Type;
        }

        /// <summary>
        /// Converts the value.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type ToNetType(DbType type)
        {
            var entry = Find(type);
            if (entry == null)
                throw new InvalidCastException(string.Format("DbType.{0} cannot be converted to a .Net type.", type.ToString()));
            return entry.Type;
        }

        /// <summary>
        /// Converts the value.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static DbType ToDbType(SqlDbType type)
        {
            var entry = Find(type);
            if (entry == null)
                throw new InvalidCastException(string.Format("{0} cannot be converted to a DbType.", type.ToString()));
            return entry.DbType;
        }

        /// <summary>
        /// Converts the value.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static DbType ToDbType(Type type)
        {
            var entry = Find(type);
            if (entry == null)
                throw new InvalidCastException(string.Format("{0} cannot be converted to a DbType.", type.ToString()));
            return entry.DbType;
        }

        /// <summary>
        /// Converts the value.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static SqlDbType ToSqlDbType(DbType type)
        {
            var entry = Find(type);
            if (entry == null)
                throw new InvalidCastException(string.Format("{0} cannot be converted to a SqlDbType.", type.ToString()));
            return entry.SqlDbType;
        }

        /// <summary>
        /// Converts the value.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static SqlDbType ToSqlDbType(Type type)
        {
            var entry = Find(type);
            if (entry == null)
                throw new InvalidCastException(string.Format("{0} cannot be converted to a SqlDbType.", type.ToString()));
            return entry.SqlDbType;
        }

        private static DbTypeMapEntry Find(Type type)
        {
            return sMap.Find((e) => { return e.Type == type; });
        }

        private static DbTypeMapEntry Find(DbType type)
        {
            return sMap.Find((e) => { return e.DbType == type; });
        }

        private static DbTypeMapEntry Find(SqlDbType type)
        {
            return sMap.Find((e) => { return e.SqlDbType == type; });
        }

        /// <summary>
        /// Determines if the .Net type can be converted to a SqlDbType/DbType or not.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool CanConvertType(Type type)
        {
            var entry = Find(type);
            return entry != null;
        }

        #endregion

        /// <summary>
        /// Represents a map entry for conversion.
        /// </summary>
        private class DbTypeMapEntry
        {

            /// <summary>
            /// Creates an instance of DbTypeMapEntry.
            /// </summary>
            /// <param name="type"></param>
            /// <param name="dbType"></param>
            /// <param name="sqlDbType"></param>
            public DbTypeMapEntry(Type type, DbType dbType, SqlDbType sqlDbType)
            {
                this.Type = type;
                this.DbType = dbType;
                this.SqlDbType = sqlDbType;
            }

            /// <summary>
            /// Gets the .Net type.
            /// </summary>
            public Type Type { get; private set; }

            /// <summary>
            /// Gets the DbType.
            /// </summary>
            public DbType DbType { get; private set; }

            /// <summary>
            /// Gets the SqlDbType.
            /// </summary>
            public SqlDbType SqlDbType { get; private set; }

        }

    }

}
