﻿using Hcsn.WebApplication.Common.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hcsn.WebApplication.Common.Entities.DTO
{
	public class AssetDTO
	{
		/// <summary>
		/// Khóa chính
		/// </summary>
		[HcsnPrimaryKey]
		[HcsnRequired]
		[HcsnName("Id tài sản")]
		public Guid fixed_asset_id { get; set; }

		/// <summary>
		/// Mã tài sản
		/// </summary>
		[HcsnRequired]
		[HcsnName("Mã tài sản")]
		[HcsnCode]
		[HcsnMaxLength(10)]
		public string fixed_asset_code { get; set; }

		/// <summary>
		/// Tên tài sản
		/// </summary>
		[HcsnRequired]
		[HcsnName("Tên tài sản")]
		[HcsnMaxLength(100)]
		public string fixed_asset_name { get; set; }

		/// <summary>
		/// Tên bộ phận sử dụng
		/// </summary>
		[HcsnRequired]
		[HcsnName("Tên bộ phận sử dụng")]
		public string department_name { get; set; }

		/// <summary>
		/// Tên loại tài sản
		/// </summary>
		[HcsnRequired]
		[HcsnName("Tên loại tài sản")]
		public string fixed_asset_category_name { get; set; }

		/// <summary>
		/// Nguyên giá
		/// </summary>
		[HcsnRequired]
		[HcsnNumber("decimal")]
		[HcsnGreateThanZero]
		[HcsnName("Nguyên giá")]
		public decimal cost { get; set; }

		/// <summary>
		/// Số lượng
		/// </summary>
		[HcsnRequired]
		[HcsnNumber("int")]
		[HcsnGreateThanZero]
		[HcsnName("Số lượng")]
		public int quantity { get; set; }

		/// <summary>
		/// Hao mòn lũy kế
		/// </summary>
		[HcsnNumber("decimal")]
		[HcsnName("Hao mòn lũy kế")]
		public decimal depreciation_value { get; set; }

		/// <summary>
		/// Giá trị còn lại
		/// </summary>
		public double residual_value { get; set; }

		/// <summary>
		/// Tống giá trị của số lượng
		/// </summary>
		public int quantity_total { get; set; }

		/// <summary>
		/// Tổng giá trị của nguyên giá
		/// </summary>
		public decimal cost_total { get; set; }

		/// <summary>
		/// Tổng giá trị hao mòn lũy kế
		/// </summary>
		public double depreciation_value_total { get; set; }

		/// <summary>
		/// Tổng giá trị còn lại
		/// </summary>
		public double residual_value_total { get; set; }
	}
}