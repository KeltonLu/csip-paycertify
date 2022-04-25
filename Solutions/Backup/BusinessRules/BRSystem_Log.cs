//******************************************************************
//*  作    者：余洋
//*  功能說明：修改結清，代償證明
//*  創建日期：2009/11/03
//*  修改記錄：

//*<author>            <time>            <TaskID>                <desc>
//*******************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using Framework.Data.OM;
using Framework.Data.OM.Collections;
using Framework.Data.OM.Transaction;
using CSIPCommonModel.BaseItem;
using CSIPCommonModel.EntityLayer;
using CSIPCommonModel.BusinessRules;
using CSIPPayCertify.EntityLayer;

namespace CSIPPayCertify.BusinessRules
{
    public class BRSystem_Log : CSIPCommonModel.BusinessRules.BRBase<EntitySystem_log>
    {
        /// <summary>
        /// 記錄查詢證明
        /// </summary>
        /// <param name="strCust_ID">客戶ID</param>
        /// <param name="strUserID">UserID</param>
        /// <param name="strLog_Start_Date">異動日期(起)</param>
        /// <param name="strLog_End_Date">異動日期(迄)</param>
        /// <param name="strSystemType">系統類型</param>
        /// <param name="strError_ID">錯誤信息ID</param>
        /// <param name="EntitySetResult">查出的數據集合</param>
        /// <param name="intPageIndex">頁碼數</param>
        /// <param name="intPageSize">每頁筆數</param>
        /// <returns></returns>
        public static bool SearchResult(string strCust_ID, string strUserID, string strLog_Start_Date, string strLog_End_Date,
                                        string strSystemType, ref string strError_ID, ref EntitySet<EntitySystem_log> EntitySetResult,
                                        int intPageIndex, int intPageSize)
        {
            try
            {
                SqlHelper objSqlHelper = new SqlHelper();

                if (strCust_ID != "")
                {
                    //*如果有輸入,添加查詢條件CustomerID等于strRole_ID
                    objSqlHelper.AddCondition(EntitySystem_log.M_CustomerID, Operator.Equal, DataTypeUtils.String, strCust_ID);
                }
                if (strUserID != "")
                {
                    //*如果有輸入,添加查詢條件User_ID等于strUserID
                    objSqlHelper.AddCondition(EntitySystem_log.M_User_ID, Operator.Equal, DataTypeUtils.String, strUserID);
                }
                //如果異動日期有一欄輸入,則執行下列條件
                if (strLog_Start_Date != "")
                {
                     //*添加查詢條件Log_Date>=strLog_Start_Date
                    objSqlHelper.AddCondition(EntitySystem_log.M_Log_date, Operator.GreaterThanEqual, DataTypeUtils.String, strLog_Start_Date);
                }
                if(strLog_End_Date != "")
                {
                    //*添加查詢條件Log_Date<=strLog_End_Date
                    objSqlHelper.AddCondition(EntitySystem_log.M_Log_date, Operator.LessThanEqual, DataTypeUtils.String, strLog_End_Date);
                }
                
                if (strSystemType != "")
                {
                    //*添加查詢條件SystemType = strSystemType
                    objSqlHelper.AddCondition(EntitySystem_log.M_System_Type, Operator.Equal, DataTypeUtils.String, strSystemType);
                }
                objSqlHelper.AddOrderCondition(EntitySystem_log.M_CustomerID, ESortType.DESC);
                objSqlHelper.AddOrderCondition(EntitySystem_log.M_Serial_No, ESortType.DESC);
                objSqlHelper.AddOrderCondition(EntitySystem_log.M_Log_date, ESortType.DESC);
                EntitySetResult = (EntitySet<EntitySystem_log>)BRSystem_Log.Search(objSqlHelper.GetFilterCondition(), intPageIndex, intPageSize);
                return true;
            }
            catch (Exception exp)
            {
                BRSystem_Log.SaveLog(exp);
                strError_ID = "04_01020500_006";
                return false;
            }
        }

        /// <summary>
        /// 添加LOG檔
        /// </summary>
        /// <param name="eSystemLog"></param>
        /// <returns></returns>
        public static bool AddNew(EntitySystem_log eSystemLog)
        {

            if (BRSystem_Log.AddNewEntity(eSystemLog))
            {
                return true;
            }
            else
            {
                return false;
            }

        }
    }

    
}
        

    

