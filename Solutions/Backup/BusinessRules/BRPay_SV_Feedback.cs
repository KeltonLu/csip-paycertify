//******************************************************************
//*  作    者：余洋
//*  功能說明：

//*  創建日期：2009/11/09
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
    public class BRPay_SV_Feedback : BRBase<EntityPay_SV_Feedback>
    {
        #region SQL
        /// <summary>
        /// 剔退記錄查詢
        /// </summary>
        private const string SEL_PAY_SV_FEEDBACK = @"SELECT UserID, UserName, ApplyDate, Count(1) AS CardCount, RebackReason FROM PAY_SV_FEEDBACK WHERE  1=1 ";
        #endregion SQL

        /// <summary>
        /// 剔退記錄查詢綁定GridView
        /// </summary>
        /// <param name="strRebackDateStart">傳入剔退日期(起)</param>
        /// <param name="strRebackDateEnd">傳入剔退日期(迄)</param>
        /// <param name="strApplyDateStart">傳入申請日期(起)</param>
        /// <param name="strApplyDateEnd">傳入申請日期(迄)</param>
        /// <param name="strUserID">傳入用戶ID</param>
        /// <param name="intPageIndex">頁面頁數</param>
        /// <param name="intPageSize">頁面大小</param>
        /// <param name="strMsgID">錯誤信息ID</param>
        /// <param name="dtblResult">查詢出的數據表</param>
        /// <param name="intTotalCount">總筆數</param>
        /// <returns>true 查詢成功;false 查詢失敗</returns>
        public static bool FeedBackQuery(string strRebackDateStart, string strRebackDateEnd, string strApplyDateStart, string strApplyDateEnd,
                                         string strUserID, int intPageIndex, int intPageSize, ref string strMsgID, ref DataTable dtblResult, ref int intTotalCount)
        {
            try
            {
                string strSearchSql = SEL_PAY_SV_FEEDBACK;
                SqlCommand sqlcmdSearchResult = new SqlCommand();
                sqlcmdSearchResult.CommandType = CommandType.Text;
                if (strUserID != "")
                {
                    strSearchSql += " AND UserID LIKE @UserID + '%'";
                    SqlParameter paramUserID = new SqlParameter("@UserID", strUserID);
                    sqlcmdSearchResult.Parameters.Add(paramUserID);
                }
                if (strRebackDateStart != "")
                {
                    strSearchSql += " AND rebackDate >= @RebackDateStart  ";
                    SqlParameter paramRebackDateStart = new SqlParameter("@RebackDateStart", strRebackDateStart);
                    sqlcmdSearchResult.Parameters.Add(paramRebackDateStart);
                }
                if (strRebackDateEnd != "")
                {
                    strSearchSql += " AND rebackDate <= @RebackDateEnd";
                    SqlParameter paramRebackDateEnd = new SqlParameter("@RebackDateEnd", strRebackDateEnd);
                    sqlcmdSearchResult.Parameters.Add(paramRebackDateEnd);
                }
                if (strApplyDateStart != "")
                {
                    strSearchSql += " AND ApplyDate >= @ApplyDateStart ";
                    SqlParameter paramAppDateStart = new SqlParameter("@ApplyDateStart", strApplyDateStart);
                    sqlcmdSearchResult.Parameters.Add(paramAppDateStart);
                }
                if (strApplyDateEnd != "")
                {
                    strSearchSql += "  AND ApplyDate <= @ApplyDateEnd";
                    SqlParameter paramAppDateEnd = new SqlParameter("@ApplyDateEnd", strApplyDateEnd);
                    sqlcmdSearchResult.Parameters.Add(paramAppDateEnd);
                }
                strSearchSql += " GROUP BY UserID,UserName, ApplyDate, rebackDate, rebackReason";
                sqlcmdSearchResult.CommandText = strSearchSql;

                DataSet dstSearchResult = BRPay_SV_Feedback.SearchOnDataSet(sqlcmdSearchResult,
                               intPageIndex, intPageSize, ref intTotalCount);
                if (dstSearchResult == null)
                {
                    strMsgID = "00_00000000_000";
                    return false;
                }
                else
                {
                    dtblResult = dstSearchResult.Tables[0]; //* 返回取得的正確結果集合
                }

                return true;
            }
            catch (Exception exp)
            {
                BRPay_SV_Feedback.SaveLog(exp);
                strMsgID = "";
                return false;
            }
        }
        /// <summary>
        /// 剔退記錄查詢綁定報表
        /// </summary>
        /// <param name="strRebackDateStart">傳入剔退日期(起)</param>
        /// <param name="strRebackDateEnd">傳入剔退日期(迄)</param>
        /// <param name="strApplyDateStart">傳入申請日期(起)</param>
        /// <param name="strApplyDateEnd">傳入申請日期(迄)</param>
        /// <param name="strUserID">傳入用戶ID</param>
        /// <param name="strMsgID">錯誤信息ID</param>
        /// <param name="dtblresult">查詢出的數據表</param>
        /// <returns>true 查詢成功;false 查詢失敗</returns>
        public static bool FeedBackQuery(string strRebackDateStart, string strRebackDateEnd, string strApplyDateStart, string strApplyDateEnd,
                                         string strUserID, ref string strMsgID, ref DataTable dtblresult)
        {
            try
            {
                string strSearchSql = SEL_PAY_SV_FEEDBACK;
                SqlCommand sqlcmdSearchResult = new SqlCommand();
                sqlcmdSearchResult.CommandType = CommandType.Text;
                if (strUserID != "")
                {
                    strSearchSql += " AND UserID LIKE @UserID + '%'";
                    SqlParameter paramUserID = new SqlParameter("@USERID", strUserID);
                    sqlcmdSearchResult.Parameters.Add(paramUserID);
                }
                if (strRebackDateStart != "")
                {
                    strSearchSql += " AND rebackDate >= @RebackDateStart  ";
                    SqlParameter paramRebackDateStart = new SqlParameter("@RebackDateStart", strRebackDateStart);
                    sqlcmdSearchResult.Parameters.Add(paramRebackDateStart);
                }
                if (strRebackDateEnd != "")
                {
                    strSearchSql += " AND rebackDate <= @RebackDateEnd";
                    SqlParameter paramRebackDateEnd = new SqlParameter("@RebackDateEnd", strRebackDateEnd);
                    sqlcmdSearchResult.Parameters.Add(paramRebackDateEnd);
                }

                if (strApplyDateStart != "")
                {
                    strSearchSql += " AND ApplyDate >= @ApplyDateStart ";
                    SqlParameter paramAppDateStart = new SqlParameter("@ApplyDateStart", strApplyDateStart);
                    sqlcmdSearchResult.Parameters.Add(paramAppDateStart);
                }
                if (strApplyDateEnd != "")
                {
                    strSearchSql += "  AND ApplyDate <= @ApplyDateEnd";
                    SqlParameter paramAppDateEnd = new SqlParameter("@ApplyDateEnd", strApplyDateEnd);
                    sqlcmdSearchResult.Parameters.Add(paramAppDateEnd);
                }
                strSearchSql += " GROUP BY UserID,UserName, ApplyDate, rebackDate, rebackReason";
                sqlcmdSearchResult.CommandText = strSearchSql;

                DataSet dstSearchResult = BRPay_SV_Feedback.SearchOnDataSet(sqlcmdSearchResult);

                if (dstSearchResult == null)
                {
                    strMsgID = "00_00000000_000";
                    return false;
                }
                else
                {
                    dtblresult = dstSearchResult.Tables[0]; //* 返回取得的正確結果集合
                }

                return true;
            }
            catch (Exception exp)
            {
                BRPay_SV_Feedback.SaveLog(exp);
                strMsgID = "04_02020600_008";
                return false;
            }
        }
    }
}
