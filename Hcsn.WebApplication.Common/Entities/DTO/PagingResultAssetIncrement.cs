using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hcsn.WebApplication.Common.Entities.DTO
{
	public class PagingResultAssetIncrement : PagingResult<FixedAssetIncrementDTO>
	{
        /// <summary>
        /// Tổng nguyên giá
        /// </summary>
        public decimal PriceTotal { get; set; }
    }
}
