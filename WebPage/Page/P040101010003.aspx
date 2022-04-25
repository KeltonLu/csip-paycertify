<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P040101010003.aspx.cs" Inherits="Page_P040101010003" %>

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
                        <td colspan="2">
                            <li>
                                <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="04_01010100_001" StickHeight="False"></cc1:CustLabel>
                            </li>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td style="text-align: right; width: 15%; height: 25px;">
                            <cc1:CustLabel ID="lblPrintDate" runat="server" FractionalDigit="2" IsColon="True"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" StickHeight="False" ShowID="04_01010100_042" CurAlign="left"
                                CurSymbol="£"></cc1:CustLabel>
                        </td>
                        <td style="text-align: left; width: 85%; height: 25px;">
                            <cc1:CustLabel ID="lblDate" runat="server" FractionalDigit="2" IsColon="False"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" StickHeight="False" CurAlign="left" ShowID="" CurSymbol="&#163;"></cc1:CustLabel>
                        </td>
                    </tr>
                    <tr class="itemTitle" align="center">
                        <td colspan="2" style="height: 25px">
                            &nbsp;<cc1:CustButton ID="btnPrint" runat="server" CssClass="smallButton" ShowID="04_01010100_043" OnClick="btnPrint_Click" />
                            <cc1:CustButton ID="btnCancel" runat="server" CssClass="smallButton" ShowID="04_01010100_044" OnClick="btnCancel_Click" />
                    </tr>
                    <tr>
                        <td colspan="2">
                            <cc1:CustGridView ID="grvSetPayCertify" runat="server" AllowSorting="True"
                                Width="100%" BorderWidth="0px" CellPadding="0" CellSpacing="1" BorderStyle="Solid" AllowPaging="False" PagerID=""  OnRowDataBound="grvSetPayCertify_RowDataBound" ShowFooter="True"
                                >
                                <RowStyle CssClass="Grid_Item" Wrap="False" />
                                <SelectedRowStyle CssClass="Grid_SelectedItem" />
                                <HeaderStyle CssClass="Grid_Header" Wrap="False" />
                                <AlternatingRowStyle CssClass="Grid_AlternatingItem" Wrap="False" />
                                <PagerSettings Visible="False" />
                                <EmptyDataRowStyle HorizontalAlign="Center" />
                                <FooterStyle Wrap="True" />
                                <Columns>
                                    <asp:BoundField DataField="M_CUST_ID">
                                        <footerstyle width="5%" wrap="False" />
                                        <itemstyle width="5%" horizontalalign="Left"/>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="M_CUST_NAME">
                                        <footerstyle width="5%" />
                                        <itemstyle width="5%" horizontalalign="Left"/>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="M_CARD_NO">
                                        <footerstyle width="5%" />
                                        <itemstyle width="5%" horizontalalign="Left"/>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="M_CLOSE_REP">
                                        <footerstyle width="5%" />
                                        <itemstyle width="5%" horizontalalign="Left"/>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="M_BALANCE">
                                        <footerstyle width="5%" wrap="False" horizontalalign="Right" />
                                        <itemstyle width="5%" horizontalalign="Right"/>
                                    </asp:BoundField>                                   
                                    <asp:BoundField DataField="M_DEBT_DATE">
                                        <footerstyle width="5%" />
                                        <itemstyle width="5%" horizontalalign="Left"/>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="M_DEBT_AMT">
                                        <footerstyle width="5%" />
                                        <itemstyle width="5%" horizontalalign="Right" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="M_DEBT_BALANCE">
                                        <footerstyle width="5%" wrap="False" horizontalalign="Right" />
                                        <itemstyle width="5%" horizontalalign="Right" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="M_LAST_PAY_DATE">
                                        <footerstyle width="5%" />
                                        <itemstyle width="5%" horizontalalign="Left"/>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="M_TOTAL_PAYMENT">
                                        <footerstyle width="5%" wrap="False" horizontalalign="Right" />
                                        <itemstyle width="5%" horizontalalign="Right"/>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="M_CLOSE_DATE">
                                        <footerstyle width="5%" />
                                        <itemstyle width="5%" horizontalalign="Left"/>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="M_CLOSE_REASON">
                                        <footerstyle width="5%" />
                                        <itemstyle width="5%" horizontalalign="Left"/>
                                    </asp:BoundField>  
                                </Columns>                               
                            </cc1:CustGridView>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
        &nbsp;
    </form>
</body>
</html>
