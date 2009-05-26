﻿//------------------------------------------------------------------------------
// <auto-generated>
//     O código foi gerado por uma ferramenta.
//     Versão de Tempo de Execução:2.0.50727.3082
//
//     As alterações ao arquivo poderão causar comportamento incorreto e serão perdidas se
//     o código for gerado novamente.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by xsd, Version=2.0.50727.3038.
// 
namespace DBInfo.XMLDatabase {
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://dbinfo.sourceforge.net/Schemas/DBInfo.xsd")]
    [System.Xml.Serialization.XmlRootAttribute("DBInfo", Namespace="http://dbinfo.sourceforge.net/Schemas/DBInfo.xsd", IsNullable=false)]
    public partial class StatementCollection {
        
        private Statement[] statementField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Statement")]
        public Statement[] Statement {
            get {
                return this.statementField;
            }
            set {
                this.statementField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(CreateView))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(CreateSequence))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(CreateFunction))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(CreatePrimaryKey))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(CreateProcedure))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(CreateTrigger))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(CreateCheckConstraint))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(CreateIndex))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(CreateForeignKey))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(CreateTable))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://dbinfo.sourceforge.net/Schemas/DBInfo.xsd")]
    public partial class Statement {
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://dbinfo.sourceforge.net/Schemas/DBInfo.xsd")]
    public partial class IndexColumn {
        
        private string nameField;
        
        private SortOrder orderField;
        
        /// <remarks/>
        public string Name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
        
        /// <remarks/>
        public SortOrder Order {
            get {
                return this.orderField;
            }
            set {
                this.orderField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://dbinfo.sourceforge.net/Schemas/DBInfo.xsd")]
    public enum SortOrder {
        
        /// <remarks/>
        Ascending,
        
        /// <remarks/>
        Descending,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://dbinfo.sourceforge.net/Schemas/DBInfo.xsd")]
    public partial class ForeignKeyColumn {
        
        private string columnNameField;
        
        private string refColumnNameField;
        
        /// <remarks/>
        public string ColumnName {
            get {
                return this.columnNameField;
            }
            set {
                this.columnNameField = value;
            }
        }
        
        /// <remarks/>
        public string RefColumnName {
            get {
                return this.refColumnNameField;
            }
            set {
                this.refColumnNameField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://dbinfo.sourceforge.net/Schemas/DBInfo.xsd")]
    public partial class Column {
        
        private string nameField;
        
        private ColumnType typeField;
        
        private string sizeField;
        
        private string precisionField;
        
        private string scaleField;
        
        private YesNo nullableField;
        
        private YesNo identityColumnField;
        
        private string defaultValueField;
        
        private string constraintDefaultNameField;
        
        /// <remarks/>
        public string Name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
        
        /// <remarks/>
        public ColumnType Type {
            get {
                return this.typeField;
            }
            set {
                this.typeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType="integer")]
        public string Size {
            get {
                return this.sizeField;
            }
            set {
                this.sizeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType="integer")]
        public string Precision {
            get {
                return this.precisionField;
            }
            set {
                this.precisionField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType="integer")]
        public string Scale {
            get {
                return this.scaleField;
            }
            set {
                this.scaleField = value;
            }
        }
        
        /// <remarks/>
        public YesNo Nullable {
            get {
                return this.nullableField;
            }
            set {
                this.nullableField = value;
            }
        }
        
        /// <remarks/>
        public YesNo IdentityColumn {
            get {
                return this.identityColumnField;
            }
            set {
                this.identityColumnField = value;
            }
        }
        
        /// <remarks/>
        public string DefaultValue {
            get {
                return this.defaultValueField;
            }
            set {
                this.defaultValueField = value;
            }
        }
        
        /// <remarks/>
        public string ConstraintDefaultName {
            get {
                return this.constraintDefaultNameField;
            }
            set {
                this.constraintDefaultNameField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://dbinfo.sourceforge.net/Schemas/DBInfo.xsd")]
    public enum ColumnType {
        
        /// <remarks/>
        Integer,
        
        /// <remarks/>
        Varchar,
        
        /// <remarks/>
        Char,
        
        /// <remarks/>
        Decimal,
        
        /// <remarks/>
        Float,
        
        /// <remarks/>
        Memo,
        
        /// <remarks/>
        Blob,
        
        /// <remarks/>
        DateTime,
        
        /// <remarks/>
        Bit,
        
        /// <remarks/>
        SmallDateTime,
        
        /// <remarks/>
        Money,
        
        /// <remarks/>
        SmallInt,
        
        /// <remarks/>
        Numeric,
        
        /// <remarks/>
        GUID,
        
        /// <remarks/>
        BigInt,
        
        /// <remarks/>
        TinyInt,
        
        /// <remarks/>
        Binary,
        
        /// <remarks/>
        NVarchar,
        
        /// <remarks/>
        RowID,
        
        /// <remarks/>
        TimeStamp,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://dbinfo.sourceforge.net/Schemas/DBInfo.xsd")]
    public enum YesNo {
        
        /// <remarks/>
        Yes,
        
        /// <remarks/>
        No,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://dbinfo.sourceforge.net/Schemas/DBInfo.xsd")]
    public partial class CreateView : Statement {
        
        private string nameField;
        
        private string sourceCodeField;
        
        /// <remarks/>
        public string Name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
        
        /// <remarks/>
        public string SourceCode {
            get {
                return this.sourceCodeField;
            }
            set {
                this.sourceCodeField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://dbinfo.sourceforge.net/Schemas/DBInfo.xsd")]
    public partial class CreateSequence : Statement {
        
        private string nameField;
        
        private string initialField;
        
        private string minValueField;
        
        private string maxValueField;
        
        private string incrementField;
        
        private YesNo cycleOnLimitField;
        
        /// <remarks/>
        public string Name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType="integer")]
        public string Initial {
            get {
                return this.initialField;
            }
            set {
                this.initialField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType="integer")]
        public string MinValue {
            get {
                return this.minValueField;
            }
            set {
                this.minValueField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType="integer")]
        public string MaxValue {
            get {
                return this.maxValueField;
            }
            set {
                this.maxValueField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType="integer")]
        public string Increment {
            get {
                return this.incrementField;
            }
            set {
                this.incrementField = value;
            }
        }
        
        /// <remarks/>
        public YesNo CycleOnLimit {
            get {
                return this.cycleOnLimitField;
            }
            set {
                this.cycleOnLimitField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://dbinfo.sourceforge.net/Schemas/DBInfo.xsd")]
    public partial class CreateFunction : Statement {
        
        private string nameField;
        
        private string sourceCodeField;
        
        /// <remarks/>
        public string Name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
        
        /// <remarks/>
        public string SourceCode {
            get {
                return this.sourceCodeField;
            }
            set {
                this.sourceCodeField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://dbinfo.sourceforge.net/Schemas/DBInfo.xsd")]
    public partial class CreatePrimaryKey : Statement {
        
        private string tableNameField;
        
        private string primaryKeyNameField;
        
        private string[] columnsField;
        
        /// <remarks/>
        public string TableName {
            get {
                return this.tableNameField;
            }
            set {
                this.tableNameField = value;
            }
        }
        
        /// <remarks/>
        public string PrimaryKeyName {
            get {
                return this.primaryKeyNameField;
            }
            set {
                this.primaryKeyNameField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Value", IsNullable=false)]
        public string[] Columns {
            get {
                return this.columnsField;
            }
            set {
                this.columnsField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://dbinfo.sourceforge.net/Schemas/DBInfo.xsd")]
    public partial class CreateProcedure : Statement {
        
        private string nameField;
        
        private string sourceCodeField;
        
        /// <remarks/>
        public string Name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
        
        /// <remarks/>
        public string SourceCode {
            get {
                return this.sourceCodeField;
            }
            set {
                this.sourceCodeField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://dbinfo.sourceforge.net/Schemas/DBInfo.xsd")]
    public partial class CreateTrigger : Statement {
        
        private string tableNameField;
        
        private string triggerNameField;
        
        private string sourceCodeField;
        
        /// <remarks/>
        public string TableName {
            get {
                return this.tableNameField;
            }
            set {
                this.tableNameField = value;
            }
        }
        
        /// <remarks/>
        public string TriggerName {
            get {
                return this.triggerNameField;
            }
            set {
                this.triggerNameField = value;
            }
        }
        
        /// <remarks/>
        public string SourceCode {
            get {
                return this.sourceCodeField;
            }
            set {
                this.sourceCodeField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://dbinfo.sourceforge.net/Schemas/DBInfo.xsd")]
    public partial class CreateCheckConstraint : Statement {
        
        private string tableNameField;
        
        private string checkConstraintNameField;
        
        private string sourceCodeField;
        
        /// <remarks/>
        public string TableName {
            get {
                return this.tableNameField;
            }
            set {
                this.tableNameField = value;
            }
        }
        
        /// <remarks/>
        public string CheckConstraintName {
            get {
                return this.checkConstraintNameField;
            }
            set {
                this.checkConstraintNameField = value;
            }
        }
        
        /// <remarks/>
        public string SourceCode {
            get {
                return this.sourceCodeField;
            }
            set {
                this.sourceCodeField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://dbinfo.sourceforge.net/Schemas/DBInfo.xsd")]
    public partial class CreateIndex : Statement {
        
        private string tableNameField;
        
        private string indexNameField;
        
        private YesNo uniqueField;
        
        private YesNo clusteredField;
        
        private IndexColumn[] columnsField;
        
        /// <remarks/>
        public string TableName {
            get {
                return this.tableNameField;
            }
            set {
                this.tableNameField = value;
            }
        }
        
        /// <remarks/>
        public string IndexName {
            get {
                return this.indexNameField;
            }
            set {
                this.indexNameField = value;
            }
        }
        
        /// <remarks/>
        public YesNo Unique {
            get {
                return this.uniqueField;
            }
            set {
                this.uniqueField = value;
            }
        }
        
        /// <remarks/>
        public YesNo Clustered {
            get {
                return this.clusteredField;
            }
            set {
                this.clusteredField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)]
        public IndexColumn[] Columns {
            get {
                return this.columnsField;
            }
            set {
                this.columnsField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://dbinfo.sourceforge.net/Schemas/DBInfo.xsd")]
    public partial class CreateForeignKey : Statement {
        
        private string tableNameField;
        
        private string foreignKeyNameField;
        
        private string refTableNameField;
        
        private ForeignKeyColumn[] columnsField;
        
        private YesNo deleteCascadeField;
        
        private YesNo updateCascadeField;
        
        /// <remarks/>
        public string TableName {
            get {
                return this.tableNameField;
            }
            set {
                this.tableNameField = value;
            }
        }
        
        /// <remarks/>
        public string ForeignKeyName {
            get {
                return this.foreignKeyNameField;
            }
            set {
                this.foreignKeyNameField = value;
            }
        }
        
        /// <remarks/>
        public string RefTableName {
            get {
                return this.refTableNameField;
            }
            set {
                this.refTableNameField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)]
        public ForeignKeyColumn[] Columns {
            get {
                return this.columnsField;
            }
            set {
                this.columnsField = value;
            }
        }
        
        /// <remarks/>
        public YesNo DeleteCascade {
            get {
                return this.deleteCascadeField;
            }
            set {
                this.deleteCascadeField = value;
            }
        }
        
        /// <remarks/>
        public YesNo UpdateCascade {
            get {
                return this.updateCascadeField;
            }
            set {
                this.updateCascadeField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://dbinfo.sourceforge.net/Schemas/DBInfo.xsd")]
    public partial class CreateTable : Statement {
        
        private string tableNameField;
        
        private Column[] columnsField;
        
        private YesNo hasIdentityField;
        
        private string identitySeedField;
        
        private string identityIncrementField;
        
        /// <remarks/>
        public string TableName {
            get {
                return this.tableNameField;
            }
            set {
                this.tableNameField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)]
        public Column[] Columns {
            get {
                return this.columnsField;
            }
            set {
                this.columnsField = value;
            }
        }
        
        /// <remarks/>
        public YesNo HasIdentity {
            get {
                return this.hasIdentityField;
            }
            set {
                this.hasIdentityField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType="integer")]
        public string IdentitySeed {
            get {
                return this.identitySeedField;
            }
            set {
                this.identitySeedField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType="integer")]
        public string IdentityIncrement {
            get {
                return this.identityIncrementField;
            }
            set {
                this.identityIncrementField = value;
            }
        }
    }
}
