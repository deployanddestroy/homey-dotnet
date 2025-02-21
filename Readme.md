# Homey REST API
An example REST API focused on best practices and simplicity.

## Features
- Minimal APIs using REPR (Request-EndPoint-Response) pattern
- Data project using EntityFramework Core
- ASP.NET Identity REST endpoints
- Request validation via stock DataAnnotation (not FluentValidations)
- Example of OutputCache for static data
- Custom extensions, attributes and filters
- Configuration via IOptions bindings (Snapshot specifically) with Options Validation
- Simple `GlobalExceptionHandler` to demonstrate exception handling
- Logging via Serilog
- Channels and Background Service for async task processing
- SMTP E-mail with MailKit

## Overview
The idea here is to create a .NET REST API project using as many best practices as possible while staying as minimal as possible. Prefer to avoid 3rd party libraries if possible.

### Minimal APIs + REPR
Big fan of the simplicity and encapsulation of the REPR pattern. All relevant code in one place. Very component-like.

### Data with EF Core
I like to split the data part into a separate project. Keeps all the DAL stuff away from the main business logic.

### ASP.NET Identity Endpoints
The default endpoints are fine, but not customizable enough. I wanted to add my own MapGroup, and potentially extend/modify the endpoints in the future.

### OutputCache
GetProTypes endpoint is using built-in OutputCache, which caches HTTP responses. Perfect for data that never or very rarely changes! Implementation is in-memory.

### Request Validation
Although not super extensive in this project, the baseline validation is there via DataAnnotations. I wanted to stick to the existing Microsoft-provided .NET APIs rather than using a 3rd party package.

### Custom extensions, attributes and filters
Puzzled why NotEmptyGuid does not exist baseline yet, so added that in. 

Wanted a quick way to retrieve the user Id out of the claims principal, so an extension there. Also wanted to add a way to quickly slap in RequestValidation on multiple endpoints at once during endpoint creation.

The RequestValidationExtensions works with the RequestValidationFilter, which is where the work of validating actually happens.

### IOptions Configuration
This is a well known pattern. Using IOptionsMonitor to ensure I'm getting the latest updates from configuration.
Also using DataAnnotations and validation on start to ensure values are present.

### Global exception handler
The implementation is pretty rudimentary, but one could imagine for every critical exception a message being sent via channel to a dedicated logger on a separate thread.

### Serilog logging
Super standard logging. Using the file sink here. Waiting for Posthog sink...

### Channels & Background Service
In-memory queue Channel API being used here to demonstrate asynchronous processing. The default e-mail interface that comes with Identity is implemented to, instead of sending emails synchronously, send a message via a channel instead.

### SMTP via MailKit
Since the stock SmtpClient is being deprecated, opted to use Microsoft's recommended MailKit solution. Configured to take in different settings depending on environment.

## Potential Improvements
1. `HomeyEmailSender` in Modules/Email does seem a bit iffy. This is one of those "don't abstract unless you need to" scenarios I think... could do a NotificationProcessor, use the strategy patern, etc.. but not necessary at this point. 
2. Augment the CustomIdentityApis class with some extra functionality. Appreciate Mirosoft's effort put into creating these though!
3. Replace Channels with MassTransit? 
   1. Channels are good for SOME things, but implementing Outbox and other functionality when app gets complex is kinda moot when it already exists in MassTransit.
   2. May not be necessary though if app does not grow in complexity.
5. Add more functionality like document storage (via abstraction of course) for Homes. Gotta store 'em PDFs somewhere!
6. Implement HybridCache once it's out of preview
7. Add a Quartz background job to read home addresses, poll an MLS API for pricing estimates, and populate a table with those.
8. Oh yeah... tests...