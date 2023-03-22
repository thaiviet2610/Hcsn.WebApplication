using Hcsn.WebApplication.API.Controllers;
using Hcsn.WebApplication.API.Database.DepartmentRepository;
using Hcsn.WebApplication.Common.Entities;
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
    internal class DepartmentsControllerTests
    {
        /// <summary>
        /// Test hàm GetDepartmentById với đầu vào là department có tồn tại và kết quả mong muốn đầu ra là thành công
        /// </summary>
        /// Created by: LTVIET (18/03/2023)
        [Test]
        public void GetDepartmentById_ExistsDepartment_ReturnsSuccess()
        {
            // Arrange
            var departmentId = new Guid("b1e61ef6-274b-4477-9d6d-85af26373d97");
            var department = new Department
            {
                department_id = departmentId,
                department_code = "HCTH",
                department_name = "Phòng Hanh chính Tổng hợp",
            };
            var expectedResult = new ObjectResult(department);
            expectedResult.StatusCode = 200;
            var fakeDepartmentRepository = Substitute.For<IDepartmentRespository>();

            fakeDepartmentRepository.QueryFirstOrDefault(
                Arg.Any<IDbConnection>(),
                Arg.Any<string>(),
                Arg.Any<object>(),
                Arg.Any<IDbTransaction>(),
                Arg.Any<int?>(),
                Arg.Any<CommandType?>()
                ).Returns(department);

            var departmentsController = new DepartmentsController(fakeDepartmentRepository);

            // Act
            var actualResult =(ObjectResult) departmentsController.GetDepartmentById(departmentId);

            // Assert
            Assert.AreEqual(expectedResult.StatusCode, actualResult.StatusCode);
            Assert.AreEqual(((Department)expectedResult.Value).department_id, ((Department)actualResult.Value).department_id);
            Assert.AreEqual(((Department)expectedResult.Value).department_code, ((Department)actualResult.Value).department_code);
            Assert.AreEqual(((Department)expectedResult.Value).department_name, ((Department)actualResult.Value).department_name);
        }

        /// <summary>
        /// Test hàm GetDepartmentById với đầu vào là department không tồn tại và kết quả mong muốn đầu ra là không tìm thấy
        /// </summary>
        /// Created by: LTVIET (18/03/2023)
        [Test]
        public void GetDepartmentById_NotExistsDepartment_ReturnsNotFound()
        {
            // Arrange
            var departmentId = new Guid("b1e61ef6-274b-4477-9d6d-85af26373d97");
            var expectedResult = new NotFoundResult();

            var fakeDepartmentRepository = Substitute.For<IDepartmentRespository>();

            fakeDepartmentRepository.QueryFirstOrDefault(
                Arg.Any<IDbConnection>(),
                Arg.Any<string>(),
                Arg.Any<object>(),
                Arg.Any<IDbTransaction>(),
                Arg.Any<int?>(),
                Arg.Any<CommandType?>()
                ).Returns((Department)null);

            var departmentsController = new DepartmentsController(fakeDepartmentRepository);


            // Act
            var actualResult = (NotFoundResult)departmentsController.GetDepartmentById(departmentId);

            // Assert
            Assert.AreEqual(expectedResult.StatusCode, actualResult.StatusCode);
        }
    }
}
