using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using CksysRecruitNew.Server.Entities;
using CksysRecruitNew.Server.Extensions;
using CksysRecruitNew.Server.Helper;
using CksysRecruitNew.Server.Models;
using CksysRecruitNew.Server.Options;
using CksysRecruitNew.Server.Repositories;
using CksysRecruitNew.Server.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using SqlSugar;
using Ubiety.Dns.Core;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("secret.json");

builder.Host.UseSerilog(new LoggerConfiguration().WriteTo.Async(c => {
  if (builder.Environment.IsProduction()) c.File("/log/app/err.log", Serilog.Events.LogEventLevel.Error);
  c.Console(restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information);

}).CreateBootstrapLogger());

var services = builder.Services;

services.AddSmtpClient();
services.AddSwaggerGen();
services.AddSingleton<JwtHelper>();
services.AddSingleton<AesHelper>();

services.AddControllers()
        .ConfigureApiBehaviorOptions(options => {
           options.InvalidModelStateResponseFactory = (context) => {
             var problamDetails = new ValidationProblemDetails(context.ModelState) {
               Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
               Status = StatusCodes.Status422UnprocessableEntity,
               Instance = context.HttpContext.Request.Path,
             };

             problamDetails.Extensions.Add("traceId", context.HttpContext.TraceIdentifier);

             return new OkObjectResult(new {
               Code = 422,
               Message = "输入参数错误",
               Errors = problamDetails.Errors
             });
           };
         });

services.AddAuthorization();

services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options => {
           options.TokenValidationParameters = new TokenValidationParameters() {
             ValidateIssuer = true,                                                                                                            //是否验证Issuer
             ValidIssuer = builder.Configuration[$"{JwtOptions.SectionKey}:Issuer"],                                                           //发行人Issuer
             ValidateAudience = true,                                                                                                          //是否验证Audience
             ValidAudience = builder.Configuration[$"{JwtOptions.SectionKey}:Audience"],                                                       //订阅人Audience
             ValidateIssuerSigningKey = true,                                                                                                  //是否验证SecurityKey
             IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration[$"{JwtOptions.SectionKey}:SecretKey"])), //SecurityKey
             ValidateLifetime = true,                                                                                                          //是否验证失效时间
             ClockSkew = TimeSpan.FromSeconds(30),                                                                                             //过期时间容错值，解决服务器端时间不同步问题（秒）
             RequireExpirationTime = true,
           };
         });

services.AddEndpointsApiExplorer();
services.AddScoped<SmsService>();
services.AddScoped<EmailService>();
services.AddScoped<IApplyInfoRepository, ApplyInfoRepository>();
services.AddSqlSugarClient(builder.Configuration.GetConnectionString("Default"));

services.Configure<JwtOptions>(options => builder.Configuration.GetSection(JwtOptions.SectionKey).Bind(options));
services.Configure<AesOptions>(options => builder.Configuration.GetSection(AesOptions.SectionKey).Bind(options));
services.Configure<SmsOptions>(options => builder.Configuration.GetSection(SmsOptions.SectionKey).Bind(options));
services.Configure<SmtpOptions>(options => builder.Configuration.GetSection(SmtpOptions.SectionKey).Bind(options));

services.AddCors(options => options.AddDefaultPolicy(b => b.SetIsOriginAllowed(_ => true).AllowAnyHeader().AllowAnyMethod().AllowCredentials()));

services.AddHttpsRedirection(options => {
  options.RedirectStatusCode = (int)HttpStatusCode.TemporaryRedirect;
  options.HttpsPort = builder.Environment.IsProduction() ? 443 : 5001;
});

services.AddHsts(options => {
  options.Preload = true;
  options.IncludeSubDomains = true;
  options.MaxAge = TimeSpan.FromDays(60);
});

var app = builder.Build();

if (app.Environment.IsDevelopment()) {
  app.UseSwagger();
  app.UseSwaggerUI();
} else {
  app.UseHsts();

  app.UseExceptionHandler(appBuilder => {
    appBuilder.Run(async context => {
      context.Response.StatusCode = 200;
      var exception = context.Features.Get<IExceptionHandlerFeature>();

      if (exception is not null) {
        Log.Error(exception.Error, "{@Endpoint} {Path}", exception.Endpoint, exception.Path);
        await context.Response.WriteAsync(JsonSerializer.Serialize(Result.Error()));
      }
    });
  });
}


app.InitDatabase(typeof(ApplyInfo), typeof(User), typeof(PhoneVerificationCode));

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication()
   .UseAuthorization();

app.MapPost("/api/login",
    async ([FromServices] ISqlSugarClient db, [FromServices] JwtHelper jwtHelper, [FromServices] AesHelper aesHelper, User user) => {
      var result = await db.Queryable<User>().FirstAsync(u => u.Username == user.Username);

      if (result is not null && user.Password == aesHelper.Decrypt(result.Password)) {
        var token = jwtHelper.CreateToken(result.Username, "admin");
        return new { Code = 200, Message = "登录成功", Token = token };
      } else {
        return new { Code = 400, Message = "密码或用户名错误", Token = "" };
      }
    });

if (app.Environment.IsDevelopment()) {

  app.MapPost("/api/user/{password}",
      ([FromServices] AesHelper aesHelper, string password) => Result.Ok(aesHelper.Encrypt(password)));

  app.MapGet("/{code}",
      async ([FromServices] SmsService notifyService, string code) => {
        try {
          await notifyService.SeedAsync(new SmsSeedParameter {
            Phone = "18075099144",
            ParameterString = $$"""{"code":"{{ code}}  "}""" ,
            SignName = "阿里云短信测试",
            TemplateCode = "SMS_154950909"
          });
        } catch (Exception ex) {
          Log.Error(ex, "消息发送失败");
          throw;
        }

      });

}


app.MapControllers();

app.Run();
