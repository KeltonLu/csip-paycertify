<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P040102040001.aspx.cs" Inherits="Page_P040102040001" %>
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
            <table cellpadding="0" cellspacing="1" width="100%" >
                <tr class="itemTitle" >
		            <td colspan="2" style="height: 25px">
		                <li>
                            <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="&#163;"  IsColon="False"  ShowID="04_01020400_001" ></cc1:CustLabel></li>
		            </td>
	            </tr>
                <tr class="trOdd">
                    <td style="text-align: right;width:15%; height: 25px;">
                        <cc1:CustLabel ID="lblUserID" runat="server" ShowID="04_01020400_002"></cc1:CustLabel>：                    
                    </td>
                    <td style="text-align: left;width:75%; height: 25px;" >
                        <cc1:CustTextBox ID="txtUserID" runat="server" MaxLength="11" InputType="LetterAndInt"  ></cc1:CustTextBox>
                    </td>    
                </tr>
                <tr class="trEven">
                    <td align="right" style="width: 15%;height:50px;">
                        <cc1:CustLabel ID="lblCertifyType" runat="server" CurAlign="left"  ShowID="04_01020400_003"></cc1:CustLabel>：</td>
                    <td align="left" style="width: 85%">
                        <asp:RadioButtonList ID="radlType" runat="server" RepeatColumns="4" RepeatDirection="Horizontal">
                        </asp:RadioButtonList></td>             
                </tr>
                <tr class="itemTitle" align="center" >
                    <td colspan="2">
                        <cc1:CustButton  ID="btnQuery" runat="server"  CssClass="smallButton" OnClick="btnQuery_Click" ShowID="04_01020400_021" />&nbsp;&nbsp;
                    </td>
               </tr>
              <tr>
                <td colspan="2">
                    <cc1:CustGridView ID="gvpbSetCertify" runat="server" AllowSorting="True"  PagerID="gpList"
                     Width="100%"  BorderWidth="0px" CellPadding="0" CellSpacing="1" BorderStyle="Solid" PagerVisible="True" OnRowDataBound="grvSetCertify_RowDataBound" OnRowEditing="grvSetCertify_RowEditing" >
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
                            <itemstyle horizontalalign="center" width="5%" />
                                <itemtemplate>
                                    <cc1:CustLinkButton id="lbtnDetail" runat="server"  CommandName="Edit" ></cc1:CustLinkButton>
                            </itemtemplate>
                        </asp:TemplateField>
                    </Columns>
                    </cc1:CustGridView>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <cc1:GridPager ID="gpList" runat="server"  CustomInfoTextAlign="Right"  AlwaysShow="True" OnPageChanged="gpList_PageChanged">
                    </cc1:GridPager>
                </td>
            </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>   
    </form>
</body>
</html>
