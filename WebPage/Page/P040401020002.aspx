<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P040401020002.aspx.cs" Inherits="Page_P040401020002" %>

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
                                <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2" IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_04010200_001" StickHeight="False"></cc1:CustLabel></li>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 20%">
                            <cc1:CustLabel ID="lblSerialNo" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_04010200_009" StickHeight="False"></cc1:CustLabel></td>
                        <td  colspan="3">
                            <cc1:CustTextBox ID="txtSerialNo" Style="ime-mode: disabled; text-align: left" runat="server" Enabled="False" MaxLength="12"></cc1:CustTextBox>
                        </td>
                                
                    </tr>
                    <tr class="trEven">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblID_No" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_04010200_003" StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 35%">
                            <cc1:CustTextBox ID="txtID_No" runat="server" Enabled="False" MaxLength="11"></cc1:CustTextBox></td>
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblID_Name" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_04010200_004" StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 35%">
                            <cc1:CustTextBox ID="txtID_Name" runat="server" Enabled="False" MaxLength="10"></cc1:CustTextBox></td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblcardNO" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_04010200_005" StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 35%">
                            <cc1:CustTextBox ID="txtcardNO" runat="server" Enabled="False" MaxLength="16"></cc1:CustTextBox>
                        </td>
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblZip" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_04010200_011" StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 35%">
                            <cc1:CustTextBox ID="txtZip" runat="server" ReadOnly="false" MaxLength="3" InputType="Int"></cc1:CustTextBox></td>
                    </tr>
                    <tr class="trEven">
                        <td align="right" style="width: 20%">
                            <cc1:CustLabel ID="lblAdd1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_04010200_012" StickHeight="False"></cc1:CustLabel></td>
                        <td  colspan="3">
                            <cc1:CustTextBox ID="txtAdd1" runat="server" ReadOnly="false" MaxLength="25" Width="500px"></cc1:CustTextBox>
                        </td>
                    </tr>
                    <tr class="trOdd">
                     <td align="right" style="width: 20%">
                            <cc1:CustLabel ID="lblAdd2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_04010200_013" StickHeight="False"></cc1:CustLabel></td>
                        <td  colspan="3">
                            <cc1:CustTextBox ID="txtAdd2" runat="server" ReadOnly="false" MaxLength="30" Width="500px"></cc1:CustTextBox>
                        </td>
                    </tr>
                     <tr class="trEven">
                          <td align="right" style="width: 20%">
                            <cc1:CustLabel ID="lblAdd3" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_04010200_014" StickHeight="False"></cc1:CustLabel></td>
                        <td  colspan="3">
                            <cc1:CustTextBox ID="txtAdd3" runat="server" ReadOnly="false" MaxLength="50" Width="500px"></cc1:CustTextBox>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblPayName" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_04010200_030" StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 35%">
                            <cc1:CustTextBox ID="txtPayName" runat="server" ReadOnly="false" MaxLength="10"></cc1:CustTextBox>
                        </td>
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblPayDay" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_04010200_031" StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 35%">
                             <cc1:DatePicker ID="dtpPayDay" Text="" MaxLength="10" runat="server" Width="150"></cc1:DatePicker></td>
                    </tr>
                     <tr class="trEven">
                          <td align="right" style="width: 20%">
                            <cc1:CustLabel ID="lblPay" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_04010200_032" StickHeight="False"></cc1:CustLabel></td>
                        <td  colspan="3">
                            <cc1:CustTextBox ID="txtPay" runat="server" ReadOnly="false" MaxLength="9" InputType="Int"></cc1:CustTextBox></td>
                    </tr>
                    <tr class="trOdd">
                    <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblRecievName" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_04010200_015" StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 35%">
                            <cc1:CustTextBox ID="txtRecievName" runat="server" ReadOnly="false" MaxLength="10"></cc1:CustTextBox>
                        </td>
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblGetType" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_04010200_010" StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 35%">
                            <cc1:CustRadioButtonList ID="radlGetType" runat="server" RepeatDirection="Horizontal" AutoPostBack="True" OnSelectedIndexChanged="radlGetType_SelectedIndexChanged">
                            </cc1:CustRadioButtonList></td>
                    </tr>
                     <tr class="trEven">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblKeyinDay" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_04010200_018" StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 35%">
                            <cc1:DatePicker ID="dtpKeyinDay" Text="" MaxLength="10" runat="server" Width="150"></cc1:DatePicker></td>
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblMailDay" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_04010200_019" StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 35%">
                            <cc1:DatePicker ID="dtpMailDay" Text="" MaxLength="10" runat="server" Width="150"></cc1:DatePicker></td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblMailType" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_04010200_020" StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 35%">
                            <cc1:CustDropDownList ID="dropMailType" runat="server" ></cc1:CustDropDownList>
                        </td>
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblMailNo" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_04010200_021" StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 35%">
                            <cc1:CustTextBox ID="txtMailNo" runat="server"  MaxLength="10" InputType="Int"></cc1:CustTextBox></td>
                    </tr>
                     <tr class="trEven">
                          <td align="right" style="width: 20%">
                            <cc1:CustLabel ID="lblPayOffDate" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_04010200_022" StickHeight="False"></cc1:CustLabel></td>
                        <td  colspan="3">
                            <cc1:DatePicker ID="dtpPayOffDate" Text="" MaxLength="10" runat="server" Width="150"></cc1:DatePicker></td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 20%">
                            <cc1:CustLabel ID="lblNote" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_04010200_023" StickHeight="False"></cc1:CustLabel></td>
                        <td  colspan="3">
                            <cc1:CustTextBox ID="txtNote" TextMode="MultiLine" Width="500px"  Height="70px" MaxLength="100" runat="server" InputType="Memo"></cc1:CustTextBox></td>
                    </tr>
                    <tr class="trEven">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblSelfDay" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_04010200_027" StickHeight="False"></cc1:CustLabel></td>
                        <td  style="width: 35%">
                            <cc1:DatePicker ID="dtpSelfDay" Text="" MaxLength="10" runat="server" Width="150"></cc1:DatePicker>
                        </td>
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblOtherDay" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_04010200_028" StickHeight="False"></cc1:CustLabel></td>
                        <td  style="width: 35%">
                            <cc1:DatePicker ID="dtpOtherDay" Text="" MaxLength="10" runat="server" Width="150"></cc1:DatePicker>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 20%">
                            <cc1:CustLabel ID="lblselfnote" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_04010200_023" StickHeight="False"></cc1:CustLabel></td>
                        <td  colspan="3">
                            <cc1:CustTextBox ID="txtselfnote"  runat="server"  TextMode="MultiLine" Width="500px"  Height="70px" MaxLength="100" InputType="Memo"></cc1:CustTextBox></td>
                    </tr>
                    <tr align="center" class="itemTitle">
                        <td colspan="4">
                            <cc1:CustButton ID="btnMark" runat="server" class="smallButton" Style="width: 50px;"  ShowID="04_04010200_024" OnClick="btnMark_Click"/>&nbsp;&nbsp;
                            <cc1:CustButton ID="btnPrint" runat="server" class="smallButton" Style="width: 50px;"  ShowID="04_04010200_026" OnClick="btnPrint_Click" />&nbsp;&nbsp;
                            <cc1:CustButton ID="btnCancel" runat="server" class="smallButton" Style="width: 70px;"  ShowID="04_04010200_025" OnClick="btnCancel_Click"/>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
