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
    /// EntityPay_CardSV
    /// </summary>
    [Serializable()]
    [AttributeTable("Pay_CardSV")]
    public class EntityPay_CardSV : Entity
    {
        
        private string _SERIALNO;
        
        /// <summary>
        /// SERIALNO
        /// </summary>
        public static string M_SERIALNO = "SERIALNO";
        
        private decimal _NO;
        
        /// <summary>
        /// NO
        /// </summary>
        public static string M_NO = "NO";
        
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
        
        private string _TYPECARD;
        
        /// <summary>
        /// TYPECARD
        /// </summary>
        public static string M_TYPECARD = "TYPECARD";
        
        private string _BLK;
        
        /// <summary>
        /// BLK
        /// </summary>
        public static string M_BLK = "BLK";
        
        /// <summary>
        /// SERIALNO
        /// </summary>
        [AttributeField("SERIALNO", "System.String", false, false, false, "String")]
        public string SERIALNO
        {
            get
            {
                return this._SERIALNO;
            }
            set
            {
                this._SERIALNO = value;
            }
        }
        
        /// <summary>
        /// NO
        /// </summary>
        [AttributeField("NO", "System.Decimal", false, false, false, "Decimal")]
        public decimal NO
        {
            get
            {
                return this._NO;
            }
            set
            {
                this._NO = value;
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
        /// BLK
        /// </summary>
        [AttributeField("BLK", "System.String", false, false, false, "String")]
        public string BLK
        {
            get
            {
                return this._BLK;
            }
            set
            {
                this._BLK = value;
            }
        }
    }
    
    /// <summary>
    /// EntityPay_CardSV
    /// </summary>
    [Serializable()]
    public class EntityPay_CardSVSet : EntitySet<EntityPay_CardSV>
    {
    }
}
