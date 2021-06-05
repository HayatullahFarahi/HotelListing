using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using HotelListing.JWTDecoder;
using Microsoft.AspNetCore.Http;

namespace HotelListing.Middleware
{
    public class ExpiredTokenMiddleware
    {
        private readonly RequestDelegate _next;
        

        public ExpiredTokenMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        //TODO: this middleware checks for expired token and returns a custom response and status on expiration
        public async Task Invoke(HttpContext context)
        {
            var jwtService = new JWTService();
            var expiryTime = jwtService.GetExpiryTimestamp(context.Request.Headers["Authorization"]);
            if (context.Request.Headers["Authorization"].Count > 0 &&  DateTime.Now > expiryTime)
            {
                context.Response.StatusCode = 455;
                await context.Response.WriteAsJsonAsync( new { title = "Unauthorized", message = "Token Expired"});
                // DO NOT CALL NEXT. THIS SHORT CIRCUITS THE PIPELINE
            }
            else
            {
                await _next(context);
            }
        }
    }
}