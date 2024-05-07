# Laptop Demo
Product Management system using c# windowform and mssql
## Features
 - Track keeping records
 - Product list management
 - Stock entry management
 - Supplier info management
 - POS records
 - Account user management
 - Cashier system

![Screenshot 2024-05-07 163608](https://github.com/hieru2604/Product-management-System-windowform/assets/88078435/d802455a-54d1-43dc-a21c-586a1b260218)

![Screenshot 2024-05-07 162041](https://github.com/hieru2604/Product-management-System-windowform/assets/88078435/5a44ddec-f00b-49e8-8017-f0b745737567)

![Screenshot 2024-05-07 162311](https://github.com/hieru2604/Product-management-System-windowform/assets/88078435/0eb36887-3308-4c8d-905b-a0a85c935ab8)

![Screenshot 2024-05-07 163218](https://github.com/hieru2604/Product-management-System-windowform/assets/88078435/a5541de4-ec4e-4c70-93e8-33c84a128799)

![Screenshot 2024-05-07 163249](https://github.com/hieru2604/Product-management-System-windowform/assets/88078435/93ad0f8e-406b-4fd6-9607-bee54a5380e5)

![Screenshot 2024-05-07 163018](https://github.com/hieru2604/Product-management-System-windowform/assets/88078435/d3296d9c-3258-496b-ae68-5f8b5f15a3fc)

## Installation
Open sql server management studio, select database engine, right click database and select import Data-tier Application.
import using the database.bacpac file included in the database folder

![Screenshot 2024-05-07 165933](https://github.com/hieru2604/Product-management-System-windowform/assets/88078435/44ed8537-c503-4b2b-8f18-11fb43ada27a)

Open the project in visual studio 2019, select server explorer in the top left corner
Right click on data connections and select add connection..
Input the server name you stored your database in, select the database 
Open the advanced and copy the entire string at the bottom of the panel

![image](https://github.com/hieru2604/Product-management-System-windowform/assets/88078435/a8677fa2-0d08-4cec-824b-bdbd62ede7d7)

Once the database is connected and test connection is success,
Open the DBConnect.cs folder and replace the connection string
```python
 public string myConnection()
        {   //besure to change the line below to the database that you connected to
            con = @"Data Source=(local);Initial Catalog=PMSDB;Integrated Security=True";
            return con;
        }
```
