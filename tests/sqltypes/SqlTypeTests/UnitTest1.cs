using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlClient;
using DAO.DatabaseVO;
using DAO.DAO;
using System.Reflection;
using DAO.ProcInputVO;
using DAO.ProcedureDAO;
using DAO.ProcOutputVO;

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
    public void InsertUpdateDeleteTest() {
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
        //Trimming strings, because Char types autocompletes with space. This is by design.
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
      
      vo2 = dao.Load(conn, null, vo.TypeTestID);
      vo2.BigIntTest = 10;
      vo2.BitTest = false;
      vo2.CharTest = "asdf";
      vo2.DateTimeTest = DateTime.Now;
      vo2.DecimalTest = 12.5M;
      vo2.FloatTest = 11.5F;
      vo2.MoneyTest = 55.5M;
      vo2.NCharTest = "a";
      vo2.NTextTest = "There goes a very long text (ntext)";
      vo2.NumericTest = 654.5M;
      vo2.NVarcharTest = "NVarchar value";
      vo2.RealTest = 10.50F;
      //Using a Date/time with "0" seconds. Will truncate when inserted on database.
      vo2.SmallDateTimeTest = new DateTime(2009, 02, 10, 05, 15, 0);
      vo2.SmallIntTest = 2;
      vo2.SmallMoneyTest = 15.5M;
      vo2.TextTest = "There goes a very long text (text)";
      vo2.TinyIntTest = 15;
      vo2.UniqueIdentifierTest = new Guid("{BFB2F594-2DDE-45c2-A19A-85C1C6684FA8}");
      vo2.VarCharTest = "Varchar value";
      dao.Update(conn, null, vo2);
      
      TypeTestDatabaseVO vo3 = dao.Load(conn, null, vo2.TypeTestID);

      foreach (PropertyInfo pi in vo.GetType().GetProperties()) {
        object v1 = pi.GetValue(vo2, null);
        object v2 = pi.GetValue(vo3, null);
        //Trimming strings, because Char types autocompletes with space. This is by design.
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
      
      dao.Delete(conn, null, vo2);
      TypeTestDatabaseVO vo4 = dao.Load(conn, null, vo2.TypeTestID);
      Assert.AreEqual(vo4, null);
      conn.Close();
    }

    [TestMethod]
    public void TransactionTest() {
      SqlConnection conn = new SqlConnection("server=(local);user=automegenerator;pwd=123456;initial catalog=SqlTypes");
      conn.Open();
      
      SqlTransaction trans = conn.BeginTransaction();

      TypeTestDatabaseVO vo = new TypeTestDatabaseVO();
      
      TypeTestDAO dao = new TypeTestDAO();
      dao.Insert(conn, trans, vo);
      
      int ID = vo.TypeTestID.Value;
      
      trans.Rollback();
      
      TypeTestDatabaseVO vo2 = dao.Load(conn, null, ID);
      Assert.AreEqual(vo2, null);      
      conn.Close();
    }
    
    [TestMethod]
    public void ProcedureWithoutRecordSetTest(){
      SqlConnection conn = new SqlConnection("server=(local);user=automegenerator;pwd=123456;initial catalog=SqlTypes");
      conn.Open();
      
      TypeTestWithoutRecordSetProcInputVO parms = new TypeTestWithoutRecordSetProcInputVO();
      
      TypeTestWithoutRecordSetProcedureDAO dao = new TypeTestWithoutRecordSetProcedureDAO();
      dao.Execute(conn, null, parms);
      
      Assert.AreNotEqual(parms.RETURN_VALUE, null);
      Assert.AreNotEqual(parms.TypeTestID, null);
      Assert.AreEqual(parms.RETURN_VALUE, parms.TypeTestID);    
    }

    [TestMethod]
    public void ProcedureWithOneRecordSetTest() {
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

      TypeTestWithOneRecordSetProcInputVO parms = new TypeTestWithOneRecordSetProcInputVO();
      parms.TypeTestID = vo.TypeTestID;

      TypeTestWithOneRecordSetProcedureDAO dao2 = new TypeTestWithOneRecordSetProcedureDAO();
      List<TypeTestWithOneRecordSetProcOutputVO> l = dao2.Execute(conn, null, parms);
      
      Assert.AreEqual(l.Count, 1);
      
      foreach (PropertyInfo pi in vo.GetType().GetProperties()) {
        PropertyInfo pi2 = l[0].GetType().GetProperty(pi.Name);
        object v1 = pi.GetValue(vo, null);
        object v2 = pi2.GetValue(l[0], null);
        //Trimming strings, because Char types autocompletes with space. This is by design.
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

    [TestMethod]
    public void ProcedureWithTwoRecordSetsTest() {
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

      TypeTestWithTwoRecordSetsProcInputVO parms = new TypeTestWithTwoRecordSetsProcInputVO();
      parms.TypeTestID = vo.TypeTestID;

      TypeTestWithTwoRecordSetsProcedureDAO dao2 = new TypeTestWithTwoRecordSetsProcedureDAO();
      List<object> result = dao2.Execute(conn, null, parms);
      List<TypeTestWithTwoRecordSetsRS1ProcOutputVO> l0 = (List<TypeTestWithTwoRecordSetsRS1ProcOutputVO>) result[0];
      List<TypeTestWithTwoRecordSetsRS2ProcOutputVO> l1 = (List<TypeTestWithTwoRecordSetsRS2ProcOutputVO>)result[1];

      Assert.AreEqual(l0.Count, 1);
      Assert.AreEqual(l1.Count, 1);

      foreach (PropertyInfo pi in vo.GetType().GetProperties()) {
        PropertyInfo pi2 = l0[0].GetType().GetProperty(pi.Name);
        object v1 = pi.GetValue(vo, null);
        object v2 = pi2.GetValue(l0[0], null);
        //Trimming strings, because Char types autocompletes with space. This is by design.
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

      foreach (PropertyInfo pi in vo.GetType().GetProperties()) {
        PropertyInfo pi2 = l1[0].GetType().GetProperty(pi.Name);
        object v1 = pi.GetValue(vo, null);
        object v2 = pi2.GetValue(l1[0], null);
        //Trimming strings, because Char types autocompletes with space. This is by design.
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
