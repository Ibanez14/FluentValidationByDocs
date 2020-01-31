using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using FluentValidationWeb.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FluentValidationWeb.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {

        /// <summary>
        /// Here, we can ourselves can create Validator and use it
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult TestValidation()
        {
            var model = new UserRegisterRequest();
            var validator = new UserRegisterRequestValidator();

            ValidationResult result = validator.Validate(model);



            // or you can validate and throw
            validator.ValidateAndThrow(model);

            if (!result.IsValid)
            {
                var response = new 
                {
                    Done = result.RuleSetsExecuted,
                    ListOfAllErrors = result.Errors,
                    StringOfAllErrors = result.ToString(), // by default separated by new line
                    StringOfAllErrorsSeparatedByComma = result.ToString(",")
                };


                return BadRequest(response);
            }

            return Ok();
        }
       


        /// <summary>
        /// Here, request model will be validatd in ValidationFilter
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Test([FromBody] UserRegisterRequest request)
        {
            return Ok();
        }



        [HttpPost] // Supposing that we have no validation in filters
        // we can do this
        public IActionResult Test2([CustomizeValidator(RuleSet ="MyRules")]
                                    UserRegisterRequest request)
        {
            return Ok();
        }




        [HttpPost] // Supposing that we have no validation in filters
        // Only  Firstname, Lastname will be validated
        public IActionResult Test3([CustomizeValidator(Properties = "Firstname, Lastname")]
                                    UserRegisterRequest request)
        {
            return Ok();
        }



    }
}
