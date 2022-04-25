<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P040201020001.aspx.cs" Inherits="Page_P040201020001" %>

<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
        <script type="text/javascript" src="../Common/Script/JavaScript.js"></script>
    <script type="text/javascript" src="../Common/Script/JQuery/jquery-1.3.2.min.js"></script>
    <script type="text/javascript" src="../Common/Script/JQuery/jquery-ui-1.7.min.js"></script>
    <script type="text/javascript" src="../Common/Script/JQuery/WINF_JQuery.js"></script>
    <link href="../App_Themes/Default/global.css" type="text/css" rel="stylesheet" />
</head>
<body class="workingArea">
    <form id="form1" runat="server" defaultbutton="btnSearch">
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
                                <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="04_02010200_001" StickHeight="False"></cc1:CustLabel>
                            </li>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblUserId" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="04_02010200_002" StickHeight="False"></cc1:CustLabel></td>
                        <td align="left" style="width: 85%">
                            <cc1:CustTextBox ID="txtUserID" Style="ime-mode: disabled; text-align: left" runat="server"
                                MaxLength="11" InputType="LetterAndInt"></cc1:CustTextBox>
                        </td>
                    </tr>
                    <tr class="trEven">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblType" runat="server" FractionalDigit="2" IsColon="True" IsCurrency="False"
                                NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False"
                                StickHeight="False" ShowID="04_02010200_003" CurAlign="left" CurSymbol="£"></cc1:CustLabel>
                        </td>
                        <td style="width: 85%">
                            <asp:RadioButtonList ID="radlType" runat="server" RepeatColumns="4" RepeatDirection="Horizontal">
                            </asp:RadioButtonList>
                        </td>
                    </tr>
                    <tr>
                        <td align="center" class="itemTitle" colspan="2">
                            <cc1:CustButton ID="btnSearch" runat="server" class="smallButton" Style="width: 50px;"
                                ShowID="04_02010200_004" OnClick="btnSearch_Click" />
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100%" colspan="2">
                            <cc1:CustGridView ID="grvPaySV" runat="server" AllowSorting="True" PagerID="gpList"
                                Width="100%" BorderWidth="0px" CellPadding="0" CellSpacing="1" BorderStyle="Solid">
                                <RowStyle CssClass="Grid_Item" Wrap="False" />
                                <SelectedRowStyle CssClass="Grid_SelectedItem" />
                                <HeaderStyle CssClass="Grid_Header" Wrap="False" />
                                <AlternatingRowStyle CssClass="Grid_AlternatingItem" Wrap="False" />
                                <PagerSettings Visible="False" />
                                <EmptyDataRowStyle HorizontalAlign="Center" />
                                <Columns>
                                    <asp:BoundField DataField="SerialNo">
                                        <itemstyle width="10%" horizontalalign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="UserID">
                                        <itemstyle width="10%" horizontalalign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="UserName">
                                        <itemstyle width="8%" horizontalalign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="MakeUp">
                                        <itemstyle width="6%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="GetFee">
                                        <itemstyle width="6%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="IsMail">
                                        <itemstyle width="6%" horizontalalign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Type">
                                        <itemstyle width="14%" horizontalalign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="CARDTYPE">
                                        <itemstyle width="8%" horizontalalign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="VOID">
                                        <itemstyle width="8%" horizontalalign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="KeyinDay">
                                        <itemstyle width="8%" horizontalalign="Left" />
                                    </asp:BoundField>
                                    <asp:TemplateField>
                                        <itemstyle horizontalalign="Center" width="5%" />
                                        <itemtemplate>
                                        <cc1:CustLinkButton id="lkbDetail" runat="server" CommandName='<%# DataBinder.Eval(Container.DataItem,"SerialNo")%>' __designer:wfdid="w2" OnClick="lkbDetail_Click"><%# BaseHelper.GetShowText("04_02010200_015")%></cc1:CustLinkButton> 
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
