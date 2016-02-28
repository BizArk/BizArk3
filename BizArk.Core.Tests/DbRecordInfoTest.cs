using Redwerb.BizArk.Core.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;
using System.Collections.Generic;

namespace TestBizArkCore
{


    /// <summary>
    ///This is a test class for DbRecordInfoTest and is intended
    ///to contain all DbRecordInfoTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DbRecordInfoTest
    {

        [TestMethod()]
        public void DbRecordInfoInitTest()
        {
            var target = new DbRecordInfo(typeof(DbTest));
            var fld = target.Fields["MyProperty"];
            Assert.IsNotNull(fld);
            Assert.AreEqual("MyField", fld.FieldName);
            Assert.AreEqual(SqlDbType.VarChar, fld.DbType);
            Assert.AreEqual(50, fld.Length);

            fld = target.Fields["AnotherProperty"];
            Assert.AreEqual("AnotherProperty", fld.FieldName);
            Assert.AreEqual(SqlDbType.SmallInt, fld.DbType);

            Assert.IsNotNull(target.PrimaryKey);
            Assert.AreEqual("DbTestID", target.PrimaryKey.Property.Name);

            try
            {
                fld = target.Fields["InvalidProperty"];
                Assert.Fail("Should have thrown KeyNotFoundException.");
            }
            catch (KeyNotFoundException)
            {
                // Expected.
            }
        }

        [TestMethod]
        public void DbRecordTest()
        {
            var target = new DbTest();
            Assert.IsFalse(target.IsNew);
            Assert.IsFalse(target.DeleteRecord);
            Assert.IsFalse(target.IsModified);
            Assert.IsFalse(target.NeedsSave);

            Assert.IsFalse(target.Fields["DbTestID"].IsSet);
            Assert.IsFalse(target.Fields["DbTestID"].IsModified);
            
            target.BeginInit();
            target.DbTestID = 0;
            target.EndInit();
            
            Assert.IsNotNull(target.Fields["DbTestID"].OriginalValue);
            Assert.AreEqual(0, target.Fields["DbTestID"].OriginalValue);
            Assert.IsTrue(target.Fields["DbTestID"].IsSet);
            Assert.IsFalse(target.Fields["DbTestID"].IsModified);
            
            target.DbTestID = 1;
            
            Assert.IsTrue(target.Fields["DbTestID"].IsModified);
        }

        [TestMethod]
        public void NonDbPropertyTest()
        {
            var target = new DbTest();

            Assert.IsFalse(target.Fields["NonDbProperty"].IsSet);
            target.NonDbProperty = 4;
            Assert.IsTrue(target.Fields["NonDbProperty"].IsSet);
            Assert.IsTrue(target.IsModified);
            Assert.IsFalse(target.NeedsSave);
        }

        [TestMethod]
        public void RepositoryTest()
        {
            var target = new DbRecordInfo(typeof(DbTest));
            var rep = target.Repository;
            Assert.IsNotNull(rep);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetNonSetValueTest()
        {
            var target = new DbTest();
            var val = target.DbTestID;
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetNonInitOrigValExTest()
        {
            var target = new DbTest();
            Assert.IsNull(target.Fields["DbTestID"].OriginalValue);
        }

        [TestMethod]
        public void JoinTest()
        {
            // setup the repository for the joined object.
            var info = DbRecordInfo.GetRecordInfo(typeof(MyJoinedObject));
            info.Repository = new MockMyJoinedObjectRepository();

            var target = new DbTest();
            target.JoinID = int.MinValue;
            Assert.IsNull(target.Join);
            target.JoinID = 5;
            Assert.IsNotNull(target.Join);
            Assert.AreEqual(target.JoinID, target.Join.MyID);
            target.JoinID = 10;
            Assert.IsNotNull(target.Join);
            Assert.AreEqual(target.JoinID, target.Join.MyID);
        }

    }

    public class DbTest
        : DbRecord
    {

        public DbTest()
        {

        }

        [DbField()]
        public int DbTestID
        {
            get { return (int)GetValue("DbTestID"); }
            set { SetValue("DbTestID", value); }
        }

        [DbField("MyField", Length = 50)]
        public string MyProperty
        {
            get { return (string)GetValue("MyProperty"); }
            set { SetValue("MyProperty", value); }
        }

        [DbField(DbType = SqlDbType.SmallInt)]
        public int AnotherProperty
        {
            get { return (int)GetValue("AnotherProperty"); }
            set { SetValue("AnotherProperty", value); }
        }

        [DbField(IncludeInSave=false)]
        public int NonDbProperty
        {
            get { return (int)GetValue("NonDbProperty"); }
            set { SetValue("NonDbProperty", value); }
        }

        [DbField()]
        public int JoinID
        {
            get { return (int)GetValue("JoinID"); }
            set { SetValue("JoinID", value); }
        }

        [DbJoin(KeyField="JoinID")]
        public MyJoinedObject Join
        {
            get { return (MyJoinedObject)GetJoin("Join"); }
            set { SetJoin("Join", value); }
        }

    }

    [DbRecord(PrimaryKey = "MyID")]
    public class MyJoinedObject
        : DbRecord
    {
        [DbField()]
        public int MyID
        {
            get { return (int)GetValue("MyID"); }
            set { SetValue("MyID", value); }
        }

        [DbField()]
        public string Name
        {
            get { return (string)GetValue("Name"); }
            set { SetValue("Name", value); }
        }
    }

    public class MockMyJoinedObjectRepository
        : IRepository
    {
        #region IRepository Members

        public DbRecord[] Load(params FieldValue[] criteria)
        {
            var records = new List<DbRecord>();
            var join = new MyJoinedObject();
            join.MyID = (int)criteria[0].Value;
            records.Add(join);
            return records.ToArray();
        }

        public void Insert(DbRecord record)
        {
            ;
        }

        public void Update(DbRecord record)
        {
            ;
        }

        public void Delete(DbRecord record)
        {
            ;
        }

        #endregion
    }


}
