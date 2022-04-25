<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P040102040002.aspx.cs" Inherits="Page_P040102040002" %>
<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<%@ Register Src="../Common/Controls/CustAddress.ascx" TagName="CustAddress" TagPrefix="uc1" %>

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
		                <td colspan="4">
		                    <li>
                                <cc1:CustLabel ID="lblTitle" runat="server" ShowID="04_01020400_001"> </cc1:CustLabel></li>
		                </td>
	                </tr>
	                <tr class="trOdd">
                        <td style="text-align: right;width:15%; height: 25px;">
                            <cc1:CustLabel ID="lblUserID" runat="server" CurAlign="left" FractionalDigit="2" IsColon="True" 
                             IsCurrency="False" NeedDateFormat="False"
                             NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_01020400_002"
                            StickHeight="False" CurSymbol="&#163;"></cc1:CustLabel>                    
                        </td>
                        <td style="text-align: left;width:35%; height: 25px;" >
                            <cc1:CustTextBox ID="txtUserID" runat="server" MaxLength="11" InputType="Upper"   ></cc1:CustTextBox>
                        </td>
                        <td style="text-align: right;width:15%; height: 25px;">
                            <cc1:CustLabel ID="lblUserName" runat="server" CurAlign="left" FractionalDigit="2" IsColon="True" 
                             IsCurrency="False" NeedDateFormat="False"
                             NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_01020400_013"
                             StickHeight="False" CurSymbol="&#163;"></cc1:CustLabel>                    
                        </td>
                        <td style="text-align: left;width:35%; height: 25px;">
                            <cc1:CustTextBox ID="txtUserName" runat="server" MaxLength="11" InputType="String"  ></cc1:CustTextBox>
                        </td>    
                    </tr>
                    <tr class="trEven">
                        <td style="text-align: right;width:15%; height: 25px;">
                            <cc1:CustLabel ID="lblPayName" runat="server" CurAlign="left" FractionalDigit="2" IsColon="True" 
                             IsCurrency="False" NeedDateFormat="False"
                             NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_01020400_022"
                            StickHeight="False" CurSymbol="&#163;"></cc1:CustLabel>                    
                        </td>
                        <td style="text-align: left;width:35%; height: 25px;" >
                            <cc1:CustTextBox ID="txtPayName" runat="server" MaxLength="8" InputType="String"  ></cc1:CustTextBox>
                        </td>
                        <td style="text-align: right;width:15%; height: 25px;">
                            <cc1:CustLabel ID="lblZip" runat="server" CurAlign="left" FractionalDigit="2" IsColon="True" 
                             IsCurrency="False" NeedDateFormat="False"
                             NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_01020400_023"
                             StickHeight="False" CurSymbol="&#163;"></cc1:CustLabel>                    
                        </td>
                        <td style="text-align: left;width:35%; height: 25px;">
                            <cc1:CustTextBox ID="txtZip" runat="server" MaxLength="3" InputType="String"  ></cc1:CustTextBox>
                        </td>    
                    </tr>
                    <tr class="trOdd">
                        <td style="text-align: right;width:15%; height: 25px;">
                            <cc1:CustLabel ID="lblAdd1" runat="server" CurAlign="left" FractionalDigit="2" IsColon="True" 
                             IsCurrency="False" NeedDateFormat="False"
                             NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_01020400_024"
                             StickHeight="False" CurSymbol="&#163;"></cc1:CustLabel>                    
                        </td>
                        <td style="text-align: left;width:35%; height: 25px;" colspan="3">
                            <uc1:CustAddress id="CustAdd1" runat="server"></uc1:CustAddress>
                        </td>
                    </tr>
                    <tr class="trEven">
                    <td style="text-align: right;width:15%; height: 25px;">
                        <cc1:CustLabel ID="lblAdd2" runat="server" CurAlign="left" FractionalDigit="2" IsColon="True" 
                        IsCurrency="False" NeedDateFormat="False"
                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_01020400_025"
                        StickHeight="False" CurSymbol="&#163;"></cc1:CustLabel>                    
                    </td>
                    <td style="text-align: left;width:50%; height: 25px;" colspan="3">
                        <cc1:CustTextBox ID="txtAdd2" runat="server" MaxLength="28" Width="410px"></cc1:CustTextBox>
                    </td>
                    </tr>
                     <tr class="trOdd">
                    <td style="text-align: right;width:15%; height: 25px;">
                        <cc1:CustLabel ID="lblAdd3" runat="server" CurAlign="left" FractionalDigit="2" IsColon="True" 
                        IsCurrency="False" NeedDateFormat="False"
                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_01020400_026"
                        StickHeight="False" CurSymbol="&#163;"></cc1:CustLabel>                    
                    </td>
                    <td style="text-align: left;width:50%; height: 25px;" colspan="3">
                        <cc1:CustTextBox ID="txtAdd3" runat="server" MaxLength="28" Width="410px" ></cc1:CustTextBox>
                    </td>
                    </tr>
                    <tr class="trEven">
                        <td style="text-align: right;width:15%; height: 25px;">
                            <cc1:CustLabel ID="lblConsignee" runat="server" CurAlign="left" FractionalDigit="2" IsColon="True" 
                             IsCurrency="False" NeedDateFormat="False"
                             NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_01020400_027"
                            StickHeight="False" CurSymbol="&#163;"></cc1:CustLabel>                    
                        </td>
                        <td style="text-align: left;width:35%; height: 25px;" >
                            <cc1:CustTextBox ID="txtConsignee" runat="server" MaxLength="7" InputType="String"  ></cc1:CustTextBox>
                        </td>
                         <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblIsFree" runat="server" IsColon="True" ShowID="04_01010100_065"></cc1:CustLabel></td>
                        <td style="width: 35%">
                            <cc1:CustCheckBox ID="chkIsFree" runat="server" Enabled="false" /></td>
                    </tr>
                     <tr class="trOdd">
                     <td style="text-align: right;width:15%; height: 25px;">
                            <cc1:CustLabel ID="lblMailDay" runat="server" CurAlign="left" FractionalDigit="2" IsColon="True" 
                             IsCurrency="False" NeedDateFormat="False"
                             NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_01020400_028"
                             StickHeight="False" CurSymbol="&#163;"></cc1:CustLabel>                    
                        </td>
                        <td style="text-align: left;width:35%; height: 25px;">
                            <cc1:CustTextBox ID="txtMailDay" runat="server" MaxLength="10" InputType="String"  ></cc1:CustTextBox>
                        </td>   
                        
                    <td style="text-align: right;width:15%; height: 25px;">
                        <cc1:CustLabel ID="lblMailNo" runat="server" CurAlign="left" FractionalDigit="2" IsColon="True" 
                        IsCurrency="False" NeedDateFormat="False"
                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_01020400_029"
                        StickHeight="False" CurSymbol="&#163;"></cc1:CustLabel>                    
                    </td>
                    <td style="text-align: left;width:50%; height: 25px;">
                        <cc1:CustTextBox ID="txtMailNo" runat="server" MaxLength="10" ></cc1:CustTextBox>
                    </td>
                    </tr>
                    <tr class="trEven">
                    <td style="text-align: right;width:15%; height: 25px;">
                        <cc1:CustLabel ID="lblNote" runat="server" CurAlign="left" FractionalDigit="2" IsColon="True" 
                        IsCurrency="False" NeedDateFormat="False"
                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_01020400_030"
                        StickHeight="False" CurSymbol="&#163;"></cc1:CustLabel>                    
                    </td>
                    <td style="text-align: left;width:50%; height: 25px;" colspan="3">
                        <cc1:CustTextBox ID="txtNote" runat="server" MaxLength="48" Width="410px"></cc1:CustTextBox>
                    </td>
                    </tr>
                     <tr class="trOdd">
                        <td style="text-align: right;width:15%; height: 25px;">
                            <cc1:CustLabel ID="lblMakeUpDate" runat="server" CurAlign="left" FractionalDigit="2" IsColon="True" 
                             IsCurrency="False" NeedDateFormat="False"
                             NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_01020400_031"
                            StickHeight="False" CurSymbol="&#163;"></cc1:CustLabel>                    
                        </td>
                        <td style="text-align: left;width:35%; height: 25px;" >
                            <cc1:CustTextBox ID="txtMakeUpDate" runat="server" MaxLength="10" InputType="String" ></cc1:CustTextBox>
                        </td>
                        <td style="text-align: right;width:15%; height: 25px;">
                            <cc1:CustLabel ID="lblEndDate" runat="server" CurAlign="left" FractionalDigit="2" IsColon="True" 
                             IsCurrency="False" NeedDateFormat="False"
                             NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_01020400_032"
                             StickHeight="False" CurSymbol="&#163;"></cc1:CustLabel>                    
                        </td>
                        <td style="text-align: left;width:35%; height: 25px;">
                            <cc1:CustTextBox ID="txtEndDate" runat="server" MaxLength="10" InputType="String"  ></cc1:CustTextBox>
                        </td>    
                    </tr>
                    <tr class="trEven">
                         <td style="text-align: right;width:15%; height: 25px;">
                            <cc1:CustLabel ID="lblMailType" runat="server" CurAlign="left" FractionalDigit="2" IsColon="True" 
                             IsCurrency="False" NeedDateFormat="False"
                             NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_01020400_033"
                            StickHeight="False" CurSymbol="&#163;"></cc1:CustLabel>                    
                        </td>
                        <td>
                            <cc1:CustDropDownList ID="dropMailType" runat="server" IsValEmpty="False">
                            </cc1:CustDropDownList>
                        </td>
                        <td style="width: 15%">
                        </td>
                        <td>
                            <cc1:CustRadioButton ID="radIsMial" runat="server" GroupName="radMailType"  />
                            <cc1:CustRadioButton ID="radIsSelf" runat="server" GroupName="radMailType"  />
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td style="text-align: right;width:15%; height: 25px;">
                            <cc1:CustLabel ID="lblshowExtra" runat="server" CurAlign="left" FractionalDigit="2" IsColon="True" 
                             IsCurrency="False" NeedDateFormat="False"
                             NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_01020400_043"
                            StickHeight="False" CurSymbol="&#163;"></cc1:CustLabel>                    
                        </td>
                        <td style="text-align: left;width:35%; height: 25px;" >
                            <cc1:CustCheckBox ID="chkshowExtra" runat="server" />
                        </td>
                        <td style="text-align: right;width:15%; height: 25px;">
                            <cc1:CustLabel ID="lblgetFee" runat="server" CurAlign="left" FractionalDigit="2" IsColon="True" 
                             IsCurrency="False" NeedDateFormat="False"
                             NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_01020400_042"
                             StickHeight="False" CurSymbol="&#163;"></cc1:CustLabel>                    
                        </td>
                        <td style="text-align: left;width:35%; height: 25px;">
                            <cc1:CustCheckBox ID="chkgetFee" runat="server" />
                        </td>    
                    </tr>
                    <tr class="trEven">
                        <td style="text-align: right;width:15%; height: 25px;">
                            <cc1:CustLabel ID="lblOwe" runat="server" CurAlign="left" FractionalDigit="2" IsColon="True" 
                             IsCurrency="False" NeedDateFormat="False"
                             NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_01020400_036"
                            StickHeight="False" CurSymbol="&#163;"></cc1:CustLabel>                    
                        </td>
                        <td style="text-align: left;width:35%; height: 25px;" >
                            <cc1:CustTextBox ID="txtOwe" runat="server" MaxLength="10" InputType="String"  ></cc1:CustTextBox>
                        </td>
                        <td style="text-align: right;width:15%; height: 25px;">
                            <cc1:CustLabel ID="lblPay" runat="server" CurAlign="left" FractionalDigit="2" IsColon="True" 
                             IsCurrency="False" NeedDateFormat="False"
                             NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_01020400_037"
                             StickHeight="False" CurSymbol="&#163;"></cc1:CustLabel>                    
                        </td>
                        <td style="text-align: left;width:35%; height: 25px;">
                            <cc1:CustTextBox ID="txtPay" runat="server" MaxLength="10" InputType="String"  ></cc1:CustTextBox>
                        </td>    
                    </tr>
                     <tr class="trOdd">
                    <td style="text-align: right;width:15%; height: 25px;">
                        <cc1:CustLabel ID="lblPayDay" runat="server" CurAlign="left" FractionalDigit="2" IsColon="True" 
                        IsCurrency="False" NeedDateFormat="False"
                        NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_01020400_038"
                        StickHeight="False" CurSymbol="&#163;"></cc1:CustLabel>                    
                    </td>
                    <td style="text-align: left;width:50%; height: 25px;" colspan="3">
                        <cc1:CustTextBox ID="txtPayDay" runat="server" MaxLength="10"></cc1:CustTextBox>
                    </td>
                    </tr>
                     <tr class="trEven">
                        <td style="text-align: right;width:15%; height: 25px;">
                            <cc1:CustLabel ID="lblReturnDay" runat="server" CurAlign="left" FractionalDigit="2" IsColon="True" 
                             IsCurrency="False" NeedDateFormat="False"
                             NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_01020400_039"
                            StickHeight="False" CurSymbol="&#163;"></cc1:CustLabel>                    
                        </td>
                        <td style="text-align: left;width:35%; height: 25px;" >
                            <cc1:CustTextBox ID="txtReturnDay" runat="server" MaxLength="10" InputType="String"  ></cc1:CustTextBox>
                        </td>
                        <td style="text-align: right;width:15%; height: 25px;">
                            <cc1:CustLabel ID="lblReturnReason" runat="server" CurAlign="left" FractionalDigit="2" IsColon="True" 
                             IsCurrency="False" NeedDateFormat="False"
                             NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_01020400_040"
                             StickHeight="False" CurSymbol="&#163;"></cc1:CustLabel>                    
                        </td>
                        <td style="text-align: left;width:35%; height: 25px;">
                            <cc1:CustTextBox ID="txtReturnReason" runat="server" MaxLength="50" InputType="String"  ></cc1:CustTextBox>
                        </td>    
                    </tr>
                    <tr class="itemTitle" align="center" >
                    <td colspan="4">
                        <cc1:CustButton  ID="btnBack" runat="server"  CssClass="smallButton" OnClick="btnBack_Click" ShowID="04_01020400_041"  />&nbsp;&nbsp;
                    </td>
               </tr>
              <tr>
                    
    
            </ContentTemplate>
    </asp:UpdatePanel>   
    </form>
</body>
</html>
