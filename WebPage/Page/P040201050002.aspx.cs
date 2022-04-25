//******************************************************************
//*  作    者：余洋
//*  功能說明：匯入清單明細
//*  創建日期：2009/12/02
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

using Framework.Data.OM;
using Framework.Data.OM.Collections;
using Framework.Common.Utility;
using Framework.Common.Message;
using Framework.Common.JavaScript;
using Framework.Common.Logging;
using Framework.WebControls;

using CSIPPayCertify.BusinessRules;
using CSIPPayCertify.EntityLayer;
using CSIPCommonModel.EntityLayer;
using CSIPCommonModel.BaseItem;

public partial class Page_P040201050002 : PageBase
{
    # region event

    protected void Page_Load(object sender, EventArgs e)
    {
       try
        {
            if (!Page.IsPostBack)
            {
                btnSubmit.Text = BaseHelper.GetShowText("04_02010500_018");
                btnSubmit.OnClientClick = "javascript:return confirm('" + BaseHelper.GetShowText("04_02010500_020") + "')";

                jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow(""));
               
                //*case_no欄位
                string strCaseNO = RedirectHelper.GetDecryptString(this.Page, "CaseNO");
                ViewState["CaseNO"] = strCaseNO;
                BindData();
            }
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            jsBuilder.RegScript(this.UpdatePanel1,BaseHelper.GetScriptForWindowErrorClose());
            return;
        }
    }
    protected void btnOK_Click(object sender, EventArgs e)
    {
        //*退件原因不得為空
        if(string.IsNullOrEmpty(txtRebackReason.Text.Trim()))
        {
            MessageHelper.ShowMessage(this.Page, "04_02010500_003");
            return;
        }
        string strMsgID = "";
        string strCaseNO = this.ViewState["CaseNO"].ToString();
        string strRebackReason = this.txtRebackReason.Text.Trim();
        //*把Pay_SV_Tmp的各欄位的值添加到Pay_SV_Feedback
        if (BRPay_SV_Tmp.RebackBycase_No(strCaseNO, strRebackReason, ref strMsgID))
        {
            jsBuilder.RegScript(this.UpdatePanel1, JSConstant.Basic_ParentReload + "window.close();");
        }
        else
        {
            MessageHelper.ShowMessage(this.Page, "04_02010500_002");
            jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.GetScriptForCloseWindow());
        }
        
    }
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        //*直接退出此頁面
        jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.GetScriptForCloseWindow());
    }

    #endregion event

    #region method

    /// <summary>
    /// 綁定由case_no傳入的第一條數據的值
    /// </summary>
    private void BindData()
    {
        try
        {
            SqlHelper sql = new SqlHelper();

            EntitySet<EntityPay_SV_Tmp> EntitySetResult = null;
            sql.AddCondition(EntityPay_SV_Tmp.M_case_no, Operator.Equal, DataTypeUtils.String, this.ViewState["CaseNO"].ToString());
            //*查出數據存入泛型中
            EntitySetResult = (EntitySet<EntityPay_SV_Tmp>)BRPay_SV_Tmp.Search(sql.GetFilterCondition());
            //*查找出第一條數據并綁定
            this.txtUserID.Text= EntitySetResult.GetEntity(0).UserID.ToString();
            this.txtUserName.Text = EntitySetResult.GetEntity(0).UserName.ToString();
            //*輸入欄位限制
            this.txtUserID.Enabled = false;
            this.txtUserName.Enabled = false;
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            jsBuilder.RegScript(this.UpdatePanel1, "04_02010500_002");
        }
    }

    #endregion method
}
