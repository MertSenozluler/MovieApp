# MovieApp

For use:

1) Connect to the database

2) Open the admin creation section in the comment line in the UserAuthenticationController and create the admin user.

3) Install category and subcategory first, then movies.

Note: Before creating admin, admin profile picture named admin.jpg, folders named default.jpg should be created before users login. Profile photo is not mandatory for Users and a default profile photo should be assigned. When it is created with the code in the comment line with the Admin code first approach, it automatically searches for the default.jpg profile photo.

Note2: I will update my project when I finish the sections such as UserPage, EmailConfirmation, Purchase with credit card.

FEATURES
Search button works on every page.
Users can register with Profile Picture.
For long comments, the view more button was placed after a certain number of characters.
Only members can comment.
Admin can delete any comment. Other users can only delete their own comments.
Movies are tied to categories and subcategories. These parts work dynamically in the navbar.
Random sorting was done according to some categories on the home page.
The movies are set as a royalty-free video from youtube.
Required pages have been paginated.
Users have ads on the pages right and left, they also have to see 10 seconds of ads before watching a movie. Only superUser and Admin users will not see these ads.
Users can load a minimum balance of $10.
PremiumUser can buy a monthly subscription for $30 and SuperUser for $50. The scheduler is defined in Repositories/TimingService.cs. For testing purposes, the expration date value was set to 2 minutes for Premium or SuperUser purchases. You can set the time you want.
Ajax was used in terms of performance for the operations to be performed according to the roles of the User.
$2 is required to purchase movies and $1.5 to purchase from balance.
Admin can deactivate or reactivate users. If a deactivated user wants to log in, it says your account is not active, please contact the administrator.
