<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P040201050001.aspx.cs" Inherits="Page_P040201050001" %>
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
                <table cellpadding="0" cellspacing="1" width="100%">
                    <tr class="itemTitle">
                        <td colspan="2">
                            <li>
                                <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="04_02010500_001" StickHeight="False"></cc1:CustLabel></li>
                        </td>
                    </tr>
                 <tr>
                        <td style="width: 100%" colspan="2">
                            <cc1:CustGridView ID="gvpbPay_SV_Tmp" runat="server" AllowSorting="True" PagerID="gpList"
                                Width="100%" BorderWidth="0px" CellPadding="0" CellSpacing="1" BorderStyle="Solid">
                                <RowStyle CssClass="Grid_Item" Wrap="False" />
                                <SelectedRowStyle CssClass="Grid_SelectedItem" />
                                <HeaderStyle CssClass="Grid_Header" Wrap="False" />
                                <AlternatingRowStyle CssClass="Grid_AlternatingItem" Wrap="False" />
                                <PagerSettings Visible="False" />
                                <EmptyDataRowStyle HorizontalAlign="Center" />                    <Columns>                
                <asp:BoundField DataField="UserID"   >
                    <itemstyle  width="5%"  horizontalalign="Center" />                    
                </asp:BoundField>
                <asp:BoundField DataField="UserName"  >
                    <itemstyle   width="8%"  horizontalalign="Center" />                   
                </asp:BoundField>
                 <asp:BoundField DataField="Address"   >
                   <itemstyle   width="11%"  horizontalalign="Center" />
                </asp:BoundField> 
                <asp:BoundField DataField="Consignee"   >
                    <itemstyle   width="8%"  horizontalalign="Center" />
                </asp:BoundField> 
                <asp:BoundField DataField="ClearDate"   >
                   <itemstyle   width="8%"  horizontalalign="Center" />
                </asp:BoundField>
                <asp:BoundField DataField="CardNo"   >
                   <itemstyle   width="5%"  horizontalalign="Center" />
                </asp:BoundField>
                 <asp:BoundField DataField="BLKCode"   >
                    <itemstyle   width="5%"  horizontalalign="Center" />
                </asp:BoundField>
                 <asp:BoundField DataField="CardID"   >
                   <itemstyle   width="8%"  horizontalalign="Center" />
                </asp:BoundField>
                 <asp:BoundField DataField="CardName"   >
                    <itemstyle   width="8%"  horizontalalign="Center" />
                </asp:BoundField>
                 <asp:BoundField DataField="Status"   >
                   <itemstyle   width="5%"  horizontalalign="Center" />
                </asp:BoundField>
                 <asp:BoundField DataField="IsPaidOffAlone"   >
                   <itemstyle   width="5%"  horizontalalign="Center" />
                </asp:BoundField>
                 <asp:BoundField DataField="ApplyUser"   >
                    <itemstyle   width="8%"  horizontalalign="Center" />
                </asp:BoundField>
                 <asp:BoundField DataField="AgreeUser"   >
                    <itemstyle   width="8%"  horizontalalign="Center" />
                </asp:BoundField>
                 <asp:TemplateField>
                <itemstyle horizontalalign="Center" width="8%" />
                    <itemtemplate>
                        <cc1:CustLinkButton id="lbtnReaback" runat="server" CommandName='<%# DataBinder.Eval(Container.DataItem,"case_no")%>' OnClick="lkReaback_Click"><%# BaseHelper.GetShowText("04_02010500_015")%></cc1:CustLinkButton> 
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
