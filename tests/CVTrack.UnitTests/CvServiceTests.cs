using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CVTrack.Application.CVs.Commands;
using CVTrack.Application.CVs.Services;
using CVTrack.Application.Interfaces;
using CVTrack.Application.DTOs;
using CVTrack.Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;
using CVTrack.Application.CVs.Queries;


namespace CVTrack.UnitTests
{
    public class CvServiceTests
    {
        private readonly Mock<ICVRepository> _cvRepoMock;
        private readonly Mock<IFileService> _fileServiceMock;
        private readonly CvService _service;

        public CvServiceTests()
        {
            _cvRepoMock = new Mock<ICVRepository>();
            _fileServiceMock = new Mock<IFileService>();
            _service = new CvService(_cvRepoMock.Object, _fileServiceMock.Object);
        }

        [Fact]
        public async Task CreateAsync_Should_Call_AddAsync_And_Return_Correct_Dto()
        {
            // Arrange
            var cmd = new CreateCvCommand
            {
                UserId = Guid.NewGuid(),
                FileName = "test.pdf"
            };
            _cvRepoMock
                .Setup(r => r.AddAsync(It.IsAny<CV>()))
                .ReturnsAsync((CV c) => c);

            // Act
            var result = await _service.CreateAsync(cmd);

            // Assert
            _cvRepoMock.Verify(r => r.AddAsync(
                    It.Is<CV>(c
                        => c.UserId == cmd.UserId
                        && c.FileName == cmd.FileName
                        && c.Id != Guid.Empty
                    )),
                Times.Once);

            result.Should().BeOfType<CVDto>();
            result.UserId.Should().Be(cmd.UserId);
            result.FileName.Should().Be(cmd.FileName);
            result.Id.Should().NotBe(Guid.Empty);
            result.UploadDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public async Task GetByUserAsync_Should_Map_All_CVs_To_DTOs()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var cvs = new List<CV>
            {
                new CV { Id = Guid.NewGuid(), UserId = userId, FileName = "a.pdf", UploadDate = DateTime.UtcNow.AddDays(-1) },
                new CV { Id = Guid.NewGuid(), UserId = userId, FileName = "b.pdf", UploadDate = DateTime.UtcNow }
            };

            _cvRepoMock
                .Setup(r => r.GetByUserIdAsync(userId))
                .ReturnsAsync(cvs);

            // Act
            var result = await _service.GetByUserAsync(new GetCvsByUserQuery { UserId = userId });

            // Assert
            result.Should().HaveCount(2);
            result.Select(d => d.Id).Should().BeEquivalentTo(cvs.Select(c => c.Id));
            result.Select(d => d.FileName).Should().BeEquivalentTo(cvs.Select(c => c.FileName));
        }

        [Fact]
        public async Task DeleteAsync_Should_SoftDelete_When_CV_Exists()
        {
            // Arrange
            var id = Guid.NewGuid();
            var cv = new CV { Id = id, UserId = Guid.NewGuid(), FileName = "x.pdf", UploadDate = DateTime.UtcNow, IsDeleted = false };
            _cvRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(cv);

            // Act
            await _service.DeleteAsync(id);

            // Assert
            _cvRepoMock.Verify(r => r.UpdateAsync(cv), Times.Once);
            cv.IsDeleted.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteAsync_Should_Throw_When_CV_NotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            _cvRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((CV?)null);

            // Act
            Func<Task> act = () => _service.DeleteAsync(id);

            // Assert
            await act.Should()
                     .ThrowAsync<KeyNotFoundException>()
                     .WithMessage($"CV with Id={id} not found.");
        }
    }
}
