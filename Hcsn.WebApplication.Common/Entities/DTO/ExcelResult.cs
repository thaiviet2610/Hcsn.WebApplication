using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hcsn.WebApplication.Common.Entities.DTO
{
	public class ExcelResult
	{
        /// <summary>
        /// Tên file excel
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Dữ liệu file excel
        /// </summary>
        public Object Data { get; set; }
    }
}
