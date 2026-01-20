using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using VoiceFirst_Admin.API.Controllers;
using VoiceFirst_Admin.Business.Contracts.IServices;
using VoiceFirst_Admin.Utilities.Constants;
using VoiceFirst_Admin.Utilities.DTOs.Features.ProgramAction;
using VoiceFirst_Admin.Utilities.DTOs.Shared;
using VoiceFirst_Admin.Utilities.Models.Common;
using VoiceFirst_Admin.Utilities.Models.Entities;
using Xunit;

namespace VoiceFirst_Admin.Unit_Test;

public class ProgramActionControllerTests
{
    private readonly Mock<IProgramActionService> _serviceMock;
    private readonly ProgramActionController _controller;

    public ProgramActionControllerTests()
    {
        _serviceMock = new Mock<IProgramActionService>();
        _controller = new ProgramActionController(_serviceMock.Object);
    }

    #region Create Tests

    [Fact]
    public async Task Create_WithNullModel_ShouldReturnBadRequest()
    {
        // Act
        var result = await _controller.Create(null!, CancellationToken.None);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequest = result as BadRequestObjectResult;
        badRequest!.Value.Should().BeOfType<ApiResponse<object>>();
    }

    [Fact]
    public async Task Create_WithExistingName_ShouldReturnConflict()
    {
        // Arrange
        var dto = new ProgramActionCreateDto { ProgramActionName = "ExistingAction" };
        _serviceMock.Setup(s => s.ExistsByNameAsync("ExistingAction", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.Create(dto, CancellationToken.None);

        // Assert
        result.Should().BeOfType<ConflictObjectResult>();
        var conflict = result as ConflictObjectResult;
        var response = conflict!.Value as ApiResponse<object>;
        response!.StatusCode.Should().Be(StatusCodes.Status409Conflict);
        response.Message.Should().Be(Messages.NameAlreadyExists);
    }

    [Fact]
    public async Task Create_WithValidData_ShouldReturnCreatedAtAction()
    {
        // Arrange
        var dto = new ProgramActionCreateDto { ProgramActionName = "NewAction" };
        var createdEntity = new SysProgramActions { SysProgramActionId = 1, ProgramActionName = "NewAction" };
        var programActionDto = new ProgramActionDto { ActionId = 1, ActionName = "NewAction", Active = true };

        _serviceMock.Setup(s => s.ExistsByNameAsync("NewAction", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _serviceMock.Setup(s => s.CreateAsync(dto, It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdEntity);
        _serviceMock.Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(programActionDto);

        // Act
        var result = await _controller.Create(dto, CancellationToken.None);

        // Assert
        result.Should().BeOfType<CreatedAtActionResult>();
        var created = result as CreatedAtActionResult;
        created!.ActionName.Should().Be(nameof(ProgramActionController.GetById));
        _serviceMock.Verify(s => s.CreateAsync(dto, It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Create_WhenGetByIdReturnsNull_ShouldReturn500()
    {
        // Arrange
        var dto = new ProgramActionCreateDto { ProgramActionName = "NewAction" };
        var createdEntity = new SysProgramActions { SysProgramActionId = 1, ProgramActionName = "NewAction" };

        _serviceMock.Setup(s => s.ExistsByNameAsync("NewAction", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _serviceMock.Setup(s => s.CreateAsync(dto, It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdEntity);
        _serviceMock.Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProgramActionDto?)null);

        // Act
        var result = await _controller.Create(dto, CancellationToken.None);

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var objectResult = result as ObjectResult;
        objectResult!.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
    }

    #endregion

    #region GetById Tests

    [Fact]
    public async Task GetById_WithValidId_ShouldReturnOk()
    {
        // Arrange
        var dto = new ProgramActionDto { ActionId = 1, ActionName = "Action1", Active = true };
        _serviceMock.Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(dto);

        // Act
        var result = await _controller.GetById(1, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        var response = okResult!.Value as ApiResponse<ProgramActionDto>;
        response!.Data.Should().NotBeNull();
        response.Data!.ActionId.Should().Be(1);
    }

    [Fact]
    public async Task GetById_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        _serviceMock.Setup(s => s.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProgramActionDto?)null);

        // Act
        var result = await _controller.GetById(999, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
        var notFound = result as NotFoundObjectResult;
        var response = notFound!.Value as ApiResponse<object>;
        response!.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }

    #endregion

    #region GetAll Tests

    [Fact]
    public async Task GetAll_WithValidFilter_ShouldReturnOk()
    {
        // Arrange
        var filter = new CommonFilterDto { PageNumber = 1, Limit = 10 };
        var pagedResult = new PagedResultDto<ProgramActionDto>
        {
            Items = new[] { new ProgramActionDto { ActionId = 1, ActionName = "Action1" } },
            TotalCount = 1,
            PageNumber = 1,
            PageSize = 10
        };
        _serviceMock.Setup(s => s.GetAllAsync(filter, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _controller.GetAll(filter, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeOfType<ApiResponse<object>>();
    }

    [Fact]
    public async Task GetAll_WithSearchText_ShouldCallServiceWithFilter()
    {
        // Arrange
        var filter = new CommonFilterDto { SearchText = "Test", PageNumber = 1, Limit = 10 };
        var pagedResult = new PagedResultDto<ProgramActionDto> { Items = new ProgramActionDto[0], TotalCount = 0 };
        _serviceMock.Setup(s => s.GetAllAsync(filter, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        // Act
        await _controller.GetAll(filter, CancellationToken.None);

        // Assert
        _serviceMock.Verify(s => s.GetAllAsync(
            It.Is<CommonFilterDto>(f => f.SearchText == "Test"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region Update Tests

    [Fact]
    public async Task Update_WithNullModel_ShouldReturnBadRequest()
    {
        // Act
        var result = await _controller.Update(1, null!, CancellationToken.None);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Update_WithMismatchedId_ShouldReturnBadRequest()
    {
        // Arrange
        var dto = new ProgramActionUpdateDto { ActionId = 2, ActionName = "Updated" };

        // Act
        var result = await _controller.Update(1, dto, CancellationToken.None);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Update_WithExistingName_ShouldReturnConflict()
    {
        // Arrange
        var dto = new ProgramActionUpdateDto { ActionId = 1, ActionName = "ExistingName" };
        _serviceMock.Setup(s => s.ExistsByNameAsync("ExistingName", 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.Update(1, dto, CancellationToken.None);

        // Assert
        result.Should().BeOfType<ConflictObjectResult>();
    }

    [Fact]
    public async Task Update_WithValidData_ShouldReturnCreatedAtAction()
    {
        // Arrange
        var dto = new ProgramActionUpdateDto { ActionId = 1, ActionName = "UpdatedAction" };
        var updatedDto = new ProgramActionDto { ActionId = 1, ActionName = "UpdatedAction", Active = true };

        _serviceMock.Setup(s => s.ExistsByNameAsync("UpdatedAction", 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _serviceMock.Setup(s => s.UpdateAsync(dto, It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _serviceMock.Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(updatedDto);

        // Act
        var result = await _controller.Update(1, dto, CancellationToken.None);

        // Assert
        result.Should().BeOfType<CreatedAtActionResult>();
        _serviceMock.Verify(s => s.UpdateAsync(dto, It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Update_WhenUpdateFails_ShouldReturnNotFound()
    {
        // Arrange
        var dto = new ProgramActionUpdateDto { ActionId = 1, ActionName = "UpdatedAction" };

        _serviceMock.Setup(s => s.ExistsByNameAsync("UpdatedAction", 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _serviceMock.Setup(s => s.UpdateAsync(dto, It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.Update(1, dto, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    #endregion

    #region Delete Tests

    [Fact]
    public async Task Delete_WithNullModel_ShouldReturnBadRequest()
    {
        // Act
        var result = await _controller.Delete(1, CancellationToken.None);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequest = result as BadRequestObjectResult;
        var response = badRequest!.Value as ApiResponse<object>;
        response!.Message.Should().Be(Messages.PayloadRequired);
    }

    [Fact]
    public async Task Delete_WithMismatchedId_ShouldReturnBadRequest()
    {
        // Arrange
        var dto = new CommonDeleteDto { Id = 2, Deleted = true };

        // Act
        var result = await _controller.Delete(1, CancellationToken.None);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Delete_WhenRecordNotFound_ShouldReturnNotFound()
    {
        // Arrange
        _serviceMock.Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProgramActionDto?)null);

        // Act
        var result = await _controller.Delete(1, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
        var notFound = result as NotFoundObjectResult;
        var response = notFound!.Value as ApiResponse<object>;
        response!.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }



    [Fact]
    public async Task Delete_WhenDeleteFails_ShouldReturnNotFound()
    {
        // Arrange
        _serviceMock.Setup(s => s.DeleteAsync(1, It.IsAny<int>(), It.IsAny<CancellationToken>()))
           .ReturnsAsync(false);

        // Act
        var result = await _controller.Delete(1, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    #endregion

    #region Restore Tests

    [Fact]
    public async Task Restore_WhenSuccessful_ShouldReturnOk()
    {
        // Arrange
        _serviceMock.Setup(s => s.RestoreAsync(1, It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.Restore(1, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        _serviceMock.Verify(s => s.RestoreAsync(1, It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Restore_WhenFails_ShouldReturnNotFound()
    {
        // Arrange
        _serviceMock.Setup(s => s.RestoreAsync(1, It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.Restore(1, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    #endregion

    #region CancellationToken Tests

    [Fact]
    public async Task Create_ShouldPassCancellationToken()
    {
        // Arrange
        var dto = new ProgramActionCreateDto { ProgramActionName = "Test" };
        var cts = new CancellationTokenSource();
        var token = cts.Token;

        _serviceMock.Setup(s => s.ExistsByNameAsync(It.IsAny<string>(), null, token))
            .ReturnsAsync(false);
        _serviceMock.Setup(s => s.CreateAsync(It.IsAny<ProgramActionCreateDto>(), It.IsAny<int>(), token))
            .ReturnsAsync(new SysProgramActions { SysProgramActionId = 1 });
        _serviceMock.Setup(s => s.GetByIdAsync(1, token))
            .ReturnsAsync(new ProgramActionDto { ActionId = 1 });

        // Act
        await _controller.Create(dto, token);

        // Assert
        _serviceMock.Verify(s => s.CreateAsync(It.IsAny<ProgramActionCreateDto>(), It.IsAny<int>(), token), Times.Once);
    }

    #endregion
}