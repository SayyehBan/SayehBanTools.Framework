namespace SayehBanTools.Framework.Model.Entities
{
    /// <summary>
    /// Model لینک API
    /// </summary>
    public class API_Link_Model
    {
        /// <summary>
        /// دریافت لینک API
        /// </summary>
        public class API_Link_Get
        {
            /// <summary>
            /// شناسه لینک API
            /// </summary>
            public int API_Link_ID { get; set; }
            /// <summary>
            /// لینک API
            /// </summary>
            public string API_Link { get; set; } = string.Empty;
            /// <summary>
            /// نام لینک API
            /// </summary>
            public string API_Name_Link { get; set; } = string.Empty;
        }
    }
}
