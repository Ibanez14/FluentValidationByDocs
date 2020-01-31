using FluentValidation;
using FluentValidation.Results;
using System;

namespace FluentValidationWeb.Models
{
    public class UserRegisterRequestValidator : AbstractValidator<UserRegisterRequest>
    {
        private object _Client;

        public UserRegisterRequestValidator(IValidator<Address> addressValidator)
        {
            // you can access it from Controller ModelState.IsValid
            RuleFor(model => model.Tagname).NotEmpty()
                                           .NotNull()
                                           .WithMessage("Request model cannot be empty");


            // Child validation
            // 1 you can use ChildValidator
            // so now UserREquestValidator will Validate both User and Address
            RuleFor(model => model.MainAddress).SetValidator(new AddressValidator());

            // or validator from injection
            RuleFor(model => model.MainAddress).SetValidator(addressValidator);

            // or from version 8.2
            // will automatically find validator

            RuleFor(model => model.MainAddress).InjectValidator();


            // or
            // 2
            // you can set rule for child
            RuleFor(model => model.MainAddress.Street).NotEmpty()
                                                  .When(model => model.MainAddress != null);

            // Rule for collections
            // CollectionIndex placeholder will be replaced by index number

            RuleForEach(model => model.FavoriteBooks).NotNull()
                .WithMessage("Book {CollectionIndex} must not be null");


            RuleForEach(model => model.AdditionalAddresses)
                .SetValidator(new AddressValidator())
                .When(model => model.AdditionalAddresses != null);


            RuleForEach(model => model.Licenses)
                .Where(licence => licence.IssueDate < DateTime.Now) // check if not expired
                .SetValidator(new LicenceValidator());


            // {PropertyName} will be replaced by AdditionalAddreess
            RuleFor(model => model.AdditionalAddresses)
                   .Must(x => x.Count < 10).WithMessage("No more than 10 addresses are allowed in {PropertyName}")
                   .ForEach(rule =>
                   {
                       rule.Must(address => address != null);
                   });


            // more eleqant
            RuleForEach(model => model.AdditionalAddresses)
                      .ChildRules(address =>
                      {
                          address.RuleFor(a => a.Street).NotEmpty();
                      });

            RuleFor(m => m.FirstName).NotNull();



            RuleSet("Names", () =>
            {
                RuleFor(x => x.FirstName).NotEmpty();
                RuleFor(x => x.LastName).NotEmpty();

                // so this enable to this
                var request = new UserRegisterRequest();
                var validator = new UserRegisterRequestValidator();
                var result = validator.Validate(request, ruleSet: "Names");

                // also ca validate props
                var result2 = validator.Validate(request, properties: "Firstname, Lastname");

                // but now RuleSets will not be applied, only other validators
                validator.Validate(request);

                // Also you can rulesets by comma
                validator.Validate(request, ruleSet: "Names, Addresses");

                // default is key word that use all rules in Validator
                validator.Validate(request, ruleSet: "default, Addresses");

                // Use all rule sets
                validator.Validate(request, ruleSet: "*");
            });






            RuleFor(model => model.LastName)
                    .NotEmpty()

                    .WithName("Last Name")
                    // Last Name will be used in Error message



                    .OverridePropertyName("Last Name");
            // Will use in collection ErrorProperty "Last Name"  instead of LastName
            // This override WithName()



            RuleFor(customer => customer.CustomerDiscount).
                GreaterThan(0).
                When(customer => customer.IsPreferredCustomer);

            RuleFor(model => model.LastName)
                .NotEmpty()
                .Unless(model => model.Licenses.Count > 0);



            // When and otherwise

            When(model => model.IsPreferred, () =>
            {
                RuleFor(c => c.LastName).NotEmpty();
            })
            .Otherwise(() =>
            {
                RuleFor(c => c.Licenses.Count > 0);
            });


            RuleFor(customer => customer.CustomerDiscount)
                 .GreaterThan(0).When(customer => customer.IsPreferredCustomer, ApplyConditionTo.CurrentValidator)
                 .Equal(0).When(customer => !customer.IsPreferredCustomer, ApplyConditionTo.CurrentValidator);


            // Cascade Mode
            CascadeMode = CascadeMode.StopOnFirstFailure;
            // or
            var validator = new UserRegisterRequestValidator();
            validator.CascadeMode = CascadeMode.StopOnFirstFailure;
            // Cool feature


            // equivalent
            RuleFor(model => model.FirstName)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty()
                .NotEqual("Foo");


            #region Root Context Data

            // Root Context Data
            // Advanced
            // it’s possible to pass arbitrary data into the 
            // validation pipeline that can be accessed from within custom property validators.

            var request = new UserRegisterRequest();
            var validationContext = new ValidationContext<UserRegisterRequest>(request);
            validationContext.RootContextData["CustomData"] = "Custom Valu";




            RuleFor(m => m.LastName).NotNull()
                .Custom((x, context) =>
                {
                    if (context.ParentContext.RootContextData.TryGetValue("MuCustomData", out object value))
                    {
                        string val = value as string;

                        if (val == "+18")
                            context.AddFailure("Error message");
                    }
                });



            var userValidator = new UserRegisterRequestValidator();



            userValidator.Validate(validationContext);


            #endregion


            #region Callbacks


            RuleFor(x => x.FirstName).NotEmpty()
                       .OnAnyFailure(model =>
                       {
                           // logging
                           Console.WriteLine();
                       });


            RuleFor(x => x.FirstName).NotEmpty()
                       .OnFailure(model =>
                       {
                           model.AdditionalAddresses = null;
                       });


            // Async Validations

            // Example: case 




            RuleFor(x => x.Id).MustAsync(async (requestModel, idProperty, cancellationToken) =>
             {
                 var result = _Client.EnsureClientRegistered(idProperty);

                 // based on that result can return
                 if (result)
                     return false;

                 return true;
             })
             .WithMessage("User Already Registered");

            // it means that you should use
            // validator.ValidateAsyncMethod() instead of Validate()
            // OR you code will be synchorniuous



            #endregion



            #region Built-in validators


            // PlaceHolders

            RuleFor(x => x.FirstName)
                .NotNull()
                .NotEqual("Black Dog")
                .NotEqual(x => x.LastName)
                .Equal(x => x.Licenses.Count.ToString())
                .WithMessage("{PropertyValue} is not valid for {PropertyName}");


            RuleFor(x => x.LastName)
                .CreditCard()
                .EmailAddress() // Internally use EmailAddressAttribute
                .MaximumLength(259)
                .Null()
                .MinimumLength(50)
                .Length(500)
                .Matches("Regular Exps")
                .Length(1, 250)
                .Length(m => /*some logic*/ 250)
                .WithMessage("{MinLength} and {MaxLength} are not provided because" +
                "you provided {TotalLength} number of chars");





            RuleFor(x => x.CustomerDiscount)
                .ExclusiveBetween(1,50) // must not be // Placeholder {From} {To}
                .InclusiveBetween(1,50) // must be in range
                .GreaterThan(14)
                .LessThan(100)
                .LessThan(c => c.CustomerDiscount)
                // predicate validator
                .Must((model, discount) =>
                {
                    // bla bla....

                    return false;
                });





            RuleFor(x => x.Money)
                .ScalePrecision(2, 4);




            // { ExpectedPrecision} = The expected precision
            // { ExpectedScale} = The expected scale
            // { Digits} = Total number of digits in the property value
            // { ActualScale} = The actual scale of the property value











            #endregion








        }

        protected override bool PreValidate(ValidationContext<UserRegisterRequest> context, ValidationResult result)
        {
            if (context.InstanceToValidate == null)
            {
                result.Errors.Add(new ValidationFailure("", "Please ensure a model was supplied."));
                return false;
            }
            return true;

        }



        // Prevalidate methd that is run before validator is invoked






    }


    public class AddressValidator : AbstractValidator<Address>
    {
        public AddressValidator()
        {
            RuleFor(address => address.Street).NotEmpty();
        }
    }


    public class LicenceValidator : AbstractValidator<Licence>
    {
        // image there is a validation
    }
}
