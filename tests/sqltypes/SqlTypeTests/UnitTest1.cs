using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlClient;
using DAO.DatabaseVO;
using DAO.DAO;
using System.Reflection;

namespace SqlTypeTests {
  /// <summary>
  /// Summary description for UnitTest1
  /// </summary>
  [TestClass]
  public class UnitTest1 {
    public UnitTest1() {
      //
      // TODO: Add constructor logic here
      //
    }

    private TestContext testContextInstance;

    /// <summary>
    ///Gets or sets the test context which provides
    ///information about and functionality for the current test run.
    ///</summary>
    public TestContext TestContext {
      get {
        return testContextInstance;
      }
      set {
        testContextInstance = value;
      }
    }

    #region Additional test attributes
    //
    // You can use the following additional attributes as you write your tests:
    //
    // Use ClassInitialize to run code before running the first test in the class
    // [ClassInitialize()]
    // public static void MyClassInitialize(TestContext testContext) { }
    //
    // Use ClassCleanup to run code after all tests in a class have run
    // [ClassCleanup()]
    // public static void MyClassCleanup() { }
    //
    // Use TestInitialize to run code before running each test 
    // [TestInitialize()]
    // public void MyTestInitialize() { }
    //
    // Use TestCleanup to run code after each test has run
    // [TestCleanup()]
    // public void MyTestCleanup() { }
    //
    #endregion

    [TestMethod]
    public void InsertTest() {
      SqlConnection conn = new SqlConnection("server=(local);user=automegenerator;pwd=123456;initial catalog=SqlTypes");
      conn.Open();
      
      TypeTestDatabaseVO vo = new TypeTestDatabaseVO();
      vo.BigIntTest = 10654654;
      vo.BitTest = true;
      vo.CharTest = "Char";
      vo.DateTimeTest = DateTime.Now;
      vo.DecimalTest = 10.5M;
      vo.FloatTest = 10.5F;
      vo.MoneyTest = 5.5M;
      vo.NCharTest = "N";
      vo.NTextTest = "NText";
      vo.NumericTest = 5.5M;
      vo.NVarcharTest = "NVarchar";
      vo.RealTest = 10.5F;
      //Using a Date/time with "0" seconds. Will truncate when inserted on database.
      vo.SmallDateTimeTest = new DateTime(2009, 01, 01, 10, 20, 0);
      vo.SmallIntTest = 5;
      vo.SmallMoneyTest = 5.5M;
      vo.TextTest = "Text";      
      vo.TinyIntTest = 5;
      vo.UniqueIdentifierTest = new Guid("{9FC64FF0-368D-4394-AF12-E4DD3A120C4F}");
      vo.VarCharTest = "Teste";
      
      TypeTestDAO dao = new TypeTestDAO();
      dao.Insert(conn, null, vo);
      
      TypeTestDatabaseVO vo2 = dao.Load(conn, null, vo.TypeTestID);
      foreach(PropertyInfo pi in vo.GetType().GetProperties()){
        object v1 = pi.GetValue(vo, null);
        object v2 = pi.GetValue(vo2, null);
        //Trimming strings, cause Char types autocompletes with space. This is by design.
        if (v1 is string)
          v1 = ((string)v1).Trim();
        if (v2 is string)
          v2 = ((string)v2).Trim();
        
        //Date comparisons as string. DateTimeKind is different when retrieved from database.        
        if (v1 is DateTime)
          v1 = ((DateTime)v1).ToString();
        if (v2 is DateTime)
          v2 = ((DateTime)v2).ToString();
        Assert.AreEqual(v1, v2);
      }
    }
  }
}
