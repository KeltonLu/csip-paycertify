//******************************************************************
//*  作    者：余洋
//*  功能說明：匯入清單明細
//*  創建日期：2009/12/02
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
//* Ares Stanley    2022/02/14    20210058-CSIP作業服務平台現代化II    調整webconfig取參數方式
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
using Framework.Data.OM;
using Framework.Data.OM.Collections;
using CSIPPayCertify.BusinessRules;
using CSIPPayCertify.EntityLayer;
using CSIPCommonModel.EntityLayer;
using CSIPCommonModel.BaseItem;
using Framework.Common.Utility;
using Framework.Common.Message;
using Framework.Common.JavaScript;
using Framework.Common.Logging;
using Framework.WebControls;

public partial class Page_P040201050001 : PageBase
{
    # region event

    protected void Page_Load(object sender, EventArgs e)
    {
        Page.Title = BaseHelper.GetShowText("04_02010500_000");
        if (!Page.IsPostBack)
        {
            //*畫面文字顯示
            ShowControlsText();
            //*CustGridView初始
            this.gpList.Visible = false;
            this.gpList.RecordCount = 0;
            this.gvpbPay_SV_Tmp.DataSource = null;
            this.gvpbPay_SV_Tmp.DataBind();
            //*數據綁定GridView
            BindGridView();
        }
    }

    /// <summary>
    /// 分頁設計
    /// </summary>
    /// <param name="src"></param>
    /// <param name="e"></param>
    protected void gpList_PageChanged(object src, Framework.WebControls.PageChangedEventArgs e)
    {
        this.gpList.CurrentPageIndex = e.NewPageIndex;
        this.BindGridView();
    }
    /// <summary>
    /// LinkButton點擊事件處理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void lkReaback_Click(object sender, EventArgs e)
    {
        LinkButton lbtnReaback = (LinkButton)sender;
        string strCaseNO = lbtnReaback.CommandName;
        if (string.IsNullOrEmpty(strCaseNO.Trim()))
        {
            MessageHelper.ShowMessage(this.Page, "04_02010500_004");
            return;
        }
        string strURL = "P040201050002.aspx?CaseNO=" + RedirectHelper.GetEncryptParam(strCaseNO).ToString() + "";
        //*打開退件說明頁面
        jsBuilder.RegScript(this.Page, BaseHelper.GetScriptForWindowOpenBackURL(this.Page, strURL));
    }
    # endregion event

    # region method
    /// <summary>
    /// 顯示文字
    /// </summary>
    private void ShowControlsText()
    {
        //*CustGridView gvpbPay_SV_Tmp列的Title設置
        this.gvpbPay_SV_Tmp.Columns[0].HeaderText = BaseHelper.GetShowText("04_02010500_002");
        this.gvpbPay_SV_Tmp.Columns[1].HeaderText = BaseHelper.GetShowText("04_02010500_003");
        this.gvpbPay_SV_Tmp.Columns[2].HeaderText = BaseHelper.GetShowText("04_02010500_004");
        this.gvpbPay_SV_Tmp.Columns[3].HeaderText = BaseHelper.GetShowText("04_02010500_005");
        this.gvpbPay_SV_Tmp.Columns[4].HeaderText = BaseHelper.GetShowText("04_02010500_006");
        this.gvpbPay_SV_Tmp.Columns[5].HeaderText = BaseHelper.GetShowText("04_02010500_007");
        this.gvpbPay_SV_Tmp.Columns[6].HeaderText = BaseHelper.GetShowText("04_02010500_008");
        this.gvpbPay_SV_Tmp.Columns[7].HeaderText = BaseHelper.GetShowText("04_02010500_009");
        this.gvpbPay_SV_Tmp.Columns[8].HeaderText = BaseHelper.GetShowText("04_02010500_010");
        this.gvpbPay_SV_Tmp.Columns[9].HeaderText = BaseHelper.GetShowText("04_02010500_011");
        this.gvpbPay_SV_Tmp.Columns[10].HeaderText = BaseHelper.GetShowText("04_02010500_012");
        this.gvpbPay_SV_Tmp.Columns[11].HeaderText = BaseHelper.GetShowText("04_02010500_013");
        this.gvpbPay_SV_Tmp.Columns[12].HeaderText = BaseHelper.GetShowText("04_02010500_014");
        this.gvpbPay_SV_Tmp.Columns[13].HeaderText = BaseHelper.GetShowText("04_02010500_015");
        //* 設置一頁顯示最大筆數
        this.gpList.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize").ToString());
        this.gvpbPay_SV_Tmp.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize").ToString());
    }
    /// <summary>
    /// gridview數據綁定
    /// </summary>
    private void BindGridView()
    {
        SqlHelper sql = new SqlHelper();
        try
        {
            //*查出數據保存在EntitySet<>中
            EntitySet<EntityPay_SV_Tmp> EntitySetResult = null;
            sql.AddOrderCondition(EntityPay_SV_Tmp.M_case_no, ESortType.ASC);
            sql.AddOrderCondition(EntityPay_SV_Tmp.M_SeqNo, ESortType.ASC);
            EntitySetResult = (EntitySet<EntityPay_SV_Tmp>)BRPay_SV_Tmp.Search(sql.GetFilterCondition(), this.gpList.CurrentPageIndex, this.gpList.PageSize);
            //*將對應數據綁定到GridView
            this.gpList.Visible = true;
            this.gpList.RecordCount = EntitySetResult.TotalCount;
            this.gvpbPay_SV_Tmp.DataSource = EntitySetResult;
            this.gvpbPay_SV_Tmp.DataBind();
            
            //* 查詢成功
            jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow("04_02010500_001"));
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            //*查詢失敗顯示
            jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow("04_02010500_002"));
        }
            
    }
    # endregion method

   
}
