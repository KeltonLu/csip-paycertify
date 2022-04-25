//******************************************************************
//*  作    者：宋戈(Ge.Song)
//*  功能說明：新增清償證明

//*  創建日期：2009/10/26
//*  修改記錄：20220221_Ares_Jack_移除選項 GCB清償證明-卡、GCB代償證明


//*<author>            <time>            <TaskID>                <desc>
//*******************************************************************
using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using CSIPPayCertify.BusinessRules;
using Framework.Common.Message;
using Framework.Common.JavaScript;
using Framework.Common.Utility;
using CSIPPayCertify.EntityLayer;
using Framework.WebControls;
using System.Text.RegularExpressions;
using Framework.Data.OM.Collections;
using Framework.Data.OM;
using Framework.Data.OM.Transaction;
using CSIPCommonModel.EntityLayer;
using CSIPCommonModel.BaseItem;
using CSIPCommonModel.BusinessRules;

public partial class Page_P040101010001 : PageBase
{
    private string sAlertMsg = "";        //* Alert提示內容
    private string strJs = "";              //* JS內容
    private EntityAGENT_INFO eAgentInfo;
    private structPageInfo sPageInfo;//*記錄網頁訊息


    /// <summary>
    /// 頁面生成結束時

    /// </summary>
    /// <param name="writer"></param>
    protected override void Render(System.Web.UI.HtmlTextWriter writer)
    {
        string strRegScript = "";

        //if (this.sAlertMsg != "")
        //{
        //    strRegScript = strRegScript + "alert('" + sAlertMsg + "');ResetHiddenTextbox();";
        //}

        if (this.strJs != "")
        {
            strRegScript = strRegScript + strJs;
        }
        jsBuilder.RegScript(this.UpdatePanel1, strRegScript);
        base.Render(writer);
  
    }
    
    /// <summary>
    /// 載入頁面
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            //* 清空末端訊息
            jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow(""));
            //* 設置文字顯示
            ShowControlsText();
            //* 焦點預設第一個欄位

            this.txtUser_ID.Focus();            
        }
        base.strClientMsg += "";
        base.strHostMsg += "";
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"]; //*Session變數集合
        sPageInfo = (structPageInfo)this.Session["PageInfo"];
    }

    /// <summary>
    /// 點擊查詢Button 查詢主機該人是否可以開立
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        this.txtUser_ID.Text = this.txtUser_ID.Text.ToUpper();
        this.txtOtherID.Text = this.txtOtherID.Text.ToUpper();

        //------------------------------------------------------
        //AuditLog to SOC
        CSIPCommonModel.EntityLayer_new.EntityL_AP_LOG log = BRL_AP_LOG.getDefaultValue(eAgentInfo, sPageInfo.strPageCode);
        log.Customer_Id = this.txtUser_ID.Text;  
        BRL_AP_LOG.Add(log);
        log = BRL_AP_LOG.getDefaultValue(eAgentInfo, sPageInfo.strPageCode);
        log.Customer_Id = this.txtOtherID.Text;
        BRL_AP_LOG.Add(log); 
        //------------------------------------------------------

        //* 點擊查詢時就激活AJAX UpdatePanel外的按鈕點擊事件
        this.btnHiden_Click(sender, e);       
    }

    /// <summary>
    /// 隱藏提交Button點選事件
    /// 修改紀錄：調整 Redirect 參數, 避免已知錯誤(ThreadAbortException)發生 by Ares Stanley 20220407
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnHiden_Click(object sender, EventArgs e)
    {
        try
        {
            #region 宣告變數
            string strMsg = "";             //* 錯誤訊息
            string strUserID = "";          //* 客戶ID
            string strType = "";            //* 證明種類    [清償證明-卡]=C;[清償證明-ML&卡]=M;[代償證明-卡]=G;[GCB清償證明-卡]=0;[GCB代償證明]=B;[清償證明-ML Only]=N;[代償證明-ML]=T
            string strCardType = "";        //* 開立種類    [主卡]=0;[附卡]=1;[主卡+附卡]=2;[開立他人附卡]=3
            string strNo = "";              //* 卡別編號
            string strOther = "N";          //* 開立他人附卡
            string strOtherID = "";         //* 他人附卡其正卡ID
            string strUrlPara = "";         //* 傳遞的參數

            string strCardNo = "";          //* 卡號
            //int intCardNO;                  //* 卡號前7碼


            bool blAmtNotZero = false;      //* 是否餘額等於0(GCB開立如果餘額等於0則可以直接開立,否則可能要連催收畫面)
            string str1144Code = "";        //* 如果不為0代表需要連催收畫面

            bool blHadJCASCheckFail = false;//* 是否經過1572 JCAS檢查;
            string strConfirmMsg = "";

            string strCardTmpID = "";       //* CardTmp的ID
            string strCardOpenDay = "";     //* 最早開卡日
            string strCardCloseDay = "";    //* 最後停卡日

            EntitySet<EntityPay_Certify> esCertify = new EntitySet<EntityPay_Certify>();
            Hashtable htInput = new Hashtable();    //* 電文上行查詢條件欄位列表
            DataTable dtblJCEH = new DataTable();   //* 1331(JCEH)電文查詢結果
            DataTable dtblJCHN = new DataTable();   //* 1350(JCHN)電文查詢結果
            Hashtable htJCAS = new Hashtable();   //* 1572(JCAS)電文查詢結果
            ArrayList arrML2PLCardList = new ArrayList();    

            bool blPass3 = this.txtHiden3.Text.Trim() == "Y" ? true : false;    //* 是否通過檢查一年內是否有開立 Confirm
            
            bool blPass5 = this.txtHiden5.Text.Trim() == "Y" ? true : false;    //* 是否通過JCAS檢查不爲空 Confirm

            bool blCheckJCHN = false;
            bool blCheckJCEH = false;
            bool blNeedJCEHCheck = false;
            bool bInsertFlag = false;
            string sML2PLMsg = "";

            #endregion
            #region 輸入驗證
            //* 如果驗證不通過,提示后退出

            if (!CheckCondition(ref strMsg, ref strUserID, ref strType, ref strNo, ref strCardType, ref strOther, ref strOtherID))
            {
                sAlertMsg = MessageHelper.GetMessage(strMsg);
                return;
            }
            #endregion

            //* 選擇[GCB清償證明-卡]( strType = "O")或者[GCB代償證明]( strType = "B") 查1350
            if (strType == "O" || strType == "B")
            {
                #region GCB清證1350邏輯
                htInput.Clear();

                htInput.Add("FUNCTION_CODE", "1");//*ID查詢
                htInput.Add("IDNO", strUserID);
                htInput.Add("LINE_CNT", "0000");//*LINE_CNT
                htInput.Add("LAST_KEY", "");

                dtblJCHN = MainFrameInfo.GetMainframeDataByPage("P4_JCHN", htInput, false, "1", eAgentInfo);
                if (dtblJCHN.Rows[0]["HtgErrMsg"].ToString() != "") //*主機返回失敗
                {
                    etClientMstType = eClientMstType.Select;
                    if (dtblJCHN.Rows[0]["MESSAGE_TYPE"].ToString() == "8888")
                    {
                        base.strClientMsg = MessageHelper.GetMessage("04_01010100_009");
                    }
                    else
                    {
                        base.strClientMsg = MessageHelper.GetMessage("04_00000000_037");
                    }
                    base.strHostMsg = dtblJCHN.Rows[0]["HtgErrMsg"].ToString();
                    return;
                }
                else //*主機返回正確
                {
                    base.strClientMsg = MessageHelper.GetMessage("04_00000000_036");
                    base.strHostMsg = dtblJCHN.Rows[0]["MESSAGE_TYPE"].ToString() + " " + MessageHelper.GetMessage("04_00000000_036");

                    //檢查 JCHN(1350) 歸戶現狀,停掛記錄,餘額
                    blCheckJCHN = CheckJCHN(dtblJCHN, ref sAlertMsg, ref blAmtNotZero, ref blNeedJCEHCheck);
                    if (blCheckJCHN)
                    {
                        if (blNeedJCEHCheck || dtblJCHN.Rows[0]["NAME"].ToString().Trim()=="") //如果JCHN(1350) 餘額 >0 則需要檢查1331
                        {
                            htInput.Clear();
                            htInput.Add("FUNCTION_CODE", "1");//*ID查詢
                            htInput.Add("ACCT_NBR", strUserID);
                            htInput.Add("LINE_CNT", "0000");//*LINE_CNT
                            dtblJCEH = MainFrameInfo.GetMainframeDataByPage("P4_JCEH", htInput, false, "1", eAgentInfo);
                            if (dtblJCEH.Rows[0]["HtgErrMsg"].ToString() != "") //*主機返回失敗
                            {
                                etClientMstType = eClientMstType.Select;
                                if (dtblJCEH.Rows[0]["MESSAGE_TYPE"].ToString() == "8888")
                                {
                                    base.strClientMsg = MessageHelper.GetMessage("04_01010100_009");
                                }
                                else
                                {
                                    base.strClientMsg = MessageHelper.GetMessage("04_00000000_008");
                                }
                                base.strHostMsg = dtblJCEH.Rows[0]["HtgErrMsg"].ToString();
                                return;
                            }
                            else //*主機返回正確
                            {
                                base.strClientMsg = MessageHelper.GetMessage("04_00000000_034");
                                base.strHostMsg = dtblJCEH.Rows[0]["MESSAGE_TYPE"].ToString() + " " + MessageHelper.GetMessage("04_00000000_034");
                                dtblJCHN.Rows[0]["NAME"] = dtblJCEH.Rows[0]["SHORT_NAME"].ToString();
                                blCheckJCEH = CheckJCEH(dtblJCEH, ref sAlertMsg, ref sML2PLMsg,strType, ref arrML2PLCardList);
                                if (!blCheckJCEH)
                                {
                                    etClientMstType = eClientMstType.Select;
                                    base.strClientMsg = sAlertMsg;
                                    return;
                                }
                                if (this.strJs != "")
                                {
                                    return;
                                }
                            }
                        }
                    }
                    else
                    {
                        etClientMstType = eClientMstType.Select;
                        base.strClientMsg = sAlertMsg;
                        return;
                    }

                }
            }
            else //* 否則,一般清證,查1331
            {
                htInput.Clear();
                htInput.Add("FUNCTION_CODE", "1");//*ID查詢
                htInput.Add("ACCT_NBR", strUserID);
                htInput.Add("LINE_CNT", "0000");//*LINE_CNT
                dtblJCEH = MainFrameInfo.GetMainframeDataByPage("P4_JCEH", htInput, false, "1", eAgentInfo);
                if (dtblJCEH.Rows[0]["HtgErrMsg"].ToString() != "") //*主機返回失敗
                {
                    if (dtblJCEH.Rows[0]["MESSAGE_TYPE"].ToString() == "8888")
                    {
                        base.strClientMsg = MessageHelper.GetMessage("04_01010100_009");
                    }
                    else
                    {
                        base.strClientMsg = MessageHelper.GetMessage("04_00000000_008");
                    }
                    base.strHostMsg = dtblJCEH.Rows[0]["HtgErrMsg"].ToString();
                    return;
                }
                else //*主機返回正確
                {
                    base.strClientMsg = MessageHelper.GetMessage("04_00000000_034");
                    base.strHostMsg = dtblJCEH.Rows[0]["MESSAGE_TYPE"].ToString() + " " + MessageHelper.GetMessage("04_00000000_034");
                    blCheckJCEH = CheckJCEH(dtblJCEH, ref sAlertMsg, ref sML2PLMsg,strType, ref arrML2PLCardList);
                    if (!blCheckJCEH)
                    {
                        etClientMstType = eClientMstType.Select;
                        base.strClientMsg = sAlertMsg;
                        return;
                    }
                    if (this.strJs != "")
                    {
                        return;
                    }
                }

                #endregion
            }   //end 一般清證邏輯


            #region 如果是信用卡+ML(strType=M)或者全部是ML(strType = N),需要連接1572電文
            if (!blPass5)
            {
                blHadJCASCheckFail = false;
                //*	如果是信用卡+ML(strType=M)或者全部是ML(strType = N),需要連接1572電文.
                if (strType == "M" || strType == "N" || strType == "T")
                {
                    for (int j = 0; j < dtblJCEH.Rows.Count; j++)
                    {
                        strCardNo = dtblJCEH.Rows[j]["CARDHOLDER"].ToString().Trim();
                        //intCardNO = int.Parse(strCardNo.PadRight(7, Convert.ToChar("0")).Substring(0, 7));
                        if (Function.IsML(strCardNo) == 1)
                        {
                            //* 廻圈檢查每張ML的卡
                            if (CheckJCAS(strCardNo, ref blHadJCASCheckFail))
                            {
                                //若有資料則判斷 <1572電文.結清日>是否為空白，若是空白則踢退
                                if (blHadJCASCheckFail)
                                {
                                    strJs = "ConfrimText('" + MessageHelper.GetMessage("04_02010100_040") + "','txtHiden5');";
                                    return;
                                }
                            }
                            else
                            {
                                return;
                            }
                        }
                    }
                }
            }
            #endregion

            #region 3.檢查一年內是否有開立

            strConfirmMsg = "";
            if (!blPass3)
            {
                if (!BRPay_Certify.GetDetailsByUserID(strUserID, true, ref strMsg, ref esCertify))
                {
                    //* 出錯拉

                }
                if (esCertify.Count > 0)
                {
                    //* 如果一年內曾經開立過

                    //* Confirm(此客戶一年內曾開立證明,是否仍要開立??)
                    strConfirmMsg = MessageHelper.GetMessage("04_01010100_016");
                    strJs = "ConfrimText('" + strConfirmMsg + "','txtHiden3');";
                    return;
                }
            }
            #endregion

            #region 4.通過WebService連催收取得最後繳款資料

            if (!BRWebService.GetMarcoDataByCustID(strUserID, ref strMsg))
            {
                etClientMstType = eClientMstType.Select;
                base.strClientMsg = MessageHelper.GetMessages("04_01010100_017");
                strJs = "ResetHiddenTextbox();";
                return;
            }
            #endregion

            #region 5.讀取卡片資料存入卡片臨時表Pay_Card_Tmp
            if (strType == "O" || strType == "B") //strType = O(GCB清償證明卡)或者strType = B(GCB代償證明)
            {
                bInsertFlag = BRPay_Card_Temp.InsertPayCardTempDataByGCB(dtblJCHN, strUserID, strType, strCardType, strOther, strOtherID, ref strMsg, ref strCardTmpID, ref strCardOpenDay, ref strCardCloseDay);
            }
            else //一般清證
            {
                bInsertFlag = BRPay_Card_Temp.InsertPayCardTempDataByNormal(dtblJCEH, strUserID, strType, strCardType, strOther, strOtherID, ref strMsg, ref strCardTmpID, ref strCardOpenDay, ref strCardCloseDay);
            }
            if (!bInsertFlag)
            {
                //* 儲存卡片資料臨時表失敗
                etClientMstType = eClientMstType.Select;
                base.strClientMsg = MessageHelper.GetMessage(strMsg);
                strJs = "ResetHiddenTextbox();";
                return;
            }
            #endregion

            #region 6.根據Check1144不同顯示不同畫面
            strHostMsg = MessageHelper.GetMessage("04_00000000_031");

            if (!CheckJCFK(strUserID, strCardTmpID,ref str1144Code))
            {
                strJs = "ResetHiddenTextbox();";
                return;
            }

            string strML2PLCardList = ""; 
            for (int j = 0; j < arrML2PLCardList.Count; j++)
            {
                if (j == 0)
                {
                    strML2PLCardList = arrML2PLCardList[j].ToString() ;
                }
                else
                {
                    strML2PLCardList = strML2PLCardList + "," + arrML2PLCardList[j].ToString();
                }
            }

            strUrlPara = "UserID=" + RedirectHelper.GetEncryptParam(strUserID) +
                        "&Type=" + RedirectHelper.GetEncryptParam(strType) +
                        "&CardType=" + RedirectHelper.GetEncryptParam(strCardType) +
                        "&No=" + RedirectHelper.GetEncryptParam(strNo) +
                        "&Other=" + RedirectHelper.GetEncryptParam(strOther.ToString()) +
                        "&OtherID=" + RedirectHelper.GetEncryptParam(strOtherID) +
                        "&CardTmpID=" + RedirectHelper.GetEncryptParam(strCardTmpID) +
                        "&CardOpenDay=" + RedirectHelper.GetEncryptParam(strCardOpenDay) +
                        "&CardCloseDay=" + RedirectHelper.GetEncryptParam(strCardCloseDay) +
                        "&ML2PLCardList=" + RedirectHelper.GetEncryptParam(strML2PLCardList) +
                        "";
            if (strType == "O" || strType == "B")
            {
                //* GCB開立
                if (blAmtNotZero)
                {
                    //* 如果餘額不爲0
                    if (str1144Code.Trim() != "")
                    {
                        //* 顯示 催收畫面
                        Response.Redirect("P040101010003.aspx?" + strUrlPara, false);
                    }
                    else
                    {
                        //* 顯示 開立畫面
                        Response.Redirect("P040101010002.aspx?" + strUrlPara, false);
                    }
                }
                else
                {
                    //* 顯示 開立畫面
                    Response.Redirect("P040101010002.aspx?" + strUrlPara, false);
                }
            }
            else
            {
                //* 一般開立

                if (str1144Code.Trim() != "")
                {
                    //* 顯示 催收畫面
                    Response.Redirect("P040101010003.aspx?" + strUrlPara, false);
                }
                else
                {
                    //* 顯示 開立畫面
                    Response.Redirect("P040101010002.aspx?" + strUrlPara, false);
                }
            }
            #endregion
        }
        catch (Exception Ex)
        {
            Framework.Common.Logging.Logging.Log(Ex, Framework.Common.Logging.LogLayer.UI);
            strClientMsg = MessageHelper.GetMessage("04_01010100_006");
            return;
        }
    }

    /// <summary>
    /// 顯示窗體文字
    /// </summary>
    private void ShowControlsText()
    {
        //* 證明種類 Type
        this.radType_C.Text = BaseHelper.GetShowText("04_01010100_005");        //* 清償證明-卡

        this.radType_N.Text = BaseHelper.GetShowText("04_01010100_006");        //* 清償證明-ML
        this.radType_M.Text = BaseHelper.GetShowText("04_01010100_007");        //* 清償證明-卡&ML
        this.radType_G.Text = BaseHelper.GetShowText("04_01010100_008");        //* 代償證明-卡

        this.radType_T.Text = BaseHelper.GetShowText("04_01010100_009");        //* 代償證明-ML

        //* 開立種類 CardType
        this.radCardType_0.Text = BaseHelper.GetShowText("04_01010100_012");    //* 只開立正卡

        this.radCardType_1.Text = BaseHelper.GetShowText("04_01010100_013");    //* 只開立附卡

        this.radCardType_2.Text = BaseHelper.GetShowText("04_01010100_014");    //* 正卡+附卡
        this.chkOther.Text      = BaseHelper.GetShowText("04_01010100_015");    //* 開立他人附卡
        this.radCardType_3.Text = BaseHelper.GetShowText("04_01010100_016");    //* 他人附卡其正卡ID      

    }

    /// <summary>
    /// 驗證輸入欄位是否正確
    /// </summary>
    /// <param name="strMsgID">返回的錯誤ID</param>
    /// <param name="strUserID">客戶ID</param>
    /// <param name="strType">證明種類</param>
    /// <param name="strNo">卡號類型</param>
    /// <param name="strCardType">開立種類</param>
    /// <param name="strOther">開立他人附卡</param>
    /// <param name="strOtherID">他人附卡其正卡ID</param>
    /// <returns></returns>
    protected bool CheckCondition(ref string strMsgID, ref string strUserID, ref string strType, ref string strNo, ref string strCardType, ref string strOther, ref string strOtherID)
    {
        #region 客戶ID
        strUserID = this.txtUser_ID.Text.Trim();
        //* 客戶ID
        if (String.IsNullOrEmpty(strUserID))
        {
            //* "請輸入客戶ID";
            strMsgID = "04_01010100_000";
            return false;
        }
        else
        {
            //* 如果身分證不是合法的格式
            if (!Regex.IsMatch(strUserID, BaseHelper.GetShowText("04_00000000_000")))
            {
                strMsgID = "04_01010100_001";
                return false;
            }
        }
        #endregion

        #region 證明種類
        strType = "";
        strNo = "";
        //* 證明種類
        //*	如果選擇的是[清償證明-卡]       strType = "C"   strNo = "1"
        //*	如果選擇的是[清償證明-ML&卡]	strType = "M"   strNo = "1"
        //*	如果選擇的是[代償證明-卡]       strType = "G"   strNo = "2"
        //*	如果選擇的是[GCB清償證明-卡]    strType = "O"   strNo = "3"
        //*	如果選擇的是[GCB代償證明]       strType = "B"   strNo = "4"
        //*	如果選擇的是[清償證明-ML Only]  strType = "N"   strNo = "1"
        //*	如果選擇的是[代償證明-ML]       strType = "T"   strNo = "2"
        if (this.radType_C.Checked == true) { strType = "C"; strNo = "1"; }
        if (this.radType_M.Checked == true) { strType = "M"; strNo = "1"; }
        if (this.radType_G.Checked == true) { strType = "G"; strNo = "2"; }
        if (this.radType_N.Checked == true) { strType = "N"; strNo = "1"; }
        if (this.radType_T.Checked == true) { strType = "T"; strNo = "2"; }
        if (String.IsNullOrEmpty(strType.Trim()))
        {
            //"請選擇開立種類";
            strMsgID = "04_01010100_002";
            return false;
        }
        #endregion

        #region 開立種類
        strCardType = "";
        //* 開立種類
        //*	主卡: strCardType = 0
        //*	附卡: strCardType =1
        //*	主卡+附卡: strCardType = 2
        //*	開立他人附卡: strCardType = 3
        if (this.radCardType_0.Checked == true) { strCardType = "0"; }
        if (this.radCardType_1.Checked == true) { strCardType = "1"; }
        if (this.radCardType_2.Checked == true) { strCardType = "2"; }
        if (this.radCardType_3.Checked == true) { strCardType = "3"; }
        if (String.IsNullOrEmpty(strCardType.Trim()))
        {
            //"請選擇開立類型";
            strMsgID = "04_01010100_003";
            return false;
        }
        #endregion

        #region 開立他人附卡
        strOther = "N";
        strOtherID = "";
        //* 開立他人附卡
        if (this.chkOther.Checked == true) { strOther = "Y"; } else { strOther = "N"; }

        //* 如果選擇 開立種類 選擇 "開立他人附卡" 則 "他人附卡其正卡ID"必須輸入
        if (strCardType == "3" && String.IsNullOrEmpty(this.txtOtherID.Text.Trim()))
        {
            //"請輸入 他人附卡其正卡人ID!";
            strMsgID = "04_01010100_004";
            return false;
        }
        else
        {
            strOtherID = this.txtOtherID.Text.Trim();
        }
        #endregion

        return true;


    }

    //Add by Carolyn 
    protected bool CheckJCHN(DataTable dtblJCHN, ref string sAlertMsg, ref bool blAmtNotZero, ref bool blNeedJCEHCheck)
    {
        string strSTAS = dtblJCHN.Rows[0]["STAS"].ToString().Trim();
        int iBALAMT = Convert.ToInt32(dtblJCHN.Rows[0]["BALAMT"].ToString().Trim() == "" ? "0" : dtblJCHN.Rows[0]["BALAMT"].ToString().Trim());
        string strSTOPREC = "";
        
        //判斷歸戶現狀
        if (strSTAS != "3" && strSTAS != "4" && strSTAS != "5")
        {
            //*不得開立GCB證明!,重置所有隱藏欄位,退出
            sAlertMsg = MessageHelper.GetMessage("04_01010100_021", strSTAS);
            return false;
        }

        //判斷停掛記錄>第一碼 != "1"
        for (int i = 0; i <= dtblJCHN.Rows.Count - 1; i++)
        {
            strSTOPREC = dtblJCHN.Rows[i]["STOPREC"].ToString().Trim();
            if (!strSTOPREC.StartsWith("1"))
            {
                //*不得開立GCB證明!,重置所有隱藏欄位,退出
                sAlertMsg = MessageHelper.GetMessage("04_01010100_022", dtblJCHN.Rows[i]["CARDNUM"].ToString(), strSTOPREC.Substring(0, 1));
                return false;
            }
        }

        //流水帳餘額 > 0 則檢查 JCEH(1331)
        //如果流水帳餘額等於0可以直接開立,無需顯示催收畫面 (iBALAMT!=0 , blAmtNotZero=true)
        if (iBALAMT > 0)
        {
            blNeedJCEHCheck = true;
            blAmtNotZero = true;
        }
        else if (iBALAMT < 0)
        {
            blNeedJCEHCheck = false;
            blAmtNotZero = true;
        }
        else
        {
            blAmtNotZero = false;
        }

        return true;
    }

    protected bool CheckJCEH(DataTable dtblJCEH, ref string sAlertMsg, ref string sML2PLMsg , string strType, ref ArrayList arrML2PLCardList)
    {
        //輪圈檢查1331,看是否有SBAMZ狀態的卡, 或者餘額大於0的卡, 或者流通中/R管製卡
        //輪圈查詢到的1331所有資料.看是否有SBAMZ的.或者餘額大於0的
        string strHostMsg = "";
        bool blHadSBAMZ = false;
        bool blHadR = false;
        string strBLKCode = "";
        string strConfirmMsg = "";
        string strUserID = this.txtUser_ID.Text;
        string strCardNo = "";
        Hashtable htRebackReason = new Hashtable(); //* 檢核不通過的理由列表
        int intP4_JCU9 = -1;            //* 是否經過JCU9檢核
        int intP4_JCII = -1;            //* 是否經過JCII檢核
        bool blPass = false;            //* 是否通過當前這次Validation()方法檢核
        bool blAllPass = true;          //* 是否所有都通過Validation()方法檢核
        string sValidationMsg = "";
        int iMLCardCount = 0;

        //* Confirm隱藏欄位含義
        bool blPass1 = this.txtHiden1.Text.Trim() == "Y" ? true : false;    //* 是否通過<JCEH1331電文.餘額>不為0 Confirm
        bool blPass2 = this.txtHiden2.Text.Trim() == "Y" ? true : false;    //* 是否通過JCU9等卡片的驗證 Confirm
        bool blPass4 = this.txtHiden4.Text.Trim() == "Y" ? true : false;    //* 是否通過該歸戶底下 有空白 或是 R Code 時候 Confirm

        for (int i = 0; i <= dtblJCEH.Rows.Count - 1; i++)
        {
            //2010.12.17 檢查該ID/CARDNO 是否存在為 ML轉PL 的未結清戶
            if (strType == "M" || strType == "N" || strType == "T")
            {
                strCardNo = dtblJCEH.Rows[i]["CARDHOLDER"].ToString().Trim();
                if (Function.IsML(strCardNo) == 1)
                {
                    if (BRML2PL.Chk_ML2PL_Exist(strCardNo))
                    {
                        arrML2PLCardList.Add(strCardNo);
                    }
                    iMLCardCount++;
                }
            }
            //
            strBLKCode = dtblJCEH.Rows[i]["BLOCK"].ToString().Trim();
            //*如果狀況碼>欄位一筆?”S”或”B”或”A” 或”M” 或”Z”則可以開立
            if (strBLKCode == "S" ||strBLKCode == "B" || strBLKCode == "A" || strBLKCode == "M" || strBLKCode == "Z")
            {
                blHadSBAMZ = true;//* 只要有一筆SBAMZ就可以開立
            }
            else if (strBLKCode == "" || strBLKCode == "R")
            {
                blHadR = true;
            }

            //*如果<1331電文.餘額不為0,需要Confirm
            if (Convert.ToInt32((dtblJCEH.Rows[i]["CURR_BAL"].ToString().Trim() == "" ? "0" : dtblJCEH.Rows[i]["CURR_BAL"])) != 0)
            {
                //* 卡號:{0} 金額為{1},是否仍要開清償證明??
                strConfirmMsg = strConfirmMsg + @"\r\n" + MessageHelper.GetMessage("04_01010100_011", dtblJCEH.Rows[i]["CARDHOLDER"].ToString(), dtblJCEH.Rows[i]["CURR_BAL"].ToString());
            }
        }

        //若有一張卡都在 ML2PL 清單中 , 則 pop 顯示提示訊息

        if (arrML2PLCardList.Count > 0) 
        {
            for (int j = 0; j < arrML2PLCardList.Count ; j++)
            {
                if (j == 0)
                {
                    sML2PLMsg = sML2PLMsg + arrML2PLCardList[j].ToString(); 
                }
                else
                {
                    sML2PLMsg = sML2PLMsg + "、" + arrML2PLCardList[j].ToString(); 
                }
            }

            sML2PLMsg = "CARD NO : " + sML2PLMsg + " " + MessageHelper.GetMessage("04_00000000_053");

            //名下所有的卡都在 ML2PL 清單中 , 則返回
            if ((arrML2PLCardList.Count == dtblJCEH.Rows.Count) || (iMLCardCount == arrML2PLCardList.Count && (strType == "N" || strType == "T")))
            {
                sAlertMsg = sML2PLMsg;
                return false;
            }
        }

        //* 如果一筆SBAMZ都沒有就不許開立
        if (blHadSBAMZ == false)
        {
            sAlertMsg = MessageHelper.GetMessage("04_01010100_010");
            return false;
        }
        if (!blPass4)
        {
            //* 如果該歸戶底下 有空白 或是 R Code 時候 ,需在跑完全部卡號時候 詢問是否開立 
            if (blHadR == true)
            {
                strJs = strJs + "ConfrimText('" + MessageHelper.GetMessage("04_01010100_031") + "','txtHiden4');";
                return true;
            }
        }
        //* 有Confirm過就跳過此檢查邏輯
        if (!blPass1)
        {
            //* 餘額不為0,需要Confirm 如果strConfirmMsg有值代表需要Confrim確認
            if (strConfirmMsg != "")
            {
                strConfirmMsg = strConfirmMsg.Substring(4);     //* 除去最前面的\R\N
                strJs = strJs + "ConfrimText('" + strConfirmMsg + "','txtHiden1');";
                return true;
            }
        }

        //輪圈驗證1331每張卡
        strConfirmMsg = "";
        if (!blPass2)
        {
            strBLKCode = "";
            for (int i = 0; i <= dtblJCEH.Rows.Count - 1; i++)
            {
                strBLKCode = dtblJCEH.Rows[i]["BLOCK"].ToString().Trim();
                if (strBLKCode == "S" || strBLKCode == "B" || strBLKCode == "A" || strBLKCode == "M" || strBLKCode == "Z")
                {
                    //* 開始驗證
                    if (!BRPay_Card_Temp.Validation(strUserID, dtblJCEH.Rows[i]["CARDHOLDER"].ToString(), strBLKCode, dtblJCEH.Rows[i]["BLOCK_DTE"].ToString(), dtblJCEH.Rows[i]["AMC_FLAG"].ToString(), ref htRebackReason, ref sValidationMsg, ref  blPass, ref intP4_JCU9, ref intP4_JCII, eAgentInfo, ref strHostMsg))
                    {
                        etClientMstType = eClientMstType.Select;
                        base.strClientMsg = sValidationMsg;
                        base.strHostMsg = strHostMsg;
                        strJs = "ResetHiddenTextbox();";
                        return false;
                    }
                    else
                    {
                        etClientMstType = eClientMstType.NoAlert;
                        base.strClientMsg = sValidationMsg;
                        base.strHostMsg = strHostMsg;
                    }
                    blAllPass = blAllPass && blPass;
                }
            }//* 結束輪圈


            //* 如果strConfirmMsg有值代表需要Confrim確認
            if (htRebackReason.Count > 0)
            {
                for (int i = 0; i < htRebackReason.Count; i++)
                {
                    strConfirmMsg = strConfirmMsg + @"\r\n" + htRebackReason[i.ToString()].ToString();
                }
                strConfirmMsg = strConfirmMsg.Substring(4);     //* 除去最前面的\R\N
                strJs = "ConfrimText('" + strConfirmMsg + "','txtHiden2');";
                return true;
            }
            //* 驗證不通過,退出

            if (!blAllPass)
            {
                sAlertMsg = MessageHelper.GetMessage("04_02010100_032");
                strJs = "ResetHiddenTextbox();";
                return false;
            }
        }
        return true;
    }

    protected bool CheckJCAS(string strCardNo , ref bool blHadJCASCheckFail)
    {
        Hashtable htInput = new Hashtable();

        htInput.Add("FUNCTION_CODE", "1");//*ID查詢
        htInput.Add("ACCT_NBR", strCardNo);
        htInput.Add("LINE_CNT", "0000");//*LINE_CNT
        try
        {
            Hashtable htReturn = MainFrameInfo.GetMainframeData("P4_JCAS", htInput, false, "1", eAgentInfo);

            if (htReturn.Contains("HtgMsg")) //*主機返回失敗
            {

                if (htReturn["MESSAGE_TYPE"].ToString() == "8888")
                {
                    base.strClientMsg = MessageHelper.GetMessage("04_00000000_035");
                    base.strHostMsg = htReturn["HtgMsg"].ToString();
                }
                else
                {
                    etMstType = eMstType.Select;
                    base.strClientMsg = MessageHelper.GetMessage("04_02010100_031");
                    base.strHostMsg = htReturn["HtgMsg"].ToString();
                    return false;
                }
            }
            else
            {
                base.strHostMsg = htReturn["HtgSuccess"].ToString();//*主機返回成功訊息     

                if (htReturn["SETTLE_DATE"].ToString().Trim() == "" || htReturn["SETTLE_DATE"].ToString().Trim() == "00000000")
                {
                    blHadJCASCheckFail= true;
                }
                
            }

            return true;
        }
        catch (Exception exp)
        {
            Framework.Common.Logging.Logging.Log(exp, Framework.Common.Logging.LogLayer.UI);
            etClientMstType = eClientMstType.Select;
            base.strClientMsg = MessageHelper.GetMessage("04_02010100_031");
            return false;
        }
    }

    protected bool CheckJCFK(string strUserID,string strCardTempID,ref string str1144Code)
    {
        string strCurrentCode = "";
        
        Hashtable htInput = new Hashtable();
        DataTable dtblOutput = new DataTable();

        htInput.Add("FUNCTION_CODE", "1");//*ID查詢
        htInput.Add("ACCT_NBR", strUserID);
        htInput.Add("LINE_CNT", "0000");//*LINE_CNT
        try
        {
            EntityPay_Card_TempSet esPayCardTemp = new EntityPay_Card_TempSet();

            dtblOutput = MainFrameInfo.GetMainframeDataByPage("P4_JCFK", htInput, false, "1", eAgentInfo);

            if (dtblOutput.Rows[0]["HtgErrMsg"].ToString() != "") //*主機返回失敗
            {

                etMstType = eMstType.Select;
                base.strClientMsg = MessageHelper.GetMessage("04_00000000_005");
                base.strHostMsg = dtblOutput.Rows[0]["HtgErrMsg"].ToString();
                return false;
            }
            else
            {
                base.strClientMsg = MessageHelper.GetMessage("04_00000000_031");
                base.strHostMsg = dtblOutput.Rows[0]["MESSAGE_TYPE"].ToString() + " " + MessageHelper.GetMessage("04_00000000_031");    

                for (int i = 0; i < dtblOutput.Rows.Count; i++)
                {
                    strCurrentCode = dtblOutput.Rows[i]["CURRENT_CODE"].ToString();
                    if (strCurrentCode == MessageHelper.GetMessage("04_00000000_046"))
                    {
                        str1144Code = "M";
                        return true;
                    }
                    else if (strCurrentCode == MessageHelper.GetMessage("04_00000000_047"))
                    {
                        esPayCardTemp.Clear();

                        SqlHelper sql = new SqlHelper();

                        sql.AddCondition(EntityPay_Card_Temp.M_CARDTEMP_ID, Operator.Equal, DataTypeUtils.Double, strCardTempID);

                        sql.AddCondition(EntityPay_Card_Temp.M_CODE2, Operator.Equal, DataTypeUtils.String, "M");

                        esPayCardTemp.FillEntitySet(sql.GetFilterCondition());

                        if (esPayCardTemp.Count > 0)
                        {
                            str1144Code = "M";
                            return true;
                        }
                    }
                }

                return true;
            }
        }
        catch (Exception exp)
        {
            Framework.Common.Logging.Logging.Log(exp, Framework.Common.Logging.LogLayer.UI);
            etClientMstType = eClientMstType.Select;
            base.strClientMsg = MessageHelper.GetMessage("00_00000000_000");
            return false;
        }

    }
}
