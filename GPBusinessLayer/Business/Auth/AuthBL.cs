using GPDataLayer.Data.ViewModels;
using GPUtils;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace GPBusinessLayer.Business.Auth
{
    public class AuthBL
    {
        private CustomLogger logger;

        public AuthBL()
        {
            logger = new CustomLogger();
        }

        public void testLogger()
        {
            logger.Debug("Testing from GP BusinessLayer");
        }

        public dynamic generateJWT()
        {
            logger.Debug("GPBusinessLayer.AuthBL@GenerateJWT");
            try
            {

            string key = ConfigurationManager.AppSettings["secretKey"].ToString();
            var issuer = ConfigurationManager.AppSettings["issuer"].ToString(); //normally this will be your site URL    
           
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            //Create a List of Claims, Keep claims name short    
            var permClaims = new List<Claim>();
            permClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            permClaims.Add(new Claim("valid", "true"));
            permClaims.Add(new Claim("userid", "xyz"));
            permClaims.Add(new Claim("name", "INTUser"));

            //Create Security Token object by giving required parameters    
            var token = new JwtSecurityToken(issuer, //Issure    
                            issuer,  //Audience    
                            permClaims,
                            expires: DateTime.Now.AddMinutes(100),
                            signingCredentials: credentials);
            var jwt_token = new JwtSecurityTokenHandler().WriteToken(token);
                return jwt_token;
            }
            catch(Exception ex)
            {
                logger.Error("GPAdapter.GPBusiness@GenerateJWT Exception Occured while generating JWT Token. ex:"+ex.ToString());
            }

            return null;
        }

        public bool verifyCredentials(AuthDAO authPayload)
        {
            logger.Debug("GPAdapter.GPBusiness@verifyCredentials called");
            try
            {

                if (authPayload.username.Equals(ConfigurationManager.AppSettings["authUser"].ToString()) && authPayload.password.Equals(ConfigurationManager.AppSettings["authKey"].ToString())) 
                {
                    return true;
                }
             

            }catch(Exception ex)
            {
                logger.Error("GPAdapter.GPBusiness@verifyCredentials Exception Occured while verifyingCredentials ex:" + ex.ToString());
            }
            return false;
        }
    
    }
}
