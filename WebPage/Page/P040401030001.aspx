<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P040401030001.aspx.cs" Inherits="Page_P040401030001" %>

<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
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
                                <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_04010300_001" StickHeight="False"></cc1:CustLabel></li>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 20%">
                            <cc1:CustLabel ID="lblID" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_04010300_003" StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 80%">
                            <cc1:CustTextBox ID="txtID" Style="ime-mode: disabled; text-align: left" runat="server"
                                MaxLength="11" InputType="LetterAndInt"></cc1:CustTextBox>
                                
                    </tr>
                    <tr class="trEven">
                        <td align="right" style="width: 20%">
                            <cc1:CustLabel ID="lblID_Name" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_04010300_004" StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 80%">
                            <cc1:CustTextBox ID="txtID_Name" runat="server" Enabled="False" MaxLength="20"></cc1:CustTextBox></td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 20%">
                            <cc1:CustLabel ID="lblcardNO" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_04010300_005" StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 80%">
                            <cc1:CustTextBox ID="txtcardNO" runat="server" ReadOnly="false" MaxLength="20" InputType="Int"></cc1:CustTextBox> 
                        </td>
                    </tr>
                    <tr align="center" class="itemTitle">
                        <td colspan="2">
                            <cc1:CustButton ID="btnSearch" runat="server" class="smallButton" Style="width: 50px;" OnClick="btnSearch_Click" ShowID="04_04010300_006"/>&nbsp;&nbsp;
                            <cc1:CustButton ID="btnClear" runat="server" class="smallButton" Style="width: 50px;" OnClick="btnClear_Click"  ShowID="04_04010300_007"/>
                        </td>
                    </tr>
                </table>
                <table width="100%" border="0" cellpadding="0" cellspacing="0" id="Table1" style="">
                    <tr>
                        <td colspan="2">
                            <cc1:CustGridView ID="grvSetPayCertify" runat="server" AllowSorting="True" PagerID="gpList"
                                Width="100%" BorderWidth="0px" CellPadding="0" CellSpacing="1" BorderStyle="Solid" OnRowEditing="grvSetPayCertify_RowEditing" OnRowDataBound="grvSetPayCertify_RowDataBound"
                                >
                                <RowStyle CssClass="Grid_Item" Wrap="False" />
                                <SelectedRowStyle CssClass="Grid_SelectedItem" />
                                <HeaderStyle CssClass="Grid_Header A" Wrap="False" />
                                <AlternatingRowStyle CssClass="Grid_AlternatingItem" Wrap="False" />
                                <PagerSettings Visible="False" />
                                <EmptyDataRowStyle HorizontalAlign="Center" />
                                <Columns>
                                    <asp:BoundField DataField="SerialNo">
                                        <itemstyle  width="45%" horizontalalign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="CardNo">
                                        <itemstyle width="45%" horizontalalign="Left"/>
                                    </asp:BoundField>
                                    <asp:TemplateField>
                                        <itemstyle horizontalalign="center" width="10%" />
                                        <itemtemplate>
                                        <cc1:CustLinkButton id="lkbDetail" runat="server"  CommandName="Edit" ></cc1:CustLinkButton>
                                        </itemtemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </cc1:CustGridView>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <cc1:GridPager ID="gpList" runat="server" AlwaysShow="True" CustomInfoTextAlign="Right"
                                InputBoxStyle="height:15px" OnPageChanged="gpList_PageChanged">
                            </cc1:GridPager>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
