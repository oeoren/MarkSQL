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
    public interface IMarkModelBuilder : IServiceProvider
    {
        Task<MarkModel> ConstructMarkModel();
        Task<MarkModel> GetModel();
    
    };
    public class MarkModelBuilder : IMarkModelBuilder 
    {
        private readonly ILogger _logger;
        private readonly DapperContext _context;

        MarkModel? _model;

        public MarkModelBuilder(ILogger<MarkModelBuilder> logger, DapperContext dapperContext)
        {
            _logger = logger; 
            _context = dapperContext;
            _model = null;
        }

        public async Task<MarkModel?> GetModel() {
            if (_model == null)
                _model = await ConstructMarkModel();
            return _model; 
        }
        public async Task<MarkModel> ConstructMarkModel()
        {
            _model = new MarkModel();
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
					ir.ROUTINE_NAME LIKE 'mq_%' 
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
                        string procName = row.ProcedureName;
                        var proc = GetProcInfo(_model, procName);
                        if (proc != null)
                        {
                            string parameterName = row.ParameterName;
                            if (parameterName != "<no params>")
                            {
                                var param = new ParameterInfo
                                {
                                    name = parameterName,
                                    precision = (row.Precision == null) ? null : int.Parse(row.Precision),
                                    scale = (row.Scale == null) ? null : int.Parse(row.Scale),
                                    maxLen = (row.Maxlen == null) ? null : int.Parse(row.MaxLen),
                                    sqlType = row.SqlType,
                                    isOutput = (row.ParameterMode != "IN")
                                };
                                proc.parameters.Add(param);
                            }
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
            return _model;
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

        private static ProcInfo GetProcInfo(MarkModel model, string procName)
        {
            foreach (var proc in model.procs)
                if (proc.name == procName)
                    return proc;
            ProcInfo newController = new ProcInfo { name = procName };
            model.procs.Add(newController);
            return newController;
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

        public object? GetService(Type serviceType)
        {
            throw new NotImplementedException();
        }
    }
}
