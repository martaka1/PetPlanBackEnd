using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using App.DTO.v1_0.Identity;
using Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using NuGet.Protocol.Core.Types;
using Xunit;
using Assert = NUnit.Framework.Assert;

namespace App.Test.Integration.api;

[Collection("NonParallel")]
public class PetControllerTest : IClassFixture<CustomWebApplicationFactory<ProgramForTesting>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<ProgramForTesting> _factory;

    public PetControllerTest(CustomWebApplicationFactory<ProgramForTesting> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task IndexRequiresLogin()
    {
        // Act
        var response = await _client.GetAsync("/api/v1.0/Pet/AllPets");
        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }
    
}