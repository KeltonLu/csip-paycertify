<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P040201040002.aspx.cs" Inherits="Page_P040201040002" %>

<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

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
					<td colspan="4">
					<li>
                        <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_01010400_025" StickHeight="False"></cc1:CustLabel></li></td>
				</tr>
    <tr class="trOdd">
          <td style="text-align: right;width:25%">
            <cc1:CustLabel ID="lblSerialNo" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_01010400_013" StickHeight="False"></cc1:CustLabel> </td>
          <td style="text-align: left;width:75%">
          <cc1:CustTextBox ID="txtSerialNo" runat="server"  Width="150px" Enabled="false"></cc1:CustTextBox></td>
    </tr>
         
   <tr class="trEven">
         <td style="text-align: right;width:25%">
            <cc1:CustLabel ID="lblUserID" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_01010400_014" StickHeight="False"></cc1:CustLabel> </td>
          <td style="text-align: left;width:75%">
          <cc1:CustTextBox ID="txtUserID" runat="server"  Width="150px" Enabled="false"></cc1:CustTextBox></td>
   </tr>
         
   <tr class="trOdd">
        <td style="text-align: right;width:25%">
            <cc1:CustLabel ID="lblDate" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_01010400_026" StickHeight="False"></cc1:CustLabel>：</td>
        <td style="text-align: left;width:85%">
         <cc1:DatePicker ID = "dtpBackDate" runat="server" Width="150" ></cc1:DatePicker>
         </td>
  </tr>
  
 <tr class="trEven">
     <td style="text-align: right;width:25%">
        <cc1:CustLabel ID="lblCase" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_01010400_027" StickHeight="False"></cc1:CustLabel> </td>
       <td style="text-align: left;width:75%">
          <cc1:CustTextBox ID="txtCase" runat="server" MaxLength="50" Width="150px"  TextMode="MultiLine" Height="70px" InputType="Memo"></cc1:CustTextBox></td>
</tr>
   
<tr class="itemTitle" align="center" >
                        <td colspan="2">
                            <asp:Button ID="btnSearch" runat="server" class="smallButton" Style="width: 50px;"   OnClick="btnSearch_Click"  />&nbsp;&nbsp;
                            <asp:Button ID="btnClear" runat="server" class="smallButton" Style="width: 50px;"    OnClick="btnClear_Click"/>

                        </td>
                        </tr>
                        </table>
        </ContentTemplate>
        </asp:UpdatePanel>   
    </form>
   
</body>
</html>
