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

            if(idToken == null)
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
    }
}
