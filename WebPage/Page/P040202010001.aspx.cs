//******************************************************************
//*  作    者：chaoma
//*  功能說明：債清客戶-資料查詢列印-債清證明統計表
//*  創建日期：2009/11/18
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
using System.Text.RegularExpressions;
using CSIPPayCertify.BusinessRules;
using CSIPPayCertify.EntityLayer;
using CSIPCommonModel.EntityLayer;
using CSIPCommonModel.BaseItem;
using Framework.Data.OM.Collections;
using Framework.Data.OM;
using Framework.WebControls;
using Framework.Common.Utility;
using Framework.Common.Message;
using Framework.Common.JavaScript;
using Framework.Common.Logging;
using Framework.Data;

public partial class Page_P040202010001 : PageBase
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Page.Title = BaseHelper.GetShowText("04_02020100_000");
        if (!Page.IsPostBack)
        {
            jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow(""));
            dpBeforeDate.Focus();

            //* 設定Tittle
            ShowControlsText();
            this.gpList.Visible = false;
            this.gpList.RecordCount = 0;
            this.grvDataView.Visible = false;
        }
    }


    /// <summary>
    /// 查詢
    /// 修改紀錄：報表產出改NPOI by Ares Stanley 20220120
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        try
        {

            if (dpBeforeDate.Text == "")
            {
                MessageHelper.ShowMessage(this.Page, "04_02020100_001");
                return;
            }
            if (dpEndDate.Text == "")
            {
                MessageHelper.ShowMessage(this.Page, "04_02020100_002");
                return;
            }

            if (dpBeforeDate.Text != "" && dpEndDate.Text != "")
            {
                DateTime dtmBeforeData = Convert.ToDateTime(dpBeforeDate.Text.Trim());
                DateTime dtmEndData = Convert.ToDateTime(dpEndDate.Text.Trim());
                if (!BRReport.CheckDataTime(dtmBeforeData, dtmEndData))
                {
                    MessageHelper.ShowMessage(this.Page, "04_02020100_000");
                    return;
                }
            }

            //* 開立日期(起)
            string strRptBeforeDate = this.dpBeforeDate.Text.Replace("/", "");
            //* 開立日期(迄)
            string strRptEndDate = this.dpEndDate.Text.Replace("/", "");
            string strMsgID = "";
            DataSet dstResult = new DataSet();
            //* 生成報表
            //20220223_Ares_Jack_取得參數判斷是S：查詢或P：列印
            bool blnReport = BRReport.Report02020100(strRptBeforeDate, strRptEndDate, ref strMsgID, ref dstResult, 0, 0, "P");
            //* 生成報表成功
            if (blnReport)
            {

                if (dstResult != null && dstResult.Tables[1].Rows.Count > 0)
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
                return;
            }


            if (dstResult != null)
            {
                //移除多餘參數 by Ares Stanley 20220214
                string strServerPathFile = this.Server.MapPath(UtilHelper.GetAppSettings("ExportExcelFilePath").ToString());
                if (!BR_Excel_File.CreateExcelFile_Report02020100(dstResult, strRptBeforeDate, strRptEndDate, ref strServerPathFile, ref strMsgID))
                {
                    jsBuilder.RegScript(this.Page, string.Format("AlertConfirm({{title:'{0}'}});", MessageHelper.GetMessage("04_00000000_058")));
                    return;
                }
                //* 將服務器端生成的文檔，下載到本地。
                string strYYYYMMDD = "000" + Function.MinGuoDate7length(DateTime.Now.ToString("yyyyMMdd"));
                strYYYYMMDD = strYYYYMMDD.Substring(strYYYYMMDD.Length - 8, 8);
                string strFileName = "債清客戶債清證明統計表" + strYYYYMMDD + ".xls";

                //* 顯示提示訊息：匯出到Excel文檔資料成功
                this.Session["ServerFile"] = strServerPathFile;
                this.Session["ClientFile"] = strFileName;
                string urlString = @"window.parent.postMessage({ func: 'ClientMsgShow', data: '" + MessageHelper.GetMessage("04_00000000_057") + "' }, '*');";
                urlString += @"location.href='DownLoadFile.aspx';";
                jsBuilder.RegScript(this.Page, urlString);
            }

        }
        catch (Exception exp)
        {
            Framework.Common.Logging.Logging.Log(exp, Framework.Common.Logging.LogLayer.UI);
            MessageHelper.ShowMessage(this.Page, "00_00000000_000");
            return;
        }
    }


    /// <summary>
    /// 專案代號:20210058-CSIP 作業服務平台現代化II
    /// 功能說明:業務新增查詢功能
    /// 作    者:Ares Stanley
    /// 修改時間:2022/01/20
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        BindGridView();
    }

    /// <summary>
    /// 專案代號:20210058-CSIP作業服務平台現代化II
    /// 功能說明:綁定畫面資料
    /// 作    者:Ares Stanley
    /// 修改時間:2022/01/20
    /// </summary>
    private void BindGridView()
    {
        try
        {

            if (dpBeforeDate.Text == "")
            {
                MessageHelper.ShowMessage(this.Page, "04_02020100_001");
                return;
            }
            if (dpEndDate.Text == "")
            {
                MessageHelper.ShowMessage(this.Page, "04_02020100_002");
                return;
            }

            if (dpBeforeDate.Text != "" && dpEndDate.Text != "")
            {
                DateTime dtmBeforeData = Convert.ToDateTime(dpBeforeDate.Text.Trim());
                DateTime dtmEndData = Convert.ToDateTime(dpEndDate.Text.Trim());
                if (!BRReport.CheckDataTime(dtmBeforeData, dtmEndData))
                {
                    MessageHelper.ShowMessage(this.Page, "04_02020100_000");
                    return;
                }
            }

            //* 開立日期(起)
            string strRptBeforeDate = this.dpBeforeDate.Text.Replace("/", "");
            //* 開立日期(迄)
            string strRptEndDate = this.dpEndDate.Text.Replace("/", "");
            string strMsgID = "";
            DataSet dstResult = new DataSet();
            string sqlcmd = "";
            DataSet PagData = new DataSet();
            int TotalCount = 0;
            //* 生成報表
            bool blnReport = BRReport.Report02020100(strRptBeforeDate, strRptEndDate, ref strMsgID, ref dstResult,ref sqlcmd);
            //* 生成報表成功
            if (blnReport)
            {
                if (dstResult.Tables[1].Rows.Count > 0)
                {
                    DataHelper dh = new DataHelper();
                    try
                    {
                        //    dh.ExecuteDataSet(sqlcmd, this.gpList.CurrentPageIndex, this.gpList.PageSize, ref TotalCount);
                         BRReport.Report02020100(strRptBeforeDate, strRptEndDate, ref strMsgID, ref PagData, this.gpList.CurrentPageIndex, this.gpList.PageSize); 
                    }
                    catch (Exception exp)
                    {
                        Logging.Log(exp, LogLayer.UI);
                        MessageHelper.ShowMessage(this.Page, "00_00000000_000");
                    }
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
                return;
            }


            DataTable dtPay_SV = dstResult.Tables[1];
            if (dtPay_SV.Rows.Count > 0)
            {
                BR_Excel_File.removeBlank(ref dtPay_SV);
                this.gpList.Visible = true;
                this.gpList.RecordCount = dtPay_SV.Rows.Count;
                this.grvDataView.Visible = true;
                this.grvDataView.DataSource = PagData.Tables[1];
                this.grvDataView.DataBind();
                jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow("04_00000000_054"));//報表查詢成功
            }
            else
            {
                this.gpList.RecordCount = 0;
                this.grvDataView.DataSource = null;
                this.grvDataView.DataBind();
                this.gpList.Visible = false;
                this.grvDataView.Visible = false;
                jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("04_00000000_056"));
                jsBuilder.RegScript(this.Page, string.Format("AlertConfirm({{title:'{0}'}});", MessageHelper.GetMessage("04_00000000_056")));//無資料
            }

        }
        catch (Exception exp)
        {
            Framework.Common.Logging.Logging.Log(exp, Framework.Common.Logging.LogLayer.UI);
            MessageHelper.ShowMessage(this.Page, "00_00000000_000");
            return;
        }
    }

    /// <summary>
    /// 專案代號:20210058-CSIP 作業服務平台現代化II
    /// 功能說明:業務新增查詢切換頁需求功能
    /// 作    者:Ares Stanley
    /// 修改時間:2022/01/20
    /// </summary>
    protected void gpList_PageChanged(object src, Framework.WebControls.PageChangedEventArgs e)
    {
        gpList.CurrentPageIndex = e.NewPageIndex;
        BindGridView();
    }

    /// <summary>
    /// 專案代號:20210058-CSIP 作業服務平台現代化II
    /// 功能說明:業務新增查詢標頭需求功能
    /// 作    者:Ares Stanley
    /// 修改時間:2022/01/20
    /// </summary>
    protected void ShowControlsText()
    {
        {
            //* 設置查詢結果GridView的列頭標題
            this.grvDataView.Columns[0].HeaderText = BaseHelper.GetShowText("04_02020100_023"); //項次
            this.grvDataView.Columns[1].HeaderText = BaseHelper.GetShowText("04_02020100_006"); //身分證字號
            this.grvDataView.Columns[2].HeaderText = BaseHelper.GetShowText("04_02020100_007"); //客戶姓名
            this.grvDataView.Columns[3].HeaderText = BaseHelper.GetShowText("04_02020100_008"); //清證序號
            this.grvDataView.Columns[4].HeaderText = BaseHelper.GetShowText("04_02020100_009"); //寄送地址
            this.grvDataView.Columns[5].HeaderText = BaseHelper.GetShowText("04_02020100_010"); //收件者
            this.grvDataView.Columns[6].HeaderText = BaseHelper.GetShowText("04_02020100_011"); //結清日
            this.grvDataView.Columns[7].HeaderText = BaseHelper.GetShowText("04_02020100_012"); //結案日
            this.grvDataView.Columns[8].HeaderText = BaseHelper.GetShowText("04_02020100_013"); //證明種類
            this.grvDataView.Columns[9].HeaderText = BaseHelper.GetShowText("04_02020100_014"); //卡號
            this.grvDataView.Columns[10].HeaderText = BaseHelper.GetShowText("04_02020100_015");//BLK
            this.grvDataView.Columns[11].HeaderText = BaseHelper.GetShowText("04_02020100_016");//卡別
            this.grvDataView.Columns[12].HeaderText = BaseHelper.GetShowText("04_02020100_017");//附卡人ID
            this.grvDataView.Columns[13].HeaderText = BaseHelper.GetShowText("04_02020100_018");//附卡人姓名
            this.grvDataView.Columns[14].HeaderText = BaseHelper.GetShowText("04_02020100_019");//協商狀態
            this.grvDataView.Columns[15].HeaderText = BaseHelper.GetShowText("04_02020100_020");//單獨受償
            this.grvDataView.Columns[16].HeaderText = BaseHelper.GetShowText("04_02020100_021");//申請經辦
            this.grvDataView.Columns[17].HeaderText = BaseHelper.GetShowText("04_02020100_022");//放行主管

            //* 設置一頁顯示最大筆數
            this.gpList.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize"));
            this.grvDataView.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize"));
        }
    }
}
