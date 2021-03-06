//******************************************************************
//*  功能說明：自動化大宗檔匯入業務邏輯層
//*  作    者：Simba Liu
//*  創建日期：2010/04/26
//*  修改記錄：
//*<author>            <time>            <TaskID>            <desc>
//*******************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using Framework.Data.OM;
using Framework.Data.OM.Collections;
using Framework.Data.OM.Transaction;
using EntityLayer;
using System.Data.SqlClient;
using System.Data;

namespace BusinessRules
{
    public class BRM_LBatchLog : CSIPCommonModel.BusinessRules.BRBase<EntityM_LBatchLog>
    {
        #region sql語句
        public const string SEL_FKEY_FORCHK = @"SELECT TOP 1 STATUS FROM  L_BATCH_LOG WHERE  FUNCTION_KEY= @FUNCTION_KEY AND JOB_ID=@JOB_ID AND SUBSTRING(CONVERT(VARCHAR,START_TIME,111),0,11)=@START_TIME ORDER BY START_TIME DESC";
        public const string DEL_RUNNING = "DELETE from  L_BATCH_LOG  where FUNCTION_KEY= @FUNCTION_KEY AND JOB_ID=@JOB_ID AND START_TIME=@START_TIME AND STATUS=@STATUS";
        #endregion

        /// <summary>
        /// 功能說明:新增一筆LBatchLog資料
        /// 作    者:Simba Liu
        /// 創建時間:2010/04/26
        /// 修改記錄:
        /// </summary>
        /// <param name="LBatchLog"></param>
        /// <returns></returns>
        public static bool insert(EntityM_LBatchLog LBatchLog)
        {
            try
            {
                using (OMTransactionScope ts = new OMTransactionScope("Connection_CSIP"))
                {
                    if (BRM_LBatchLog.AddNewEntity(LBatchLog, "Connection_CSIP"))
                    {
                        ts.Complete();
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
            }
            catch (Exception exp)
            {
                BRM_LBatchLog.SaveLog(exp.Message);
                return false;
            }
        }

        /// <summary>
        /// 功能說明:刪除一筆LBatchLog資料
        /// 作    者:Simba Liu
        /// 創建時間:2010/04/26
        /// 修改記錄:
        /// </summary>
        /// <param name="LBatchLog"></param>
        /// <param name="strCondition"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool delete(EntityM_LBatchLog LBatchLog, string strCondition, ref string strMsgID)
        {
            try
            {

                using (OMTransactionScope ts = new OMTransactionScope("Connection_CSIP"))
                {
                    if (BRM_LBatchLog.DeleteEntityByCondition(LBatchLog, strCondition, "Connection_CSIP"))
                    {
                        ts.Complete();
                        strMsgID = "06_06040100_005";
                        return true;
                    }
                    else
                    {
                        strMsgID = "06_06040100_006";
                        return false;
                    }
                }
            }
            catch (Exception exp)
            {
                BRM_LBatchLog.SaveLog(exp.Message);
                strMsgID = "06_06040100_006";
                return false;
            }
        }

        /// <summary>
        /// 功能說明:更新一筆LBatchLog資料
        /// 作    者:Simba Liu
        /// 創建時間:2010/04/26
        /// 修改記錄:
        /// </summary>
        /// <param name="LBatchLog"></param>
        /// <param name="strCondition"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool update(EntityM_LBatchLog LBatchLog, string strCondition, ref string strMsgID, params  string[] FiledSpit)
        {
            try
            {
                using (OMTransactionScope ts = new OMTransactionScope("Connection_CSIP"))
                {
                    if (BRM_LBatchLog.UpdateEntityByCondition(strCondition, LBatchLog, "Connection_CSIP", FiledSpit))
                    {
                        ts.Complete();
                        strMsgID = "06_06040100_003";
                        return true;
                    }
                    else
                    {
                        strMsgID = "06_06040100_004";
                        return false;
                    }
                }
            }
            catch (Exception exp)
            {
                BRM_LBatchLog.SaveLog(exp.Message);
                strMsgID = "06_06040100_004";
                return false;
            }
        }


        /// <summary>
        /// 功能說明:查詢Ftp登陸失敗次數
        /// 作    者:Simba Liu
        /// 創建時間:2010/04/26
        /// 修改記錄:
        /// </summary>
        /// <param name="dtLBatchLog"></param>
        /// <param name="strLoginTime"></param>
        /// <returns></returns>
        public static bool SearchLoginCount(ref  int LoginCount, string strLoginTime)
        {
            try
            {
                string sql = @"select * from 
                                (
                                SELECT FUNCTION_KEY,JOB_ID,COUNT(1) AS TryNUM, min(START_TIME) AS FIRST_TIME
                                FROM L_BATCH_LOG
                                WHERE START_TIME > '2010/03/08' AND STATUS = 'F'
                                GROUP BY FUNCTION_KEY,JOB_ID
                                ) AS A
                                LEFT OUTER JOIN 
                                (
                                SELECT FUNCTION_KEY,JOB_ID
                                FROM L_BATCH_LOG
                                WHERE START_TIME > '2010/03/08' AND STATUS = 'S'
                                GROUP BY FUNCTION_KEY,JOB_ID
                                ) AS B
                                ON A.FUNCTION_KEY = B.FUNCTION_KEY AND A.JOB_ID = B.JOB_ID
                                WHERE ISNULL(B.JOB_ID,'') = ''
                                ORDER BY A.FIRST_TIME";

                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = sql;
                DataSet ds = BRM_LBatchLog.SearchOnDataSet(sqlcmd, "Connection_CSIP");
                if (ds != null)
                {
                    DataTable dtLBatchLog = ds.Tables[0];
                    if (dtLBatchLog != null && dtLBatchLog.Rows.Count > 0)
                    {
                        LoginCount = int.Parse(dtLBatchLog.Rows[0][0].ToString());
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception exp)
            {
                BRM_LBatchLog.SaveLog(exp.Message);
                return false;
            }

        }


        /// <summary>
        /// 功能說明:判断是否有重復的屬性匯入日志
        /// 作    者:Simba Liu
        /// 創建時間:2010/04/26
        /// 修改記錄:
        /// </summary>
        /// <param name="LBatchLog">LBatchLog</param>
        /// <returns>Repeat true,unrepeat false</returns>
        public static bool IsRepeat(EntityM_LBatchLog LBatchLog)
        {
            SqlHelper Sql = new SqlHelper();

            Sql.AddCondition(EntityM_LBatchLog.M_END_TIME, Operator.Equal, DataTypeUtils.String, LBatchLog.FUNCTION_KEY);

            Sql.AddCondition(EntityM_LBatchLog.M_END_TIME, Operator.Equal, DataTypeUtils.String, LBatchLog.FUNCTION_KEY);

            if (BRM_LBatchLog.Search(Sql.GetFilterCondition(), "Connection_CSIP").Count > 0)
            {
                return true;
            }

            return false;
        }
        /// <summary>
        /// Insert Data
        /// </summary>
        /// <param name="strFK">功能標識編號</param>
        /// <param name="strJOBID">JOB編號</param>
        /// <param name="dtST">開始時間</param>
        /// <param name="strStatus">執行狀態(R:執行中F:失敗,S:成功,P:部份成功)</param>
        /// <param name="strRMsg">提示訊息</param>
        /// <returns>是否成功</returns>
        public static bool Insert(string strFK, string strJOBID, DateTime dtST,string strStatus, string strRMsg)
        {
            EntityM_LBatchLog LBatchLog = new EntityM_LBatchLog();

            LBatchLog.FUNCTION_KEY = strFK;
            LBatchLog.JOB_ID = strJOBID;
            LBatchLog.START_TIME = dtST;
            LBatchLog.END_TIME = "1900-01-01 00:00:00.000";
            LBatchLog.STATUS = strStatus;
            LBatchLog.RETURN_MESSAGE = strRMsg;

            try
            {
                return BRM_LBatchLog.AddNewEntity(LBatchLog, "Connection_CSIP");
            }
            catch (Exception ex)
            {
                BRM_LBatchLog.SaveLog(ex);
                return false;
            }
        }

        /// <summary>
        /// Insert Data
        /// </summary>
        /// <param name="strFK">功能標識編號</param>
        /// <param name="strJOBID">JOB編號</param>
        /// <param name="dtST">開始時間</param>
        /// <param name="strStatus">執行狀態(R:執行中F:失敗,S:成功,P:部份成功)</param>
        /// <param name="strRMsg">提示訊息</param>
        /// <returns>是否成功</returns>
        public static bool Insert(string strFK, string strJOBID, DateTime dtST, DateTime dtED, string strStatus, string strRMsg)
        {
            EntityM_LBatchLog LBatchLog = new EntityM_LBatchLog();

            LBatchLog.FUNCTION_KEY = strFK;
            LBatchLog.JOB_ID = strJOBID;
            LBatchLog.START_TIME = dtST;
            LBatchLog.END_TIME = dtED;
            LBatchLog.STATUS = strStatus;
            LBatchLog.RETURN_MESSAGE = strRMsg;

            try
            {
                return BRM_LBatchLog.AddNewEntity(LBatchLog, "Connection_CSIP");
            }
            catch (Exception ex)
            {
                BRM_LBatchLog.SaveLog(ex);
                return false;
            }
        }

        /// 功能說明:檢測JOB是否在執行中 
        /// 作    者:Linda
        /// 創建時間:2010/05/31 
        /// <param name="strFK">功能標識編號</param>
        /// <param name="strJOBID">JOB編號</param>
        /// <param name="dtTimeSt">開始時間</param>
        /// <returns>是否成功</returns>
        public static bool JobStatusChk(string strFK, string strJOBID, DateTime dtTimeSt)
        {
            DataSet dstStatus = null;
            DataTable dtblStatus = null;
            bool bolReturn = false;

            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = SEL_FKEY_FORCHK;
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.Parameters.Add(new SqlParameter("@FUNCTION_KEY", strFK));
            sqlcmd.Parameters.Add(new SqlParameter("@JOB_ID", strJOBID));
            sqlcmd.Parameters.Add(new SqlParameter("@START_TIME", dtTimeSt.ToString("yyyy/MM/dd")));

            try
            {
                dstStatus = SearchOnDataSet(sqlcmd, "Connection_CSIP");
                if (dstStatus == null)
                {
                    bolReturn = false;
                }
                else
                {
                    dtblStatus = dstStatus.Tables[0];
                    if (dtblStatus.Rows.Count > 0)
                    {
                        if (dtblStatus.Rows[0][0].ToString() == "R")
                        {
                            bolReturn = true;
                        }
                    }
                    else
                    {
                        bolReturn = false;
                    }
                }
                return bolReturn;
            }
            catch (Exception exp)
            {
                BRM_LBatchLog.SaveLog(exp);
                return true;//JOB return 不繼續執行
            }
        }
        /// <summary>
        /// Delete Data
        /// </summary>
        /// <param name="strFK">功能標識編號</param>
        /// <param name="strJOBID">JOB編號</param>
        /// <param name="strStatus">執行狀態(R)</param>
        /// <returns>是否成功</returns>
        public static bool Delete(string strFK, string strJOBID, DateTime dtStartTime, string strStatus)
        {
            SqlCommand sqlComm = new SqlCommand();
            sqlComm.CommandText = DEL_RUNNING;
            sqlComm.CommandType = CommandType.Text;

            SqlParameter parmFUNCTION_KEY = new SqlParameter("@FUNCTION_KEY", strFK);
            sqlComm.Parameters.Add(parmFUNCTION_KEY);
            SqlParameter parmJOB_ID = new SqlParameter("@JOB_ID", strJOBID);
            sqlComm.Parameters.Add(parmJOB_ID);
            SqlParameter parmSTART_TIME = new SqlParameter("@START_TIME", dtStartTime);
            sqlComm.Parameters.Add(parmSTART_TIME);
            SqlParameter parmSTATUS = new SqlParameter("@STATUS", strStatus);
            sqlComm.Parameters.Add(parmSTATUS);
            return BRM_LBatchLog.Delete(sqlComm, "Connection_CSIP");
        }
    }
}
