# dotnet_core_simplejwt

https://www.nuget.org/packages/dotnet_core_simple_jwt/1.0.0


JWT Middleware for asp.net core using Identity.

STILL A WIP:

TODO:

Better error handling
More configuration
Better Docs
Code cleanup

# Getting started 

  Scaffold out a mvc core application with identity setup


`
  dotnet new mvc --auth Individual
`

In your configure services method add the following lines below the identity and mvc setup


```
services.Configure<JwtMiddlewareOptions>(options =>
{
    options.secret = "change_this_first";
    options.tokenEndpoint = "/login";
    options.registerEndpoint = "/signup";
});
services.AddScoped<AuthorizationAttribute>();
```

And in your configure method add this line below the use identity line,

```
app.UseJwt<ApplicationUser>();
```


To register a user hit `/signup` with
```
{
  "username": "",
  "password": ""
}

```

To get a token hit `/login` with

```
{
  "username": "",
  "password": ""
}
```


