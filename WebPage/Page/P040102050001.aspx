<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P040102050001.aspx.cs" Inherits="Page_P040102050001" %>
<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
    <script src="../Common/Script/JavaScript.js"></script>
    
    <script src="../Common/Script/JQuery/jquery-1.3.2.min.js"></script>
    
    <script src="../Common/Script/JQuery/jquery-ui-1.7.min.js"></script>
    
    <script src="../Common/Script/JQuery/WINF_JQuery.js"></script>
    
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
                <div id="divProgress" class="progress" align="center" style="position: absolute;top: 290px; width: 100%; filter: Alpha(opacity=80);text-align: center;">
                    <div id="divProgress2" class="progress" align="center" style="background-color: White;width: 50%;margin:0px auto;">
                    <br />
                    <img src="../Common/images/Waiting.gif" alt="Please Wait..." />
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
                            <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="&#163;" FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_01020500_010" StickHeight="False"></cc1:CustLabel></li>
		            </td>
	            </tr>
                <tr class="trOdd">
                    <td style="text-align: right;width:15%; height: 25px;">
                        <cc1:CustLabel ID="lblCust_ID" runat="server" CurAlign="left" CurSymbol="&#163;" FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_01020500_001" StickHeight="False"></cc1:CustLabel>：
                    </td>
                    <td style="text-align: left;width:85%; height: 25px;" >
                        <cc1:CustTextBox ID="txtCust_ID" runat="server" MaxLength="11" InputType="LetterAndInt"  ></cc1:CustTextBox>
                    </td>    
                </tr>
                <tr class="trEven">
                    <td style="text-align: right;width:15%; height: 25px;">
                        <cc1:CustLabel ID="lblUser_ID" runat="server" CurAlign="left" CurSymbol="&#163;" FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_01020500_002" StickHeight="False"></cc1:CustLabel>：
                    </td>
                    <td style="text-align: left;width:85%; height: 25px;" >
                        <cc1:CustTextBox ID="txtUser_ID" runat="server" MaxLength="12" InputType="LetterAndInt" ></cc1:CustTextBox>
                    </td>    
                </tr>
                <tr class="trOdd">
                    <td style="text-align: right;width:15%; height: 25px;">
                        <cc1:CustLabel ID="lblLog_Date" runat="server" CurAlign="left" CurSymbol="&#163;" FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_01020500_003" StickHeight="False"></cc1:CustLabel>：
                    </td>
                    <td align="left" style="width: 85%; height: 25px;">
                            <cc1:DatePicker ID="dpBeforeDate" runat="server" Width="150"></cc1:DatePicker>
                            ~<cc1:DatePicker ID="dpEndDate" runat="server" Width="150"></cc1:DatePicker>
                    </td>
                </tr>
                <tr class="itemTitle" align="center" >
                    <td colspan="2">
                        <cc1:CustButton  ID="btnQuery" runat="server"  CssClass="smallButton" OnClick="btnQuery_Click" ShowID="04_01020500_004" />&nbsp;&nbsp;
                    </td>
                </tr>
                <tr>
                    <td style="width: 100%" colspan="2">
                        <cc1:CustGridView ID="gvpbCust" runat="server" AllowSorting="True" PagerID="gpList"
                             Width="100%" BorderWidth="0px" CellPadding="0" CellSpacing="1" BorderStyle="Solid">
                            <RowStyle CssClass="Grid_Item" Wrap="False" />
                            <SelectedRowStyle CssClass="Grid_SelectedItem" />
                            <HeaderStyle CssClass="Grid_Header" Wrap="False" />
                            <AlternatingRowStyle CssClass="Grid_AlternatingItem" Wrap="False" />
                            <PagerSettings Visible="False" />
                            <EmptyDataRowStyle HorizontalAlign="Center" />                                <Columns>                
                                    <asp:BoundField DataField="CustomerID"   >
                                        <itemstyle  width="15%"   />
                                        <headerstyle width="15%"  />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Serial_No"  >
                                        <itemstyle   width="15%" />
                                        <headerstyle width="15%"  />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Log_Action"   >
                                        <itemstyle  width="15%"   />
                                        <headerstyle width="15%"  />
                                    </asp:BoundField> 
                                    <asp:BoundField DataField="Log_date"   >
                                        <itemstyle  width="15%" horizontalalign="Center"   />
                                        <headerstyle width="15%"  />
                                    </asp:BoundField> 
                                    <asp:BoundField DataField="Log_Time"   >
                                        <itemstyle  width="10%" horizontalalign="Center"   />
                                        <headerstyle width="10%"  />
                                    </asp:BoundField> 
                                    <asp:BoundField DataField="User_ID"   >
                                        <itemstyle  width="15%"   />
                                        <headerstyle width="15%"  />
                                    </asp:BoundField> 
                                    <asp:BoundField DataField="User_Name"   >
                                        <itemstyle  width="15%"   />
                                        <headerstyle width="15%"  />
                                    </asp:BoundField>
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
