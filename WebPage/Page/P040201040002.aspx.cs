//******************************************************************
//*  作    者：偉林
//*  功能說明：退件清償證明

//*  創建日期：2009/11/12
//*  修改記錄：

//*<author>            <time>            <TaskID>                <desc>
//* Ares Stanley    2021/12/24  20210058-CSIP作業服務平台現代化II    confirm改sweetAlert2, 調整js註冊位置
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
using Framework.Common.JavaScript;
using Framework.WebControls;
using System.Text.RegularExpressions;
using Framework.Common.Message;
using CSIPPayCertify.BusinessRules;
using CSIPPayCertify.EntityLayer;
using CSIPCommonModel.EntityLayer;
using CSIPCommonModel.BaseItem;
using Framework.Common.Logging;
using Framework.Common.Utility;
using System.Text;

public partial class Page_P040201040002 : PageBase
{
    #region Event

    protected void Page_Load(object sender, EventArgs e)
    {
        string title = string.Empty;
        title = BaseHelper.GetShowText("04_02010400_023") + RedirectHelper.GetDecryptString(this.Page, "SerialNo") + BaseHelper.GetShowText("04_02010400_024");
        this.btnSearch.OnClientClick = string.Format("return AlertYesNo({{title: '{0}', bn: this, link:'' }});", title);

        if (!Page.IsPostBack)
        {
            try
            {
                string strSerialNo = RedirectHelper.GetDecryptString(this.Page, "SerialNo");
                string strCustomerID = RedirectHelper.GetDecryptString(this.Page, "CustomerID");

                txtSerialNo.Text = strSerialNo;

                txtUserID.Text = strCustomerID;

                if (strSerialNo == "" || strCustomerID == "")
                {
                    jsBuilder.RegScript(this.Page,BaseHelper.GetScriptForWindowErrorClose());
                    return;
                }


                ShowControlsText();
            }
            catch (Exception exp)
            {
                Logging.Log(exp, LogLayer.UI);
                jsBuilder.RegScript(this.Page,BaseHelper.GetScriptForWindowErrorClose());
                return;
            }
        }
    }

    /// <summary>
    /// 清除
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnClear_Click(object sender, EventArgs e)
    {
        jsBuilder.RegScript(this.Page, BaseHelper.GetScriptForCloseWindow());
    }

    /// <summary>
    /// 確定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        try
        {
            string strMsgID = "";
            if (this.dtpBackDate.Text.Trim() == "")
            {
                MessageHelper.ShowMessage(this.Page, "00_00000000_027");
                return;
            }
            if (this.txtCase.Text.Trim() == "")
            {
                MessageHelper.ShowMessage(this.Page, "00_00000000_028");
                return;
            }
            if (!CheckCondition(ref strMsgID))
            {
                MessageHelper.ShowMessage(this.Page, strMsgID);
                return;
            }

            EntityPay_ReturnSV eyPayReturnSV = new EntityPay_ReturnSV();
            eyPayReturnSV.serialNo = txtSerialNo.Text.Trim();
            eyPayReturnSV.userID = txtUserID.Text;
            eyPayReturnSV.returnDay = dtpBackDate.Text.Trim().Replace("/", "");
            eyPayReturnSV.returnReason = txtCase.Text.Trim();

            CSIPCommonModel.EntityLayer.EntityAGENT_INFO eAgentinfo = (CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"];

            string agent_id = eAgentinfo.agent_id;
            string agent_name = eAgentinfo.agent_name;

            if (BRPay_SV.MarkReturn(eyPayReturnSV, agent_id, agent_name))
            {
                jsBuilder.RegScript(this.Page, "opener.window.GetRerush();" + BaseHelper.GetScriptForCloseWindow());
                return;
            }
            else
            {
                jsBuilder.RegScript(this.Page, BaseHelper.GetScriptForWindowErrorClose());
            }
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow("04_02010400_005"));
            return;
        }


    }
    #endregion


    #region Function

    /// <summary>
    /// 顯示窗體文字
    /// </summary>
    private void ShowControlsText()
    {
        //* 設置頁面控件顯示文字
        this.btnSearch.Text = BaseHelper.GetShowText("04_02010400_028");
        this.btnClear.Text = BaseHelper.GetShowText("04_02010400_029");

    }


    /// <summary>
    /// 查詢時，檢核欄位的合法性
    /// </summary>
    /// <param name="strMsg"></param>
    /// <returns></returns>
    protected bool CheckCondition(ref string strMsgID)
    {
        //* 退件日期欄位格式檢核

        if (this.dtpBackDate.Text.Trim() != "")
        {

            DateTime dtmOut = DateTime.Parse("1900-01-01");
            if (!Function.IsDateTime(this.dtpBackDate.Text.Trim(), out dtmOut))
            {
                strMsgID = "04_02010400_006";
                return false;
            }
        }



        return true;
    }
    #endregion

}
