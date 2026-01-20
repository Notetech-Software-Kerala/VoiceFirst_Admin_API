using System.Threading; // Provides CancellationToken for cooperative cancellation
using System.Threading.Tasks; // Provides Task and async/await support
using AutoMapper; // AutoMapper abstractions for mapping DTOs <-> entities
using FluentAssertions; // Fluent assertion extensions for readable test assertions
using Microsoft.AspNetCore.Mvc; // ASP.NET Core MVC result types like OkObjectResult, BadRequestObjectResult
using Moq; // Mocking framework used to create test doubles for interfaces
using VoiceFirst_Admin.API.Controllers; // Controller under test
using VoiceFirst_Admin.Business.Services; // Service implementation under test
using VoiceFirst_Admin.Data.Contracts.IRepositories; // Repository interface dependency for the service
using VoiceFirst_Admin.Utilities.Constants; // Application-wide constant messages
using VoiceFirst_Admin.Utilities.DTOs.Features.SysBusinessActivity; // DTOs for SysBusinessActivity feature
using VoiceFirst_Admin.Utilities.Exceptions; // Business exceptions used by the service
using VoiceFirst_Admin.Utilities.Models.Entities; // Entity model for SysBusinessActivity
using Xunit; // xUnit attributes and test runner support

namespace VoiceFirst_Admin.Unit_Test // Test project namespace
{
    // Focused tests only for Create behavior (controller + service)
    public class SysBusinessActivity_CreateTests // Test class grouping Create-related tests
    {
        private readonly Mock<ISysBusinessActivityRepo> _repoMock; // Mock of repository used by the service
        private readonly Mock<IMapper> _mapperMock; // Mock of AutoMapper for entity/DTO mapping
        private readonly SysBusinessActivityService _service; // Service under test (uses mocks)
        private readonly SysBusinessActivityController _controller; // Controller under test (uses real service)
        private const int TestUserId = 1; // Simulated logged-in user id used in service calls

        public SysBusinessActivity_CreateTests() // Test fixture constructor to initialize mocks and SUTs
        {
            _repoMock = new Mock<ISysBusinessActivityRepo>(); // Create repository mock
            _mapperMock = new Mock<IMapper>(); // Create mapper mock
            _service = new SysBusinessActivityService(_repoMock.Object, _mapperMock.Object); // Wire service with mocks
            _controller = new SysBusinessActivityController(_service); // Wire controller with the service
        }

        [Fact] // Marks a test method
        public async Task Controller_CreateAsync_ShouldReturnBadRequest_WhenModelIsNull() // Ensures null payload returns 400
        {
            // Act: invoke controller with null model and a default cancellation token
            var result = await _controller.CreateAsync(null!, CancellationToken.None);

            // Assert: result is 400 BadRequest with an error payload
            result.Should().BeOfType<BadRequestObjectResult>(); // Expect BadRequest
            var bad = result as BadRequestObjectResult; // Cast to access payload
            bad!.Value.Should().NotBeNull(); // Ensure error body exists
        }

        [Fact] // Marks a test method
        public async Task Controller_CreateAsync_ShouldReturnOk_WhenModelValid() // Ensures valid payload returns 200 with data
        {
            // Arrange: create input DTO, expected entity, and expected returned DTO
            var dto = new SysBusinessActivityCreateDTO { Name = "Biz_A" }; // Client-provided payload
            var entity = new SysBusinessActivity { SysBusinessActivityId = 10, BusinessActivityName = "Biz_A" }; // Entity state after create
            var resultDto = new SysBusinessActivityDTO { Id = 10, Name = "Biz_A" }; // DTO returned to client

            _repoMock.Setup(r => r.BusinessActivityExistsAsync("Biz_A", null, It.IsAny<CancellationToken>())) // Duplicate check returns false
                .ReturnsAsync(false);
            _mapperMock.Setup(m => m.Map<SysBusinessActivity>(dto)).Returns(entity); // Map CreateDTO -> Entity
            _repoMock.Setup(r => r.CreateAsync(entity, It.IsAny<CancellationToken>())) // Repository returns new id
                .ReturnsAsync(10);
            _mapperMock.Setup(m => m.Map<SysBusinessActivityDTO>(It.Is<SysBusinessActivity>(e => e.SysBusinessActivityId == 10))) // Map Entity -> DTO
                .Returns(resultDto);

            // Act: call controller endpoint with a valid payload
            var result = await _controller.CreateAsync(dto, CancellationToken.None);

            // Assert: result is 200 OK with a response body
            result.Should().BeOfType<OkObjectResult>(); // Expect Ok
            var ok = result as OkObjectResult; // Cast to OkObjectResult
            ok!.Value.Should().NotBeNull(); // Ensure response body is present
        }

        [Fact] // Marks a test method
        public async Task Service_CreateAsync_ShouldThrowConflict_WhenNameExists() // Ensures duplicate name throws conflict
        {
            // Arrange: repository signals the name already exists
            var dto = new SysBusinessActivityCreateDTO { Name = "Dup" }; // Duplicate name
            _repoMock.Setup(r => r.BusinessActivityExistsAsync("Dup", null, It.IsAny<CancellationToken>())) // Duplicate check returns true
                .ReturnsAsync(true);

            // Act: run service CreateAsync in a lambda for assertion
            var act = async () => await _service.CreateAsync(dto, TestUserId, CancellationToken.None);

            // Assert: a BusinessConflictException is thrown
            await act.Should().ThrowAsync<BusinessConflictException>();
        }

        [Fact] // Marks a test method
        public async Task Service_CreateAsync_ShouldReturnDto_OnSuccess() // Ensures successful creation returns mapped DTO
        {
            // Arrange: valid input and expected repository + mapper interactions
            var dto = new SysBusinessActivityCreateDTO { Name = "CreateMe" }; // Input DTO from client
            var entity = new SysBusinessActivity { SysBusinessActivityId = 0, BusinessActivityName = "CreateMe" }; // Entity before repo assigns id

            var resultDto = new SysBusinessActivityDTO { Id = 7, Name = "CreateMe" }; // Expected returned DTO

            _repoMock.Setup(r => r.BusinessActivityExistsAsync("CreateMe", null, It.IsAny<CancellationToken>())) // No duplicate found
                .ReturnsAsync(false);
            _mapperMock.Setup(m => m.Map<SysBusinessActivity>(dto)).Returns(entity); // Map CreateDTO -> Entity
            _repoMock.Setup(r => r.CreateAsync(entity, It.IsAny<CancellationToken>())) // Repo returns the new id
                .ReturnsAsync(7);
            _mapperMock.Setup(m => m.Map<SysBusinessActivityDTO>(It.Is<SysBusinessActivity>(e => e.SysBusinessActivityId == 7))) // Map Entity -> DTO with assigned id
                .Returns(resultDto);

            // Act: call service CreateAsync directly
            var result = await _service.CreateAsync(dto, TestUserId, CancellationToken.None);

            // Assert: returned DTO has expected id and name
            result.Should().NotBeNull(); // Ensure non-null result
            result.Id.Should().Be(7); // Id from repository
            result.Name.Should().Be("CreateMe"); // Name matches input
        }
    }
}
