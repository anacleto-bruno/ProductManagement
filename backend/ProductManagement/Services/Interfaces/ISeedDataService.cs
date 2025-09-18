using ProductManagement.Dtos;

namespace ProductManagement.Services.Interfaces;

public interface ISeedDataService
{
    Task<SeedResponseDto> SeedDataAsync(int numRows);
}