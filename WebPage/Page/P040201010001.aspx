<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P040201010001.aspx.cs" Inherits="Page_P040201010001" %>

<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>

    <script src="../Common/Script/JavaScript.js"></script>

    <script src="../Common/Script/JQuery/jquery-1.3.2.min.js"></script>

    <script src="../Common/Script/JQuery/jquery-ui-1.7.min.js"></script>

    <script src="../Common/Script/JQuery/WINF_JQuery.js"></script>

    <link href="../App_Themes/Default/global.css" type="text/css" rel="stylesheet" />
    <style type="text/css">

   .btnHiden

    {display:none; }

    </style>
</head>
<script language="javascript" type="text/javascript">
    function ResetHiddenTextbox()
    {
        $("#txtHiden1").attr("value","");   //* 原萬通客戶

        $("#txtHiden2").attr("value","");   //* 狀況碼不為SBAMZ
        $("#txtHiden3").attr("value","");   //* 歸戶下含有流通或R管制卡片
        $("#txtHiden4").attr("value","");   //* 歸戶目前餘額不為0   
        $("#txtHiden5").attr("value","");       
        $("#txtHiden6").attr("value","");       
        $("#txtHiden7").attr("value","");       
        $("#txtHiden8").attr("value","");    
        $("#txtHiden9").attr("value","");
        
    }
    function ConfrimText(strConfirmText,strHiddenControlID){
        if(confirm(strConfirmText) == false)
        {
            ResetHiddenTextbox();
            $("#btnCancelOne").click();
            
        }
        else
        {
            $("#"+strHiddenControlID).attr("value","Y");
            $("#btnCheckOne").click();
        }
    }
</script>
<body class="workingArea">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" EnablePageMethods="True" runat="server">
        </asp:ScriptManager>

        <script language="javascript" type="text/javascript">
			window.addEventListener("scroll", scroll, false);
              function   scroll()   
              {   
                $("#divProgress").css("top",290+document.documentElement.scrollTop);
              }    
		</script>

        <asp:UpdateProgress ID="updateProgress1" runat="server">
            <ProgressTemplate>
                <div id="divProgress" align="center" class="progress" style="position: absolute;
                    top: 290px; width: 100%; filter: Alpha(opacity=80); text-align: center;">
                    <div id="divProgress2" align="center" class="progress" style="background-color: White;
                        width: 50%; margin: 0px auto;">
                        <br />
                        <img alt="Please Wait..." src="../Common/images/Waiting.gif" />
                        <br />
                        <cc1:CustLabel ID="lblWaiting" runat="server" CurAlign="center" CurSymbol="£" FractionalDigit="2"
                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                            SetBreak="False" SetOmit="False" ShowID="00_00000000_000" StickHeight="False"></cc1:CustLabel>
                    </div>
                </div>
            </ProgressTemplate>
        </asp:UpdateProgress>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
            <ContentTemplate>
                <table cellpadding="0" cellspacing="1" width="100%">
                    <tr class="itemTitle">
                        <td>
                            <li>
                                <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="04_02010100_001" StickHeight="False"></cc1:CustLabel></li>
                        </td>
                    </tr>
                    <tr class="itemTitle">
                        <td>
                            <li>
                                <cc1:CustLabel ID="lblAddDetail" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="04_02010100_002" StickHeight="False"></cc1:CustLabel></li>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <cc1:CustGridView ID="grvAddDetail" runat="server" AllowSorting="True" Width="100%"
                                BorderWidth="0px" CellPadding="0" CellSpacing="1" BorderStyle="Solid" PagerID="GridPager1"
                                OnRowDataBound="grvAddDetail_DataBound" AllowPaging="False">
                                <Columns>
                                    <asp:TemplateField>
                                        <itemstyle width="5%" />
                                        <headerstyle width="5%" />
                                        <itemtemplate>
                                        <asp:Label id="lblAddNo" runat="server" ></asp:Label> 
                                        </itemtemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="USERID">
                                        <itemstyle width="15%" />
                                        <headerstyle width="15%" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="USERNAME">
                                        <itemstyle width="15%" />
                                        <headerstyle width="15%" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="APPLYDATE">
                                        <itemstyle width="20%" horizontalalign="Center" />
                                        <headerstyle width="20%" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="NUM">
                                        <itemstyle width="15%" horizontalalign="Right" />
                                        <headerstyle width="15%" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="ISCHECK">
                                        <itemstyle width="15%" horizontalalign="Center" />
                                        <headerstyle width="15%" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="ISREBACK">
                                        <itemstyle width="15%" horizontalalign="Center" />
                                        <headerstyle width="15%" />
                                    </asp:BoundField>
                                </Columns>
                                <RowStyle CssClass="Grid_Item" />
                                <SelectedRowStyle CssClass="Grid_SelectedItem" />
                                <HeaderStyle CssClass="Grid_Header" Wrap="False" />
                                <AlternatingRowStyle CssClass="Grid_AlternatingItem" Wrap="False" />
                                <PagerSettings Visible="False" />
                            </cc1:CustGridView>
                        </td>
                    </tr>
                    <tr class="itemTitle" align="center">
                        <td>
                            <cc1:CustButton ID="btnSave" runat="server" CssClass="smallButton" OnClick="btnSave_Click"
                                ShowID="04_02010100_004" />&nbsp;&nbsp;
                            <cc1:CustButton ID="btnAllValidate" runat="server" CssClass="smallButton" OnClick="btnAllValidate_Click"
                                ShowID="04_02010100_005" /></td>
                    </tr>
                    <tr class="itemTitle">
                        <td>
                            <li>
                                <cc1:CustLabel ID="lblRejectDetail" runat="server" CurAlign="left" CurSymbol="£"
                                    FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False"
                                    NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_02010100_003"
                                    StickHeight="False"></cc1:CustLabel></li>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <cc1:CustGridView ID="grvRejectDetail" runat="server" AllowSorting="True" Width="100%"
                                BorderWidth="0px" CellPadding="0" CellSpacing="1" BorderStyle="Solid" OnRowDataBound="grvRejectDetail_RowDataBound"
                                PagerID="GridPager1" OnRowUpdating="grvRejectDetail_RowUpdating" DataKeyNames="case_no"
                                AllowPaging="False">
                                <Columns>
                                <asp:TemplateField>
                                        <itemtemplate>
                                        <asp:Label id="lblRejectNo" runat="server" ></asp:Label> 
                                        
</itemtemplate>
                                        <headerstyle width="5%" />
                                        <itemstyle width="5%" />
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="USERID">
                                        <headerstyle width="15%" />
                                        <itemstyle width="15%" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="USERNAME">
                                        <headerstyle width="15%" />
                                        <itemstyle width="15%" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="APPLYDATE">
                                        <headerstyle width="15%" />
                                        <itemstyle width="15%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="NUM">
                                        <headerstyle width="10%" />
                                        <itemstyle width="10%" horizontalalign="Right" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="REBACKREASON">
                                        <headerstyle width="20%" />
                                        <itemstyle width="20%" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="CHECKTIME">
                                        <headerstyle width="10%" />
                                        <itemstyle width="10%" horizontalalign="Right" />
                                    </asp:BoundField>
                                    
                                    <asp:TemplateField>
                                        <edititemtemplate>
<asp:TextBox runat="server" id="TextBox1"></asp:TextBox>
</edititemtemplate>
                                        <itemtemplate>
<cc1:CustLinkButton id="lbtnValidate" runat="server"  CausesValidation="False" CommandName="update"></cc1:CustLinkButton> 
</itemtemplate>
                                        <headerstyle width="10%" />
                                        <itemstyle width="10%" horizontalalign="Center" />
                                    </asp:TemplateField>
                                </Columns>
                                <RowStyle CssClass="Grid_Item" />
                                <SelectedRowStyle CssClass="Grid_SelectedItem" />
                                <HeaderStyle CssClass="Grid_Header" Wrap="False" />
                                <AlternatingRowStyle CssClass="Grid_AlternatingItem" Wrap="False" />
                                <PagerSettings Visible="False" />
                            </cc1:CustGridView>
                        </td>
                    </tr>
                    <tr class="itemTitle" align="center">
                        <td>
                            <cc1:CustButton ID="btnReject" runat="server" CssClass="smallButton" OnClick="btnReject_Click"
                                ShowID="04_02010100_017" /></td>
                    </tr>
                </table>
            <cc1:CustTextBox ID="txthidCaseNo" runat="server" CssClass="btnHiden"></cc1:CustTextBox>      
            <cc1:CustTextBox ID="txtHiden1" runat="server" CssClass="btnHiden"></cc1:CustTextBox>        
            <cc1:CustTextBox ID="txtHiden2" runat="server" CssClass="btnHiden"></cc1:CustTextBox>        
            <cc1:CustTextBox ID="txtHiden3" runat="server" CssClass="btnHiden"></cc1:CustTextBox>        
            <cc1:CustTextBox ID="txtHiden4" runat="server" CssClass="btnHiden"></cc1:CustTextBox>        
            <cc1:CustTextBox ID="txtHiden5" runat="server" CssClass="btnHiden"></cc1:CustTextBox>        
            <cc1:CustTextBox ID="txtHiden6" runat="server" CssClass="btnHiden"></cc1:CustTextBox>        
            <cc1:CustTextBox ID="txtHiden7" runat="server" CssClass="btnHiden"></cc1:CustTextBox>        
            <cc1:CustTextBox ID="txtHiden8" runat="server" CssClass="btnHiden"></cc1:CustTextBox>        
            <cc1:CustTextBox ID="txtHiden9" runat="server" CssClass="btnHiden"></cc1:CustTextBox>   
            </ContentTemplate>
        </asp:UpdatePanel>        
        <asp:Button ID="btnCheckOne" runat="server" Text="btnCheckOne" OnClick="btnCheckOne_Click" CssClass="btnHiden"/>        
        <asp:Button ID="btnCancelOne" runat="server" Text="btnCancelOne" OnClick="btnCancelOne_Click" CssClass="btnHiden"/>
    </form>
</body>
</html>
