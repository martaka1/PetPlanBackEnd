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

namespace App.Test.Integration.api
{
    public class BlogPostCommentControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory<ProgramForTesting>>
    {
        private readonly HttpClient _client;

        public BlogPostCommentControllerIntegrationTests(CustomWebApplicationFactory<ProgramForTesting> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAllBlogPostComments_ReturnsSuccessAndCorrectContentType()
        {
            var response = await _client.GetAsync("/api/v1.0/BlogPostComment/AllBlogPostComment");
            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType!.ToString());
        }

        [Fact]
        public async Task GetBlogPostComment_ReturnsNotFoundForInvalidId()
        {
            var response = await _client.GetAsync("/api/v1.0/BlogPostComment/GetBlogPostComment/invalid-id");
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        

        [Fact]
        public async Task DeleteBlogPostComment_ReturnsUnauthorizedForUnauthenticatedUser()
        {
            var response = await _client.DeleteAsync("/api/v1.0/BlogPostComment/DeleteBlogPostComment/valid-id");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task AddNewBlogPostComment_ReturnsUnauthorizedForUnauthenticatedUser()
        {
            var blogPostComment = new BlogPostComment
            {
                Comment = "Test comment",
                BlogPostId = Guid.NewGuid()
            };

            var payload = new StringContent(JsonSerializer.Serialize(blogPostComment), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/v1.0/BlogPostComment/AddBlogPostComment", payload);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task UpdateBlogPostComment_ReturnsUnauthorizedForUnauthenticatedUser()
        {
            var blogPostComment = new BlogPostComment
            {
                Id = Guid.NewGuid(),
                Comment = "Updated comment",
                BlogPostId = Guid.NewGuid()
            };

            var payload = new StringContent(JsonSerializer.Serialize(blogPostComment), Encoding.UTF8, "application/json");
            var response = await _client.PutAsync("/api/v1.0/BlogPostComment/UpdateBlogPostComment", payload);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        // Helper method to simulate an authenticated request
        private void AuthenticateClient(string token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        
    }
}
