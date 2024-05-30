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
    public class BlogTagControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory<ProgramForTesting>>
    {
        private readonly HttpClient _client;

        public BlogTagControllerIntegrationTests(CustomWebApplicationFactory<ProgramForTesting> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAllBlogTags_ReturnsSuccessAndCorrectContentType()
        {
            var response = await _client.GetAsync("/api/v1.0/BlogTag/AllBlogTags");
            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType!.ToString());
        }

        [Fact]
        public async Task GetBlogPostComment_ReturnsNotFoundForInvalidId()
        {
            var response = await _client.GetAsync($"/api/v1.0/BlogTag/GetBlogTag/invalid-id");
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        
    }
}
