//******************************************************************
//*  作    者：宋戈
//*  功能說明：
//*  創建日期：2009/11/09
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
//*******************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using CSIPCommonModel.BusinessRules;
using CSIPPayCertify.EntityLayer;
using System.Data;
using System.Collections;
using Framework.Data.OM;
using Framework.Data.OM.Collections;
using System.Data.SqlClient;
using Framework.Data.OM.Transaction;
using Framework.Common.Message;
using CSIPCommonModel.BaseItem;
using CSIPCommonModel.EntityLayer;

namespace CSIPPayCertify.BusinessRules
{
    public class BRPay_Card_Temp : BRBase<EntityPay_Card_Temp>
    {

        #region sql語句

        public const string SEL_MAX_CardTempID = @"SELECT ISNULL(MAX(CardTemp_ID) + 1,1) AS CardTempID FROM Pay_Card_Temp";

        #endregion

        /// <summary>
        /// 產生Pay_Card_Temp序號
        /// </summary>
        /// <param name="strCardTempID">Pay_Card_Temp序號</param>
        /// <param name="strMsgID">錯誤信息ID</param>
        /// <returns>True---查詢成功；False----查詢失敗</returns>
        public static bool GetCardTempID(ref String strCardTempID, ref string strMsgID)
        {
            DataSet dstResult = BRPay_Card_Temp.SearchOnDataSet(SEL_MAX_CardTempID);
            if (dstResult == null)
            {
                strMsgID = "04_02010100_022";
                return false;
            }
            else
            {
                strMsgID = "04_02010100_021";
                strCardTempID = dstResult.Tables[0].Rows[0]["CardTempID"].ToString();
                return true;
            }
        }

        /// <summary>
        /// 通過UserID從主機取得卡的信息存入Pay_Card_Temp
        /// </summary>
        /// <param name="dtblJCHN">JCHN 查詢結果</param>
        /// <param name="strUserID">客戶ID</param>
        /// <param name="strType">證明種類</param>
        /// <param name="strCardType">開立種類</param>
        /// <param name="strOther">是否開立他人附卡</param>
        /// <param name="strMainCard4E">他人附卡其正卡ID</param>
        /// <param name="strMsgID">末端錯誤訊息ID</param>
        /// <param name="strCardTmpID">儲存后作為標識的CardTempID</param>
        /// <param name="strCardOpenDay">最早開卡日</param>
        /// <param name="strCardCloseDay">最晚付款日</param>
        /// <returns>True   -   成功;Flase  -   失敗</returns>
        public static bool InsertPayCardTempDataByGCB(DataTable dtblJCHN, string strUserID, string strType, string strCardType, string strOther, string strMainCard4E, ref string strMsgID, ref string strCardTmpID, ref string strCardOpenDay, ref string strCardCloseDay)
        {
            strMainCard4E = strMainCard4E.Trim();
            try
            {
                #region 宣告變數
                strCardOpenDay = "";                    //* 最早開卡日
                strCardCloseDay = "";                   //* 最后繳款日
                strCardTmpID = "";                      //* 卡片臨時表ID
                bool blFirst = true;           
                EntityPay_Card_Temp ePayCardTemp=new EntityPay_Card_Temp();       //* 卡片臨時表                string strJCHN_CARDNUM= "";                string strJCHN_CARDDTE= "";                string strConvert_CARDDTE= "";                string strJCHN_CUSTID= "";                #endregion

                //* 如果取得卡片臨時表失敗,返回()
                if (!BRPay_Card_Temp.GetCardTempID(ref strCardTmpID, ref strMsgID)) {  return false; }

                ePayCardTemp.CARDTEMP_ID = Convert.ToInt32(strCardTmpID);

                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    for (int i = 0; i < dtblJCHN.Rows.Count; i++)
                    {
                        strJCHN_CARDNUM = dtblJCHN.Rows[i]["CARDNUM"].ToString().Trim();
                        strJCHN_CARDDTE = dtblJCHN.Rows[i]["CARDDTE"].ToString().Trim();
                        strConvert_CARDDTE = ConvertLPAYD(strJCHN_CARDDTE);
                        strJCHN_CUSTID = dtblJCHN.Rows[i]["CUSTID"].ToString().Trim();

                        if (dtblJCHN.Rows[i]["STOPREC"].ToString().Trim().StartsWith("1"))
                        {
                            //* 如果停掛記錄 = 1則進行以下邏輯
                            //* 2010/02/25 應要求改為以1開始即可
                            #region 最早開卡日和最後停卡日
                            int iIsML = Function.IsML(strJCHN_CARDNUM);
                            if (iIsML != 1) 
                            {
                                //* 卡+ML的 所有, CardOnly的非ML, ML Only的ML才會計算最後日期
                                //* 如果是第一張卡
                                if (blFirst)
                                {
                                    blFirst = false;
                                    //* strCardOpenDay = <1350電文.發卡日>
                                    strCardOpenDay = strConvert_CARDDTE;
                                }
                                else
                                {
                                    //* strCardOpenDay = strCardOpenDay與<1350電文.發卡日>的小者(最早發卡日)
                                    if (Convert.ToInt32((strCardOpenDay.Trim() == "" ? "0" : strCardOpenDay))
                                        > Convert.ToInt32((strConvert_CARDDTE == "" ? "0" : strConvert_CARDDTE)))
                                    {
                                        strCardOpenDay = strConvert_CARDDTE;
                                    }
                                }
                            }
                            #endregion

                            #region 插資料
                            //* strCardOpenDay = <1350電文.停卡日>
                            strCardCloseDay = ConvertLPAYD(dtblJCHN.Rows[0]["LPAYD"].ToString().Trim());

                            ePayCardTemp.CODE1 = "";
                            ePayCardTemp.CLOSEDAY1 = "";
                            ePayCardTemp.CODE2 = "";
                            ePayCardTemp.CLOSEDAY2 = "";
                            ePayCardTemp.TYPECARD = "";
                            ePayCardTemp.SOLD = " ";
                            ePayCardTemp.VALID = "Y";
                            ePayCardTemp.LASTPAYDATE = strCardCloseDay;        //* LPAYD只有一筆
                            ePayCardTemp.CARDNO = strJCHN_CARDNUM;
                            ePayCardTemp.CUSID = dtblJCHN.Rows[i]["ALID"].ToString().Trim();
                            ePayCardTemp.CUSNAME = dtblJCHN.Rows[0]["NAME"].ToString().Trim();      //* NAME只有一筆所以都是取0的                            ePayCardTemp.OPENDAY = strConvert_CARDDTE;

                                if (ePayCardTemp.CUSID != strJCHN_CUSTID)
                                {
                                    //* //如果<1350電文.卡人ID> 不等於 <1350電文.歸戶ID>
                                    if (strOther == "Y" || strCardType == "3")
                                    {
                                        //* 如果選擇了開立他人附卡 
                                        if (strJCHN_CUSTID == strMainCard4E)
                                        {
                                            //* 如果<1350電文.歸戶ID> = strMainCard4E ([他人附卡其正卡ID](TextBox)), 則eCardTmp.TypeCard = S
                                            ePayCardTemp.TYPECARD = "S";
                                        }
                                        else
                                        {
                                            //* 否則eCardTmp.TypeCard = E
                                            ePayCardTemp.TYPECARD = "E";
                                        }
                                    }
                                    else
                                    {
                                        if (strUserID == strJCHN_CUSTID)
                                        {
                                            //* 如果<1350電文.卡號> = <1350電文.歸戶ID>則 eCardTmp.TypeCard = E 
                                            ePayCardTemp.TYPECARD = "E";
                                        }
                                    }
                                }
                                else
                                {
                                    if (!strJCHN_CARDNUM.StartsWith("1"))
                                    {
                                        //* 如果<1350電文.卡號>第1碼不等於1, eCardTmp.TypeCard = M
                                        ePayCardTemp.TYPECARD = "M";
                                    }
                                    else
                                    {
                                        //* 否則eCardTmp.TypeCard = L
                                        ePayCardTemp.TYPECARD = "L";
                                    }
                                }
                            }

                            if(!String.IsNullOrEmpty(ePayCardTemp.TYPECARD))
                            {
                                if (!BRPay_Card_Temp.AddNewEntity(ePayCardTemp))
                                {
                                    strMsgID = "04_01010100_023";
                                    return false;
                                }
                            }

                            #endregion
                        }//* end 回圈
                        ts.Complete();
                    }//* end 事務

            }
            catch (Exception Ex)
            {
                BRPay_Card_Temp.SaveLog(Ex);
                strMsgID = "04_01010100_023";
                return false;
            }
            return true;
        }

        //* 一般開立
        public static bool InsertPayCardTempDataByNormal(DataTable dtblJCEH, string strUserID, string strType, string strCardType, string strOther, string strMainCard4E, ref string strMsgID, ref string strCardTmpID, ref string strCardOpenDay, ref string strCardCloseDay)
        {
            strMainCard4E = strMainCard4E.Trim();
            try
            {
                #region 宣告變數
                string strTempCloseDay = "";            //* 臨時繳款日

                string strBLKDate = "";                 //* 狀況日
                string strAltBLKDate = "";              //* 前狀況日
                strCardOpenDay = "";                    //* 最早開卡日
                strCardCloseDay = "";                   //* 最后繳款日
                strCardTmpID = "";                      //* 卡片臨時表ID
                bool blFirst = true;
                EntityPay_Card_Temp ePayCardTemp = new EntityPay_Card_Temp();       //* 卡片臨時表

                #endregion

                //* 如果取得卡片臨時表失敗,返回()
                if (!BRPay_Card_Temp.GetCardTempID(ref strCardTmpID, ref strMsgID)) { return false; }

                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    //* 找得到

                    for (int i = 0; i < dtblJCEH.Rows.Count; i++)
                    {
                        //*如果<1331電文.狀況碼>欄位不?”S”或”B”或”A” 或”M” 或”Z”則需要剔退
                        if (dtblJCEH.Rows[i]["BLOCK"].ToString() != "S" &&
                           dtblJCEH.Rows[i]["BLOCK"].ToString() != "B" &&
                           dtblJCEH.Rows[i]["BLOCK"].ToString() != "A" &&
                           dtblJCEH.Rows[i]["BLOCK"].ToString() != "M" &&
                           dtblJCEH.Rows[i]["BLOCK"].ToString() != "Z")
                        {
                            //* 跳過此次輪圈
                            continue;
                        }
                        else
                        {
                            #region 最早開卡日和最後停卡日
                            int iIsML = Function.IsML(dtblJCEH.Rows[i]["CARDHOLDER"].ToString().Trim());
                            if ((strType == "M") ||
                                ((strType == "C" || strType == "G") && iIsML != 1) ||
                                ((strType == "N" || strType == "T") && iIsML == 1)
                                )
                            {
                                //* 卡+ML的 所有, CardOnly的非ML, ML Only的ML才會計算最後日期

                                strBLKDate = ConvertBLKDate(dtblJCEH.Rows[i]["BLOCK_DTE"].ToString().Trim());
                                strBLKDate = (strBLKDate == "" ? "0" : strBLKDate);
                                strAltBLKDate = ConvertBLKDate(dtblJCEH.Rows[i]["ALT_BLOCK_DTE"].ToString().Trim());
                                strAltBLKDate = (strAltBLKDate == "" ? "0" : strAltBLKDate);

                                //* StrTempCloseDay = <1331電文.狀況碼日期>與<1331電文.前況碼日期>的大者

                                if (Convert.ToInt32(strBLKDate) >= Convert.ToInt32(strAltBLKDate))
                                {
                                    strTempCloseDay = strBLKDate;
                                }
                                else
                                {
                                    strTempCloseDay = strAltBLKDate;
                                }

                                //* 如果是第一張卡
                                if (blFirst)
                                {
                                    blFirst = false;
                                    //* strCardOpenDay = <1331電文.發卡日>
                                    strCardOpenDay = dtblJCEH.Rows[i]["OPENED"].ToString();
                                    //* strCardCloseDay = StrTempCloseDay
                                    strCardCloseDay = strTempCloseDay;
                                }
                                else
                                {
                                    //* strCardOpenDay = strCardOpenDay與<1331電文.發卡日>的小者(最早發卡日)
                                    if (Convert.ToInt32((strCardOpenDay.Trim() == "" ? "0" : strCardOpenDay))
                                        > Convert.ToInt32((dtblJCEH.Rows[i]["OPENED"].ToString().Trim() == "" ? "0" : dtblJCEH.Rows[i]["OPENED"].ToString())))
                                    {
                                        strCardOpenDay = dtblJCEH.Rows[i]["OPENED"].ToString();
                                    }
                                    //* strCardCloseDay = strCardCloseDay與StrTempCloseDay的大者(最後停卡日)
                                    if (Convert.ToInt32((strCardCloseDay.Trim() == "" ? "0" : strCardCloseDay))
                                        < Convert.ToInt32((strTempCloseDay.Trim() == "" ? "0" : strTempCloseDay)))
                                    {
                                        strCardCloseDay = strTempCloseDay;
                                    }
                                }
                            }
                            #endregion

                            #region 插資料


                            //* 只有1筆所以取0的

                            ePayCardTemp.CARDTEMP_ID = Convert.ToInt32(strCardTmpID);
                            ePayCardTemp.TYPECARD = "";

                            ePayCardTemp.VALID = "N";


                            ePayCardTemp.CARDNO = dtblJCEH.Rows[i]["CARDHOLDER"].ToString().Trim();
                            ePayCardTemp.CUSID = dtblJCEH.Rows[i]["CUSTID"].ToString().Trim();
                            ePayCardTemp.CUSNAME = dtblJCEH.Rows[i]["CARDHOLDER_NAME"].ToString().Trim();

                            ePayCardTemp.OPENDAY = dtblJCEH.Rows[i]["OPENED"].ToString().Trim();
                            ePayCardTemp.CODE1 = dtblJCEH.Rows[i]["BLOCK"].ToString().Trim();
                            ePayCardTemp.CLOSEDAY1 = (strBLKDate == "0" ? "" : strBLKDate);
                            ePayCardTemp.CODE2 = dtblJCEH.Rows[i]["ALT_BLOCK"].ToString().Trim();
                            ePayCardTemp.CLOSEDAY2 = (strAltBLKDate == "0" ? "" : strAltBLKDate);
                            ePayCardTemp.SOLD = dtblJCEH.Rows[0]["AMC_FLAG"].ToString().Trim();

                            ePayCardTemp.LASTPAYDATE = dtblJCEH.Rows[i]["DTE_LST_PYMT"].ToString().Trim();
                            //* 如果選擇的是[清償證明ML Only]則

                            if (strType == "N")
                            {
                                //* (開立本人) ,如果<1331電文.卡人ID> 等於<1331電文.歸戶ID>
                                if (ePayCardTemp.CUSID == dtblJCEH.Rows[i]["ACCT_CUST_ID"].ToString().Trim())
                                {
                                    //* 20100226 取消123408判斷
                                    if (ePayCardTemp.CARDNO.StartsWith("123408"))
                                    {
                                        //* 如果<1331電文.卡號>前6碼=123408(ML)則eCardTmp.TypeCard = M
                                        ePayCardTemp.TYPECARD = "M";
                                    }
                                    else
                                    {
                                        if (ePayCardTemp.CARDNO.StartsWith("1"))
                                        {

                                            //* 否則如果<1331電文.卡號>第1碼=1則eCardTmp.TypeCard = L
                                            ePayCardTemp.TYPECARD = "L";

                                        }
                                    }

                                }
                            }
                            else
                            {
                                if (ePayCardTemp.CUSID != dtblJCEH.Rows[i]["ACCT_CUST_ID"].ToString().Trim())
                                {
                                    //* (開立附卡) 如果<1331電文.卡人ID> 不等於 <1331電文.歸戶ID>
                                    if (strOther == "Y" || strCardType == "3")
                                    {
                                        //(開立他人附卡 )如果選擇了[開立他人附卡](CheckBox) OR[他人附卡其正卡ID](Radio)
                                        if (dtblJCEH.Rows[i]["ACCT_CUST_ID"].ToString().Trim() == strMainCard4E)
                                        {
                                            //如果<1331電文.歸戶ID> = [他人附卡其正卡ID](TextBox), 則eCardTmp.TypeCard = S
                                            ePayCardTemp.TYPECARD = "S";
                                        }
                                        else
                                        {
                                            //否則eCardTmp.TypeCard = E
                                            ePayCardTemp.TYPECARD = "E";
                                        }
                                    }
                                    else if (strUserID == dtblJCEH.Rows[i]["ACCT_CUST_ID"].ToString().Trim())
                                    {
                                        //* (開立ML)否則如果[客戶ID] = <1331電文.歸戶ID>
                                        if (ePayCardTemp.CUSID == dtblJCEH.Rows[i]["ACCT_CUST_ID"].ToString().Substring(1).Trim())
                                        {
                                            //* 如果<1331電文.卡人ID> = <1331電文.歸戶ID>第2碼之後的字串eCardTmp.TypeCard = M
                                            ePayCardTemp.TYPECARD = "M";
                                        }
                                        else
                                        {
                                            //* 否則eCardTmp.TypeCard = E
                                            ePayCardTemp.TYPECARD = "E";
                                        }
                                    }
                                }
                                else
                                {
                                    //* 20100226 取消123408判斷
                                    //否則(開立主卡)
                                    if (ePayCardTemp.CARDNO.StartsWith("123408"))
                                    {
                                        //* (開立ML)如果<1331電文.卡號>前6碼=123408則eCardTmp.TypeCard = M
                                        ePayCardTemp.TYPECARD = "M";
                                    }
                                    else
                                    {
                                        if (!ePayCardTemp.CARDNO.StartsWith("1"))
                                        {
                                            //* 否則如果<1331電文.卡號>第1碼不等於eCardTmp.TypeCard = M
                                            ePayCardTemp.TYPECARD = "M";
                                        }
                                        else
                                        {
                                            //* 否則eCardTmp.TypeCard = L
                                            ePayCardTemp.TYPECARD = "L";
                                        }
                                    }
                                }
                            }
                            if (!String.IsNullOrEmpty(ePayCardTemp.TYPECARD))
                            {
                                if (!BRPay_Card_Temp.AddNewEntity(ePayCardTemp))
                                {
                                    strMsgID = "04_01010100_023";
                                    return false;
                                }
                            }
                            #endregion
                        }
                    }//* end 回圈
                    ts.Complete();
                }//* end 事務

            }
            catch (Exception Ex)
            {
                BRPay_Card_Temp.SaveLog(Ex);
                strMsgID = "04_01010100_023";
                return false;
            }
            return true;
        }

        /// <summary>
        /// 验证卡是否符合要求
        /// </summary>
        /// <param name="strCustID">客戶ID</param>
        /// <param name="strCardNo">卡號</param>
        /// <param name="strStatusCode">狀況碼</param>
        /// <param name="strStautsDate">狀況碼日期</param>
        /// <param name="strSold">不良資產是否已出售</param>
        /// <param name="htRebackReason">需要Confirm的內容列表</param>
        /// <param name="strMsgID">主機訊息</param>
        /// <param name="blPass">是否通過</param>
        /// <param name="intP4_JCU9">是否已經過1913(JCU9)電文檢驗</param>
        /// <param name="intP4_JCII">是否已經過1615(JCII)電文檢驗</param>
        /// <returns>True   -   成功;   False   -   失敗</returns>
        public static bool Validation(string strCustID, string strCardNo, string strStatusCode, string strStautsDate, string strSold, ref Hashtable htRebackReason, ref string strAlertMsg, ref bool blPass, ref int intP4_JCU9, ref int intP4_JCII,EntityAGENT_INFO eAgentInfo,ref string strHostMsg)
        {
            blPass = true;                          //* 是否驗證通過
            string strMsg = "";                     //* 錯誤訊息
            Hashtable htInput = new Hashtable();    //* 電文傳入參數
            DataTable dtblOutput = new DataTable(); //* 電文返回結果集

            //* 要傳回的欄位集合
            //* 查詢起始筆數,未到期餘額
            try
            {
                //* strStautsDate和當前日期的天數差
                strStautsDate = strStautsDate.Trim().Substring(4, 4) + @"/" + strStautsDate.Trim().Substring(0, 2) + @"/" + strStautsDate.Trim().Substring(2,2);
                int intDateDiff = int.Parse(Function.DateDiff("d", DateTime.Parse(strStautsDate), DateTime.Now).ToString());

                #region BLKCode = Z
                if (strStatusCode == "Z")
                {
                    if (strSold != "")
                    {
                        //* 如果strStatusCode = Z,如果strSold欄位不等於空
                        //*  Confirm(請確認 卡號: strCardNo +(換行) +不良資產已出售!是否仍要開清償證明??)
                        htRebackReason.Add(htRebackReason.Count.ToString(), MessageHelper.GetMessage("04_01010100_012", strCardNo));
                        blPass = false;
                    }
                }
                #endregion

                #region BLKCode = M
                if (strStatusCode == "M")
                {
                    if (intDateDiff <= 7)
                    {
                        //* 如果strStautsDate和今天DateDiff小於等於7天
                        //* Confirm(請確認 卡號: strCardNo 已灌入MACRO!是否仍然要開立清償證明??)
                        htRebackReason.Add(htRebackReason.Count.ToString(), MessageHelper.GetMessage("04_01010100_013", strCardNo));
                        blPass = false;
                    }
                    else if (strSold != "")
                    {
                        //* 如果strSold欄位不等於空
                        //* Confirm(請確認 卡號: strCardNo 不良資產已出售!是否仍要開清償證明??)
                        htRebackReason.Add(htRebackReason.Count.ToString(), MessageHelper.GetMessage("04_01010100_012", strCardNo));
                        blPass = false;
                    }
                }
                #endregion

                #region BLKCode = S,B,A
                if ((strStatusCode == "S" || strStatusCode == "B" || strStatusCode == "A") && intDateDiff <= 6000)
                {
                    #region JCU9(1913)
                    //* 如果strStautsDate和今天DateDiff小於等於6000天
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
                                strAlertMsg = MessageHelper.GetMessage("04_01010100_009");
                                strHostMsg = dtblOutput.Rows[0]["HtgErrMsg"].ToString();
                                intP4_JCU9 = 0;
                            }
                            else
                            {
                                SaveLog(strMsg);
                                strAlertMsg = MessageHelper.GetMessage("04_00000000_006");
                                strHostMsg = dtblOutput.Rows[0]["HtgErrMsg"].ToString();
                                return false;
                            }
                        }
                        else //*主機返回正確
                        {
                            strAlertMsg = MessageHelper.GetMessage("04_00000000_032");
                            strHostMsg = dtblOutput.Rows[0]["MESSAGE_TYPE"].ToString() + " " + MessageHelper.GetMessage("04_00000000_032");

                            if (dtblOutput.Rows[0]["ORDER_BAL"].ToString() != "")
                            {
                                //*如果<1913電文.未到期餘額>不爲空
                                //* Confirm(卡號: strCardNo 分期或 BT 月費及 40000160(帳款分期金)是否尚未拋帳!(from 1913) 是否仍要開清償證明??))
                                htRebackReason.Add(htRebackReason.Count.ToString(), MessageHelper.GetMessage("04_01010100_015", strCardNo));
                                intP4_JCU9 = 1;
                                blPass = false;
                            }
                            else
                            {
                                intP4_JCU9 = 0;
                            }
                        }
                    }
                    #endregion
                    #region JCII(1615)
                    if (intP4_JCU9 == 0)
                    {
                        //*如果<1913電文.未到期餘額>爲空
                        if (intP4_JCII == -1)
                        {
                            //*還沒有查過1615的,開始查1615
                            htInput.Clear();
                            dtblOutput.Clear();

                            htInput.Add("FUNCTION_CODE", "1");//*ID查詢
                            htInput.Add("ACCT_NBR", strCustID);
                            htInput.Add("LINE_CNT", "0000");//*LINE_CNT

                            dtblOutput = MainFrameInfo.GetMainframeDataByPage("P4_JCII", htInput, false, "1", eAgentInfo);
                            if (dtblOutput.Rows[0]["HtgErrMsg"].ToString() != "") //*主機返回失敗
                            {

                                //if (dtblOutput.Rows[0]["MESSAGE_TYPE"].ToString() == "8888")
                                //{
                                //    strAlertMsg = MessageHelper.GetMessage("04_01010100_009");
                                //    strHostMsg = dtblOutput.Rows[0]["HtgErrMsg"].ToString();
                                //    intP4_JCII = 0;
                                //}
                                //else
                                //{
                                //    SaveLog(strMsg);
                                //    strHostMsg = dtblOutput.Rows[0]["HtgErrMsg"].ToString();
                                //    strAlertMsg = MessageHelper.GetMessage("04_02010100_032") + strHostMsg;
                                //    return false;
                                //}

                                intP4_JCII = 0;

                            }
                            else //*主機返回正確
                            {
                                strAlertMsg = MessageHelper.GetMessage("04_00000000_033");
                                strHostMsg = dtblOutput.Rows[0]["MESSAGE_TYPE"].ToString() + " " + MessageHelper.GetMessage("04_00000000_033");

                                intP4_JCII = 1;
                                for (int iRow = 0; iRow < dtblOutput.Rows.Count; iRow++)
                                {
                                    //*如果1615電文找的到資料且未出帳總金額不為0
                                    if (dtblOutput.Rows[iRow]["STATUS"].ToString().Trim() == "N" &&
                                        (int.Parse(dtblOutput.Rows[iRow]["UNBILLED_AMT"].ToString() == "" ? "0" : dtblOutput.Rows[iRow]["UNBILLED_AMT"].ToString()) != 0))
                                    {
                                        //* Confirm(卡號: strCardNo + (換行) + 分期或 BT 月費及 40000160(帳款分期金)是否尚未拋帳!(from 1615) + (換行) + 是否仍要開清償證明??)
                                        htRebackReason.Add(htRebackReason.Count.ToString(), MessageHelper.GetMessage("04_01010100_014", strCardNo));
                                        blPass = false;
                                        break;
                                    }
                                }                                
                            }
                        }                        
                    }
                    if (intP4_JCII == 0)
                    {
                        if (strSold != "")
                        {
                            htRebackReason.Add(htRebackReason.Count.ToString(), MessageHelper.GetMessage("04_01010100_012", strCardNo));
                            blPass = false;
                        }
                    }
                    #endregion
                }
                #endregion

            }
            catch (Exception Ex)
            {
                BRPay_Card_Temp.SaveLog(Ex);
                strAlertMsg = MessageHelper.GetMessage("00_00000000_000");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 轉換BlockDate和altBlockDate
        /// </summary>
        /// <param name="strBLKDate"></param>
        /// <returns></returns>
        public static string ConvertBLKDate(string strBLKDate)
        {
            if (string.IsNullOrEmpty(strBLKDate) || strBLKDate == "00000000")
            {
                return "";
            }
            else
            {
                if (strBLKDate.Length >= 8)
                {
                    return strBLKDate.Substring(4, 4) + strBLKDate.Substring(0, 2) + strBLKDate.Substring(2, 2);
                }
                else
                {
                    return strBLKDate;
                }
            }
        }

        /// <summary>
        /// 取得某Pay_Card_Temp序號下最後繳款日
        /// </summary>
        /// <param name="strCardTempID"></param>
        /// <returns></returns>
        public static string GetLasyPayDate(string strCardTempID,string strType)
        {
            int iLastPayDate = 0;
            int iThis = 0;
            SqlHelper objSqlHelper = new SqlHelper();
            EntitySet<EntityPay_Card_Temp> esPayCardTemp = new EntitySet<EntityPay_Card_Temp>();
            objSqlHelper.AddCondition(EntityPay_Card_Temp.M_CARDTEMP_ID, Operator.Equal, DataTypeUtils.String, strCardTempID);
            esPayCardTemp = (EntitySet<EntityPay_Card_Temp>)BRPay_Card_Temp.Search(objSqlHelper.GetFilterCondition());
            int iIsML;
            for (int i = 0; i < esPayCardTemp.Count; i++)
            {
                iIsML = Function.IsML(esPayCardTemp.GetEntity(i).CARDNO);
                if ((strType == "M") ||
                    ((strType == "C" || strType == "G" || strType == "O" || strType == "B") && iIsML != 1) ||
                    ((strType == "N" || strType == "T") && iIsML == 1))
                {
                    if (string.IsNullOrEmpty(esPayCardTemp.GetEntity(i).LASTPAYDATE.Trim()) || esPayCardTemp.GetEntity(i).LASTPAYDATE.Trim() == "00000000")
                    {
                        iThis = 0;
                    }
                    else
                    {
                        iThis = Convert.ToInt32(esPayCardTemp.GetEntity(i).LASTPAYDATE.Trim());
                    }

                    if (iThis > iLastPayDate)
                    {
                        iLastPayDate = iThis;
                    }
                }
            }

            if (iLastPayDate == 0)
            {
                return "";
            }
            else
            {
                return iLastPayDate.ToString();
            }

        }

        /// <summary>
        /// 將YYMMDD格式的民國年轉化爲YYYYMMDD格式的西元年(如921001->20031001)
        /// </summary>
        /// <param name="strLPAYD">民國日期</param>
        /// <returns></returns>
        public static string ConvertLPAYD(string strLPAYD)
        {
            int iTmp = 0;
            if (string.IsNullOrEmpty(strLPAYD)) { return ""; }
            int.TryParse(strLPAYD,out iTmp);
            if (iTmp == 0)
            {
                return "";
            }
            else
            {
                return (iTmp + 19110000).ToString();
            }                      
        }
    }

}
