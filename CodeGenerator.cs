using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace SpParamClassGenerater
{
    public class CodeGenerator
    {
        StringBuilder sbuilder;
        static Dictionary<string, string> Errors;
        public CodeGenerator()
        {
            sbuilder = new StringBuilder();
            Errors = new Dictionary<string, string>();
        }
        public string GenerateConsts(List<SPlist> splist)
        {



            sbuilder.Append(" using System; \n");
            sbuilder.Append("namespace SpHelper.Consts \n");
            sbuilder.Append(" \n \t {");
            sbuilder.Append("\t \t public class SP \n");
            sbuilder.Append("\t \t \t {");
            foreach (var item in splist)
            {
                string ContName = item.ClassName;
                if (string.IsNullOrEmpty(item.ClassName))
                {
                    ContName = item.Spname;
                    ContName = ContName.Replace("[", "");
                    ContName = ContName.Replace("]", "");
                    ContName = ContName.Replace(".", "");
                }
                sbuilder.Append($"\n public const string {ContName} = \"{item.Spname}\"; \n");
            }

            sbuilder.Append("\n \t \t \t }");
            sbuilder.Append("\n \t } ");

            return sbuilder.ToString();
        }
        public SpParamResultDetail GenerateModel(List<SPlist> splist, string conStr, string NameSpace)
        {
            string JsonN = string.Empty;
            string queryText = string.Empty;
            string resultCls = string.Empty;
            string nspce = string.Empty;
            string cls = string.Empty;
            string jsn = string.Empty;
            string param = string.Empty;
            string result = string.Empty;
            nspce = "using System; \n";
            nspce += $"\n namespace {NameSpace}";
            cls += "\n  {";

            foreach (var sp in splist)
            {

                string clsmName = sp.ClassName;
                if (string.IsNullOrEmpty(clsmName))
                {
                    clsmName = sp.Spname;
                    clsmName = clsmName.Replace("[", "");
                    clsmName = clsmName.Replace("]", "");
                    //clsmName = clsmName.Replace(".", "");
                    if (clsmName.Contains('.'))
                        clsmName = clsmName.Split('.')[1];
                }

                var Params = GetSPDetails(sp.Spname, conStr)?.Parameters;
                cls += $"\n \t//{sp.Spname}";
                cls += $"\n \tpublic partial class {clsmName}Param";
                cls += "\n \t {";
                foreach (var item in Params)
                {
                    cls += $"\n \t\t public {item.NetType} {item.Parameter_name} " + "{ get; set; }";
                    jsn += $"\n  '{item.Parameter_name}':null";
                }
                cls += "\n \t }";
                JsonN += $"//{sp.Spname}" + " \n {" + jsn + " \n }";
                queryText = CreateSqlQuery(conStr, sp.Spname);
                resultCls += "\n " + GenerateResultClass(conStr, queryText, clsmName, sp.Spname);

            }
            param = nspce += cls + "\n  }";
            result = $"using System; \n  namespace {NameSpace} \n " + "\n  {" + resultCls + "\n  }";
            return new SpParamResultDetail { Param = param, Json = JsonN, Result = result };
        }
        public SpParamResultDetail GenerateModel(string spName, string clasName, string conStr, string NameSpace)
        {
            try
            {

                if (string.IsNullOrEmpty(spName) || string.IsNullOrEmpty(clasName))
                {
                    throw new Exception("Please Provide Name of SP name and Class");
                }
                else
                {
                    var Params = GetSPDetails(spName, conStr)?.Parameters;

                    string cls = string.Empty;
                    string jsn = string.Empty;
                    cls = "using System; \n";
                    cls += $"namespace {NameSpace}";
                    cls += "\n  {";
                    cls += $"\n \t // {spName}";
                    cls += $"\n \tpublic partial class {clasName}Param";
                    cls += "\n \t {";
                    foreach (var item in Params)
                    {
                        cls += $"\n \t\t public {item.NetType} {item.Parameter_name} " + "{ get; set; }";
                        jsn += $"\n  '{item.Parameter_name}':null";
                    }
                    cls += "\n \t }";
                    cls += "\n  }";
                    string JsonN = "\n {" + jsn + " \n }";
                    string queryText = CreateSqlQuery(conStr, spName);
                    string resultCls = $" using System; \n namespace {NameSpace} \n" + GenerateResultClass(conStr, queryText, clasName, spName) + "\n }";
                    return new SpParamResultDetail { Param = cls, Json = JsonN, Result = resultCls };

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }
        public static SpDetail GetSPDetails(string spname, string conStr)
        {
            // string[] SPs = spname.Split(',');
            // List<SpDetail> SpList = new List<SpDetail>();
            //foreach (var SP in SPs)
            DataSet dsReoports = new DataSet();
            //{
            if (!string.IsNullOrEmpty(conStr))
            {
                SqlConnection con = new SqlConnection(conStr);
                SqlCommand cmd = new SqlCommand($"sp_help '{spname.Trim()}'", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dsReoports);
            }
            else
            {
                var _Database = DatabaseFactory.CreateDatabase("DefaultConnection");
                var dbCommand = _Database.GetSqlStringCommand($"sp_help '{spname.Trim()}'");
                dsReoports = _Database.ExecuteDataSet(dbCommand);
            }
            SpDetail sp = new SpDetail();
            sp.Name = dsReoports.Tables[0].Rows[0]["Name"].ToString();
            for (int i = 0; i < dsReoports.Tables[1].Rows.Count; i++)
            {
                if (sp.Parameters == null)
                    sp.Parameters = new List<ParamDetail>();
                sp.Parameters.Add(
                    new ParamDetail
                    {
                        Parameter_name = dsReoports.Tables[1].Rows[i]["Parameter_name"].ToString().Replace("@", " ").Trim(),
                        Param_order = int.Parse(dsReoports.Tables[1].Rows[i]["Param_order"].ToString()),
                        DBType = dsReoports.Tables[1].Rows[i]["Type"].ToString(),
                        NetType = GetNetDataType(dsReoports.Tables[1].Rows[i]["Type"].ToString())
                    }
                );
            }
            //SpList.Add(sp);
            //}

            //return SpList;
            return sp;
        }
        public static string GetNetDataType(string sqlDataTypeName)
        {
            switch (sqlDataTypeName.ToLower())
            {
                case "bigint":
                    return "Int64";
                case "binary":
                case "image":
                case "varbinary":
                    return "byte[]";
                case "bit":
                    return "bool";
                case "char":
                    return "char";
                case "datetime":
                case "smalldatetime":
                    return "DateTime";
                case "decimal":
                case "money":
                case "numeric":
                    return "decimal";
                case "float":
                    return "double";
                case "int":
                    return "int";
                case "nchar":
                case "nvarchar":
                case "text":
                case "varchar":
                case "xml":
                    return "string";
                case "real":
                    return "single";
                case "smallint":
                    return "Int16";
                case "tinyint":
                    return "Byte";
                case "uniqueidentifier":
                    return "Guid";

                default:
                    return null;
            }
        }

        //-------
        private string CreateSqlQuery(string connection, string procedureName)
        {
            string sqlQuery = string.Empty;
            if (!procedureName.StartsWith("exec", StringComparison.OrdinalIgnoreCase) && !procedureName.StartsWith("select", StringComparison.OrdinalIgnoreCase))
            {
                StringBuilder completeProc = new StringBuilder();
                completeProc.Append("Exec " + procedureName + " ");
                using (SqlConnection conn = new SqlConnection(connection))
                {
                    SqlCommand cmd = new SqlCommand(procedureName, conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    SqlCommandBuilder.DeriveParameters(cmd);
                    foreach (SqlParameter p in cmd.Parameters)
                    {
                        if (p.Direction != ParameterDirection.ReturnValue)
                        {
                            completeProc.Append(p.ParameterName + "=null,");
                        }

                    }
                    sqlQuery = completeProc.ToString().TrimEnd(',');
                }
            }
            else
                sqlQuery = procedureName;
            return sqlQuery;
        }
        public string GenerateResultClass(string connection, string query, string clasName, string Spname)
        {
            StringBuilder sbuilder = new StringBuilder();
            try
            {
                List<List<SchemaField>> ColumnsDSetList = GetSchemaFields(connection, query);
                int classnum = 0;
                foreach (var Columns in ColumnsDSetList)
                {
                    sbuilder.Append($"\n \t  // {Spname}");
                    sbuilder.Append($"\n \t public partial class {clasName ?? Spname}Result");
                    sbuilder.Append("\r\n \t {");
                    sbuilder.Append("\r\n \t");
                    for (int i = 0; i < Columns.Count; i++)
                    {
                        string IsNullableProperty = string.Empty;
                        string columnName = Columns[i].ColumnName;
                        string typename = GetNetDataType(Columns[i].DataTypeName);
                        if (typename != "string")
                        {
                            IsNullableProperty = Columns[i].AllowDBNull ? "?" : string.Empty;
                        }
                        sbuilder.Append(string.Format("\t \t public {0}{1} {2} {{ get; set; }}", typename, IsNullableProperty, columnName));
                        sbuilder.Append("\r\n \t");
                    }
                    sbuilder.Append(" }");
                    sbuilder.Append("\r\n \t");
                    sbuilder.Append("\r\n \t");
                }
                return sbuilder.ToString();
            }
            catch (Exception ex)
            {
                sbuilder.Append($"/* Error: {Spname}\n \t {ex.Message.ToString()}  \n */");
                Errors.Add(Spname, ex.Message);
                return sbuilder.ToString();
            }
        }
        private List<List<SchemaField>> GetSchemaFields(string ConnectionString, string Query)
        {
            // DataTable SchemaTable = GetSchema(Query, ConnectionString);

            DataSet SchemaDataSet = GetSchema(Query, ConnectionString);


            List<List<SchemaField>> resultDataSet = new List<List<SchemaField>>();

            foreach (DataTable SchemaTable in SchemaDataSet.Tables)
            {
                List<SchemaField> result = new List<SchemaField>();
                for (int i = 0; i < SchemaTable.Rows.Count; i++)
                {
                    var schemaFieldRow = new SchemaField();
                    for (int j = 0; j < SchemaTable.Columns.Count; j++)
                    {

                        if (SchemaTable.Rows[i][j] != System.DBNull.Value)
                        {
                            switch (SchemaTable.Columns[j].ColumnName)
                            {
                                case "ColumnName":
                                    schemaFieldRow.ColumnName = SchemaTable.Rows[i][j].ToString();
                                    break;
                                case "ColumnSize":
                                    schemaFieldRow.ColumnSize = Convert.ToInt32(SchemaTable.Rows[i][j]);
                                    break;
                                case "DataType":
                                    schemaFieldRow.DataType = ((System.Type)SchemaTable.Rows[i][j]).FullName;
                                    break;
                                case "AllowDBNull":
                                    schemaFieldRow.AllowDBNull = Convert.ToBoolean(SchemaTable.Rows[i][j]);
                                    break;
                                case "DataTypeName":
                                    schemaFieldRow.DataTypeName = SchemaTable.Rows[i][j].ToString();
                                    break;
                            }
                        }
                    }
                    result.Add(schemaFieldRow);
                }
                resultDataSet.Add(result);
            }

            return resultDataSet;
        }
        private DataSet GetSchema(string storedProcedureText, string connection)
        {
            DataSet dtSet = new DataSet();
            DataTable DTSchema = null;
            int i = 0;
            using (SqlConnection sqlCon = new SqlConnection(connection))
            {
                //storedProcedureText = "SET FMTONLY ON;" + storedProcedureText;
                SqlCommand sqlCmd = new SqlCommand(storedProcedureText, sqlCon);
                sqlCon.Open();
              //  SqlDataReader sqlReader = sqlCmd.ExecuteReader(CommandBehavior.SchemaOnly);
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();
                while (sqlReader.FieldCount != 0)
                {
                    i++;
                    DataTable dt = new DataTable();
                    // DTSchema = sqlReader.GetSchemaTable();
                    dt = sqlReader.GetSchemaTable();
                    dt.TableName = "dtTable" + i;
                    sqlReader.NextResult();
                    dtSet.Tables.Add(dt);
                }
            }
            return dtSet;

        }


        //-- repo Generation 
        public string GenerateRepo(List<SPlist> param, string Namespace, string ClassName)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("using System; \n");
            sb.Append("using System.Collections.Generic; \n");
            if (Home.IsSpHelper)
                sb.Append($"using {Home.SphelperNamespace };\n");
            sb.Append($"namespace {Namespace}\n");
            sb.Append("{ \n");
            sb.Append($"\n \t public class {ClassName}");
            sb.Append("\n \t \t {");
            sb.Append("\n \t \t SpHelper  _SpHelper;");
            sb.Append($"\n \t \t public  {ClassName}(SpHelper spHelper)" + "\n \t\t\t{ \n \t\t\t _SpHelper = spHelper; \n \t\t\t}");
            foreach (var sp in param)
            {
                if (Errors.Any(x => x.Key == sp.Spname))
                    continue;
                string methodName = sp.MethodName ?? $"Execute{sp.Spname}";
                methodName = methodName.Replace("[", "");
                methodName = methodName.Replace("]", "");
                methodName = methodName.Replace(".", "");
                //method
                sb.Append($"\n \t \t \t public {GetRetunType(sp)} {methodName} ({GetParamType(sp)} param) \n \t \t \t => {GetQueryMethod(sp)} ;\n\n");
            }

            sb.Append("\n \t \t } \n");
            sb.Append("} \n");

            return sb.ToString();
        }
        public string GenerateRepoMethod(SPlist sp, string MethodName = default(string))
        {
            string methodName = sp.MethodName ?? $"Execute{sp.Spname}";
            methodName = methodName.Replace("[", "");
            methodName = methodName.Replace("]", "");
            methodName = methodName.Replace(".", "");
            sbuilder.Append($"\n \t \t \t public {GetRetunType(sp)} {methodName} ({GetParamType(sp)} param) => {GetQueryMethod(sp)} ");
            return sbuilder.ToString();
        }
        private string GetRetunType(SPlist param)
        {

            if (param.IsNonQuery)
                return "bool";
            if (param.IsSingle)
                return param.ClassName;
            string returnTypeName = param.ClassName;
            if (string.IsNullOrEmpty(param.ClassName))
            {
                returnTypeName = param.Spname;
                returnTypeName = returnTypeName.Replace("[", "");
                returnTypeName = returnTypeName.Replace("]", "");
                returnTypeName = returnTypeName.Replace(".", "");
            }


            return $"List<{returnTypeName}Result>";
        }
        private string GetParamType(SPlist param)
        {
            string paramName = param.ClassName;
            if (string.IsNullOrEmpty(param.ClassName))
            {
                paramName = param.Spname;
                paramName = paramName.Replace("[", "");
                paramName = paramName.Replace("]", "");
                paramName = paramName.Replace(".", "");
            }
            return $"{paramName}Param";
        }
        private string GetQueryMethod(SPlist param, string MethodName = default(string))
        {

            string paramName = string.IsNullOrEmpty(MethodName) ? param.ClassName : MethodName;
            if (string.IsNullOrEmpty(MethodName) && string.IsNullOrEmpty(param.ClassName))
            {
                paramName = param.Spname;
                paramName = paramName.Replace("[", "");
                paramName = paramName.Replace("]", "");
                paramName = paramName.Replace(".", "");
            }

            if (param.IsNonQuery)
                return $"_SpHelper.NonQuery(SP.{paramName},param)";
            else
                return $"_SpHelper.Query<{paramName}Result>(SP.{paramName},param)";
        }
        //-- Get Splist from DB

        public List<string> GetSpListFromDB(string schemaName = "dbo")
        {
            List<string> Sps = new List<string>();
            using (SqlConnection con = new SqlConnection(Home.Constr))
            {
                SqlCommand cmd = new SqlCommand($"SELECT SchemaName = s.name,ProcedureName = pr.name FROM sys.procedures pr INNER JOIN sys.schemas s ON pr.schema_id = s.schema_id where s.name='{schemaName}'", con);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Sps.Add($" {dr["SchemaName"].ToString()}.{dr["ProcedureName"].ToString()}");
                }


            }
            return Sps;
        }

        //-- Table to class
        public string ConvertTableToClass(string tblName)
        {
            using (SqlConnection con = new SqlConnection(Home.Constr))
            {
                string qry = @"declare @TableName sysname = '-TBL-'
declare @Result varchar(max) = 'public class ' + @TableName + '
{'

select @Result = @Result + '
    public ' + ColumnType + NullableSign + ' ' + ColumnName + ' { get; set; }
'
from
(
    select 
        replace(col.name, ' ', '_') ColumnName,
        column_id ColumnId,
        case typ.name 
            when 'bigint' then 'long'
            when 'binary' then 'byte[]'
            when 'bit' then 'bool'
            when 'char' then 'string'
            when 'date' then 'DateTime'
            when 'datetime' then 'DateTime'
            when 'datetime2' then 'DateTime'
            when 'datetimeoffset' then 'DateTimeOffset'
            when 'decimal' then 'decimal'
            when 'float' then 'float'
            when 'image' then 'byte[]'
            when 'int' then 'int'
            when 'money' then 'decimal'
            when 'nchar' then 'string'
            when 'ntext' then 'string'
            when 'numeric' then 'decimal'
            when 'nvarchar' then 'string'
            when 'real' then 'double'
            when 'smalldatetime' then 'DateTime'
            when 'smallint' then 'short'
            when 'smallmoney' then 'decimal'
            when 'text' then 'string'
            when 'time' then 'TimeSpan'
            when 'timestamp' then 'DateTime'
            when 'tinyint' then 'Byte'
            when 'uniqueidentifier' then 'Guid'
            when 'varbinary' then 'byte[]'
            when 'varchar' then 'string'
            else 'UNKNOWN_' + typ.name
        end ColumnType,
        case 
            when col.is_nullable = 1 and typ.name in ('bigint', 'bit', 'date', 'datetime', 'datetime2', 'datetimeoffset', 'decimal', 'float', 'int', 'money', 'numeric', 'real', 'smalldatetime', 'smallint', 'smallmoney', 'time', 'tinyint', 'uniqueidentifier') 
            then '?' 
            else '' 
        end NullableSign
    from sys.columns col
        join sys.types typ on
            col.system_type_id = typ.system_type_id AND col.user_type_id = typ.user_type_id
    where object_id = object_id(@TableName)
) t
order by ColumnId

set @Result = @Result  + '
}'
Select @Result";
                qry = qry.Replace("-TBL-", tblName);
                SqlCommand cmd = new SqlCommand(qry, con);
                con.Open();
                string result = cmd.ExecuteScalar().ToString();
                con.Close();
                return result;
            }
        }
    }



    public class SpDetail
    {
        public string Name { get; set; }
        public List<ParamDetail> Parameters { get; set; }


    }
    public class ParamDetail
    {
        public string Parameter_name { get; set; }
        public int Param_order { get; set; }
        public string DBType { get; set; }
        public string NetType { get; set; }


    }
    public class SchemaField
    {
        public string ColumnName { get; set; }
        public string DataType { get; set; }
        public bool AllowDBNull { get; set; }
        public string DataTypeName { get; set; }
        public int ColumnSize { get; set; }
    }

    public class SPlist
    {
        public int id { get; set; }
        public string Spname { get; set; }
        public string ClassName { get; set; }
        public string MethodName { get; set; }
        public bool IsNonQuery { get; set; }
        public bool IsSingle { get; set; }
        public bool isError { get; set; }
    }
    public class SpParamResultDetail
    {
        public string Param { get; set; }
        public string Json { get; set; }
        public string Result { get; set; }
        public string Repo { get; set; }
        public string RepoMethod { get; set; }
        public string Consts { get; set; }
    }
    public enum QeryMode
    {
        Query = 0,
        NonQuery = 1
    }
    public enum ReturnMode
    {
        List = 0,
        Single = 1
    }
}






