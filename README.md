/**** HOW TO USE ***/

Run Image Search.exe
Windows defender will prompt as there is no SSL certification with this exe.
It may request downloading .NET 8 to run it, Microsoft should be able to prompt for that.
Press "Import Images"
Select a csv in the format of the sample provided
You can now look at each entry and be able to add/remove tags per image
The Search Criteria works where you can press "Add Criteria" to add a search parameter
A search parameter is a property, operator and value.
Once you have all the criteria you want, press "Search"

Geolocation Search uses the Center metadata on the image to determine if they are within the polygon you create by adding points.  A valid polygon requires 3 points.  I will admit the function was pulled from stackoverflow and I am not confident in its validity considering my lack of exposure to geolocation logic.


/**** DEV APPROACH ***/

I chose to create a WPF app with MVVM architecture.
Created simple views
Created a few services for operations such as importing and geo location.


/*** CONSIDERATIONS IN A COMMERCIAL ENVIRONMENT ***/
I have written inline comments for some considerations but in general:

1. Allow for importing from different sources.
2. Build upon imports instead of clearing every time a user imports.
3. Write Unit Tests to confirm logic.  I would have saved an hour on the Search functionality when considering Tags if I had Unit Tests written.
4. Consider ORM integration to a database and query on a component that is designed to work with querying large datasets using IQueryable.
5. Implement DI for the services instead of creating a new instance.
6. Error Handling and User Feedback on errors.
    6a. An invalid row should not break the import. It should be ignored and reported on (7)
7. Logging mechanism to show the user what they imported and what may have failed.
8. Simple items like no string literals.
9. A better UI with maybe a subscription to a control service. 
10. Dark mode.

