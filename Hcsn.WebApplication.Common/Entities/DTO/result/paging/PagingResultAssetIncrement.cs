using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hcsn.WebApplication.Common.Entities.DTO.entityDTO;

namespace Hcsn.WebApplication.Common.Entities.DTO.result.paging
{
    public class PagingResultAssetIncrement : PagingResult<FixedAssetIncrementDTO>
    {
        /// <summary>
        /// Tổng nguyên giá
        /// </summary>
        public decimal PriceTotal { get; set; }
    }
}
