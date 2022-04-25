//------------------------------------------------------------------------------
// <auto-generated>
//     �o�q�{���X�O�Ѥu�㲣�ͪ��C
//     ���涥�q����:2.0.50727.3603
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


namespace EntityLayer
{


    /// <summary>
    /// L_BATCH_LOG
    /// </summary>
    [Serializable()]
    [AttributeTable("L_BATCH_LOG")]
    public class EntityM_LBatchLog : Framework.Data.OM.Entity
    {

        private string _FUNCTION_KEY;

        /// <summary>
        /// FUNCTION_KEY
        /// </summary>
        public static string M_FUNCTION_KEY = "FUNCTION_KEY";

        private string _JOB_ID;

        /// <summary>
        /// JOB_ID
        /// </summary>
        public static string M_JOB_ID = "JOB_ID";

        private object _START_TIME;

        /// <summary>
        /// START_TIME
        /// </summary>
        public static string M_START_TIME = "START_TIME";

        private object _END_TIME;

        /// <summary>
        /// END_TIME
        /// </summary>
        public static string M_END_TIME = "END_TIME";

        private string _STATUS;

        /// <summary>
        /// STATUS
        /// </summary>
        public static string M_STATUS = "STATUS";

        private string _RETURN_MESSAGE;

        /// <summary>
        /// RETURN_MESSAGE
        /// </summary>
        public static string M_RETURN_MESSAGE = "RETURN_MESSAGE";

        /// <summary>
        /// FUNCTION_KEY
        /// </summary>
        [AttributeField("FUNCTION_KEY", "System.String", false, true, false, "String")]
        public string FUNCTION_KEY
        {
            get
            {
                return this._FUNCTION_KEY;
            }
            set
            {
                this._FUNCTION_KEY = value;
            }
        }

        /// <summary>
        /// JOB_ID
        /// </summary>
        [AttributeField("JOB_ID", "System.String", false, true, false, "String")]
        public string JOB_ID
        {
            get
            {
                return this._JOB_ID;
            }
            set
            {
                this._JOB_ID = value;
            }
        }

        /// <summary>
        /// START_TIME
        /// </summary>
        [AttributeField("START_TIME", "System.DateTime", false, false, false, "DateTime")]
        public object START_TIME
        {
            get
            {
                return this._START_TIME;
            }
            set
            {
                this._START_TIME = value;
            }
        }

        /// <summary>
        /// END_TIME
        /// </summary>
        [AttributeField("END_TIME", "System.DateTime", false, false, false, "DateTime")]
        public object END_TIME
        {
            get
            {
                return this._END_TIME;
            }
            set
            {
                this._END_TIME = value;
            }
        }

        /// <summary>
        /// STATUS
        /// </summary>
        [AttributeField("STATUS", "System.String", false, false, false, "String")]
        public string STATUS
        {
            get
            {
                return this._STATUS;
            }
            set
            {
                this._STATUS = value;
            }
        }

        /// <summary>
        /// RETURN_MESSAGE
        /// </summary>
        [AttributeField("RETURN_MESSAGE", "System.String", false, false, false, "String")]
        public string RETURN_MESSAGE
        {
            get
            {
                return this._RETURN_MESSAGE;
            }
            set
            {
                this._RETURN_MESSAGE = value;
            }
        }
    }

    /// <summary>
    /// L_BATCH_LOG
    /// </summary>
    [Serializable()]
    public class EntityM_LBatchLogSet : EntitySet<EntityM_LBatchLog>
    {
    }
}


