using Hcsn.WebApplication.API.Controllers;
using Hcsn.WebApplication.Common;
using Hcsn.WebApplication.API.Database.AssetRepository;
using Hcsn.WebApplication.Common.Entities;
using Hcsn.WebApplication.Common.Entities.DTO;
using Hcsn.WebApplication.Common.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hcsn.WebApplication.API.UnitTests
{
    internal class AssetsControllerTests
    {
        [Test]
        public void InsertAsset_DuplicateCode_ReturnsDuplicateError()
        {
            // Arrange
            var assetId = new Guid("b1e61ef6-274b-4477-9d6d-85af26373d97");
            var asset = new FixedAsset
            {
                fixed_asset_id = assetId,
                fixed_asset_code = "TS00008",
                fixed_asset_name = "Iphone 11",
                department_id = Guid.NewGuid(),
                department_code = "TK",
                department_name = "Thư ký",
                fixed_asset_category_id = Guid.NewGuid(),
                fixed_asset_category_code = "MI",
                fixed_asset_category_name = "Máy in",
                cost = 2343434343,
                quantity = 1,
                depreciation_rate = 1.34F,
                purchase_date = DateTime.Now,
                tracked_year = 2023,
                production_year = DateTime.Now,
                life_time = 2

            };

            var assetSearch = new List<FixedAsset>()
            {
                new FixedAsset
                {
                    fixed_asset_id = Guid.NewGuid(),
                    fixed_asset_code = "TS00005",
                    fixed_asset_name = "Iphone 5",
                    department_id = Guid.NewGuid(),
                    department_code = "TK",
                    department_name = "Thư ký",
                    fixed_asset_category_id = Guid.NewGuid(),
                    fixed_asset_category_code = "MI",
                    fixed_asset_category_name = "Máy in",
                    cost = 2343434343,
                    quantity = 1,
                    depreciation_rate = 1.34F,
                    purchase_date = DateTime.Now,
                    tracked_year = 2023,
                    production_year = DateTime.Now,
                    life_time = 2
                }
            };
            var errorResult = new ErrorResult
            {
                ErrorCode = ErrorCode.DuplicateCode,
                DevMsg = Resource.DevMsg_DuplicateCode,
                UserMsg = Resource.UserMsg_DuplicateCode,
                MoreInfo = "https://"
            };
            var expectedResult = new ObjectResult(errorResult)
            {
                StatusCode = 400
            };
            var fakeAssetRepository = Substitute.For<IAssetRepository>();
            fakeAssetRepository.QueryMultiple(
                Arg.Any<IDbConnection>(),
                Arg.Any<string>(),
                Arg.Any<object>(),
                Arg.Any<IDbTransaction>(),
                Arg.Any<int?>(),
                Arg.Any<CommandType?>()
                ).Returns(assetSearch);
            var assetsController = new AssetsController(fakeAssetRepository);
            // Act
            var actualResult = (ObjectResult)assetsController.InsertAsset(asset);

            // Asserts
            Assert.AreEqual(expectedResult.StatusCode, actualResult.StatusCode);
            Assert.AreEqual(((ErrorResult)expectedResult.Value).ErrorCode, ((ErrorResult)actualResult.Value).ErrorCode);
            Assert.AreEqual(((ErrorResult)expectedResult.Value).DevMsg, ((ErrorResult)actualResult.Value).DevMsg);
            Assert.AreEqual(((ErrorResult)expectedResult.Value).UserMsg, ((ErrorResult)actualResult.Value).UserMsg);
            Assert.AreEqual(((ErrorResult)expectedResult.Value).MoreInfo, ((ErrorResult)actualResult.Value).MoreInfo);
        }

        [Test]
        public void InsertAsset_ValidInput_ReturnSuccess()
        {
            // Arrange
            var asset = new FixedAsset
            {
                fixed_asset_id = Guid.NewGuid(),
                fixed_asset_code = "TS00012",
                fixed_asset_name = "Iphone 11",
                department_id = Guid.NewGuid(),
                department_code = "TK",
                department_name = "Thư ký",
                fixed_asset_category_id = Guid.NewGuid(),
                fixed_asset_category_code = "MI",
                fixed_asset_category_name = "Máy in",
                cost = 2343434343,
                quantity = 1,
                depreciation_rate = 1.34F,
                purchase_date = DateTime.Now,
                tracked_year = 2023,
                production_year = DateTime.Now,
                life_time = 2

            };

            var expectedResult = new StatusCodeResult(201);
            var assetSearch = new List<FixedAsset>();
            var fakeAssetRepository = Substitute.For<IAssetRepository>();
            fakeAssetRepository.QueryMultiple(
                Arg.Any<IDbConnection>(),
                Arg.Any<string>(),
                Arg.Any<object>(),
                Arg.Any<IDbTransaction>(),
                Arg.Any<int?>(),
                Arg.Any<CommandType?>()
                ).Returns(assetSearch);

            fakeAssetRepository.Execute(
                Arg.Any<IDbConnection>(),
                Arg.Any<string>(),
                Arg.Any<object>(),
                Arg.Any<IDbTransaction>(),
                Arg.Any<int?>(),
                Arg.Any<CommandType?>()
                ).Returns(1);

            var assetsController = new AssetsController(fakeAssetRepository);
            // Act
            var actualResult = (StatusCodeResult)assetsController.InsertAsset(asset);

            // Assert
            Assert.AreEqual(expectedResult.StatusCode, actualResult.StatusCode);

        }

        [Test]
        public void InsertAsset_InsertFail_ReturnException()
        {
            // Arrange
            var asset = new FixedAsset
            {
                fixed_asset_id = Guid.NewGuid(),
                fixed_asset_code = "TS00012",
                fixed_asset_name = "Iphone 11",
                department_id = Guid.NewGuid(),
                department_code = "TK",
                department_name = "Thư ký",
                fixed_asset_category_id = Guid.NewGuid(),
                fixed_asset_category_code = "MI",
                fixed_asset_category_name = "Máy in",
                cost = 2343434343,
                quantity = 1,
                depreciation_rate = 1.34F,
                purchase_date = DateTime.Now,
                tracked_year = 2023,
                production_year = DateTime.Now,
                life_time = 2

            };
            var errorResult = new ErrorResult
            {
                ErrorCode = ErrorCode.Exception,
                DevMsg = Resource.DevMsg_Exception,
                UserMsg = Resource.UserMsg_Exception,
                MoreInfo = "https://"
            };
            var expectedResult = new ObjectResult(errorResult);
            expectedResult.StatusCode = 500;
            var assetSearch = new List<FixedAsset>();
            var fakeAssetRepository = Substitute.For<IAssetRepository>();
            fakeAssetRepository.QueryMultiple(
                Arg.Any<IDbConnection>(),
                Arg.Any<string>(),
                Arg.Any<object>(),
                Arg.Any<IDbTransaction>(),
                Arg.Any<int?>(),
                Arg.Any<CommandType?>()
                ).Returns(assetSearch);

            fakeAssetRepository.Execute(
                Arg.Any<IDbConnection>(),
                Arg.Any<string>(),
                Arg.Any<object>(),
                Arg.Any<IDbTransaction>(),
                Arg.Any<int?>(),
                Arg.Any<CommandType?>()
                ).Returns(0);

            var assetsController = new AssetsController(fakeAssetRepository);
            // Act
            var actualResult = (ObjectResult)assetsController.InsertAsset(asset);

            // Assert
            Assert.AreEqual(expectedResult.StatusCode, actualResult.StatusCode);
            Assert.AreEqual(((ErrorResult)expectedResult.Value).ErrorCode, ((ErrorResult)actualResult.Value).ErrorCode);
            Assert.AreEqual(((ErrorResult)expectedResult.Value).DevMsg, ((ErrorResult)actualResult.Value).DevMsg);
            Assert.AreEqual(((ErrorResult)expectedResult.Value).UserMsg, ((ErrorResult)actualResult.Value).UserMsg);
            Assert.AreEqual(((ErrorResult)expectedResult.Value).MoreInfo, ((ErrorResult)actualResult.Value).MoreInfo);

        }
    }
}
