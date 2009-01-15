using System;
using System.Collections;

namespace DBInfo.Core.Model
{
	public class Table
	{
    public string TableName;

    [System.Xml.Serialization.XmlArray("Columns"), System.Xml.Serialization.XmlArrayItem(typeof(Column))]
    public ArrayList Columns;

		[System.Xml.Serialization.XmlArray("ForeignKeys"), System.Xml.Serialization.XmlArrayItem(typeof(ForeignKey))]
    public ArrayList ForeignKeys;

    public string PrimaryKeyName;

    [System.Xml.Serialization.XmlArray("PrimaryKeyColumns"), System.Xml.Serialization.XmlArrayItem(typeof(Column))]
    public ArrayList PrimaryKeyColumns;
			
    [System.Xml.Serialization.XmlArray("Indexes"), System.Xml.Serialization.XmlArrayItem(typeof(Index))]
    public ArrayList Indexes;
		
    public bool HasIdentity;
		public int IdentitySeed;
		public int IdentityIncrement;

    [System.Xml.Serialization.XmlArray("CheckConstraints"), System.Xml.Serialization.XmlArrayItem(typeof(CheckConstraint))]
    public ArrayList CheckConstraints;

		public string Area; //Progress
		public string Description; //Progress
		public string DumpName; //Progress
		public string Label; //Progress
		public string ValExp; //Progress
		public string ValMsg; //Progress
		public string ForeignName; //Progress

		//Progress
		public ArrayList TableTriggers;
		
		public Table(){
		}

		public Table(string ATableName)
		{
      TableName = ATableName;	  
      Columns = new ArrayList();
      ForeignKeys = new ArrayList();
      PrimaryKeyColumns = new ArrayList();
      Indexes = new ArrayList();
      PrimaryKeyName = String.Empty;
      IdentitySeed = 0;
      IdentityIncrement = 0;
      CheckConstraints = new ArrayList();
			TableTriggers = new ArrayList();
		}

    public Column FindColumn(string AColumnName)
    {
      Column TmpColumn = null;
      foreach (Column c in Columns)
      {
        if (c.Name == AColumnName)
        {
          TmpColumn = c;
          break;
        }
      }
      return TmpColumn;
    }

    public ForeignKeyColumn ColumnHasForeignKey(Column AColumn)
    {
      foreach (ForeignKey fk in ForeignKeys)
      {
        foreach (ForeignKeyColumn fkcol in fk.Columns)
        {
          if (fkcol.Column == AColumn)
          {
            return fkcol;
          }
        }
      }
      return null;
    }

	}
}
