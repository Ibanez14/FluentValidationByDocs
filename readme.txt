Steps:

1) install-package FluentValidation.AspNetCore

2)  services.AddControllers()
            .AddFluentValidation();

3) We have PersonRegisterModel. So Create Validator class that inherit from AsbstractValidator (see Validators);
4) We created ValidationFilter and ErrorResponse, and registered ValidationFilter in Startup
    -services.AddControllers(ops=> ops.Filters.Add<ValidationFilter>())


5) Also, to access your validator you can add it as service as we did in startup
 - services.AddTransient<IValidator<RequestModel>, RequestModelValidator>();
 -- or you can register all validators

 6) Also you can inject child validators (see UserRegisterValidator)
    
