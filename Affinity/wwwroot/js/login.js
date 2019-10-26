
(function(){
    // Initialize the FirebaseUI Widget using Firebase.
    var ui = new firebaseui.auth.AuthUI(firebase.auth());
    var uiConfig = {
        callbacks: {
            signInSuccessWithAuthResult: function (authResult, redirectUrl) {
                // User successfully signed in.
                // Return type determines whether we continue the redirect automatically
                // or whether we leave that to developer to handle.
                //If this is a new user && his provider is password (username/password registeration) && his email is not verified,
                if (authResult.additionalUserInfo.isNewUser && authResult.additionalUserInfo.providerId == 'password' && !authResult.user.emailVerified) {
                    // To apply the default browser preference instead of explicitly setting it.
                    // firebase.auth().useDeviceLanguage();

                    //Send him the verification email, show him a toastr message, then apply a force logout
                    authResult.user.sendEmailVerification();
                    firebase.auth().signOut();
                    alert("Account created successfully. Please verify your email address.");
                }

                //If this is not a new user && his provider is password (username/password registeration) && his email is not verified.
                //On the other hand, the UI shows the user a button to resend the verification email.
                else if (!authResult.additionalUserInfo.isNewUser && authResult.additionalUserInfo.providerId == 'password' && !authResult.user.emailVerified) {
                    // To apply the default browser preference instead of explicitly setting it.
                    // firebase.auth().useDeviceLanguage();

                    //Show him a toaster message that his email is not verified, then apply a force logout.
                    firebase.auth().signOut();
                    const shallSendVerficationEmail = confirm("Your email address is not verified. Would you like to resend verification email?")
                    if (shallSendVerficationEmail)
                        authResult.user.sendEmailVerification();
                }
                //Otherwise an old user login normally.
                else
                    return true;    
          },
          uiShown: function() {
            // The widget is rendered.
            // Hide the loader.
            document.getElementById('loader').style.display = 'none';
          }
        },
        // Will use popup for IDP Providers sign-in flow instead of the default, redirect.
        signInFlow: 'popup',
        signInSuccessUrl: '/',
        signInOptions: [
          // Leave the lines as is for the providers you want to offer your users.
          //firebase.auth.GoogleAuthProvider.PROVIDER_ID,
          //firebase.auth.FacebookAuthProvider.PROVIDER_ID,
          //firebase.auth.TwitterAuthProvider.PROVIDER_ID,
          //firebase.auth.GithubAuthProvider.PROVIDER_ID,
          //firebase.auth.PhoneAuthProvider.PROVIDER_ID,
          firebase.auth.EmailAuthProvider.PROVIDER_ID
        ],
        // Terms of service url.
        tosUrl: '/',
        // Privacy policy url.
        privacyPolicyUrl: '/'
        };

        // The start method will wait until the DOM is loaded.
        ui.start('#firebaseui-auth-container', uiConfig);
})()