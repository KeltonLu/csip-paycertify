<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P040102030001.aspx.cs" Inherits="Page_P040102030001" %>

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
                                    SetBreak="False" SetOmit="False" ShowID="04_01020300_001" StickHeight="False"></cc1:CustLabel></li>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblUserID" runat="server" CurAlign="left" CurSymbol="&#163;" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="04_02020300_002" StickHeight="False"></cc1:CustLabel>：
                        </td>
                        <td align="left" style="width: 85%">
                            <cc1:CustTextBox ID="txtUserID" runat="server" MaxLength="11" InputType="LetterAndInt"></cc1:CustTextBox></td>
                    </tr>
                    <tr class="trEven">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblDate" runat="server" CurAlign="left" CurSymbol="&#163;" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="04_01020300_014" StickHeight="False"></cc1:CustLabel>：
                        </td>
                        <td align="left" style="width: 85%">
                            <cc1:DatePicker ID="dpBeforeDate" runat="server" Width="150"></cc1:DatePicker>
                            ~
                            <cc1:DatePicker ID="dpEndDate" runat="server" Width="150"></cc1:DatePicker>
                        </td>
                    </tr>
                     <tr class="trOdd">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblType" runat="server" CurAlign="left" CurSymbol="&#163;" FractionalDigit="2"
                                IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="04_01020300_003" StickHeight="False"></cc1:CustLabel>：
                        </td>
                        <td align="left" style="width: 85%">
                            <asp:RadioButtonList ID="radlType" runat="server" RepeatColumns="4" RepeatDirection="Horizontal">
                            </asp:RadioButtonList></td>
                    </tr>
                    <tr class="itemTitle">
                        <td colspan="2" align="center">
                            <cc1:CustButton ID="btnQuery" runat="server" CssClass="smallButton" OnClick="btnQuery_Click"
                                ShowID="04_02020300_016" />&nbsp;&nbsp;<cc1:CustButton ID="btnAllprint" runat="server"
                                    CssClass="smallButton" OnClick="btnAllprint_Click" ShowID="04_02020300_017" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <cc1:CustGridView ID="grvPayCertify" runat="server" AllowSorting="True" PagerID="gpList"
                                Width="100%" BorderWidth="0px" CellPadding="0" CellSpacing="1" BorderStyle="Solid">
                                <Columns>
                                    <asp:BoundField DataField="SerialNo">
                                        <itemstyle width="8%"  />
                                        <headerstyle width="8%" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="USERID">
                                        <itemstyle width="8%" />
                                        <headerstyle width="8%" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="USERNAME">
                                        <itemstyle width="7%" />
                                        <headerstyle width="7%" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="MakeUp">
                                        <itemstyle width="5%" horizontalalign="Center" />
                                        <headerstyle width="5%" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="GetFee">
                                        <itemstyle width="5%" horizontalalign="Center" />
                                        <headerstyle width="5%" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="IsMail">
                                        <itemstyle width="5%" horizontalalign="Center"/>
                                        <headerstyle width="5%" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Type">
                                        <itemstyle width="5%" />
                                        <headerstyle width="5%" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="CARDTYPE" >
                                    <itemstyle width="3%" />
                                        <headerstyle width="3%" />   
                                        </asp:BoundField>
                                    <asp:BoundField DataField="VOID" >
                                          <itemstyle width="7%" />
                                        <headerstyle width="7%" />  
                                       </asp:BoundField>
                                    <asp:BoundField DataField="KeyinDay" >
                                          <itemstyle width="7%"  />
                                        <headerstyle width="7%" />  
                                       </asp:BoundField>
                                    <asp:TemplateField>
                                        <itemstyle width="5%" horizontalalign="Center" />
                                        <headerstyle width="5%" />
                                        <itemtemplate>
<cc1:CustLinkButton id="lbtnValidate" runat="server"  OnClick="lbtnValidate_Click"    CommandName='<%# DataBinder.Eval(Container.DataItem,"SerialNo")%>'  ><%# BaseHelper.GetShowText("04_01020300_018")%></cc1:CustLinkButton> 
</itemtemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <RowStyle CssClass="Grid_Item" />
                                <SelectedRowStyle CssClass="Grid_SelectedItem" />
                                <HeaderStyle CssClass="Grid_Header" Wrap="False" />
                                <AlternatingRowStyle CssClass="Grid_AlternatingItem" Wrap="False" />
                                <PagerSettings Visible="False" />
                            </cc1:CustGridView>
                            <cc1:GridPager ID="gpList" runat="server" AlwaysShow="True" CustomInfoTextAlign="Right" OnPageChanged="gpList_PageChanged" Visible="False">
                            </cc1:GridPager>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
