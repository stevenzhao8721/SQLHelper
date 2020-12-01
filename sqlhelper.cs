using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLHelper
{
    public class sqlhelper
    {

        private string _constring;
        /// <summary>
        /// SQL数据库
        /// </summary>
        /// <param name="connectionString">数据库连接配置</param>
        public sqlhelper(string connectionString)
        {
            _constring = connectionString；
        }
        
        public int ExecuteSync(string sqlcmd,Sqlparameter[] paras=null)
        {

        }





        private static SqlConnection conn = null;
        private static SqlCommand cmd = null;
        private static SqlDataReader sdr = null;

        /// <summary>
        /// 在App.config 里设置目标数据库的连接信息，然后使用该类
        /// </summary>
        /// <param name="sqldatabase">数据库名称</param>
        public void connect(string sqldatabase)
        {
            conn = new SqlConnection(sqldatabase);
        }

        private SqlConnection GetConn()
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            return conn;
        }

        /// <summary>
        /// SQL C\U\D
        /// </summary>
        /// <param name="cmdText">sql命令语句</param>
        /// <param name="paras">可选cmd参数</param>
        /// <param name="commandType">可选command类型</param>
        /// <returns></returns>
        public int CUD(string cmdText, SqlParameter[] paras = null, CommandType commandType = CommandType.Text)
        {
            int res;
            using (cmd = new SqlCommand(cmdText, GetConn()))
            {
                if (paras != null)
                    cmd.Parameters.AddRange(paras);
                cmd.CommandType = commandType;
                res = cmd.ExecuteNonQuery();
            }
            return res;
        }

        /// <summary>
        /// SQL Read，异步
        /// </summary>
        /// <param name="cmdText">sql命令语句</param>
        /// <param name="callback">回调函数</param>
        /// <param name="paras">可选cmd参数</param>
        /// <param name="commandType">可选command类型</param>
        public void AsyncRead(string cmdText, AsyncCallback callback, SqlParameter[] paras = null, CommandType commandType = CommandType.Text)
        {
            DataTable dt = new DataTable();
            cmd = new SqlCommand(cmdText, GetConn());
            cmd.CommandType = commandType;
            if (paras != null)
                cmd.Parameters.AddRange(paras);
            cmd.BeginExecuteReader(callback, cmd);
        }

        /// <summary>
        /// SQL Read，同步
        /// </summary>
        /// <param name="cmdText">带参数的SQL查询语句</param>
        /// <param name="paras">参数集合</param>
        /// <param name="ct">执行类型</param>
        /// <returns>DataTable</returns>
        public DataTable SyncRead(string cmdText, SqlParameter[] paras=null, CommandType commandType = CommandType.Text)
        {
            DataTable dt = new DataTable();
            cmd = new SqlCommand(cmdText, GetConn());
            if (paras != null)
                cmd.Parameters.AddRange(paras);
            cmd.CommandType = commandType;
            using (sdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
            {
                dt.Load(sdr);
            }
            return dt;
        }

        /// <summary>
        /// 查询单个数据
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="paras"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public string SingleDataRead(string cmdText, SqlParameter[] paras=null, CommandType commandType = CommandType.Text)
        {
            string result = "";
            using (cmd=new SqlCommand(cmdText,GetConn()))
            {
                if (paras != null)
                    cmd.Parameters.AddRange(paras);
                cmd.CommandType = commandType;
                result = cmd.ExecuteScalar().ToString().Trim();
            }
            return result;
        }

        /// <summary>
        /// 异步参考
        /// </summary>
        /// <param name="ar"></param>
        private void ProcessData(IAsyncResult ar)
        {
            SqlCommand cmd = (SqlCommand)ar.AsyncState;
            using (cmd.Connection)
            {
                using (cmd)
                {
                    SqlDataReader sdr = cmd.EndExecuteReader(ar);
                    DataTable dt = new DataTable();
                    dt.Load(sdr);
                }
            }
        }
    }
}
