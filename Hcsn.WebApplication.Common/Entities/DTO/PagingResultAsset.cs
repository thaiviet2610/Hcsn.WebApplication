using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hcsn.WebApplication.Common.Entities.DTO
{
    public class PagingResultAsset : PagingResult<FixedAssetDTO>
    {
        /// <summary>
        /// Tổng số lượng của các bản ghi thỏa mãn điều kiện
        /// </summary>
        public int QuantityTotal { get; set; }

        /// <summary>
        /// Tổng nguyên giá của các bản ghi thỏa mãn điều kiện
        /// </summary>
        public decimal CostTotal { get; set; }

        /// <summary>
        /// Tổng khấu hao lũy kế của các bản ghi thỏa mãn điều kiện
        /// </summary>
        public decimal DepreciationValueTotal { get; set; }

		/// <summary>
		/// Tổng giá trị còn lại của các bản ghi thỏa mãn điều kiện
		/// </summary>
		public decimal ResidualValueTotal { get; set; }
	}
}
