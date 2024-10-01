<h1> In the folder Documents the relevant information like: feseability study, RASD LogiHR, Design Document Requirements, Use Cases, Can be found.

<h1>LogiHR - Human Resource Management System</h1> <h3>Overview</h3> <p>LogiHR is a Human Resource Management System developed using C# in Visual Studio. The system is designed to facilitate the management of employee records, authentication, and warehouse-related operations within a company. 
  
  
  LogiHR includes two key user roles – Admin and Warehouse Manager – each with specific permissions and functions. Admins can handle employee management, while Warehouse Managers oversee the inventory. The system enables efficient employee data management, authentication, and inventory control.</p> <h3>Features</h3> <h6>User Roles:</h6> <p>1. 
  
  Admin: Manages employee records (create, update, delete), sets roles and salary, and has access to the full HR functionalities.</p> <p>2. Warehouse Manager: Manages inventory items, including raw materials and finished products, with full CRUD capabilities.</p> <h3>Key Functionalities:</h3> <h6>Employee Management (Admin):</h6> <p>Admins can add, update, and delete employee information such as name, contact details, salary, and role within the company.</p> <h6>Inventory Management (Warehouse Manager):</h6> <p>Warehouse Managers can create, read, update, and delete inventory items, track quantities, costs, and categories of both raw materials and finished products.</p> <h6>User Authentication:</h6> <p>Login functionality with role-based access control. Admins and Warehouse Managers have separate interfaces.</p> <h3>Database</h3> <p>MS SQL Server is used to store employee and inventory data, ensuring secure and real-time updates to the system.</p> <h3>Usage</h3>. Create a new MS SQL Server database.</p> <p>2. Configure the connection string in the project settings to link with your SQL Server database.</p> <h3>Run the Application:</h3> <p>1. Open the project in Visual Studio.</p> <p>2. Build the solution and run the application.</p> <h3>Dependencies</h3> <p>Ensure you have Visual Studio installed to run and modify the project. The project uses MS SQL Server for database management.</p>
<h3>Map of the site:</h3> 

<h6>/LoginHR</h6> <p>This folder contains the whole code developed in C# with all the forms al libraries required</p>
<h6>/Documents</h6> <p>This folder contains 3 folders, the two deliverable files and the aditiona files required for the project </p>
<h6>/FinalEXE</h6> <p>The executable program is here and the configuration files too </p>

<h3>User Instruction:</h3> 
<p>To execute the program open the file <strong>Config.xml</strong>, in this file put your local mail information, this information is required to send the mails. If it is not correct, the program will still execute, but will not send the mails correctly.</p>
<p>Then open the file <strong>DbConfig.xml</strong> and fill the information with your local Database. Please remember that the database should have the following structure</p>
<img src="./images/DB_Diagram.png" alt="Database Configuration" width="500">
<p>Then double click the .exe file and execute the program.</p>
