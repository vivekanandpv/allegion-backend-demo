using System;
using CCAP.Api.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace CCAP.Api.Filters {
    public class GeneralExceptionHandlerFilter: IActionFilter, IOrderedFilter {
        // private readonly ILogger<GeneralExceptionHandlerFilter> _logger;
        //
        // public GeneralExceptionHandlerFilter(ILogger<GeneralExceptionHandlerFilter> logger) {
        //     _logger = logger;
        // }

        public int Order => int.MaxValue - 10;
        
        public void OnActionExecuted(ActionExecutedContext context) {
            if (context.Exception is LoginFailedException) {
                // log it here to logging framework
                // _logger.LogError($"LOG: {context.Exception.Message}");
                Console.WriteLine($"LOG: {context.Exception.Message}");
                context.Result = new ObjectResult(
                    new { Message = "Cannot process the login" }
                ) {
                    StatusCode = 401
                };

                context.ExceptionHandled = true;
            }
            
            if (context.Exception is DuplicateUserRegistrationException) {
                // log it here to logging framework
                // _logger.LogError($"LOG: {context.Exception.Message}");
                Console.WriteLine($"LOG: {context.Exception.Message}");
                context.Result = new ObjectResult(
                    new { Message = "User already exists" }
                ) {
                    StatusCode = 400
                };

                context.ExceptionHandled = true;
            }
            
            if (context.Exception is DomainValidationException) {
                // log it here to logging framework
                // _logger.LogError($"LOG: {context.Exception.Message}");
                Console.WriteLine($"LOG: {context.Exception.Message}");
                context.Result = new ObjectResult(
                    new { Message = "Validation rules prevent this action" }
                ) {
                    StatusCode = 400
                };

                context.ExceptionHandled = true;
            }
            
            if (context.Exception is RoleAbsentException) {
                // log it here to logging framework
                // _logger.LogError($"LOG: {context.Exception.Message}");
                Console.WriteLine($"LOG: {context.Exception.Message}");
                context.Result = new ObjectResult(
                    new { Message = "Please select existing roles for this user" }
                ) {
                    StatusCode = 400
                };

                context.ExceptionHandled = true;
            }
            
            if (context.Exception is RecordNotFoundException) {
                // log it here to logging framework
                // _logger.LogError($"LOG: {context.Exception.Message}");
                Console.WriteLine($"LOG: {context.Exception.Message}");
                context.Result = new ObjectResult(
                    new { Message = $"Could not find the required record: {context.Exception.Message}" }
                ) {
                    StatusCode = 400
                };

                context.ExceptionHandled = true;
            }
            
            //  general
            if (!context.ExceptionHandled && context.Exception != null) {
                // log it here to logging framework
                // _logger.LogError($"LOG: {context.Exception.Message}");
                Console.WriteLine($"LOG: {context.Exception.Message}");
                context.Result = new ObjectResult(
                    new { Message = "Contact system administrator" }
                ) {
                    StatusCode = 503
                };

                context.ExceptionHandled = true;
            }
        }

        public void OnActionExecuting(ActionExecutingContext context) {
            
        }
      
    }
}