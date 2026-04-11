using AutoMapper;
using backend.Models.Common;
using backend.Models.DTOs;
using backend.Models.Repositories.Interfaces;
using backend.Services.Interfaces;

namespace backend.Services
{
    /// <summary>
    /// Сервис для работы со справочником регионов
    /// </summary>
    public class RegionService : IRegionService
    {
        private readonly IRegionRepository _regionRepository;
        private readonly IMapper _mapper;

        public RegionService(IRegionRepository regionRepository, IMapper mapper)
        {
            _regionRepository = regionRepository;
            _mapper = mapper;
        }

        public async Task<Result<List<LookupItemDto>>> GetAllAsync(string? query = null, string? sortBy = null, bool sortDesc = false)
        {
            var regions = await _regionRepository.GetAllAsync(query, sortBy, sortDesc);
            var dtos = _mapper.Map<List<LookupItemDto>>(regions);
            return Result<List<LookupItemDto>>.Success(dtos);
        }
    }
}
