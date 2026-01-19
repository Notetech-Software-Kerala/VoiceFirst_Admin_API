
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using VoiceFirst_Admin.API.Controllers;
//using VoiceFirst_Admin.Data.Contracts.IRepositories;
//using VoiceFirst_Admin.Utilities.Constants;
//using VoiceFirst_Admin.Utilities.DTOs.Features.ProgramAction;
//using VoiceFirst_Admin.Utilities.DTOs.Shared;
//using VoiceFirst_Admin.Utilities.Models.Common;
//using VoiceFirst_Admin.Utilities.Models.Entities;

//namespace VoiceFirst_Admin.Unit_Test;

///// <summary>
///// Comprehensive automation tests for ProgramAction CRUD operations
///// </summary>
//public class ProgramActionAutomationTests
//{
//    private readonly Mock<IProgramActionRepo> _repoMock;
//    private readonly Mock<IMapper> _mapperMock;
//    private readonly ProgramActionService _service;
//    private readonly ProgramActionController _controller;
//    private const int TestUserId = 1;

//    public ProgramActionAutomationTests()
//    {
//        _repoMock = new Mock<IProgramActionRepo>();
//        _mapperMock = new Mock<IMapper>();
//        _service = new ProgramActionService(_mapperMock.Object, _repoMock.Object);
//        _controller = new ProgramActionController(_service);
//    }

//    #region End-to-End Create-Read-Update-Delete Flow Tests

//    [Fact]
//    public async Task FullCRUDFlow_ShouldCompleteSuccessfully()
//    {
//        // Arrange - Setup test data
//        var createDto = new ProgramActionCreateDto { ProgramActionName = "AutoTest_Action_001" };
//        var createdEntity = new SysProgramActions
//        {
//            SysProgramActionId = 1,
//            ProgramActionName = "AutoTest_Action_001",
//            CreatedBy = TestUserId,
//            IsActive = true,
//            IsDeleted = false
//        };
//        var programActionDto = new ProgramActionDto
//        {
//            ActionId = 1,
//            ActionName = "AutoTest_Action_001",
//            Active = true
//        };

//        // Step 1: CREATE
//        _repoMock.Setup(r => r.ExistsByNameAsync("AutoTest_Action_001", null, It.IsAny<CancellationToken>()))
//            .ReturnsAsync(false);
//        _repoMock.Setup(r => r.CreateAsync(It.IsAny<SysProgramActions>(), It.IsAny<CancellationToken>()))
//            .ReturnsAsync(createdEntity);
//        _repoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
//            .ReturnsAsync(createdEntity);
//        _mapperMock.Setup(m => m.Map<ProgramActionDto>(It.IsAny<SysProgramActions>()))
//            .Returns(programActionDto);

//        var createResult = await _controller.Create(createDto, CancellationToken.None);
//        createResult.Should().BeOfType<CreatedAtActionResult>();

//        // Step 2: READ (GetById)
//        var getByIdResult = await _controller.GetById(1, CancellationToken.None);
//        getByIdResult.Should().BeOfType<OkObjectResult>();
//        var getByIdResponse = (getByIdResult as OkObjectResult)!.Value as ApiResponse<ProgramActionDto>;
//        getByIdResponse!.Data!.ActionName.Should().Be("AutoTest_Action_001");

//        // Step 3: UPDATE
//        var updateDto = new ProgramActionUpdateDto
//        {
//            ActionId = 1,
//            ActionName = "AutoTest_Action_001_Updated",
//            Active = true
//        };
//        var updatedEntity = new SysProgramActions
//        {
//            SysProgramActionId = 1,
//            ProgramActionName = "AutoTest_Action_001_Updated",
//            IsActive = true
//        };
//        var updatedDto = new ProgramActionDto
//        {
//            ActionId = 1,
//            ActionName = "AutoTest_Action_001_Updated",
//            Active = true
//        };

//        _repoMock.Setup(r => r.ExistsByNameAsync("AutoTest_Action_001_Updated", 1, It.IsAny<CancellationToken>()))
//            .ReturnsAsync(false);
//        _repoMock.Setup(r => r.UpdateAsync(It.IsAny<SysProgramActions>(), It.IsAny<CancellationToken>()))
//            .ReturnsAsync(true);
//        _repoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
//            .ReturnsAsync(updatedEntity);
//        _mapperMock.Setup(m => m.Map<ProgramActionDto>(It.Is<SysProgramActions>(e => e.ProgramActionName == "AutoTest_Action_001_Updated")))
//            .Returns(updatedDto);

//        var updateResult = await _controller.Update(1, updateDto, CancellationToken.None);
//        updateResult.Should().BeOfType<CreatedAtActionResult>();

//        // Step 4: DELETE
//        var deleteDto = new CommonDeleteDto { Id = 1, Deleted = true };
//        var toDeleteEntity = new SysProgramActions { SysProgramActionId = 1, ProgramActionName = "AutoTest_Action_001_Updated" };
//        var toDeleteDtoForService = new ProgramActionDto { ActionId = 1, ActionName = "AutoTest_Action_001_Updated", Active = true };

//        _repoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
//            .ReturnsAsync(toDeleteEntity);
//        _mapperMock.Setup(m => m.Map<ProgramActionDto>(It.Is<SysProgramActions>(e => e.SysProgramActionId == 1)))
//            .Returns(toDeleteDtoForService);
//        _repoMock.Setup(r => r.DeleteAsync(It.IsAny<SysProgramActions>(), It.IsAny<CancellationToken>()))
//            .ReturnsAsync(true);

//        var deleteResult = await _controller.Delete(1, deleteDto, CancellationToken.None);
//        deleteResult.Should().BeOfType<OkObjectResult>();

//        // Verify all operations executed
//        _repoMock.Verify(r => r.CreateAsync(It.IsAny<SysProgramActions>(), It.IsAny<CancellationToken>()), Times.Once);
//        _repoMock.Verify(r => r.UpdateAsync(It.IsAny<SysProgramActions>(), It.IsAny<CancellationToken>()), Times.Once);
//        _repoMock.Verify(r => r.DeleteAsync(It.IsAny<SysProgramActions>(), It.IsAny<CancellationToken>()), Times.Once);
//    }

//    #endregion

//    #region Bulk Operations Tests

//    [Fact]
//    public async Task BulkCreate_MultipleActions_ShouldCreateAll()
//    {
//        // Arrange
//        var actions = new[]
//        {
//            new ProgramActionCreateDto { ProgramActionName = "Action_001" },
//            new ProgramActionCreateDto { ProgramActionName = "Action_002" },
//            new ProgramActionCreateDto { ProgramActionName = "Action_003" }
//        };

//        var createdIds = new List<int>();
//        for (int i = 0; i < actions.Length; i++)
//        {
//            var index = i;
//            var entity = new SysProgramActions { SysProgramActionId = i + 1, ProgramActionName = actions[i].ProgramActionName };
//            var dto = new ProgramActionDto { ActionId = i + 1, ActionName = actions[i].ProgramActionName, Active = true };

//            _repoMock.Setup(r => r.ExistsByNameAsync(actions[index].ProgramActionName, null, It.IsAny<CancellationToken>()))
//                .ReturnsAsync(false);
//            _repoMock.Setup(r => r.CreateAsync(It.Is<SysProgramActions>(e => e.ProgramActionName == actions[index].ProgramActionName), It.IsAny<CancellationToken>()))
//                .ReturnsAsync(entity);
//            _repoMock.Setup(r => r.GetByIdAsync(i + 1, It.IsAny<CancellationToken>()))
//                .ReturnsAsync(entity);
//            _mapperMock.Setup(m => m.Map<ProgramActionDto>(It.Is<SysProgramActions>(e => e.SysProgramActionId == i + 1)))
//                .Returns(dto);
//        }

//        // Act
//        foreach (var action in actions)
//        {
//            var result = await _controller.Create(action, CancellationToken.None);
//            result.Should().BeOfType<CreatedAtActionResult>();
//            var createdResult = result as CreatedAtActionResult;
//            var response = createdResult!.Value as ApiResponse<ProgramActionDto>;
//            createdIds.Add(response!.Data!.ActionId);
//        }

//        // Assert
//        createdIds.Should().HaveCount(3);
//        createdIds.Should().Contain(new[] { 1, 2, 3 });
//        _repoMock.Verify(r => r.CreateAsync(It.IsAny<SysProgramActions>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
//    }

//    [Fact]
//    public async Task GetAll_WithPagination_ShouldReturnPagedResults()
//    {
//        // Arrange
//        var filter = new CommonFilterDto { PageNumber = 1, Limit = 10 };
//        var entities = Enumerable.Range(1, 10).Select(i => new SysProgramActions
//        {
//            SysProgramActionId = i,
//            ProgramActionName = $"Action_{i:D3}",
//            IsActive = true
//        }).ToList();

//        var pagedResult = new PagedResultDto<SysProgramActions>
//        {
//            Items = entities,
//            TotalCount = 25,
//            PageNumber = 1,
//            PageSize = 10
//        };

//        var dtos = entities.Select(e => new ProgramActionDto
//        {
//            ActionId = e.SysProgramActionId,
//            ActionName = e.ProgramActionName,
//            Active = e.IsActive
//        }).ToList();

//        _repoMock.Setup(r => r.GetAllAsync(filter, It.IsAny<CancellationToken>()))
//            .ReturnsAsync(pagedResult);
//        _mapperMock.Setup(m => m.Map<IEnumerable<ProgramActionDto>>(It.IsAny<IEnumerable<SysProgramActions>>()))
//            .Returns(dtos);

//        // Act
//        var result = await _controller.GetAll(filter, CancellationToken.None);

//        // Assert
//        result.Should().BeOfType<OkObjectResult>();
//        var okResult = result as OkObjectResult;
//        var response = okResult!.Value as ApiResponse<object>;
//        response.Should().NotBeNull();
//        response!.Message.Should().Be(Messages.ProgramActionCreatedRetrieveSucessfully);
//    }

//    #endregion

//    #region Validation and Business Rule Tests

//    [Fact]
//    public async Task Create_WithDuplicateName_ShouldPreventCreation()
//    {
//        // Arrange
//        var dto = new ProgramActionCreateDto { ProgramActionName = "DuplicateAction" };
//        _repoMock.Setup(r => r.ExistsByNameAsync("DuplicateAction", null, It.IsAny<CancellationToken>()))
//            .ReturnsAsync(true);

//        // Act
//        var result = await _controller.Create(dto, CancellationToken.None);

//        // Assert
//        result.Should().BeOfType<ConflictObjectResult>();
//        var conflictResult = result as ConflictObjectResult;
//        var response = conflictResult!.Value as ApiResponse<object>;
//        response!.Message.Should().Be(Messages.NameAlreadyExists);
//        response.StatusCode.Should().Be(StatusCodes.Status409Conflict);

//        // Verify create was never called
//        _repoMock.Verify(r => r.CreateAsync(It.IsAny<SysProgramActions>(), It.IsAny<CancellationToken>()), Times.Never);
//    }

//    [Fact]
//    public async Task Update_WithNonExistentId_ShouldReturnNotFound()
//    {
//        // Arrange
//        var updateDto = new ProgramActionUpdateDto { ActionId = 999, ActionName = "NonExistent" };
//        _repoMock.Setup(r => r.ExistsByNameAsync("NonExistent", 999, It.IsAny<CancellationToken>()))
//            .ReturnsAsync(false);
//        _repoMock.Setup(r => r.UpdateAsync(It.IsAny<SysProgramActions>(), It.IsAny<CancellationToken>()))
//            .ReturnsAsync(false);

//        // Act
//        var result = await _controller.Update(999, updateDto, CancellationToken.None);

//        // Assert
//        result.Should().BeOfType<NotFoundObjectResult>();
//        var notFoundResult = result as NotFoundObjectResult;
//        var response = notFoundResult!.Value as ApiResponse<object>;
//        response!.StatusCode.Should().Be(StatusCodes.Status404NotFound);
//    }

//    [Theory]
//    [InlineData("")]
//    [InlineData("   ")]
//    public async Task Create_WithInvalidActionName_ShouldBeEmpty(string invalidName)
//    {
//        // Arrange
//        var dto = new ProgramActionCreateDto { ProgramActionName = invalidName };

//        // Act & Assert - validation check
//        dto.ProgramActionName.Should().BeNullOrWhiteSpace();
//    }

//    #endregion

//    #region Search and Filter Tests

//    [Fact]
//    public async Task GetAll_WithSearchText_ShouldFilterResults()
//    {
//        // Arrange
//        var searchText = "Admin";
//        var filter = new CommonFilterDto { SearchText = searchText, PageNumber = 1, Limit = 10 };

//        var matchingEntities = new List<SysProgramActions>
//        {
//            new SysProgramActions { SysProgramActionId = 1, ProgramActionName = "Admin_Action", IsActive = true },
//            new SysProgramActions { SysProgramActionId = 2, ProgramActionName = "AdminPanel", IsActive = true }
//        };

//        var pagedResult = new PagedResultDto<SysProgramActions>
//        {
//            Items = matchingEntities,
//            TotalCount = 2,
//            PageNumber = 1,
//            PageSize = 10
//        };

//        var dtos = matchingEntities.Select(e => new ProgramActionDto
//        {
//            ActionId = e.SysProgramActionId,
//            ActionName = e.ProgramActionName,
//            Active = e.IsActive
//        }).ToList();

//        _repoMock.Setup(r => r.GetAllAsync(It.Is<CommonFilterDto>(f => f.SearchText == searchText), It.IsAny<CancellationToken>()))
//            .ReturnsAsync(pagedResult);
//        _mapperMock.Setup(m => m.Map<IEnumerable<ProgramActionDto>>(It.IsAny<IEnumerable<SysProgramActions>>()))
//            .Returns(dtos);

//        // Act
//        var result = await _controller.GetAll(filter, CancellationToken.None);

//        // Assert
//        result.Should().BeOfType<OkObjectResult>();
//        _repoMock.Verify(r => r.GetAllAsync(It.Is<CommonFilterDto>(f => f.SearchText == searchText), It.IsAny<CancellationToken>()), Times.Once);
//    }

//    [Fact]
//    public async Task GetAll_WithIsActiveFilter_ShouldReturnOnlyActiveRecords()
//    {
//        // Arrange
//        var filter = new CommonFilterDto { Active = true, PageNumber = 1, Limit = 10 };
//        var activeEntities = new List<SysProgramActions>
//        {
//            new SysProgramActions { SysProgramActionId = 1, ProgramActionName = "Active1", IsActive = true },
//            new SysProgramActions { SysProgramActionId = 2, ProgramActionName = "Active2", IsActive = true }
//        };

//        var pagedResult = new PagedResultDto<SysProgramActions>
//        {
//            Items = activeEntities,
//            TotalCount = 2
//        };

//        var dtos = activeEntities.Select(e => new ProgramActionDto
//        {
//            ActionId = e.SysProgramActionId,
//            ActionName = e.ProgramActionName,
//            Active = e.IsActive
//        }).ToList();

//        _repoMock.Setup(r => r.GetAllAsync(It.Is<CommonFilterDto>(f => f.Active == true), It.IsAny<CancellationToken>()))
//            .ReturnsAsync(pagedResult);
//        _mapperMock.Setup(m => m.Map<IEnumerable<ProgramActionDto>>(It.IsAny<IEnumerable<SysProgramActions>>()))
//            .Returns(dtos);

//        // Act
//        var result = await _controller.GetAll(filter, CancellationToken.None);

//        // Assert
//        result.Should().BeOfType<OkObjectResult>();
//        _repoMock.Verify(r => r.GetAllAsync(It.Is<CommonFilterDto>(f => f.Active == true), It.IsAny<CancellationToken>()), Times.Once);
//    }

//    #endregion

//    #region Concurrency and Performance Tests

//    [Fact]
//    public async Task ParallelCreate_MultipleConcurrentRequests_ShouldHandleGracefully()
//    {
//        // Arrange
//        var tasks = new List<Task<IActionResult>>();
//        for (int i = 1; i <= 5; i++)
//        {
//            var index = i;
//            var dto = new ProgramActionCreateDto { ProgramActionName = $"Concurrent_Action_{index}" };
//            var entity = new SysProgramActions { SysProgramActionId = index, ProgramActionName = dto.ProgramActionName };
//            var programDto = new ProgramActionDto { ActionId = index, ActionName = dto.ProgramActionName, Active = true };

//            _repoMock.Setup(r => r.ExistsByNameAsync($"Concurrent_Action_{index}", null, It.IsAny<CancellationToken>()))
//                .ReturnsAsync(false);
//            _repoMock.Setup(r => r.CreateAsync(It.Is<SysProgramActions>(e => e.ProgramActionName == $"Concurrent_Action_{index}"), It.IsAny<CancellationToken>()))
//                .ReturnsAsync(entity);
//            _repoMock.Setup(r => r.GetByIdAsync(index, It.IsAny<CancellationToken>()))
//                .ReturnsAsync(entity);
//            _mapperMock.Setup(m => m.Map<ProgramActionDto>(It.Is<SysProgramActions>(e => e.SysProgramActionId == index)))
//                .Returns(programDto);

//            tasks.Add(_controller.Create(dto, CancellationToken.None));
//        }

//        // Act
//        var results = await Task.WhenAll(tasks);

//        // Assert
//        results.Should().HaveCount(5);
//        results.Should().AllBeOfType<CreatedAtActionResult>();
//    }

//    [Fact]
//    public async Task CancellationToken_WhenCancelled_ShouldPropagate()
//    {
//        // Arrange
//        var cts = new CancellationTokenSource();
//        var dto = new ProgramActionCreateDto { ProgramActionName = "CancelTest" };

//        _repoMock.Setup(r => r.ExistsByNameAsync(It.IsAny<string>(), null, It.IsAny<CancellationToken>()))
//            .Returns(async (string name, int? excludeId, CancellationToken ct) =>
//            {
//                await Task.Delay(100, ct);
//                return false;
//            });

//        cts.Cancel();

//        // Act & Assert
//        await Assert.ThrowsAnyAsync<OperationCanceledException>(
//            async () => await _controller.Create(dto, cts.Token));
//    }

//    #endregion

//    #region Error Handling Tests

//    [Fact]
//    public async Task Create_WhenRepositoryThrowsException_ShouldPropagateError()
//    {
//        // Arrange
//        var dto = new ProgramActionCreateDto { ProgramActionName = "ErrorTest" };
//        _repoMock.Setup(r => r.ExistsByNameAsync("ErrorTest", null, It.IsAny<CancellationToken>()))
//            .ReturnsAsync(false);
//        _repoMock.Setup(r => r.CreateAsync(It.IsAny<SysProgramActions>(), It.IsAny<CancellationToken>()))
//            .ThrowsAsync(new InvalidOperationException("Database error"));

//        // Act & Assert
//        await Assert.ThrowsAsync<InvalidOperationException>(
//            async () => await _controller.Create(dto, CancellationToken.None));
//    }

//    [Fact]
//    public async Task Update_AfterDelete_ShouldReturnNotFound()
//    {
//        // Arrange - Create and delete
//        var deleteDto = new CommonDeleteDto { Id = 1, Deleted = true };
//        var existingEntity = new SysProgramActions { SysProgramActionId = 1, ProgramActionName = "ToDelete" };
//        var existingDtoMapped = new ProgramActionDto { ActionId = 1, ActionName = "ToDelete", Active = true };

//        _repoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
//            .ReturnsAsync(existingEntity);
//        _mapperMock.Setup(m => m.Map<ProgramActionDto>(It.Is<SysProgramActions>(e => e.SysProgramActionId == 1)))
//            .Returns(existingDtoMapped);
//        _repoMock.Setup(r => r.DeleteAsync(It.IsAny<SysProgramActions>(), It.IsAny<CancellationToken>()))
//            .ReturnsAsync(true);

//        await _controller.Delete(1, deleteDto, CancellationToken.None);

//        // Now try to update (simulate record no longer exists)
//        _repoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
//            .ReturnsAsync((SysProgramActions?)null);
//        _repoMock.Setup(r => r.UpdateAsync(It.IsAny<SysProgramActions>(), It.IsAny<CancellationToken>()))
//            .ReturnsAsync(false);

//        var updateDto = new ProgramActionUpdateDto { ActionId = 1, ActionName = "UpdateDeleted" };

//        // Act
//        var result = await _controller.Update(1, updateDto, CancellationToken.None);

//        // Assert
//        result.Should().BeOfType<NotFoundObjectResult>();
//    }

//    #endregion

//    #region Integration Scenario Tests

//    [Fact]
//    public async Task CompleteWorkflow_CreateMultiple_UpdateSome_DeleteOne_GetAll()
//    {
//        // Step 1: Create 3 actions
//        var createDtos = new[]
//        {
//            new ProgramActionCreateDto { ProgramActionName = "Workflow_Action_1" },
//            new ProgramActionCreateDto { ProgramActionName = "Workflow_Action_2" },
//            new ProgramActionCreateDto { ProgramActionName = "Workflow_Action_3" }
//        };

//        for (int i = 0; i < createDtos.Length; i++)
//        {
//            var entity = new SysProgramActions { SysProgramActionId = i + 1, ProgramActionName = createDtos[i].ProgramActionName };
//            var dto = new ProgramActionDto { ActionId = i + 1, ActionName = createDtos[i].ProgramActionName, Active = true };

//            _repoMock.Setup(r => r.ExistsByNameAsync(createDtos[i].ProgramActionName, null, It.IsAny<CancellationToken>()))
//                .ReturnsAsync(false);
//            _repoMock.Setup(r => r.CreateAsync(It.Is<SysProgramActions>(e => e.ProgramActionName == createDtos[i].ProgramActionName), It.IsAny<CancellationToken>()))
//                .ReturnsAsync(entity);
//            _repoMock.Setup(r => r.GetByIdAsync(i + 1, It.IsAny<CancellationToken>()))
//                .ReturnsAsync(entity);
//            _mapperMock.Setup(m => m.Map<ProgramActionDto>(It.Is<SysProgramActions>(e => e.SysProgramActionId == i + 1)))
//                .Returns(dto);

//            await _controller.Create(createDtos[i], CancellationToken.None);
//        }

//        // Step 2: Update action 2
//        var updateDto = new ProgramActionUpdateDto { ActionId = 2, ActionName = "Workflow_Action_2_Updated", Active = true };
//        var updatedEntity = new SysProgramActions { SysProgramActionId = 2, ProgramActionName = "Workflow_Action_2_Updated", IsActive = true };
//        var updatedDto = new ProgramActionDto { ActionId = 2, ActionName = "Workflow_Action_2_Updated", Active = true };

//        _repoMock.Setup(r => r.ExistsByNameAsync("Workflow_Action_2_Updated", 2, It.IsAny<CancellationToken>()))
//            .ReturnsAsync(false);
//        _repoMock.Setup(r => r.UpdateAsync(It.IsAny<SysProgramActions>(), It.IsAny<CancellationToken>()))
//            .ReturnsAsync(true);
//        _repoMock.Setup(r => r.GetByIdAsync(2, It.IsAny<CancellationToken>()))
//            .ReturnsAsync(updatedEntity);
//        _mapperMock.Setup(m => m.Map<ProgramActionDto>(It.Is<SysProgramActions>(e => e.SysProgramActionId == 2 && e.ProgramActionName == "Workflow_Action_2_Updated")))
//            .Returns(updatedDto);

//        await _controller.Update(2, updateDto, CancellationToken.None);

//        // Step 3: Delete action 1
//        var deleteDto = new CommonDeleteDto { Id = 1, Deleted = true };
//        var toDeleteEntity = new SysProgramActions { SysProgramActionId = 1, ProgramActionName = "Workflow_Action_1" };
//        var toDeleteDto = new ProgramActionDto { ActionId = 1, ActionName = "Workflow_Action_1", Active = true };

//        _repoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
//            .ReturnsAsync(toDeleteEntity);
//        _mapperMock.Setup(m => m.Map<ProgramActionDto>(It.Is<SysProgramActions>(e => e.SysProgramActionId == 1)))
//            .Returns(toDeleteDto);
//        _repoMock.Setup(r => r.DeleteAsync(It.IsAny<SysProgramActions>(), It.IsAny<CancellationToken>()))
//            .ReturnsAsync(true);

//        await _controller.Delete(1, deleteDto, CancellationToken.None);

//        // Step 4: Get all (should return 2 active records)
//        var remainingEntities = new List<SysProgramActions>
//        {
//            new SysProgramActions { SysProgramActionId = 2, ProgramActionName = "Workflow_Action_2_Updated", IsActive = true },
//            new SysProgramActions { SysProgramActionId = 3, ProgramActionName = "Workflow_Action_3", IsActive = true }
//        };
//        var pagedResult = new PagedResultDto<SysProgramActions> { Items = remainingEntities, TotalCount = 2 };
//        var remainingDtos = remainingEntities.Select(e => new ProgramActionDto
//        {
//            ActionId = e.SysProgramActionId,
//            ActionName = e.ProgramActionName,
//            Active = e.IsActive
//        }).ToList();

//        _repoMock.Setup(r => r.GetAllAsync(It.IsAny<CommonFilterDto>(), It.IsAny<CancellationToken>()))
//            .ReturnsAsync(pagedResult);
//        _mapperMock.Setup(m => m.Map<IEnumerable<ProgramActionDto>>(It.IsAny<IEnumerable<SysProgramActions>>()))
//            .Returns(remainingDtos);

//        var getAllResult = await _controller.GetAll(new CommonFilterDto(), CancellationToken.None);

//        // Assert
//        getAllResult.Should().BeOfType<OkObjectResult>();
//        _repoMock.Verify(r => r.CreateAsync(It.IsAny<SysProgramActions>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
//        _repoMock.Verify(r => r.UpdateAsync(It.IsAny<SysProgramActions>(), It.IsAny<CancellationToken>()), Times.Once);
//        _repoMock.Verify(r => r.DeleteAsync(It.IsAny<SysProgramActions>(), It.IsAny<CancellationToken>()), Times.Once);
//    }

//    #endregion
//}