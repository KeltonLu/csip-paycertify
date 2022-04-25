﻿//******************************************************************
//*  作    者：rosicky(yangyu)
//*  功能說明：債清與開立
//*  創建日期：2009/11/16
//*  修改記錄：
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
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Framework.Common.JavaScript;
using CSIPPayCertify.BusinessRules;
using CSIPPayCertify.EntityLayer;
using CSIPCommonModel.EntityLayer;
using CSIPCommonModel.BaseItem;
using Framework.Data.OM;
using Framework.Data.OM.Collections;
using Framework.Data.OM.Transaction;
using Framework.Common.Message;
using Framework.Common.Utility;
using Framework.Common.Logging;
using Framework.WebControls;
// JaJa暫時移除
// using CrystalDecisions.CrystalReports.Engine;

public partial class Page_P040201010001 : PageBase
{

     private string strJs = "";              //* 需要注冊的JS
    private EntityAGENT_INFO eAgentInfo;
    #region Event

    /// <summary>
    /// 頁面展現時顯示confirm,alert,末端訊息
    /// </summary>
    /// <param name="writer"></param>
    protected override void Render(System.Web.UI.HtmlTextWriter writer)
    {
        string strRegScript = "";

        if (this.strJs != "")
        {
            strRegScript = strRegScript + strJs;
        }
        jsBuilder.RegScript(this.UpdatePanel1, strRegScript);
        base.Render(writer);
    }

    /// <summary>
    /// 頁面載入
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow(""));
            //* 顯示窗體文字
            Show();
            //* 刷新列表
            BindGridView();
        }
        base.strClientMsg += "";
        base.strHostMsg += "";
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"]; //*Session變數集合
    }

    /// <summary>
    /// 確定剔退
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnReject_Click(object sender, EventArgs e)
    {
        Reject();
        BindGridView();
    }

    /// <summary>
    /// 開立清證
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSave_Click(object sender, EventArgs e)
    {
        Save();
        BindGridView();
    }

    /// <summary>
    /// 批次驗證
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnAllValidate_Click(object sender, EventArgs e)
    {
        //AllValidate();
        AllValidate();
        BindGridView();
    }

    /// <summary>
    /// 綁定剔退列表時顯示後面的單筆驗證link
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void grvRejectDetail_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        LinkButton lbtnValidate;
        Label olabel;
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            lbtnValidate = (LinkButton)e.Row.FindControl("lbtnValidate");
            lbtnValidate.Text = BaseHelper.GetShowText("04_02010100_016");

            olabel = (Label)e.Row.Cells[0].FindControl("lblRejectNo");
            olabel.Text = Convert.ToString(this.grvRejectDetail.Rows.Count + 1);
        }
    }

    /// <summary>
    /// 點擊剔退列表後面的[單筆驗證]按鈕
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void grvRejectDetail_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        this.txthidCaseNo.Text = grvRejectDetail.DataKeys[e.RowIndex]["case_no"].ToString();
        if (this.txthidCaseNo.Text.Trim() != "")
        {
            CheckOne(this.txthidCaseNo.Text.Trim());
            BindGridView();
        }
    }

    /// <summary>
    /// 綁定Grid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void grvAddDetail_DataBound(object sender, GridViewRowEventArgs e)
    {
        Label olabel;
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (e.Row.Cells[5].Text == "1")
            {
                e.Row.Cells[5].Text = "是";
            }
            else
            {
                e.Row.Cells[5].Text = "否";
            }

            if (e.Row.Cells[6].Text == "1")
            {
                e.Row.Cells[6].Text = "是";
            }
            else
            {
                e.Row.Cells[6].Text = "否";
            }

            olabel = (Label)e.Row.Cells[0].FindControl("lblAddNo");
            olabel.Text = Convert.ToString(this.grvAddDetail.Rows.Count + 1);
        }
    }

    /// <summary>
    /// 在confirm對話框中點選確認.繼續向下檢查
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnCheckOne_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(this.txthidCaseNo.Text.Trim()))
        {
            return;
        }
        CheckOne(this.txthidCaseNo.Text.Trim());
        BindGridView();

    }

    /// <summary>
    /// 在confirm對話框中點選取消.按最後一個原因寫剔退原因
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnCancelOne_Click(object sender, EventArgs e)
    {
        string strShowMsg = "";
        EntityPay_SV_Tmp ePaySVTmp = new EntityPay_SV_Tmp();
        ePaySVTmp.case_no = this.txthidCaseNo.Text.Trim();
        ePaySVTmp.CheckTime = Convert.ToInt32(ViewState["CheckTimes"].ToString() == "" ? "0" : ViewState["CheckTimes"].ToString().Trim());
        ePaySVTmp.RebackReason = ViewState["ReabckReason"].ToString();
        if (!BRPay_SV_Tmp.SetReback(ref strShowMsg, ePaySVTmp))
        {
            //剔退失敗
            etClientMstType = eClientMstType.Select;
            base.strClientMsg = MessageHelper.GetMessage("04_02010100_031") + "   " + MessageHelper.GetMessage(strShowMsg);
            return;
        }
        else
        {
            //剔退成功,跳出循環
            BindGridView();
        }
    }
    #endregion

    #region  Method
    /// <summary>
    /// 顯示窗體文字
    /// </summary>
    private void Show()
    {
        Page.Title = BaseHelper.GetShowText("04_02010100_000");
        grvAddDetail.Columns[0].HeaderText = BaseHelper.GetShowText("04_02010100_006");
        grvAddDetail.Columns[1].HeaderText = BaseHelper.GetShowText("04_02010100_007");
        grvAddDetail.Columns[2].HeaderText = BaseHelper.GetShowText("04_02010100_008");
        grvAddDetail.Columns[3].HeaderText = BaseHelper.GetShowText("04_02010100_009");
        grvAddDetail.Columns[4].HeaderText = BaseHelper.GetShowText("04_02010100_010");
        grvAddDetail.Columns[5].HeaderText = BaseHelper.GetShowText("04_02010100_011");
        grvAddDetail.Columns[6].HeaderText = BaseHelper.GetShowText("04_02010100_012");
        grvRejectDetail.Columns[0].HeaderText = BaseHelper.GetShowText("04_02010100_006");
        grvRejectDetail.Columns[1].HeaderText = BaseHelper.GetShowText("04_02010100_007");
        grvRejectDetail.Columns[2].HeaderText = BaseHelper.GetShowText("04_02010100_008");
        grvRejectDetail.Columns[3].HeaderText = BaseHelper.GetShowText("04_02010100_009");
        grvRejectDetail.Columns[4].HeaderText = BaseHelper.GetShowText("04_02010100_010");
        grvRejectDetail.Columns[5].HeaderText = BaseHelper.GetShowText("04_02010100_013");
        grvRejectDetail.Columns[6].HeaderText = BaseHelper.GetShowText("04_02010100_014");
        grvRejectDetail.Columns[7].HeaderText = BaseHelper.GetShowText("04_02010100_015");
    }

    /// <summary>
    ///綁定GridView數據源
    ///修改紀錄：調整try catch範圍 by Ares Stanley 20220214
    /// </summary>
    private void BindGridView()
    {
        DataTable dtblAddDetail = null;
        DataTable dtblRejectDetail = null;
        string strShowMsg = "";

        try
        {
            //* 查詢未驗證或通過驗證列表
            if (!BRPay_SV_Tmp.SearchNoAddInfo(ref strShowMsg, ref dtblAddDetail))
            {
                etClientMstType = eClientMstType.Select;
                base.strClientMsg = MessageHelper.GetMessage(strShowMsg);
                return;
            }
            //* 查詢剔退列表
            if (!BRPay_SV_Tmp.SearchRebackInfo(ref strShowMsg, ref dtblRejectDetail))
            {
                etClientMstType = eClientMstType.Select;
                base.strClientMsg = MessageHelper.GetMessage(strShowMsg);
                return;
            }

            this.grvAddDetail.DataSource = dtblAddDetail;
            this.grvAddDetail.DataBind();

            this.grvRejectDetail.DataSource = dtblRejectDetail;
            this.grvRejectDetail.DataBind();

        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            etClientMstType = eClientMstType.Select;
            base.strClientMsg = MessageHelper.GetMessage("00_00000000_000") + " : " + exp;
        }

    }

    /// <summary>
    /// 確定剔退所有
    /// 修改紀錄：添加try catch by Ares Stanley 20220214
    /// </summary>
    private void Reject()
    {
        string strShowMsg = "";
        try
        {
            if (!BRPay_SV_Tmp.RebackAll(ref strShowMsg))
            {
                etClientMstType = eClientMstType.Select;
                base.strClientMsg = MessageHelper.GetMessage(strShowMsg);
                return;
            }

            etClientMstType = eClientMstType.Select;
            base.strClientMsg = MessageHelper.GetMessage("04_02010100_005");
        }
        catch(Exception ex)
        {
            Logging.Log(ex);
            etClientMstType = eClientMstType.Select;
            base.strClientMsg = MessageHelper.GetMessage("04_02010100_006"); //剔退失敗
        }
        
    }

    /// <summary>
    /// 開立已經驗證的資料冰列印
    /// 修改紀錄：添加try catch by Ares Stanley 20220214
    /// </summary>
    private void Save()
    {
        string strShowMsg = "";
        string strSerialNoList = "";
        try
        {
            if (!BRPay_SV_Tmp.AddAll(ref strShowMsg, ref strSerialNoList))
            {
                //* 儲存開立資料到正式表失敗
                etClientMstType = eClientMstType.Select;
                base.strClientMsg = MessageHelper.GetMessage(strShowMsg);
                return;
            }
            else
            {
                //* 儲存開立資料到正式表成功,彈出列印窗口
                //strJs = BaseHelper.GetScriptForWindowOpenURL(this.Page, "P040201010002.aspx?SerialNoList=" + RedirectHelper.GetEncryptParam(strSerialNoList));

                string strMsgID = "";
                if (string.IsNullOrEmpty(strSerialNoList))
                {
                    //04_02010100_010
                    jsBuilder.RegScript(this.form1, jsBuilder.GetAlert(MessageHelper.GetMessage("04_02010100_010")) + " setTimeout(closewindow,1000);");
                    return;
                }
                DataSet dstResult = new DataSet();

                if (BRReport.Report02010100(strSerialNoList, ref strMsgID, ref dstResult))
                {
                    if (dstResult.Tables.Count > 0)
                    {
                    }
                    else
                    {
                        //無資料
                        jsBuilder.RegScript(this.Page, string.Format("AlertConfirm({{title:'{0}'}});", MessageHelper.GetMessage("04_00000000_056")));
                        return;
                    }
                }
                else
                {
                    //查詢失敗
                    jsBuilder.RegScript(this.Page, string.Format("AlertConfirm({{title:'{0}'}});", MessageHelper.GetMessage("04_00000000_055")));
                    jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow(strMsgID));
                    return;
                }
                if (dstResult != null)
                {
                    string strServerPathFile = this.Server.MapPath(UtilHelper.GetAppSettings("ExportExcelFilePath").ToString());
                    if (!BR_Excel_File.CreateExcelFile_Report02010100(dstResult, ref strServerPathFile))
                    {
                        jsBuilder.RegScript(this.Page, string.Format("AlertConfirm({{title:'{0}'}});", MessageHelper.GetMessage("04_00000000_058")));
                        return;
                    }
                    //* 將服務器端生成的文檔，下載到本地。
                    string strYYYYMMDD = "000" + Function.MinGuoDate7length(DateTime.Now.ToString("yyyyMMdd"));
                    strYYYYMMDD = strYYYYMMDD.Substring(strYYYYMMDD.Length - 8, 8);
                    string strFileName = "債清客戶開立債清證明" + strYYYYMMDD + ".xls";

                    //* 顯示提示訊息：匯出到Excel文檔資料成功
                    this.Session["ServerFile"] = strServerPathFile;
                    this.Session["ClientFile"] = strFileName;
                    string urlString = @"window.parent.postMessage({ func: 'ClientMsgShow', data: '" + MessageHelper.GetMessage("04_00000000_057") + "' }, '*');";
                    urlString += @"location.href='DownLoadFile.aspx';";
                    jsBuilder.RegScript(this.Page, urlString);
                    base.strClientMsg = MessageHelper.GetMessage("04_02010100_008");
                }


            }
        }
        catch(Exception ex)
        {
            Logging.Log(ex);
            base.strClientMsg = MessageHelper.GetMessage("04_02010100_009"); //開立失敗
        }
        

    }

    /// <summary>
    /// 批次驗證所有沒有驗證的資料(無Confirm直接判錯)
    /// </summary>
    private void AllValidate()
    {
        #region 宣告變數
        Hashtable htInput = new Hashtable();    //* 電文上行輸入欄位
        DataTable dtblJCEH;                     //* 主機回傳JCEH電文
        DataView dvJECH;                        //* 主機回傳JCEH電文
        string strShowMsg = "";                   //* 方法回傳消息ID
        string strHostMsg = ""; 
        string strMsg = "";                     //* 電文回傳信息
        EntityPay_SV_Tmp ePaySVTmp;             //* 剔退的
        Hashtable htJCU9RebackReason = new Hashtable();     //* 驗證JCU9和JCII返回的需確認信息列表
        Hashtable htSVRebackReason = new Hashtable();       //* 比對JCEH電文和SV的需確認信息列表
        string strType = ""; //* 清證種類  C-信用卡Only , N - ML Only , M - 卡+ML
        int intP4_JCU9 = -1;
        int intP4_JCII = -1;
        string strUserID = "";
        #endregion

        try
        {
            //* 取得需要驗證的資料,沒有則返回
            DataTable dtblCheckInfo = BRPay_SV_Tmp.GetNoCheck();
            if (dtblCheckInfo == null || dtblCheckInfo.Rows.Count <= 0)
            {
                //* 無需驗證資料
                etClientMstType = eClientMstType.Select;
                base.strClientMsg = MessageHelper.GetMessage("04_02010100_042");
                return;
            }

            //* 得到所有還未驗證的CaseNo並且廻圈驗證
            DataView dvCheckInfo = dtblCheckInfo.DefaultView;
            DataTable dtblCaseNo = dvCheckInfo.ToTable(true, "UserID", "case_no");
            for (int i = 0; i < dtblCaseNo.Rows.Count; i++)
            {
                #region 得到當前需要驗證的CaseNo資料
                dvCheckInfo.RowFilter = " case_no = '" + dtblCaseNo.Rows[i]["case_no"].ToString() + "' ";
                #endregion
                #region 設置剔退資料
                ePaySVTmp = new EntityPay_SV_Tmp();
                ePaySVTmp.case_no = dvCheckInfo[0]["CASE_NO"].ToString();
                ePaySVTmp.CheckTime = int.Parse(dvCheckInfo[0]["CheckTime"].ToString() == "" ? "0" : dvCheckInfo[0]["CheckTime"].ToString());
                #endregion
                //得到主機JCEH傳回信息
                strUserID = dtblCaseNo.Rows[i]["UserID"].ToString();

                htInput.Clear();
                htInput.Add("FUNCTION_CODE", "1");//*ID查詢
                htInput.Add("ACCT_NBR", strUserID);
                htInput.Add("LINE_CNT", "0000");//*LINE_CNT
 
                dtblJCEH = MainFrameInfo.GetMainframeDataByPage("P4_JCEH", htInput, false, "1", eAgentInfo);

                if (dtblJCEH.Rows[0]["HtgErrMsg"].ToString() != "") //*主機返回失敗
                {
                    base.strHostMsg = dtblJCEH.Rows[0]["HtgErrMsg"].ToString();
                    if (dtblJCEH.Rows[0]["MESSAGE_TYPE"].ToString() == "8888")
                    {
                        base.strClientMsg = MessageHelper.GetMessage("04_01010100_009");

                        //*如果該ID不存在則需要剔退
                        ePaySVTmp.RebackReason = MessageHelper.GetMessage("04_01010100_009");
                        if (!BRPay_SV_Tmp.SetReback(ref strShowMsg, ePaySVTmp))
                        {
                            //*剔退失敗
                            etClientMstType = eClientMstType.Select;
                            base.strClientMsg = MessageHelper.GetMessage("04_02010100_031") + "   " + MessageHelper.GetMessage(strShowMsg);
                            return;
                        }
                        else
                        {
                            //*剔退成功,繼續下個廻圈
                            continue;
                        }
                    }
                    else
                    {
                        base.strClientMsg = MessageHelper.GetMessage("04_00000000_008") + " : " + MessageHelper.GetMessage("04_02010100_031");
                        return;
                    }
                }
                else //*主機返回正確
                {
                    base.strClientMsg = MessageHelper.GetMessage("04_00000000_034");
                    base.strHostMsg = dtblJCEH.Rows[0]["MESSAGE_TYPE"].ToString() + " " + MessageHelper.GetMessage("04_00000000_034");

                    dvJECH = dtblJCEH.DefaultView;
                    #region 1.如果有GCB卡.剔退
                    dvJECH.RowFilter = " CARDHOLDER LIKE '40000189%'";
                    if (dvJECH.Count > 0)
                    {
                        //* 原萬通客戶
                        ePaySVTmp.RebackReason = MessageHelper.GetMessage("04_02010100_043");
                        if (!BRPay_SV_Tmp.SetReback(ref strShowMsg, ePaySVTmp))
                        {
                            //剔退失敗
                            etClientMstType = eClientMstType.Select;
                            base.strClientMsg = MessageHelper.GetMessage("04_02010100_031") + "   " + MessageHelper.GetMessage(strShowMsg);
                            return;
                        }
                        else
                        {
                            //剔退成功,跳出循環
                            continue;
                        }
                    }
                    #endregion
                    #region 2.如果BLK沒有SBAMZ,剔退
                    //* 是否沒有SBAZM卡
                    dvJECH.RowFilter = " BLOCK ='S' OR BLOCK ='B' OR BLOCK ='A' OR BLOCK ='M' OR BLOCK ='Z'";
                    if (dvJECH.Count <= 0)
                    {
                        //* 狀況碼不為SBAMZ
                        ePaySVTmp.RebackReason = MessageHelper.GetMessage("04_02010100_044");
                        if (!BRPay_SV_Tmp.SetReback(ref strShowMsg, ePaySVTmp))
                        {
                            //剔退失敗
                            etClientMstType = eClientMstType.Select;
                            base.strClientMsg = MessageHelper.GetMessage("04_02010100_031") + "   " + MessageHelper.GetMessage(strShowMsg);
                            return;
                        }
                        else
                        {
                            //剔退成功,跳出循環
                            continue;
                        }
                    }
                    #endregion
                    #region 3.如果BLK有空白或R,剔退
                    //* 是否有BLK=空白或R的卡
                    dvJECH.RowFilter = " BLOCK ='' OR BLOCK ='R'";
                    if (dvJECH.Count > 0)
                    {
                        //* 歸戶下含有流通或R管制卡片
                        ePaySVTmp.RebackReason = MessageHelper.GetMessage("04_02010100_045");
                        if (!BRPay_SV_Tmp.SetReback(ref strShowMsg, ePaySVTmp))
                        {
                            //剔退失敗
                            etClientMstType = eClientMstType.Select;
                            base.strClientMsg = MessageHelper.GetMessage("04_02010100_031") + "   " + MessageHelper.GetMessage(strShowMsg);
                            return;
                        }
                        else
                        {
                            //剔退成功,跳出循環
                            continue;
                        }
                    }
                    #endregion
                    #region 4.歸戶目前餘額不為0,剔退
                    //* 歸戶目前餘額不為0
                    dvJECH.RowFilter = " ISNULL(CURR_BAL,'0') <> '0' AND CURR_BAL <> ''";
                    for (int j = 0; j < dvJECH.Count; j++)
                    {
                        if (!string.IsNullOrEmpty(dvJECH[j]["CURR_BAL"].ToString()) && dvJECH[j]["CURR_BAL"].ToString().Trim() != "0")
                        {
                            //* 歸戶目前餘額不為0
                            ePaySVTmp.RebackReason = MessageHelper.GetMessage("04_02010100_046");
                            if (!BRPay_SV_Tmp.SetReback(ref strShowMsg, ePaySVTmp))
                            {
                                //剔退失敗
                                etClientMstType = eClientMstType.Select;
                                base.strClientMsg = MessageHelper.GetMessage("04_02010100_031") + "   " + MessageHelper.GetMessage(strShowMsg);
                                return;
                            }
                            else
                            {
                                //剔退成功,跳出循環
                                continue;
                            }
                        }
                    }
                    #endregion
                    #region 5.廻圈驗證每張卡.同時得到清證種類
                    intP4_JCU9 = -1;
                    intP4_JCII = -1;
                    strType = "";
                    htJCU9RebackReason.Clear();
                    dvJECH.RowFilter = " BLOCK ='S' OR BLOCK ='B' OR BLOCK ='A' OR BLOCK ='M' OR BLOCK ='Z'";
                    for (int j = 0; j < dvJECH.Count; j++)
                    {

                        if (!BRPay_SV_Tmp.ValidationSV(strUserID,
                                                        dvJECH[j]["CARDHOLDER"].ToString().Trim(),
                                                        dvJECH[j]["BLOCK"].ToString().Trim(),
                                                        dvJECH[j]["BLOCK_DTE"].ToString(),
                                                        dvJECH[j]["AMC_FLAG"].ToString(),
                                                        ref htJCU9RebackReason, ref strShowMsg, ref intP4_JCU9, ref intP4_JCII,eAgentInfo,ref strHostMsg))
                        {
                            //* 出錯
                            etClientMstType = eClientMstType.Select;
                            base.strHostMsg = MessageHelper.GetMessage(strHostMsg);
                            base.strClientMsg = MessageHelper.GetMessage(strShowMsg) + " : " + MessageHelper.GetMessage("04_02010100_031");
                            return;
                        }

                        #region 得到清證種類 (C-CardOnly,N-MLOnly,M-Card+ML)
                        if (Function.IsML(dvJECH[j]["CARDHOLDER"].ToString()) == 1)
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
                    }
                    if (htJCU9RebackReason.Count > 0)
                    {
                        ePaySVTmp.RebackReason = htJCU9RebackReason["0"].ToString();
                        if (!BRPay_SV_Tmp.SetReback(ref strShowMsg, ePaySVTmp))
                        {
                            //剔退失敗
                            etClientMstType = eClientMstType.Select;
                            base.strClientMsg = MessageHelper.GetMessage("04_02010100_031") + "   " + MessageHelper.GetMessage(strShowMsg);
                            return;
                        }
                        else
                        {
                            //剔退成功,跳出循環
                            continue;
                        }
                    }
                    #endregion
                    #region 6.驗證SV與JCEH是否相符
                    htSVRebackReason.Clear();
                    dvJECH.RowFilter = " BLOCK ='S' OR BLOCK ='B' OR BLOCK ='A' OR BLOCK ='M' OR BLOCK ='Z'";
                    CheckSVSame(strType, dvCheckInfo, dvJECH, ref htSVRebackReason);
                    if (htSVRebackReason.Count >= 2)
                    {
                        //* 多于2條:SV資料與主機資料比對不合
                        ePaySVTmp.RebackReason = MessageHelper.GetMessage("04_02010100_056");
                        if (!BRPay_SV_Tmp.SetReback(ref strShowMsg, ePaySVTmp))
                        {
                            //剔退失敗
                            etClientMstType = eClientMstType.Select;
                            base.strClientMsg = MessageHelper.GetMessage("04_02010100_031") + "   " + MessageHelper.GetMessage(strShowMsg);
                            return;
                        }
                        else
                        {
                            //剔退成功,跳出循環
                            continue;
                        }
                    }
                    else if (htSVRebackReason.Count > 0)
                    {
                        ePaySVTmp.RebackReason = htSVRebackReason["0"].ToString();
                        if (!BRPay_SV_Tmp.SetReback(ref strShowMsg, ePaySVTmp))
                        {
                            //剔退失敗
                            etClientMstType = eClientMstType.Select;
                            base.strClientMsg = MessageHelper.GetMessage("04_02010100_031") + "   " + MessageHelper.GetMessage(strShowMsg);
                            return;
                        }
                        else
                        {
                            //剔退成功,跳出循環
                            continue;
                        }
                    }
                    #endregion
                    #region 7.如果開立類型爲M或者N,連接JCAS
                    strMsg = "";
                    dvJECH.RowFilter = " BLOCK ='S' OR BLOCK ='B' OR BLOCK ='A' OR BLOCK ='M' OR BLOCK ='Z'";
                    if (strType == "N" || strType == "M")
                    {
                        if (CheckJCAS(dvJECH, ref strMsg))
                        {
                            //* 驗證成功
                            if (!string.IsNullOrEmpty(strMsg))
                            {
                                //* 有需要confirm剔退信息
                                ePaySVTmp.RebackReason = strMsg;
                                if (!BRPay_SV_Tmp.SetReback(ref strShowMsg, ePaySVTmp))
                                {
                                    //剔退失敗
                                    etClientMstType = eClientMstType.Select;
                                    base.strClientMsg = MessageHelper.GetMessage("04_02010100_031") + "   " + MessageHelper.GetMessage(strShowMsg);
                                    return;
                                }
                                else
                                {
                                    //剔退成功,跳出循環
                                    continue;
                                }
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(strMsg))
                            {
                                //* 驗證失敗
                                etClientMstType = eClientMstType.Select;
                                base.strClientMsg = strMsg + " " + MessageHelper.GetMessage("04_02010100_032");
                                return;
                            }
                        }
                    }
                    #endregion
                    #region 8.檢查催收結案日期或原因
                    strMsg = "";
                    dvJECH.RowFilter = " BLOCK ='S' OR BLOCK ='B' OR BLOCK ='A' OR BLOCK ='M' OR BLOCK ='Z'";
                    if (CheckMacro(strUserID, ref strMsg))
                    {
                        ePaySVTmp.RebackReason = strMsg;
                        if (!BRPay_SV_Tmp.SetReback(ref strShowMsg, ePaySVTmp))
                        {
                            //剔退失敗
                            etClientMstType = eClientMstType.Select;
                            base.strClientMsg = MessageHelper.GetMessage("04_02010100_031") + "   " + MessageHelper.GetMessage(strShowMsg);
                            return;
                        }
                        else
                        {
                            //剔退成功,跳出循環
                            continue;
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(strMsg))
                        {
                            etClientMstType = eClientMstType.Select;
                            base.strClientMsg = strMsg;
                            return;
                        }
                    }
                    #endregion
                    #region 9.結案日期比對
                    strMsg = "";
                    dvJECH.RowFilter = " BLOCK ='S' OR BLOCK ='B' OR BLOCK ='A' OR BLOCK ='M' OR BLOCK ='Z'";
                    if (CheckLastDate(strUserID, strType, dvCheckInfo, dvJECH, ref strMsg))
                    {
                        ePaySVTmp.RebackReason = strMsg;
                        if (!BRPay_SV_Tmp.SetReback(ref strShowMsg, ePaySVTmp))
                        {
                            //剔退失敗
                            etClientMstType = eClientMstType.Select;
                            base.strClientMsg = MessageHelper.GetMessage("04_02010100_031") + "   " + MessageHelper.GetMessage(strShowMsg);
                            return;
                        }
                        else
                        {
                            //剔退成功,跳出循環
                            continue;
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(strMsg))
                        {
                            etClientMstType = eClientMstType.Select;
                            base.strClientMsg = strMsg;
                            return;
                        }
                    }
                    #endregion

                    #region Final.設置驗證通過主表該客戶資料
                    if (!BRPay_SV_Tmp.SetAdd(ref strShowMsg, ePaySVTmp))
                    {
                        //*開立失敗
                        etClientMstType = eClientMstType.Select;
                        base.strClientMsg = MessageHelper.GetMessage("04_02010100_031") + "   " + MessageHelper.GetMessage(strShowMsg);
                        return;
                    }
                    #endregion
                }
            }
        }
        catch (Exception Ex)
        {
            etClientMstType = eClientMstType.Select;
            base.strClientMsg = MessageHelper.GetMessage("04_02010100_031") + " : " + Ex;
            Logging.Log(Ex, LogLayer.UI);
            return;
        }

    }

    /// <summary>
    /// 通過CaseNo來單筆驗證資料(有Confirm)
    /// </summary>
    /// <param name="strCaseNo"></param>
    private void CheckOne(string strCaseNo)
    {

        try
        {
            //* 1331(JCEH)電文欄位: 查詢起始筆數,訊息別,客戶姓名,客戶地址1,客戶地址2,客戶地址3,郵遞區號,卡號,卡人ID,歸戶ID,發卡日,卡人姓名,狀況碼,狀況碼日期,前況碼,前況碼日期,最後繳款日,餘額,不良資產是否已經出售
            string[] aryJCEHCol = new string[] { "LINE_CNT", "MESSAGE_TYPE", "SHORT_NAME", "CITY", "ADDR_1", "ADDR_2", "ZIP", "CARDHOLDER", "CUSTID", "ACCT_CUST_ID", "CARDHOLDER_NAME", "OPENED", "BLOCK", "BLOCK_DTE", "ALT_BLOCK", "ALT_BLOCK_DTE", "DTE_LST_PYMT", "CURR_BAL", "AMC_FLAG" };
            Hashtable htInput = new Hashtable();    //* 電文上行輸入欄位
            DataTable dtblJCEH;                     //* 主機回傳JCEH電文
            DataView dvJECH;                        //* 主機回傳JCEH電文
            string strShowMsg = "";                   //* 方法回傳消息ID
            string strMsg = "";                     //* 電文回傳信息
            EntityPay_SV_Tmp ePaySVTmp;             //* 剔退的
            Hashtable htJCU9RebackReason = new Hashtable();     //* 驗證JCU9和JCII返回的需確認信息列表
            Hashtable htSVRebackReason = new Hashtable();       //* 比對JCEH電文和SV的需確認信息列表
            string strType = "";                    //* 清證種類  C-信用卡Only , N - ML Only , M - 卡+ML
            int intP4_JCU9 = -1;                    //* 是否連接過JCU9,-1沒連接過,0-查無資料,1餘額不為0
            int intP4_JCII = -1;                    //* 是否連接過JCII,-1沒連接過,0-查無資料,1餘額不為0,2餘額爲0
            string strUserID = "";
            ViewState["ReabckReason"] = "";
            ViewState["CheckTimes"] = "";
            string strConfirmMsg = "";
            #region 取得sv資料
            DataTable dtblCheckInfo = BRPay_SV_Tmp.GetDataByCaseNo(strCaseNo);
            if (dtblCheckInfo == null || dtblCheckInfo.Rows.Count <= 0) { return; }
            DataView dvCheckInfo = dtblCheckInfo.DefaultView;
            string str1144Code = "";
            #endregion

            #region 設置剔退資料
            ePaySVTmp = new EntityPay_SV_Tmp();
            ePaySVTmp.case_no = strCaseNo;
            ePaySVTmp.CheckTime = Convert.ToInt32(dvCheckInfo[0]["CheckTime"].ToString() == "" ? "0" : dvCheckInfo[0]["CheckTime"].ToString());
            ViewState["CheckTimes"] = ePaySVTmp.CheckTime;
            #endregion

            //得到主機JCEH傳回信息
            strUserID = dvCheckInfo[0]["UserID"].ToString().Trim();

            htInput.Clear();
            htInput.Add("FUNCTION_CODE", "1");//*ID查詢
            htInput.Add("ACCT_NBR", strUserID);
            htInput.Add("LINE_CNT", "0000");//*LINE_CNT

            dtblJCEH = MainFrameInfo.GetMainframeDataByPage("P4_JCEH", htInput, false, "1", eAgentInfo);

            if (dtblJCEH.Rows[0]["HtgErrMsg"].ToString() != "") //*主機返回失敗
            {
                base.strHostMsg = dtblJCEH.Rows[0]["HtgErrMsg"].ToString();
                if (dtblJCEH.Rows[0]["MESSAGE_TYPE"].ToString() == "8888")
                {
                    base.strClientMsg = MessageHelper.GetMessage("04_01010100_009");

                    //*如果該ID不存在則需要剔退
                    ePaySVTmp.RebackReason = MessageHelper.GetMessage("04_01010100_009");
                    if (!BRPay_SV_Tmp.SetReback(ref strShowMsg, ePaySVTmp))
                    {
                        //*剔退失敗
                        etClientMstType = eClientMstType.Select;
                        base.strClientMsg = MessageHelper.GetMessage("04_02010100_031") + "   " + MessageHelper.GetMessage(strShowMsg);
                        return;
                    }
                }
                else
                {
                    base.strClientMsg = MessageHelper.GetMessage("04_00000000_008") + " : " + MessageHelper.GetMessage("04_02010100_031");
                    return;
                }
            }
            else //*主機返回正確
            {
                base.strClientMsg = MessageHelper.GetMessage("04_00000000_034");
                base.strHostMsg = dtblJCEH.Rows[0]["MESSAGE_TYPE"].ToString() + " " + MessageHelper.GetMessage("04_00000000_034");

                dvJECH = dtblJCEH.DefaultView;
                #region 1.如果有GCB卡.剔退
                if (this.txtHiden1.Text.Trim() != "Y")
                {
                    dvJECH.RowFilter = " CARDHOLDER LIKE '40000189%'";
                    if (dvJECH.Count > 0)
                    {
                        //* 原萬通客戶
                        ViewState["ReabckReason"] = MessageHelper.GetMessage("04_02010100_043");
                        strJs = strJs + "ConfrimText('" + MessageHelper.GetMessage("04_02010100_043") + MessageHelper.GetMessage("04_02010100_041") + "','txtHiden1');";
                        return;
                    }
                }
                #endregion

                #region 2.如果BLK沒有SBAMZ,剔退
                if (this.txtHiden2.Text.Trim() != "Y")
                {
                    //* 是否沒有SBAZM卡
                    dvJECH.RowFilter = " BLOCK ='S' OR BLOCK ='B' OR BLOCK ='A' OR BLOCK ='M' OR BLOCK ='Z'";
                    if (dvJECH.Count <= 0)
                    {
                        //* 狀況碼不為SBAMZ
                        ViewState["ReabckReason"] = MessageHelper.GetMessage("04_02010100_044");
                        strJs = strJs + "ConfrimText('" + MessageHelper.GetMessage("04_02010100_044") + MessageHelper.GetMessage("04_02010100_041") + "','txtHiden2');";
                        return;
                    }
                }
                #endregion
                #region 3.如果BLK有空白或R,剔退
                if (this.txtHiden3.Text.Trim() != "Y")
                {
                    //* 是否有BLK=空白或R的卡
                    dvJECH.RowFilter = " BLOCK ='' OR BLOCK ='R'";
                    if (dvJECH.Count > 0)
                    {
                        //* 歸戶下含有流通或R管制卡片
                        ViewState["ReabckReason"] = MessageHelper.GetMessage("04_02010100_045");
                        strJs = strJs + "ConfrimText('" + MessageHelper.GetMessage("04_02010100_045") + MessageHelper.GetMessage("04_02010100_041") + "','txtHiden3');";
                        return;
                    }
                }
                #endregion
                #region 4.歸戶目前餘額不為0,剔退
                //* 歸戶目前餘額不為0
                if (this.txtHiden4.Text.Trim() != "Y")
                {
                    dvJECH.RowFilter = " ISNULL(CURR_BAL,'0') <> '0' AND CURR_BAL <> ''";
                    for (int j = 0; j < dvJECH.Count; j++)
                    {
                        if (!string.IsNullOrEmpty(dvJECH[j]["CURR_BAL"].ToString()) && dvJECH[j]["CURR_BAL"].ToString().Trim() != "0")
                        {
                            //* 歸戶目前餘額不為0
                            ViewState["ReabckReason"] = MessageHelper.GetMessage("04_02010100_046");
                            strJs = strJs + "ConfrimText('" + MessageHelper.GetMessage("04_02010100_046") + MessageHelper.GetMessage("04_02010100_041") + "','txtHiden4');";
                            return;
                        }
                    }
                }
                #endregion
                #region 5.廻圈驗證每張卡.同時得到清證種類
                intP4_JCU9 = -1;
                intP4_JCII = -1;
                strType = "";
                htJCU9RebackReason.Clear();
                dvJECH.RowFilter = " BLOCK ='S' OR BLOCK ='B' OR BLOCK ='A' OR BLOCK ='M' OR BLOCK ='Z'";
                for (int j = 0; j < dvJECH.Count; j++)
                {
                    #region 廻圈驗證每張卡
                    if (this.txtHiden5.Text.Trim() != "Y")
                    {
                        if (!BRPay_SV_Tmp.ValidationSV(strUserID,
                                                        dvJECH[j]["CARDHOLDER"].ToString().Trim(),
                                                        dvJECH[j]["BLOCK"].ToString().Trim(),
                                                        dvJECH[j]["BLOCK_DTE"].ToString(),
                                                        dvJECH[j]["AMC_FLAG"].ToString(),
                                                        ref htJCU9RebackReason, ref strShowMsg, ref intP4_JCU9, ref intP4_JCII, eAgentInfo, ref strHostMsg))
                        {
                            //* 出錯
                            etClientMstType = eClientMstType.Select;
                            base.strHostMsg = MessageHelper.GetMessage(strHostMsg);
                            base.strClientMsg = MessageHelper.GetMessage(strShowMsg) + " : " + MessageHelper.GetMessage("04_02010100_031");
                            return;
                        }
                    }
                    #endregion
                    #region 得到清證種類 (C-CardOnly,N-MLOnly,M-Card+ML)
                    if (Function.IsML(dvJECH[j]["CARDHOLDER"].ToString()) == 1)
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
                }
                #region 廻圈驗證每張卡.有問題就要confirm
                if (this.txtHiden5.Text.Trim() != "Y")
                {
                    //* 如果strConfirmMsg有值代表需要Confrim確認
                    if (htJCU9RebackReason.Count > 0)
                    {

                        strConfirmMsg = "";
                        for (int i = 0; i < htJCU9RebackReason.Count; i++)
                        {
                            strConfirmMsg = strConfirmMsg + @"\r\n" + htJCU9RebackReason[i.ToString()].ToString() + MessageHelper.GetMessage("04_02010100_041");
                        }
                        strConfirmMsg = strConfirmMsg.Substring(4);     //* 除去最前面的\R\N
                        strJs = "ConfrimText('" + strConfirmMsg + "','txtHiden5');";
                        ViewState["ReabckReason"] = htJCU9RebackReason["0"].ToString();
                        return;
                    }
                }
                #endregion

                #endregion
                #region 6.驗證SV與JCEH是否相符
                htJCU9RebackReason.Clear();
                dvJECH.RowFilter = " BLOCK ='S' OR BLOCK ='B' OR BLOCK ='A' OR BLOCK ='M' OR BLOCK ='Z'";
                if (this.txtHiden6.Text.Trim() != "Y")
                {


                    CheckSVSame(strType, dvCheckInfo, dvJECH, ref htSVRebackReason);
                    //* 還沒甲CONFIRM
                    if (htSVRebackReason.Count > 0)
                    {
                        strConfirmMsg = "";
                        for (int i = 0; i < htSVRebackReason.Count; i++)
                        {
                            strConfirmMsg = strConfirmMsg + @"\r\n" + htSVRebackReason[i.ToString()].ToString() + MessageHelper.GetMessage("04_02010100_041");
                        }
                        strConfirmMsg = strConfirmMsg.Substring(4);     //* 除去最前面的\R\N
                        strJs = "ConfrimText('" + strConfirmMsg + "','txtHiden6');";

                        if (htSVRebackReason.Count >= 2)
                        {
                            ViewState["ReabckReason"] = MessageHelper.GetMessage("04_02010100_056");
                        }
                        else
                        {
                            ViewState["ReabckReason"] = htSVRebackReason["0"].ToString();
                        }

                        return;
                    }
                }
                #endregion
                #region 7.如果開立類型爲M或者N,連接JCAS
                strMsg = "";
                dvJECH.RowFilter = " BLOCK ='S' OR BLOCK ='B' OR BLOCK ='A' OR BLOCK ='M' OR BLOCK ='Z'";
                if (this.txtHiden7.Text.Trim() != "Y")
                {
                    if (strType == "N" || strType == "M")
                    {
                        if (CheckJCAS(dvJECH, ref strMsg))
                        {
                            //* 驗證成功
                            if (!string.IsNullOrEmpty(strMsg))
                            {
                                ViewState["ReabckReason"] = strMsg;
                                strJs = strJs + "ConfrimText('" + strMsg + MessageHelper.GetMessage("04_02010100_041") + "','txtHiden7');";
                                return;
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(strMsg))
                            {
                                //* 驗證失敗
                                etClientMstType = eClientMstType.Select;
                                base.strClientMsg = strMsg + " " + MessageHelper.GetMessage("04_02010100_032");
                                return;
                            }
                        }
                    }
                }
                #endregion
                #region 8.檢查催收結案日期或原因
                strMsg = "";
                dvJECH.RowFilter = " BLOCK ='S' OR BLOCK ='B' OR BLOCK ='A' OR BLOCK ='M' OR BLOCK ='Z'";
                if (this.txtHiden8.Text.Trim() != "Y")
                {
                    if (CheckMacro(strUserID, ref strMsg))
                    {
                        ViewState["ReabckReason"] = strMsg;
                        strJs = strJs + "ConfrimText('" + strMsg + MessageHelper.GetMessage("04_02010100_041") + "','txtHiden8');";
                        return;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(strMsg))
                        {
                            etClientMstType = eClientMstType.Select;
                            base.strClientMsg = strMsg;
                            return;
                        }
                    }
                }
                #endregion
                #region 9.結案日期比對
                strMsg = "";
                dvJECH.RowFilter = " BLOCK ='S' OR BLOCK ='B' OR BLOCK ='A' OR BLOCK ='M' OR BLOCK ='Z'";
                if (this.txtHiden9.Text.Trim() != "Y")
                {
                    if (CheckLastDate(strUserID, strType, dvCheckInfo, dvJECH, ref strMsg))
                    {

                        ViewState["ReabckReason"] = strMsg;
                        strJs = strJs + "ConfrimText('" + strMsg + MessageHelper.GetMessage("04_02010100_041") + "','txtHiden9');";
                        return;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(strMsg))
                        {
                            etClientMstType = eClientMstType.Select;
                            base.strClientMsg = strMsg;
                            return;
                        }
                    }
                }
                #endregion

                #region Final.設置驗證通過主表該客戶資料
                if (!BRPay_SV_Tmp.SetAdd(ref strShowMsg, ePaySVTmp))
                {
                    //*開立失敗
                    etClientMstType = eClientMstType.Select;
                    base.strClientMsg = MessageHelper.GetMessage("04_02010100_031") + "   " + MessageHelper.GetMessage(strShowMsg);
                    return;
                }
                strJs = strJs + "ResetHiddenTextbox();";
                #endregion

                #region 根據JCFK(1144)不同判斷是否需要顯示Macro頁面

                if (!CheckJCFK(strUserID, dvJECH, ref strMsg, ref str1144Code))
                {
                    strJs = strJs + "ResetHiddenTextbox();";
                    return;
                }
                //* 如果str1144Status 不等於 ""(即為人工轉呆帳或停卡滿五年),顯示Marco畫面提供列印
                if (str1144Code != "")
                {
                    strJs = strJs + BaseHelper.GetScriptForWindowOpenURL(this.Page, "P040201010003.aspx?UserID=" + RedirectHelper.GetEncryptParam(strUserID));
                }
                #endregion
            }
        }
        catch (Exception Ex)
        {
            etClientMstType = eClientMstType.Select;
            base.strClientMsg =  MessageHelper.GetMessage("04_02010100_032") + " : " + Ex;
            Logging.Log(Ex, LogLayer.UI);
            return;
        }
    }

    /// <summary>
    /// 檢查所有ML或卡+ML的開立的所有卡片. ML是否結清,是否需要confrim
    /// </summary>
    /// <param name="dvJCEH">JCEH的電文</param>
    /// <param name="strMsg">傳出的消息</param>
    /// <returns>True - 需要confirm, False - 不需要confirm或出錯</returns>
    private bool CheckJCAS(DataView dvJCEH, ref string strMsg)
    {
        for (int j = 0; j < dvJCEH.Count; j++)
        {
            string strCardNo = dvJCEH[j]["CARDHOLDER"].ToString().Trim();
            if (Function.IsML(strCardNo) == 1)
            {
                //* 要傳回的欄位集合 結清日

                Hashtable htInput = new Hashtable();

                htInput.Add("FUNCTION_CODE", "1");//*ID查詢
                htInput.Add("ACCT_NBR", strCardNo);
                htInput.Add("LINE_CNT", "0000");//*LINE_CNT

                Hashtable htJCAS = MainFrameInfo.GetMainframeData("P4_JCAS", htInput, false, "1", eAgentInfo);

                if (htJCAS.Contains("HtgMsg")) //*主機返回失敗
                {
                    strMsg = MessageHelper.GetMessage("04_00000000_009");
                    base.strHostMsg = htJCAS["HtgMsg"].ToString();
                    return false;
                }
                else
                {
                    base.strHostMsg = htJCAS["HtgSuccess"].ToString();//*主機返回成功訊息     
                     //若有資料則判斷 <1572電文.結清日>是否為空白，若是空白則踢退
                    if (htJCAS["SETTLE_DATE"].ToString().Trim() == "" || htJCAS["SETTLE_DATE"].ToString().Trim() == "00000000")
                    {
                        //* ML未結清
                        strMsg = MessageHelper.GetMessage("04_02010100_019");
                        base.strClientMsg = strMsg;
                        return true;
                    }
                }
            }
        }
        return false;
    }

    /// <summary>
    /// 檢查SV匯入資料是否和JCEH電文下來的資料一致,是否需要confirm
    /// </summary>
    /// <param name="strType"></param>
    /// <param name="dvCheckInfo"></param>
    /// <param name="dvJCEH"></param>
    /// <param name="htRebackReason">傳出的消息</param>
    private void CheckSVSame(string strType, DataView dvCheckInfo, DataView dvJCEH, ref Hashtable htRebackReason)
    {
        //* 0. 筆數不同  (顯示 筆數有誤)
        //* 1. 姓名欄位  (顯示 姓名有誤)
        //* 2. 卡號  (顯示  xxx 卡號有誤)
        //* 3. BLK    (做法於 S-IR-0105 )  (顯示 xxx 卡號  BLK 有誤)
        //* 4. 卡別   (顯示 xxx 卡號  卡別有誤)
        //* 5. 附卡人ID  (顯示 附卡人ID有誤)
        //* 6. 附卡人姓名  (顯示 附卡人姓名有誤)
        //* 以上資料 都與 1331 主機比對即可
        htRebackReason = new Hashtable();
        dvJCEH.RowFilter = "";
        string strJCEHName = dvJCEH[0]["SHORT_NAME"].ToString().Trim();
        dvJCEH.RowFilter = " BLOCK ='S' OR BLOCK ='B' OR BLOCK ='A' OR BLOCK ='M' OR BLOCK ='Z'";

        string strUserID = dvCheckInfo[0]["UserID"].ToString().Trim();
        string strTypeCard = "";
        string strSVCardNo = "";
        string strLogMsg = "";
        #region 0. 驗證筆數
        //* 驗證筆數        
        if (dvCheckInfo.Count != dvJCEH.Count)
        {
            //* SV筆數有誤
            htRebackReason.Add(htRebackReason.Count.ToString(), MessageHelper.GetMessage("04_02010100_048"));
            strLogMsg += "\r\n" + "客戶ID:" + strUserID + ", SV筆數有誤,SV:" + dvCheckInfo.Count + " ,主機:" + dvJCEH.Count.ToString();
            if (dvJCEH.Count <= 0)
            {
                return;
            }
        }

        #endregion
        #region 1. 姓名欄位  (顯示 姓名有誤)
        //* 1. 姓名欄位  (顯示 姓名有誤)
        if (dvCheckInfo[0]["UserName"].ToString().Trim() != strJCEHName)
        {
            //* 姓名有誤
            htRebackReason.Add(htRebackReason.Count.ToString(), MessageHelper.GetMessage("04_02010100_049"));
            strLogMsg += "\r\n" + "客戶ID:" + strUserID + ", 姓名有誤,SV:" + dvCheckInfo[0]["UserName"].ToString().Trim() + " ,主機:" + strJCEHName;
        }
        #endregion
        //* 廻圈驗證每張卡
        for (int i = 0; i < dvCheckInfo.Count; i++)
        {
            strSVCardNo = dvCheckInfo[i]["CardNo"].ToString().Trim();
            dvJCEH.RowFilter = "CARDHOLDER = '" + dvCheckInfo[i]["CardNo"].ToString().Trim() + "' AND (BLOCK ='S' OR BLOCK ='B' OR BLOCK ='A' OR BLOCK ='M' OR BLOCK ='Z')";
            if (dvJCEH.Count > 0)
            {
                #region 3. BLK     (顯示 xxx 卡號  BLK 有誤)
                //* 3. BLK     (顯示 xxx 卡號  BLK 有誤)
                if (dvCheckInfo[i]["BLKCode"].ToString().Trim() != dvJCEH[0]["BLOCK"].ToString().Trim())
                {
                    //* 卡號{0}BLK有誤
                    htRebackReason.Add(htRebackReason.Count.ToString(), MessageHelper.GetMessage("04_02010100_051", strSVCardNo));
                    strLogMsg += "\r\n" + "客戶ID:" + strUserID + ", 卡號" + strSVCardNo + "BLK有誤,SV:" + dvCheckInfo[i]["BLKCode"].ToString().Trim() + " ,主機:" + dvJCEH[0]["BLOCK"].ToString().Trim();
                }
                #endregion
                #region 4. 卡別   (顯示 xxx 卡號  卡別有誤)

                //* 當查詢ID= 歸戶ID =  卡人ID  => 正卡 0
                //* 當查詢ID= 歸戶ID<> 卡人ID  => 附卡 1
                //* 卡人ID CUSTID, 歸戶ID:ACCT_CUST_ID
                strTypeCard = "";
                if (strUserID == dvJCEH[0]["ACCT_CUST_ID"].ToString().Trim() && strUserID == dvJCEH[0]["CUSTID"].ToString().Trim())
                {
                    strTypeCard = "0";
                }
                if (strUserID == dvJCEH[0]["ACCT_CUST_ID"].ToString().Trim() && strUserID != dvJCEH[0]["CUSTID"].ToString().Trim())
                {
                    strTypeCard = "1";
                }
                strLogMsg += "\r\n查詢ID:" + strUserID +
                            " ,歸戶ID:" + dvJCEH[0]["ACCT_CUST_ID"].ToString().Trim() +
                            " ,卡人ID:" + dvJCEH[0]["CUSTID"].ToString().Trim() +
                            ",卡別:" + strTypeCard;
                if (dvCheckInfo[i]["CardType"].ToString().Trim() != strTypeCard)
                {
                    //* 卡號{0}卡別有誤
                    htRebackReason.Add(htRebackReason.Count.ToString(), MessageHelper.GetMessage("04_02010100_052", strSVCardNo));
                    strLogMsg += "\r\n" + "客戶ID:" + strUserID + ", 卡號" + strSVCardNo + "卡別有誤,SV:" + dvCheckInfo[i]["CardType"].ToString().Trim() + " ,主機:" + strTypeCard;
                }
                #endregion
                #region 5. 附卡人ID  (顯示 附卡人ID有誤)
                //* 5. 附卡人ID  (顯示 附卡人ID有誤)
                if (dvCheckInfo[i]["CardID"].ToString().Trim() != dvJCEH[0]["CUSTID"].ToString().Trim())
                {
                    //* 卡號{0}附卡人ID有誤
                    htRebackReason.Add(htRebackReason.Count.ToString(), MessageHelper.GetMessage("04_02010100_053", strSVCardNo));
                    strLogMsg += "\r\n" + "客戶ID:" + strUserID + ", 卡號" + strSVCardNo + "附卡人ID有誤,SV:" + dvCheckInfo[i]["CardID"].ToString().Trim() + " ,主機:" + dvJCEH[0]["CUSTID"].ToString().Trim();
                }
                #endregion
                #region 6.附卡人姓名  (顯示 附卡人姓名有誤)
                //* 6. 附卡人姓名  (顯示 附卡人姓名有誤)
                if (dvCheckInfo[i]["CardName"].ToString().Trim() != dvJCEH[0]["CARDHOLDER_NAME"].ToString().Trim())
                {
                    //* 卡號{0}附卡人姓名有誤
                    htRebackReason.Add(htRebackReason.Count.ToString(), MessageHelper.GetMessage("04_02010100_054", strSVCardNo));
                    strLogMsg += "\r\n" + "客戶ID:" + strUserID + ", 卡號" + strSVCardNo + "附卡人姓名有誤,SV:" + dvCheckInfo[i]["CardName"].ToString().Trim() + " ,主機:" + dvJCEH[0]["CARDHOLDER_NAME"].ToString().Trim();
                }
                #endregion
            }
            else
            {
                #region 2.SV里有這個卡號,主機上下傳沒有.剔退
                //* {0}卡號有誤
                htRebackReason.Add(htRebackReason.Count.ToString(), MessageHelper.GetMessage("04_02010100_050", strSVCardNo));
                strLogMsg += "\r\n" + "客戶ID:" + strUserID + ", 卡號" + strSVCardNo + "有誤,SV:" + dvCheckInfo[i]["CardName"].ToString().Trim() + " ,主機:沒有";
                #endregion
            }
        }
        if (!string.IsNullOrEmpty(strLogMsg))
        {
            Logging.Log(strLogMsg, LogState.Debug, LogLayer.UI);
        }
    }

    /// <summary>
    /// 檢查催收平臺主機.判斷原因,是否需要confirm
    /// </summary>
    /// <param name="strUserID"></param>
    /// <param name="strMsg"></param>
    /// <returns></returns>
    private bool CheckMacro(string strUserID, ref string strMsg)
    {
        EntityPay_MacroSet esPayMacro = new EntityPay_MacroSet();
        string strShowMsg = "";
        //* 通過WebService連接催收平臺取得客戶最後繳款資料
        if (!BRWebService.GetMarcoDataByCustID(strUserID, ref strShowMsg))
        {
            strMsg = MessageHelper.GetMessage("04_02010100_031") + "   " + MessageHelper.GetMessage(strShowMsg);
            return false;
        }

        //* 取得剛剛得到的催收平臺(MACRO)最後繳款資料       
        if (BRPay_Macro.GetDataByCustID(strUserID, ref strShowMsg, ref esPayMacro))
        {
            //*如果結案日期只要有一筆結案日期期不為空白
            for (int j = 0; j < esPayMacro.Count; j++)
            {

                if (esPayMacro.GetEntity(j).M_CLOSE_DATE != "" &&
                    (esPayMacro.GetEntity(j).M_CLOSE_REASON == "結清還款" ||
                    esPayMacro.GetEntity(j).M_CLOSE_REASON == "免清證手續-開清證" ||
                    esPayMacro.GetEntity(j).M_CLOSE_REASON == "債減-可開清證" ||
                    esPayMacro.GetEntity(j).M_CLOSE_REASON == "本金減免(專案)" ||
                    esPayMacro.GetEntity(j).M_CLOSE_REASON == "全額結清" ||
                    esPayMacro.GetEntity(j).M_CLOSE_REASON == "呆前本金減免餘額" ||
                    esPayMacro.GetEntity(j).M_CLOSE_REASON == "呆後債減結清" ||
                    esPayMacro.GetEntity(j).M_CLOSE_REASON == "債清前置協商結束" ||
                    esPayMacro.GetEntity(j).M_CLOSE_REASON == "債清更生結束" ||
                    esPayMacro.GetEntity(j).M_CLOSE_REASON == "債清清算成功"))
                {
                    continue;
                }
                else
                {
                    //* 需要confirm
                    strMsg = MessageHelper.GetMessage("04_02010100_030");
                    return true;
                }
            }
            return false;
        }
        else
        {
            //* 出錯了
            strMsg = MessageHelper.GetMessage("04_02010100_031") + "   " + MessageHelper.GetMessage(strShowMsg);
            return false;
        }
    }

    /// <summary>
    /// 檢查最後繳款日
    /// </summary>
    /// <param name="strUserID">客戶ID</param>
    /// <param name="strType">開立類型</param>
    /// <param name="dvCheckInfo"></param>
    /// <param name="dvJCEH"></param>
    /// <param name="strMsg">傳出的消息</param>
    /// <returns></returns>
    private bool CheckLastDate(string strUserID, string strType, DataView dvCheckInfo, DataView dvJCEH, ref string strMsg)
    {
        bool bMCode = false;
        string strHTGLastPayDate = "";
        string strLastPayDate = "";
        string strBLKDate = "";
        string strAltBLKDate = "";
        string strTempCloseDay = "";
        string strCardCloseDay = "";
        string strMacroLastPayDay = "";
        string strLogMsg = "";
        string strLogMsg2 = "";
        for (int i = 0; i < dvJCEH.Count; i++)
        {
            #region 檢查是否需要讀取MACRO的LastPayDate
            //* 檢查是否需要讀取MACRO的LastPayDate
            if (dvJCEH[i]["BLOCK"].ToString().Trim() == "M" || (dvJCEH[i]["BLOCK"].ToString().Trim() == "Z" && dvJCEH[i]["ALT_BLOCK"].ToString().Trim() == "M"))
            {
                //* 如果<1331電文.狀況碼> = M 或 (<1331電文.狀況碼>=Z AND <1331電文.前況碼>=M)
                if (IsSameMan(strUserID.Trim(), dvJCEH[i]["ACCT_CUST_ID"].ToString().Trim()))
                {
                    //* 如果开立的ID是归户ID
                    bMCode = true;
                }
            }
            #endregion

            #region 取得主機下傳的最後繳款日,最晚停卡日

            //* 卡+ML的 所有, CardOnly的非ML, ML Only的ML才會計算最後日期
            //* 取得主機下傳的LastPayDate
            if (dvJCEH[i]["BLOCK"].ToString() == "S" ||
               dvJCEH[i]["BLOCK"].ToString() == "B" ||
               dvJCEH[i]["BLOCK"].ToString() == "A" ||
               dvJCEH[i]["BLOCK"].ToString() == "M" ||
               dvJCEH[i]["BLOCK"].ToString() == "Z")
            {
                int iIsML = Function.IsML(dvJCEH[i]["CARDHOLDER"].ToString().Trim());
                if ((strType == "M") ||
                    ((strType == "C" || strType == "G" || strType == "O" || strType == "B") && iIsML != 1) ||
                    ((strType == "N" || strType == "T") && iIsML == 1))
                {
                    #region Log
                    strLogMsg2 = strLogMsg2 + "\r\n\r\n主機卡號:" + dvJCEH[i]["CARDHOLDER"].ToString().Trim() + ", ML類型:" + iIsML + ",TYPE:" + strType;
                    #endregion
                    #region 最後繳款日
                    //* 最後繳款日
                    if (Convert.ToInt32((dvJCEH[i]["DTE_LST_PYMT"].ToString().Trim() == "" ? "0" : dvJCEH[i]["DTE_LST_PYMT"].ToString().Trim()))
                        > Convert.ToInt32((strHTGLastPayDate.Trim() == "" ? "0" : strHTGLastPayDate)))
                    {
                        #region Log
                        strLogMsg2 = strLogMsg2 + " \r\nDTE_LST_PYMT=" + dvJCEH[i]["DTE_LST_PYMT"].ToString().Trim() + "大於現有:" + strHTGLastPayDate + " [最後繳款日]取:" + dvJCEH[i]["DTE_LST_PYMT"].ToString().Trim();
                        #endregion
                        strHTGLastPayDate = dvJCEH[i]["DTE_LST_PYMT"].ToString().Trim();
                    }
                    else
                    {
                        #region Log
                        strLogMsg2 = strLogMsg2 + " \r\nDTE_LST_PYMT=" + dvJCEH[i]["DTE_LST_PYMT"].ToString().Trim() + "小於現有:" + strHTGLastPayDate + " [最後繳款日]不變";
                        #endregion
                    }
                    #endregion
                    #region 最後停卡日
                    //* 最後停卡日
                    strBLKDate = BRPay_Card_Temp.ConvertBLKDate(dvJCEH[i]["BLOCK_DTE"].ToString().Trim());
                    strBLKDate = (strBLKDate == "" ? "0" : strBLKDate);
                    strAltBLKDate = BRPay_Card_Temp.ConvertBLKDate(dvJCEH[i]["ALT_BLOCK_DTE"].ToString().Trim());
                    strAltBLKDate = (strAltBLKDate == "" ? "0" : strAltBLKDate);
                    //* StrTempCloseDay = <1331電文.狀況碼日期>與<1331電文.前況碼日期>的大者
                    if (Convert.ToInt32(strBLKDate) >= Convert.ToInt32(strAltBLKDate))
                    {
                        strTempCloseDay = strBLKDate;
                        #region Log
                        strLogMsg2 = strLogMsg2 + " \r\nBLKDate:" + strBLKDate + "大於ALTBLKDATE:" + strAltBLKDate + " 取大者:" + strTempCloseDay;
                        #endregion
                    }
                    else
                    {
                        strTempCloseDay = strAltBLKDate;
                        #region Log
                        strLogMsg2 = strLogMsg2 + " \r\nBLKDate:" + strBLKDate + "小於ALTBLKDATE:" + strAltBLKDate + " 取大者:" + strTempCloseDay;
                        #endregion
                    }
                    if (Convert.ToInt32((strCardCloseDay.Trim() == "" ? "0" : strCardCloseDay))
                        < Convert.ToInt32((strTempCloseDay.Trim() == "" ? "0" : strTempCloseDay)))
                    {
                        #region Log
                        strLogMsg2 = strLogMsg2 + ", BLK大者:" + strTempCloseDay + "大於現有[最後停卡日]日期:" + strCardCloseDay + " [最後停卡日]取" + strTempCloseDay;
                        #endregion
                        strCardCloseDay = strTempCloseDay;
                    }
                    #endregion
                }
            }

            #endregion
        }

        #region 以主機的最後繳款日爲主,沒有的話取最後停卡日
        strLastPayDate = strHTGLastPayDate;
        #region Log
        strLogMsg2 = strLogMsg2 + "\r\n \r\n主機最後繳款日:" + strHTGLastPayDate + "最後停卡日:" + strCardCloseDay;
        #endregion
        if (string.IsNullOrEmpty(strLastPayDate))
        {
            #region Log
            strLogMsg2 = strLogMsg2 + "\r\n \r\n主機最後繳款日爲空,取最後停卡日:" + strCardCloseDay;
            #endregion
            strLastPayDate = strCardCloseDay;
        }
        #endregion
        #region Log
        Logging.Log(strLogMsg2, LogState.Debug, LogLayer.UI);
        #endregion

        #region 取主機最後繳款日和MACRO最後繳款日大者
        if (bMCode)
        {
            if (BRPay_Macro.GetLastPayDate(strUserID.Trim(), strType, ref strMsg, ref strMacroLastPayDay))
            {
                if (Convert.ToInt32(strMacroLastPayDay == "" ? "0" : strMacroLastPayDay) > Convert.ToInt32(strLastPayDate == "" ? "0" : strLastPayDate))
                {
                    strLastPayDate = strMacroLastPayDay;
                }
            }
            else
            {
                //* 出錯.傳出strMsg
                return false;
            }
        }
        #endregion

        #region 檢查SV傳遞來的最後繳款日和主機/MACRO大者是否相同
        for (int j = 0; j < dvCheckInfo.Count; j++)
        {
            if (dvCheckInfo[j]["ClearDate"].ToString().Trim() != strLastPayDate)
            {
                //* 卡號{0}最後結清日有誤
                strMsg = MessageHelper.GetMessage("04_02010100_060", dvCheckInfo[j]["CardNo"].ToString());
                #region Log
                //* 寫LOG
                strLogMsg += "\r\n" + "客戶ID:" + strUserID +
                            " , 卡號" + dvCheckInfo[j]["CardNo"].ToString() + "結清日有誤" +
                            " , SV:" + dvCheckInfo[j]["ClearDate"].ToString().Trim() +
                            " , 主機:" + strLastPayDate +
                            " , (主機最後繳款日:" + strHTGLastPayDate +
                            " , 主機停卡日:" + strCardCloseDay +
                            " , 是否連MACRO:" + bMCode.ToString() +
                            " , MACRO最後繳款日:" + strMacroLastPayDay + ")";
                Logging.Log(strLogMsg, LogState.Debug, LogLayer.UI);
                #endregion
                return true;
            }
        }
        #endregion
        return false;
    }

    /// <summary>
    /// 比较2个ID是否是同一人
    /// </summary>
    /// <param name="strCardID">ID1</param>
    /// <param name="strCardID2">ID2</param>
    /// <returns></returns>
    private bool IsSameMan(string strCardID, string strCardID2)
    {
        bool bResult = false;
        if (strCardID2.Trim().Length == 11)
        {
            if (strCardID.Trim() == strCardID2.Substring(1))
            {
                bResult = true;
            }
        }
        else
        {
            if (strCardID.Trim() == strCardID2.Trim())
            {
                bResult = true;
            }
        }
        return bResult;
    }

    /// <param name="strUserID">客戶ID</param>
    /// <param name="strCardTempID">序號</param>
    /// <param name="strMsg">錯誤信息</param>
    /// <param name="str1144Code">1144電文</param>
    /// <returns>True---檢核成功；False----檢核失敗</returns>
    public bool CheckJCFK(string strUserID, DataView dvJCEH, ref string strMsg, ref string str1144Code)
    {
        Hashtable htInput = new Hashtable();
        DataTable dtblOutput = new DataTable();
        string strCurrentCode = "";
        strMsg = "";
        str1144Code = "";

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
                        dvJCEH.RowFilter = "ALT_BLOCK = 'M' AND (BLOCK ='S' OR BLOCK ='B' OR BLOCK ='A' OR BLOCK ='M' OR BLOCK ='Z') ";
                        if (dvJCEH.Count > 0)
                        {
                            str1144Code = "M";
                            return true;
                        }
                    }
                }
            }

            return true;
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            etClientMstType = eClientMstType.Select;
            base.strClientMsg = MessageHelper.GetMessage("00_00000000_000");
            return false;
        }
    }
    #endregion
}
