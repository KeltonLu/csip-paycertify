//******************************************************************
//*  作    者：chaoma
//*  功能說明：
//*  創建日期：2009/11/06
//*  修改記錄：調整SP執行，增加 queryType 參數判斷是S：查詢或P：列印，增加reportName取得顯示頁名稱 by Ares Jack 20220207
//*<author>            <time>            <TaskID>                <desc>
//* Ares Stanley    2022/02/14    20210058-CSIP作業服務平台現代化II    調整webconfig取參數方式
//*******************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
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

namespace CSIPPayCertify.BusinessRules
{

    public class BRPay_Certify : BRBase<EntityPay_Certify>
    {

        #region sql語句

        public static string strCSIP = UtilHelper.GetAppSettings("ServerCSIP");

        public static string strPaycheck = UtilHelper.GetAppSettings("ServerPaycheck");


        public static string SEL_PAY_CERTIFY_BY_KEYINDAY = @"SELECT A.[SerialNo], A.[UserID], A.[UserName], A.[MakeUp], A.[GetFee],A.[IsMail],
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
FROM " + strPaycheck + @"..[Pay_Certify] AS A
LEFT OUTER JOIN " + strCSIP + @"..[M_PROPERTY_CODE] AS B 
ON A.[Type] = B.[PROPERTY_CODE] AND B.[FUNCTION_KEY] = '04' AND B.[PROPERTY_KEY] = 'TYPE'
LEFT OUTER JOIN " + strCSIP + @"..[M_PROPERTY_CODE] AS C 
ON A.[CardType] = C.[PROPERTY_CODE] AND C.[FUNCTION_KEY] = '04' AND C.[PROPERTY_KEY] = 'CARDTYPE'
LEFT OUTER JOIN " + strCSIP + @"..[M_PROPERTY_CODE] AS D 
ON A.[Void] = D.[PROPERTY_CODE] AND D.[FUNCTION_KEY] = '04' AND D.[PROPERTY_KEY] = 'VOID'
WHERE (A.[UserID] = @USERID OR @USERID = '') 
";

        public static string SEL_CURRENT_NO = @"SELECT A.[SerialNo], A.[UserID], A.[UserName], A.[MakeUp], A.[GetFee],A.[IsMail],
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
                                                FROM " + strPaycheck + @"..[Pay_Certify] AS A
                                                LEFT OUTER JOIN  " + strCSIP + @"..[M_PROPERTY_CODE] AS B 
                                                ON A.[Type] = B.[PROPERTY_CODE] AND B.[FUNCTION_KEY] = '04' AND B.[PROPERTY_KEY] = 'TYPE'
                                                LEFT OUTER JOIN " + strCSIP + @"..[M_PROPERTY_CODE] AS C 
                                                ON A.[CardType] = C.[PROPERTY_CODE] AND C.[FUNCTION_KEY] = '04' AND C.[PROPERTY_KEY] = 'CARDTYPE'
                                                LEFT OUTER JOIN " + strCSIP + @"..[M_PROPERTY_CODE] AS D 
                                                ON A.[Void] = D.[PROPERTY_CODE] AND D.[FUNCTION_KEY] = '04' AND D.[PROPERTY_KEY] = 'VOID'
                                                WHERE A.[UserID] = @USERID
                                                ";


        private static string SEL_PAY_CERTIFY_DETAIL = @"SELECT  
                                            A.SerialNo, A.UserID, UserName, Zip, Add1, Add2, Add3, MailDay, MailNo, Note, PayName, owe, Pay, PayDay, GetFee, IsMail, EndDate, OpenDay, CloseDay, MakeUp, MakeUpDate, showExtra, Consignee, [Type], Void, KeyinDay, A.CardType, C.ReturnDay, C.ReturnReason, A.mainCard4E, A.MakeUpOther
                                            , CASE ISNULL(B.PROPERTY_CODE,'')
	                                            WHEN '' THEN '4' 
	                                            ELSE A.MailMethod
                                            END AS MailMethod
                                            , ISNULL(B.PROPERTY_CODE,'4') AS MailMethod_Id,A.isFree 
                                            FROM " + strPaycheck + @"..[Pay_Certify] AS A
                                            LEFT OUTER JOIN  " + strCSIP + @"..[M_PROPERTY_CODE] AS B ON A.MailMethod=B.PROPERTY_NAME AND B.[FUNCTION_KEY] = '04' AND B.[PROPERTY_KEY] = 'MAILMETHOD'
                                            LEFT OUTER JOIN " + strPaycheck + @"..[Pay_Return] AS C ON A.SERIALNO = C.SERIALNO 
                                            WHERE A.SerialNo = @SERIALNO";
        #endregion

        /// <summary>
        /// 查詢
        /// </summary>
        /// <param name="ePay_Certify"></param>
        /// <param name="iPageIndex">顯示的頁編號</param>
        /// <param name="iPageSize">每頁大小</param>
        /// <param name="iTotalCount">查詢結果總條數</param>
        /// <param name="strMsgID">返回錯誤ID號</param>
        /// <param name="dtblResult">返回結果集</param>
        /// <param name="queryType">查詢類型S：查詢/P：列印</param>
        /// <param name="reportName">顯示頁名稱</param>
        /// <returns>返回查詢成功失敗標示</returns>
        public static bool SearchResult(EntityPay_Certify ePay_Certify, int iPageIndex, int iPageSize, ref int iTotalCount, ref string strMsgID, ref  DataTable dtblPayCertify, string queryType = "S", string reportName = "")
        {
            try
            {
                string queryTypeName = queryType == "S" ? "查詢" : "列印";
                Stopwatch sw = new Stopwatch();
                string strSearchSql = SEL_CURRENT_NO;
                SqlCommand sqlcmdSearchResult = new SqlCommand();
                sqlcmdSearchResult.CommandType = CommandType.Text;

                SqlParameter parmUserID = new SqlParameter("@USERID", ePay_Certify.userID);
                sqlcmdSearchResult.Parameters.Add(parmUserID);

                if (ePay_Certify.type != "ALL")
                {
                    strSearchSql += " AND A.[TYPE] = @TYPE ORDER BY SUBSTRING([SerialNo], 3, 6) DESC, SUBSTRING([SerialNo], 9,3) DESC ";
                    SqlParameter parmType = new SqlParameter("@TYPE", ePay_Certify.type);
                    sqlcmdSearchResult.Parameters.Add(parmType);
                }
                else
                {
                    strSearchSql += " ORDER BY SUBSTRING([SerialNo], 3, 6) DESC, SUBSTRING([SerialNo], 9,3) DESC ";
                }
                sqlcmdSearchResult.CommandText = strSearchSql;

                //* 查詢已結清記錄
                BRReport.RecordSQL($"{reportName} [{queryTypeName}]", BRReport.GetFullSqlCmd(sqlcmdSearchResult));
                sw.Start();
                DataSet dstSearchResult = BRPay_Certify.SearchOnDataSet(sqlcmdSearchResult,
                               iPageIndex, iPageSize, ref iTotalCount);
                sw.Stop();
                int dataCount = 0;
                
                if (dstSearchResult == null)
                {
                    strMsgID = "04_01010300_003";
                    dataCount = 0;
                    return false;
                }
                else
                {
                    dtblPayCertify = dstSearchResult.Tables[0]; //* 返回取得的正確結果集合
                    dataCount = dtblPayCertify.Rows.Count;
                    strMsgID = "04_01010300_002";
                }
                BRReport.RecordSQLExecuteTime($"{reportName} [{queryTypeName}]", sw, dataCount);
                return true;

            }
            catch (Exception exp)
            {
                dtblPayCertify = null;
                BRPay_Certify.SaveLog(exp);
                strMsgID = "04_01010300_003";
                return false;
            }
        
        }

        /// <summary>
        /// 注銷
        /// </summary>
        /// <param name="eSystem_Log"></param>
        /// <returns></returns>
        public static bool MarkCancel(EntitySystem_log eSystem_Log)
        {

            try
            {
                using (OMTransactionScope ts = new OMTransactionScope())
                {

                    EntityPay_Certify ePay_Certify = new EntityPay_Certify();
                    ePay_Certify.@void = "Y";

                    SqlHelper Sql = new SqlHelper();
                    Sql.AddCondition(EntityPay_Certify.M_serialNo, Operator.Equal, DataTypeUtils.String, eSystem_Log.Serial_No);

                    if (ePay_Certify.DB_UpdateEntityByCondition(Sql.GetFilterCondition(), EntityPay_Certify.M_void))
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
                BRPay_Certify.SaveLog(exp);
                return false;
            }
        }

        /// <summary>
        /// 查詢某ID一年內的資料
        /// </summary>
        /// <param name="strUserID">需要查詢的某UserID</param>
        /// <param name="bInOneYear">是否只要1年內的資料</param>
        /// <param name="strMsgID">出錯的錯誤ID</param>
        /// <param name="esCertify">查詢結果</param>
        /// <returns>True   -   成功;False  -   失敗</returns>
        public static bool GetDetailsByUserID(string strUserID,bool bInOneYear, ref string strMsgID, ref EntitySet<EntityPay_Certify> esCertify)
        {
            try
            {
                string str = DateTime.Now.AddYears(-1).ToString("yyyyMMdd");
                SqlHelper sql = new SqlHelper();
                if (bInOneYear)
                {
                    sql.AddCondition(EntityPay_Certify.M_keyinDay, Operator.GreaterThanEqual, DataTypeUtils.String, DateTime.Now.AddYears(-1).ToString("yyyyMMdd"));
                }
                sql.AddCondition(EntityPay_Certify.M_userID, Operator.Equal, DataTypeUtils.String, strUserID);
                sql.AddOrderCondition(EntityPay_Certify.M_keyinDay, ESortType.DESC);
                esCertify = (EntitySet<EntityPay_Certify>)BRPay_Certify.Search(sql.GetFilterCondition());
            }
            catch (Exception Ex)
            {
                BRPay_Certify.SaveLog(Ex);
                return false;
            }
            return true;
        }


        /// <summary>
        /// 退件
        /// </summary>
        /// <param name="ePayReturn"></param>
        /// <param name="strID">當前登入人員ID</param>
        /// <param name="strName">當前登入人員姓名</param>
        /// <returns></returns>
        public static bool MarkReturn(EntityPay_Return ePayReturn,string strID,string strName)
        {

            try
            {
                using (OMTransactionScope ts = new OMTransactionScope())
                {

                    EntityPay_Certify ePay_Certify = new EntityPay_Certify();
                    ePay_Certify.@void = "R";
                    

                    

                    SqlHelper Sql = new SqlHelper();
                    Sql.AddCondition(EntityPay_Certify.M_serialNo, Operator.Equal, DataTypeUtils.String, ePayReturn.serialNo);

                    EntitySet<EntityPay_Certify> esResult = null;
                    esResult = (EntitySet<EntityPay_Certify>)BRPay_Certify.Search(Sql.GetFilterCondition());

                    string strTmp = esResult.GetEntity(0).note + "(退件日期：" + ePayReturn.returnDay.Trim() + ")";
                    if (strTmp.Length > 50)
                    {
                        ePay_Certify.note = "(退件日期：" + ePayReturn.returnDay.Trim() + ")";
                    }
                    else
                    {
                        ePay_Certify.note = strTmp;
                    }
                    

                        if (ePay_Certify.DB_UpdateEntityByCondition(Sql.GetFilterCondition(), EntityPay_Certify.M_void, EntityPay_Certify.M_note))
                        {

                            if (BRPay_Reutrn.AddNew(ePayReturn))
                            {



                                EntitySystem_log eSystemlog = new EntitySystem_log();

                                eSystemlog.Log_date = DateTime.Now.ToString("yyyyMMdd");
                                eSystemlog.Log_Time = DateTime.Now.Hour.ToString().PadLeft(2, '0') + DateTime.Now.Minute.ToString().PadLeft(2, '0');
                                eSystemlog.CustomerID = ePayReturn.userID;
                                eSystemlog.Log_Action = "退件註記";
                                eSystemlog.Serial_No = ePayReturn.serialNo;
                                eSystemlog.User_Name = strName;
                                eSystemlog.User_ID = strID;
                                eSystemlog.System_Type = "Certify";

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
                BRPay_Certify.SaveLog(exp);
                return false;
            }
        }



        /// <summary>
        /// 查詢
        /// </summary>
        /// <param name="ePay_Certify">查詢條件</param>
        /// <param name="strBeginKeyInDay">開立起始時間</param>
        /// <param name="strEndKeyInDay">開立截至時間</param>
        /// <param name="iPageIndex">顯示的頁編號</param>
        /// <param name="iPageSize">每頁大小</param>
        /// <param name="iTotalCount">查詢結果總條數</param>
        /// <param name="strMsgID">返回錯誤ID號</param>
        /// <param name="dtblResult">返回結果集</param>
        /// <param name="queryType">查詢類型S：查詢/P：列印</param>
        /// <param name="reportName">顯示頁名稱</param>
        /// <returns>返回查詢成功失敗標示</returns>
        public static bool SearchResult(EntityPay_Certify ePay_Certify, string strBeginKeyInDay, string strEndKeyInDay, int iPageIndex, int iPageSize, ref int iTotalCount, ref string strMsgID, ref DataTable dtblResult, string queryType = "S", string reportName = "")
        {
            try
            {
                string queryTypeName = queryType == "S" ? "查詢" : "列印";
                Stopwatch sw = new Stopwatch();
                string strSql = SEL_PAY_CERTIFY_BY_KEYINDAY;
                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;

                SqlParameter parmUserID = new SqlParameter("@USERID", ePay_Certify.userID);
                sqlcmd.Parameters.Add(parmUserID);

                if (strBeginKeyInDay != "" && strEndKeyInDay!="")
                {
                    strSql += " AND  A.[KEYINDAY]>=@BEGINKEYINDAY AND  A.[KEYINDAY]<=@ENDKEYINDAY";
                    SqlParameter parmBeginKeyInDate = new SqlParameter("@BEGINKEYINDAY", strBeginKeyInDay);
                    sqlcmd.Parameters.Add(parmBeginKeyInDate);

                    SqlParameter parmEndKeyInDate = new SqlParameter("@ENDKEYINDAY", strEndKeyInDay);
                    sqlcmd.Parameters.Add(parmEndKeyInDate);
                }
                if (ePay_Certify.type != "ALL")
                {
                    strSql += " AND A.[TYPE] = @TYPE ORDER BY SUBSTRING([SerialNo], 3, 6) , SUBSTRING([SerialNo], 9,3)  ";
                    SqlParameter parmType = new SqlParameter("@TYPE", ePay_Certify.type);
                    sqlcmd.Parameters.Add(parmType);
                }
                else
                {
                    strSql += " ORDER BY SUBSTRING([SerialNo], 3, 6) , SUBSTRING([SerialNo], 9,3)  ";
                }
                sqlcmd.CommandText = strSql;

                //* 查詢清償記錄
                //20220222_Ares_Jack_新增LOG紀錄
                BRReport.RecordSQL($"{reportName} [{queryTypeName}]", BRReport.GetFullSqlCmd(sqlcmd));
                sw.Start();
                DataSet dstSearchResult = BRPay_Certify.SearchOnDataSet(sqlcmd,
                               iPageIndex, iPageSize, ref iTotalCount);
                sw.Stop();

                int dataCount = 0;

                if (dstSearchResult == null)
                {
                    strMsgID = "04_01020300_004";
                    dataCount = 0;
                    return false;
                }
                else
                {
                    dtblResult = dstSearchResult.Tables[0]; //* 返回取得的正確結果集合
                    dataCount = dtblResult.Rows.Count;
                }

                strMsgID = "04_01020300_003";
                BRReport.RecordSQLExecuteTime($"{reportName} [{queryTypeName}]", sw, dataCount);
                return true;

            }
            catch (Exception exp)
            {
                BRPay_Certify.SaveLog(exp);
                strMsgID = "04_01020300_004";
                return false;
            }
        }

        /// <summary>
        /// 清償證明明細綁定
        /// </summary>
        /// <param name="strSerialNo">傳入的序列號</param>
        /// <param name="strMsgID">錯誤信息ID</param>
        /// <param name="dtblResult">查詢出的數據表</param>
        /// <returns></returns>
        /// 
        public static bool GetDetail(string strSerialNo, ref string strMsgID, ref DataTable dtblResult)
        {
            try
            {
                string strSearchSql = SEL_PAY_CERTIFY_DETAIL;
                SqlCommand sqlcmdSearchResult = new SqlCommand();
                sqlcmdSearchResult.CommandType = CommandType.Text;
                SqlParameter paramSerialNo = new SqlParameter("@SERIALNO", strSerialNo);
                sqlcmdSearchResult.Parameters.Add(paramSerialNo);
                sqlcmdSearchResult.CommandText = strSearchSql;

                DataSet dsSearchResult = BRPay_Certify.SearchOnDataSet(sqlcmdSearchResult);
                if (dsSearchResult == null)
                {
                    strMsgID = "00_00000000_000";
                    return false;
                }
                else
                {
                    dtblResult = dsSearchResult.Tables[0]; //* 返回取得的正確結果集合
                }
                strMsgID = "04_01020400_003";
                return true;
            }
            catch (Exception exp)
            {
                BRPay_Certify.SaveLog(exp);
                strMsgID = "04_01020400_005";
                dtblResult = null;
                return false;
            }
        }
    }
}
