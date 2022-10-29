﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using CksysRecruitNew.Server.Options;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CksysRecruitNew.Server.Helper;

public class JwtHelper {
  private readonly JwtOptions _options;

  public JwtHelper(IOptions<JwtOptions> options) {
    _options = options.Value;
  }

  public string CreateToken(string username) {
    // 1. 定义需要使用到的Claims
    var claims = new[] {
      new Claim("Username", username),
    };

    // 2. 从 appsettings.json 中读取SecretKey
    var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));

    // 3. 选择加密算法
    var algorithm = SecurityAlgorithms.HmacSha256;

    // 4. 生成Credentials
    var signingCredentials = new SigningCredentials(secretKey, algorithm);

    // 5. 根据以上，生成token
    var jwtSecurityToken = new JwtSecurityToken(
        _options.Issuer,   //Issuer
        _options.Audience, //Audience
        claims,                         //Claims,
        DateTime.Now,                   //notBefore
        DateTime.Now.AddMinutes(30),    //expires
        signingCredentials              //Credentials
    );

    // 6. 将token变为string
    var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

    return token;
  }
}
