//******************************************************************
//*  作    者：余洋(rosicky)
//*  功能說明：
//*  創建日期：2009/10/27
//*  修改記錄：調整SP執行，增加 queryType 參數判斷是S：查詢或P：列印，增加reportName取得顯示頁名稱 by Ares Jack 20220207
//*<author>            <time>            <TaskID>                <desc>
//* Ares Stanley    2022/02/14    20210058-CSIP作業服務平台現代化II    調整webconfig取參數方式
//*******************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using CSIPCommonModel.BusinessRules;
using CSIPPayCertify.EntityLayer;
using CSIPCommonModel.BaseItem;
using Framework.Data.OM;
using Framework.Data.OM.Transaction;
using System.Data.SqlClient;
using System.Data;
using Framework.Data.OM.Collections;
using System.Configuration;
using Framework.Common.Utility;
using System.Diagnostics;

namespace CSIPPayCertify.BusinessRules
{
    public class BRPay_SV : BRBase<EntityPay_SV>
    {
        #region sql語句

        public static string strCSIP = UtilHelper.GetAppSettings("ServerCSIP");

        public static string strPaycheck = UtilHelper.GetAppSettings("ServerPaycheck");

        public static string SEL_Pay_SV = @"SELECT A.[SerialNo], A.[UserID], A.[UserName], A.[MakeUp], A.[GetFee],A.[IsMail],
                                                CASE ISNULL(A.[Type],'')
	                                                WHEN '' THEN '' 
	                                                ELSE ISNULL(A.[Type],'')+':'+ISNULL(B.[PROPERTY_NAME],'') 
                                                END AS [Type], 
                                                CASE ISNULL(A.[CardType],'')
	                                                WHEN '' THEN '' 
	                                                ELSE ISNULL(A.[CardType],'')+':'+ISNULL(C.[PROPERTY_NAME],'')
                                                END AS [CARDTYPE], 
                                                CASE ISNULL(A.[Void],'')
	                                                WHEN '' THEN '' 
	                                                ELSE ISNULL(A.[Void],'')+':'+ISNULL(D.[PROPERTY_NAME],'') 
                                                END AS [VOID]
                                                , A.[KeyinDay] 
                                                FROM " + strPaycheck + @"..[Pay_SV] AS A
                                                LEFT OUTER JOIN " + strCSIP + @"..[M_PROPERTY_CODE] AS B 
                                                ON A.[Type] = B.[PROPERTY_CODE] AND B.[FUNCTION_KEY] = '04' AND B.[PROPERTY_KEY] = 'TYPE'
                                                LEFT OUTER JOIN " + strCSIP + @"..[M_PROPERTY_CODE] AS C 
                                                ON A.[CardType] = C.[PROPERTY_CODE] AND C.[FUNCTION_KEY] = '04' AND C.[PROPERTY_KEY] = 'CARDTYPE'
                                                LEFT OUTER JOIN " + strCSIP + @"..[M_PROPERTY_CODE] AS D 
                                                ON A.[Void] = D.[PROPERTY_CODE] AND D.[FUNCTION_KEY] = '04' AND D.[PROPERTY_KEY] = 'VOID'
                                                WHERE A.[UserID] = @USERID
                                                ";
        public static string SEL_PAY_SV_DETAIL = @"SELECT A.SerialNo, A.UserID, UserName, Zip,Add1, Add2, Add3, MailDay, 
                                          MailNo, Note, GetFee, IsMail, EndDate, MakeUp, MakeUpDate, showExtra, 
                                          Consignee, [Type], Void, KeyinDay, A.CardType, C.ReturnDay, C.ReturnReason, 
                                          CASE ISNULL(B.PROPERTY_CODE,'') WHEN '' THEN '4' ELSE A.MailMethod END AS MailMethod, 
                                          ISNULL(B.PROPERTY_CODE,'4') AS MailMethod_Id 
                                          FROM " + strPaycheck + @"..[Pay_SV] AS A 
                                               LEFT OUTER JOIN  " + strCSIP + @"..[M_PROPERTY_CODE] AS B 
                                                    ON A.MailMethod=B.PROPERTY_NAME AND B.[FUNCTION_KEY] = '04' AND B.[PROPERTY_KEY] = 'MAILMETHOD' 
                                               LEFT OUTER JOIN " + strPaycheck + @"..[Pay_ReturnSV] AS C 
                                                    ON A.SERIALNO = C.SERIALNO 
                                          WHERE A.SerialNo = @SERIALNO";
        private static string SEL_Pay_SV_BY_KEYINDATE = @"SELECT A.[SerialNo], A.[UserID], A.[UserName], A.[MakeUp], A.[GetFee],A.[IsMail],
CASE ISNULL(A.[Type],'')
	WHEN '' THEN '' 
	ELSE ISNULL(A.[Type],'')+':'+ISNULL(B.[PROPERTY_NAME],'') 
END AS [Type], 
CASE ISNULL(A.[CardType],'')
	WHEN '' THEN '' 
	ELSE ISNULL(A.[CardType],'')+':'+ISNULL(C.[PROPERTY_NAME],'')
END AS [CARDTYPE], 
CASE ISNULL(A.[Void],'')
	WHEN '' THEN '' 
	ELSE ISNULL(A.[Void],'')+':'+ISNULL(D.[PROPERTY_NAME],'') 
END AS [VOID]
, A.[KeyinDay] 
FROM " + strPaycheck + @"..[Pay_SV] AS A
LEFT OUTER JOIN " + strCSIP + @"..[M_PROPERTY_CODE] AS B 
ON A.[Type] = B.[PROPERTY_CODE] AND B.[FUNCTION_KEY] = '04' AND B.[PROPERTY_KEY] = 'TYPE'
LEFT OUTER JOIN " + strCSIP + @"..[M_PROPERTY_CODE] AS C 
ON A.[CardType] = C.[PROPERTY_CODE] AND C.[FUNCTION_KEY] = '04' AND C.[PROPERTY_KEY] = 'CARDTYPE'
LEFT OUTER JOIN " + strCSIP + @"..[M_PROPERTY_CODE] AS D 
ON A.[Void] = D.[PROPERTY_CODE] AND D.[FUNCTION_KEY] = '04' AND D.[PROPERTY_KEY] = 'VOID'
WHERE  ISNULL(KEYINDAY,'') <> '' 
AND A.keyinDay>=@STARTDATE AND A.keyinday<=@ENDDATE
";

        private static string SEL_Pay_SV_BY_KEYINDATE_NULL = @"SELECT A.[SerialNo], A.[UserID], A.[UserName], A.[MakeUp], A.[GetFee],A.[IsMail],
CASE ISNULL(A.[Type],'')
	WHEN '' THEN '' 
	ELSE ISNULL(A.[Type],'')+':'+ISNULL(B.[PROPERTY_NAME],'') 
END AS [Type], 
CASE ISNULL(A.[CardType],'')
	WHEN '' THEN '' 
	ELSE ISNULL(A.[CardType],'')+':'+ISNULL(C.[PROPERTY_NAME],'')
END AS [CARDTYPE], 
CASE ISNULL(A.[Void],'')
	WHEN '' THEN '' 
	ELSE ISNULL(A.[Void],'')+':'+ISNULL(D.[PROPERTY_NAME],'') 
END AS [VOID]
, A.[KeyinDay] 
FROM " + strPaycheck + @"..[Pay_SV] AS A
LEFT OUTER JOIN " + strCSIP + @"..[M_PROPERTY_CODE] AS B 
ON A.[Type] = B.[PROPERTY_CODE] AND B.[FUNCTION_KEY] = '04' AND B.[PROPERTY_KEY] = 'TYPE'
LEFT OUTER JOIN " + strCSIP + @"..[M_PROPERTY_CODE] AS C 
ON A.[CardType] = C.[PROPERTY_CODE] AND C.[FUNCTION_KEY] = '04' AND C.[PROPERTY_KEY] = 'CARDTYPE'
LEFT OUTER JOIN " + strCSIP + @"..[M_PROPERTY_CODE] AS D 
ON A.[Void] = D.[PROPERTY_CODE] AND D.[FUNCTION_KEY] = '04' AND D.[PROPERTY_KEY] = 'VOID'
WHERE  ISNULL(KEYINDAY,'') = '' 
AND A.keyinDay>=@STARTDATE AND A.keyinday<=@ENDDATE
";
        #endregion

        /// <summary>
        /// 查詢
        /// </summary>
        /// <param name="eyPay_Certify"></param>
        /// <param name="iPageIndex">顯示的頁編號</param>
        /// <param name="iPageSize">每頁大小</param>
        /// <param name="iTotalCount">查詢結果總條數</param>
        /// <param name="strMsgID">返回錯誤ID號</param>
        /// <param name="dtblResult">返回結果集</param>
        /// <param name="queryType">查詢類型S：查詢/P：列印</param>
        /// <param name="reportName">顯示頁名稱</param>
        /// <returns>返回查詢成功失敗標示</returns>
        public static bool SearchResult(EntityPay_SV eyPay_SV, int iPageIndex, int iPageSize, ref int iTotalCount, ref string strMsgID, ref DataTable dtblPay_SV, string queryType = "S", string reportName = "")
        {
            try
            {
                string queryTypeName = queryType == "S" ? "查詢" : "列印";
                Stopwatch sw = new Stopwatch();
                string strSearchSql = SEL_Pay_SV;
                SqlCommand sqlcmdSearchResult = new SqlCommand();
                sqlcmdSearchResult.CommandType = CommandType.Text;

                SqlParameter parmUserID = new SqlParameter("@USERID", eyPay_SV.UserID);
                sqlcmdSearchResult.Parameters.Add(parmUserID);

                if (eyPay_SV.Type != "ALL")
                {
                    strSearchSql += " AND A.[TYPE] = @TYPE ORDER BY [SerialNo] ASC ";
                    SqlParameter parmType = new SqlParameter("@TYPE", eyPay_SV.Type);
                    sqlcmdSearchResult.Parameters.Add(parmType);
                }
                else
                {
                    strSearchSql += " ORDER BY [SerialNo] ASC ";
                }
                sqlcmdSearchResult.CommandText = strSearchSql;

                //* 查詢已結清記錄

                //20220222_Ares_Jack_新增LOG紀錄
                BRReport.RecordSQL($"{reportName} [{queryTypeName}]", BRReport.GetFullSqlCmd(sqlcmdSearchResult));
                sw.Start();
                DataSet dstSearchResult = BRPay_SV.SearchOnDataSet(sqlcmdSearchResult,
                               iPageIndex, iPageSize, ref iTotalCount);
                sw.Stop();
                int dataCount = 0;
                if (dstSearchResult == null)
                {
                    strMsgID = "04_02010300_003";
                    dataCount = 0;
                    return false;
                }
                else
                {
                    dtblPay_SV = dstSearchResult.Tables[0]; //* 返回取得的正確結果集合
                    dataCount = dtblPay_SV.Rows.Count;

                }

                strMsgID = "04_02010300_002";
                BRReport.RecordSQLExecuteTime($"{reportName} [{queryTypeName}]", sw, dataCount);
                return true;

            }
            catch (Exception exp)
            {
                BRPay_SV.SaveLog(exp);
                strMsgID = "04_02010300_003";
                return false;
            }
        }

        /// <summary>
        /// 查詢資料庫Pay_SV
        /// </summary>
        /// <param name="strSerialNo">序號</param>
        /// <param name="strMsg">消息</param>
        /// <param name="dtblResult">查詢到的結果集存放在DataTable中</param>
        /// <returns></returns>
        public static bool GetDetail(string strSerialNo, ref string strMsg, ref DataTable dtblPay_SV)
        {
            try
            {
                string strSearchSql = SEL_PAY_SV_DETAIL;
                SqlCommand sqlcmdSearchResult = new SqlCommand();
                sqlcmdSearchResult.CommandType = CommandType.Text;

                SqlParameter parmUserID = new SqlParameter("@SERIALNO", strSerialNo);
                sqlcmdSearchResult.Parameters.Add(parmUserID);
                sqlcmdSearchResult.CommandText = strSearchSql;
                //int iTotalCount = 1;

                //* 查詢客戶資料
                DataSet dstSearchResult = BRPay_SV.SearchOnDataSet(sqlcmdSearchResult);
                if (dstSearchResult == null)
                {
                    strMsg = "04_01020500_006";
                    return false;
                }
                else
                {
                    dtblPay_SV = dstSearchResult.Tables[0]; //* 返回取得的正確結果集合
                }
                return true;
            }
            catch (Exception exp)
            {
                BRPay_SV.SaveLog(exp);
                strMsg = "04_01020500_006";
                return false;
            }

        }

        /// <summary>
        /// 註銷
        /// </summary>
        /// <param name="eSystem_Log"></param>
        /// <returns></returns>
        public static bool MarkCancel(EntitySystem_log eSystem_Log)
        {

            try
            {
                using (OMTransactionScope ts = new OMTransactionScope())
                {

                    EntityPay_SV ePay_SV = new EntityPay_SV();
                    ePay_SV.@void = "Y";

                    SqlHelper Sql = new SqlHelper();
                    Sql.AddCondition(EntityPay_SV.M_SerialNo, Operator.Equal, DataTypeUtils.String, eSystem_Log.Serial_No);


                    EntitySet<EntityPay_SV> esResult = null;
                    esResult = (EntitySet<EntityPay_SV>)BRPay_SV.Search(Sql.GetFilterCondition());

                    string strTmp = esResult.GetEntity(0).Note + "(註銷日期：" + DateTime.Today.ToString("yyyyMMdd") + ")";
                    if (strTmp.Length > 50)
                    {
                        ePay_SV.Note = "(註銷日期：" + DateTime.Today.ToString("yyyyMMdd") + ")";
                    }
                    else
                    {
                        ePay_SV.Note = strTmp;
                    }




                    if (ePay_SV.DB_UpdateEntityByCondition(Sql.GetFilterCondition(), EntityPay_SV.M_void, EntityPay_SV.M_Note))
                    {

                        if (BRSystem_Log.AddNew(eSystem_Log))
                        {
                            ts.Complete();
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

            }
            catch (Exception exp)
            {
                BRPay_SV.SaveLog(exp);
                return false;
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="etPaySv"></param>
        /// <param name="strCondition"></param>
        /// <returns></returns>
        public static bool UpdatePay(EntityPay_SV etPaySv, string strCondition)
        {

            if (!BRPay_SV.UpdateEntityByCondition(etPaySv, strCondition, EntityPay_SV.M_UserName
                , EntityPay_SV.M_Zip, EntityPay_SV.M_Add1, EntityPay_SV.M_Add2, EntityPay_SV.M_Add3, EntityPay_SV.M_MailDay
                , EntityPay_SV.M_MailNo, EntityPay_SV.M_Note, EntityPay_SV.M_Consignee, EntityPay_SV.M_IsMail
                , EntityPay_SV.M_GetFee, EntityPay_SV.M_void, EntityPay_SV.M_ShowExtra, EntityPay_SV.M_Enddate
                , EntityPay_SV.M_MakeUp, EntityPay_SV.M_MakeUpDate, EntityPay_SV.M_MailMethod))
            {
                return false;
            }
            return true;


        }


        /// <summary>
        /// 退件
        /// </summary>
        /// <param name="eyPayReturn"></param>
        /// <param name="strID">當前登入人員ID</param>
        /// <param name="strName">當前登入人員姓名</param>
        /// <returns></returns>
        public static bool MarkReturn(EntityPay_ReturnSV eyPayReturnSV, string strID, string strName)
        {

            try
            {
                using (OMTransactionScope ts = new OMTransactionScope())
                {

                    EntityPay_SV ePay_SV = new EntityPay_SV();
                    ePay_SV.@void = "R";
                    
                    SqlHelper Sql = new SqlHelper();
                    Sql.AddCondition(EntityPay_SV.M_SerialNo, Operator.Equal, DataTypeUtils.String, eyPayReturnSV.serialNo);

                    EntitySet<EntityPay_SV> esResult = null;
                    esResult = (EntitySet<EntityPay_SV>)BRPay_SV.Search(Sql.GetFilterCondition());

                    string strTmp = esResult.GetEntity(0).Note + "(退件日期：" + eyPayReturnSV.returnDay.Trim() + ")";
                    if (strTmp.Length > 50)
                    {
                        ePay_SV.Note = "(退件日期：" + eyPayReturnSV.returnDay.Trim() + ")";
                    }
                    else
                    {
                        ePay_SV.Note = strTmp;
                    }


                    if (ePay_SV.DB_UpdateEntityByCondition(Sql.GetFilterCondition(), EntityPay_SV.M_void, EntityPay_SV.M_Note))
                    {

                        if (BRPay_ReutrnSV.AddNew(eyPayReturnSV))
                        {



                            EntitySystem_log eSystemlog = new EntitySystem_log();

                            eSystemlog.Log_date = DateTime.Now.ToString("yyyyMMdd");
                            eSystemlog.Log_Time = DateTime.Now.Hour.ToString().PadLeft(2, '0') + DateTime.Now.Minute.ToString().PadLeft(2, '0');
                            eSystemlog.CustomerID = eyPayReturnSV.userID;
                            eSystemlog.Log_Action = "退件註記";
                            eSystemlog.Serial_No = eyPayReturnSV.serialNo;
                            eSystemlog.User_Name = strName;
                            eSystemlog.User_ID = strID;
                            eSystemlog.System_Type = "SV";

                            if (BRSystem_Log.AddNew(eSystemlog))
                            {
                                ts.Complete();
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
                    else
                    {
                        return false;
                    }


                }

            }
            catch (Exception exp)
            {
                BRPay_SV.SaveLog(exp);
                return false;
            }
        }

        /// <summary>
        /// 債清證明窗體信息綁定
        /// </summary>
        /// <param name="strUserID">傳入用戶名</param>
        /// <param name="blIsKeyIn">是否開立</param>
        /// <param name="strBeforeDate">開立日期起</param>
        /// <param name="strEndDate">開立日期迄</param
        /// <param name="intPageIndex">頁面編號</param>
        /// <param name="intPageSize">頁面大小</param>
        /// <param name="intTotalCount">總筆數</param>
        /// <param name="strMsgID">錯誤信息ID</param>
        /// <param name="dtblResult">查詢出的數據表</param>
        /// <param name="queryType">查詢類型S：查詢/P：列印</param>
        /// <param name="reportName">顯示頁名稱</param>
        /// <returns></returns>
        public static bool SearchResult(string strUserID, bool blIsKeyIn, string strBeforeDate, string strEndDate, int intPageIndex, int intPageSize, ref int intTotalCount, ref string strMsgID, ref  DataTable dtblResult, string queryType = "S", string reportName = "")
        {
            try
            {
                string queryTypeName = queryType == "S" ? "查詢" : "列印";
                Stopwatch sw = new Stopwatch();
                SqlCommand sqlcmdSearchResult = new SqlCommand();
                sqlcmdSearchResult.CommandType = CommandType.Text;
                string strSearchSql = "";
                if (blIsKeyIn)
                {
                    strSearchSql = SEL_Pay_SV_BY_KEYINDATE;
                }
                else
                {
                    strSearchSql = SEL_Pay_SV_BY_KEYINDATE_NULL;
                }

                if (strUserID != "")
                {
                    strSearchSql += " AND UserID = @UserID  ORDER BY [SerialNo] ASC ";
                    SqlParameter parmType = new SqlParameter("@UserID", strUserID);
                    sqlcmdSearchResult.Parameters.Add(parmType);
                }
                else
                {
                    strSearchSql += " ORDER BY [SerialNo] ASC ";
                }

                if (strBeforeDate == "")
                {
                    strBeforeDate = "00000000";
                }

                if (strEndDate == "")
                {
                    strEndDate = "99999999";
                }

                SqlParameter paramBeforeDate = new SqlParameter("@STARTDATE", strBeforeDate);
                sqlcmdSearchResult.Parameters.Add(paramBeforeDate);

                SqlParameter paramEndDate = new SqlParameter("@ENDDATE", strEndDate);
                sqlcmdSearchResult.Parameters.Add(paramEndDate);

                sqlcmdSearchResult.CommandText = strSearchSql;

                //20220222_Ares_Jack_新增LOG紀錄
                BRReport.RecordSQL($"{reportName} [{queryTypeName}]", BRReport.GetFullSqlCmd(sqlcmdSearchResult));
                sw.Start();
                DataSet dstSearchResult = BRPay_Certify.SearchOnDataSet(sqlcmdSearchResult,
                                intPageIndex, intPageSize, ref intTotalCount);
                sw.Stop();
                int dataCount = 0;
                if (dstSearchResult == null)
                {
                    strMsgID = "00_00000000_000";
                    dataCount = 0;
                    return false;
                }
                else
                {
                    dtblResult = dstSearchResult.Tables[0]; //* 返回取得的正確結果集合
                    dataCount = dtblResult.Rows.Count;
                }
                strMsgID = "04_02020300_003";
                BRReport.RecordSQLExecuteTime($"{reportName} [{queryTypeName}]", sw, dataCount);
                return true;
            }
            catch (Exception exp)
            {
                BRPay_SV.SaveLog(exp);
                strMsgID = "04_02020400_004";
                dtblResult = null;
                return false;
            }
        }
    }
}
