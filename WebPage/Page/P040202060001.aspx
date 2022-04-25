<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P040202060001.aspx.cs" Inherits="Page_P040202060001" %>
<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
    <script type="text/javascript" src="../Common/Script/JavaScript.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-1.3.2.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-ui-1.7.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/WINF_JQuery.js"></script>

    <link href="../App_Themes/Default/global.css" type="text/css" rel="stylesheet" />
</head>
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
                <table width="100%" border="0" cellpadding="0" cellspacing="1">
                    <tr class="itemTitle">
                        <td colspan="2">
                            <li>
                                <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="&#163;" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="04_02020600_001" StickHeight="False"></cc1:CustLabel></b></li>
                        </td>
                    </tr>
                 <tr class="trOdd">
                        <td align="right" style="width: 20%; height: 25px;">
                            <cc1:CustLabel ID="lblRebackDate" runat="server" CurAlign="left" CurSymbol="&#163;"
                                FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_02020600_002"
                                StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 80%; height: 25px;">
                            <cc1:DatePicker ID="dpRebackBeforeDate" runat="server" MaxLength="10" Width="150"></cc1:DatePicker>&nbsp;~&nbsp;
                            <cc1:DatePicker ID="dpRebackEndDate" runat="server" MaxLength="10" Width="150"></cc1:DatePicker>
                        </td>
                    </tr>
                    <tr class="trEven">
                        <td align="right" style="width: 20%">
                            <cc1:CustLabel ID="lblApplyDate" runat="server" CurAlign="left" CurSymbol="&#163;" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="04_02020600_003" StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 80%">
                        <cc1:DatePicker ID="dpApplyBeforeDate" runat="server" MaxLength="10" Width="150"></cc1:DatePicker>&nbsp;~&nbsp;
                            <cc1:DatePicker ID="dpApplyEndDate" runat="server" MaxLength="10" Width="150"></cc1:DatePicker>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 20%">
                            <cc1:CustLabel ID="lblUserID" runat="server" CurAlign="left" CurSymbol="&#163;"
                                FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_02020600_004"
                                StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 80%">
                            <cc1:CustTextBox ID="txtUserID" runat="server" MaxLength="11" InputType="LetterAndInt"></cc1:CustTextBox>
                        </td>
                    </tr>
                    
                    <tr align="center" class="itemTitle">
                        <td colspan="2">
                            <cc1:CustButton ID="btnSearch" runat="server" class="smallButton" Style="width: 50px;"
                                OnClick="btnSearch_Click" ShowID="04_02020600_005" />&nbsp;&nbsp;
                           
                            <cc1:CustButton ID="btnPrint" runat="server" class="smallButton" Style="width: 50px;"
                                 ShowID="04_02020600_006" OnClick="btnPrint_Click" />
                        </td>
                    </tr>
                </table>
                <table width="100%" border="0" cellpadding="0" cellspacing="0" id="Table1" style="">
                    <tr>
                        <td colspan="2">
                            <cc1:CustGridView ID="gvpbPay_SV_FeedBack" runat="server" AllowSorting="True" PagerID="gpList"
                                Width="100%" BorderWidth="0px" CellPadding="0" CellSpacing="1" BorderStyle="Solid" OnRowDataBound="gvpbPay_SV_FeedBack_RowDataBound">
                                <RowStyle CssClass="Grid_Item" Wrap="False" />
                                <SelectedRowStyle CssClass="Grid_SelectedItem" />
                                <HeaderStyle CssClass="Grid_Header A" Wrap="False" />
                                <AlternatingRowStyle CssClass="Grid_AlternatingItem" Wrap="False" />
                                <PagerSettings Visible="False" />
                                <EmptyDataRowStyle HorizontalAlign="Center" />
                                <Columns>
                                     <asp:TemplateField>
                                        <itemstyle horizontalalign="center" width="5%" />
                                        <itemtemplate>
                                            <cc1:CustLabel id="lblSerialNo" runat="server" width="5%"> </cc1:CustLabel>
                                        </itemtemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="UserID">
                                        <itemstyle width="10%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="UserName">
                                        <itemstyle width="20%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="ApplyDate">
                                        <itemstyle width="20%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="CardCount">
                                        <itemstyle width="20%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="RebackReason">
                                        <itemstyle width="25%" horizontalalign="Center" />
                                    </asp:BoundField> 
                                </Columns>
                            </cc1:CustGridView>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <cc1:GridPager ID="gpList" runat="server" AlwaysShow="True" CustomInfoTextAlign="Right"
                                InputBoxStyle="height:15px" OnPageChanged="gpList_PageChanged" PrevPageText="<前一頁">
                            </cc1:GridPager>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>