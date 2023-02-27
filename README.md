# MovieApp
https://www.youtube.com/watch?v=aj9UKQzHCrU&ab_channel=Mert%C5%9Een%C3%B6zl%C3%BCler

For use:

1) Connect to the database

2) Open the admin creation section in the comment line in the UserAuthenticationController and create the admin user.

3) Post category and subcategory first, then movies.

Note: I used Mailtrap for e-mail section. In appsettings.json, i added this part:

"AppName": "Movie App",
  "Application": {
    "LoginPath": "UserAuthentication/login",
    "AppDomain": "https://localhost:7268/",
    "EmailConfirmation": "confirm-email?uid={0}&token={1}",
    "ForgotPassword": "reset-password?uid={0}&token={1}"
  },
  
  
  "SMTPConfig": {
    "SenderAddress": ".....",
    "SenderDisplayName": "Movie App",
    "UserName": "......",
    "Password": "........",
    "Host": "sandbox.smtp.mailtrap.io",
    "Port": 465,
    "EnableSSL": true,
    "UseDefaultCredentials": false,
    "IsBodyHtml": true
  }
 
 For using e-mail part, you should arrange this part according to your e-mail service.
  
Note2: for using profil photo in Admin, the admin profile picture named admin.jpg should be uploaded. The folder named default.jpg should be loaded before users can log in. Profile photo is not mandatory for Users and a default profile photo should be assigned. When it is created with the code in the comment line with the Admin code first approach, it automatically searches for the default.jpg profile photo.

Note3: I will update my project when I finish the sections of Purchase with credit card.

FEATURES
1) Users have ads on the pages right and left, they also have to see 10 seconds of ads before watching a movie. Only superUser and Admin users will not see these ads.
2) Users can load a minimum balance of $10.
3) PremiumUser can buy a monthly subscription for $30 and SuperUser for $50. The scheduler is defined in Repositories/TimingService.cs. For testing purposes, the expration date value was set to 2 minutes for Premium or SuperUser purchases. You can set the time you want.
4) Ajax was used in terms of performance for the operations to be performed according to the roles of the User.
5) $2 is required to purchase movies and $1.5 to purchase from balance.
6) Admin can deactivate or reactivate users. If a deactivated user wants to log in, it says your account is not active, please contact the administrator.
7) Search button works on every page.
8) Users can register with Profile Picture.
9) For long comments, the view more button was placed after a certain number of characters.
10) Only members can comment.
11) Admin can delete any comment. Other users can only delete their own comments.
12) Movies are tied to categories and subcategories. These parts work dynamically in the navbar.
13) Random sorting was done according to some categories on the home page.
14) The movies are set as a free video from youtube.
15) Required pages have been paginated.
16) A simple admin page and user page has been made
17) E-mail confirmation and forgot password section is active. 

