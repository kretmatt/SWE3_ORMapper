# SWE3_ORMapper
The project SWE3_ORMapper is an OR-Mapping-Framework for C# and PostgreSQL.

## What was used?
The framework is specifically designed for PostgreSQL and was implemented in C#. The Npgsql nuget package is used to connect to the database. Additionally, Moq was used for some unit tests.

## What are the additional features?
Aside from the required features, a transaction and table creation feature was implemented.

## Projects
The entire project consists of three separate projects:
- _ORMapper_Framework_ - The actual framework that provides the functionality. Internals are visible to the test project. 
- _ORMapper_Framework.Tests_ - This sub project is a test application and contains some unit tests for the functionality of the ORMapper framework.
- _ORMapperDemo_ - Just a demo application that demonstrates the functionality of the framework. In order for it to work, the database connection string needs to be inserted into Program.cs. The tables will be created by the program. 

## How does the framework work / how can it be used?

### Defining entities
In order to make new entity classes the following points must be followed:
- Inherit from AEntity
- Mark objects / lists as foreign keys (FKAttribute)
- Mark primary key with PKAttribute
- Optionally use Unique / Ignore Attributes
- Define additional metadata through Column Attributes

Afterwards the entity classes can be registered through ORMapper.RegisterNewEntity methods. The ORMapper will automatically detect other entity classes through relations and through the inheritance. IMPORTANT: It will only look up classes further up the inheritance hierarchy. In order to completely capture an entire inheritance hierarchy one must register all classes on the bottom

### Using the framework

The ORMapper provides the following methods in general:
- _RegisterNewEntity_: Registers a class as an entity and automatically detects other classes through properties / inheritance.
- _EnsureDeleted_: Drops the tables of the registered entity definitions.
- _EnsureCreated_: Creates tables for the registered entity definitions.
- _Read_:  Retrieves data for a specific type with the specified primary key. Throws an exception if the object can't be found in the database.
- _Save_: Inserts an object into the database or updates it if there are changes. If the object is part of an inheritance hierarchy, data gets inserted into or updated in all affected tables. 
- _Delete_: Deletes an object in the database. 
- _Transaction_: The framework also provides transaction functionality.
	-  _StartTransaction_: Starts an PostgreSQL transaction.
	-  _CommitTransaction_: Commits all commands issued during the transaction to the database and releases locks.
	-  _RollbackTransaction_: Rolls back all changes made during the transaction and releases locks.
- _ClearCache_: Clears the cache of the ORMapper class. Useful for reloading data from the database.
- _LockObject_: Locks an object in the database on ROW level. Useful for transactions. The concrete lock mode (FOR UPDATE, FOR NO KEY UPDATE, FOR SHARE, FOR KEY SHARE) can also be specified.

Additionally, ORMapper has two public properties:
- _Database_: Connection to the database
- _InTransaction_: Represents whether ORMapper currently is in a transaction. Useful for rolling back a transaction if an exception occurs.

In order to work with the database, the connection must be opened. If tables still need to be created, entity classes also need to be registered. If there are no further commands for the database, the connection can be closed.

Here is a possible workflow:
1. Set the ORMapper database connection
2. Register entity classes
3. Open the connection
4. (Drop and) create the database table structure
5. Start transaction
6. Save / Update / Delete an object
7. Commit transaction (Rollback if there is an exception)
8. Close the connection

### Querying

In order to retrieve an object of a specific type with a specific primary key, the Read method of the ORMapper class can be used. Alternatively, there are also queries that can be used for retrieving data from the database.

Firstly, a new Query object with an entity class type needs to be created. In order to retrieve all objects of a specific type, this base query can be used. Otherwise further query options can be defined after using the Where() extension method.

Here are the possible query extension methods (most of the extension methods also have an optional boolean parameter, that inverses the operator):
- Equals
- NotEquals
- GreaterThan
- LessThan
- Where
- And 
- Or
- Not
- BeginSet, EndSet -> ()
- IsNull
- In
- Between
- Like

After the query is built, the results can be retrieved throught the Execute method. Execute returns a list of objects, that fulfilled the conditions of the query.







