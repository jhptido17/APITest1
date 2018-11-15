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
      * api/customers/id
    * POST
      * api/customers
        * FromBody Post you need add json with next fields: {"Name":"TestCustomer", "Surname":"Test2Customer"}
    * PUT
      * api/customers/id
        * FromBody Put you need add json with next fields: {"Name":"TestCustomer", "Surname":"Test2Customer"}
    * Delete
      * api/customers/id
      
  * CustomersImageController
  * UsersController
  * LoginController
