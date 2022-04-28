using Core2.Interfaces;
using Core2.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Core2.Repository
{
    public class LoginRegister : ILoginRegister
    {
        private readonly UserManager<ApplicationsUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public LoginRegister(
            UserManager<ApplicationsUser> userManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        //LOGIN
        public async Task<ClientValidation> LoginUserAsync(Login login)
        {
            //CHECK USER LOGIN DETAILS PRESENT
            if (login == null)
                throw new NullReferenceException("Please provide login credentials");


            //CHECK USER EXISTS
            var userExists = await _userManager.FindByEmailAsync(login.Email);

            if (userExists == null)
                return new ClientValidation
                {
                    Message = "User does not exists, please use correct credentials",
                    IsError = true
                };


            //CHECK LOGIN DETAILS CORRECT
            var checkUserPass = await _userManager.CheckPasswordAsync(userExists, login.Password);

            if (!checkUserPass)
                return new ClientValidation
                {
                    Message = "Provide correct login credentials",
                    IsError = true
                };

            //CREATE TOKEN
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Thisissigningkey"));

            //get user claims
            var claims = await GetUserClaims(userExists);

            var token = new JwtSecurityToken
                (
                  issuer: "https://localhost:44346/",
                  audience: "Users",
                  claims:claims,
                  expires:DateTime.Now.AddDays(2),
                  signingCredentials:new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                );

            //TOKEN TO STRING
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return new ClientValidation
            {
                Message = tokenString,
                IsError = false,
                ExpireDate = token.ValidTo
            };

        }


        //GET CLAIMS TO A APPLICATION USER AND ASSIGNED ROLE(S)
        private async Task<List<Claim>> GetUserClaims(ApplicationsUser user)
        {
            var _options = new IdentityOptions();

            var claims = new List<Claim>
            {
                //ClaimTypes.Email ClaimTypes.NameIdentifier
                new Claim("Email", user.Email),
                new Claim("Id", user.Id)
            };

            var userClaims = await _userManager.GetClaimsAsync(user);
            if (userClaims != null)
                claims.AddRange(userClaims);

            //CHECK ROLES USER IN AND GET CLAIMS
            var userRole = await _userManager.GetRolesAsync(user);

            //LOOP THROUGH THE ROLE GETTING THEIR CLAIM
            foreach(var role in userRole)
            {
                var identityRole = await _roleManager.FindByNameAsync(role);
                var roleClaim = await _roleManager.GetClaimsAsync(identityRole);

                //ClaimTypes.Role
                claims.Add(new Claim("Role", role));
                foreach(var claim in roleClaim)
                {
                    claims.Add(claim);
                }
            }

            return claims;
        }

        //REGISTER
        public async Task<ClientValidation> RegisterUserAsync(Register register)
        {
            //CHECK USER REGISTRATION DETAILS PRESENT
            if (register == null)
                throw new NullReferenceException("Provide details for registration");

            
            //CHECK WHETHER USER DEFINED ALREADY
            var userExists = await _userManager.FindByEmailAsync(register.Email);
            if (userExists != null)
                return new ClientValidation
                {
                    Message = $"{register.Email} already exists, use another Email",
                    IsError = true
                };

            //CHECK IF PASSWORDS MATCH
            if (register.Password != register.ConfirmPassword)
                return new ClientValidation
                {
                    Message = "Passwords do not match",
                    IsError = true
                };


            //CREATE APP USER IN DB
            var user = new ApplicationsUser
            {
                Email = register.Email,
                FirstName = register.FirstName,
                LastName = register.LastName,
                PhoneNumber = register.Phone,
                UserName = register.Email
            };

            var createUser = await _userManager.CreateAsync(user, register.Password);

            var assignToRole = await _userManager.AddToRoleAsync(user, "Developer");


            if (!createUser.Succeeded)
            {
                return new ClientValidation
                {
                    Message = "Failed to create user",
                    IsError = true
                };
            }

            return new ClientValidation
            {
                Message = $"{register.Email} created successfully",
                IsError = false
            };

        }
    }
}
