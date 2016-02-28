using System;
using System.Data.Common;
using System.Data.SqlServerCe;
using BizArk.DB;
using BizArk.DB.Adapters;

namespace BizArk.Core.Tests
{
    public class SqlCeDbAdapter : SqlDbAdapter
    {

        #region Initialization and Destruction

        /// <summary>
        /// Creates an instance of SqlDbInfo.
        /// </summary>
        /// <param name="connStr"></param>
        public SqlCeDbAdapter(string connStr)
            : base(connStr)
        {
        }

        #endregion

        #region Methods

        public override DateTime NowUtc(Database db)
        {
            var cmd = CreateCommand();
            cmd.CommandText = "SELECT GETDATE()";
            var dt = db.ExecuteScalar<DateTime>(cmd);
            return dt.ToUniversalTime();
        }

        /// <summary>
        /// Creates an instance of SqlConnection. The connection is NOT opened.
        /// </summary>
        /// <returns></returns>
        public override DbConnection CreateConnection()
        {
            return new SqlCeConnection(ConnStr);
        }

        protected override DbCommand CreateCommand()
        {
            return new SqlCeCommand();
        }

        protected override int InsertIdentity(Database db, DbCommand cmd, string identityFieldName)
        {
            db.ExecuteNonQuery(cmd);
            cmd = CreateCommand("SELECT @@IDENTITY");
            return db.ExecuteScalar<int>(cmd);
        }

        #endregion

    }
}
