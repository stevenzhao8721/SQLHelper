using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLHelper
{
    public class sqlhelper
    {

        private string _constring, _logpath;
        /// <summary>
        /// 数据库帮助类
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="logfilePath">故障记录文件路径</param>
        public sqlhelper(string connectionString, string logfilePath)
        {
            _constring = connectionString;
            _logpath = logfilePath;
        }

        private void ErrorLog(string errmessage)
        {
            try
            {
                StreamWriter sw = new StreamWriter(_logpath,true);
                //开始写入
                sw.WriteLine(errmessage + "||| time:" + DateTime.Now);
                //清空缓冲区
                sw.Flush();
                //关闭流
                sw.Close();
            }
            catch
            {

            }           
        }

        /// <summary>
        /// 同步CURD
        /// </summary>
        /// <param name="sqlcmd"></param>
        /// <param name="paras"></param>
        /// <param name="commandType"></param>
        /// <returns>Error return:-99</returns>
        public int ExecuteSync(string sqlcmd,SqlParameter[] paras=null, CommandType commandType = CommandType.Text)
        {
            using(var newconnection = new SqlConnection(_constring))
            using (var cmd = new SqlCommand(sqlcmd, newconnection))
            {
                try
                {
                    if (paras != null)
                        cmd.Parameters.AddRange(paras);
                    cmd.CommandType = commandType;
                    newconnection.Open();
                    return cmd.ExecuteNonQuery();
                }
                catch(Exception ex)
                {
                    ErrorLog(ex.Message + "|| cmd: sqlcmd");
                    return -99;
                }              
            }
        }
        /// <summary>
        /// 异步CURD
        /// </summary>
        /// <param name="sqlcmd"></param>
        /// <param name="paras"></param>
        /// <param name="commandType"></param>
        /// <returns>Error return:-99</returns>
        public async Task<int> ExecuteAsync(string sqlcmd, SqlParameter[] paras = null, CommandType commandType = CommandType.Text)
        {
            using (var newconnection = new SqlConnection(_constring))
            using (var cmd = new SqlCommand(sqlcmd, newconnection))
            {
                try
                {
                    if (paras != null)
                        cmd.Parameters.AddRange(paras);
                    cmd.CommandType = commandType;
                    await newconnection.OpenAsync().ConfigureAwait(false);
                    return await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
                catch(Exception ex)
                {
                    ErrorLog(ex.Message + "|| cmd: sqlcmd");
                    return -99;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlcmd"></param>
        /// <param name="paras"></param>
        /// <param name="commandType"></param>
        /// <returns>Error return:null</returns>
        public DataTable ExecuteQuerySync(string sqlcmd, SqlParameter[] paras = null, CommandType commandType = CommandType.Text)
        {
            DataTable dt = new DataTable();
            using (var newconnection = new SqlConnection(_constring))
            using (var cmd = new SqlCommand(sqlcmd, newconnection))
            {
                try
                {
                    if (paras != null)
                        cmd.Parameters.AddRange(paras);
                    cmd.CommandType = commandType;
                    newconnection.Open();
                    dt.Load(cmd.ExecuteReader(CommandBehavior.CloseConnection));
                    return dt;
                }
                catch (Exception ex)
                {
                    ErrorLog(ex.Message + "|| cmd: sqlcmd");
                    return null;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlcmd"></param>
        /// <param name="paras"></param>
        /// <param name="commandType"></param>
        /// <returns>Error return:null</returns>
        public async Task<DataTable> ExecuteQueryAsync(string sqlcmd, SqlParameter[] paras = null, CommandType commandType = CommandType.Text)
        {
            DataTable dt = new DataTable();
            using (var newconnection = new SqlConnection(_constring))
            using (var cmd = new SqlCommand(sqlcmd, newconnection))
            {
                try
                {
                    if (paras != null)
                        cmd.Parameters.AddRange(paras);
                    cmd.CommandType = commandType;
                    await newconnection.OpenAsync().ConfigureAwait(false);
                    dt.Load(await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection).ConfigureAwait(false));
                    return dt;
                }
                catch (Exception ex)
                {
                    ErrorLog(ex.Message + "|| cmd: sqlcmd");
                    return null;
                }
            }
        }

       /// <summary>
       /// 
       /// </summary>
       /// <param name="sqlcmd"></param>
       /// <param name="paras"></param>
       /// <param name="commandType"></param>
       /// <returns>Error return:null</returns>
        public string SingleDataRead(string sqlcmd, SqlParameter[] paras=null, CommandType commandType = CommandType.Text)
        {
            using (var newconnection = new SqlConnection(_constring))
            using (var cmd = new SqlCommand(sqlcmd, newconnection))
            {
                try
                {
                    if (paras != null)
                        cmd.Parameters.AddRange(paras);
                    cmd.CommandType = commandType;
                    newconnection.Open();
                    string result = cmd.ExecuteScalar().ToString().Trim();
                    newconnection.Close();
                    return result;
                }
                catch(Exception ex)
                {
                    ErrorLog(ex.Message + "|| cmd: sqlcmd");
                    return null;
                }                
            }           
        }
    }
}
