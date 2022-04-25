//******************************************************************
//*  作    者：chaoma
//*  功能說明：大宗掛號報表

//*  創建日期：2009/12/04
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

public partial class Page_P040102020001 : PageBase
{
    #region event
    protected void Page_Load(object sender, EventArgs e)
    {
        //*標題文字顯示
        Page.Title = BaseHelper.GetShowText("04_01020200_000");
        if (!Page.IsPostBack)
        {
            jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow(""));
            Set_MailType();
            lblMailDay.Focus();

            //* 設定Tittle
            ShowControlsText();
            this.gpList.Visible = false;
            this.gpList.RecordCount = 0;
            this.grvDataView.Visible = false;
        }
    }
    /// <summary>
    /// 修改紀錄：報表產出改NPOI by Ares Stanley 20211220
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnPrint_Click(object sender, EventArgs e)
    {
        try
        {
            string strMsg = "";
            //*檢查輸入欄位合法性
            if (!Checkcondition(ref strMsg))
            {
                //* 檢核不成功時，提示不符訊息
                MessageHelper.ShowMessage(this.UpdatePanel1, strMsg);
                return;
            }
            //*檢核成功后操作
            this.ViewState["SearchMailDay"] = dpMailDay.Text.Trim().Replace("/", "");
            this.ViewState["SearchMailType"] = dropMailType.SelectedValue;

            string strMAILDAY = this.ViewState["SearchMailDay"].ToString();
            string strMAILTYPE = this.ViewState["SearchMailType"].ToString();
            //*傳入變量判斷
            if (strMAILDAY == null || strMAILTYPE == null)
            {
                jsBuilder.RegScript(this.Page, string.Format("AlertConfirm({{title:'{0}'}});", MessageHelper.GetMessage("04_00000000_056")));
            }
            if ("01".Equals(strMAILTYPE))
            {
                strMAILTYPE = BaseHelper.GetShowText("04_01020200_005");
            }
            else
            {
                strMAILTYPE = BaseHelper.GetShowText("04_01020200_006");
            }


            string strMsgID = "";
            //*生成報表
            DataSet dstResult = new DataSet();
            //20220223_Ares_Jack_取得參數判斷是S：查詢或P：列印
            bool blnReport = BRReport.Report01020200(strMAILDAY, strMAILTYPE, ref strMsgID, ref dstResult, "P");

            if (blnReport)
            {
                if (dstResult.Tables.Count <= 0)
                {
                    //無資料
                    jsBuilder.RegScript(this.Page, string.Format("AlertConfirm({{title:'{0}'}});", MessageHelper.GetMessage("04_00000000_056")));
                    return;
                }

                if (dstResult.Tables[0].Rows.Count > 0)
                {
                }
                else
                {
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

            string strServerPathFile = this.Server.MapPath(UtilHelper.GetAppSettings("ExportExcelFilePath").ToString());
            if (!BR_Excel_File.CreateExcelFile_Report01020200(dstResult, strMAILDAY, strMAILTYPE, ref strServerPathFile, ref strMsgID))
            {
                jsBuilder.RegScript(this.Page, string.Format("AlertConfirm({{title:'{0}'}});", MessageHelper.GetMessage("04_00000000_058")));
                return;
            }
            //* 將服務器端生成的文檔，下載到本地。
            string strYYYYMMDD = "000" + Function.MinGuoDate7length(DateTime.Now.ToString("yyyyMMdd"));
            strYYYYMMDD = strYYYYMMDD.Substring(strYYYYMMDD.Length - 8, 8);
            string strFileName = "清償證明大宗掛號函件存根聯" + strYYYYMMDD + ".xls";

            //* 顯示提示訊息：匯出到Excel文檔資料成功
            this.Session["ServerFile"] = strServerPathFile;
            this.Session["ClientFile"] = strFileName;
            string urlString = @"window.parent.postMessage({ func: 'ClientMsgShow', data: '" + MessageHelper.GetMessage("04_00000000_057") + "' }, '*');";
            urlString += @"location.href='DownLoadFile.aspx';";
            jsBuilder.RegScript(this.Page, urlString);
        }
        catch(Exception ex)
        {
            Logging.Log(ex);
            MessageHelper.ShowMessage(this.Page, "04_00000000_058");
        }
    }


    /// <summary>
    /// 專案代號:20210058-CSIP 作業服務平台現代化II
    /// 功能說明:業務新增查詢功能
    /// 作    者:Ares Stanley
    /// 修改時間:2021/12/20
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
    /// 修改時間:2021/12/20
    /// </summary>
    private void BindGridView()
    {
        try
        {
            string strMsg = "";
            //*檢查輸入欄位合法性
            if (!Checkcondition(ref strMsg))
            {
                //* 檢核不成功時，提示不符訊息
                MessageHelper.ShowMessage(this.UpdatePanel1, strMsg);
                return;
            }
            //*檢核成功后操作
            this.ViewState["SearchMailDay"] = dpMailDay.Text.Trim().Replace("/", "");
            this.ViewState["SearchMailType"] = dropMailType.SelectedValue;

            string strMAILDAY = this.ViewState["SearchMailDay"].ToString();
            string strMAILTYPE = this.ViewState["SearchMailType"].ToString();
            //*傳入變量判斷
            if (strMAILDAY == null || strMAILTYPE == null)
            {
                jsBuilder.RegScript(this.Page, string.Format("AlertConfirm({{title:'{0}'}});", MessageHelper.GetMessage("04_00000000_056")));
            }
            if ("01".Equals(strMAILTYPE))
            {
                strMAILTYPE = BaseHelper.GetShowText("04_01020200_005");
            }
            else
            {
                strMAILTYPE = BaseHelper.GetShowText("04_01020200_006");
            }


            string strMsgID = "";
            //*生成報表
            DataSet dstResult = new DataSet();
            string sqlcmd = "";
            bool blnReport = BRReport.Report01020200(strMAILDAY, strMAILTYPE, ref strMsgID, ref dstResult,ref sqlcmd);

            DataSet PagData = new DataSet();
            int TotalCount = 0;
            if (blnReport)
            {
                if (dstResult.Tables.Count <= 0)
                {
                    //無資料
                    jsBuilder.RegScript(this.Page, string.Format("AlertConfirm({{title:'{0}'}});", MessageHelper.GetMessage("04_00000000_056")));
                    return;
                }
                else
                {
                    DataHelper dh = new DataHelper();
                    try
                    {
                        PagData = dh.ExecuteDataSet(sqlcmd, this.gpList.CurrentPageIndex, this.gpList.PageSize, ref TotalCount);
                    }
                    catch (Exception exp)
                    {
                        Logging.Log(exp, LogLayer.UI);
                        MessageHelper.ShowMessage(this.Page, "00_00000000_000");
                    }
                }
                if (dstResult.Tables[0].Rows.Count > 0)
                {
                }
                else
                {
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

            DataTable dt = PagData.Tables[0];
            if (dt.Rows.Count > 0)
            {
                BR_Excel_File.removeBlank(ref dt);
                this.gpList.Visible = true;
                this.gpList.RecordCount = dstResult.Tables[0].Rows.Count;
                this.grvDataView.Visible = true;
                //組fullAddress
                dt.Columns.Add("fullAddress", typeof(String));
                //dt.Columns.Add("note", typeof(String));
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    dt.Rows[i]["fullAddress"] = dt.Rows[i]["zip"].ToString() + dt.Rows[i]["add1"].ToString() + dt.Rows[i]["add2"].ToString() + dt.Rows[i]["add3"].ToString();
                }

                this.grvDataView.DataSource = dt;
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
        catch (Exception ex)
        {
            Logging.Log(ex);
            MessageHelper.ShowMessage(this.Page, "04_00000000_055");
        }
    }

    /// <summary>
    /// 專案代號:20210058-CSIP 作業服務平台現代化II
    /// 功能說明:業務新增查詢切換頁需求功能
    /// 作    者:Ares Stanley
    /// 修改時間:2021/12/20
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
    /// 修改時間:2021/12/20
    /// </summary>
    protected void ShowControlsText()
    {
        {
            //* 設置查詢結果GridView的列頭標題
            this.grvDataView.Columns[0].HeaderText = BaseHelper.GetShowText("04_03020600_010");//項次
            this.grvDataView.Columns[1].HeaderText = BaseHelper.GetShowText("04_03020600_011");//掛號號碼
            this.grvDataView.Columns[2].HeaderText = BaseHelper.GetShowText("04_03020600_012");//姓名
            this.grvDataView.Columns[3].HeaderText = BaseHelper.GetShowText("04_03020600_013");//詳細地址
            this.grvDataView.Columns[4].HeaderText = BaseHelper.GetShowText("04_03020600_014");//備註

            //* 設置一頁顯示最大筆數
            this.gpList.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize"));
            this.grvDataView.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize"));
        }
    }
    #endregion event

    #region method
    /// <summary>
    /// 輸入驗證
    /// </summary>
    /// <param name="strMsg"></param>
    /// <returns></returns>
    private bool Checkcondition(ref string strMsg)
    {
        //*郵寄日期欄位判斷
        if (this.dpMailDay.Text.Trim() != "")
        {
            DateTime dtmMailDayOut = DateTime.Parse("1900-01-01");
            if (!Function.IsDateTime(this.dpMailDay.Text.Trim(), out dtmMailDayOut))
            {
                strMsg = "04_01020200_001";
                return false;
            }
        }
        else
        {
            strMsg = "04_01020200_002";
            return false;
        }
        return true;
    }
    /// <summary>
    /// 郵寄方式欄位設置
    /// </summary>
    private void Set_MailType()
    {
        dropMailType.Items.Add(new ListItem(BaseHelper.GetShowText("04_01020200_005"),"01"));
        dropMailType.Items.Add(new ListItem(BaseHelper.GetShowText("04_01020200_006"),"02"));
        dropMailType.Items[0].Selected = true;
    }
    #endregion method
}
