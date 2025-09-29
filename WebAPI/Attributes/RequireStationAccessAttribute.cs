using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace WebAPI.Attributes
{
    public class RequireStationAccessAttribute : ActionFilterAttribute
    {
        private readonly string _stationIdParameterName;

        public RequireStationAccessAttribute(string stationIdParameterName = "stationId")
        {
            _stationIdParameterName = stationIdParameterName;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var userRole = context.HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;

            // Backoffice has access to all stations
            if (userRole == "Backoffice")
            {
                base.OnActionExecuting(context);
                return;
            }

            // For Station Operators, check assigned stations
            if (userRole == "StationOperator")
            {
                var stationId = GetStationIdFromRequest(context);
                if (string.IsNullOrEmpty(stationId))
                {
                    context.Result = new BadRequestObjectResult(new
                    {
                        Success = false,
                        Message = "Station ID is required"
                    });
                    return;
                }

                var assignedStations = context.HttpContext.User.FindAll("assignedStation").Select(c => c.Value).ToList();
                if (!assignedStations.Contains(stationId))
                {
                    context.Result = new ObjectResult(new
                    {
                        Success = false,
                        Message = "Access denied. You are not assigned to this station."
                    })
                    {
                        StatusCode = 403
                    };
                    return;
                }
            }
            else
            {
                context.Result = new ObjectResult(new
                {
                    Success = false,
                    Message = "Access denied. Insufficient permissions."
                })
                {
                    StatusCode = 403
                };
                return;
            }

            base.OnActionExecuting(context);
        }

        private string? GetStationIdFromRequest(ActionExecutingContext context)
        {
            // Try route values first
            if (context.RouteData.Values.TryGetValue(_stationIdParameterName, out var routeValue))
            {
                return routeValue?.ToString();
            }

            // Try action parameters
            if (context.ActionArguments.TryGetValue(_stationIdParameterName, out var paramValue))
            {
                return paramValue?.ToString();
            }

            // Try query parameters
            if (context.HttpContext.Request.Query.TryGetValue(_stationIdParameterName, out var queryValue))
            {
                return queryValue.ToString();
            }

            return null;
        }
    }
}
