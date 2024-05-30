using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using App.BLL.DTO;
using App.DAL.EF;
using App.DTO.v1_0;
using App.Domain.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;
using Assert = Xunit.Assert;
using BlogPostComment = App.BLL.DTO.BlogPostComment;
using ExcerciseRating = App.BLL.DTO.ExcerciseRating;

namespace App.Test.Integration.api
{
    public class ExcdrciseRatingControllerTests : IClassFixture<CustomWebApplicationFactory<ProgramForTesting>>
    {
        private readonly HttpClient _client;

        public ExcdrciseRatingControllerTests(CustomWebApplicationFactory<ProgramForTesting> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAllBlogPostComments_ReturnsSuccessAndCorrectContentType()
        {
            var response = await _client.GetAsync("/api/v1.0/ExcerciseRating/AllExcerciseRating");
            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType!.ToString());
        }

        [Fact]
        public async Task GetExcerciseRatingComment_ReturnsNotFoundForInvalidId()
        {
            var response = await _client.GetAsync($"/api/v1.0/ExcerciseRating/GetExcerciseRating/invalid");
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        

        [Fact]
        public async Task DeleteExcerciseRatingComment_ReturnsUnauthorizedForUnauthenticatedUser()
        {
            var response = await _client.DeleteAsync($"/api/v1.0/ExcerciseRating/DeleteExcerciseRating/faed5d2d-6930-48d0-96cc-123f4f6d123e");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task AddNewExcerciseRatingComment_ReturnsUnauthorizedForUnauthenticatedUser()
        {
            var ExcerciseRatingComment = new ExcerciseRating
            {
                Comment = "Test comment",
                Rating = 3,
                Id = Guid.NewGuid()
            };

            var payload = new StringContent(JsonSerializer.Serialize(ExcerciseRatingComment), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/v1.0/ExcerciseRating/AddExcerciseRating", payload);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task UpdateExcerciseRatingComment_ReturnsUnauthorizedForUnauthenticatedUser()
        {
            var ExcerciseRatingComment = new ExcerciseRating
            {
                Id = Guid.NewGuid(),
                Comment = "Updated comment",
                ExcerciseId = Guid.NewGuid()
            };

            var payload = new StringContent(JsonSerializer.Serialize(ExcerciseRatingComment), Encoding.UTF8, "application/json");
            var response = await _client.PutAsync("/api/v1.0/ExcerciseRating/UpdateExcerciseRating", payload);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        // Helper method to simulate an authenticated request
        private void AuthenticateClient(string token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        
    }
}
