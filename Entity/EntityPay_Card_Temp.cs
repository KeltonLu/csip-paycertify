//------------------------------------------------------------------------------
// <auto-generated>
//     這段程式碼是由工具產生的。
//     執行階段版本:2.0.50727.832
//
//     對這個檔案所做的變更可能會造成錯誤的行為，而且如果重新產生程式碼，
//     變更將會遺失。
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using Framework.Data.OM.OMAttribute;
using Framework.Data.OM;
using Framework.Data.OM.Collections;


namespace CSIPPayCertify.EntityLayer
{
    
    
    /// <summary>
    /// EntityPay_Card_Temp
    /// </summary>
    [Serializable()]
    [AttributeTable("Pay_Card_Temp")]
    public class EntityPay_Card_Temp : Entity
    {
        
        private decimal _CARDTEMP_ID;
        
        /// <summary>
        /// CARDTEMP_ID
        /// </summary>
        public static string M_CARDTEMP_ID = "CARDTEMP_ID";
        
        private string _CARDNO;
        
        /// <summary>
        /// CARDNO
        /// </summary>
        public static string M_CARDNO = "CARDNO";
        
        private string _CUSID;
        
        /// <summary>
        /// CUSID
        /// </summary>
        public static string M_CUSID = "CUSID";
        
        private string _CUSNAME;
        
        /// <summary>
        /// CUSNAME
        /// </summary>
        public static string M_CUSNAME = "CUSNAME";
        
        private string _OPENDAY;
        
        /// <summary>
        /// OPENDAY
        /// </summary>
        public static string M_OPENDAY = "OPENDAY";
        
        private string _CODE1;
        
        /// <summary>
        /// CODE1
        /// </summary>
        public static string M_CODE1 = "CODE1";
        
        private string _CLOSEDAY1;
        
        /// <summary>
        /// CLOSEDAY1
        /// </summary>
        public static string M_CLOSEDAY1 = "CLOSEDAY1";
        
        private string _CODE2;
        
        /// <summary>
        /// CODE2
        /// </summary>
        public static string M_CODE2 = "CODE2";
        
        private string _CLOSEDAY2;
        
        /// <summary>
        /// CLOSEDAY2
        /// </summary>
        public static string M_CLOSEDAY2 = "CLOSEDAY2";
        
        private string _TYPECARD;
        
        /// <summary>
        /// TYPECARD
        /// </summary>
        public static string M_TYPECARD = "TYPECARD";
        
        private string _SOLD;
        
        /// <summary>
        /// SOLD
        /// </summary>
        public static string M_SOLD = "SOLD";
        
        private string _VALID;
        
        /// <summary>
        /// VALID
        /// </summary>
        public static string M_VALID = "VALID";
        
        private string _LASTPAYDATE;
        
        /// <summary>
        /// LASTPAYDATE
        /// </summary>
        public static string M_LASTPAYDATE = "LASTPAYDATE";
        
        /// <summary>
        /// CARDTEMP_ID
        /// </summary>
        [AttributeField("CARDTEMP_ID", "System.Decimal", false, false, false, "Decimal")]
        public decimal CARDTEMP_ID
        {
            get
            {
                return this._CARDTEMP_ID;
            }
            set
            {
                this._CARDTEMP_ID = value;
            }
        }
        
        /// <summary>
        /// CARDNO
        /// </summary>
        [AttributeField("CARDNO", "System.String", false, false, false, "String")]
        public string CARDNO
        {
            get
            {
                return this._CARDNO;
            }
            set
            {
                this._CARDNO = value;
            }
        }
        
        /// <summary>
        /// CUSID
        /// </summary>
        [AttributeField("CUSID", "System.String", false, false, false, "String")]
        public string CUSID
        {
            get
            {
                return this._CUSID;
            }
            set
            {
                this._CUSID = value;
            }
        }
        
        /// <summary>
        /// CUSNAME
        /// </summary>
        [AttributeField("CUSNAME", "System.String", false, false, false, "String")]
        public string CUSNAME
        {
            get
            {
                return this._CUSNAME;
            }
            set
            {
                this._CUSNAME = value;
            }
        }
        
        /// <summary>
        /// OPENDAY
        /// </summary>
        [AttributeField("OPENDAY", "System.String", false, false, false, "String")]
        public string OPENDAY
        {
            get
            {
                return this._OPENDAY;
            }
            set
            {
                this._OPENDAY = value;
            }
        }
        
        /// <summary>
        /// CODE1
        /// </summary>
        [AttributeField("CODE1", "System.String", false, false, false, "String")]
        public string CODE1
        {
            get
            {
                return this._CODE1;
            }
            set
            {
                this._CODE1 = value;
            }
        }
        
        /// <summary>
        /// CLOSEDAY1
        /// </summary>
        [AttributeField("CLOSEDAY1", "System.String", false, false, false, "String")]
        public string CLOSEDAY1
        {
            get
            {
                return this._CLOSEDAY1;
            }
            set
            {
                this._CLOSEDAY1 = value;
            }
        }
        
        /// <summary>
        /// CODE2
        /// </summary>
        [AttributeField("CODE2", "System.String", false, false, false, "String")]
        public string CODE2
        {
            get
            {
                return this._CODE2;
            }
            set
            {
                this._CODE2 = value;
            }
        }
        
        /// <summary>
        /// CLOSEDAY2
        /// </summary>
        [AttributeField("CLOSEDAY2", "System.String", false, false, false, "String")]
        public string CLOSEDAY2
        {
            get
            {
                return this._CLOSEDAY2;
            }
            set
            {
                this._CLOSEDAY2 = value;
            }
        }
        
        /// <summary>
        /// TYPECARD
        /// </summary>
        [AttributeField("TYPECARD", "System.String", false, false, false, "String")]
        public string TYPECARD
        {
            get
            {
                return this._TYPECARD;
            }
            set
            {
                this._TYPECARD = value;
            }
        }
        
        /// <summary>
        /// SOLD
        /// </summary>
        [AttributeField("SOLD", "System.String", false, false, false, "String")]
        public string SOLD
        {
            get
            {
                return this._SOLD;
            }
            set
            {
                this._SOLD = value;
            }
        }
        
        /// <summary>
        /// VALID
        /// </summary>
        [AttributeField("VALID", "System.String", false, false, false, "String")]
        public string VALID
        {
            get
            {
                return this._VALID;
            }
            set
            {
                this._VALID = value;
            }
        }
        
        /// <summary>
        /// LASTPAYDATE
        /// </summary>
        [AttributeField("LASTPAYDATE", "System.String", false, false, false, "String")]
        public string LASTPAYDATE
        {
            get
            {
                return this._LASTPAYDATE;
            }
            set
            {
                this._LASTPAYDATE = value;
            }
        }
    }
    
    /// <summary>
    /// EntityPay_Card_Temp
    /// </summary>
    [Serializable()]
    public class EntityPay_Card_TempSet : EntitySet<EntityPay_Card_Temp>
    {
    }
}
