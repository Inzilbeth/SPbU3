namespace Task1.Models
{
    /// <summary>
    /// ViewModel for an Error.
    /// </summary>
    public class ErrorViewModel
    {
        /// <summary>
        /// Request's Id.
        /// </summary>
        public string RequestId { get; set; }

        /// <summary>
        /// Shows a RequestId.
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
