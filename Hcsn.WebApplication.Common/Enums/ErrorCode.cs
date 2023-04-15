namespace Hcsn.WebApplication.Common.Enums
{
    /// <summary>
    /// Thông tin các mã code lỗi
    /// </summary>
    public enum ErrorCode
    {
        /// <summary>
        /// Lỗi ngoại lệ
        /// </summary>
        Exception = 1,

        /// <summary>
        /// Lỗi dữ liệu không hợp lệ
        /// </summary>
        InvalidateData = 2,

        /// <summary>
        /// Lỗi thêm mới thất bại
        /// </summary>
        InsertFailed = 3,
        
        /// <summary>
        /// Lỗi cập nhật thất bại
        /// </summary>
        UpdateFailed = 4,

        /// <summary>
        /// Lỗi xóa thất bại
        /// </summary>
        DeleteFailed = 5,

        /// <summary>
        /// Lỗi không tìm thấy bản ghi nào phù hợp
        /// </summary>
        NotFound = 6,

        /// <summary>
        /// Lỗi sinh ra code mới thất bại
        /// </summary>
        GenerateNewCodefailed = 7,

		/// <summary>
		/// Lỗi xóa nhiều bản ghi thất bại
		/// </summary>
		DeleteMultipleFailed = 8,

        /// <summary>
        /// Lỗi xuất dữ liệu ra file excel không thành công
        /// </summary>
        ExportExcelFailed = 9
    }
}