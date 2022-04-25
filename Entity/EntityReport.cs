//------------------------------------------------------------------------------
// ����Report����
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
    /// CPMAST
    /// </summary>
    [Serializable()]
    [AttributeTable("CPMAST")]
    public class EntityReport : Entity
    {

        private string _TYPE;

        /// <summary>
        /// TYPE
        /// </summary>
        public static string M_TYPE = "TYPE";

        private string _CUST_ID;

        /// <summary>
        /// CUST_ID
        /// </summary>
        public static string M_CUST_ID = "CUST_ID";

        private string _CARD_TYPE;

        /// <summary>
        /// CARD_TYPE
        /// </summary>
        public static string M_CARD_TYPE = "CARD_TYPE";

        private string _FLD_NAME;

        /// <summary>
        /// FLD_NAME
        /// </summary>
        public static string M_FLD_NAME = "FLD_NAME";

        private string _BEFOR_UPD;

        /// <summary>
        /// BEFOR_UPD
        /// </summary>
        public static string M_BEFOR_UPD = "BEFOR_UPD";

        private string _AFTER_UPD;

        /// <summary>
        /// AFTER_UPD
        /// </summary>
        public static string M_AFTER_UPD = "AFTER_UPD";

        private int _LST_LIMIT;

        /// <summary>
        /// LST_LIMIT
        /// </summary>
        public static string M_LST_LIMIT = "LST_LIMIT";

        private int _CUR_LIMIT;

        /// <summary>
        /// CUR_LIMIT
        /// </summary>
        public static string M_CUR_LIMIT = "CUR_LIMIT";

        private string _MAINT_D;

        /// <summary>
        /// MAINT_D
        /// </summary>
        public static string M_MAINT_D = "MAINT_D";

        private string _MAINT_T;

        /// <summary>
        /// MAINT_T
        /// </summary>
        public static string M_MAINT_T = "MAINT_T";

        private string _USER_ID;

        /// <summary>
        /// USER_ID
        /// </summary>
        public static string M_USER_ID = "USER_ID";

        private string _TER_ID;

        /// <summary>
        /// TER_ID
        /// </summary>
        public static string M_TER_ID = "TER_ID";

        private string _EXE_Name;

        /// <summary>
        /// EXE_Name
        /// </summary>
        public static string M_EXE_Name = "EXE_Name";

        /// <summary>
        /// TYPE
        /// </summary>
        [AttributeField("TYPE", "System.String", false, false, false, "String")]
        public string TYPE
        {
            get
            {
                return this._TYPE;
            }
            set
            {
                this._TYPE = value;
            }
        }

        /// <summary>
        /// CUST_ID
        /// </summary>
        [AttributeField("CUST_ID", "System.String", false, false, false, "String")]
        public string CUST_ID
        {
            get
            {
                return this._CUST_ID;
            }
            set
            {
                this._CUST_ID = value;
            }
        }

        /// <summary>
        /// CARD_TYPE
        /// </summary>
        [AttributeField("CARD_TYPE", "System.String", false, false, false, "String")]
        public string CARD_TYPE
        {
            get
            {
                return this._CARD_TYPE;
            }
            set
            {
                this._CARD_TYPE = value;
            }
        }

        /// <summary>
        /// FLD_NAME
        /// </summary>
        [AttributeField("FLD_NAME", "System.String", false, false, false, "String")]
        public string FLD_NAME
        {
            get
            {
                return this._FLD_NAME;
            }
            set
            {
                this._FLD_NAME = value;
            }
        }

        /// <summary>
        /// BEFOR_UPD
        /// </summary>
        [AttributeField("BEFOR_UPD", "System.String", false, false, false, "String")]
        public string BEFOR_UPD
        {
            get
            {
                return this._BEFOR_UPD;
            }
            set
            {
                this._BEFOR_UPD = value;
            }
        }

        /// <summary>
        /// AFTER_UPD
        /// </summary>
        [AttributeField("AFTER_UPD", "System.String", false, false, false, "String")]
        public string AFTER_UPD
        {
            get
            {
                return this._AFTER_UPD;
            }
            set
            {
                this._AFTER_UPD = value;
            }
        }

        /// <summary>
        /// LST_LIMIT
        /// </summary>
        [AttributeField("LST_LIMIT", "System.Int32", false, false, false, "Int32")]
        public int LST_LIMIT
        {
            get
            {
                return this._LST_LIMIT;
            }
            set
            {
                this._LST_LIMIT = value;
            }
        }

        /// <summary>
        /// CUR_LIMIT
        /// </summary>
        [AttributeField("CUR_LIMIT", "System.Int32", false, false, false, "Int32")]
        public int CUR_LIMIT
        {
            get
            {
                return this._CUR_LIMIT;
            }
            set
            {
                this._CUR_LIMIT = value;
            }
        }

        /// <summary>
        /// MAINT_D
        /// </summary>
        [AttributeField("MAINT_D", "System.String", false, false, false, "String")]
        public string MAINT_D
        {
            get
            {
                return this._MAINT_D;
            }
            set
            {
                this._MAINT_D = value;
            }
        }

        /// <summary>
        /// MAINT_T
        /// </summary>
        [AttributeField("MAINT_T", "System.String", false, false, false, "String")]
        public string MAINT_T
        {
            get
            {
                return this._MAINT_T;
            }
            set
            {
                this._MAINT_T = value;
            }
        }

        /// <summary>
        /// USER_ID
        /// </summary>
        [AttributeField("USER_ID", "System.String", false, false, false, "String")]
        public string USER_ID
        {
            get
            {
                return this._USER_ID;
            }
            set
            {
                this._USER_ID = value;
            }
        }

        /// <summary>
        /// TER_ID
        /// </summary>
        [AttributeField("TER_ID", "System.String", false, false, false, "String")]
        public string TER_ID
        {
            get
            {
                return this._TER_ID;
            }
            set
            {
                this._TER_ID = value;
            }
        }

        /// <summary>
        /// EXE_Name
        /// </summary>
        [AttributeField("EXE_Name", "System.String", false, false, false, "String")]
        public string EXE_Name
        {
            get
            {
                return this._EXE_Name;
            }
            set
            {
                this._EXE_Name = value;
            }
        }
    }

    /// <summary>
    /// CPMAST
    /// </summary>
    [Serializable()]
    public class EntityReportSet : EntitySet<EntityReport>
    {
    }
}
