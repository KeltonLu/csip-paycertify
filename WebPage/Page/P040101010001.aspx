<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P040101010001.aspx.cs" Inherits="Page_P040101010001" %>
<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript" src="../Common/Script/JavaScript.js"></script>
    <script type="text/javascript" src="../Common/Script/JQuery/jquery-1.3.2.min.js"></script>
    <script type="text/javascript" src="../Common/Script/JQuery/jquery-ui-1.7.min.js"></script>
    <script type="text/javascript" src="../Common/Script/JQuery/WINF_JQuery.js"></script>
    <link href="../App_Themes/Default/global.css" type="text/css" rel="stylesheet" />
    <script language="javascript" type="text/javascript">
    function ResetHiddenTextbox()
    {
        $("#txtHiden1").attr("value","");   //* SBAMZ Comfirm
        $("#txtHiden2").attr("value","");   //*
        $("#txtHiden3").attr("value","");
        $("#txtHiden4").attr("value","");        
        $("#txtHiden5").attr("value","");
        
    }
   
        function ConfrimText(strConfirmText, strHiddenControlID) {
         // LoadSweetalert2Libraray();
         AlertYesNoOther({ title: strConfirmText, bn: "#btnHiden1", hid: strHiddenControlID });
        //if(confirm(strConfirmText) == false)
        //{
        //    ResetHiddenTextbox();
        //}
        //else
        //{
        //    $("#"+strHiddenControlID).attr("value","Y");
        //    $("#btnHiden1").click();
        //}
    }
        function ChkCardTypeRadio()
        {
            //if($("#radCardType_3").attr("Checked"))
            if (document.getElementById('radCardType_3').checked)
            {
                //$("#txtOtherID").attr("Enabled","Enabled");
                document.getElementById('txtOtherID').removeAttribute("disabled");
            }
            else
            {
                $("#txtOtherID").attr("value","");
                $("#txtOtherID").attr("disabled","disabled");
            }
        }

        /**
 * @Description 彈跳視窗基本YesNo格式
 * @author      Neal 修改
 * @version     v1.2022/02/23 10:00:00
 * @param       {object} [options={}] 參數資訊
 * @param       {string} options.title 標題
 * @param       {string} options.text 訊息
 * @param       {string} options.icon 圖示
 * @param       {object} options.bn 按鈕元件
 */
        const AlertYesNoOther = function (options) {

            LoadSweetalert2Libraray();

            options = (ValIsNullOrEmpty(options) ? {} : options);
            const bn = options.bn;
            const hid = options.hid;
            const ops = {};
            const arr = ['title', 'text', 'icon'];
            arr.forEach(function (key) {
                if (!ValIsNullOrEmpty(options[key])) {
                    ops[key] = options[key];
                }
            });
            ops.showCancelButton = true;
            ops.confirmButtonText = '是';
            ops.cancelButtonText = '否';

            Swal.fire(ops).then(function (res) {
                if (res.isConfirmed) {
                    $("#" + hid).attr("value", "Y");
                    $(bn).click();
                }
                else {
                    ResetHiddenTextbox();
                }
            });
        };
    </script>
    
    <style type="text/css" >

   .btnHiden

    {display:none; }

    </style>

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
                                    SetBreak="False" SetOmit="False" ShowID="04_01010100_001" StickHeight="False"></cc1:CustLabel></li>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td style="text-align: right; width: 15%; height: 25px;">
                            <cc1:CustLabel ID="lblUser_ID" runat="server" FractionalDigit="2" IsColon="True"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" StickHeight="False" ShowID="04_01010100_002" CurAlign="left"
                                CurSymbol="£"></cc1:CustLabel>
                        </td>
                        <td style="text-align: left; width: 85%; height: 25px;">
                            <cc1:CustTextBox ID="txtUser_ID" runat="server" MaxLength="11" InputType="LetterAndInt"></cc1:CustTextBox>
                        </td>
                    </tr>
                    <tr class="trEven">
                        <td style="text-align: right; width: 15%; height: 25px;">
                            <cc1:CustLabel ID="lblType" runat="server" FractionalDigit="2" IsColon="True" IsCurrency="False"
                                NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False"
                                StickHeight="False" ShowID="04_01010100_003"></cc1:CustLabel>
                        </td>
                        <td style="text-align: left; width: 85%; height: 25px;">
                            <table width="100%" border="0" cellpadding="0" cellspacing="0">
                                <tr>
                                    <td style="height: 25px">
                                        <cc1:CustRadioButton ID="radType_C" runat="server" GroupName="radType" Checked="true" /></td>
                                    <td style="height: 25px">
                                        <cc1:CustRadioButton ID="radType_N" runat="server" GroupName="radType" /></td>
                                    <td style="height: 25px">
                                        <cc1:CustRadioButton ID="radType_M" runat="server" GroupName="radType" /></td>
                                    <td style="height: 25px">
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <cc1:CustRadioButton ID="radType_G" runat="server" GroupName="radType" /></td>
                                    <td>
                                        <cc1:CustRadioButton ID="radType_T" runat="server" GroupName="radType" /></td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td style="text-align: right; width: 15%; height: 25px;">
                            <cc1:CustLabel ID="lblCardType" runat="server" FractionalDigit="2" IsColon="True"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" StickHeight="False" ShowID="04_01010100_004"></cc1:CustLabel>
                        </td>
                        <td align="left" style="width: 85%; height: 25px;">
                            <cc1:CustRadioButton ID="radCardType_0" runat="server" GroupName="radCardType" onclick="Javascript:ChkCardTypeRadio();"  />
                            <cc1:CustRadioButton ID="radCardType_1" runat="server" GroupName="radCardType" onclick="Javascript:ChkCardTypeRadio();" />
                            <cc1:CustRadioButton ID="radCardType_2" runat="server" GroupName="radCardType" onclick="Javascript:ChkCardTypeRadio();" Checked="True" />
                            <cc1:CustCheckBox ID="chkOther" runat="server" />
                            <cc1:CustRadioButton ID="radCardType_3" runat="server" GroupName="radCardType" onclick="Javascript:ChkCardTypeRadio();" />   
                            <cc1:CustTextBox ID="txtOtherID" runat="server" MaxLength="11" InputType="LetterAndInt" ></cc1:CustTextBox>   
                        </td>
                    </tr>
                    <tr class="itemTitle" align="center">
                        <td colspan="2" style="height: 25px">
                        <cc1:CustButton ID="btnQuery" runat="server" CssClass="smallButton" OnClick="btnQuery_Click" ShowID="04_01010100_017"/>                       </tr>
                </table>
            </ContentTemplate>
            
        </asp:UpdatePanel>
        <cc1:CustTextBox ID="txtHiden1" runat="server" Visible="True" CssClass="btnHiden"></cc1:CustTextBox>
        <cc1:CustTextBox ID="txtHiden2" runat="server" Visible="True" CssClass="btnHiden"></cc1:CustTextBox>
        <cc1:CustTextBox ID="txtHiden3" runat="server" Visible="True" CssClass="btnHiden"></cc1:CustTextBox>  
        <cc1:CustTextBox ID="txtHiden4" runat="server" Visible="True" CssClass="btnHiden"></cc1:CustTextBox> 
        <cc1:CustTextBox ID="txtHiden5" runat="server" Visible="True" CssClass="btnHiden"></cc1:CustTextBox>       
        <asp:Button ID="btnHiden1" runat="server" Text="Button" OnClick="btnHiden_Click" CssClass="btnHiden" />
    </form>
    <script language="javascript" type="text/javascript">
    ChkCardTypeRadio();
    </script>
</body>
</html>
