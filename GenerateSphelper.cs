
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System;
using Newtonsoft.Json;

using System.Reflection;
using System.Data;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.Entity;
using SpParamClassGenerater;
namespace Generator.SpHelper
{

    class GenerateSphelper
    {
        static string cd;
        string path;
        static GenerateSphelper()
        {
            cd = @"
using System.Data.Entity;
using System.Reflection;
using System.Data;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace -NMSP-
{
public class SpHelper
    {
        DbContext _DBContext;

        public SpHelper()
        {
            _DBContext = new DbContext(""-CONSTR-"");


        }

        public dynamic Query(string qry)
        {
            return _DBContext.Database.SqlQuery<dynamic>(qry).ToList();
        }
        public List<T> Query<T>(string qry)
        {
            List<T> result = _DBContext.Database.SqlQuery<T>(qry).ToList();
            return result;
        }
        public List<T> Query<T>(string spName, List<CodeNameValue> codeValueNames)
        {
            List<SqlParameter> sqlParams = new List<SqlParameter>();
            codeValueNames.ForEach(x => { sqlParams.Add(new SqlParameter() { ParameterName = x.Name, Value = x.Value }); });
            string sp = GetSPwithSchema(spName + "" "" + string.Join("","", codeValueNames.Select(x => ""@"" + x.Name).ToArray()));
            _DBContext.Database.CommandTimeout = 0;
            List<T> result = _DBContext.Database.SqlQuery<T>(sp, sqlParams.ToArray()).ToList();
            return result;
        }
        public List<Tresult> Query<Tresult>(string SpName, object param)
        {
            List<CodeNameValue> reultParams = GetParam(SpName, param);
            return Query<Tresult>(SpName, reultParams.OrderBy(x => x.ParamOrder).ToList());
        }

        public IList<Tresult> Query<Tresult>(string SpName, string ParamName, object paramValue)
        {
            var param = new SqlParameter() { ParameterName = ParamName, Value = paramValue };
            var res = _DBContext.Database.SqlQuery<Tresult>(GetSPwithSchema(SpName) + ""  @"" + ParamName, param).ToList();
            return res;
        }
        public Tresult QuerySingle<Tresult>(string SpName, string ParamName, object paramValue) where Tresult : new()
        {
            var param = new SqlParameter() { ParameterName = ParamName, Value = paramValue };
            var res = _DBContext.Database.SqlQuery<Tresult>(GetSPwithSchema(SpName) + ""  @"" + ParamName, param).ToList();
            if (res.Any())
                return res[0];
            else
                return new Tresult();
        }


        public bool NonQuery(string SpName, object param)
        {
            List<CodeNameValue> reultParams = GetParam(SpName, param);
            return NonQuery(SpName, reultParams.OrderBy(x => x.ParamOrder).ToList());
        }
        public bool NonQuery(string spName, List<CodeNameValue> codeValueNames)
        {
            return Query<dynamic>(spName, codeValueNames).Count >= 0;
        }

        private string GetSPwithSchema(string sp)
        {
            return ConfigurationManager.AppSettings[""SchemaName""].ToString() + ""."" + sp;
        }

        private static List<CodeNameValue> GetParam<Tparam>(string SpName, Tparam param)
        {
            var res = SpHelper.SpList.FirstOrDefault(x => x.Name == SpName)?.Parameters;
            var reultParams = new List<CodeNameValue>();
            IList<PropertyInfo> properties = typeof(Tparam).GetProperties().ToList();
            if (properties.Count == 0)
                properties = param.GetType().GetProperties().ToList();
            foreach (var property in properties)
            {
                if (res.Any(x => x.Parameter_name.Contains(property.Name)))
                {
                    CodeNameValue obj = new CodeNameValue();

                    obj.Name = property.Name;
                    obj.Value = property.GetValue(param) ?? string.Empty;
                    obj.ParamOrder = res.FirstOrDefault(x => x.Parameter_name.ToUpper() == property.Name.ToUpper()).Param_order;

                    reultParams.Add(obj);
                }

            }

            return reultParams;
        }
        //public static List<SpDetail> SpList { get; set; }

public static List<SpDetail> SpList
            => Newtonsoft.Json.JsonConvert.DeserializeObject<List<SpDetail>>(System.IO.File.ReadAllText(""-PATH-""));


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
    public class CodeNameValue
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public object Value { get; set; }
        public int ParamOrder { get; set; }

    }
}
";
        }
        public static string CodeStr => cd.Replace("-CONSTR-", Home.Constr).Replace("-NMSP-", Home.SphelperNamespace);

       
    }


}
