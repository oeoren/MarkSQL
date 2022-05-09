using Dapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using MarkSql.Shared;

namespace MarkSql.ServerLib
{
    public interface IModelBuilder
    {

    }
    public class ModelBuilder : IModelBuilder
    {
        private readonly ILogger _logger;
        private readonly DapperContext _context;

        ModelBuilder(ILogger logger, DapperContext dapperContext)
        {
            _logger = logger; 
            _context = dapperContext;
        }

        public async Task<Model> ConstructModel(string conString)
        {
            Model model = new Model();
            using (var connection = _context.CreateConnection())
            {
                string query = @"SELECT  
					ProcedureName = ir.ROUTINE_NAME, 
					ParameterName = COALESCE(ip.PARAMETER_NAME, '<no params>'),
                    SqlType = ip.DATA_TYPE,  Precision = ip.NUMERIC_PRECISION, Scale = ip.NUMERIC_SCALE, MaxLen = ip.CHARACTER_MAXIMUM_LENGTH,
					DataType = COALESCE(UPPER(ip.DATA_TYPE) + CASE 
						WHEN ip.DATA_TYPE IN ('NUMERIC', 'DECIMAL') THEN  
							'(' + CAST(ip.NUMERIC_PRECISION AS VARCHAR)  
							+ ', ' + CAST(ip.NUMERIC_SCALE AS VARCHAR) + ')'  
						WHEN RIGHT(ip.DATA_TYPE, 4) = 'CHAR' THEN 
							'(' + CAST(ip.CHARACTER_MAXIMUM_LENGTH AS VARCHAR) + ')' 
						ELSE '' END + CASE ip.PARAMETER_MODE  
						WHEN 'INOUT' THEN ' OUTPUT' ELSE ' ' END, '-'),
					ParameterMode =	ip.PARAMETER_MODE 
				FROM  
					INFORMATION_SCHEMA.ROUTINES ir 
					LEFT OUTER JOIN 
					INFORMATION_SCHEMA.PARAMETERS ip 
					ON ir.ROUTINE_NAME = ip.SPECIFIC_NAME 
				WHERE 
					ir.ROUTINE_NAME LIKE 'API%' 
					AND ir.ROUTINE_TYPE = 'PROCEDURE' 
					AND COALESCE(OBJECTPROPERTY 
					( 
						OBJECT_ID(ip.SPECIFIC_NAME), 
						'IsMsShipped' 
					), 0) = 0 
				ORDER BY  
					ir.ROUTINE_NAME, 
					ip.ORDINAL_POSITION";

                try
                {
                    var rows = await connection.QueryAsync(query);

                    foreach (var row in rows)
                    {
                        string procName = row["ProcedureName"].ToString();
                        ControllerInfo controller = GetControllerInfo(model, procName);
                        if (controller != null)
                        {
                            ProcInfo proc = GetProcInfo(controller, procName);
                            string parameterName = row["ParameterName"].ToString();
                            if (parameterName != "<no params>")
                                proc.parameters.Add(new ParameterInfo
                                {
                                    name = parameterName,
                                    precision = (DBNull.Value == row["Precision"]) ? 0 : int.Parse(row["Precision"].ToString()),
                                    scale = (DBNull.Value == row["Scale"]) ? 0 : int.Parse(row["Scale"].ToString()),
                                    maxLen = (DBNull.Value == row["Maxlen"]) ? 0 : int.Parse(row["MaxLen"].ToString()),
                                    sqlType = row["SqlType"].ToString(),
                                    isOutput = (row["ParameterMode"].ToString() != "IN")
                                });
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                }
            }
            return model;
        }
/*
        private static void GetXmlComments(SqlConnection con, ProcInfo proc)
        {
            SqlCommand sqlCommand = new SqlCommand("sys.sp_helptext", con);
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.Parameters.AddWithValue("@objname", "api_" + proc.name);
            DataSet ds = new DataSet();
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
            sqlDataAdapter.SelectCommand = sqlCommand;
            sqlDataAdapter.Fill(ds);
            string comments = "";
            if (ds.Tables.Count > 0)
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    if (r.ItemArray.Length > 0)
                    {
                        string l = r[0].ToString();
                        l = l.Trim();
                        if (l != null && l.Length > 3 && l.Substring(0, 3) == "---")
                            comments += "///" + l.Substring(3) + "\n";
                    }
                }
            proc.xmlComments = comments;
        }

*/

        private static string SkipAPI(string name)
        {
            name = name.ToLower();
            if (!name.StartsWith("api"))
                return null;
            name = name.Substring(3);
            if (name.StartsWith("_"))
                name = name.Substring(1);
            return name;
        }

        private static ProcInfo GetProcInfo(ControllerInfo controller, string procName)
        {
            procName = SkipAPI(procName);
            foreach (var proc in controller.procs)
                if (proc.name == procName)
                    return proc;
            ProcInfo newProc = new ProcInfo { name = procName };
            controller.procs.Add(newProc);
            return newProc;
        }

        private static ControllerInfo GetControllerInfo(Model model, string procName)
        {
            procName = SkipAPI(procName);
            if (procName.EndsWith("delete"))
                procName = procName.Substring(0, procName.Length - 6);
            else if (procName.EndsWith("get"))
                procName = procName.Substring(0, procName.Length - 3);
            else if (procName.EndsWith("put"))
                procName = procName.Substring(0, procName.Length - 3);
            else if (procName.EndsWith("post"))
                procName = procName.Substring(0, procName.Length - 4);
            else
                return null;

            if (procName.EndsWith("_"))
                procName = procName.Substring(0, procName.Length - 1);

            foreach (var controller in model.controllers)
                if (controller.name == procName)
                    return controller;
            ControllerInfo newController = new ControllerInfo { name = procName };
            model.controllers.Add(newController);
            return newController;
        }
    }
}
