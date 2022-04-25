//******************************************************************
//*  作    者：余洋(rosicky)
//*  功能說明：驗證及開立
//*  創建日期：2009/10/26
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
//*******************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using Framework.Data.OM.Transaction;
using Framework.Data.OM;
using Framework.Common.Message;
using Framework.Common.Logging;
using CSIPCommonModel.BaseItem;
using CSIPCommonModel.EntityLayer;
using CSIPCommonModel.BusinessRules;
using CSIPPayCertify.EntityLayer;

namespace CSIPPayCertify.BusinessRules
{
    public class BRPay_SV_Tmp : BRBase<EntityPay_SV_Tmp>
    {
        #region sql語句 

//        public const string SEL_PAY_SV_TMP_BY_NOADDINFO = @"SELECT ROW_NUMBER() OVER (ORDER BY C.USERID) AS  NO,C.* FROM 
//(
//SELECT DISTINCT A.CASE_NO,A.NUM,USERID,USERNAME,APPLYDATE,ISADD,ISCHECK,ISREBACK,CHECKTIME FROM
//(
//SELECT CASE_NO,COUNT(1) AS NUM FROM PAY_SV_TMP WHERE CAST(ISNULL(ISREBACK,'0')AS INT) <> '1' OR CAST(ISNULL(ISADD, '0') AS INT) <> '0' GROUP BY CASE_NO
//) AS A,
//(
//SELECT CASE_NO,USERID,USERNAME,APPLYDATE,ISCHECK,ISADD,ISREBACK,CHECKTIME FROM PAY_SV_TMP
//) AS B 
//WHERE A.CASE_NO = B.CASE_NO
//) AS C";
//        public const string SEL_PAY_SV_TMP_BY_REBACKINFO = @"SELECT ROW_NUMBER() OVER (ORDER BY C.USERID) AS  NO,C.* FROM 
//(
//SELECT DISTINCT A.CASE_NO,A.NUM,USERID,USERNAME,APPLYDATE,ISCHECK,ISREBACK,CHECKTIME,REBACKREASON FROM
//(
//SELECT CASE_NO,COUNT(1) AS NUM FROM PAY_SV_TMP WHERE CAST(ISNULL(ISREBACK,'0')AS INT) = '1' AND CAST(ISNULL(ISADD, '0') AS INT) = '0' GROUP BY CASE_NO
//) AS A,
//(
//SELECT CASE_NO,USERID,USERNAME,APPLYDATE,ISCHECK,ISREBACK,CHECKTIME,REBACKREASON FROM PAY_SV_TMP
//) AS B 
//WHERE A.CASE_NO = B.CASE_NO
//) AS C";

        public const string SEL_PAY_SV_TMP_BY_NOADDINFO = @"
                SELECT DISTINCT A.CASE_NO,A.NUM,USERID,USERNAME,APPLYDATE,ISADD,ISCHECK,ISREBACK,CHECKTIME 
                FROM
                (
                    SELECT CASE_NO,COUNT(1) AS NUM 
                    FROM PAY_SV_TMP 
                    WHERE CAST(ISNULL(ISREBACK,'0')AS INT) <> '1' OR CAST(ISNULL(ISADD, '0') AS INT) <> '0' 
                    GROUP BY CASE_NO
                ) AS A,
                (
                    SELECT CASE_NO,USERID,USERNAME,APPLYDATE,ISCHECK,ISADD,ISREBACK,CHECKTIME 
                    FROM PAY_SV_TMP
                ) AS B 
                WHERE A.CASE_NO = B.CASE_NO
                ";
        public const string SEL_PAY_SV_TMP_BY_REBACKINFO = @"
                SELECT DISTINCT A.CASE_NO,A.NUM,USERID,USERNAME,APPLYDATE,ISCHECK,ISREBACK,CHECKTIME,REBACKREASON 
                FROM
                (
	                SELECT CASE_NO,COUNT(1) AS NUM 
	                FROM PAY_SV_TMP 
	                WHERE CAST(ISNULL(ISREBACK,'0')AS INT) = '1' AND CAST(ISNULL(ISADD, '0') AS INT) = '0' 
	                GROUP BY CASE_NO
                ) AS A,
                (
	                SELECT CASE_NO,USERID,USERNAME,APPLYDATE,ISCHECK,ISREBACK,CHECKTIME,REBACKREASON 
	                FROM PAY_SV_TMP
                ) AS B 
                WHERE A.CASE_NO = B.CASE_NO
                ";
        public const string SEL_PAY_SV_TMP_BY_NOCHECK = @"SELECT UserID,UserName,ApplyDate,Status,Address,ApplyUser,AgreeUser,ClearDate,Enddate,Consignee,CardNo,BLKCode,CardType,CardName,CardID,IsPaidOffAlone,case_no,SeqNo,IsCheck,IsReback,IsADD,CheckTime,RebackReason FROM Pay_SV_Tmp WHERE ISNULL(IsCheck,0) = 0 OR IsCheck = ''";
        public const string SEL_PAY_SV_TMP_BY_ADD = @"SELECT UserID,UserName,ApplyDate,Status,Address,ApplyUser,AgreeUser,ClearDate,Enddate,Consignee,CardNo,BLKCode,CardType,CardName,CardID,IsPaidOffAlone,case_no,SeqNo,IsCheck,IsReback,IsADD,CheckTime,RebackReason FROM Pay_SV_Tmp WHERE IsCheck = '1' AND IsADD = '1'";
        public const string SEL_PAY_SV_TMP_BY_CASENO = @"SELECT UserID,UserName,ApplyDate,Status,Address,ApplyUser,AgreeUser,ClearDate,Enddate,Consignee,CardNo,BLKCode,CardType,CardName,CardID,IsPaidOffAlone,case_no,SeqNo,IsCheck,IsReback,IsADD,CheckTime,RebackReason FROM Pay_SV_Tmp WHERE Case_NO = @case_no";

        public const string ADD_Pay_SV_Feedback_BY_RebackAll = @"INSERT INTO Pay_SV_Feedback(UserID, UserName, ApplyDate, Status, Address, ApplyUser, AgreeUser, ClearDate, Enddate, Consignee, CardNo, BLKCode, CardType, CardName, CardID, IsPaidOffAlone, case_no, SeqNo, IsCheck, IsReback, IsADD, CheckTime, RebackReason, rebackDate, rebackDateTime, rebackDateUser) SELECT UserID, UserName, ApplyDate, Status, Address, ApplyUser, AgreeUser, ClearDate, Enddate, Consignee, CardNo, BLKCode, CardType, CardName, CardID, IsPaidOffAlone, case_no, SeqNo, IsCheck, IsReback, IsADD, CheckTime, RebackReason, @rebackDate, @rebackDateTime, @rebackDateUser FROM Pay_SV_Tmp WHERE Case_NO = @case_no";

        public const string SEL_REBACKBY_CASE_NO = @"INSERT INTO Pay_SV_Feedback
(UserID, UserName, ApplyDate, Status, Address, ApplyUser, AgreeUser, ClearDate, Enddate, Consignee, CardNo, BLKCode, CardType, CardName, CardID, IsPaidOffAlone, case_no, SeqNo, IsCheck, IsReback, IsADD, CheckTime, RebackReason, rebackDate, rebackDateTime, rebackDateUser) 
SELECT UserID, UserName, ApplyDate, Status, Address, ApplyUser, AgreeUser, ClearDate, Enddate, Consignee, CardNo, BLKCode, 
CardType, CardName, CardID, IsPaidOffAlone, case_no, SeqNo, IsCheck, IsReback, IsADD, CheckTime, @RebackReason, @DATE, @TIME, @CSIPUSER
FROM Pay_SV_Tmp
WHERE
Case_No = @Case_No";
        public const string DELETE_PAY_SV_TMP = @"DELETE Pay_SV_Tmp WHERE Case_No = @Case_No";

        #endregion

        /// <summary>
        /// 查詢未驗證+已驗證未開立資料
        /// </summary>
        /// <param name="strRtnMsg">錯誤訊息ID</param>
        /// <param name="dtblResult">返回結果集</param>
        /// <returns>True---查詢成功；False----查詢失敗</returns>
        public static bool SearchNoAddInfo(ref string strRtnMsg, ref DataTable dtblResult)
        {
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = SEL_PAY_SV_TMP_BY_NOADDINFO;
            sqlcmd.CommandType = CommandType.Text;
            dtblResult = BRPay_SV_Tmp.SearchOnDataSet(sqlcmd).Tables[0];
            if (dtblResult == null)
            {
                strRtnMsg = "04_02010100_002";
                return false;
            }
            strRtnMsg = "04_02010100_001";            
            return true;
        }
        /// <summary>
        /// 查詢需要剔退的資料
        /// </summary>
        /// <param name="strRtnMsg">錯誤訊息ID</param>
        /// <param name="dtblResult">返回結果集</param>
        /// <returns>True---查詢成功；False----查詢失敗</returns>
        public static bool SearchRebackInfo(ref string strRtnMsg, ref DataTable dtblResult)
        {
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandText = SEL_PAY_SV_TMP_BY_REBACKINFO;
            sqlcmd.CommandType = CommandType.Text;
            dtblResult = BRPay_SV_Tmp.SearchOnDataSet(sqlcmd).Tables[0];
            if (dtblResult == null)
            {
                strRtnMsg = "04_02010100_004";
                return false;
            }
            strRtnMsg = "04_02010100_003";
            return true;
        }
        /// <summary>
        /// 取得所有沒有檢核的資料
        /// </summary>
        /// <returns></returns>
        public static DataTable GetNoCheck()
        {
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandText = SEL_PAY_SV_TMP_BY_NOCHECK;
            DataSet dsResult = SearchOnDataSet(sqlcmd);
            if (dsResult == null || dsResult.Tables.Count <= 0) { return null; }
            return dsResult.Tables[0];
        }
        /// <summary>
        /// 通過CaseNo取得資料信息
        /// </summary>
        /// <param name="strCaseNo"></param>
        /// <returns></returns>
        public static DataTable GetDataByCaseNo(string strCaseNo)
        {
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandText = SEL_PAY_SV_TMP_BY_CASENO;
            sqlcmd.Parameters.Add(new SqlParameter("@case_no", strCaseNo));
            DataSet dsResult = SearchOnDataSet(sqlcmd);
            if (dsResult == null || dsResult.Tables.Count <= 0)
            {
                return null;
            }
            else
            {
                return dsResult.Tables[0];
            }

        }


        /// <summary>
        /// 傳入strAddress地址串,拆分成ZIP,地址123傳出
        /// </summary>
        /// <param name="strAddress">地址串</param>
        /// <param name="strAdd1">地址1</param>
        /// <param name="strAdd2">地址2</param>
        /// <param name="strAdd3">地址3</param>
        /// <returns>True---查詢成功；False----查詢失敗</returns>
        /// <summary>
        public static bool GetAddress(string strAddress,ref string  strZip, ref string strAdd1, ref string strAdd2, ref string strAdd3 )
        {
            string strTmpAddress = strAddress.Trim();
            if (strTmpAddress.Length < 3)
            {
                return false;
            }
            strZip = strTmpAddress.Substring(0,3).Trim();
            strTmpAddress = strTmpAddress.Substring(3).Trim();
            string strTmp="";
            try
            {
                for (int i = 5; i >= 0; i--)
                {
                    strTmp = strTmpAddress.Substring(i, 1);
                    if (strTmp == MessageHelper.GetMessage("04_00000000_018") || strTmp == MessageHelper.GetMessage("04_00000000_019") || strTmp == MessageHelper.GetMessage("04_00000000_020") || strTmp == MessageHelper.GetMessage("04_00000000_021") || strTmp == MessageHelper.GetMessage("04_00000000_022") || strTmp == MessageHelper.GetMessage("04_00000000_023") || strTmp == MessageHelper.GetMessage("04_00000000_024"))
                    {
                        strAdd1 = strTmpAddress.Substring(0, i+1).Trim();
                        strTmpAddress = strTmpAddress.Substring(i+1).Trim();
                        break;
                    }
                }

                if (strTmpAddress.Length > 14)
                {
                    strAdd2 = strTmpAddress.Substring(0, 14).Trim();
                    strTmpAddress = strTmpAddress.Substring(14);
                    strAdd3 = strTmpAddress.Substring(0, 14).Trim();
                }
                else
                {
                    strAdd2 = strTmpAddress.Trim();
                    strAdd3 = "";

                }
            }
            catch(Exception exp)
            {
                BRPay_SV_Tmp.SaveLog(exp);
                return false; 
            }
            return true;
        }

   
        /// <summary>
        /// 驗證卡片
        /// </summary>
        /// <param name="strCustID">客戶ID</param>
        /// <param name="strCardNo">卡號</param>
        /// <param name="strStatusCode">BLKCode</param>
        /// <param name="strStautsDate">BLKDate</param>
        /// <param name="strSold">不良資產是否已出售</param>
        /// <param name="htRebackReason">剔退原因</param>
        /// <param name="strRtnMsg">出錯訊息</param>
        /// <param name="intP4_JCU9">-1沒連過JCU9,0:JCU9查無資料或爲0,1:JCU9不為0</param>
        /// <param name="intP4_JCII">-1沒連過JCII,0:JCII查無資料或爲0,1:JCII不為0</param>
        /// <returns></returns>
        public static bool ValidationSV(string strCustID, string strCardNo, string strStatusCode, string strStautsDate, string strSold, ref Hashtable htRebackReason, ref string strRtnMsg, ref int intP4_JCU9, ref int intP4_JCII, EntityAGENT_INFO eAgentInfo, ref string strHostMsg)
        {
            string strMsg = "";
            Hashtable htInput = new Hashtable();
            DataTable dtblOutput = new DataTable();

            try
            {
                //* strStautsDate和當前日期的天數差
                strStautsDate = strStautsDate.Substring(4, 4) + "-" + strStautsDate.Substring(0, 2) + "-" + strStautsDate.Substring(2, 2);
                int intDateDiff = int.Parse(Function.DateDiff("d", DateTime.Parse(strStautsDate), DateTime.Now).ToString());
                #region BLK = Z
                if (strStatusCode == "Z" && strSold != "")
                {
                    //* 卡號:{0} 不良資產已出售
                    htRebackReason.Add(htRebackReason.Count.ToString(), MessageHelper.GetMessage("04_02010100_057",strCardNo));
                }
                #endregion

                #region BLK = M
                if (strStatusCode == "M")
                {
                    if (intDateDiff <= 7)
                    {
                        //* 資料是否已灌入催收平台
                        htRebackReason.Add(htRebackReason.Count.ToString(), MessageHelper.GetMessage("04_02010100_058",strCardNo));
                    }
                    else if (strSold != "")
                    {
                        //* 卡號:{0} 不良資產已出售
                        htRebackReason.Add(htRebackReason.Count.ToString(), MessageHelper.GetMessage("04_02010100_057",strCardNo));
                    }
                }
                #endregion

                #region BLK = S,B,A
                if ((strStatusCode == "S" || strStatusCode == "B" || strStatusCode == "A") && intDateDiff<=6000)
                {
                    #region JCU9
                    if (intP4_JCU9 == -1)
                    {
                        //*得到主機傳回信息
                        htInput.Add("FUNCTION_CODE", "1");//*ID查詢
                        htInput.Add("ACCT_NBR", strCustID);
                        htInput.Add("LINE_CNT", "0000");//*LINE_CNT

                        dtblOutput = MainFrameInfo.GetMainframeDataByPage("P4_JCU9", htInput, false, "1", eAgentInfo);
                        if (dtblOutput.Rows[0]["HtgErrMsg"].ToString() != "") //*主機返回失敗
                        {
                            if (dtblOutput.Rows[0]["MESSAGE_TYPE"].ToString() == "8888")
                            {
                                strRtnMsg = MessageHelper.GetMessage("04_01010100_009");
                                strHostMsg = dtblOutput.Rows[0]["HtgErrMsg"].ToString();
                                //* 查不到資料算通過.
                                intP4_JCU9 = 0;
                            }
                            else
                            {
                                SaveLog(strRtnMsg);
                                strRtnMsg = MessageHelper.GetMessage("04_00000000_006");
                                strHostMsg = dtblOutput.Rows[0]["HtgErrMsg"].ToString();
                                return false;
                            }
                        }
                        else //*主機返回正確
                        {
                            strRtnMsg = MessageHelper.GetMessage("04_00000000_032");
                            strHostMsg = dtblOutput.Rows[0]["MESSAGE_TYPE"].ToString() + " " + MessageHelper.GetMessage("04_00000000_032");
                            //*如果<1913電文.未到期餘額>不爲空
                            if (dtblOutput.Rows[0]["ORDER_BAL"].ToString() != "")
                            {
                                htRebackReason.Add(htRebackReason.Count.ToString(), MessageHelper.GetMessage("04_02010100_059"));
                                intP4_JCU9 = 1;
                            }
                            else
                            {
                                //* 等於0也算JCU9通過
                                intP4_JCU9 = 0;
                            }
                        }
                    }
                    #endregion
                    #region JCII
                    //*如果<1913電文.未到期餘額>爲空
                    if (intP4_JCU9 == 0)
                    {
                        //* 如果JCU9查不到就要查JCII
                        if (intP4_JCII==-1)
                        {
                            //* JCII沒查過就進來查,查過了就不查了                            htInput.Clear();
                            dtblOutput.Clear();

                            htInput.Add("FUNCTION_CODE", "1");//*ID查詢
                            htInput.Add("ACCT_NBR", strCustID);
                            htInput.Add("LINE_CNT", "0000");//*LINE_CNT

                            dtblOutput = MainFrameInfo.GetMainframeDataByPage("P4_JCII", htInput, false, "1", eAgentInfo);
                            if (dtblOutput.Rows[0]["HtgErrMsg"].ToString() != "") //*主機返回失敗
                            {
                                //if (dtblOutput.Rows[0]["MESSAGE_TYPE"].ToString() == "8888")
                                //{
                                //    strRtnMsg = MessageHelper.GetMessage("04_01010100_009");
                                //    strHostMsg = dtblOutput.Rows[0]["HtgErrMsg"].ToString();
                                //    intP4_JCII = 0;
                                //}
                                //else
                                //{
                                //    SaveLog(strMsg);
                                //    strRtnMsg = MessageHelper.GetMessage("04_00000000_007");
                                //    strHostMsg = dtblOutput.Rows[0]["HtgErrMsg"].ToString();
                                //    return false;
                                //}

                                intP4_JCII = 0;
                            }
                            else //*主機返回正確
                            {
                                strRtnMsg = MessageHelper.GetMessage("04_00000000_033");
                                strHostMsg = dtblOutput.Rows[0]["MESSAGE_TYPE"].ToString() + " " + MessageHelper.GetMessage("04_00000000_033");
                                intP4_JCII = 1;
                                for (int iRow = 0; iRow < dtblOutput.Rows.Count; iRow++)
                                {
                                    
                                    //*如果1615電文找的到資料且未出帳總金額不為0
                                    if (dtblOutput.Rows[iRow]["STATUS"].ToString().Trim() == "N" &&
                                        (int.Parse(dtblOutput.Rows[iRow]["UNBILLED_AMT"].ToString() == "" ? "0" : dtblOutput.Rows[iRow]["UNBILLED_AMT"].ToString()) != 0))
                                    {
                                        intP4_JCII = 1;
                                        htRebackReason.Add(htRebackReason.Count.ToString(), MessageHelper.GetMessage("04_02010100_059"));
                                        break;
                                    }
                                    else
                                    {
                                        //* 有資料.
                                        intP4_JCII = 2;

                                    }                               
                                }                                
                            }
                        }
                    }
                    if (intP4_JCII == 0)
                    {
                        if (strSold != "")
                        {
                            //* 卡號:{0} 不良資產已出售
                            htRebackReason.Add(htRebackReason.Count.ToString(), MessageHelper.GetMessage("04_02010100_057",strCardNo));
                        }
                    }
                    #endregion

                }
                #endregion
            }
            catch(Exception ex)
            {
                strRtnMsg = "00_00000000_000";
                BRPay_SV_Tmp.SaveLog(ex);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 批量開立
        /// </summary>
        /// <param name="strRtnMsg"></param>
        /// <param name="strSerialNoList"></param>
        /// <returns></returns>
        public static bool AddAll(ref string strRtnMsg, ref string strSerialNoList)
        {
            try
            {
                string strType = "";        //* 清證種類 C-CardOnly,M-Card&ML,N-MLOnly
                string strCardType = "";    //* 0-主卡Only,1-附卡Only,2-主卡+附卡
                string strTypeCard = "";    //* 卡片類型, M-主卡,E-附卡,N-ML
                string strSerialNo = "";    //* 清證序號
                string strZip = "";
                string strAdd1 = "";
                string strAdd2 = "";
                string strAdd3 = "";
                string strCaseNo = "";
                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.CommandText = SEL_PAY_SV_TMP_BY_ADD;
                DataSet dsResult = SearchOnDataSet(sqlcmd);
                if (dsResult == null || dsResult.Tables.Count <= 0) 
                {        
                    //* 無開立數據
                    strRtnMsg = "04_02010100_010";
                    return false; 
                }
                DataView dvResult = dsResult.Tables[0].DefaultView;
                DataTable dtblCaseNo = dvResult.ToTable(true, "UserID", "case_no");
                for (int i = 0; i < dtblCaseNo.Rows.Count; i++)
                {
                    strCaseNo = dtblCaseNo.Rows[i]["case_no"].ToString().Trim();
                    dvResult.RowFilter = " case_no = '" + strCaseNo  + "'";
                    strType = "";
                    strCardType = "";
                    strTypeCard = "";
                    strSerialNo = "";
                    for (int j = 0; j < dvResult.Count; j++)
                    {
                        #region 取得清證種類
                        if (Function.IsML(dvResult[j]["CardNo"].ToString()) == 1)
                        {
                            //* 當前卡是ML
                            //* 當前卡是信用卡
                            switch (strType)
                            {
                                case "":
                                    strType = "N";
                                    break;
                                case "C":
                                    strType = "M";
                                    break;
                                case "N":
                                    strType = "N";
                                    break;
                                case "M":
                                    strType = "M";
                                    break;
                                default:
                                    break;
                            }
                        }
                        else
                        {
                            //* 當前卡是信用卡
                            switch (strType)
                            {
                                case "":
                                    strType = "C";
                                    break;
                                case "C":
                                    strType = "C";
                                    break;
                                case "N":
                                    strType = "M";
                                    break;
                                case "M":
                                    strType = "M";
                                    break;
                                default:
                                    break;
                            }
                        }
                        #endregion
                        #region 取得開立類型
                        if (dvResult[j]["CardType"].ToString().Trim() == "0")
                        {
                            //* 主卡
                            switch (strCardType)
                            {
                                case "":
                                    strCardType = "0";  //* 主卡Only
                                    break;
                                case "0":
                                    strCardType = "0";  //* 主卡Only
                                    break;
                                case "1":
                                    strCardType = "2";  //* 主卡+附卡
                                    break;
                                case "2":
                                    strCardType = "2";  //* 主卡+附卡
                                    break;
                                default:
                                    break;
                            }
                        }
                        else
                        {
                            //* 附卡
                            switch (strCardType)
                            {
                                case "":
                                    strCardType = "1";  //* 附卡Only
                                    break;
                                case "0":
                                    strCardType = "2";  //* 主卡+附卡
                                    break;
                                case "1":
                                    strCardType = "1";  //* 附卡Only
                                    break;
                                case "2":
                                    strCardType = "2";  //* 主卡+附卡
                                    break;
                                default:
                                    break;
                            }
                        }
                        #endregion  
                    }

                    //* 取得序號
                    if (!BRPay_SerialSV.GetSerialSVNo(ref strSerialNo, ref strRtnMsg))
                    {
                        return false;
                    }
                    #region Pay_SV主表
                    GetAddress(dvResult[0]["Address"].ToString().Trim(), ref strZip, ref strAdd1, ref  strAdd2, ref strAdd3);
                    EntityPay_SV ePaySV = new EntityPay_SV();
                    //* 將序號存入序號列表(For 後面列印出這些證明書用)
                                       
                    ePaySV.SerialNo = strSerialNo;
                    ePaySV.UserID = dvResult[0]["UserID"].ToString().Trim();
                    ePaySV.UserName = dvResult[0]["UserName"].ToString().Trim();
                    ePaySV.ApplyDate = dvResult[0]["ApplyDate"].ToString().Trim();
                    ePaySV.Status = dvResult[0]["Status"].ToString().Trim();
                    ePaySV.ApplyUser = dvResult[0]["ApplyUser"].ToString().Trim();
                    ePaySV.AgreeUser = dvResult[0]["AgreeUser"].ToString().Trim();
                    ePaySV.ClearDate = dvResult[0]["ClearDate"].ToString().Trim();
                    ePaySV.Enddate = dvResult[0]["Enddate"].ToString().Trim();
                    ePaySV.Consignee = dvResult[0]["Consignee"].ToString().Trim();
                    ePaySV.BLKCode = dvResult[0]["BLKCode"].ToString().Trim();
                    ePaySV.Zip = strZip;
                    ePaySV.Add1 = strAdd1;
                    ePaySV.Add2 = strAdd2;
                    ePaySV.Add3 = strAdd3;
                    ePaySV.MailDay = DateTime.Now.AddDays(2).ToString("yyyyMMdd");
                    ePaySV.MailNo = "";
                    ePaySV.Note = "免收-SV" + dvResult[0]["ApplyDate"].ToString().Trim();
                    ePaySV.MakeUp = "N";
                    ePaySV.MakeUpDate = "";
                    ePaySV.MailMethod = "平信";
                    ePaySV.ShowExtra = "Y";
                    ePaySV.GetFee = "N";
                    ePaySV.IsMail = "Y";
                    ePaySV.KeyinDay = DateTime.Now.ToString("yyyyMMdd") ;
                    ePaySV.Type = strType;
                    ePaySV.@void = "";
                    ePaySV.CardType = strCardType;
                    ePaySV.CreateDate = DateTime.Now.ToString("yyyyMMdd");
                    ePaySV.IsPrinted = "Y";
                    ePaySV.IsPaidOffAlone = dvResult[0]["IsPaidOffAlone"].ToString().Trim();
                    #endregion
                    #region Pay_CardSV副表
                    EntityPay_CardSVSet esPayCardSV = new EntityPay_CardSVSet();
                    for (int j = 0; j < dvResult.Count; j++)
                    {
                        if (dvResult[j]["CardType"].ToString().Trim() == "0")
                        {
                            //* 主卡
                            strTypeCard = "M";
                        }
                        else
                        {
                            if (Function.IsML(dvResult[j]["CardNo"].ToString()) == 1)
                            {
                                //* ML
                                strTypeCard = "L";
                            }
                            else
                            {
                                //* 附卡
                                strTypeCard = "E";
                            }
                        }

                        EntityPay_CardSV ePayCardSV = new EntityPay_CardSV();
                        ePayCardSV.SERIALNO = strSerialNo;
                        ePayCardSV.NO = Convert.ToDecimal(j);
                        ePayCardSV.CARDNO = dvResult[j]["CardNo"].ToString().Trim();
                        ePayCardSV.CUSID = dvResult[j]["CardID"].ToString().Trim();
                        ePayCardSV.CUSNAME = dvResult[j]["CardName"].ToString().Trim();
                        ePayCardSV.TYPECARD = strTypeCard;
                        ePayCardSV.BLK = dvResult[j]["BLKCode"].ToString().Trim();
                        esPayCardSV.Add(ePayCardSV);
                    }
                    #endregion
                    #region 開批量始插入
                    using (OMTransactionScope ts = new OMTransactionScope())
                    {
                        if (BRPay_CardSV.BatInsert(esPayCardSV))
                        {
                            //* 插完從表開始查主表
                            if (BRPay_SV.AddNewEntity(ePaySV))
                            {
                                
                                //* 主表插完當筆剔退,原因爲空
                                if (BRPay_SV_Tmp.RebackBycase_No(strCaseNo, "", ref strRtnMsg))
                                {
                                    strSerialNoList = strSerialNoList + "'" + strSerialNo + "',";
                                }
                                else
                                {
                                    strRtnMsg = "04_02010100_009";
                                    return false;
                                }                                
                            }
                            else
                            {
                                strRtnMsg = "04_02010100_009";
                                return false;
                            }
                        }
                        else
                        {
                            strRtnMsg = "04_02010100_009";
                            return false;
                        }
                        ts.Complete();
                    }
                    #endregion
                   
                }//* end for

                if (!string.IsNullOrEmpty(strSerialNoList))
                {
                    strSerialNoList = strSerialNoList.Substring(0, strSerialNoList.Length - 1);
                    Logging.SaveLog(strSerialNoList, ELogType.Info);
                }
                return true;
            }
            catch (Exception Ex)
            {
                BRPay_SV_Tmp.SaveLog(Ex);
                strRtnMsg = "04_02010100_009";
                return false;
            }            
            
        }

        /// <summary>
        /// 剔退所有
        /// </summary>
        /// <param name="strRtnMsg">錯誤訊息</param>
        /// <returns>Ture-成功/False-失敗</returns>
        public static bool RebackAll(ref string strRtnMsg)
        {
            DataTable dtblRejectDetail = new DataTable();
            if (SearchRebackInfo(ref strRtnMsg, ref dtblRejectDetail) == false) { return false; }

            if (dtblRejectDetail.Rows.Count > 0)
            {
                try
                {
                    SqlCommand sqlcmd = new SqlCommand();
                    sqlcmd.CommandText = ADD_Pay_SV_Feedback_BY_RebackAll;
                    sqlcmd.CommandType = CommandType.Text;
                    DateTime dtmCurrently = DateTime.Now;
                    string strDate = dtmCurrently.ToString("yyyyMMdd");
                    string strTime = dtmCurrently.ToString("HHmmss");
                    EntityPay_SV_Tmp ePaySVTmp = new EntityPay_SV_Tmp();
                    //* 事務處理。
                    using (OMTransactionScope ts = new OMTransactionScope())
                    {
                        for (int i = 0; i < dtblRejectDetail.Rows.Count; i++)
                        {
                            SqlParameter parmRebackDate = new SqlParameter("@" + EntityPay_SV_Feedback.M_rebackDate, strDate);
                            sqlcmd.Parameters.Add(parmRebackDate);

                            SqlParameter parmRebackDateTime = new SqlParameter("@" + EntityPay_SV_Feedback.M_rebackDateTime, strTime);
                            sqlcmd.Parameters.Add(parmRebackDateTime);

                            SqlParameter parmRebackDateUser = new SqlParameter("@" + EntityPay_SV_Feedback.M_rebackDateUser, ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)System.Web.HttpContext.Current.Session["Agent"]).agent_name.ToString());
                            sqlcmd.Parameters.Add(parmRebackDateUser);

                            SqlParameter parmCaseNO = new SqlParameter("@" + EntityPay_SV_Tmp.M_case_no, dtblRejectDetail.Rows[i]["Case_NO"].ToString());
                            sqlcmd.Parameters.Add(parmCaseNO);

                            //* 先新增
                            if (!BRPay_SV_Feedback.Add(sqlcmd))
                            {
                                strRtnMsg = "04_02010100_006";
                                return false;
                            }
                            sqlcmd.Parameters.Clear();
                        }

                        SqlHelper Sql = new SqlHelper();
                        Sql.AddCondition(EntityPay_SV_Tmp.M_IsADD, Operator.Equal, DataTypeUtils.String, "0");
                        Sql.AddCondition(EntityPay_SV_Tmp.M_IsReback, Operator.Equal, DataTypeUtils.String, "1");
                        if (!BRPay_SV_Tmp.DeleteEntityByCondition(ePaySVTmp, Sql.GetFilterCondition()))
                        {
                            strRtnMsg = "04_02010100_006";
                            return false;
                        }
                        //* 事務提交
                        ts.Complete();
                        strRtnMsg = "04_02010100_005";
                        return true;
                    }
                }
                catch (Exception exp)
                {
                    BRPay_SV_Tmp.SaveLog(exp);
                    strRtnMsg = "04_02010100_006";
                    return false;
                }
            }
            strRtnMsg = "04_02010100_007";
            return false;
        }
        /// <summary>
        /// 設置剔退Flag
        /// </summary>
        /// <param name="strRtnMsg">錯誤訊息ID</param>
        /// <param name="ePaySVTmp">傳入數據</param>
        /// <returns>True---查詢成功；False----查詢失敗</returns>

        public static bool SetReback(ref string strRtnMsg, EntityPay_SV_Tmp ePaySVTmp)
        {
            SqlHelper sql = new SqlHelper();

            sql.AddCondition(EntityPay_SV_Tmp.M_case_no, Operator.Equal, DataTypeUtils.String, ePaySVTmp.case_no);

            ePaySVTmp.CheckTime = ePaySVTmp.CheckTime + 1;
            ePaySVTmp.IsReback = "1";
            ePaySVTmp.IsCheck = "1";
            if (!BRPay_SV_Tmp.UpdateEntityByCondition(ePaySVTmp, sql.GetFilterCondition(), EntityPay_SV_Tmp.M_IsCheck, EntityPay_SV_Tmp.M_IsReback, EntityPay_SV_Tmp.M_CheckTime, EntityPay_SV_Tmp.M_RebackReason))
            {
                strRtnMsg = "04_02010100_006";
                return false;
            }
            strRtnMsg = "04_02010100_005";
            return true;
        }
        /// <summary>
        /// 設置開立Flag
        /// </summary>
        /// <param name="strRtnMsg">錯誤訊息ID</param>
        /// <param name="ePaySVTmp">傳入數據</param>
        /// <returns>True---查詢成功；False----查詢失敗</returns>
        public static bool SetAdd(ref string strRtnMsg, EntityPay_SV_Tmp ePaySVTmp)
        {
            SqlHelper sql = new SqlHelper();

            sql.AddCondition(EntityPay_SV_Tmp.M_case_no, Operator.Equal, DataTypeUtils.String, ePaySVTmp.case_no);

            ePaySVTmp.CheckTime = ePaySVTmp.CheckTime + 1;
            ePaySVTmp.IsADD = "1";
            ePaySVTmp.IsCheck = "1";
            if (!BRPay_SV_Tmp.UpdateEntityByCondition(ePaySVTmp,sql.GetFilterCondition(), EntityPay_SV_Tmp.M_IsCheck, EntityPay_SV_Tmp.M_IsADD, EntityPay_SV_Tmp.M_CheckTime))
            {
                strRtnMsg = "04_02010100_009";
                return false;
            }
            strRtnMsg = "04_02010100_008";
            return true;
        }
        
        /// <summary>
        /// 剔退由case_no傳入的第一條數據(有傳入RebackReason)
        /// </summary>
        /// <param name="strCase_No">傳入欄位case_no</param>
        /// <param name="strRebackReason">輸入的欄位RebackReason</param>
        /// <param name="strRtnMsg">傳出的錯誤信息</param>
        /// <returns>True--查詢成功,False--查詢失敗</returns>
        public static bool RebackBycase_No(string strCase_No, string strRebackReason, ref string strRtnMsg)
        {
            try
            {
                string strSearchSql = SEL_REBACKBY_CASE_NO;
                SqlCommand sqlcmdSearchResult = new SqlCommand();
                sqlcmdSearchResult.CommandType = CommandType.Text;
                //*加入查詢退件原因
                SqlParameter parmRebackReason = new SqlParameter("@RebackReason", strRebackReason);
                sqlcmdSearchResult.Parameters.Add(parmRebackReason);
                //*加入查詢日期
                SqlParameter parmDate = new SqlParameter("@DATE", DateTime.Today.ToString("yyyyMMdd"));
                sqlcmdSearchResult.Parameters.Add(parmDate);
                //*加入查詢時間
                SqlParameter parmTime = new SqlParameter("@TIME", DateTime.Now.ToString("HHmmss"));
                sqlcmdSearchResult.Parameters.Add(parmTime);
                //*加入當前登入者ID
                SqlParameter parmCSIUser = new SqlParameter("@CSIPUSER", ((CSIPCommonModel.EntityLayer.EntityAGENT_INFO)System.Web.HttpContext.Current.Session["Agent"]).agent_name.ToString());
                sqlcmdSearchResult.Parameters.Add(parmCSIUser);
                //加入case_no欄位
                SqlParameter parmCase_No = new SqlParameter("@Case_No", strCase_No);
                sqlcmdSearchResult.Parameters.Add(parmCase_No);
                
                sqlcmdSearchResult.CommandText = strSearchSql;
                //*查詢結果裝入DataSet
                DataSet dstSearchResult = BRPay_SV_Tmp.SearchOnDataSet(sqlcmdSearchResult);
                if (dstSearchResult == null)
                {
                    //*pay_sv_Tmp表導入pay_sv表失敗
                    strRtnMsg = "04_02010500_002";
                    return false;
                }
                else
                {
                    string strDeleteSql = DELETE_PAY_SV_TMP;                  
                    SqlCommand sqlcmdDeleteResult = new SqlCommand();
                    sqlcmdDeleteResult.CommandType = CommandType.Text;
                    //*加入case_no欄位
                    SqlParameter parmcase_no = new SqlParameter("@Case_No", strCase_No);
                    sqlcmdDeleteResult.Parameters.Add(parmcase_no);
                    sqlcmdDeleteResult.CommandText = strDeleteSql;
                    DataSet dsDeleteResult = BRPay_SV_Tmp.SearchOnDataSet(sqlcmdDeleteResult);               
                    if (dsDeleteResult == null)
                    {
                        strRtnMsg = "04_02010500_002";
                        return false;
                    }
                    return true;
                }                
            }
            catch (Exception exp)
            {
                BRPay_SV_Tmp.SaveLog(exp);
                strRtnMsg = "04_02010500_002";
                return false;
            }

            
        }
        
    }
}
