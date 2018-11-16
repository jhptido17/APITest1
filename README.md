# APITest1
## TheCRMService
### DB Needed
For make work this project you need a data base with two tables:
* Users
  * Id
    * int with identity, is key, not null
  * Username
    * varchar(50), not null
  * Password
    * varchar(50), not null
  * Role
    * vachar(5), not null
  * Status
    * int, this field is not used for now
    
* Customers
  * Id
    * int with identity, is key, not null
  * Name
    * varchar(50), not null
  * Surname
    * varchar(50), not null
  * Image
    * varchar(100)
  * CreatedBy
    * varchar(50), not null
  * UpdateBy
    * varchar(50)
    
### Pages
The Pages are made with razor, the purpose of the pages is to make an administrative interface, 
for test the APIControllers and its security with basic authentication. The security is in the API,
the pages are exempt from own security, only use sessions to remember the authentication data that
will be sent to the API with a HttpClient.

### Structure of Get, Post, Put and Delete of each API Controller
You can access to Controller with the next http path: api/[controllername]
  * CustomersController
    * GET
      * api/customers
        * Get a list of all customers
      * api/customers/id
        * Get one customer data found by id
    * POST
      * api/customers
        * FromBody Post, you need add json with the next fields: {"Name":"TestCustomer", "Surname":"Test2Customer"}
    * PUT
      * api/customers/id
        * FromBody Put, you need add json with the next fields: {"Name":"TestCustomer", "Surname":"Test2Customer"}
    * Delete
      * api/customers/id
        * Delete a customer by id
      
  * CustomersImageController
    * GET
      * api/customersimage/id
        * Get image path of wwwroot, this path with the root page http path allow you to get the image directly
    * POST
      * api/customersimage/id
        * Post image from IForm using multipart, the image is stored in wwwroot in UserImages directory,
        * in database is searched by the customer and the path is stored
    * Delete
      * api/customersimage/id
        * Search customer by id and remove the image path from database, and after remove the image from wwwroot/UserImages
    
  * UsersController
    * GET
      * api/users
        * Get a list of all users
      * api/users/id
        * Get one user data found by id
    * POST
      * api/users
        * FromBody Post, you need add json with the next fields: {"Username":"TestUser", "Password":"TestPassword",        "Role":"admin or user if you post other, works but you won't access to API"}
    * PUT
      * api/users/id
    * Delete
      * api/users/id
        * Delete a user by id
    
  * LoginController
    * POST
      * api/login
        * FromBody Post, you need add json with the next fields: {"Usename":"TestUser", "Password":"TestPassword"}
        * Only return to you Ok(200) if the user with the Username and Password is in the data base
