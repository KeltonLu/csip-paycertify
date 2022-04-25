<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P040202010001.aspx.cs" Inherits="Page_P040202010001" %>

<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%--新增查詢功能 by Ares Stanley 20220120--%>
<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1"  runat="server">
     <title></title>
    <script src="../Common/Script/JavaScript.js"></script>

    <script src="../Common/Script/JQuery/jquery-1.3.2.min.js"></script>

    <script src="../Common/Script/JQuery/jquery-ui-1.7.min.js"></script>

    <script src="../Common/Script/JQuery/WINF_JQuery.js"></script>
     <script type="text/javascript"> 

    </script>
<link href="../App_Themes/Default/global.css" type="text/css" rel="stylesheet" />
</head>
<body class="workingArea">
        <form id="form1" runat="server" >
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
    <table cellpadding="0" cellspacing="1" width="100%" >
    <tr class="itemTitle" >
					<td colspan="2">
					<li>
                        <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="&#163;" FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_02020100_001" StickHeight="False"></cc1:CustLabel></li></td>
				</tr>
         
   <tr class="trOdd">
        <td style="text-align: right;width:15%">
            <cc1:CustLabel ID="lblDate" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_02020100_002" StickHeight="False"></cc1:CustLabel> </td>
        <td style="text-align: left;width:85%">
         <cc1:DatePicker ID = "dpBeforeDate" runat="server" ></cc1:DatePicker>
         ~
         <cc1:DatePicker ID = "dpEndDate" runat="server" ></cc1:DatePicker>
         </td>
  </tr>

<tr class="itemTitle" align="center" >
    <td colspan="2">
        <cc1:CustButton ID="btnSearch" runat="server" CssClass="smallButton" onclick="btnSearch_Click" ShowID="04_02020100_005"/>
        <cc1:CustButton ID="btnOK" runat="server" CssClass="smallButton" onclick="btnOK_Click" ShowID="04_02020100_003"/></td></tr>
    </table>
            <table width="100%" border="0" cellpadding="0" cellspacing="0" id="Table1">
					<tr>
						<td colspan="20">
							<cc1:CustGridView ID="grvDataView" runat="server" AllowSorting="True" AllowPaging="False"
								PagerID="gpList" Width="100%" BorderWidth="0px" CellPadding="0" CellSpacing="1"
								BorderStyle="Solid">
								<RowStyle CssClass="Grid_Item" Wrap="True" />
								<SelectedRowStyle CssClass="Grid_SelectedItem" />
								<HeaderStyle CssClass="Grid_Header" Wrap="False" />
								<AlternatingRowStyle CssClass="Grid_AlternatingItem" Wrap="True" />
								<PagerSettings Visible="False" />
								<EmptyDataRowStyle HorizontalAlign="Center" />
								<Columns>
                                    <asp:TemplateField ItemStyle-Width="3%" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <%#  ((this.gpList.CurrentPageIndex-1)*this.gpList.PageSize)+grvDataView.Rows.Count + 1 %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="userID">
										<ItemStyle Width="7%" HorizontalAlign="Center" />
									</asp:BoundField>
                                    <asp:BoundField DataField="UserName">
										<ItemStyle Width="7%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="SerialNo">
										<ItemStyle Width="7%" HorizontalAlign="Center" />
									</asp:BoundField>
									<asp:BoundField DataField="Address">
										<ItemStyle Width="14%" HorizontalAlign="Center" />
									</asp:BoundField>
                                    <asp:BoundField DataField="Consignee">
										<ItemStyle Width="7%" HorizontalAlign="Center" />
									</asp:BoundField>
                                    <asp:BoundField DataField="ClearDate">
										<ItemStyle Width="7%" HorizontalAlign="Center" />
									</asp:BoundField>
                                    <asp:BoundField DataField="EndDate">
										<ItemStyle Width="7%" HorizontalAlign="Center" />
									</asp:BoundField>
                                    <asp:BoundField DataField="Type">
										<ItemStyle Width="7%" HorizontalAlign="Center" />
									</asp:BoundField>
                                    <asp:BoundField DataField="CardNo">
										<ItemStyle Width="14%" HorizontalAlign="Center" />
									</asp:BoundField>
                                    <asp:BoundField DataField="BLKCode">
										<ItemStyle Width="7%" HorizontalAlign="Center" />
									</asp:BoundField>
                                    <asp:BoundField DataField="TypeCard">
										<ItemStyle Width="7%" HorizontalAlign="Center" />
									</asp:BoundField>
                                    <asp:BoundField DataField="CUSID">
										<ItemStyle Width="7%" HorizontalAlign="Center" />
									</asp:BoundField>
                                    <asp:BoundField DataField="CUSNAME">
										<ItemStyle Width="7%" HorizontalAlign="Center" />
									</asp:BoundField>
                                    <asp:BoundField DataField="Status">
										<ItemStyle Width="7%" HorizontalAlign="Center" />
									</asp:BoundField>
                                    <asp:BoundField DataField="IsPaidOffAlone">
										<ItemStyle Width="7%" HorizontalAlign="Center" />
									</asp:BoundField>
                                    <asp:BoundField DataField="ApplyUser">
										<ItemStyle Width="7%" HorizontalAlign="Center" />
									</asp:BoundField>
                                    <asp:BoundField DataField="AgreeUser">
										<ItemStyle Width="7%" HorizontalAlign="Center" />
									</asp:BoundField>
								</Columns>
							</cc1:CustGridView>
						</td>
					</tr>
					<tr>
						<td>
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
