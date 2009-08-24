namespace DAO.DAO {
  using DatabaseVO;
  using System.Collections.Generic;
  
  
  public class TypeTestDAO {
    
    public static string Aster = @"TypeTestID, BigIntTest, BinaryTest, BitTest, CharTest, DateTimeTest, DecimalTest, FloatTest, ImageTest, MoneyTest, NCharTest, NTextTest, NumericTest, NVarcharTest, RealTest, SmallDateTimeTest, SmallIntTest, SmallMoneyTest, TextTest, TinyIntTest, VarBinaryTest, VarCharTest, UniqueIdentifierTest, XmlTest, TimeStampTest";
    
    public virtual TypeTestDatabaseVO Load(System.Data.SqlClient.SqlConnection Connection, System.Data.SqlClient.SqlTransaction Transaction, System.Nullable<int> TypeTestID) {
      System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(@"select TypeTestID, BigIntTest, BitTest, CharTest, DateTimeTest, DecimalTest, FloatTest, MoneyTest, NCharTest, NTextTest, NumericTest, NVarcharTest, RealTest, SmallDateTimeTest, SmallIntTest, SmallMoneyTest, TextTest, TinyIntTest, VarCharTest, UniqueIdentifierTest from TypeTest where TypeTestID = @TypeTestID", Connection, Transaction);
      System.Data.SqlClient.SqlParameter p1 = new System.Data.SqlClient.SqlParameter();
      p1.ParameterName = "@TypeTestID";
      p1.SqlDbType = System.Data.SqlDbType.Int;
      p1.Size = 4;
      p1.Value = TypeTestID;
      cmd.Parameters.Add(p1);
      System.Data.SqlClient.SqlDataReader rd = cmd.ExecuteReader();
      if (rd.Read()) {
        TypeTestDatabaseVO vo = new TypeTestDatabaseVO();
        if (rd.IsDBNull(0)) {
          vo.TypeTestID = null;
        }
        else {
          vo.TypeTestID = rd.GetInt32(0);
        }
        if (rd.IsDBNull(1)) {
          vo.BigIntTest = null;
        }
        else {
          vo.BigIntTest = rd.GetInt64(1);
        }
        if (rd.IsDBNull(2)) {
          vo.BitTest = null;
        }
        else {
          vo.BitTest = rd.GetBoolean(2);
        }
        if (rd.IsDBNull(3)) {
          vo.CharTest = null;
        }
        else {
          vo.CharTest = rd.GetString(3);
        }
        if (rd.IsDBNull(4)) {
          vo.DateTimeTest = null;
        }
        else {
          vo.DateTimeTest = rd.GetDateTime(4);
        }
        if (rd.IsDBNull(5)) {
          vo.DecimalTest = null;
        }
        else {
          vo.DecimalTest = rd.GetDecimal(5);
        }
        if (rd.IsDBNull(6)) {
          vo.FloatTest = null;
        }
        else {
          vo.FloatTest = rd.GetDouble(6);
        }
        if (rd.IsDBNull(7)) {
          vo.MoneyTest = null;
        }
        else {
          vo.MoneyTest = rd.GetDecimal(7);
        }
        if (rd.IsDBNull(8)) {
          vo.NCharTest = null;
        }
        else {
          vo.NCharTest = rd.GetString(8);
        }
        if (rd.IsDBNull(9)) {
          vo.NTextTest = null;
        }
        else {
          vo.NTextTest = rd.GetString(9);
        }
        if (rd.IsDBNull(10)) {
          vo.NumericTest = null;
        }
        else {
          vo.NumericTest = rd.GetDecimal(10);
        }
        if (rd.IsDBNull(11)) {
          vo.NVarcharTest = null;
        }
        else {
          vo.NVarcharTest = rd.GetString(11);
        }
        if (rd.IsDBNull(12)) {
          vo.RealTest = null;
        }
        else {
          vo.RealTest = rd.GetFloat(12);
        }
        if (rd.IsDBNull(13)) {
          vo.SmallDateTimeTest = null;
        }
        else {
          vo.SmallDateTimeTest = rd.GetDateTime(13);
        }
        if (rd.IsDBNull(14)) {
          vo.SmallIntTest = null;
        }
        else {
          vo.SmallIntTest = rd.GetInt16(14);
        }
        if (rd.IsDBNull(15)) {
          vo.SmallMoneyTest = null;
        }
        else {
          vo.SmallMoneyTest = rd.GetDecimal(15);
        }
        if (rd.IsDBNull(16)) {
          vo.TextTest = null;
        }
        else {
          vo.TextTest = rd.GetString(16);
        }
        if (rd.IsDBNull(17)) {
          vo.TinyIntTest = null;
        }
        else {
          vo.TinyIntTest = rd.GetByte(17);
        }
        if (rd.IsDBNull(18)) {
          vo.VarCharTest = null;
        }
        else {
          vo.VarCharTest = rd.GetString(18);
        }
        if (rd.IsDBNull(19)) {
          vo.UniqueIdentifierTest = null;
        }
        else {
          vo.UniqueIdentifierTest = rd.GetGuid(19);
        }
        rd.Close();
        return vo;
      }
      else {
        rd.Close();
        return null;
      }
    }
    
    public virtual void Insert(System.Data.SqlClient.SqlConnection Connection, System.Data.SqlClient.SqlTransaction Transaction, TypeTestDatabaseVO vo) {
      System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(@"insert into TypeTest ( BigIntTest, BinaryTest, BitTest, CharTest, DateTimeTest, DecimalTest, FloatTest, ImageTest, MoneyTest, NCharTest, NTextTest, NumericTest, NVarcharTest, RealTest, SmallDateTimeTest, SmallIntTest, SmallMoneyTest, TextTest, TinyIntTest, VarBinaryTest, VarCharTest, UniqueIdentifierTest, XmlTest, TimeStampTest) values (@BigIntTest, null, @BitTest, @CharTest, @DateTimeTest, @DecimalTest, @FloatTest, null, @MoneyTest, @NCharTest, @NTextTest, @NumericTest, @NVarcharTest, @RealTest, @SmallDateTimeTest, @SmallIntTest, @SmallMoneyTest, @TextTest, @TinyIntTest, null, @VarCharTest, @UniqueIdentifierTest, null, null)
select SCOPE_IDENTITY()", Connection, Transaction);
      object val = null;
      if ((vo.BigIntTest == null)) {
        val = System.DBNull.Value;
      }
      else {
        val = vo.BigIntTest;
      }
      System.Data.SqlClient.SqlParameter p1 = new System.Data.SqlClient.SqlParameter();
      p1.ParameterName = "@BigIntTest";
      p1.SqlDbType = System.Data.SqlDbType.BigInt;
      p1.Size = 8;
      p1.Value = val;
      cmd.Parameters.Add(p1);
      if ((vo.BitTest == null)) {
        val = System.DBNull.Value;
      }
      else {
        val = vo.BitTest;
      }
      System.Data.SqlClient.SqlParameter p2 = new System.Data.SqlClient.SqlParameter();
      p2.ParameterName = "@BitTest";
      p2.SqlDbType = System.Data.SqlDbType.Bit;
      p2.Size = 1;
      p2.Value = val;
      cmd.Parameters.Add(p2);
      if ((vo.CharTest == null)) {
        val = System.DBNull.Value;
      }
      else {
        val = vo.CharTest;
      }
      System.Data.SqlClient.SqlParameter p3 = new System.Data.SqlClient.SqlParameter();
      p3.ParameterName = "@CharTest";
      p3.SqlDbType = System.Data.SqlDbType.Char;
      p3.Size = 10;
      p3.Value = val;
      cmd.Parameters.Add(p3);
      if ((vo.DateTimeTest == null)) {
        val = System.DBNull.Value;
      }
      else {
        val = vo.DateTimeTest;
      }
      System.Data.SqlClient.SqlParameter p4 = new System.Data.SqlClient.SqlParameter();
      p4.ParameterName = "@DateTimeTest";
      p4.SqlDbType = System.Data.SqlDbType.DateTime;
      p4.Size = 8;
      p4.Value = val;
      cmd.Parameters.Add(p4);
      if ((vo.DecimalTest == null)) {
        val = System.DBNull.Value;
      }
      else {
        val = vo.DecimalTest;
      }
      System.Data.SqlClient.SqlParameter p5 = new System.Data.SqlClient.SqlParameter();
      p5.ParameterName = "@DecimalTest";
      p5.SqlDbType = System.Data.SqlDbType.Decimal;
      p5.Size = 9;
      p5.Value = val;
      cmd.Parameters.Add(p5);
      if ((vo.FloatTest == null)) {
        val = System.DBNull.Value;
      }
      else {
        val = vo.FloatTest;
      }
      System.Data.SqlClient.SqlParameter p6 = new System.Data.SqlClient.SqlParameter();
      p6.ParameterName = "@FloatTest";
      p6.SqlDbType = System.Data.SqlDbType.Float;
      p6.Size = 8;
      p6.Value = val;
      cmd.Parameters.Add(p6);
      if ((vo.MoneyTest == null)) {
        val = System.DBNull.Value;
      }
      else {
        val = vo.MoneyTest;
      }
      System.Data.SqlClient.SqlParameter p7 = new System.Data.SqlClient.SqlParameter();
      p7.ParameterName = "@MoneyTest";
      p7.SqlDbType = System.Data.SqlDbType.Money;
      p7.Size = 8;
      p7.Value = val;
      cmd.Parameters.Add(p7);
      if ((vo.NCharTest == null)) {
        val = System.DBNull.Value;
      }
      else {
        val = vo.NCharTest;
      }
      System.Data.SqlClient.SqlParameter p8 = new System.Data.SqlClient.SqlParameter();
      p8.ParameterName = "@NCharTest";
      p8.SqlDbType = System.Data.SqlDbType.NChar;
      p8.Size = 2;
      p8.Value = val;
      cmd.Parameters.Add(p8);
      if ((vo.NTextTest == null)) {
        val = System.DBNull.Value;
      }
      else {
        val = vo.NTextTest;
      }
      System.Data.SqlClient.SqlParameter p9 = new System.Data.SqlClient.SqlParameter();
      p9.ParameterName = "@NTextTest";
      p9.SqlDbType = System.Data.SqlDbType.NText;
      p9.Size = 2147483647;
      p9.Value = val;
      cmd.Parameters.Add(p9);
      if ((vo.NumericTest == null)) {
        val = System.DBNull.Value;
      }
      else {
        val = vo.NumericTest;
      }
      System.Data.SqlClient.SqlParameter p10 = new System.Data.SqlClient.SqlParameter();
      p10.ParameterName = "@NumericTest";
      p10.SqlDbType = System.Data.SqlDbType.Decimal;
      p10.Size = 9;
      p10.Value = val;
      cmd.Parameters.Add(p10);
      if ((vo.NVarcharTest == null)) {
        val = System.DBNull.Value;
      }
      else {
        val = vo.NVarcharTest;
      }
      System.Data.SqlClient.SqlParameter p11 = new System.Data.SqlClient.SqlParameter();
      p11.ParameterName = "@NVarcharTest";
      p11.SqlDbType = System.Data.SqlDbType.NVarChar;
      p11.Size = 200;
      p11.Value = val;
      cmd.Parameters.Add(p11);
      if ((vo.RealTest == null)) {
        val = System.DBNull.Value;
      }
      else {
        val = vo.RealTest;
      }
      System.Data.SqlClient.SqlParameter p12 = new System.Data.SqlClient.SqlParameter();
      p12.ParameterName = "@RealTest";
      p12.SqlDbType = System.Data.SqlDbType.Real;
      p12.Size = 4;
      p12.Value = val;
      cmd.Parameters.Add(p12);
      if ((vo.SmallDateTimeTest == null)) {
        val = System.DBNull.Value;
      }
      else {
        val = vo.SmallDateTimeTest;
      }
      System.Data.SqlClient.SqlParameter p13 = new System.Data.SqlClient.SqlParameter();
      p13.ParameterName = "@SmallDateTimeTest";
      p13.SqlDbType = System.Data.SqlDbType.SmallDateTime;
      p13.Size = 4;
      p13.Value = val;
      cmd.Parameters.Add(p13);
      if ((vo.SmallIntTest == null)) {
        val = System.DBNull.Value;
      }
      else {
        val = vo.SmallIntTest;
      }
      System.Data.SqlClient.SqlParameter p14 = new System.Data.SqlClient.SqlParameter();
      p14.ParameterName = "@SmallIntTest";
      p14.SqlDbType = System.Data.SqlDbType.SmallInt;
      p14.Size = 2;
      p14.Value = val;
      cmd.Parameters.Add(p14);
      if ((vo.SmallMoneyTest == null)) {
        val = System.DBNull.Value;
      }
      else {
        val = vo.SmallMoneyTest;
      }
      System.Data.SqlClient.SqlParameter p15 = new System.Data.SqlClient.SqlParameter();
      p15.ParameterName = "@SmallMoneyTest";
      p15.SqlDbType = System.Data.SqlDbType.SmallMoney;
      p15.Size = 4;
      p15.Value = val;
      cmd.Parameters.Add(p15);
      if ((vo.TextTest == null)) {
        val = System.DBNull.Value;
      }
      else {
        val = vo.TextTest;
      }
      System.Data.SqlClient.SqlParameter p16 = new System.Data.SqlClient.SqlParameter();
      p16.ParameterName = "@TextTest";
      p16.SqlDbType = System.Data.SqlDbType.Text;
      p16.Size = 2147483647;
      p16.Value = val;
      cmd.Parameters.Add(p16);
      if ((vo.TinyIntTest == null)) {
        val = System.DBNull.Value;
      }
      else {
        val = vo.TinyIntTest;
      }
      System.Data.SqlClient.SqlParameter p17 = new System.Data.SqlClient.SqlParameter();
      p17.ParameterName = "@TinyIntTest";
      p17.SqlDbType = System.Data.SqlDbType.TinyInt;
      p17.Size = 1;
      p17.Value = val;
      cmd.Parameters.Add(p17);
      if ((vo.VarCharTest == null)) {
        val = System.DBNull.Value;
      }
      else {
        val = vo.VarCharTest;
      }
      System.Data.SqlClient.SqlParameter p18 = new System.Data.SqlClient.SqlParameter();
      p18.ParameterName = "@VarCharTest";
      p18.SqlDbType = System.Data.SqlDbType.VarChar;
      p18.Size = 100;
      p18.Value = val;
      cmd.Parameters.Add(p18);
      if ((vo.UniqueIdentifierTest == null)) {
        val = System.DBNull.Value;
      }
      else {
        val = vo.UniqueIdentifierTest;
      }
      System.Data.SqlClient.SqlParameter p19 = new System.Data.SqlClient.SqlParameter();
      p19.ParameterName = "@UniqueIdentifierTest";
      p19.SqlDbType = System.Data.SqlDbType.UniqueIdentifier;
      p19.Size = 16;
      p19.Value = val;
      cmd.Parameters.Add(p19);
      vo.TypeTestID = decimal.ToInt32(((decimal)(cmd.ExecuteScalar())));
    }
    
    public virtual int Update(System.Data.SqlClient.SqlConnection Connection, System.Data.SqlClient.SqlTransaction Transaction, TypeTestDatabaseVO vo) {
      System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(@"update TypeTest set  BigIntTest = @BigIntTest, BitTest = @BitTest, CharTest = @CharTest, DateTimeTest = @DateTimeTest, DecimalTest = @DecimalTest, FloatTest = @FloatTest, MoneyTest = @MoneyTest, NCharTest = @NCharTest, NTextTest = @NTextTest, NumericTest = @NumericTest, NVarcharTest = @NVarcharTest, RealTest = @RealTest, SmallDateTimeTest = @SmallDateTimeTest, SmallIntTest = @SmallIntTest, SmallMoneyTest = @SmallMoneyTest, TextTest = @TextTest, TinyIntTest = @TinyIntTest, VarCharTest = @VarCharTest, UniqueIdentifierTest = @UniqueIdentifierTest where TypeTestID = @TypeTestID", Connection, Transaction);
      object val = null;
      if ((vo.TypeTestID == null)) {
        val = System.DBNull.Value;
      }
      else {
        val = vo.TypeTestID;
      }
      System.Data.SqlClient.SqlParameter p1 = new System.Data.SqlClient.SqlParameter();
      p1.ParameterName = "@TypeTestID";
      p1.SqlDbType = System.Data.SqlDbType.Int;
      p1.Size = 4;
      p1.Value = val;
      cmd.Parameters.Add(p1);
      if ((vo.BigIntTest == null)) {
        val = System.DBNull.Value;
      }
      else {
        val = vo.BigIntTest;
      }
      System.Data.SqlClient.SqlParameter p2 = new System.Data.SqlClient.SqlParameter();
      p2.ParameterName = "@BigIntTest";
      p2.SqlDbType = System.Data.SqlDbType.BigInt;
      p2.Size = 8;
      p2.Value = val;
      cmd.Parameters.Add(p2);
      if ((vo.BitTest == null)) {
        val = System.DBNull.Value;
      }
      else {
        val = vo.BitTest;
      }
      System.Data.SqlClient.SqlParameter p3 = new System.Data.SqlClient.SqlParameter();
      p3.ParameterName = "@BitTest";
      p3.SqlDbType = System.Data.SqlDbType.Bit;
      p3.Size = 1;
      p3.Value = val;
      cmd.Parameters.Add(p3);
      if ((vo.CharTest == null)) {
        val = System.DBNull.Value;
      }
      else {
        val = vo.CharTest;
      }
      System.Data.SqlClient.SqlParameter p4 = new System.Data.SqlClient.SqlParameter();
      p4.ParameterName = "@CharTest";
      p4.SqlDbType = System.Data.SqlDbType.Char;
      p4.Size = 10;
      p4.Value = val;
      cmd.Parameters.Add(p4);
      if ((vo.DateTimeTest == null)) {
        val = System.DBNull.Value;
      }
      else {
        val = vo.DateTimeTest;
      }
      System.Data.SqlClient.SqlParameter p5 = new System.Data.SqlClient.SqlParameter();
      p5.ParameterName = "@DateTimeTest";
      p5.SqlDbType = System.Data.SqlDbType.DateTime;
      p5.Size = 8;
      p5.Value = val;
      cmd.Parameters.Add(p5);
      if ((vo.DecimalTest == null)) {
        val = System.DBNull.Value;
      }
      else {
        val = vo.DecimalTest;
      }
      System.Data.SqlClient.SqlParameter p6 = new System.Data.SqlClient.SqlParameter();
      p6.ParameterName = "@DecimalTest";
      p6.SqlDbType = System.Data.SqlDbType.Decimal;
      p6.Size = 9;
      p6.Value = val;
      cmd.Parameters.Add(p6);
      if ((vo.FloatTest == null)) {
        val = System.DBNull.Value;
      }
      else {
        val = vo.FloatTest;
      }
      System.Data.SqlClient.SqlParameter p7 = new System.Data.SqlClient.SqlParameter();
      p7.ParameterName = "@FloatTest";
      p7.SqlDbType = System.Data.SqlDbType.Float;
      p7.Size = 8;
      p7.Value = val;
      cmd.Parameters.Add(p7);
      if ((vo.MoneyTest == null)) {
        val = System.DBNull.Value;
      }
      else {
        val = vo.MoneyTest;
      }
      System.Data.SqlClient.SqlParameter p8 = new System.Data.SqlClient.SqlParameter();
      p8.ParameterName = "@MoneyTest";
      p8.SqlDbType = System.Data.SqlDbType.Money;
      p8.Size = 8;
      p8.Value = val;
      cmd.Parameters.Add(p8);
      if ((vo.NCharTest == null)) {
        val = System.DBNull.Value;
      }
      else {
        val = vo.NCharTest;
      }
      System.Data.SqlClient.SqlParameter p9 = new System.Data.SqlClient.SqlParameter();
      p9.ParameterName = "@NCharTest";
      p9.SqlDbType = System.Data.SqlDbType.NChar;
      p9.Size = 2;
      p9.Value = val;
      cmd.Parameters.Add(p9);
      if ((vo.NTextTest == null)) {
        val = System.DBNull.Value;
      }
      else {
        val = vo.NTextTest;
      }
      System.Data.SqlClient.SqlParameter p10 = new System.Data.SqlClient.SqlParameter();
      p10.ParameterName = "@NTextTest";
      p10.SqlDbType = System.Data.SqlDbType.NText;
      p10.Size = 2147483647;
      p10.Value = val;
      cmd.Parameters.Add(p10);
      if ((vo.NumericTest == null)) {
        val = System.DBNull.Value;
      }
      else {
        val = vo.NumericTest;
      }
      System.Data.SqlClient.SqlParameter p11 = new System.Data.SqlClient.SqlParameter();
      p11.ParameterName = "@NumericTest";
      p11.SqlDbType = System.Data.SqlDbType.Decimal;
      p11.Size = 9;
      p11.Value = val;
      cmd.Parameters.Add(p11);
      if ((vo.NVarcharTest == null)) {
        val = System.DBNull.Value;
      }
      else {
        val = vo.NVarcharTest;
      }
      System.Data.SqlClient.SqlParameter p12 = new System.Data.SqlClient.SqlParameter();
      p12.ParameterName = "@NVarcharTest";
      p12.SqlDbType = System.Data.SqlDbType.NVarChar;
      p12.Size = 200;
      p12.Value = val;
      cmd.Parameters.Add(p12);
      if ((vo.RealTest == null)) {
        val = System.DBNull.Value;
      }
      else {
        val = vo.RealTest;
      }
      System.Data.SqlClient.SqlParameter p13 = new System.Data.SqlClient.SqlParameter();
      p13.ParameterName = "@RealTest";
      p13.SqlDbType = System.Data.SqlDbType.Real;
      p13.Size = 4;
      p13.Value = val;
      cmd.Parameters.Add(p13);
      if ((vo.SmallDateTimeTest == null)) {
        val = System.DBNull.Value;
      }
      else {
        val = vo.SmallDateTimeTest;
      }
      System.Data.SqlClient.SqlParameter p14 = new System.Data.SqlClient.SqlParameter();
      p14.ParameterName = "@SmallDateTimeTest";
      p14.SqlDbType = System.Data.SqlDbType.SmallDateTime;
      p14.Size = 4;
      p14.Value = val;
      cmd.Parameters.Add(p14);
      if ((vo.SmallIntTest == null)) {
        val = System.DBNull.Value;
      }
      else {
        val = vo.SmallIntTest;
      }
      System.Data.SqlClient.SqlParameter p15 = new System.Data.SqlClient.SqlParameter();
      p15.ParameterName = "@SmallIntTest";
      p15.SqlDbType = System.Data.SqlDbType.SmallInt;
      p15.Size = 2;
      p15.Value = val;
      cmd.Parameters.Add(p15);
      if ((vo.SmallMoneyTest == null)) {
        val = System.DBNull.Value;
      }
      else {
        val = vo.SmallMoneyTest;
      }
      System.Data.SqlClient.SqlParameter p16 = new System.Data.SqlClient.SqlParameter();
      p16.ParameterName = "@SmallMoneyTest";
      p16.SqlDbType = System.Data.SqlDbType.SmallMoney;
      p16.Size = 4;
      p16.Value = val;
      cmd.Parameters.Add(p16);
      if ((vo.TextTest == null)) {
        val = System.DBNull.Value;
      }
      else {
        val = vo.TextTest;
      }
      System.Data.SqlClient.SqlParameter p17 = new System.Data.SqlClient.SqlParameter();
      p17.ParameterName = "@TextTest";
      p17.SqlDbType = System.Data.SqlDbType.Text;
      p17.Size = 2147483647;
      p17.Value = val;
      cmd.Parameters.Add(p17);
      if ((vo.TinyIntTest == null)) {
        val = System.DBNull.Value;
      }
      else {
        val = vo.TinyIntTest;
      }
      System.Data.SqlClient.SqlParameter p18 = new System.Data.SqlClient.SqlParameter();
      p18.ParameterName = "@TinyIntTest";
      p18.SqlDbType = System.Data.SqlDbType.TinyInt;
      p18.Size = 1;
      p18.Value = val;
      cmd.Parameters.Add(p18);
      if ((vo.VarCharTest == null)) {
        val = System.DBNull.Value;
      }
      else {
        val = vo.VarCharTest;
      }
      System.Data.SqlClient.SqlParameter p19 = new System.Data.SqlClient.SqlParameter();
      p19.ParameterName = "@VarCharTest";
      p19.SqlDbType = System.Data.SqlDbType.VarChar;
      p19.Size = 100;
      p19.Value = val;
      cmd.Parameters.Add(p19);
      if ((vo.UniqueIdentifierTest == null)) {
        val = System.DBNull.Value;
      }
      else {
        val = vo.UniqueIdentifierTest;
      }
      System.Data.SqlClient.SqlParameter p20 = new System.Data.SqlClient.SqlParameter();
      p20.ParameterName = "@UniqueIdentifierTest";
      p20.SqlDbType = System.Data.SqlDbType.UniqueIdentifier;
      p20.Size = 16;
      p20.Value = val;
      cmd.Parameters.Add(p20);
      return cmd.ExecuteNonQuery();
    }
    
    public virtual int Delete(System.Data.SqlClient.SqlConnection Connection, System.Data.SqlClient.SqlTransaction Transaction, TypeTestDatabaseVO vo) {
      System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand("delete from TypeTest  where TypeTestID = @TypeTestID", Connection, Transaction);
      System.Data.SqlClient.SqlParameter p1 = new System.Data.SqlClient.SqlParameter();
      p1.ParameterName = "@TypeTestID";
      p1.SqlDbType = System.Data.SqlDbType.Int;
      p1.Size = 4;
      p1.Value = vo.TypeTestID;
      cmd.Parameters.Add(p1);
      return cmd.ExecuteNonQuery();
    }
  }
  
  public class TypeTestBinaryTestBlobStream : System.IO.Stream {
    
    private int _ReadPosition;
    
    private System.Data.SqlClient.SqlConnection _Connection;
    
    private System.Data.SqlClient.SqlTransaction _Transaction;
    
    private System.Nullable<int> _TypeTestID;
    
    private object _TextPtrOut;
    
    private object _LengthOut;
    
    public TypeTestBinaryTestBlobStream(System.Data.SqlClient.SqlConnection Connection, System.Data.SqlClient.SqlTransaction Transaction, System.Nullable<int> TypeTestID) {
      _Connection = Connection;
      _Transaction = Transaction;
      _TypeTestID = TypeTestID;
      this.UpdateTextPointer();
    }
    
    public override bool CanRead {
      get {
        return true;
      }
    }
    
    public override bool CanWrite {
      get {
        return true;
      }
    }
    
    public override bool CanSeek {
      get {
        return false;
      }
    }
    
    public override long Position {
      get {
        return System.Convert.ToInt64(_LengthOut);
      }
      set {
        throw new System.NotImplementedException();
      }
    }
    
    public override long Length {
      get {
        return ((long)(_LengthOut));
      }
    }
    
    private void UpdateTextPointer() {
      System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand("select   @Ptr=TEXTPTR(BinaryTest),   @Length=DATALENGTH(BinaryTest) from TypeTest" +
          " where TypeTestID = @TypeTestID", _Connection, _Transaction);
      System.Data.SqlClient.SqlParameter ptrParam = new System.Data.SqlClient.SqlParameter();
      ptrParam.ParameterName = "@Ptr";
      ptrParam.SqlDbType = System.Data.SqlDbType.VarBinary;
      ptrParam.Size = 100;
      ptrParam.Direction = System.Data.ParameterDirection.Output;
      cmd.Parameters.Add(ptrParam);
      System.Data.SqlClient.SqlParameter lengthParam = new System.Data.SqlClient.SqlParameter();
      lengthParam.ParameterName = "@Length";
      lengthParam.SqlDbType = System.Data.SqlDbType.Int;
      lengthParam.Size = 4;
      lengthParam.Direction = System.Data.ParameterDirection.Output;
      cmd.Parameters.Add(lengthParam);
      System.Data.SqlClient.SqlParameter pkParm1 = new System.Data.SqlClient.SqlParameter();
      pkParm1.ParameterName = "@TypeTestID";
      pkParm1.SqlDbType = System.Data.SqlDbType.Int;
      pkParm1.Size = 4;
      pkParm1.Value = _TypeTestID;
      cmd.Parameters.Add(pkParm1);
      cmd.ExecuteNonQuery();
      _TextPtrOut = ptrParam.Value;
      _LengthOut = lengthParam.Value;
    }
    
    public override void Write(byte[] buffer, int offset, int count) {
      if ((_TextPtrOut == System.DBNull.Value)) {
        System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand("update TypeTest set BinaryTest = @Chunk where TypeTestID = @TypeTestID", _Connection, _Transaction);
        System.Data.SqlClient.SqlParameter chunkParam = new System.Data.SqlClient.SqlParameter();
        chunkParam.ParameterName = "@Chunk";
        chunkParam.SqlDbType = System.Data.SqlDbType.Binary;
        chunkParam.Size = count;
        chunkParam.Value = buffer;
        chunkParam.Direction = System.Data.ParameterDirection.Input;
        cmd.Parameters.Add(chunkParam);
        System.Data.SqlClient.SqlParameter pkParm1 = new System.Data.SqlClient.SqlParameter();
        pkParm1.ParameterName = "@TypeTestID";
        pkParm1.SqlDbType = System.Data.SqlDbType.Int;
        pkParm1.Size = 4;
        pkParm1.Value = _TypeTestID;
        cmd.Parameters.Add(pkParm1);
        cmd.ExecuteNonQuery();
      }
      else {
        System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand("updatetext TypeTest.BinaryTest @Ptr @Offset 0 WITH LOG @Chunk", _Connection, _Transaction);
        System.Data.SqlClient.SqlParameter ptrParam = new System.Data.SqlClient.SqlParameter();
        ptrParam.ParameterName = "@Ptr";
        ptrParam.SqlDbType = System.Data.SqlDbType.VarBinary;
        ptrParam.Size = 16;
        ptrParam.Value = _TextPtrOut;
        ptrParam.Direction = System.Data.ParameterDirection.Input;
        cmd.Parameters.Add(ptrParam);
        System.Data.SqlClient.SqlParameter offsetParam = new System.Data.SqlClient.SqlParameter();
        offsetParam.ParameterName = "@Offset";
        offsetParam.SqlDbType = System.Data.SqlDbType.Int;
        offsetParam.Size = 4;
        offsetParam.Value = _LengthOut;
        offsetParam.Direction = System.Data.ParameterDirection.Input;
        cmd.Parameters.Add(offsetParam);
        System.Data.SqlClient.SqlParameter chunkParam = new System.Data.SqlClient.SqlParameter();
        chunkParam.ParameterName = "@Chunk";
        chunkParam.SqlDbType = System.Data.SqlDbType.Binary;
        chunkParam.Size = count;
        chunkParam.Value = buffer;
        chunkParam.Direction = System.Data.ParameterDirection.Input;
        cmd.Parameters.Add(chunkParam);
        cmd.ExecuteNonQuery();
      }
      this.UpdateTextPointer();
    }
    
    public override int Read(byte[] buffer, int offset, int count) {
      if ((_ReadPosition >= ((int)(_LengthOut)))) {
        return 0;
      }
      int bytesToRead;
      if (((_ReadPosition + count) 
            > ((int)(_LengthOut)))) {
        return 0;
      }
      else {
        bytesToRead = count;
      }
      System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand("readtext TypeTest.BinaryTest @Ptr @Offset @Size HOLDLOCK", _Connection, _Transaction);
      System.Data.SqlClient.SqlParameter ptrParam = new System.Data.SqlClient.SqlParameter();
      ptrParam.ParameterName = "@Ptr";
      ptrParam.SqlDbType = System.Data.SqlDbType.VarBinary;
      ptrParam.Size = 16;
      ptrParam.Value = _TextPtrOut;
      ptrParam.Direction = System.Data.ParameterDirection.Input;
      cmd.Parameters.Add(ptrParam);
      System.Data.SqlClient.SqlParameter offsetParam = new System.Data.SqlClient.SqlParameter();
      offsetParam.ParameterName = "@Offset";
      offsetParam.SqlDbType = System.Data.SqlDbType.Int;
      offsetParam.Size = 4;
      offsetParam.Value = _ReadPosition;
      offsetParam.Direction = System.Data.ParameterDirection.Input;
      cmd.Parameters.Add(offsetParam);
      System.Data.SqlClient.SqlParameter sizeParam = new System.Data.SqlClient.SqlParameter();
      sizeParam.ParameterName = "@Size";
      sizeParam.SqlDbType = System.Data.SqlDbType.Int;
      sizeParam.Size = 4;
      sizeParam.Value = bytesToRead;
      sizeParam.Direction = System.Data.ParameterDirection.Input;
      cmd.Parameters.Add(sizeParam);
      System.Data.SqlClient.SqlDataReader rd = cmd.ExecuteReader(System.Data.CommandBehavior.SingleResult);
      rd.Read();
      rd.GetBytes(0, 0, buffer, offset, bytesToRead);
      rd.Close();
      _ReadPosition = (_ReadPosition + bytesToRead);
      return bytesToRead;
    }
    
    public override void SetLength(long value) {
      throw new System.NotImplementedException();
    }
    
    public override long Seek(long offset, System.IO.SeekOrigin origin) {
      throw new System.NotImplementedException();
    }
    
    public override void Flush() {
    }
  }
  
  public class TypeTestImageTestBlobStream : System.IO.Stream {
    
    private int _ReadPosition;
    
    private System.Data.SqlClient.SqlConnection _Connection;
    
    private System.Data.SqlClient.SqlTransaction _Transaction;
    
    private System.Nullable<int> _TypeTestID;
    
    private object _TextPtrOut;
    
    private object _LengthOut;
    
    public TypeTestImageTestBlobStream(System.Data.SqlClient.SqlConnection Connection, System.Data.SqlClient.SqlTransaction Transaction, System.Nullable<int> TypeTestID) {
      _Connection = Connection;
      _Transaction = Transaction;
      _TypeTestID = TypeTestID;
      this.UpdateTextPointer();
    }
    
    public override bool CanRead {
      get {
        return true;
      }
    }
    
    public override bool CanWrite {
      get {
        return true;
      }
    }
    
    public override bool CanSeek {
      get {
        return false;
      }
    }
    
    public override long Position {
      get {
        return System.Convert.ToInt64(_LengthOut);
      }
      set {
        throw new System.NotImplementedException();
      }
    }
    
    public override long Length {
      get {
        return ((long)(_LengthOut));
      }
    }
    
    private void UpdateTextPointer() {
      System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand("select   @Ptr=TEXTPTR(ImageTest),   @Length=DATALENGTH(ImageTest) from TypeTest w" +
          "here TypeTestID = @TypeTestID", _Connection, _Transaction);
      System.Data.SqlClient.SqlParameter ptrParam = new System.Data.SqlClient.SqlParameter();
      ptrParam.ParameterName = "@Ptr";
      ptrParam.SqlDbType = System.Data.SqlDbType.VarBinary;
      ptrParam.Size = 100;
      ptrParam.Direction = System.Data.ParameterDirection.Output;
      cmd.Parameters.Add(ptrParam);
      System.Data.SqlClient.SqlParameter lengthParam = new System.Data.SqlClient.SqlParameter();
      lengthParam.ParameterName = "@Length";
      lengthParam.SqlDbType = System.Data.SqlDbType.Int;
      lengthParam.Size = 4;
      lengthParam.Direction = System.Data.ParameterDirection.Output;
      cmd.Parameters.Add(lengthParam);
      System.Data.SqlClient.SqlParameter pkParm1 = new System.Data.SqlClient.SqlParameter();
      pkParm1.ParameterName = "@TypeTestID";
      pkParm1.SqlDbType = System.Data.SqlDbType.Int;
      pkParm1.Size = 4;
      pkParm1.Value = _TypeTestID;
      cmd.Parameters.Add(pkParm1);
      cmd.ExecuteNonQuery();
      _TextPtrOut = ptrParam.Value;
      _LengthOut = lengthParam.Value;
    }
    
    public override void Write(byte[] buffer, int offset, int count) {
      if ((_TextPtrOut == System.DBNull.Value)) {
        System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand("update TypeTest set ImageTest = @Chunk where TypeTestID = @TypeTestID", _Connection, _Transaction);
        System.Data.SqlClient.SqlParameter chunkParam = new System.Data.SqlClient.SqlParameter();
        chunkParam.ParameterName = "@Chunk";
        chunkParam.SqlDbType = System.Data.SqlDbType.Binary;
        chunkParam.Size = count;
        chunkParam.Value = buffer;
        chunkParam.Direction = System.Data.ParameterDirection.Input;
        cmd.Parameters.Add(chunkParam);
        System.Data.SqlClient.SqlParameter pkParm1 = new System.Data.SqlClient.SqlParameter();
        pkParm1.ParameterName = "@TypeTestID";
        pkParm1.SqlDbType = System.Data.SqlDbType.Int;
        pkParm1.Size = 4;
        pkParm1.Value = _TypeTestID;
        cmd.Parameters.Add(pkParm1);
        cmd.ExecuteNonQuery();
      }
      else {
        System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand("updatetext TypeTest.ImageTest @Ptr @Offset 0 WITH LOG @Chunk", _Connection, _Transaction);
        System.Data.SqlClient.SqlParameter ptrParam = new System.Data.SqlClient.SqlParameter();
        ptrParam.ParameterName = "@Ptr";
        ptrParam.SqlDbType = System.Data.SqlDbType.VarBinary;
        ptrParam.Size = 16;
        ptrParam.Value = _TextPtrOut;
        ptrParam.Direction = System.Data.ParameterDirection.Input;
        cmd.Parameters.Add(ptrParam);
        System.Data.SqlClient.SqlParameter offsetParam = new System.Data.SqlClient.SqlParameter();
        offsetParam.ParameterName = "@Offset";
        offsetParam.SqlDbType = System.Data.SqlDbType.Int;
        offsetParam.Size = 4;
        offsetParam.Value = _LengthOut;
        offsetParam.Direction = System.Data.ParameterDirection.Input;
        cmd.Parameters.Add(offsetParam);
        System.Data.SqlClient.SqlParameter chunkParam = new System.Data.SqlClient.SqlParameter();
        chunkParam.ParameterName = "@Chunk";
        chunkParam.SqlDbType = System.Data.SqlDbType.Binary;
        chunkParam.Size = count;
        chunkParam.Value = buffer;
        chunkParam.Direction = System.Data.ParameterDirection.Input;
        cmd.Parameters.Add(chunkParam);
        cmd.ExecuteNonQuery();
      }
      this.UpdateTextPointer();
    }
    
    public override int Read(byte[] buffer, int offset, int count) {
      if ((_ReadPosition >= ((int)(_LengthOut)))) {
        return 0;
      }
      int bytesToRead;
      if (((_ReadPosition + count) 
            > ((int)(_LengthOut)))) {
        return 0;
      }
      else {
        bytesToRead = count;
      }
      System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand("readtext TypeTest.ImageTest @Ptr @Offset @Size HOLDLOCK", _Connection, _Transaction);
      System.Data.SqlClient.SqlParameter ptrParam = new System.Data.SqlClient.SqlParameter();
      ptrParam.ParameterName = "@Ptr";
      ptrParam.SqlDbType = System.Data.SqlDbType.VarBinary;
      ptrParam.Size = 16;
      ptrParam.Value = _TextPtrOut;
      ptrParam.Direction = System.Data.ParameterDirection.Input;
      cmd.Parameters.Add(ptrParam);
      System.Data.SqlClient.SqlParameter offsetParam = new System.Data.SqlClient.SqlParameter();
      offsetParam.ParameterName = "@Offset";
      offsetParam.SqlDbType = System.Data.SqlDbType.Int;
      offsetParam.Size = 4;
      offsetParam.Value = _ReadPosition;
      offsetParam.Direction = System.Data.ParameterDirection.Input;
      cmd.Parameters.Add(offsetParam);
      System.Data.SqlClient.SqlParameter sizeParam = new System.Data.SqlClient.SqlParameter();
      sizeParam.ParameterName = "@Size";
      sizeParam.SqlDbType = System.Data.SqlDbType.Int;
      sizeParam.Size = 4;
      sizeParam.Value = bytesToRead;
      sizeParam.Direction = System.Data.ParameterDirection.Input;
      cmd.Parameters.Add(sizeParam);
      System.Data.SqlClient.SqlDataReader rd = cmd.ExecuteReader(System.Data.CommandBehavior.SingleResult);
      rd.Read();
      rd.GetBytes(0, 0, buffer, offset, bytesToRead);
      rd.Close();
      _ReadPosition = (_ReadPosition + bytesToRead);
      return bytesToRead;
    }
    
    public override void SetLength(long value) {
      throw new System.NotImplementedException();
    }
    
    public override long Seek(long offset, System.IO.SeekOrigin origin) {
      throw new System.NotImplementedException();
    }
    
    public override void Flush() {
    }
  }
  
  public class TypeTestVarBinaryTestBlobStream : System.IO.Stream {
    
    private int _ReadPosition;
    
    private System.Data.SqlClient.SqlConnection _Connection;
    
    private System.Data.SqlClient.SqlTransaction _Transaction;
    
    private System.Nullable<int> _TypeTestID;
    
    private object _TextPtrOut;
    
    private object _LengthOut;
    
    public TypeTestVarBinaryTestBlobStream(System.Data.SqlClient.SqlConnection Connection, System.Data.SqlClient.SqlTransaction Transaction, System.Nullable<int> TypeTestID) {
      _Connection = Connection;
      _Transaction = Transaction;
      _TypeTestID = TypeTestID;
      this.UpdateTextPointer();
    }
    
    public override bool CanRead {
      get {
        return true;
      }
    }
    
    public override bool CanWrite {
      get {
        return true;
      }
    }
    
    public override bool CanSeek {
      get {
        return false;
      }
    }
    
    public override long Position {
      get {
        return System.Convert.ToInt64(_LengthOut);
      }
      set {
        throw new System.NotImplementedException();
      }
    }
    
    public override long Length {
      get {
        return ((long)(_LengthOut));
      }
    }
    
    private void UpdateTextPointer() {
      System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand("select   @Ptr=TEXTPTR(VarBinaryTest),   @Length=DATALENGTH(VarBinaryTest) from Ty" +
          "peTest where TypeTestID = @TypeTestID", _Connection, _Transaction);
      System.Data.SqlClient.SqlParameter ptrParam = new System.Data.SqlClient.SqlParameter();
      ptrParam.ParameterName = "@Ptr";
      ptrParam.SqlDbType = System.Data.SqlDbType.VarBinary;
      ptrParam.Size = 100;
      ptrParam.Direction = System.Data.ParameterDirection.Output;
      cmd.Parameters.Add(ptrParam);
      System.Data.SqlClient.SqlParameter lengthParam = new System.Data.SqlClient.SqlParameter();
      lengthParam.ParameterName = "@Length";
      lengthParam.SqlDbType = System.Data.SqlDbType.Int;
      lengthParam.Size = 4;
      lengthParam.Direction = System.Data.ParameterDirection.Output;
      cmd.Parameters.Add(lengthParam);
      System.Data.SqlClient.SqlParameter pkParm1 = new System.Data.SqlClient.SqlParameter();
      pkParm1.ParameterName = "@TypeTestID";
      pkParm1.SqlDbType = System.Data.SqlDbType.Int;
      pkParm1.Size = 4;
      pkParm1.Value = _TypeTestID;
      cmd.Parameters.Add(pkParm1);
      cmd.ExecuteNonQuery();
      _TextPtrOut = ptrParam.Value;
      _LengthOut = lengthParam.Value;
    }
    
    public override void Write(byte[] buffer, int offset, int count) {
      if ((_TextPtrOut == System.DBNull.Value)) {
        System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand("update TypeTest set VarBinaryTest = @Chunk where TypeTestID = @TypeTestID", _Connection, _Transaction);
        System.Data.SqlClient.SqlParameter chunkParam = new System.Data.SqlClient.SqlParameter();
        chunkParam.ParameterName = "@Chunk";
        chunkParam.SqlDbType = System.Data.SqlDbType.Binary;
        chunkParam.Size = count;
        chunkParam.Value = buffer;
        chunkParam.Direction = System.Data.ParameterDirection.Input;
        cmd.Parameters.Add(chunkParam);
        System.Data.SqlClient.SqlParameter pkParm1 = new System.Data.SqlClient.SqlParameter();
        pkParm1.ParameterName = "@TypeTestID";
        pkParm1.SqlDbType = System.Data.SqlDbType.Int;
        pkParm1.Size = 4;
        pkParm1.Value = _TypeTestID;
        cmd.Parameters.Add(pkParm1);
        cmd.ExecuteNonQuery();
      }
      else {
        System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand("updatetext TypeTest.VarBinaryTest @Ptr @Offset 0 WITH LOG @Chunk", _Connection, _Transaction);
        System.Data.SqlClient.SqlParameter ptrParam = new System.Data.SqlClient.SqlParameter();
        ptrParam.ParameterName = "@Ptr";
        ptrParam.SqlDbType = System.Data.SqlDbType.VarBinary;
        ptrParam.Size = 16;
        ptrParam.Value = _TextPtrOut;
        ptrParam.Direction = System.Data.ParameterDirection.Input;
        cmd.Parameters.Add(ptrParam);
        System.Data.SqlClient.SqlParameter offsetParam = new System.Data.SqlClient.SqlParameter();
        offsetParam.ParameterName = "@Offset";
        offsetParam.SqlDbType = System.Data.SqlDbType.Int;
        offsetParam.Size = 4;
        offsetParam.Value = _LengthOut;
        offsetParam.Direction = System.Data.ParameterDirection.Input;
        cmd.Parameters.Add(offsetParam);
        System.Data.SqlClient.SqlParameter chunkParam = new System.Data.SqlClient.SqlParameter();
        chunkParam.ParameterName = "@Chunk";
        chunkParam.SqlDbType = System.Data.SqlDbType.Binary;
        chunkParam.Size = count;
        chunkParam.Value = buffer;
        chunkParam.Direction = System.Data.ParameterDirection.Input;
        cmd.Parameters.Add(chunkParam);
        cmd.ExecuteNonQuery();
      }
      this.UpdateTextPointer();
    }
    
    public override int Read(byte[] buffer, int offset, int count) {
      if ((_ReadPosition >= ((int)(_LengthOut)))) {
        return 0;
      }
      int bytesToRead;
      if (((_ReadPosition + count) 
            > ((int)(_LengthOut)))) {
        return 0;
      }
      else {
        bytesToRead = count;
      }
      System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand("readtext TypeTest.VarBinaryTest @Ptr @Offset @Size HOLDLOCK", _Connection, _Transaction);
      System.Data.SqlClient.SqlParameter ptrParam = new System.Data.SqlClient.SqlParameter();
      ptrParam.ParameterName = "@Ptr";
      ptrParam.SqlDbType = System.Data.SqlDbType.VarBinary;
      ptrParam.Size = 16;
      ptrParam.Value = _TextPtrOut;
      ptrParam.Direction = System.Data.ParameterDirection.Input;
      cmd.Parameters.Add(ptrParam);
      System.Data.SqlClient.SqlParameter offsetParam = new System.Data.SqlClient.SqlParameter();
      offsetParam.ParameterName = "@Offset";
      offsetParam.SqlDbType = System.Data.SqlDbType.Int;
      offsetParam.Size = 4;
      offsetParam.Value = _ReadPosition;
      offsetParam.Direction = System.Data.ParameterDirection.Input;
      cmd.Parameters.Add(offsetParam);
      System.Data.SqlClient.SqlParameter sizeParam = new System.Data.SqlClient.SqlParameter();
      sizeParam.ParameterName = "@Size";
      sizeParam.SqlDbType = System.Data.SqlDbType.Int;
      sizeParam.Size = 4;
      sizeParam.Value = bytesToRead;
      sizeParam.Direction = System.Data.ParameterDirection.Input;
      cmd.Parameters.Add(sizeParam);
      System.Data.SqlClient.SqlDataReader rd = cmd.ExecuteReader(System.Data.CommandBehavior.SingleResult);
      rd.Read();
      rd.GetBytes(0, 0, buffer, offset, bytesToRead);
      rd.Close();
      _ReadPosition = (_ReadPosition + bytesToRead);
      return bytesToRead;
    }
    
    public override void SetLength(long value) {
      throw new System.NotImplementedException();
    }
    
    public override long Seek(long offset, System.IO.SeekOrigin origin) {
      throw new System.NotImplementedException();
    }
    
    public override void Flush() {
    }
  }
  
  public class TypeTestXmlTestBlobStream : System.IO.Stream {
    
    private int _ReadPosition;
    
    private System.Data.SqlClient.SqlConnection _Connection;
    
    private System.Data.SqlClient.SqlTransaction _Transaction;
    
    private System.Nullable<int> _TypeTestID;
    
    private object _TextPtrOut;
    
    private object _LengthOut;
    
    public TypeTestXmlTestBlobStream(System.Data.SqlClient.SqlConnection Connection, System.Data.SqlClient.SqlTransaction Transaction, System.Nullable<int> TypeTestID) {
      _Connection = Connection;
      _Transaction = Transaction;
      _TypeTestID = TypeTestID;
      this.UpdateTextPointer();
    }
    
    public override bool CanRead {
      get {
        return true;
      }
    }
    
    public override bool CanWrite {
      get {
        return true;
      }
    }
    
    public override bool CanSeek {
      get {
        return false;
      }
    }
    
    public override long Position {
      get {
        return System.Convert.ToInt64(_LengthOut);
      }
      set {
        throw new System.NotImplementedException();
      }
    }
    
    public override long Length {
      get {
        return ((long)(_LengthOut));
      }
    }
    
    private void UpdateTextPointer() {
      System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand("select   @Ptr=TEXTPTR(XmlTest),   @Length=DATALENGTH(XmlTest) from TypeTest where" +
          " TypeTestID = @TypeTestID", _Connection, _Transaction);
      System.Data.SqlClient.SqlParameter ptrParam = new System.Data.SqlClient.SqlParameter();
      ptrParam.ParameterName = "@Ptr";
      ptrParam.SqlDbType = System.Data.SqlDbType.VarBinary;
      ptrParam.Size = 100;
      ptrParam.Direction = System.Data.ParameterDirection.Output;
      cmd.Parameters.Add(ptrParam);
      System.Data.SqlClient.SqlParameter lengthParam = new System.Data.SqlClient.SqlParameter();
      lengthParam.ParameterName = "@Length";
      lengthParam.SqlDbType = System.Data.SqlDbType.Int;
      lengthParam.Size = 4;
      lengthParam.Direction = System.Data.ParameterDirection.Output;
      cmd.Parameters.Add(lengthParam);
      System.Data.SqlClient.SqlParameter pkParm1 = new System.Data.SqlClient.SqlParameter();
      pkParm1.ParameterName = "@TypeTestID";
      pkParm1.SqlDbType = System.Data.SqlDbType.Int;
      pkParm1.Size = 4;
      pkParm1.Value = _TypeTestID;
      cmd.Parameters.Add(pkParm1);
      cmd.ExecuteNonQuery();
      _TextPtrOut = ptrParam.Value;
      _LengthOut = lengthParam.Value;
    }
    
    public override void Write(byte[] buffer, int offset, int count) {
      if ((_TextPtrOut == System.DBNull.Value)) {
        System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand("update TypeTest set XmlTest = @Chunk where TypeTestID = @TypeTestID", _Connection, _Transaction);
        System.Data.SqlClient.SqlParameter chunkParam = new System.Data.SqlClient.SqlParameter();
        chunkParam.ParameterName = "@Chunk";
        chunkParam.SqlDbType = System.Data.SqlDbType.Binary;
        chunkParam.Size = count;
        chunkParam.Value = buffer;
        chunkParam.Direction = System.Data.ParameterDirection.Input;
        cmd.Parameters.Add(chunkParam);
        System.Data.SqlClient.SqlParameter pkParm1 = new System.Data.SqlClient.SqlParameter();
        pkParm1.ParameterName = "@TypeTestID";
        pkParm1.SqlDbType = System.Data.SqlDbType.Int;
        pkParm1.Size = 4;
        pkParm1.Value = _TypeTestID;
        cmd.Parameters.Add(pkParm1);
        cmd.ExecuteNonQuery();
      }
      else {
        System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand("updatetext TypeTest.XmlTest @Ptr @Offset 0 WITH LOG @Chunk", _Connection, _Transaction);
        System.Data.SqlClient.SqlParameter ptrParam = new System.Data.SqlClient.SqlParameter();
        ptrParam.ParameterName = "@Ptr";
        ptrParam.SqlDbType = System.Data.SqlDbType.VarBinary;
        ptrParam.Size = 16;
        ptrParam.Value = _TextPtrOut;
        ptrParam.Direction = System.Data.ParameterDirection.Input;
        cmd.Parameters.Add(ptrParam);
        System.Data.SqlClient.SqlParameter offsetParam = new System.Data.SqlClient.SqlParameter();
        offsetParam.ParameterName = "@Offset";
        offsetParam.SqlDbType = System.Data.SqlDbType.Int;
        offsetParam.Size = 4;
        offsetParam.Value = _LengthOut;
        offsetParam.Direction = System.Data.ParameterDirection.Input;
        cmd.Parameters.Add(offsetParam);
        System.Data.SqlClient.SqlParameter chunkParam = new System.Data.SqlClient.SqlParameter();
        chunkParam.ParameterName = "@Chunk";
        chunkParam.SqlDbType = System.Data.SqlDbType.Binary;
        chunkParam.Size = count;
        chunkParam.Value = buffer;
        chunkParam.Direction = System.Data.ParameterDirection.Input;
        cmd.Parameters.Add(chunkParam);
        cmd.ExecuteNonQuery();
      }
      this.UpdateTextPointer();
    }
    
    public override int Read(byte[] buffer, int offset, int count) {
      if ((_ReadPosition >= ((int)(_LengthOut)))) {
        return 0;
      }
      int bytesToRead;
      if (((_ReadPosition + count) 
            > ((int)(_LengthOut)))) {
        return 0;
      }
      else {
        bytesToRead = count;
      }
      System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand("readtext TypeTest.XmlTest @Ptr @Offset @Size HOLDLOCK", _Connection, _Transaction);
      System.Data.SqlClient.SqlParameter ptrParam = new System.Data.SqlClient.SqlParameter();
      ptrParam.ParameterName = "@Ptr";
      ptrParam.SqlDbType = System.Data.SqlDbType.VarBinary;
      ptrParam.Size = 16;
      ptrParam.Value = _TextPtrOut;
      ptrParam.Direction = System.Data.ParameterDirection.Input;
      cmd.Parameters.Add(ptrParam);
      System.Data.SqlClient.SqlParameter offsetParam = new System.Data.SqlClient.SqlParameter();
      offsetParam.ParameterName = "@Offset";
      offsetParam.SqlDbType = System.Data.SqlDbType.Int;
      offsetParam.Size = 4;
      offsetParam.Value = _ReadPosition;
      offsetParam.Direction = System.Data.ParameterDirection.Input;
      cmd.Parameters.Add(offsetParam);
      System.Data.SqlClient.SqlParameter sizeParam = new System.Data.SqlClient.SqlParameter();
      sizeParam.ParameterName = "@Size";
      sizeParam.SqlDbType = System.Data.SqlDbType.Int;
      sizeParam.Size = 4;
      sizeParam.Value = bytesToRead;
      sizeParam.Direction = System.Data.ParameterDirection.Input;
      cmd.Parameters.Add(sizeParam);
      System.Data.SqlClient.SqlDataReader rd = cmd.ExecuteReader(System.Data.CommandBehavior.SingleResult);
      rd.Read();
      rd.GetBytes(0, 0, buffer, offset, bytesToRead);
      rd.Close();
      _ReadPosition = (_ReadPosition + bytesToRead);
      return bytesToRead;
    }
    
    public override void SetLength(long value) {
      throw new System.NotImplementedException();
    }
    
    public override long Seek(long offset, System.IO.SeekOrigin origin) {
      throw new System.NotImplementedException();
    }
    
    public override void Flush() {
    }
  }
  
  public class TypeTestTimeStampTestBlobStream : System.IO.Stream {
    
    private int _ReadPosition;
    
    private System.Data.SqlClient.SqlConnection _Connection;
    
    private System.Data.SqlClient.SqlTransaction _Transaction;
    
    private System.Nullable<int> _TypeTestID;
    
    private object _TextPtrOut;
    
    private object _LengthOut;
    
    public TypeTestTimeStampTestBlobStream(System.Data.SqlClient.SqlConnection Connection, System.Data.SqlClient.SqlTransaction Transaction, System.Nullable<int> TypeTestID) {
      _Connection = Connection;
      _Transaction = Transaction;
      _TypeTestID = TypeTestID;
      this.UpdateTextPointer();
    }
    
    public override bool CanRead {
      get {
        return true;
      }
    }
    
    public override bool CanWrite {
      get {
        return true;
      }
    }
    
    public override bool CanSeek {
      get {
        return false;
      }
    }
    
    public override long Position {
      get {
        return System.Convert.ToInt64(_LengthOut);
      }
      set {
        throw new System.NotImplementedException();
      }
    }
    
    public override long Length {
      get {
        return ((long)(_LengthOut));
      }
    }
    
    private void UpdateTextPointer() {
      System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand("select   @Ptr=TEXTPTR(TimeStampTest),   @Length=DATALENGTH(TimeStampTest) from Ty" +
          "peTest where TypeTestID = @TypeTestID", _Connection, _Transaction);
      System.Data.SqlClient.SqlParameter ptrParam = new System.Data.SqlClient.SqlParameter();
      ptrParam.ParameterName = "@Ptr";
      ptrParam.SqlDbType = System.Data.SqlDbType.VarBinary;
      ptrParam.Size = 100;
      ptrParam.Direction = System.Data.ParameterDirection.Output;
      cmd.Parameters.Add(ptrParam);
      System.Data.SqlClient.SqlParameter lengthParam = new System.Data.SqlClient.SqlParameter();
      lengthParam.ParameterName = "@Length";
      lengthParam.SqlDbType = System.Data.SqlDbType.Int;
      lengthParam.Size = 4;
      lengthParam.Direction = System.Data.ParameterDirection.Output;
      cmd.Parameters.Add(lengthParam);
      System.Data.SqlClient.SqlParameter pkParm1 = new System.Data.SqlClient.SqlParameter();
      pkParm1.ParameterName = "@TypeTestID";
      pkParm1.SqlDbType = System.Data.SqlDbType.Int;
      pkParm1.Size = 4;
      pkParm1.Value = _TypeTestID;
      cmd.Parameters.Add(pkParm1);
      cmd.ExecuteNonQuery();
      _TextPtrOut = ptrParam.Value;
      _LengthOut = lengthParam.Value;
    }
    
    public override void Write(byte[] buffer, int offset, int count) {
      if ((_TextPtrOut == System.DBNull.Value)) {
        System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand("update TypeTest set TimeStampTest = @Chunk where TypeTestID = @TypeTestID", _Connection, _Transaction);
        System.Data.SqlClient.SqlParameter chunkParam = new System.Data.SqlClient.SqlParameter();
        chunkParam.ParameterName = "@Chunk";
        chunkParam.SqlDbType = System.Data.SqlDbType.Binary;
        chunkParam.Size = count;
        chunkParam.Value = buffer;
        chunkParam.Direction = System.Data.ParameterDirection.Input;
        cmd.Parameters.Add(chunkParam);
        System.Data.SqlClient.SqlParameter pkParm1 = new System.Data.SqlClient.SqlParameter();
        pkParm1.ParameterName = "@TypeTestID";
        pkParm1.SqlDbType = System.Data.SqlDbType.Int;
        pkParm1.Size = 4;
        pkParm1.Value = _TypeTestID;
        cmd.Parameters.Add(pkParm1);
        cmd.ExecuteNonQuery();
      }
      else {
        System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand("updatetext TypeTest.TimeStampTest @Ptr @Offset 0 WITH LOG @Chunk", _Connection, _Transaction);
        System.Data.SqlClient.SqlParameter ptrParam = new System.Data.SqlClient.SqlParameter();
        ptrParam.ParameterName = "@Ptr";
        ptrParam.SqlDbType = System.Data.SqlDbType.VarBinary;
        ptrParam.Size = 16;
        ptrParam.Value = _TextPtrOut;
        ptrParam.Direction = System.Data.ParameterDirection.Input;
        cmd.Parameters.Add(ptrParam);
        System.Data.SqlClient.SqlParameter offsetParam = new System.Data.SqlClient.SqlParameter();
        offsetParam.ParameterName = "@Offset";
        offsetParam.SqlDbType = System.Data.SqlDbType.Int;
        offsetParam.Size = 4;
        offsetParam.Value = _LengthOut;
        offsetParam.Direction = System.Data.ParameterDirection.Input;
        cmd.Parameters.Add(offsetParam);
        System.Data.SqlClient.SqlParameter chunkParam = new System.Data.SqlClient.SqlParameter();
        chunkParam.ParameterName = "@Chunk";
        chunkParam.SqlDbType = System.Data.SqlDbType.Binary;
        chunkParam.Size = count;
        chunkParam.Value = buffer;
        chunkParam.Direction = System.Data.ParameterDirection.Input;
        cmd.Parameters.Add(chunkParam);
        cmd.ExecuteNonQuery();
      }
      this.UpdateTextPointer();
    }
    
    public override int Read(byte[] buffer, int offset, int count) {
      if ((_ReadPosition >= ((int)(_LengthOut)))) {
        return 0;
      }
      int bytesToRead;
      if (((_ReadPosition + count) 
            > ((int)(_LengthOut)))) {
        return 0;
      }
      else {
        bytesToRead = count;
      }
      System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand("readtext TypeTest.TimeStampTest @Ptr @Offset @Size HOLDLOCK", _Connection, _Transaction);
      System.Data.SqlClient.SqlParameter ptrParam = new System.Data.SqlClient.SqlParameter();
      ptrParam.ParameterName = "@Ptr";
      ptrParam.SqlDbType = System.Data.SqlDbType.VarBinary;
      ptrParam.Size = 16;
      ptrParam.Value = _TextPtrOut;
      ptrParam.Direction = System.Data.ParameterDirection.Input;
      cmd.Parameters.Add(ptrParam);
      System.Data.SqlClient.SqlParameter offsetParam = new System.Data.SqlClient.SqlParameter();
      offsetParam.ParameterName = "@Offset";
      offsetParam.SqlDbType = System.Data.SqlDbType.Int;
      offsetParam.Size = 4;
      offsetParam.Value = _ReadPosition;
      offsetParam.Direction = System.Data.ParameterDirection.Input;
      cmd.Parameters.Add(offsetParam);
      System.Data.SqlClient.SqlParameter sizeParam = new System.Data.SqlClient.SqlParameter();
      sizeParam.ParameterName = "@Size";
      sizeParam.SqlDbType = System.Data.SqlDbType.Int;
      sizeParam.Size = 4;
      sizeParam.Value = bytesToRead;
      sizeParam.Direction = System.Data.ParameterDirection.Input;
      cmd.Parameters.Add(sizeParam);
      System.Data.SqlClient.SqlDataReader rd = cmd.ExecuteReader(System.Data.CommandBehavior.SingleResult);
      rd.Read();
      rd.GetBytes(0, 0, buffer, offset, bytesToRead);
      rd.Close();
      _ReadPosition = (_ReadPosition + bytesToRead);
      return bytesToRead;
    }
    
    public override void SetLength(long value) {
      throw new System.NotImplementedException();
    }
    
    public override long Seek(long offset, System.IO.SeekOrigin origin) {
      throw new System.NotImplementedException();
    }
    
    public override void Flush() {
    }
  }
}
