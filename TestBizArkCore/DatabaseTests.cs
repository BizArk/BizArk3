using System.Data.SqlServerCe;
using System.IO;
using BizArk.Core.Extensions.DataExt;
using BizArk.DB;
using BizArk.DB.ORM;
using NUnit.Framework;

namespace BizArk.Core.Tests
{

    [TestFixture,Ignore]
    public class DatabaseTests
    {

        private const string cNorthwindConnStr = "Data Source=NorthwindTmp.sdf;Persist Security Info=False;";

        [TestFixtureSetUp]
        public void Setup()
        {
            // Make a copy of the test DB so we don't mess with the original.
            File.Copy(@"Data\Northwind.sdf", "NorthwindTmp.sdf", true);

            // The Northwind sample DB is used in multiple tests. Register it so it's easier to use.
            var adapter = new SqlCeDbAdapter(cNorthwindConnStr);
            Database.RegisterAdapter("nw", adapter);
        }

        [TestFixtureTearDown]
        public void Teardown()
        {
            // Unregister the adapter (the DB won't exist after Teardown).
            Database.RemoveAdapter("nw");

            // The temporary DB is no longer needed.
            File.Delete("NorthwindTmp.sdf");
        }

        [Test]
        public void _StartTest()
        {
            /// It can take about half a second to prepare the database 
            /// the first time you try to access it. This method is just
            /// intended to prep the database so the times can be a little
            /// more reasonable.

            SelectSingleDbObjectTest();
        }

        [Test]
        public void SelectSingleTest()
        {
            using (var db = Database.CreateDB("nw"))
            {
                var cmd = db.CreateCommand("SELECT * FROM Customers WHERE CustomerID = @CustomerID", new { CustomerID = "ALFKI" });
                var customer = db.SelectSingle<Customer>(cmd, (row) =>
                {
                    return new Customer()
                    {
                        CustomerID = row.GetString("CustomerID"),
                        CompanyName = row.GetString("CompanyName"),
                        ContactTitle = row.GetString("ContactTitle"),
                        Address = row.GetString("Address"),
                        City = row.GetString("City"),
                        Region = row.GetString("Region"),
                        PostalCode = row.GetString("PostalCode"),
                        Country = row.GetString("Country"),
                        Phone = row.GetString("Phone"),
                        Fax = row.GetString("Fax")
                    };
                });

                Assert.AreEqual("Customers", customer.DbState.TableName);
                Assert.IsNotNull(customer);
                Assert.AreEqual("ALFKI", customer.CustomerID);
            }
        }

        [Test]
        public void SelectSingleDbObjectTest()
        {
            using (var db = Database.CreateDB("nw"))
            {
                var cmd = db.CreateCommand("SELECT * FROM Customers WHERE CustomerID = @CustomerID", new { CustomerID = "ALFKI" });
                var customer = db.SelectSingle<Customer>(cmd);

                Assert.AreEqual("Customers", customer.DbState.TableName);
                Assert.IsNotNull(customer);
                Assert.AreEqual("ALFKI", customer.CustomerID);
                Assert.IsFalse(customer.DbState.IsModified);

                customer.ContactName = "Christine";
                Assert.IsTrue(customer.DbState.IsModified);
            }
        }

        [Test]
        public void SelectDbObjectTest()
        {
            using (var db = Database.CreateDB("nw"))
            {
                var cmd = db.CreateCommand("SELECT * FROM Customers");
                var customers = db.Select<Customer>(cmd);

                Assert.IsTrue(customers.Length > 0);
            }
        }

        [Test]
        public void SaveDbObjectTest()
        {
            var customer = new Customer()
                {
                    CustomerID = "ABCDE",
                    CompanyName = "TEST"
                };
            customer.DbState.IsNew = true;
            Assert.IsTrue(customer.DbState.IsModified);
            Assert.IsTrue(customer.DbState.IsNew);

            using (var db = Database.CreateDB("nw"))
            {
                db.Save(customer);
                Assert.IsFalse(customer.DbState.IsModified);
                Assert.IsFalse(customer.DbState.IsNew);

                customer.ContactName = "Christine";
                Assert.IsTrue(customer.DbState.IsModified);
                Assert.IsFalse(customer.DbState.IsNew);

                db.Save(customer);
                Assert.IsFalse(customer.DbState.IsModified);
                Assert.IsFalse(customer.DbState.IsNew);

                var selCmd = new SqlCeCommand("SELECT * FROM Customers WHERE CustomerID = @CustomerID");
                selCmd.Parameters.AddWithValue("@CustomerID", "ABCDE");
                customer = db.SelectSingle<Customer>(selCmd);
                Assert.AreEqual("ABCDE", customer.CustomerID);
                Assert.IsFalse(customer.DbState.IsModified);
                Assert.IsFalse(customer.DbState.IsNew);

                customer.DbState.IsDeleted = true;
                db.Save(customer);

                customer = db.SelectSingle<Customer>(selCmd);
                Assert.IsNull(customer);

            }
        }

        [Test]
        public void TableNameTest()
        {
            var test1 = new TestDbObject1();
            Assert.AreEqual("TestDbObject1", test1.DbState.TableName);
            var test2 = new TestDbObject2();
            Assert.AreEqual("XXX", test2.DbState.TableName);
        }

        [Test]
        public void DbFieldsTest()
        {
            var test1 = new TestDbObject1();
            var flds = test1.DbState.Fields;
            Assert.AreEqual(3, flds.Count);

            var nameFld = test1.DbState["Name"] as DbFieldValue<string>;
            Assert.AreEqual("Name", nameFld.FieldName);
            Assert.IsFalse(nameFld.IsInitialized);
            Assert.IsFalse(nameFld.IsModified);
            Assert.IsFalse(test1.DbState.IsModified);
            AssertEx.Throws<DbFieldValueNotInitializedException>(() => { var tmp = nameFld.Value; });

            nameFld.Value = "Christine";
            Assert.IsTrue(nameFld.IsInitialized);
            Assert.IsTrue(nameFld.IsModified);
            Assert.IsTrue(test1.DbState.IsModified);
            Assert.AreEqual("Christine", nameFld.Value);

            nameFld.SetOriginal();
            Assert.IsTrue(nameFld.IsInitialized);
            Assert.IsFalse(nameFld.IsModified);
            Assert.IsFalse(test1.DbState.IsModified);
            Assert.AreEqual("Christine", nameFld.Value);

            AssertEx.Throws<DbFieldValueNotInitializedException>(() => { var tmp = test1.Age; });
            test1.Age = 40;
            Assert.IsTrue(test1.DbState.IsModified);

            test1.DbState.SetOriginals();
            Assert.IsFalse(test1.DbState.IsModified);
        }

        [Test]
        public void IdentityFieldTest()
        {
            var shipper = new Shipper();
            var idFld = shipper.DbState["ShipperID"] as DbIdentityFieldValue;
            Assert.AreEqual("ShipperID", idFld.FieldName);
            Assert.IsTrue(idFld.IsIdentity);
            Assert.AreSame(idFld, shipper.DbState.Identity);
            Assert.IsFalse(idFld.IsInitialized);
            Assert.IsTrue(shipper.DbState.IsNew);

            shipper.CompanyName = "Fly By Night Shipping";
            shipper.Phone = "123-456-7890";

            using (var db = Database.CreateDB("nw"))
            {
                db.Save(shipper);
                Assert.IsTrue(idFld.IsInitialized);
                Assert.IsFalse(shipper.DbState.IsNew);
            }
        }

        private class TestDbObject1 : DbObject
        {

            private DbFieldValue<int> mID = new DbIdentityFieldValue("ID");
            public int ID
            {
                get { return mID.Value; }
                set { mID.Value = value; }
            }

            private DbFieldValue<string> mName = new DbFieldValue<string>("Name");
            public string Name
            {
                get { return mName.Value; }
                set { mName.Value = value; }
            }

            private DbFieldValue<int?> mAge = new DbFieldValue<int?>("Age");
            public int? Age
            {
                get { return mAge.Value; }
                set { mAge.Value = value; }
            }
        }

        private class TestDbObject2 : ISupportDbState
        {
            private DbObjectState mState;
            public DbObjectState DbState
            {
                get 
                {
                    if (mState == null)
                        mState = new DbObjectState(this, "XXX");
                    return mState;
                }
            }
        }

        private class Customer : DbObject
        {

            public Customer() : base("Customers") { }

            private DbFieldValue<string> mCustomerID = new DbFieldValue<string>("CustomerID", true);
            public string CustomerID
            {
                get { return mCustomerID.Value; }
                set { mCustomerID.Value = value; }
            }

            private DbFieldValue<string> mCompanyName = new DbFieldValue<string>("CompanyName");
            public string CompanyName
            {
                get { return mCompanyName.Value; }
                set { mCompanyName.Value = value; }
            }

            private DbFieldValue<string> mContactName = new DbFieldValue<string>("ContactName");
            public string ContactName
            {
                get { return mContactName.Value; }
                set { mContactName.Value = value; }
            }

            private DbFieldValue<string> mContactTitle = new DbFieldValue<string>("ContactTitle");
            public string ContactTitle
            {
                get { return mContactTitle.Value; }
                set { mContactTitle.Value = value; }
            }

            private DbFieldValue<string> mAddress = new DbFieldValue<string>("Address");
            public string Address
            {
                get { return mAddress.Value; }
                set { mAddress.Value = value; }
            }

            private DbFieldValue<string> mCity = new DbFieldValue<string>("City");
            public string City
            {
                get { return mCity.Value; }
                set { mCity.Value = value; }
            }

            private DbFieldValue<string> mRegion = new DbFieldValue<string>("Region");
            public string Region
            {
                get { return mRegion.Value; }
                set { mRegion.Value = value; }
            }

            private DbFieldValue<string> mPostalCode = new DbFieldValue<string>("PostalCode");
            public string PostalCode
            {
                get { return mPostalCode.Value; }
                set { mPostalCode.Value = value; }
            }

            private DbFieldValue<string> mCountry = new DbFieldValue<string>("Country");
            public string Country
            {
                get { return mCountry.Value; }
                set { mCountry.Value = value; }
            }

            private DbFieldValue<string> mPhone = new DbFieldValue<string>("Phone");
            public string Phone
            {
                get { return mPhone.Value; }
                set { mPhone.Value = value; }
            }

            private DbFieldValue<string> mFax = new DbFieldValue<string>("Fax");
            public string Fax
            {
                get { return mFax.Value; }
                set { mFax.Value = value; }
            }
            
        }

        private class Shipper : DbObject
        {

            public Shipper() : base("Shippers") { }

            private DbIdentityFieldValue mShipperID = new DbIdentityFieldValue("ShipperID");
            public int ShipperID
            {
                get { return mShipperID.Value; }
                set { mShipperID.Value = value; }
            }

            private DbFieldValue<string> mCompanyName = new DbFieldValue<string>("CompanyName");
            public string CompanyName
            {
                get { return mCompanyName.Value; }
                set { mCompanyName.Value = value; }
            }

            private DbFieldValue<string> mPhone = new DbFieldValue<string>("Phone");
            public string Phone
            {
                get { return mPhone.Value; }
                set { mPhone.Value = value; }
            }

        }

    }
}
