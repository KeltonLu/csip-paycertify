<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P040301030002.aspx.cs" Inherits="Page_P040301030002" %>

<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
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
                <table width="100%" border="0" cellpadding="0" cellspacing="1">
                    <tr class="itemTitle">
                        <td colspan="4">
                            <li>
                                <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_03010300_001" StickHeight="False"></cc1:CustLabel></li>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 20%">
                            <cc1:CustLabel ID="lblSerialNo" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_03010300_009" StickHeight="False"></cc1:CustLabel></td>
                        <td  colspan="3">
                            <cc1:CustTextBox ID="txtSerialNo" Style="ime-mode: disabled; text-align: left" runat="server" Enabled="False"></cc1:CustTextBox></td>
                                
                    </tr>
                    <tr class="trEven">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblID_No" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_03010300_003" StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 35%">
                            <cc1:CustTextBox ID="txtID_No" runat="server" Enabled="False" MaxLength="11"></cc1:CustTextBox></td>
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblID_Name" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_03010300_004" StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 35%">
                            <cc1:CustTextBox ID="txtID_Name" runat="server" Enabled="False" MaxLength="20"></cc1:CustTextBox></td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblcardNO" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_03010300_005" StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 35%">
                            <cc1:CustTextBox ID="txtcardNO" runat="server" Enabled="False" MaxLength="20"></cc1:CustTextBox>
                        </td>
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblZip" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_03010300_011" StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 35%">
                            <cc1:CustTextBox ID="txtZip" runat="server" Enabled="False" MaxLength="3"></cc1:CustTextBox></td>
                    </tr>
                    <tr class="trEven">
                        <td align="right" style="width: 20%">
                            <cc1:CustLabel ID="lblAdd1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_03010300_012" StickHeight="False"></cc1:CustLabel></td>
                        <td  colspan="3">
                            <cc1:CustTextBox ID="txtAdd1" Style="ime-mode: disabled; text-align: left" runat="server" Enabled="False" MaxLength="50" Width="500px"></cc1:CustTextBox>
                        </td>
                    </tr>
                    <tr class="trOdd">
                     <td align="right" style="width: 20%">
                            <cc1:CustLabel ID="lblAdd2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_03010300_013" StickHeight="False"></cc1:CustLabel></td>
                        <td  colspan="3">
                            <cc1:CustTextBox ID="txtAdd2" Style="ime-mode: disabled; text-align: left" runat="server" Enabled="False" MaxLength="60" Width="500px"></cc1:CustTextBox>
                        </td>
                    </tr>
                     <tr class="trEven">
                          <td align="right" style="width: 20%">
                            <cc1:CustLabel ID="lblAdd3" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_03010300_014" StickHeight="False"></cc1:CustLabel></td>
                        <td  colspan="3">
                            <cc1:CustTextBox ID="txtAdd3" Style="ime-mode: disabled; text-align: left" runat="server" Enabled="False" MaxLength="100" Width="500px"></cc1:CustTextBox>
                        </td>
                    </tr>
                    <tr class="trOdd">
                    <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblRecievName" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_03010300_015" StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 35%">
                            <cc1:CustTextBox ID="txtRecievName" runat="server" Enabled="False" MaxLength="20"></cc1:CustTextBox>
                        </td>
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblGetType" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_03010300_010" StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 35%">
                            <cc1:CustRadioButtonList ID="rblGetType" runat="server" RepeatDirection="Horizontal">
                                <asp:ListItem>
                                </asp:ListItem>
                                <asp:ListItem>
                                </asp:ListItem>
                            </cc1:CustRadioButtonList></td>
                    </tr>
                     <tr class="trEven">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblKeyinDay" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_03010300_018" StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 35%">
                            <cc1:CustTextBox ID="txtKeyinDay" runat="server" Enabled="False" MaxLength="10"></cc1:CustTextBox></td>
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblMailDay" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_03010300_019" StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 35%">
                            <cc1:CustTextBox ID="txtMailDay" runat="server" Enabled="False" MaxLength="10"></cc1:CustTextBox></td>
                    </tr>
                    <tr class="trOdd">
                    <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblMailType" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_03010300_020" StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 35%">
                            <cc1:CustDropDownList ID="dropMailType" runat="server" Enabled="False"></cc1:CustDropDownList>
                        </td>
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblMailNo" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_03010300_021" StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 35%">
                            <cc1:CustTextBox ID="txtMailNo" runat="server" Enabled="False" MaxLength="10"></cc1:CustTextBox></td>
                    </tr>
                     <tr class="trEven">
                          <td align="right" style="width: 20%">
                            <cc1:CustLabel ID="lblPayOffDate" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_03010300_022" StickHeight="False"></cc1:CustLabel></td>
                        <td  colspan="3">
                            <cc1:CustTextBox ID="txtPayOffDate" Style="ime-mode: disabled; text-align: left" runat="server" Enabled="False" MaxLength="10"></cc1:CustTextBox></td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 20%">
                            <cc1:CustLabel ID="lblNote" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_03010300_023" StickHeight="False"></cc1:CustLabel></td>
                        <td  colspan="3">
                            <cc1:CustTextBox ID="txtNote" Style="ime-mode: disabled; text-align: left" runat="server" Enabled="False" TextMode="MultiLine" Width="500px"  Height="70px" MaxLength="200"></cc1:CustTextBox></td>
                    </tr>
                    <tr align="center" class="itemTitle">
                        <td colspan="4">
                            <asp:Button ID="btnMark" runat="server" class="smallButton" Style="width: 50px;"   OnClick="btnMark_Click"/>&nbsp;&nbsp;
                            <asp:Button ID="btnCancel" runat="server" class="smallButton" Style="width: 70px;"   OnClick="btnCancel_Click"/>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>

