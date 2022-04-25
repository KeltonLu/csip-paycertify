//******************************************************************
//*  作    者：宋戈(Ge.Song)
//*  功能說明：新增Macro頁面
//*  創建日期：2009/10/26
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
using CSIPPayCertify.BusinessRules;
using CSIPPayCertify.EntityLayer;
using Framework.Data.OM.Collections;
using Framework.Common.Message;
using Framework.Data.OM;
using Framework.Common.Utility;
using System.Text.RegularExpressions;
using System.Text;
using Framework.Common.JavaScript;
using CSIPCommonModel.BaseItem;
using Framework.Common.Logging;

public partial class Page_P040101010003 : PageBase
{
    #region 宣告變數
    private double dblBalance;
    private double dblDebtBalance;
    private double dblTtlPayment;
    #endregion

    #region Event
    /// <summary>
    /// 載入畫面
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            string strMsgID = "";

            //* 清空末端訊息
            jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow(""));
            //* 設置文字顯示
            ShowControlsText();

            ViewState["UserID"] = (RedirectHelper.GetDecryptString(this.Page, "UserID") + "").Trim();
            ViewState["Type"] = (RedirectHelper.GetDecryptString(this.Page, "Type") + "").Trim();
            ViewState["CardType"] = (RedirectHelper.GetDecryptString(this.Page, "CardType") + "").Trim();
            ViewState["No"] = (RedirectHelper.GetDecryptString(this.Page, "No") + "").Trim();
            ViewState["Other"] = (RedirectHelper.GetDecryptString(this.Page, "Other") + "").Trim();
            ViewState["OtherID"] = (RedirectHelper.GetDecryptString(this.Page, "OtherID") + "").Trim();
            ViewState["CardTmpID"] = (RedirectHelper.GetDecryptString(this.Page, "CardTmpID") + "").Trim();
            ViewState["CardOpenDay"] = (RedirectHelper.GetDecryptString(this.Page, "CardOpenDay") + "").Trim();
            ViewState["CardCloseDay"] = (RedirectHelper.GetDecryptString(this.Page, "CardCloseDay") + "").Trim();
            ViewState["ML2PLCardList"] = (RedirectHelper.GetDecryptString(this.Page, "ML2PLCardList") + "").Trim();

            if (!CheckRedirect(ref strMsgID))
            {
                //* 出错
                MessageHelper.ShowMessageAndGoto(this.UpdatePanel1, "P040101010001.aspx", strMsgID);
                return;
            }
            //* 綁定資料列
            InitalGridData();
        }

    }

    /// <summary>
    /// 綁定Gird時增加統計
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void grvSetPayCertify_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            dblBalance = dblBalance + Convert.ToDouble(e.Row.Cells[4].Text.Trim());
            dblDebtBalance = dblDebtBalance + Convert.ToDouble(e.Row.Cells[7].Text.Trim());
            dblTtlPayment = dblTtlPayment + Convert.ToDouble(e.Row.Cells[9].Text.Trim());
            if (e.Row.Cells[5].Text.Trim().Length == 8)
            {
                e.Row.Cells[5].Text = e.Row.Cells[5].Text.Substring(0, 4) + @"/" + e.Row.Cells[5].Text.Substring(4, 2) + @"/" + e.Row.Cells[5].Text.Substring(6, 2);
            }
            if (e.Row.Cells[8].Text.Trim().Length == 8)
            {
                e.Row.Cells[8].Text = e.Row.Cells[8].Text.Substring(0, 4) + @"/" + e.Row.Cells[8].Text.Substring(4, 2) + @"/" + e.Row.Cells[8].Text.Substring(6, 2);
            }
            if (e.Row.Cells[10].Text.Trim().Length == 8)
            {
                e.Row.Cells[10].Text = e.Row.Cells[10].Text.Substring(0, 4) + @"/" + e.Row.Cells[10].Text.Substring(4, 2) + @"/" + e.Row.Cells[10].Text.Substring(6, 2);
            }
        }

        if (e.Row.RowType == DataControlRowType.Footer)
        {
            e.Row.Cells[0].Text = BaseHelper.GetShowText("04_01010100_057");
            e.Row.Cells[4].Text = dblBalance.ToString();
            e.Row.Cells[7].Text = dblDebtBalance.ToString();
            e.Row.Cells[9].Text = dblTtlPayment.ToString();
            if ((((EntitySet<EntityPay_Macro>)this.grvSetPayCertify.DataSource).Count % 2) == 0)
            {
                e.Row.CssClass = "Grid_Item";
            }
            else
            {
                e.Row.CssClass = "Grid_AlternatingItem";
            }
        }

    }

    /// <summary>
    /// 點擊[列印]按鈕開始列印
    /// 修改紀錄：報表產出改NPOI by Ares Jack 20220406; 調整參數、Log紀錄 by Ares Stanley 20220407
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnPrint_Click(object sender, EventArgs e)
    {
        //string strURL = "P040101010004.aspx?Cust_ID=" + RedirectHelper.GetEncryptParam(this.ViewState["UserID"].ToString());

        ////* 打開列印簽收總表畫面
        //jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.GetScriptForWindowOpenURL(this.Page, strURL));

        try
        {

            //*傳入客戶ID信息
            string strUserID = this.ViewState["UserID"].ToString();
            string strMsgID = "";

            DataSet dstResult = new DataSet();

            if (BRReport.Report01010100(strUserID, ref strMsgID, ref dstResult, "P"))
            {
                if (dstResult.Tables[0].Rows.Count > 0)
                {
                }
                else
                {
                    //無資料
                    jsBuilder.RegScript(this.form1, string.Format("AlertConfirm({{title:'{0}'}});", MessageHelper.GetMessage("04_00000000_056")));
                    return;
                }
            }
            else
            {
                //查詢失敗
                jsBuilder.RegScript(this.form1, string.Format("AlertConfirm({{title:'{0}'}});", MessageHelper.GetMessage("04_00000000_055")));
                return;
            }



            if (dstResult != null)
            {
                string strServerPathFile = this.Server.MapPath(UtilHelper.GetAppSettings("ExportExcelFilePath").ToString());
                if (!BR_Excel_File.CreateExcelFile_Report0101010003(dstResult, ref strServerPathFile, ref strMsgID))
                {
                    jsBuilder.RegScript(this.Page, string.Format("AlertConfirm({{title:'{0}'}});", MessageHelper.GetMessage("04_00000000_058")));
                    return;
                }

                //* 將服務器端生成的文檔，下載到本地。
                string strYYYYMMDD = "000" + Function.MinGuoDate7length(DateTime.Now.ToString("yyyyMMdd"));
                strYYYYMMDD = strYYYYMMDD.Substring(strYYYYMMDD.Length - 8, 8);
                string strFileName = "新增清償證明" + strYYYYMMDD + ".xls";

                //* 顯示提示訊息：匯出到Excel文檔資料成功
                this.Session["ServerFile"] = strServerPathFile;
                this.Session["ClientFile"] = strFileName;
                string urlString = @"window.parent.postMessage({ func: 'ClientMsgShow', data: '" + MessageHelper.GetMessage("04_00000000_057") + "' }, '*');";
                urlString += @"location.href='DownLoadFile.aspx';";
                jsBuilder.RegScript(this.Page, urlString);
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
            MessageHelper.ShowMessage(this.Page, "04_00000000_058");
        }
    }

    /// <summary>
    /// 點擊[離開]按鈕,判斷是否有權限開立清證
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        EntityPay_MacroSet esMacro = new EntityPay_MacroSet();
        string strMsgID = "";
        bool bPassFlag = false;
        string strCloseReason;
        string strUrl = "P040101010002.aspx?" +
                            "UserID=" + RedirectHelper.GetEncryptParam(ViewState["UserID"].ToString().Trim()) +
                            "&Type=" + RedirectHelper.GetEncryptParam(ViewState["Type"].ToString().Trim()) +
                            "&CardType=" + RedirectHelper.GetEncryptParam(ViewState["CardType"].ToString().Trim()) +
                            "&No=" + RedirectHelper.GetEncryptParam(ViewState["No"].ToString().Trim()) +
                            "&Other=" + RedirectHelper.GetEncryptParam(ViewState["Other"].ToString().Trim()) +
                            "&OtherID=" + RedirectHelper.GetEncryptParam(ViewState["OtherID"].ToString().Trim()) +
                            "&CardTmpID=" + RedirectHelper.GetEncryptParam(ViewState["CardTmpID"].ToString().Trim()) +
                            "&CardOpenDay=" + RedirectHelper.GetEncryptParam(ViewState["CardOpenDay"].ToString().Trim()) +
                            "&CardCloseDay=" + RedirectHelper.GetEncryptParam(ViewState["CardCloseDay"].ToString().Trim()) +
                            "&ML2PLCardList=" + RedirectHelper.GetEncryptParam(ViewState["ML2PLCardList"].ToString().Trim()) +
                            "";

        if (BRPay_Macro.GetDataByCustID(ViewState["UserID"].ToString().Trim(), ref strMsgID, ref esMacro))
        {
            for (int i = 0; i <= esMacro.Count - 1; i++)
            {
                if (esMacro.GetEntity(i).M_CLOSE_DATE != "")
                {
                    strCloseReason = esMacro.GetEntity(i).M_CLOSE_REASON;
                    if (strCloseReason == "結清還款" ||
                        strCloseReason == "免清證手續-開清證" ||
                        strCloseReason == "債減-可開清證" ||
                        strCloseReason == "本金減免(專案)"||
                        strCloseReason == "全額結清" ||
                        strCloseReason == "呆前本金減免餘額" ||
                        strCloseReason == "呆後債減結清" ||
                        strCloseReason == "債清前置協商結束" ||
                        strCloseReason == "債清更生結束" ||
                        strCloseReason == "債清清算成功")
                    {
                        //* 如果esMacro.M_Close_Reason = 結清還款 或 免清證手續-開清證 或 債減-可開清證 或 本金減免(專案)
                        //* 2010/02/02 add 全額結清 , 呆前本金減免餘額, 呆後債減結清,債清前置協商結束,債清更生結束,債清清算成功
                        bPassFlag = true;
                    }
                    else
                    {
                        bPassFlag = false;
                        break;
                    }

                }
                else
                {
                    bPassFlag = false;
                    break;
                }
            }
        
            if (bPassFlag || esMacro.Count <= 0)
            {
                //*跳轉到新增明細畫面P040101000002.ASPX
                Response.Redirect(strUrl);
            }
            else
            {
                //* Confirm(沒有結案日期/結案原因不符結案條件,是否要強制開立證明)
                //* 選擇”確定”跳轉到新增明細畫面P040101010002.ASPX
                //* 選擇”取消”跳轉到新增查詢畫面P040101010001.ASPX
                StringBuilder sb = new StringBuilder();
                sb.Append("if(confirm('").Append(MessageHelper.GetMessage("04_01010100_027")).Append("') == false)");
                sb.Append("{");
                sb.Append("gotoWin('P040101010001.aspx');");
                sb.Append("}else{");
                sb.Append("gotoWin('").Append(strUrl).Append("');");
                sb.Append("}");
                jsBuilder.RegScript(this.UpdatePanel1, sb.ToString());
            }            

        }
        else
        {
            //* 出錯了
            MessageHelper.ShowMessageAndGoto(this.UpdatePanel1, "P040101010001.aspx", strMsgID);
            return;
        }
    }

    #endregion

    #region Function
    /// <summary>
    /// 設置Grid標題
    /// </summary>
    /// <returns></returns>
    private bool ShowControlsText()
    {
        //* 列印日期 = 當天
        this.lblDate.Text = DateTime.Today.ToString("yyyy/MM/dd");
        //* Gird
        this.grvSetPayCertify.Columns[0].HeaderText = BaseHelper.GetShowText("04_01010100_045");
        this.grvSetPayCertify.Columns[1].HeaderText = BaseHelper.GetShowText("04_01010100_046");
        this.grvSetPayCertify.Columns[2].HeaderText = BaseHelper.GetShowText("04_01010100_047");
        this.grvSetPayCertify.Columns[3].HeaderText = BaseHelper.GetShowText("04_01010100_048");
        this.grvSetPayCertify.Columns[4].HeaderText = BaseHelper.GetShowText("04_01010100_049");
        this.grvSetPayCertify.Columns[5].HeaderText = BaseHelper.GetShowText("04_01010100_050");
        this.grvSetPayCertify.Columns[6].HeaderText = BaseHelper.GetShowText("04_01010100_051");
        this.grvSetPayCertify.Columns[7].HeaderText = BaseHelper.GetShowText("04_01010100_052");
        this.grvSetPayCertify.Columns[8].HeaderText = BaseHelper.GetShowText("04_01010100_053");
        this.grvSetPayCertify.Columns[9].HeaderText = BaseHelper.GetShowText("04_01010100_054");
        this.grvSetPayCertify.Columns[10].HeaderText = BaseHelper.GetShowText("04_01010100_055");
        this.grvSetPayCertify.Columns[11].HeaderText = BaseHelper.GetShowText("04_01010100_056");

        return true;
    }

    /// <summary>
    /// Grid查詢綁定資料
    /// </summary>
    /// <returns></returns>
    private bool InitalGridData()
    {
        EntityPay_MacroSet esMacro = new EntityPay_MacroSet();
        string strMsgID = "";
        if (!BRPay_Macro.GetDataByCustID(ViewState["UserID"].ToString().Trim(), ref strMsgID, ref esMacro))
        {
            //* 出錯了
        }
        else
        {
            this.grvSetPayCertify.DataSource = esMacro;
            this.grvSetPayCertify.DataBind();
        }
        return true;
    }


    /// <summary>
    /// 檢查跳轉需要輸入的欄位是否有輸入
    /// </summary>
    /// <param name="strMsgID"></param>
    /// <returns></returns>
    private bool CheckRedirect(ref string strMsgID)
    {
        string strUserID = ViewState["UserID"].ToString().Trim();
        string strType = ViewState["Type"].ToString().Trim();
        string strCardType = ViewState["CardType"].ToString().Trim();
        string strNo = ViewState["No"].ToString().Trim();
        string strOther = ViewState["Other"].ToString().Trim();
        string strOtherID = ViewState["OtherID"].ToString().Trim();
        string strCardTmpID = ViewState["CardTmpID"].ToString().Trim();
        string strCardOpenDay = ViewState["CardOpenDay"].ToString().Trim();
        string strCardCloseDay = ViewState["CardCloseDay"].ToString().Trim();

        if (String.IsNullOrEmpty(strUserID.Trim()))
        {
            //* "請輸入客戶ID";
            strMsgID = "04_01010100_000";
            return false;
        }
        else
        {
            //* 如果身分證不是合法的格式
            if (!Regex.IsMatch(strUserID.Trim(), BaseHelper.GetShowText("04_00000000_000")))
            {
                strMsgID = "04_01010100_001";
                return false;
            }
        }

        if (String.IsNullOrEmpty(strType.Trim()))
        {
            //"請選擇開立種類";
            strMsgID = "04_01010100_002";
            return false;
        }
        else
        {
            if (strType.Trim() != "C" &&
                strType.Trim() != "M" &&
                strType.Trim() != "G" &&
                strType.Trim() != "O" &&
                strType.Trim() != "B" &&
                strType.Trim() != "N" &&
                strType.Trim() != "T")
            {
                //"請選擇開立種類";
                strMsgID = "04_01010100_002";
                return false;
            }
        }

        if (String.IsNullOrEmpty(strNo.Trim()))
        {
            //"請選擇開立種類";
            strMsgID = "04_01010100_002";
            return false;
        }
        else
        {
            if (strNo.Trim() != "1" &&
                strNo.Trim() != "2" &&
                strNo.Trim() != "3" &&
                strNo.Trim() != "4")
            {
                //"請選擇開立種類";
                strMsgID = "04_01010100_002";
                return false;
            }
        }

        if (String.IsNullOrEmpty(strCardType.Trim()))
        {
            //"請選擇開立種類";
            strMsgID = "04_01010100_002";
            return false;
        }
        else
        {
            if (strCardType.Trim() != "0" &&
                strCardType.Trim() != "1" &&
                strCardType.Trim() != "2" &&
                strCardType.Trim() != "3")
            {
                //"請選擇開立類型";
                strMsgID = "04_01010100_003";
                return false;
            }
        }

        if (strCardType.Trim() == "3" && String.IsNullOrEmpty(strOtherID))
        {
            //"請輸入 他人附卡其正卡人ID!";
            strMsgID = "04_01010100_004";
            return false;
        }

        if (String.IsNullOrEmpty(strCardTmpID.Trim()))
        {
            //* CardTempID不能爲空
            strMsgID = "04_01010100_018";
            return false;
        }
        if (String.IsNullOrEmpty(strCardOpenDay.Trim()))
        {
            //* CardOpenDay不能爲空
            strMsgID = "04_01010100_019";
            return false;
        }
        if (String.IsNullOrEmpty(strCardCloseDay.Trim()))
        {
            //* CardCloseDay不能爲空
            strMsgID = "04_01010100_020";
            return false;
        }

        return true;
    }

    #endregion


}
