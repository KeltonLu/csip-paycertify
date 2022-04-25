<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P040201050002.aspx.cs" Inherits="Page_P040201050002" %>
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
                            <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£"  IsColon="False"  ShowID="04_02010500_016" FractionalDigit="2" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" StickHeight="False" ></cc1:CustLabel></li>
		            </td>
	            </tr>
	             <tr class="trOdd">
                    <td style="text-align: right;width:15%; height: 25px;">
                        <cc1:CustLabel ID="lblUserID" runat="server" ShowID="04_02010500_002" CurAlign="left" CurSymbol="&#163;" FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>：                    
                    </td>
                    <td style="text-align: left;width:75%; height: 25px;" >
                        <cc1:CustTextBox ID="txtUserID" runat="server" MaxLength="11" InputType="String"  ></cc1:CustTextBox>
                    </td>    
                </tr>
                <tr class="trEven">
                    <td style="text-align: right;width:15%; height: 25px;">
                        <cc1:CustLabel ID="lblUserName" runat="server" ShowID="04_02010500_003" CurAlign="left" CurSymbol="&#163;" FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>：                    
                    </td>
                   <td style="text-align: left;width:75%; height: 25px;" >
                        <cc1:CustTextBox ID="txtUserName" runat="server" MaxLength="11" InputType="String"  ></cc1:CustTextBox>
                    </td>
                </tr>
                <tr class="trOdd">
                    <td style="text-align: right;width:15%; height: 50px;">
                        <cc1:CustLabel ID="lblRebackReason" runat="server" ShowID="04_02010500_017" CurAlign="left" CurSymbol="&#163;" FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" StickHeight="False"></cc1:CustLabel>：                    
                    </td>
                    <td style="text-align: left;width:75%; height: 50px;" >
                        <cc1:CustTextBox ID="txtRebackReason" runat="server" MaxLength="50" InputType="Memo" TextMode="MultiLine"   ></cc1:CustTextBox>
                    </td>    
                </tr>
                 <tr align="center" class="itemTitle">
                        <td colspan="2" style="height: 25px">
                        <asp:Button ID="btnSubmit" runat="server" OnClick="btnOK_Click" CssClass="smallButton" Style="width: 50px;" />&nbsp;&nbsp; 
                            <%--<cc1:CustButton ID="btnOK" runat="server" OnClientClick="javascript:return confirm('123');" class="smallButton" Style="width: 50px;"  ShowID="04_02010500_018" OnClick="btnOK_Click" />--%>                          
                            <cc1:CustButton ID="btnCancel" runat="server" class="smallButton" Style="width: 50px;"  ShowID="04_02010500_019" OnClick="btnCancel_Click"  />
                        </td>
                    </tr>
	        </table>
        </ContentTemplate>
   </asp:UpdatePanel>   
   </form>
</body>
</html>
