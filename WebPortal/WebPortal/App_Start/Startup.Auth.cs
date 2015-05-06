// AppId = "1549521788629862",
//AppSecret = "3072d557ae33bd64013e58ed3dc44006",
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.DataProtection;
using Owin;
using System;
using WebPortal.Models;
using Microsoft.Owin.Security.Facebook;
using System.Collections.Generic;

namespace WebPortal
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the UserManager
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(20),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager)),
                    // EJ - I added this line here as a workaround I found here:
                    // https://katanaproject.codeplex.com/discussions/565294
                    // Was having some weird null reference problem relating to cookies or something. Not sure what this workaround does
                    // But it seems to be working
                    OnException = context => { }
                }
            });
            // Use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");
            
            var x = new FacebookAuthenticationOptions();
            x.Scope.Add("email");
            //x.Scope.Add("friends_about_me");
            //x.Scope.Add("friends_photos");
            x.Scope.Add("read_stream");
            x.Scope.Add("publish_actions");
            x.AppId = "1549521788629862";
            x.AppSecret = "3072d557ae33bd64013e58ed3dc44006";
            x.Provider = new FacebookAuthenticationProvider()
           {
               OnAuthenticated = async context =>
               {
                   //Get the access token from FB and store it in the database and
                   //use FacebookC# SDK to get more information about the user
                   context.Identity.AddClaim(
                   new System.Security.Claims.Claim("FacebookAccessToken",
                                                        context.AccessToken));
               }
           };
            x.SignInAsAuthenticationType = DefaultAuthenticationTypes.ExternalCookie;
            
            
            app.UseFacebookAuthentication(x);

            //app.UseGoogleAuthentication();
        }
    }
}