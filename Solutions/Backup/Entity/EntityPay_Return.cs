//------------------------------------------------------------------------------
// <auto-generated>
//     �o�q�{���X�O�Ѥu�㲣�ͪ��C
//     ���涥�q����:2.0.50727.832
//
//     ��o���ɮשҰ����ܧ�i��|�y�����~���欰�A�ӥB�p�G���s���͵{���X�A
//     �ܧ�N�|�򥢡C
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
    /// EntityPay_Return
    /// </summary>
    [Serializable()]
    [AttributeTable("Pay_Return")]
    public class EntityPay_Return : Entity
    {
        
        private string _serialNo;
        
        /// <summary>
        /// serialNo
        /// </summary>
        public static string M_serialNo = "serialNo";
        
        private string _userID;
        
        /// <summary>
        /// userID
        /// </summary>
        public static string M_userID = "userID";
        
        private string _returnDay;
        
        /// <summary>
        /// returnDay
        /// </summary>
        public static string M_returnDay = "returnDay";
        
        private string _returnReason;
        
        /// <summary>
        /// returnReason
        /// </summary>
        public static string M_returnReason = "returnReason";
        
        /// <summary>
        /// serialNo
        /// </summary>
        [AttributeField("serialNo", "System.String", false, false, false, "String")]
        public string serialNo
        {
            get
            {
                return this._serialNo;
            }
            set
            {
                this._serialNo = value;
            }
        }
        
        /// <summary>
        /// userID
        /// </summary>
        [AttributeField("userID", "System.String", false, false, false, "String")]
        public string userID
        {
            get
            {
                return this._userID;
            }
            set
            {
                this._userID = value;
            }
        }
        
        /// <summary>
        /// returnDay
        /// </summary>
        [AttributeField("returnDay", "System.String", false, false, false, "String")]
        public string returnDay
        {
            get
            {
                return this._returnDay;
            }
            set
            {
                this._returnDay = value;
            }
        }
        
        /// <summary>
        /// returnReason
        /// </summary>
        [AttributeField("returnReason", "System.String", false, false, false, "String")]
        public string returnReason
        {
            get
            {
                return this._returnReason;
            }
            set
            {
                this._returnReason = value;
            }
        }
    }
    
    /// <summary>
    /// EntityPay_Return
    /// </summary>
    [Serializable()]
    public class EntityPay_ReturnSet : EntitySet<EntityPay_Return>
    {
    }
}