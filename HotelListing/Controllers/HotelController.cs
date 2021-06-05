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
       public class HotelController : ControllerBase
       {
          private readonly IUnitOfWork _unitOfWork;
          private readonly ILogger<HotelController> _logger;
          private readonly IMapper _mapper;
    
          public HotelController(IUnitOfWork unitOfWork, ILogger<HotelController> logger, IMapper mapper)
          {
             _unitOfWork = unitOfWork;
             _logger = logger;
             _mapper = mapper;
          }
    
          [HttpGet]
          public async Task<IActionResult> GetHotels()
          {
             try
             {
                var hotels = await _unitOfWork.Hotels.GetAll();
                var results = _mapper.Map<IList<HotelDTO>>(hotels);
                return Ok(results);
             }
             catch (Exception ex)
             {
                _logger.LogError(ex, $"Something went wrong is the {nameof(GetHotels)}");
                return StatusCode(500, new
                {
                   CustomeMessage = "Internal server error, something went wrong",
                   Message = ex.Message
                });
             }
          }
          
          [HttpGet("{id:int}", Name = "GetHotel")]
          public async Task<IActionResult> GetHotel(int id)
          {
             try
             {
                var hotel = await _unitOfWork.Hotels.Get(q => q.Id == id,
                   new List<string>{ "Country" });
                var result = _mapper.Map<HotelDTO>(hotel);
                return Ok(result);
             }
             catch (Exception ex)
             {
                _logger.LogError(ex, $"Something went wrong is the {nameof(GetHotel)}");
                return StatusCode(500, new
                {
                   CustomeMessage = "Internal server error, something went wrong",
                   Message = ex.Message
                }
                );
             }
          }

          [Authorize( Roles = "Admin")]
          [HttpPost]
          public async Task<IActionResult> CreateHotel([FromBody] CreateHotelDTO hotelDTO)
          {
             if (!ModelState.IsValid)
             {
                _logger.LogError($"Invalid POST attempt in {nameof(CreateHotel)}");
                return BadRequest(ModelState);
             }
             var hotel = _mapper.Map<Hotel>(hotelDTO);
             await _unitOfWork.Hotels.Insert(hotel);
             await _unitOfWork.Save();
             return CreatedAtRoute("GetHotel", new { id = hotel.Id}, hotel);
             }
          [Authorize]
          [HttpPut("{id:int}")]
          public async Task<IActionResult> UpdateHotel(int id, [FromBody] UpdateHotelDTO hotelDTO)
          {
             if (!ModelState.IsValid || id < 1)
             {
                return BadRequest(ModelState);
             }
             var hotel = await _unitOfWork.Hotels.Get(q => q.Id == id);
                if (hotel == null)
                {
                   return NotFound(new
                   {
                      title = "Not Found",
                      message = "The submitted hotel was not found!"
                   });
                }

                 _mapper.Map(hotelDTO, hotel);
                 _unitOfWork.Hotels.Update(hotel);
                 await _unitOfWork.Save();

                 return Ok(new ResponseModel
                 {
                    Title = "Updated",
                    Message = "The record was updated successfully!"
                 });
             
          }

          [Authorize( Roles = "Admin")]
          [HttpDelete("{id:int}")]
          public async Task<IActionResult> DeleteHotel(int id)
          {
             if (id < 1)
             {
                _logger.LogError($"Invalid DELETE attempt in {nameof(DeleteHotel)}");
                return NotFound(new ResponseModel
                {
                   Title = "Not Found",
                   Message = "Hotel not found!"
                });
             }
             var hotel = await _unitOfWork.Hotels.Get(q => q.Id == id);
              if (hotel == null)
              {
                 _logger.LogError($"Invalid DELETE attempt in {nameof(DeleteHotel)}");
                 return BadRequest("Submitted data is invalid");
              }

              await _unitOfWork.Hotels.Delete(id);
              await _unitOfWork.Save();

              return Ok(new ResponseModel
              {
                 Title = "Deleted",
                 Message = "The record was deleted successfully!"
              });
            
          }
          
       }
}