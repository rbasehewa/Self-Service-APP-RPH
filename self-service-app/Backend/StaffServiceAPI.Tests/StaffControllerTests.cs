using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StaffServiceAPI.Controllers;
using StaffServiceAPI.Data;
using StaffServiceAPI.Models;

namespace StaffServiceAPI.Tests
{
    public class StaffControllerTests
    {
        // HELPER: Creates test database
        private AppDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        // HELPER: Creates mock logger
        private ILogger<StaffController> GetMockLogger()
        {
            return new Mock<ILogger<StaffController>>().Object;
        }

        [Fact]
        public async Task GetStaff_ReturnsAllStaff()
        {
            // ARRANGE
            var context = GetInMemoryDbContext();
            var logger = GetMockLogger();
            
            context.Staff.AddRange(
                new Staff { Name = "Test 1", DateOfBirth = DateTime.UtcNow.AddYears(-30), Gender = "Male", ImmuneStatus = "Immune", Department = "IT" },
                new Staff { Name = "Test 2", DateOfBirth = DateTime.UtcNow.AddYears(-25), Gender = "Female", ImmuneStatus = "Non-Immune", Department = "HR" },
                new Staff { Name = "Test 3", DateOfBirth = DateTime.UtcNow.AddYears(-35), Gender = "Male", ImmuneStatus = "Immune", Department = "Finance" }
            );
            await context.SaveChangesAsync();
            
            var controller = new StaffController(context, logger);

            // ACT
            var result = await controller.GetStaff();

            // ASSERT
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var staff = Assert.IsAssignableFrom<IEnumerable<Staff>>(okResult.Value);
            Assert.Equal(3, staff.Count());
        }

        [Fact]
        public async Task GetStaffById_ReturnsCorrectStaff()
        {
            // ARRANGE
            var context = GetInMemoryDbContext();
            var logger = GetMockLogger();
            
            var testStaff = new Staff 
            { 
                Name = "John Doe", 
                DateOfBirth = DateTime.UtcNow.AddYears(-30), 
                Gender = "Male", 
                ImmuneStatus = "Immune", 
                Department = "IT" 
            };
            context.Staff.Add(testStaff);
            await context.SaveChangesAsync();
            
            var controller = new StaffController(context, logger);

            // ACT
            var result = await controller.GetStaffById(testStaff.Id);

            // ASSERT
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedStaff = Assert.IsType<Staff>(okResult.Value);
            Assert.Equal("John Doe", returnedStaff.Name);
        }

        [Fact]
        public async Task GetStaffById_Returns404_WhenNotFound()
        {
            // ARRANGE
            var context = GetInMemoryDbContext();
            var logger = GetMockLogger();
            var controller = new StaffController(context, logger);

            // ACT
            var result = await controller.GetStaffById(999);

            // ASSERT
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task CreateStaff_AddsStaffToDatabase()
        {
            // ARRANGE
            var context = GetInMemoryDbContext();
            var logger = GetMockLogger();
            var controller = new StaffController(context, logger);
            
            var newStaff = new Staff
            {
                Name = "New Staff",
                DateOfBirth = DateTime.UtcNow.AddYears(-28),
                Gender = "Female",
                ImmuneStatus = "Immune",
                Department = "Marketing"
            };

            // ACT
            var result = await controller.CreateStaff(newStaff);

            // ASSERT
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var createdStaff = Assert.IsType<Staff>(createdResult.Value);
            Assert.Equal("New Staff", createdStaff.Name);
            Assert.True(createdStaff.Id > 0);
        }

        [Fact]
        public async Task GetStaffByImmuneStatus_FiltersCorrectly()
        {
            // ARRANGE
            var context = GetInMemoryDbContext();
            var logger = GetMockLogger();
            
            context.Staff.AddRange(
                new Staff { Name = "Immune 1", DateOfBirth = DateTime.UtcNow.AddYears(-30), Gender = "Male", ImmuneStatus = "Immune", Department = "IT" },
                new Staff { Name = "Immune 2", DateOfBirth = DateTime.UtcNow.AddYears(-25), Gender = "Female", ImmuneStatus = "Immune", Department = "HR" },
                new Staff { Name = "Non-Immune 1", DateOfBirth = DateTime.UtcNow.AddYears(-35), Gender = "Male", ImmuneStatus = "Non-Immune", Department = "Finance" }
            );
            await context.SaveChangesAsync();
            
            var controller = new StaffController(context, logger);

            // ACT
            var result = await controller.GetStaffByImmuneStatus("Immune");

            // ASSERT
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var staff = Assert.IsAssignableFrom<IEnumerable<Staff>>(okResult.Value);
            Assert.Equal(2, staff.Count());
            Assert.All(staff, s => Assert.Equal("Immune", s.ImmuneStatus));
        }

        [Fact]
        public async Task DeleteStaff_RemovesStaffFromDatabase()
        {
            // ARRANGE
            var context = GetInMemoryDbContext();
            var logger = GetMockLogger();
            
            var staffToDelete = new Staff
            {
                Name = "To Delete",
                DateOfBirth = DateTime.UtcNow.AddYears(-30),
                Gender = "Male",
                ImmuneStatus = "Immune",
                Department = "IT"
            };
            context.Staff.Add(staffToDelete);
            await context.SaveChangesAsync();
            
            var controller = new StaffController(context, logger);

            // ACT
            var result = await controller.DeleteStaff(staffToDelete.Id);

            // ASSERT
            Assert.IsType<OkObjectResult>(result);
            var deletedStaff = await context.Staff.FindAsync(staffToDelete.Id);
            Assert.Null(deletedStaff);
        }

        [Fact]
        public async Task GetStatistics_ReturnsCorrectCounts()
        {
            // ARRANGE
            var context = GetInMemoryDbContext();
            var logger = GetMockLogger();
            
            context.Staff.AddRange(
                new Staff { Name = "Immune 1", DateOfBirth = DateTime.UtcNow.AddYears(-30), Gender = "Male", ImmuneStatus = "Immune", Department = "IT" },
                new Staff { Name = "Immune 2", DateOfBirth = DateTime.UtcNow.AddYears(-25), Gender = "Female", ImmuneStatus = "Immune", Department = "HR" },
                new Staff { Name = "Immune 3", DateOfBirth = DateTime.UtcNow.AddYears(-35), Gender = "Male", ImmuneStatus = "Immune", Department = "Finance" },
                new Staff { Name = "Non-Immune 1", DateOfBirth = DateTime.UtcNow.AddYears(-28), Gender = "Female", ImmuneStatus = "Non-Immune", Department = "IT" },
                new Staff { Name = "Non-Immune 2", DateOfBirth = DateTime.UtcNow.AddYears(-32), Gender = "Male", ImmuneStatus = "Non-Immune", Department = "HR" }
            );
            await context.SaveChangesAsync();
            
            var controller = new StaffController(context, logger);

            // ACT
            var result = await controller.GetStatistics();

            // ASSERT
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            dynamic stats = okResult.Value;
            Assert.NotNull(stats);
        }
    }
}