# Book Store | ASP.NET Core
This project is a simple store app that sells books to businesses and private customers.
The store allows customers to buy books in bulk to receive a discount, or as a single items at full price.
Customers Need to be registered before placing orders and purchasing books.


</br>
</br>

# Details

This project was made following a course on ASP.NET and emphasizes functionality over looks. This was a good practice project and a nice introduction into Razor pages which was something i had neglected. The project it self follows the MVC pattern but with a healthy dose of Razor pages.<br/>
There is a special repository pattern which allows the use of alternative ORMs like Dapper for example. For such a small project it might seem like a little to much, but this is a project made with learning in mind.

</br>

## Prerequisites
- .NET Core 3.1 +
- Entity framework Core
- MS SQL database

## Role based Management system and security.
  - Admin
    -  Total control (Add some employees and users to test out the) site .
    -  Create customer, employees, other Admins
    -  Lock Users
    -  Add Products(books), Product details etc
    -  
  - Employee
    - Edit Products
    - Can process, cancel and ship orders.
    - Add Businesses / Companies and verify them for additional benefits. 
    - Read customer information.
  - Users
    - Can edit their own information
    - Shop for books

<br/>

## Login And Registration

- Registration dose not use E mail confirmation, so you can supply anything you want, as long as it is formatted as a E-mail address. eks -> Test@Test.com.<br/> The Reason for this is simplicity for people to register and test the application and i don't want to involve active E-mail accounts and 3rd party email providers. I don't want any personal data to be supplied.

<br/>

- External authentication for google users is supported but disabled. If you wish to test this you can uncomment the code in Startup.cs and Appsettings.json. You will also need to provide the necessary google keys to.

<br/>

- A Admin account is crated for you. The admin information is found in DataBaseInitializer.cs in the Data Access project.
  - Email/User Name : "Admin@Admin.com
  - Password: Admin_123

</br>
</br>

## Install
Download the project and run it(ctrl+f5 in VS2019) in your editor of choice.<br/>
This project is for display and testing only and **will generate a SQL database on your computer to work. run this project at your own risk** .
</br>
This solution when Run for the first time will apply all the migrations and generate a SQL database to work with. **The connection string is appsettings.json. By default the a database named BookStoreMVC will be crated to a local sql database server**.
