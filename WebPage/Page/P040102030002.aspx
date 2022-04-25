<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P040102030002.aspx.cs" Inherits="Page_P040102030002" %>

<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<%@ Register Src="../Common/Controls/CustAddress.ascx" TagName="CustAddress" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
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
                <table cellpadding="0" cellspacing="1" width="100%" >
                    <tr class="itemTitle" >
		                <td colspan="4">
		                    <li>
                                <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="&#163;" FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_01020300_001" StickHeight="False"></cc1:CustLabel></li>
		                </td>
	                </tr>
	                <tr class="trOdd">
                        <td style="text-align: right;width:15%; height: 25px;">
                            <cc1:CustLabel ID="lblUserID" runat="server" CurAlign="left" FractionalDigit="2" IsColon="True" 
                             IsCurrency="False" NeedDateFormat="False"
                             NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_01020300_002"
                            StickHeight="False" CurSymbol="&#163;"></cc1:CustLabel>                    
                        </td>
                        <td style="text-align: left;width:35%; height: 25px;" >
                            <cc1:CustTextBox ID="txtUserID" runat="server" InputType="String" Enabled="False"   ></cc1:CustTextBox>
                        </td>
                        <td style="text-align: right;width:15%; height: 25px;">
                            <cc1:CustLabel ID="lblUserName" runat="server" CurAlign="left" FractionalDigit="2" IsColon="True" 
                             IsCurrency="False" NeedDateFormat="False"
                             NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_01020300_007"
                             StickHeight="False" CurSymbol="&#163;"></cc1:CustLabel>                    
                        </td>
                        <td style="text-align: left;width:35%; height: 25px;">
                            <cc1:CustTextBox ID="txtUserName" runat="server" InputType="String" Enabled="False"  ></cc1:CustTextBox>
                        </td>    
                    </tr>
                    <tr class="trEven">
                    <td style="text-align: right;width:15%; height: 25px;">
                            <cc1:CustLabel ID="lblPayName" runat="server" CurAlign="left" FractionalDigit="2" IsColon="True" 
                             IsCurrency="False" NeedDateFormat="False"
                             NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_01020300_038"
                            StickHeight="False" CurSymbol="&#163;"></cc1:CustLabel>                    
                        </td>
                        <td style="text-align: left;width:35%; height: 25px;" >
                            <cc1:CustTextBox ID="txtPayName" runat="server" Enabled="False" InputType="String"  ></cc1:CustTextBox>
                        </td>
                        <td style="text-align: right;width:15%; height: 25px;">
                            <cc1:CustLabel ID="lblZip" runat="server" CurAlign="left" FractionalDigit="2" IsColon="True" 
                             IsCurrency="False" NeedDateFormat="False"
                             NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_01020300_020"
                             StickHeight="False" CurSymbol="&#163;"></cc1:CustLabel>                    
                        </td>
                        <td colspan="3" style="text-align: left;width:35%; height: 25px;">
                            <cc1:CustTextBox ID="txtZip" runat="server" InputType="String" Enabled="False"  ></cc1:CustTextBox></td>    
                    </tr>
                    <tr class="trOdd">
                        <td style="text-align: right;width:15%; height: 25px;">
                            <cc1:CustLabel ID="lblAdd1" runat="server" CurAlign="left" FractionalDigit="2" IsColon="True" 
                             IsCurrency="False" NeedDateFormat="False"
                             NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_01020300_021"
                             StickHeight="False" CurSymbol="&#163;"></cc1:CustLabel>                    
                        </td>
                        <td style="text-align: left;width:35%; height: 25px;" colspan="3">
                           <uc1:CustAddress id="CustAdd1" runat="server" ></uc1:CustAddress>
                        </td>
                    </tr>
                    <tr class="trEven">
                    <td style="text-align: right;width:15%; height: 25px;">
                        <cc1:CustLabel ID="lblAdd2" runat="server" CurAlign="left" FractionalDigit="2" IsColon="True" 
                        IsCurrency="False" NeedDateFormat="False"
                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_01020300_022"
                        StickHeight="False" CurSymbol="&#163;"></cc1:CustLabel>                    
                    </td>
                    <td style="text-align: left;width:50%; height: 25px;" colspan="3">
                        <cc1:CustTextBox ID="txtAdd2" runat="server" Width="410px" Enabled="False" EnableTheming="True"></cc1:CustTextBox>
                    </td>
                    </tr>
                     <tr class="trOdd">
                    <td style="text-align: right;width:15%; height: 25px;">
                        <cc1:CustLabel ID="lblAdd3" runat="server" CurAlign="left" FractionalDigit="2" IsColon="True" 
                        IsCurrency="False" NeedDateFormat="False"
                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_01020300_023"
                        StickHeight="False" CurSymbol="&#163;"></cc1:CustLabel>                    
                    </td>
                    <td style="text-align: left;width:50%; height: 25px;" colspan="3">
                        <cc1:CustTextBox ID="txtAdd3" runat="server" Width="410px" Enabled="False" EnableTheming="True" ></cc1:CustTextBox>
                    </td>
                    </tr>
                    <tr class="trEven">
                        <td style="text-align: right;width:15%; height: 25px;">
                            <cc1:CustLabel ID="lblConsignee" runat="server" CurAlign="left" FractionalDigit="2" IsColon="True" 
                             IsCurrency="False" NeedDateFormat="False"
                             NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_01020300_024"
                            StickHeight="False" CurSymbol="&#163;"></cc1:CustLabel>                    
                        </td>
                        <td style="text-align: left;width:35%; height: 25px;" >
                            <cc1:CustTextBox ID="txtConsignee" runat="server" InputType="String" Enabled="False"  ></cc1:CustTextBox>
                        </td>
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblIsFree" runat="server" IsColon="True" ShowID="04_01010100_065"></cc1:CustLabel></td>
                        <td style="width: 35%">
                            <cc1:CustCheckBox ID="chkIsFree" runat="server" Enabled="False" /></td>        
                    </tr>
                     <tr class="trOdd">
                     <td style="text-align: right;width:15%; height: 25px;">
                            <cc1:CustLabel ID="lblMailDay" runat="server" CurAlign="left" FractionalDigit="2" IsColon="True" 
                             IsCurrency="False" NeedDateFormat="False"
                             NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_01020300_025"
                             StickHeight="False" CurSymbol="&#163;"></cc1:CustLabel>                    
                        </td>
                        <td style="text-align: left;width:35%; height: 25px;">
                            <cc1:CustTextBox ID="txtMailDay" runat="server" InputType="String" Enabled="False"  ></cc1:CustTextBox>
                        </td>    
                        
                    <td style="text-align: right;width:15%; height: 25px;">
                        <cc1:CustLabel ID="lblMailNo" runat="server" CurAlign="left" FractionalDigit="2" IsColon="True" 
                        IsCurrency="False" NeedDateFormat="False"
                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_01020300_026"
                        StickHeight="False" CurSymbol="&#163;"></cc1:CustLabel>                    
                    </td>
                    <td style="text-align: left;width:50%; height: 25px;">
                        <cc1:CustTextBox ID="txtMailNo" runat="server" Enabled="False" ></cc1:CustTextBox>
                    </td>
                    </tr>
                    <tr class="trEven">
                    <td style="text-align: right;width:15%; height: 25px;">
                        <cc1:CustLabel ID="lblNote" runat="server" CurAlign="left" FractionalDigit="2" IsColon="True" 
                        IsCurrency="False" NeedDateFormat="False"
                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_01020300_027"
                        StickHeight="False" CurSymbol="&#163;"></cc1:CustLabel>                    
                    </td>
                    <td style="text-align: left;width:50%; height: 25px;" colspan="3">
                        <cc1:CustTextBox ID="txtNote" runat="server" Width="410px" Enabled="False"></cc1:CustTextBox>
                    </td>
                    </tr>
                     <tr class="trOdd">
                        <td style="text-align: right;width:15%; height: 25px;">
                            <cc1:CustLabel ID="lblMakeUpDate" runat="server" CurAlign="left" FractionalDigit="2" IsColon="True" 
                             IsCurrency="False" NeedDateFormat="False"
                             NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_01020300_028"
                            StickHeight="False" CurSymbol="&#163;"></cc1:CustLabel>                    
                        </td>
                        <td style="text-align: left;width:35%; height: 25px;" >
                            <cc1:CustTextBox ID="txtMakeUpDate" runat="server" InputType="String" Enabled="False" ></cc1:CustTextBox>
                        </td>
                        <td style="text-align: right;width:15%; height: 25px;">
                            <cc1:CustLabel ID="lblEndDate" runat="server" CurAlign="left" FractionalDigit="2" IsColon="True" 
                             IsCurrency="False" NeedDateFormat="False"
                             NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_01020300_029"
                             StickHeight="False" CurSymbol="&#163;"></cc1:CustLabel>                    
                        </td>
                        <td style="text-align: left;width:35%; height: 25px;">
                            <cc1:CustTextBox ID="txtEndDate" runat="server" InputType="String" Enabled="False"  ></cc1:CustTextBox>
                        </td>    
                    </tr>
                    <tr class="trEven">
                         <td style="text-align: right;width:15%; height: 25px;">
                            <cc1:CustLabel ID="lblMailType" runat="server" CurAlign="left" FractionalDigit="2" IsColon="True" 
                             IsCurrency="False" NeedDateFormat="False"
                             NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_01020300_030"
                            StickHeight="False" CurSymbol="&#163;"></cc1:CustLabel>                    
                        </td>
                        <td style="width: 35%">
                            <cc1:CustDropDownList ID="dropMailType" runat="server" IsValEmpty="False" Enabled="False">
                            </cc1:CustDropDownList>
                        </td>
                        <td style="width: 15%">
                        </td>
                        <td>
                            <cc1:CustRadioButton ID="radIsMial" runat="server" GroupName="MailType" Enabled="False"  />
                            <cc1:CustRadioButton ID="radIsSelf" runat="server" GroupName="MailType" Enabled="False"  />
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td style="text-align: right;width:15%; height: 25px;">
                            <cc1:CustLabel ID="lblshowExtra" runat="server" CurAlign="left" FractionalDigit="2" IsColon="True" 
                             IsCurrency="False" NeedDateFormat="False"
                             NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_01020300_035"
                            StickHeight="False" CurSymbol="&#163;"></cc1:CustLabel>                    
                        </td>
                        <td style="text-align: left;width:35%; height: 25px;" >
                            <cc1:CustCheckBox ID="chkshowExtra" runat="server" Enabled="False" /></td>
                        <td style="text-align: right;width:15%; height: 25px;">
                            <cc1:CustLabel ID="lblgetFee" runat="server" CurAlign="left" FractionalDigit="2" IsColon="True" 
                             IsCurrency="False" NeedDateFormat="False"
                             NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_01020300_009"
                             StickHeight="False" CurSymbol="&#163;"></cc1:CustLabel>                    
                        </td>
                        <td style="text-align: left;width:35%; height: 25px;">
                            <cc1:CustCheckBox ID="chkgetFee" runat="server" Enabled="False" /></td>    
                    </tr>
                     <tr class="trEven">
                        <td style="text-align: right;width:15%; height: 25px;">
                            <cc1:CustLabel ID="lblOwe" runat="server" CurAlign="left" FractionalDigit="2" IsColon="True" 
                             IsCurrency="False" NeedDateFormat="False"
                             NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_01020300_033"
                            StickHeight="False" CurSymbol="&#163;"></cc1:CustLabel>                    
                        </td>
                        <td style="text-align: left;width:35%; height: 25px;" >
                            <cc1:CustTextBox ID="txtOwe" runat="server" InputType="String" Enabled="False" EnableTheming="True"  ></cc1:CustTextBox>
                        </td>
                        <td style="text-align: right;width:15%; height: 25px;">
                            <cc1:CustLabel ID="lblPay" runat="server" CurAlign="left" FractionalDigit="2" IsColon="True" 
                             IsCurrency="False" NeedDateFormat="False"
                             NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_01020300_034"
                             StickHeight="False" CurSymbol="&#163;"></cc1:CustLabel>                    
                        </td>
                        <td style="text-align: left;width:35%; height: 25px;">
                            <cc1:CustTextBox ID="txtPay" runat="server" InputType="String" Enabled="False" EnableTheming="True"  ></cc1:CustTextBox>
                        </td>    
                    </tr>
                                  <tr class="trOdd">
                        <td style="text-align: right;width:15%; height: 25px;">
                            <cc1:CustLabel ID="lblPayDay" runat="server" CurAlign="left" FractionalDigit="2" IsColon="True" 
                             IsCurrency="False" NeedDateFormat="False"
                             NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_01020300_019"
                            StickHeight="False" CurSymbol="&#163;"></cc1:CustLabel>                    
                        </td>
                        <td style="text-align: left;width:35%; height: 25px;"  colspan="3">
                            <cc1:CustTextBox ID="txtPayDay" runat="server" Enabled="False" EnableTheming="True"
                                InputType="String"></cc1:CustTextBox></td> 
                    </tr>
                    <tr class="itemTitle" align="center" >
                    <td colspan="4">
                        <cc1:CustButton  ID="btnPrint" runat="server"  CssClass="smallButton" OnClick="btnPrint_Click" ShowID="04_01020300_036"  />&nbsp;&nbsp;
                        <cc1:CustButton  ID="btnBack" runat="server"  CssClass="smallButton" OnClick="btnBack_Click" ShowID="04_01020300_037"  /></td>
               </tr>
              <tr>
                    
    
            </ContentTemplate>
    </asp:UpdatePanel>   
    </form>
</body>
</html>
