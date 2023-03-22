namespace Hcsn.WebApplication.Common.Entities.DTO
{
    public class PagingResult
    {
        /// <summary>
        /// Danh sách tài sản trong database
        /// </summary>
        public List<Asset> Data { get; set; }

        /// <summary>
        /// Tổng số bản ghi thỏa mãn điều kiện
        /// </summary>
        public int TotalRecord { get; set; }
    }
}
