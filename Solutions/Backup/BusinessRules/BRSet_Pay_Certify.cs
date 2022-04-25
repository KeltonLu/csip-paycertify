//******************************************************************
//*  作    者：占偉林(WeilinZhan)
//*  功能說明：查詢證明
//*  創建日期：2009/10/03
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
//*******************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Framework.Data.OM;
using Framework.Data.OM.Collections;
using Framework.Data.OM.Transaction;
using Framework.Common.HTG;
using Framework.Common.Logging;
using CSIPCommonModel.BaseItem;
using CSIPCommonModel.EntityLayer;
using CSIPCommonModel.BusinessRules;
using CSIPPayCertify.EntityLayer;
using Framework.Common.Message;

namespace CSIPPayCertify.BusinessRules
{
    public class BRSet_Pay_Certify : CSIPCommonModel.BusinessRules.BRBase<EntitySet_Pay_Certify>
    {
        #region SQL
        /// <summary>
        /// 查詢已開立結清證明的SQL
        /// </summary>        
        private static string SEL_PAY_CERTIFY_03020100 = @" SELECT DISTINCT " + 
                                                           "     A.CardNo " + 
                                                           "     ,A.LoanDate " + 
                                                           "     ,A.PayOffAmt " + 
                                                           "     ,A.PayOffDate " + 
                                                           "     ,A.mailDay " + 
                                                           "     ,A.isMail " + 
                                                           "     ,A.MailType " + 
                                                           "     ,A.serialNo " + 
                                                           "     ,A.voidDay " +
                                                           "     ,ISNULL( B.[User_Name] , A.KeyInUser )  as User_Name" + 
                                                           "     ,A.Back_Date " + 
                                                           "     ,A.BackNote " + 
                                                           "     ,A.UserName " + 
                                                           "     ,A.UserID " + 
                                                           " FROM " +
                                                           "     " + ConfigurationManager.AppSettings["ServerPaycheck"] + @"..[Set_Pay_Certify] AS A " + 
                                                           "     LEFT OUTER JOIN " +
                                                           "     " + ConfigurationManager.AppSettings["ServerCSIP"] + @"..[M_USER] AS B " + 
                                                           "     ON A.KeyInUser = B.USER_ID " + 
                                                           " WHERE " +
                                                           "          A.Type = '0'";
        //private const string SEL_PAY_CERTIFY_03020100 = @"SELECT A.CardNo,A.LoanDate,A.PayOffAmt,A.PayOffDate," +
        //        "A.mailDay,A.isMail,A.MailType,A.serialNo,A.voidDay,B.[User_Name],A.Back_Date," +
        //        "A.BackNote,A.UserName,A.UserID " +
        //        "FROM Set_Pay_Certify AS A, set_chk_User AS B " +
        //        "WHERE A.KeyInUser = B.User_ID " +
        //        "AND A.Type = '0' ";

        /// <summary>
        /// 查詢已開立代償證明的SQL
        /// </summary>
        private static string SEL_PAY_CERTIFY_04020100 = @" SELECT DISTINCT " +
                                                           "     A.CardNo " +
                                                           "     ,A.LoanDate " +
                                                           "     ,A.PayOffAmt " +
                                                           "     ,A.PayOffDate " +
                                                           "     ,A.mailDay " +
                                                           "     ,A.isMail " +
                                                           "     ,A.MailType " +
                                                           "     ,A.serialNo " +
                                                           "     ,A.voidDay " +
                                                           "     ,ISNULL( B.[User_Name] , A.KeyInUser )  as User_Name" +
                                                           "     ,A.Back_Date " +
                                                           "     ,A.BackNote " +
                                                           "     ,A.UserName " +
                                                           "     ,A.UserID " +
                                                           " FROM " +
                                                           "     " + ConfigurationManager.AppSettings["ServerPaycheck"] + @"..[Set_Pay_Certify] AS A " + 
                                                           "     LEFT OUTER JOIN " +
                                                           "     " + ConfigurationManager.AppSettings["ServerCSIP"] + @"..[M_USER] AS B " +
                                                           "     ON A.KeyInUser = B.USER_ID " +
                                                           " WHERE " +
                                                           "          A.Type = '1'";
        //private const string SEL_PAY_CERTIFY_04020100 = @"SELECT A.CardNo,A.LoanDate,A.PayOffAmt,A.PayOffDate," +
        //        "A.mailDay,A.isMail,A.MailType,A.serialNo,A.voidDay,B.[User_Name],A.Back_Date," +
        //        "A.BackNote,A.UserName,A.UserID " +
        //        "FROM Set_Pay_Certify AS A, set_chk_User AS B " +
        //        "WHERE A.KeyInUser = B.User_ID " +
        //        "AND A.Type = '1' ";

        /// <summary>
        /// 新增結清證明時的查詢
        /// </summary>
        private const string SEL_SET_PAY_CERTIFY_BY_SERIALNO = @"SELECT isnull(right('0' + convert(varchar(12), (convert(bigint,MAX(serialNo))+1)),11)," +
                        "right('0' + convert(varchar(20), convert(bigint,convert(varchar(20),getdate(),112))-19110000),7) + '0001') " +
                        "FROM SET_PAY_CERTIFY " +
                        "WHERE left(serialNo,7) = right('0' + convert(varchar(20),convert(bigint,convert(varchar(20),getdate(),112))-19110000),7)";

        #endregion SQL
       
        /// <summary>
        /// 作廢
        /// </summary>
        /// <param name="strSerialNo"></param>
        public static bool MarkCancel(String strSerialNo)
        {
            try
            {
                EntitySet_Pay_Certify eSetPayCertify = new EntitySet_Pay_Certify();
                //設置void = Y
                eSetPayCertify.@void = "Y";
                //設置VoidDay=當前日期
                eSetPayCertify.voidDay = DateTime.Now.ToString("yyyyMMdd");//* (格式YYYYMMDD)(原來資料庫存的是民國年日期需要修改).
                SqlHelper Sql = new SqlHelper();
                //條件是SerialNo= SerialNo
                Sql.AddCondition(EntitySet_Pay_Certify.M_serialNo, Operator.Equal, DataTypeUtils.String, strSerialNo);
                if (BRSet_Pay_Certify.UpdateEntityByCondition(eSetPayCertify, Sql.GetFilterCondition(), EntitySet_Pay_Certify.M_void, EntitySet_Pay_Certify.M_voidDay))
                {
                    return true;
                }
                else
                {
                    return false;

                }
            }
            catch (Exception ex)
            {
                BRSet_Pay_Certify.SaveLog(ex);
                return false;
            }

        }
        /// <summary>
        /// 退件
        /// </summary>
        /// <param name="strSerialNo"></param>
        /// <param name="strBackDate"></param>
        /// <param name="strBackNote"></param>
        public static bool MarkReturn(string strSerialNo, string strBackDate, string strBackNote)
        {
            try
            {
                
               
                EntitySet_Pay_Certify eSetPayCertify = new EntitySet_Pay_Certify();
                //設置void = Y
                eSetPayCertify.Back_Date = strBackDate;
                //設置VoidDay=當前日期
                eSetPayCertify.BackNote = strBackNote;
                SqlHelper Sql = new SqlHelper();
                //條件是SerialNo= SerialNo
                Sql.AddCondition(EntitySet_Pay_Certify.M_serialNo, Operator.Equal, DataTypeUtils.String, strSerialNo);
                if (BRSet_Pay_Certify.UpdateEntityByCondition(eSetPayCertify, Sql.GetFilterCondition(), EntitySet_Pay_Certify.M_Back_Date, EntitySet_Pay_Certify.M_BackNote))
                {
                    return true;
                }
                else
                {
                    return false;

                }
            }
            catch (Exception ex)
            {
                BRSet_Pay_Certify.SaveLog(ex); 
                return false;
            }

        }
        /// <summary>
        /// 修改
        /// </summary>
        public static bool UpDate(EntitySet_Pay_Certify eSetPayCertify,string strCondition)
        {
            try
            {
                if (!BRSet_Pay_Certify.UpdateEntityByCondition(eSetPayCertify, strCondition, EntitySet_Pay_Certify.M_zip
                    , EntitySet_Pay_Certify.M_add1, EntitySet_Pay_Certify.M_add2, EntitySet_Pay_Certify.M_add3, EntitySet_Pay_Certify.M_RecvName
                    , EntitySet_Pay_Certify.M_keyinDay, EntitySet_Pay_Certify.M_PayOffDate, EntitySet_Pay_Certify.M_note, EntitySet_Pay_Certify.M_isMail
                    , EntitySet_Pay_Certify.M_MailType, EntitySet_Pay_Certify.M_mailNo, EntitySet_Pay_Certify.M_mailDay, EntitySet_Pay_Certify.M_pay
                    , EntitySet_Pay_Certify.M_payDay, EntitySet_Pay_Certify.M_payName))
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                BRSet_Pay_Certify.SaveLog(ex); 
                return false;
            }
        }
        /// <summary>
        /// 按條件查詢已開立結清證明
        /// </summary>
        /// <param name="strStartSerialNo">證明編號(起)</param>
        /// <param name="strEndSerialNo">證明編號(迄)</param>
        /// <param name="strUserID">身份證字號</param>
        /// <param name="strMailDay">自取日期</param>
        /// <param name="strCardNo">卡號</param>
        /// <param name="intPageIndex">顯示的頁編號</param>
        /// <param name="intPageSize">每頁大小</param>
        /// <param name="intTotalCount">查詢結果總條數</param>
        /// <param name="strMsgID">返回錯誤ID號</param>
        /// <param name="dtblResult">返回結果集</param>
        /// <returns>返回查詢成功失敗標示</returns>
        public static bool SearchResult_03020100(String strStartSerialNo, String strEndSerialNo, 
                String strUserID,String strMailDay,String strCardNo, 
                int intPageIndex, int intPageSize, ref int intTotalCount, 
                ref string strMsgID,ref  DataTable dtblResult)
         {
             try
             {
                 string strSearchSql = SEL_PAY_CERTIFY_03020100;
                 SqlCommand sqlcmdSearchResult = new SqlCommand();
                 sqlcmdSearchResult.CommandType = CommandType.Text;

                 //* 證明編號
                 if (strStartSerialNo != "" || strEndSerialNo != "")
                 {
                     strSearchSql += "AND ( A.SerialNo BETWEEN  @STARTSERIALNO AND @ENDSERIALNO ) ";
                     if (strStartSerialNo == "")
                     {
                         strStartSerialNo = strEndSerialNo;
                     }
                     if (strEndSerialNo == "")
                     {
                         strEndSerialNo = strStartSerialNo;
                     }

                     //* 證明編號(起)
                     SqlParameter parmStartSerialNo = new SqlParameter("@STARTSERIALNO", strStartSerialNo);
                     sqlcmdSearchResult.Parameters.Add(parmStartSerialNo);
                     //* 證明編號(迄)
                     SqlParameter parmEndSerialNo = new SqlParameter("@ENDSERIALNO", strEndSerialNo);
                     sqlcmdSearchResult.Parameters.Add(parmEndSerialNo);
                 }

                 //* 身份證字號
                 if (strUserID != "")
                 {
                     strSearchSql += "AND A.UserID LIKE @USERID ";
                     SqlParameter parmUserID = new SqlParameter("@USERID", strUserID + "%");
                     sqlcmdSearchResult.Parameters.Add(parmUserID);
                 }

                 //* 自取日期
                 if (strMailDay != "")
                 {
                     strSearchSql += "AND A.MailDay = @MAILDAY ";
                     SqlParameter parmMailDay = new SqlParameter("@MAILDAY", strMailDay);
                     sqlcmdSearchResult.Parameters.Add(parmMailDay);
                 }

                 //* 卡號
                 if (strCardNo != "")
                 {
                     strSearchSql += "AND A.CardNo LIKE @CARDNO ";
                     SqlParameter parmCardNo = new SqlParameter("@CARDNO", strCardNo + "%");
                     sqlcmdSearchResult.Parameters.Add(parmCardNo);
                 }

                 sqlcmdSearchResult.CommandText = strSearchSql;
                 //* 查詢已結清記錄
                 DataSet dstSearchResult = BRSet_Pay_Certify.SearchOnDataSet(sqlcmdSearchResult, 
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
             catch(Exception exp)
             {
                 BRSet_Pay_Certify.SaveLog(exp);
                 strMsgID = "04_03020100_007";
                 return false;
             }
         }

         /// <summary>
         /// 按條件查詢已開立代償證明
         /// </summary>
         /// <param name="strStartSerialNo">證明編號(起)</param>
         /// <param name="strEndSerialNo">證明編號(迄)</param>
         /// <param name="strUserID">身份證字號</param>
         /// <param name="strMailDay">自取日期</param>
         /// <param name="strCardNo">卡號</param>
         /// <param name="intPageIndex">顯示的頁編號</param>
         /// <param name="intPageSize">每頁大小</param>
         /// <param name="intTotalCount">查詢結果總條數</param>
         /// <param name="strMsgID">返回錯誤ID號</param>
         /// <param name="dtblResult">返回結果集</param>
         /// <returns>返回查詢成功失敗標示</returns>
         public static bool SearchResult_04020100(String strStartSerialNo, String strEndSerialNo,
                 String strUserID, String strMailDay, String strCardNo,
                 int intPageIndex, int intPageSize, ref int intTotalCount,
                 ref string strMsgID, ref  DataTable dtblResult)
         {
             try
             {
                 string strSearchSql = SEL_PAY_CERTIFY_04020100;
                 SqlCommand sqlcmdSearchResult = new SqlCommand();
                 sqlcmdSearchResult.CommandType = CommandType.Text;

                 //* 證明編號
                 if (strStartSerialNo != "" || strEndSerialNo != "")
                 {
                     strSearchSql += "AND ( A.SerialNo BETWEEN  @STARTSERIALNO AND @ENDSERIALNO ) ";
                     if (strStartSerialNo == "")
                     {
                         strStartSerialNo = strEndSerialNo;
                     }
                     if (strEndSerialNo == "")
                     {
                         strEndSerialNo = strStartSerialNo;
                     }

                     //* 證明編號(起)
                     SqlParameter parmStartSerialNo = new SqlParameter("@STARTSERIALNO", strStartSerialNo);
                     sqlcmdSearchResult.Parameters.Add(parmStartSerialNo);
                     //* 證明編號(迄)
                     SqlParameter parmEndSerialNo = new SqlParameter("@ENDSERIALNO", strEndSerialNo);
                     sqlcmdSearchResult.Parameters.Add(parmEndSerialNo);
                 }

                 //* 身份證字號
                 if (strUserID != "")
                 {
                     strSearchSql += "AND A.UserID LIKE @USERID ";
                     SqlParameter parmUserID = new SqlParameter("@USERID", strUserID + "%");
                     sqlcmdSearchResult.Parameters.Add(parmUserID);
                 }

                 //* 自取日期
                 if (strMailDay != "")
                 {
                     strSearchSql += "AND A.MailDay = @MAILDAY ";
                     SqlParameter parmMailDay = new SqlParameter("@MAILDAY", strMailDay);
                     sqlcmdSearchResult.Parameters.Add(parmMailDay);
                 }

                 //* 卡號
                 if (strCardNo != "")
                 {
                     strSearchSql += "AND A.CardNo LIKE @CARDNO ";
                     SqlParameter parmCardNo = new SqlParameter("@CARDNO", strCardNo + "%");
                     sqlcmdSearchResult.Parameters.Add(parmCardNo);
                 }

                 sqlcmdSearchResult.CommandText = strSearchSql;
                 //* 查詢代償記錄
                 DataSet dstSearchResult = BRSet_Pay_Certify.SearchOnDataSet(sqlcmdSearchResult,
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
                 BRSet_Pay_Certify.SaveLog(exp);
                 strMsgID = "04_04020100_007";
                 return false;
             }
         }

         /// <summary>
         /// 查詢自取簽收表(結清證明)
         /// </summary>
         /// <param name="strStartSerialNo">證明編號(起)</param>
         /// <param name="strEndSerialNo">證明編號(迄)</param>
         /// <param name="strUserID">身份證字號</param>
         /// <param name="strMailDay">自取日期</param>
         /// <param name="strCardNo">卡號</param>
         /// <param name="intPageIndex">顯示的頁編號</param>
         /// <param name="intPageSize">每頁大小</param>
         /// <param name="intTotalCount">查詢結果總條數</param>
         /// <param name="strMsgID">返回錯誤ID號</param>
         /// <param name="esSearchResult">返回結果集</param>
         /// <returns>返回查詢成功失敗標示</returns>
        public static bool SearchResult_03020200(String strStartSerialNo, String strEndSerialNo,
                 String strUserID, String strMailDay, String strCardNo,
                 int intPageIndex, int intPageSize, 
                 ref string strMsgID, ref EntitySet<EntitySet_Pay_Certify> esSearchResult)
         {
             try
             {
                SqlHelper Sql = new SqlHelper();
                //* 添加查詢條件IsMail等於N
                Sql.AddCondition(EntitySet_Pay_Certify.M_isMail, Operator.Equal, DataTypeUtils.String, "N");
                //* 添加查詢條件Type等於0 - 結清, 1 - 代償
                Sql.AddCondition(EntitySet_Pay_Certify.M_type, Operator.Equal, DataTypeUtils.String, "0");
                //* 如果有輸入 證明編號起,則添加查詢條件 SerialNo >= [證明編號起]
                if (strStartSerialNo!="" || strEndSerialNo!="")
                {
                    if (strStartSerialNo=="")
                    {
                        strStartSerialNo = strEndSerialNo;
                    }
                    if (strEndSerialNo == "")
                    {
                        strEndSerialNo = strStartSerialNo;
                    }
                    //* 如果有輸入 證明編號起,則添加查詢條件 SerialNo >= [證明編號起]
                    Sql.AddCondition(EntitySet_Pay_Certify.M_serialNo, Operator.GreaterThanEqual, DataTypeUtils.String, strStartSerialNo);
                    //* 如果有輸入 證明編號迄,則添加查詢條件 SerialNo <= [證明編號迄]
                    Sql.AddCondition(EntitySet_Pay_Certify.M_serialNo, Operator.LessThanEqual, DataTypeUtils.String, strEndSerialNo); 
                }
                if (strUserID!="")
                {
                    //* 如果有輸入[身份證字號],則添加查詢條件UserID LIKE [身份證字號]%
                    Sql.AddCondition(EntitySet_Pay_Certify.M_userID, Operator.LikeLeft, DataTypeUtils.String, strUserID);     
                }
                if (strMailDay!="")
                {
                    //* 如果有輸入[自取日期],則添加查詢條件MailDay = [自取日期]
                    Sql.AddCondition(EntitySet_Pay_Certify.M_mailDay, Operator.Equal, DataTypeUtils.String, strMailDay);     
                }
                if (strCardNo!="")
                {
                    //* 如果有輸入[卡號],則添加查詢條件CardNo LIKE [卡號]%
                    Sql.AddCondition(EntitySet_Pay_Certify.M_CardNo, Operator.LikeLeft, DataTypeUtils.String, strCardNo);                     
                }

                //* 按條件檢索自取簽收表
                esSearchResult = (EntitySet<EntitySet_Pay_Certify>)BRSet_Pay_Certify.Search(Sql.GetFilterCondition(), 
                                    intPageIndex, intPageSize);
                return true;
             }
             catch (Exception exp)
             {
                 BRSet_Pay_Certify.SaveLog(exp);
                 strMsgID = "04_03020200_007";
                 return false;
             }
         }

         /// <summary>
         /// 查詢自取簽收表(代償證明)
         /// </summary>
         /// <param name="strStartSerialNo">證明編號(起)</param>
         /// <param name="strEndSerialNo">證明編號(迄)</param>
         /// <param name="strUserID">身份證字號</param>
         /// <param name="strMailDay">自取日期</param>
         /// <param name="strCardNo">卡號</param>
         /// <param name="intPageIndex">顯示的頁編號</param>
         /// <param name="intPageSize">每頁大小</param>
         /// <param name="intTotalCount">查詢結果總條數</param>
         /// <param name="strMsgID">返回錯誤ID號</param>
         /// <param name="esSearchResult">返回結果集</param>
         /// <returns>返回查詢成功失敗標示</returns>
         public static bool SearchResult_04020200(String strStartSerialNo, String strEndSerialNo,
                  String strUserID, String strMailDay, String strCardNo,
                  int intPageIndex, int intPageSize,
                  ref string strMsgID, ref EntitySet<EntitySet_Pay_Certify> esSearchResult)
         {
             try
             {
                 SqlHelper Sql = new SqlHelper();
                 //* 添加查詢條件IsMail等於N
                 Sql.AddCondition(EntitySet_Pay_Certify.M_isMail, Operator.Equal, DataTypeUtils.String, "N");
                 //* 添加查詢條件Type等於0 - 結清, 1 - 代償
                 Sql.AddCondition(EntitySet_Pay_Certify.M_type, Operator.Equal, DataTypeUtils.String, "1");
                 //* 如果有輸入 證明編號起,則添加查詢條件 SerialNo >= [證明編號起]
                 if (strStartSerialNo != "" || strEndSerialNo != "")
                 {
                     if (strStartSerialNo == "")
                     {
                         strStartSerialNo = strEndSerialNo;
                     }
                     if (strEndSerialNo == "")
                     {
                         strEndSerialNo = strStartSerialNo;
                     }
                     //* 如果有輸入 證明編號起,則添加查詢條件 SerialNo >= [證明編號起]
                     Sql.AddCondition(EntitySet_Pay_Certify.M_serialNo, Operator.GreaterThanEqual, DataTypeUtils.String, strStartSerialNo);
                     //* 如果有輸入 證明編號迄,則添加查詢條件 SerialNo <= [證明編號迄]
                     Sql.AddCondition(EntitySet_Pay_Certify.M_serialNo, Operator.LessThanEqual, DataTypeUtils.String, strEndSerialNo);
                 }
                 if (strUserID != "")
                 {
                     //* 如果有輸入[身份證字號],則添加查詢條件UserID LIKE [身份證字號]%
                     Sql.AddCondition(EntitySet_Pay_Certify.M_userID, Operator.LikeLeft, DataTypeUtils.String, strUserID);
                 }
                 if (strMailDay != "")
                 {
                     //* 如果有輸入[自取日期],則添加查詢條件MailDay = [自取日期]
                     Sql.AddCondition(EntitySet_Pay_Certify.M_mailDay, Operator.Equal, DataTypeUtils.String, strMailDay);
                 }
                 if (strCardNo != "")
                 {
                     //* 如果有輸入[卡號],則添加查詢條件CardNo LIKE [卡號]%
                     Sql.AddCondition(EntitySet_Pay_Certify.M_CardNo, Operator.LikeLeft, DataTypeUtils.String, strCardNo);
                 }

                 //* 按條件檢索自取簽收表
                 esSearchResult = (EntitySet<EntitySet_Pay_Certify>)BRSet_Pay_Certify.Search(Sql.GetFilterCondition(),
                                     intPageIndex, intPageSize);
                 return true;
             }
             catch(Exception exp)
             {
                 BRSet_Pay_Certify.SaveLog(exp);
                 strMsgID = "04_04020200_007";
                 return false;
             }
         }

        /// <summary>
        /// ML結清證明開立登記表(結清證明)
        /// </summary>
        /// <param name="strStartSerialNo">證明編號(起)</param>
        /// <param name="strEndSerialNo">證明編號(迄)</param>
        /// <param name="strUserID">身份證字號</param>
        /// <param name="strMailDay">自取日期</param>
        /// <param name="strCardNo">卡號</param>
        /// <param name="intPageIndex">顯示的頁編號</param>
        /// <param name="intPageSize">每頁大小</param>
        /// <param name="intTotalCount">查詢結果總條數</param>
        /// <param name="strMsgID">返回錯誤ID號</param>
        /// <param name="esSearchResult">返回結果集</param>
        /// <returns>返回查詢成功失敗標示</returns>
        public static bool SearchResult_03020300(String strStartSerialNo, String strEndSerialNo,
                  String strUserID, String strMailDay, String strCardNo,
                  int intPageIndex, int intPageSize,
                  ref string strMsgID, ref EntitySet<EntitySet_Pay_Certify> esSearchResult)
        {
            try
            {
                SqlHelper Sql = new SqlHelper();
                //添加查詢條件Void不等於Y
                Sql.AddCondition(EntitySet_Pay_Certify.M_void, Operator.NotEqual, DataTypeUtils.String, "Y"); 
                //* 添加查詢條件Type等於0 - 結清, 1 - 代償
                Sql.AddCondition(EntitySet_Pay_Certify.M_type, Operator.Equal, DataTypeUtils.String, "0");
                //* 如果有輸入 證明編號起,則添加查詢條件 SerialNo >= [證明編號起]
                if (strStartSerialNo != "" || strEndSerialNo != "")
                {
                    if (strStartSerialNo == "")
                    {
                        strStartSerialNo = strEndSerialNo;
                    }
                    if (strEndSerialNo == "")
                    {
                        strEndSerialNo = strStartSerialNo;
                    }
                    //* 如果有輸入 證明編號起,則添加查詢條件 SerialNo >= [證明編號起]
                    Sql.AddCondition(EntitySet_Pay_Certify.M_serialNo, Operator.GreaterThanEqual, DataTypeUtils.String, strStartSerialNo);
                    //* 如果有輸入 證明編號迄,則添加查詢條件 SerialNo <= [證明編號迄]
                    Sql.AddCondition(EntitySet_Pay_Certify.M_serialNo, Operator.LessThanEqual, DataTypeUtils.String, strEndSerialNo);
                }
                if (strUserID != "")
                {
                    //* 如果有輸入[身份證字號],則添加查詢條件UserID LIKE [身份證字號]%
                    Sql.AddCondition(EntitySet_Pay_Certify.M_userID, Operator.LikeLeft, DataTypeUtils.String, strUserID);
                }
                if (strMailDay != "")
                {
                    //* 如果有輸入[自取日期],則添加查詢條件MailDay = [自取日期]
                    Sql.AddCondition(EntitySet_Pay_Certify.M_mailDay, Operator.Equal, DataTypeUtils.String, strMailDay);
                }
                if (strCardNo != "")
                {
                    //* 如果有輸入[卡號],則添加查詢條件CardNo LIKE [卡號]%
                    Sql.AddCondition(EntitySet_Pay_Certify.M_CardNo, Operator.LikeLeft, DataTypeUtils.String, strCardNo);
                }
                Sql.AddOrderCondition(EntitySet_Pay_Certify.M_serialNo, ESortType.ASC);
                                 //* 按條件檢索自取簽收表
                esSearchResult = (EntitySet<EntitySet_Pay_Certify>)BRSet_Pay_Certify.Search(Sql.GetFilterCondition(),
                                    intPageIndex, intPageSize);
                return true;
            }
            catch (Exception exp)
            {
                BRSet_Pay_Certify.SaveLog(exp);
                strMsgID = "04_03020300_007";
                return false;
            }
        }

        /// <summary>
        /// ML代償證明開立登記表(代償證明)
        /// </summary>
        /// <param name="strStartSerialNo">證明編號(起)</param>
        /// <param name="strEndSerialNo">證明編號(迄)</param>
        /// <param name="strUserID">身份證字號</param>
        /// <param name="strMailDay">自取日期</param>
        /// <param name="strCardNo">卡號</param>
        /// <param name="intPageIndex">顯示的頁編號</param>
        /// <param name="intPageSize">每頁大小</param>
        /// <param name="intTotalCount">查詢結果總條數</param>
        /// <param name="strMsgID">返回錯誤ID號</param>
        /// <param name="esSearchResult">返回結果集</param>
        /// <returns>返回查詢成功失敗標示</returns>
        public static bool SearchResult_04020300(String strStartSerialNo, String strEndSerialNo,
                  String strUserID, String strMailDay, String strCardNo,
                  int intPageIndex, int intPageSize,
                  ref string strMsgID, ref EntitySet<EntitySet_Pay_Certify> esSearchResult)
        {
            try
            {
                SqlHelper Sql = new SqlHelper();
                //添加查詢條件Void不等於Y
                Sql.AddCondition(EntitySet_Pay_Certify.M_void, Operator.NotEqual, DataTypeUtils.String, "Y");
                //* 添加查詢條件Type等於0 - 結清, 1 - 代償
                Sql.AddCondition(EntitySet_Pay_Certify.M_type, Operator.Equal, DataTypeUtils.String, "1");
                //* 如果有輸入 證明編號起,則添加查詢條件 SerialNo >= [證明編號起]
                if (strStartSerialNo != "" || strEndSerialNo != "")
                {
                    if (strStartSerialNo == "")
                    {
                        strStartSerialNo = strEndSerialNo;
                    }
                    if (strEndSerialNo == "")
                    {
                        strEndSerialNo = strStartSerialNo;
                    }
                    //* 如果有輸入 證明編號起,則添加查詢條件 SerialNo >= [證明編號起]
                    Sql.AddCondition(EntitySet_Pay_Certify.M_serialNo, Operator.GreaterThanEqual, DataTypeUtils.String, strStartSerialNo);
                    //* 如果有輸入 證明編號迄,則添加查詢條件 SerialNo <= [證明編號迄]
                    Sql.AddCondition(EntitySet_Pay_Certify.M_serialNo, Operator.LessThanEqual, DataTypeUtils.String, strEndSerialNo);
                }
                if (strUserID != "")
                {
                    //* 如果有輸入[身份證字號],則添加查詢條件UserID LIKE [身份證字號]%
                    Sql.AddCondition(EntitySet_Pay_Certify.M_userID, Operator.LikeLeft, DataTypeUtils.String, strUserID);
                }
                if (strMailDay != "")
                {
                    //* 如果有輸入[自取日期],則添加查詢條件MailDay = [自取日期]
                    Sql.AddCondition(EntitySet_Pay_Certify.M_mailDay, Operator.Equal, DataTypeUtils.String, strMailDay);
                }
                if (strCardNo != "")
                {
                    //* 如果有輸入[卡號],則添加查詢條件CardNo LIKE [卡號]%
                    Sql.AddCondition(EntitySet_Pay_Certify.M_CardNo, Operator.LikeLeft, DataTypeUtils.String, strCardNo );
                }
                Sql.AddOrderCondition(EntitySet_Pay_Certify.M_serialNo, ESortType.ASC);
                //* 按條件檢索自取簽收表
                esSearchResult = (EntitySet<EntitySet_Pay_Certify>)BRSet_Pay_Certify.Search(Sql.GetFilterCondition(),
                                    intPageIndex, intPageSize);
                return true;
            }
            catch (Exception exp)
            {
                BRSet_Pay_Certify.SaveLog(exp);
                strMsgID = "04_04020300_007";
                return false;
            }
        }

        /// <summary>
        /// 列印大宗掛號函存根聯(結清證明)
        /// </summary>
        /// <param name="strMailDay">郵寄日期</param>
        /// <param name="strMailType">郵寄方式</param>
        /// <param name="intPageIndex">顯示的頁編號</param>
        /// <param name="intPageSize">每頁大小</param>
        /// <param name="intTotalCount">查詢結果總條數</param>
        /// <param name="strMsgID">返回錯誤ID號</param>
        /// <param name="esSearchResult">返回結果集</param>
        /// <returns>返回查詢成功失敗標示</returns>
        public static bool SearchResult_03020600(String strMailDay, String strMailType,
                  int intPageIndex, int intPageSize,
                  ref string strMsgID, ref EntitySet<EntitySet_Pay_Certify> esSearchResult)
        {
            try
            {
                SqlHelper Sql = new SqlHelper();
                //添加查詢條件Void不等於Y
                Sql.AddCondition(EntitySet_Pay_Certify.M_void, Operator.NotEqual, DataTypeUtils.String, "Y");
                //* 添加查詢條件Type等於0 - 結清, 1 - 代償
                Sql.AddCondition(EntitySet_Pay_Certify.M_type, Operator.Equal, DataTypeUtils.String, "0");
                //* 添加查詢條件isMail等於Y
                Sql.AddCondition(EntitySet_Pay_Certify.M_isMail, Operator.Equal, DataTypeUtils.String, "Y");

                //* 如果有輸入郵寄日期，MailDay = [郵寄日期]
                if (strMailDay != "")
                {
                    Sql.AddCondition(EntitySet_Pay_Certify.M_mailDay, Operator.Equal, DataTypeUtils.String, strMailDay);
                }

                //* 郵寄方式
                if (strMailType != "0")
                {
                    //* 掛號或限時掛號
                    Sql.AddCondition(EntitySet_Pay_Certify.M_MailType, Operator.Equal, DataTypeUtils.String, strMailType);
                }
                else
                { 
                    //* 全部
                    Sql.AddCondition(EntitySet_Pay_Certify.M_MailType, Operator.In, DataTypeUtils.String, "2,3");
                }
                
                //* 按條件檢索自取簽收表
                esSearchResult = (EntitySet<EntitySet_Pay_Certify>)BRSet_Pay_Certify.Search(Sql.GetFilterCondition(),
                                    intPageIndex, intPageSize);
                return true;
            }
            catch (Exception exp)
            {
                BRSet_Pay_Certify.SaveLog(exp);
                strMsgID = "04_03020600_002";
                return false;
            }
        }

        /// <summary>
        /// 列印大宗掛號函存根聯(代償證明)
        /// </summary>
        /// <param name="strMailDay">郵寄日期</param>
        /// <param name="strMailType">郵寄方式</param>
        /// <param name="intPageIndex">顯示的頁編號</param>
        /// <param name="intPageSize">每頁大小</param>
        /// <param name="intTotalCount">查詢結果總條數</param>
        /// <param name="strMsgID">返回錯誤ID號</param>
        /// <param name="esSearchResult">返回結果集</param>
        /// <returns>返回查詢成功失敗標示</returns>
        public static bool SearchResult_04020600(String strMailDay, String strMailType,
                  int intPageIndex, int intPageSize,
                  ref string strMsgID, ref EntitySet<EntitySet_Pay_Certify> esSearchResult)
        {
            try
            {
                SqlHelper Sql = new SqlHelper();
                //添加查詢條件Void不等於Y
                Sql.AddCondition(EntitySet_Pay_Certify.M_void, Operator.NotEqual, DataTypeUtils.String, "Y");
                //* 添加查詢條件Type等於0 - 結清, 1 - 代償
                Sql.AddCondition(EntitySet_Pay_Certify.M_type, Operator.Equal, DataTypeUtils.String, "1");
                //* 添加查詢條件isMail等於Y
                Sql.AddCondition(EntitySet_Pay_Certify.M_isMail, Operator.Equal, DataTypeUtils.String, "Y");

                //* 如果有輸入郵寄日期，MailDay = [郵寄日期]
                if (strMailDay != "")
                {
                    Sql.AddCondition(EntitySet_Pay_Certify.M_mailDay, Operator.Equal, DataTypeUtils.String, strMailDay);
                }

                //* 郵寄方式
                if (strMailType != "0")
                {
                    //* 掛號或限時掛號
                    Sql.AddCondition(EntitySet_Pay_Certify.M_MailType, Operator.Equal, DataTypeUtils.String, strMailType);
                }
                else
                {
                    //* 全部
                    Sql.AddCondition(EntitySet_Pay_Certify.M_MailType, Operator.In, DataTypeUtils.String, "2,3");
                }

                //* 按條件檢索自取簽收表
                esSearchResult = (EntitySet<EntitySet_Pay_Certify>)BRSet_Pay_Certify.Search(Sql.GetFilterCondition(),
                                    intPageIndex, intPageSize);
                return true;
            }
            catch (Exception exp)
            {
                BRSet_Pay_Certify.SaveLog(exp);
                strMsgID = "04_04020600_002";
                return false;
            }
        }



        /// 作者 占偉林
        /// 創建日期：2009/11/02
        /// <summary>
        /// 查詢序號
        /// </summary>
        /// <param name="strSerialNo">身分證號</param>
        /// <returns>返回查詢結果</returns>
        public static string GetSerialNo()
        {
            try
            {
                SqlCommand sqlcmdSearch = new SqlCommand();
                sqlcmdSearch.CommandText = SEL_SET_PAY_CERTIFY_BY_SERIALNO;
                sqlcmdSearch.CommandType = CommandType.Text;
                DataTable dtblSerialNo = ((DataSet)BRSet_Pay_Certify.SearchOnDataSet(sqlcmdSearch)).Tables[0];
                if (dtblSerialNo.Rows.Count > 0)
                {
                    return dtblSerialNo.Rows[0][0].ToString();
                }
            }
            catch(Exception exp) 
            {
                BRSet_Pay_Certify.SaveLog(exp);    
            }

            return "";
        }
    }
}
