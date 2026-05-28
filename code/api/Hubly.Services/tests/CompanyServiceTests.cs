using Hubly.api.Services.Fixtures;
using Hubly.api.Services.Interfaces;
using Hubly.api.Services.Problems;
using Hubly.api.Domain.Entities;
using Hubly.api.Infrastructure.Interfaces;
using Moq;
using Xunit;

namespace Hubly.api.Services.Tests;

public class CompanyServiceTests : IClassFixture<CompanyServiceFixture>
{
    private readonly CompanyServiceFixture _fixture;

    public CompanyServiceTests(CompanyServiceFixture fixture)
    {
        _fixture = fixture;
        _fixture.ResetMocks();
    }

    // ---------------------------------------------------------Tests Register--------------------------------------------------------------

    [Fact]
    public async Task Register_ShouldReturnInvalidWebsite_WhenUrlIsInvalid()
    {
        int userId = 1;
        int companySizeId = 100;
        string name = "Name";
        string description = "Desc";
        List<string> sectors = new List<string>();
        string invalidUrl = "invalid-url";
        string countryHeadquarters = "Portugal";


        var result = await _fixture.CompanyService.Register(userId, companySizeId, name, description, sectors, invalidUrl, countryHeadquarters);
        Assert.True(result.IsT1);
        Assert.IsType<CompanyError.InvalidWebSiteLink>(result.AsT1);
    }

    [Fact]
    public async Task Register_ShouldReturnInvalidCountry_WhenCountryIsInvalid()
    {
        int userId = 1;
        int companySizeId = 100;
        string name = "Name";
        string description = "Desc";
        List<string> sectors = new List<string>();
        string validUrl = "http://valid.com";
        string countryHeadquarters = "InvalidCountry";


        var result = await _fixture.CompanyService.Register(userId, companySizeId, name, description, sectors, validUrl, countryHeadquarters);
        Assert.True(result.IsT1);
        Assert.IsType<CompanyError.InvalidCountryHeadquarters>(result.AsT1);
    }

    [Fact]
    public async Task Register_ShouldReturnInvalidSector_WhenSectorNotFound()
    {
        int userId = 1;
        int companySizeId = 100;
        string name = "Name";
        string description = "Desc";
        List<string> sectors = new List<string> { "S1" };
        string validUrl = "http://valid.com";
        string countryHeadquarters = "Portugal";

        _fixture.SetupGetSectorByName(new List<string> { "S1" }, new List<Sector>()); // mismatch
        var result = await _fixture.CompanyService.Register(userId, companySizeId, name, description, sectors, validUrl, countryHeadquarters);
        Assert.True(result.IsT1);
        Assert.IsType<CompanyError.InvalidSectorName>(result.AsT1);
    }

    [Fact]
    public async Task Register_ShouldReturnCompanyAlreadyExists_WhenUserAlreadyHasCompany()
    {
        int userId = 1;
        int companySizeId = 100;
        string name = "Name";
        string description = "Desc";
        List<string> sectors = new List<string>();
        string validUrl = "http://valid.com";
        string countryHeadquarters = "Portugal";

        _fixture.SetupGetSectorByName(new(), new());
        _fixture.SetupCompanyExists(userId, true);
        var result = await _fixture.CompanyService.Register(userId, companySizeId, name, description, sectors, validUrl, countryHeadquarters);
        Assert.True(result.IsT1);
        Assert.IsType<CompanyError.CompanyAlreadyExists>(result.AsT1);
    }

    [Fact]
    public async Task Register_ShouldReturnUserIsCreator_WhenUserIsRegisteredAsCreator()
    {
        int userId = 1;
        int companySizeId = 100;
        string name = "Name";
        string description = "Desc";
        var sectors = new List<string> { "S1" };
        string validUrl = "http://valid.com";
        string countryHeadquarters = "Portugal";

        _fixture.SetupGetSectorByName(sectors, new List<Sector> { new Sector() });
        _fixture.SetupCompanyExists(userId, false);
        _fixture.SetupCreatorExists(userId, true);
        var result = await _fixture.CompanyService.Register(userId, companySizeId, name, description, sectors, validUrl, countryHeadquarters);
        Assert.True(result.IsT1);
        Assert.IsType<CompanyError.UserAlreadyRegisteredAsCreator>(result.AsT1);
    }

    // ---------------------------------------------------------Tests EditProfile --------------------------------------------------------------

    [Fact]
    public async Task EditProfile_ShouldReturnError_WhenUserIsCreator()
    {
        int userId = 1;
        int companySizeId = 100;
        string name = "N";
        string description = "D";
        List<string> sectors = new List<string>();
        string validUrl = "http://v.com";
        string countryHeadquarters = "Portugal";

        _fixture.SetupCreatorExists(userId, true);
        var result = await _fixture.CompanyService.EditProfile(userId, companySizeId, name, description, sectors, validUrl, countryHeadquarters);
        Assert.True(result.IsT1);
        Assert.IsType<CompanyError.UserAlreadyRegisteredAsCreator>(result.AsT1);
    }

    [Fact]
    public async Task EditProfile_ShouldReturnFailedToGetInfo_WhenCompanyDoesNotExist()
    {
        int userId = 1;
        int companySizeId = 100;
        string name = "N";
        string description = "D";
        List<string> sectors = new List<string>();
        string validUrl = "http://v.com";
        string countryHeadquarters = "Portugal";

        _fixture.SetupCreatorExists(userId, false);
        _fixture.SetupGetByUserId(userId, null);
        var result = await _fixture.CompanyService.EditProfile(userId, companySizeId, name, description, sectors, validUrl, countryHeadquarters);
        Assert.True(result.IsT1);
        Assert.IsType<CompanyError.FailedToGetCompanyInfo>(result.AsT1);
    }

    // ---------------------------------------------------------Tests GetById and Search --------------------------------------------------------------

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenCompanyDoesNotExist()
    {
        int userId = 99;

        _fixture.SetupGetByUserId(userId, null);
        var result = await _fixture.CompanyService.GetById(userId, 1);
        Assert.True(result.IsT1);
        Assert.IsType<CompanyError.CompanyNotFound>(result.AsT1);
    }

    [Fact]
    public async Task GetTrendingCompanies_ShouldReturnEmptyList_WhenHistoryIsNull()
    {
        _fixture.SetupTrendingCompanies(null);
        var result = await _fixture.CompanyService.GetTrendingCompanies(5);
        Assert.True(result.IsT0);
        Assert.Empty(result.AsT0);
    }


}
