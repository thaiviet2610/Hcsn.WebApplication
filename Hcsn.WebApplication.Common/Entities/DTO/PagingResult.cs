namespace Hcsn.WebApplication.Common.Entities.DTO
{
    /// <summary>
    /// Đối tượng chứa kết quả ccuar việc phân trang, lọc danh sách đối tượng
    /// Created by: LTVIET (10/03/2023)
    /// </summary>
    public class PagingResult<T>
    {
        /// <summary>
        /// Danh sách tài sản trong database
        /// </summary>
        public List<T> Data { get; set; }

        /// <summary>
        /// Tổng số bản ghi thỏa mãn điều kiện
        /// </summary>
        public int TotalRecord { get; set; }
	}
}
