using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Http;

namespace Affinity.Models
{
    public class Utils
    {
        public static async Task<bool> CheckFirebaseToken(IHttpContextAccessor httpContextAccessor)
        {
            string idToken = httpContextAccessor.HttpContext.Request.Cookies["aff_t"];

            if(string.IsNullOrWhiteSpace(idToken))
            {
                return false;
            }

            try
            {
                FirebaseToken decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);
                return true;
            }
            catch (FirebaseAuthException)
            {
                return false;
            }           
        }
        public static async Task<FirebaseToken> GetUserFirebaseToken(IHttpContextAccessor httpContextAccessor)
        {
            string idToken = httpContextAccessor.HttpContext.Request.Cookies["aff_t"];

            if (string.IsNullOrWhiteSpace(idToken))
            {
                return null;
            }

            try
            {
                FirebaseToken decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);
                return decodedToken;
            }
            catch (FirebaseAuthException)
            {
                return null;
            }
        }

    }
}
