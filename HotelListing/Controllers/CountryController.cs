using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using HotelListing.Data;
using HotelListing.IRepository;
using HotelListing.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HotelListing.Controllers
{
   [Route("api/[controller]")]
   [ApiController]
   public class CountryController : ControllerBase
   {
      private readonly IUnitOfWork _unitOfWork;
      private readonly ILogger<CountryController> _logger;
      private readonly IMapper _mapper;

      public CountryController(IUnitOfWork unitOfWork, ILogger<CountryController> logger, IMapper mapper)
      {
         _unitOfWork = unitOfWork;
         _logger = logger;
         _mapper = mapper;
      }

      [HttpGet]
      [ProducesResponseType(StatusCodes.Status200OK)]
      [ProducesResponseType(StatusCodes.Status500InternalServerError)]
      public async Task<IActionResult> GetCountries()
      {
         try
         {
            var countries = await _unitOfWork.Countries.GetAll();
            var results = _mapper.Map<IList<CountryDTO>>(countries);
            return Ok(results);
         }
         catch (Exception ex)
         {
            _logger.LogError(ex, $"Something went wrong is the {nameof(GetCountries)}");
            return StatusCode(500, "Internal Server Error. Please try again later");
         }
      }
      [HttpGet("{id:int}", Name = "GetCountry")]
      [ProducesResponseType(StatusCodes.Status200OK)]
      [ProducesResponseType(StatusCodes.Status500InternalServerError)]
      public async Task<IActionResult> GetCountry(int id)
      {
         try
         {
            var country = await _unitOfWork.Countries.Get(q => q.Id == id,
               new List<string>{ "Hotels" });
            var result = _mapper.Map<CountryDTO>(country);
            return Ok(result);
         }
         catch (Exception ex)
         {
            _logger.LogError(ex, $"Something went wrong is the {nameof(GetCountry)}");
            return StatusCode(500, ex.Message);
         }
      }
      
      [Authorize( Roles = "Admin")]
      [HttpPost]
      public async Task<IActionResult> CreateCountry([FromBody] CreateCountryDTO countryDTO)
      {
         if (!ModelState.IsValid)
         {
            _logger.LogError($"Invalid POST attempt in {nameof(CreateCountry)}");
            return BadRequest(ModelState);
         }

         try
         {
            var country = _mapper.Map<Country>(countryDTO);
            await _unitOfWork.Countries.Insert(country);
            await _unitOfWork.Save();
            return CreatedAtRoute("GetCountry", new { id = country.Id}, country);
         }
         catch (Exception e)
         {
            _logger.LogError(e, $"Something went wrong in the {nameof(CreateCountry)}");
            return StatusCode(500, new
            {
               cusomeMessage = "Internal server error. Please try again later",
               message = e.Message
            });
         }
      }
      
      [Authorize]
      [HttpPut("{id:int}")]
      public async Task<IActionResult> UpdateCountry(int id, [FromBody] UpdateCountryDTO countryDTO)
      {
         if (!ModelState.IsValid || id < 1)
         {
            return BadRequest(ModelState);
         }

         try
         {
            var country = await _unitOfWork.Countries.Get(q => q.Id == id);
            if (country == null)
            {
               return NotFound(new
               {
                  title = "Not Found",
                  message = "The submitted country was not found!"
               });
            }

            _mapper.Map(countryDTO, country);
            _unitOfWork.Countries.Update(country);
            await _unitOfWork.Save();

            return Ok(new ResponseModel
            {
               Title = "Updated",
               Message = "The record was updated successfully!"
            });
         }
         catch (Exception e)
         {
            _logger.LogError(e, $"Something went wrong in the {nameof(UpdateCountry)}");
            return StatusCode(500, new
            {
               cusomeMessage = "Internal server error. Please try again later",
               message = e.Message
            });
         }
      }
      
      [Authorize( Roles = "Admin")]
      [HttpDelete("{id:int}")]
      public async Task<IActionResult> DeleteCountry(int id)
      {
         if (id < 1)
         {
            _logger.LogError($"Invalid DELETE attempt in {nameof(DeleteCountry)}");
            return BadRequest(new ResponseModel
            {
               Title = "Not Found",
               Message = "Country not found!"
            });
         }

         try
         {
            var country = await _unitOfWork.Countries.Get(q => q.Id == id);
            if (country == null)
            {
               _logger.LogError($"Invalid DELETE attempt in {nameof(DeleteCountry)}");
               return BadRequest(new ResponseModel
               {
                  Title = "Not Found",
                  Message = "Hotel not found!"
               });
            }

            await _unitOfWork.Countries.Delete(id);
            await _unitOfWork.Save();

            return Ok(new ResponseModel
            {
               Title = "Deleted",
               Message = "The record was deleted successfully!"
            });

         }
         catch (Exception e)
         {
            _logger.LogError(e, $"Something went wrong in the {nameof(DeleteCountry)}");
            return StatusCode(500, new
            {
               cusomeMessage = "Internal server error. Please try again later",
               message = e.Message
            });
         }

         
      }
      
   }
}